# hmac-manager

Istio ext-authz HTTP server for HMAC request authentication.

The waypoint proxy or ingress gateway calls this service before forwarding any inbound request. A valid HMAC signature passes; anything else is rejected with `403 Forbidden`. Redis is bundled automatically — no external dependencies to manage.

```
Client → Istio waypoint / ingress gateway
              ↓ ext-authz check
         hmac-manager (this chart)
              ↓ VerifyAsync
         200 OK → forward   |   403 → reject
```

## Prerequisites

- Kubernetes 1.25+
- Istio 1.21+ (ambient mode or gateway mode)
- Helm 3.10+
- An existing Kubernetes Secret containing each policy's private key (created externally — e.g. via [External Secrets Operator](https://external-secrets.io/))

## Install

```bash
helm repo add hmac-manager https://jzills.github.io/HmacManager
helm repo update
```

```bash
helm install hmac-manager hmac-manager/hmac-manager \
  --namespace hmac-system \
  --create-namespace \
  --set "policies[0].name=MyPolicy" \
  --set "policies[0].publicKey=00000000-0000-0000-0000-000000000001" \
  --set "policies[0].privateKeySecret.name=my-hmac-secrets" \
  --set "policies[0].privateKeySecret.key=MyPolicy-privateKey"
```

Or with a values file:

```yaml
# values.yaml
policies:
  - name: MyPolicy
    publicKey: "00000000-0000-0000-0000-000000000001"
    privateKeySecret:
      name: my-hmac-secrets   # pre-existing Secret, e.g. from External Secrets
      key: MyPolicy-privateKey
```

```bash
helm install hmac-manager hmac-manager/hmac-manager \
  --namespace hmac-system \
  --create-namespace \
  -f values.yaml
```

## Register the ext-authz provider

After installation, NOTES.txt prints a ready-to-run script. The manual equivalent:

```bash
kubectl patch configmap istio -n istio-system --type merge -p '{
  "data": {
    "mesh": "extensionProviders:\n- name: hmac-manager\n  envoyExtAuthzHttp:\n    service: hmac-manager.hmac-system.svc.cluster.local\n    port: 8080\n    includeRequestHeadersInCheck:\n      - authorization\n      - hmac-policy\n      - hmac-nonce\n      - hmac-daterequested\n    withRequestBody:\n      maxRequestBytes: 8192\n      allowPartialMessage: false\n"
  }
}'
kubectl rollout restart deployment/istiod -n istio-system
```

## Configuration

### Policies

At least one policy is required. Private keys must live in a pre-existing Kubernetes Secret — never pass them as chart values.

```yaml
policies:
  - name: MyPolicy
    publicKey: "00000000-0000-0000-0000-000000000001"
    privateKeySecret:
      name: my-hmac-secrets     # name of a pre-existing Secret
      key: MyPolicy-privateKey  # key within that Secret
    algorithms:
      contentHash: SHA256       # SHA1 | SHA256 | SHA512   (default: SHA256)
      signingHash: HMACSHA256   # HMACSHA1 | HMACSHA256 | HMACSHA512 (default: HMACSHA256)
    nonce:
      maxAgeInSeconds: 60       # replay attack window in seconds (default: 60)
    schemes:                    # optional: named header sets included in the signature
      - name: UserScheme
        headers:
          - name: X-UserId
            claimType: userId
```

### Redis (replay protection)

Redis is deployed as part of this release when `redis.enabled=true` (the default). All policies automatically use the distributed nonce cache. No connection strings, no external Redis cluster required.

| Value | Default | Description |
|---|---|---|
| `redis.enabled` | `true` | Deploy bundled Redis and use distributed nonce cache. Set to `false` for single-replica deployments only. |

When `redis.enabled=false` the chart refuses `replicaCount > 1` — the in-process nonce cache is not shared across pods.

### Other values

| Value | Default | Description |
|---|---|---|
| `environment` | `Production` | `Production` or `Development`. `Development` activates the signing helper endpoint on port 8081. |
| `replicaCount` | `1` | Number of replicas. Values > 1 require `redis.enabled=true`. |
| `namespace` | `hmac-system` | Namespace to deploy into. |
| `image.repository` | `zills/hmac-manager` | Container image repository. |
| `image.tag` | `0.0.1` | Container image tag. |
| `service.port` | `8080` | Port the ext-authz service listens on. |
| `istio.enabled` | `true` | Render Istio `AuthorizationPolicy` resources. |
| `istio.ingressGateway.enabled` | `true` | Protect ingress gateway traffic. |
| `istio.waypoint.enabled` | `false` | Protect east-west (ambient mode) traffic. |

## Multiple policies

Add additional entries to the `policies` list. Each policy can have its own keys, algorithms, nonce TTL, and schemes:

```yaml
policies:
  - name: PublicAPI
    publicKey: "00000000-0000-0000-0000-000000000001"
    privateKeySecret:
      name: api-secrets
      key: public-api-private-key

  - name: InternalService
    publicKey: "00000000-0000-0000-0000-000000000002"
    privateKeySecret:
      name: api-secrets
      key: internal-service-private-key
    algorithms:
      signingHash: HMACSHA512
    nonce:
      maxAgeInSeconds: 30
```

## Signing requests (client side)

Use the [HmacManager NuGet package](https://www.nuget.org/packages/HmacManager) to sign outgoing requests from any .NET client.

## Development signing endpoint

Set `environment: Development` to activate a `/sign` helper endpoint on port 8081 (not exposed by the Kubernetes Service). Port-forward to reach it:

```bash
kubectl port-forward deploy/hmac-manager 9090:8081 -n hmac-system

curl -s -X POST http://localhost:9090/sign \
  -H "Content-Type: application/json" \
  -d '{"policy":"MyPolicy","method":"GET","uri":"http://echo.default.svc.cluster.local/"}'
```

Returns the HMAC headers to attach to your request. Never use `Development` in a production cluster.

## Source

[github.com/jzills/HmacManager](https://github.com/jzills/HmacManager)
