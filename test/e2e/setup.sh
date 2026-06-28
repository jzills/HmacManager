#!/usr/bin/env bash
set -euo pipefail

ISTIO_VERSION="${ISTIO_VERSION:-1.30.2}"
GATEWAY_API_VERSION="${GATEWAY_API_VERSION:-v1.2.0}"
CLUSTER_NAME="${CLUSTER_NAME:-hmac-manager-e2e}"
IMAGE_TAG="${IMAGE_TAG:-e2e-test}"
NAMESPACE="${NAMESPACE:-hmac-system}"
TEST_PUBLIC_KEY="${TEST_PUBLIC_KEY:-00000000-0000-0000-0000-000000000001}"
TEST_PRIVATE_KEY="${TEST_PRIVATE_KEY:-euJ9iXZfURClr78ERyZr8csmHoVkZIfeoSVu3jNZf0w=}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

log() { echo "[setup] $*"; }

# ── 1. Cluster ────────────────────────────────────────────────────────────────
log "Creating kind cluster '$CLUSTER_NAME'..."
if kind get clusters 2>/dev/null | grep -q "^${CLUSTER_NAME}$"; then
    log "Cluster already exists, skipping creation."
else
    kind create cluster --name "$CLUSTER_NAME" --wait 60s
fi
kubectl config use-context "kind-${CLUSTER_NAME}"

# ── 2. Gateway API CRDs ───────────────────────────────────────────────────────
log "Installing Gateway API CRDs ($GATEWAY_API_VERSION)..."
if [[ -n "${GATEWAY_API_CRD_FILE:-}" && -f "$GATEWAY_API_CRD_FILE" ]]; then
    kubectl apply -f "$GATEWAY_API_CRD_FILE"
else
    kubectl apply -f "https://github.com/kubernetes-sigs/gateway-api/releases/download/${GATEWAY_API_VERSION}/standard-install.yaml"
fi

# ── 3. Istio ──────────────────────────────────────────────────────────────────
log "Installing istioctl $ISTIO_VERSION..."
if ! command -v istioctl &>/dev/null || [[ "$(istioctl version --remote=false 2>/dev/null)" != *"$ISTIO_VERSION"* ]]; then
    curl -sSL https://istio.io/downloadIstio | ISTIO_VERSION="$ISTIO_VERSION" TARGET_ARCH=x86_64 sh -
    export PATH="$PWD/istio-${ISTIO_VERSION}/bin:$PATH"
fi

log "Installing Istio with ambient profile..."
istioctl install --set profile=ambient --skip-confirmation
kubectl rollout status deployment/istiod -n istio-system --timeout=120s
kubectl rollout status daemonset/ztunnel -n istio-system --timeout=120s

# ── 4. Namespace setup ────────────────────────────────────────────────────────
log "Labeling default namespace for ambient mode..."
kubectl label namespace default \
    istio.io/dataplane-mode=ambient \
    istio.io/use-waypoint=waypoint \
    --overwrite

# ── 5. Waypoint ───────────────────────────────────────────────────────────────
log "Creating waypoint proxy..."
istioctl waypoint apply --enroll-namespace -n default --wait

# ── 6. Test workloads ─────────────────────────────────────────────────────────
log "Deploying echo and curl pods..."
kubectl apply -f "$SCRIPT_DIR/manifests/echo.yaml"
kubectl apply -f "$SCRIPT_DIR/manifests/curl.yaml"
kubectl wait --for=condition=Ready pod/echo pod/curl -n default --timeout=60s

# ── 7. Docker image ───────────────────────────────────────────────────────────
log "Building hmac-manager image ($IMAGE_TAG)..."
docker build -f "$REPO_ROOT/kubernetes/service/Dockerfile" \
    -t "hmac-manager:${IMAGE_TAG}" \
    "$REPO_ROOT"

log "Loading image into kind..."
kind load docker-image "hmac-manager:${IMAGE_TAG}" --name "$CLUSTER_NAME"

# ── 8. Helm install ───────────────────────────────────────────────────────────
log "Installing hmac-manager Helm chart..."
helm upgrade --install hmac-manager "$REPO_ROOT/kubernetes/chart" \
    --namespace "$NAMESPACE" \
    --create-namespace \
    --set image.tag="$IMAGE_TAG" \
    --set image.pullPolicy=Never \
    --set config.ASPNETCORE_ENVIRONMENT=Development \
    --set "config.HmacManager__0__Keys__PublicKey=$TEST_PUBLIC_KEY" \
    --set "secretData.HmacManager__0__Keys__PrivateKey=$TEST_PRIVATE_KEY" \
    --set istio.ingressGateway.enabled=false \
    --set istio.waypoint.enabled=true \
    --set istio.waypoint.authorizationPolicy.enabled=true \
    --set istio.waypoint.name=waypoint \
    --set istio.waypoint.namespace=default \
    --wait --timeout=120s

# ── 9. MeshConfig: register ext-authz provider ───────────────────────────────
log "Patching Istio MeshConfig with ext-authz provider..."
PATCH=$(python3 - <<'EOF'
import subprocess, yaml, json, sys

result = subprocess.run(
    ['kubectl', 'get', 'configmap', 'istio', '-n', 'istio-system', '-o', 'jsonpath={.data.mesh}'],
    capture_output=True, text=True
)
mesh = yaml.safe_load(result.stdout) or {}
providers = [p for p in mesh.get('extensionProviders', []) if p.get('name') != 'hmac-manager']
providers.append({
    'name': 'hmac-manager',
    'envoyExtAuthzHttp': {
        'service': 'hmac-manager.hmac-system.svc.cluster.local',
        'port': 8080,
        'includeRequestHeadersInCheck': [
            'authorization', 'hmac-policy', 'hmac-nonce', 'hmac-daterequested',
        ],
        'withRequestBody': {'maxRequestBytes': 8192, 'allowPartialMessage': False}
    }
})
mesh['extensionProviders'] = providers
print(json.dumps({'data': {'mesh': yaml.dump(mesh)}}))
EOF
)
kubectl patch configmap istio -n istio-system --type merge -p "$PATCH"

log "Restarting istiod to pick up MeshConfig changes..."
kubectl rollout restart deployment/istiod -n istio-system
kubectl rollout status deployment/istiod -n istio-system --timeout=120s

log "Setup complete."
