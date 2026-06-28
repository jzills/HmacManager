#!/usr/bin/env bats

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

HMAC_SVC="hmac-manager.hmac-system.svc.cluster.local"
ECHO_SVC="echo.default.svc.cluster.local"
SIGN_PORT=9090

sign_request() {
    local method="$1" uri="$2"
    curl -sf -X POST "http://localhost:${SIGN_PORT}/sign" \
        -H "Content-Type: application/json" \
        -d "{\"Policy\":\"MyPolicy\",\"Method\":\"${method}\",\"Uri\":\"${uri}\"}"
}

extract() {
    local json="$1" key="$2"
    echo "$json" | python3 -c "import sys,json; print(json.load(sys.stdin)['${key}'])"
}

send_signed() {
    local sign_json="$1"
    local auth policy nonce date
    auth=$(extract "$sign_json" "Authorization")
    policy=$(extract "$sign_json" "Hmac-Policy")
    nonce=$(extract "$sign_json" "Hmac-Nonce")
    date=$(extract "$sign_json" "Hmac-DateRequested")

    kubectl exec -n default curl -- curl -sf -o /dev/null -w "%{http_code}" \
        -H "Authorization: $auth" \
        -H "Hmac-Policy: $policy" \
        -H "Hmac-Nonce: $nonce" \
        -H "Hmac-DateRequested: $date" \
        "http://${ECHO_SVC}/"
}

# ---------------------------------------------------------------------------
# Lifecycle
# ---------------------------------------------------------------------------

setup_file() {
    kubectl port-forward svc/hmac-manager "${SIGN_PORT}:8080" -n hmac-system \
        >/tmp/pf-hmac.log 2>&1 &
    echo "$!" > /tmp/pf-hmac.pid
    sleep 3
}

teardown_file() {
    if [[ -f /tmp/pf-hmac.pid ]]; then
        kill "$(cat /tmp/pf-hmac.pid)" 2>/dev/null || true
        rm -f /tmp/pf-hmac.pid
    fi
}

# ---------------------------------------------------------------------------
# Waypoint enforcement
# ---------------------------------------------------------------------------

@test "unsigned request from default namespace returns 403" {
    run kubectl exec -n default curl -- \
        curl -s -o /dev/null -w "%{http_code}" "http://${ECHO_SVC}/"
    [ "$output" = "403" ]
}

@test "signed request from default namespace returns 200" {
    local sign
    sign=$(sign_request "GET" "http://${ECHO_SVC}/")
    run send_signed "$sign"
    [ "$output" = "200" ]
}

@test "replay attack returns 403" {
    local sign
    sign=$(sign_request "GET" "http://${ECHO_SVC}/")

    # First use succeeds
    run send_signed "$sign"
    [ "$output" = "200" ]

    # Reuse of the same nonce is rejected
    run send_signed "$sign"
    [ "$output" = "403" ]
}

@test "wrong policy name returns 403" {
    local sign auth nonce date
    sign=$(sign_request "GET" "http://${ECHO_SVC}/")
    auth=$(extract "$sign" "Authorization")
    nonce=$(extract "$sign" "Hmac-Nonce")
    date=$(extract "$sign" "Hmac-DateRequested")

    run kubectl exec -n default curl -- curl -s -o /dev/null -w "%{http_code}" \
        -H "Authorization: $auth" \
        -H "Hmac-Policy: WrongPolicy" \
        -H "Hmac-Nonce: $nonce" \
        -H "Hmac-DateRequested: $date" \
        "http://${ECHO_SVC}/"
    [ "$output" = "403" ]
}

# ---------------------------------------------------------------------------
# Cross-namespace enforcement
# ---------------------------------------------------------------------------

@test "unsigned request from ambient-enrolled namespace returns 403" {
    kubectl create namespace test-ns --dry-run=client -o yaml | kubectl apply -f -
    kubectl label namespace test-ns istio.io/dataplane-mode=ambient --overwrite
    kubectl run curl-ext --image=curlimages/curl:latest -n test-ns \
        --restart=Never --command -- sleep 3600 \
        --dry-run=client -o yaml | kubectl apply -f -
    kubectl wait --for=condition=Ready pod/curl-ext -n test-ns --timeout=30s

    run kubectl exec -n test-ns curl-ext -- \
        curl -s -o /dev/null -w "%{http_code}" "http://${ECHO_SVC}/"
    [ "$output" = "403" ]

    kubectl delete namespace test-ns --wait=false
}

@test "signed request from ambient-enrolled namespace returns 200" {
    kubectl create namespace test-ns --dry-run=client -o yaml | kubectl apply -f -
    kubectl label namespace test-ns istio.io/dataplane-mode=ambient --overwrite
    kubectl run curl-ext --image=curlimages/curl:latest -n test-ns \
        --restart=Never --command -- sleep 3600 \
        --dry-run=client -o yaml | kubectl apply -f -
    kubectl wait --for=condition=Ready pod/curl-ext -n test-ns --timeout=30s

    local sign auth policy nonce date
    sign=$(sign_request "GET" "http://${ECHO_SVC}/")
    auth=$(extract "$sign" "Authorization")
    policy=$(extract "$sign" "Hmac-Policy")
    nonce=$(extract "$sign" "Hmac-Nonce")
    date=$(extract "$sign" "Hmac-DateRequested")

    run kubectl exec -n test-ns curl-ext -- curl -s -o /dev/null -w "%{http_code}" \
        -H "Authorization: $auth" \
        -H "Hmac-Policy: $policy" \
        -H "Hmac-Nonce: $nonce" \
        -H "Hmac-DateRequested: $date" \
        "http://${ECHO_SVC}/"
    [ "$output" = "200" ]

    kubectl delete namespace test-ns --wait=false
}

@test "unsigned request from non-ambient namespace bypasses waypoint" {
    kubectl create namespace external-ns --dry-run=client -o yaml | kubectl apply -f -
    kubectl run curl-ext --image=curlimages/curl:latest -n external-ns \
        --restart=Never --command -- sleep 3600 \
        --dry-run=client -o yaml | kubectl apply -f -
    kubectl wait --for=condition=Ready pod/curl-ext -n external-ns --timeout=30s

    # Not enrolled in ambient — bypasses the waypoint entirely
    run kubectl exec -n external-ns curl-ext -- \
        curl -s -o /dev/null -w "%{http_code}" "http://${ECHO_SVC}/"
    [ "$output" = "200" ]

    kubectl delete namespace external-ns --wait=false
}

# ---------------------------------------------------------------------------
# Ingress gateway enforcement
# ---------------------------------------------------------------------------

setup_ingress() {
    kubectl apply -f - <<'EOF'
apiVersion: gateway.networking.k8s.io/v1
kind: Gateway
metadata:
  name: ingress-gateway
  namespace: default
spec:
  gatewayClassName: istio
  listeners:
  - name: http
    protocol: HTTP
    port: 80
---
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: echo-route
  namespace: default
spec:
  parentRefs:
  - name: ingress-gateway
    namespace: default
  rules:
  - matches:
    - path:
        type: PathPrefix
        value: /
    backendRefs:
    - name: echo
      port: 80
---
apiVersion: security.istio.io/v1
kind: AuthorizationPolicy
metadata:
  name: hmac-manager-ingress-auth
  namespace: default
spec:
  targetRefs:
  - group: gateway.networking.k8s.io
    kind: Gateway
    name: ingress-gateway
  action: CUSTOM
  provider:
    name: hmac-manager
  rules:
  - {}
EOF
    kubectl wait --for=condition=Programmed gateway/ingress-gateway -n default --timeout=60s
    sleep 3  # allow xDS propagation

    GW_IP=$(kubectl get gateway ingress-gateway -n default \
        -o jsonpath='{.status.addresses[0].value}')

    kubectl port-forward svc/ingress-gateway-istio 8888:80 -n default \
        >/tmp/pf-ingress.log 2>&1 &
    echo "$!" > /tmp/pf-ingress.pid
    sleep 2

    export INGRESS_URL="http://localhost:8888"
}

teardown_ingress() {
    if [[ -f /tmp/pf-ingress.pid ]]; then
        kill "$(cat /tmp/pf-ingress.pid)" 2>/dev/null || true
        rm -f /tmp/pf-ingress.pid
    fi
    kubectl delete gateway ingress-gateway -n default --ignore-not-found
    kubectl delete httproute echo-route -n default --ignore-not-found
    kubectl delete authorizationpolicy hmac-manager-ingress-auth -n default --ignore-not-found
}

@test "unsigned request through ingress gateway returns 403" {
    setup_ingress
    run curl -s -o /dev/null -w "%{http_code}" "$INGRESS_URL/"
    teardown_ingress
    [ "$output" = "403" ]
}

@test "signed request through ingress gateway returns 200" {
    setup_ingress

    local sign
    sign=$(sign_request "GET" "$INGRESS_URL/")
    local auth policy nonce date
    auth=$(extract "$sign" "Authorization")
    policy=$(extract "$sign" "Hmac-Policy")
    nonce=$(extract "$sign" "Hmac-Nonce")
    date=$(extract "$sign" "Hmac-DateRequested")

    run curl -s -o /dev/null -w "%{http_code}" "$INGRESS_URL/" \
        -H "Authorization: $auth" \
        -H "Hmac-Policy: $policy" \
        -H "Hmac-Nonce: $nonce" \
        -H "Hmac-DateRequested: $date"
    teardown_ingress
    [ "$output" = "200" ]
}

@test "replay attack through ingress gateway returns 403" {
    setup_ingress

    local sign
    sign=$(sign_request "GET" "$INGRESS_URL/")
    local auth policy nonce date
    auth=$(extract "$sign" "Authorization")
    policy=$(extract "$sign" "Hmac-Policy")
    nonce=$(extract "$sign" "Hmac-Nonce")
    date=$(extract "$sign" "Hmac-DateRequested")

    curl -s -o /dev/null "http://localhost:8888/" \
        -H "Authorization: $auth" -H "Hmac-Policy: $policy" \
        -H "Hmac-Nonce: $nonce" -H "Hmac-DateRequested: $date"

    run curl -s -o /dev/null -w "%{http_code}" "$INGRESS_URL/" \
        -H "Authorization: $auth" -H "Hmac-Policy: $policy" \
        -H "Hmac-Nonce: $nonce" -H "Hmac-DateRequested: $date"
    teardown_ingress
    [ "$output" = "403" ]
}
