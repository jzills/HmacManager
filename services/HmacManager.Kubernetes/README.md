# HmacManager.Kubernetes

A containerized ASP.NET Core minimal API that acts as a centralized HMAC verification service for Kubernetes clusters running [Istio](https://istio.io/) in **ambient mode**.

The Istio waypoint proxy calls `POST /check` before forwarding any inbound request. If HMAC verification passes, the request is forwarded to the upstream service. If it fails, the waypoint rejects it with `403 Forbidden` — no request ever reaches your workloads with an invalid or missing signature.

```
Client → ztunnel (mTLS, L4) → Waypoint proxy (L7)
                                    ↓ POST /check
                                HmacManager.Kubernetes
                                    ↓ VerifyAsync
                                200 OK → forward upstream
                                403    → reject
```

## Prerequisites

| Tool | Purpose |
|------|---------|
| [Docker](https://docs.docker.com/get-docker/) | Build the container image |
| [kubectl](https://kubernetes.io/docs/tasks/tools/) | Apply Kubernetes manifests |
| [kind](https://kind.sigs.k8s.io/docs/user/quick-start/) or [k3d](https://k3d.io/stable/) | Local Kubernetes cluster |
| [istioctl](https://istio.io/latest/docs/setup/install/istioctl/) | Install Istio and manage waypoints |
| [.NET SDK 8+](https://dotnet.microsoft.com/download) | Build the project locally (optional) |

## Step 1 — Create a local cluster

**kind:**
```bash
kind create cluster --name hmac-test
```

**k3d:**
```bash
k3d cluster create hmac-test
```

## Step 2 — Install Istio in ambient mode

```bash
istioctl install --set profile=ambient --skip-confirmation
```

Verify the install:
```bash
kubectl get pods -n istio-system
# ztunnel and istiod pods should be Running
```

## Step 3 — Build the Docker image

Run from the **repository root** (the Dockerfile copies files relative to the root):

```bash
docker build -f services/HmacManager.Kubernetes/Dockerfile -t hmac-manager:latest .
```

Load the image into your local cluster:

**kind:**
```bash
kind load docker-image hmac-manager:latest --name hmac-test
```

**k3d:**
```bash
k3d image import hmac-manager:latest --cluster hmac-test
```

## Step 4 — Configure your HMAC policies

The service loads policies from `appsettings.json` or environment variables. For local testing the simplest approach is a ConfigMap for non-secret values and a Secret for the private key.

Create the `hmac-system` namespace (the service runs here, **outside** the protected waypoint namespace to avoid a circular ext-authz loop):

```bash
kubectl create namespace hmac-system
```

Create a Secret with your private key:

```bash
kubectl create secret generic hmac-manager-secrets \
  --namespace hmac-system \
  --from-literal=HmacManager__0__Keys__PrivateKey=your-private-key-here
```

Create a ConfigMap with the rest of the policy config:

```bash
kubectl create configmap hmac-manager-config \
  --namespace hmac-system \
  --from-literal=HmacManager__0__Name=MyPolicy \
  --from-literal=HmacManager__0__Keys__PublicKey=00000000-0000-0000-0000-000000000001 \
  --from-literal=HmacManager__0__Algorithms__ContentHashAlgorithm=SHA256 \
  --from-literal=HmacManager__0__Algorithms__SigningHashAlgorithm=HMACSHA256 \
  --from-literal=HmacManager__0__Nonce__MaxAgeInSeconds=30 \
  --from-literal=HmacManager__0__Nonce__CacheType=Memory
```

> The `HmacManager__0__` prefix is .NET's environment variable binding convention for the first element of the `HmacManager` JSON array. Add `HmacManager__1__...` for a second policy, and so on.

## Step 5 — Deploy the service

For local testing, enable the `Development` environment so the `/sign` endpoint is available (see [Signing endpoint](#signing-endpoint-development-only)):

```bash
kubectl set env deployment/hmac-manager ASPNETCORE_ENVIRONMENT=Development -n hmac-system
```

Apply the manifests:

```bash
kubectl apply -f services/HmacManager.Kubernetes/deploy/deployment.yaml
kubectl apply -f services/HmacManager.Kubernetes/deploy/service.yaml
```

Wait for the pod to be ready:

```bash
kubectl rollout status deployment/hmac-manager -n hmac-system
```

## Step 6 — Register the ext-authz provider in Istio

Patch the Istio MeshConfig to register `hmac-manager` as an extension provider:

```bash
kubectl patch configmap istio \
  -n istio-system \
  --type merge \
  -p '{"data":{"mesh":"extensionProviders:\n- name: hmac-manager\n  envoyExtAuthzHttp:\n    service: hmac-manager.hmac-system.svc.cluster.local\n    port: 8080\n    withRequestBody:\n      maxRequestBytes: 8192\n      allowPartialMessage: false\n"}}'
```

Or apply the provided patch file directly:

```bash
kubectl apply -f services/HmacManager.Kubernetes/deploy/mesh-config-patch.yaml
```

> `withRequestBody` is required because HMAC signatures cover the request body hash. Adjust `maxRequestBytes` to match your largest expected payload.

Restart `istiod` to pick up the new provider:

```bash
kubectl rollout restart deployment/istiod -n istio-system
```

## Step 7 — Enroll a workload namespace and create a waypoint

Label the `default` namespace for ambient mode enrollment:

```bash
kubectl label namespace default istio.io/dataplane-mode=ambient
```

Create a waypoint proxy for the namespace:

```bash
istioctl waypoint apply --namespace default --enroll-namespace
```

Verify the waypoint is ready:

```bash
kubectl get gateway -n default
# NAME       CLASS            ADDRESS       PROGRAMMED   AGE
# waypoint   istio-waypoint   10.96.x.x     True         ...
```

## Step 8 — Apply the AuthorizationPolicy

```bash
kubectl apply -f services/HmacManager.Kubernetes/deploy/authorization-policy.yaml
```

This tells the waypoint to call `hmac-manager` via `action: CUSTOM` for all inbound requests to workloads in the `default` namespace.

## Step 9 — Test it

Deploy a simple echo server in the protected namespace:

```bash
kubectl run echo --image=ealen/echo-server --expose --port=80 -n default
```

**Unsigned request — should be rejected:**

```bash
kubectl run curl --image=curlimages/curl --restart=Never --rm -it \
  -- curl -sv http://echo.default.svc.cluster.local/test
# Expected: HTTP/1.1 403 Forbidden
```

**Signed request — should be forwarded:**

Use the `/sign` endpoint (see below) to get the headers, then attach them to your curl.

## Signing endpoint (Development only)

`POST /sign` is only registered when `ASPNETCORE_ENVIRONMENT=Development`. It accepts a request description and returns the HMAC headers you need to attach to your actual request — no separate .NET client required.

**Request:**

```json
{
  "policy": "MyPolicy",
  "method": "GET",
  "uri": "http://echo.default.svc.cluster.local/test",
  "scheme": null,
  "body": null
}
```

| Field | Required | Description |
|-------|----------|-------------|
| `policy` | Yes | Policy name as configured in `appsettings.json` |
| `method` | Yes | HTTP method of the request you want to sign |
| `uri` | Yes | Full URI of the request you want to sign (must match exactly what the real request will use) |
| `scheme` | No | Named scheme within the policy, if any |
| `body` | No | Request body string; include this if the real request has a body so the content hash covers it |

**Response:**

```json
{
  "Authorization": "Hmac <signature>",
  "Hmac-Policy": "MyPolicy",
  "Hmac-Nonce": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Hmac-DateRequested": "2026-06-15T10:00:00.000Z",
  "Hmac-Options": "..."
}
```

**End-to-end test with curl:**

```bash
# 1. Get signed headers from the signing endpoint
SIGN_RESPONSE=$(kubectl run curl-sign --image=curlimages/curl --restart=Never --rm -q -it \
  -- curl -s -X POST http://hmac-manager.hmac-system.svc.cluster.local:8080/sign \
  -H "Content-Type: application/json" \
  -d '{"policy":"MyPolicy","method":"GET","uri":"http://echo.default.svc.cluster.local/test"}')

AUTH=$(echo "$SIGN_RESPONSE"       | grep -o '"Authorization":"[^"]*"' | cut -d'"' -f4)
POLICY=$(echo "$SIGN_RESPONSE"     | grep -o '"Hmac-Policy":"[^"]*"'   | cut -d'"' -f4)
NONCE=$(echo "$SIGN_RESPONSE"      | grep -o '"Hmac-Nonce":"[^"]*"'    | cut -d'"' -f4)
DATE=$(echo "$SIGN_RESPONSE"       | grep -o '"Hmac-DateRequested":"[^"]*"' | cut -d'"' -f4)
OPTIONS=$(echo "$SIGN_RESPONSE"    | grep -o '"Hmac-Options":"[^"]*"'  | cut -d'"' -f4)

# 2. Send the signed request through the waypoint
kubectl run curl-test --image=curlimages/curl --restart=Never --rm -it \
  -- curl -sv http://echo.default.svc.cluster.local/test \
  -H "Authorization: $AUTH" \
  -H "Hmac-Policy: $POLICY" \
  -H "Hmac-Nonce: $NONCE" \
  -H "Hmac-DateRequested: $DATE" \
  -H "Hmac-Options: $OPTIONS"
# Expected: 200 OK, response from echo server
```

> The `/sign` endpoint is not registered when `ASPNETCORE_ENVIRONMENT` is anything other than `Development`. In staging and production the route returns 404 — it does not exist.

## Verifying ext-authz calls

Check the waypoint proxy logs to confirm it is calling the ext-authz service:

```bash
kubectl logs -l gateway.istio.io/managed=istio-waypoint -n default -c istio-proxy | grep ext_authz
```

Check the HmacManager service logs:

```bash
kubectl logs -l app=hmac-manager -n hmac-system
```

## Configuration reference

| Setting | Description | Default |
|---------|-------------|---------|
| `HmacManager__N__Name` | Policy name (matches `Hmac-Policy` header sent by client) | — |
| `HmacManager__N__Keys__PublicKey` | Public key GUID sent in the signed content | — |
| `HmacManager__N__Keys__PrivateKey` | Private key used for HMAC computation | — |
| `HmacManager__N__Algorithms__ContentHashAlgorithm` | `SHA256` or `SHA512` | `SHA256` |
| `HmacManager__N__Algorithms__SigningHashAlgorithm` | `HMACSHA256` or `HMACSHA512` | `HMACSHA256` |
| `HmacManager__N__Nonce__MaxAgeInSeconds` | Replay attack window | `30` |
| `HmacManager__N__Nonce__CacheType` | `Memory` or `Distributed` | `Memory` |

## Teardown

```bash
kind delete cluster --name hmac-test
# or
k3d cluster delete hmac-test
```
