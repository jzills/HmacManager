# Releasing

This project has three independently versioned artifacts, each with its own release pipeline. All releases follow Gitflow: work lands on `develop` via feature branches, gets stabilized on a release branch, merges into `main`, and is tagged at that merge commit.

## Artifacts and Tag Prefixes

| Artifact | Tag format | Trigger |
|---|---|---|
| NuGet package | `nuget/vX.Y.Z` | `.github/workflows/release.yml` |
| Docker image (ext-authz service) | `service/vX.Y.Z` | `.github/workflows/service-release.yml` |
| Helm chart | `chart/vX.Y.Z` | `.github/workflows/chart-release.yml` |

Versions are extracted from the tag at publish time — no source files need manual version bumps except `Chart.yaml`'s `appVersion` field (see [Helm chart release](#helm-chart) below).

---

## NuGet Package

Covers changes to the core library under `src/`.

```bash
# 1. Cut a release branch from develop
git checkout develop && git pull origin develop
git checkout -b release/v2.7.0

# 2. Stabilize (bug fixes, changelog, etc.), then merge into main
git checkout main && git pull origin main
git merge --no-ff release/v2.7.0
git push origin main

# 3. Tag and publish via GitHub Release
gh release create nuget/v2.7.0 --target main --generate-notes

# 4. Back-merge into develop
git checkout develop
git merge --no-ff main
git push origin develop

# 5. Delete the release branch
git push origin --delete release/v2.7.0
git branch -d release/v2.7.0
```

**Required secret:** `NUGET_API_KEY` (Settings → Secrets → Actions).

---

## Docker Image (ext-authz service) {#docker-image}

Covers changes to `kubernetes/service/`.

```bash
# 1. Cut a release branch from develop
git checkout develop && git pull origin develop
git checkout -b release/service/v1.2.0

# 2. Stabilize, then merge into main
git checkout main && git pull origin main
git merge --no-ff release/service/v1.2.0
git push origin main

# 3. Tag and publish via GitHub Release
gh release create service/v1.2.0 --target main --generate-notes

# 4. Back-merge into develop
git checkout develop
git merge --no-ff main
git push origin develop

# 5. Delete the release branch
git push origin --delete release/service/v1.2.0
git branch -d release/service/v1.2.0
```

**Required secrets:** `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN` (Settings → Secrets → Actions).

Builds multi-arch images (`linux/amd64`, `linux/arm64`) and pushes two tags:
- `your-username/hmac-manager:1.2.0`
- `your-username/hmac-manager:latest`

---

## Helm Chart {#helm-chart}

Covers changes to `kubernetes/chart/`.

### When to update `appVersion`

`Chart.yaml` contains two version fields:

- `version` — the chart version; bump when templates or values change
- `appVersion` — the Docker image version the chart deploys; bump when releasing a new service version

If you are releasing a new service version alongside a chart update, update `appVersion` in `Chart.yaml` on the release branch before merging.

```bash
# 1. Cut a release branch from develop
git checkout develop && git pull origin develop
git checkout -b release/chart/v1.5.0

# 2. If the chart now targets a new service version, update appVersion
#    Edit kubernetes/chart/Chart.yaml: appVersion: "1.2.0"

# 3. Stabilize, then merge into main
git checkout main && git pull origin main
git merge --no-ff release/chart/v1.5.0
git push origin main

# 4. Tag and publish via GitHub Release
gh release create chart/v1.5.0 --target main --generate-notes

# 5. Back-merge into develop
git checkout develop
git merge --no-ff main
git push origin develop

# 6. Delete the release branch
git push origin --delete release/chart/v1.5.0
git branch -d release/chart/v1.5.0
```

The chart is published two ways from the same `chart/vX.Y.Z` tag:

- **HTTP repo** on GitHub Pages (via `helm/chart-releaser-action`) — the conventional `helm repo add` experience and the source Artifact Hub indexes.
- **OCI artifact** on GHCR — for users who prefer pulling charts from the same registry as images.

Install via the HTTP repo:

```bash
helm repo add hmac-manager https://jzills.github.io/HmacManager
helm repo update
helm install hmac-manager hmac-manager/hmac-manager --version 1.5.0
```

Or via OCI:

```bash
helm install hmac-manager oci://ghcr.io/jzills/charts/hmac-manager --version 1.5.0
```

> **One-time setup for the HTTP repo:** create an empty `gh-pages` branch and enable
> GitHub Pages for it (Settings → Pages → Branch: `gh-pages`, folder `/`). The
> `pages` job in `chart-release.yml` then publishes `index.yaml`, the packaged
> chart, and `artifacthub-repo.yml` to the Pages root on each chart release.
> Point Artifact Hub at `https://jzills.github.io/HmacManager`; because the repo
> metadata file is served at the root, the Verified Publisher badge works with no
> extra OCI metadata push.

---

## Releasing Multiple Artifacts Together

When a service change also requires chart changes (e.g., new env var in values.yaml), release them in order:

1. **Service first** — merge `release/service/vX.Y.Z` → `main`, tag `service/vX.Y.Z`
2. **Chart second** — on the chart release branch, update `appVersion` to match, merge → `main`, tag `chart/vX.Y.Z`

This ensures the Docker image exists on Docker Hub before the chart that references it is published.

---

## Secrets Checklist

| Secret | Used by | Where to obtain |
|---|---|---|
| `NUGET_API_KEY` | `release.yml` | nuget.org → Account → API Keys (push-only, scoped to HmacManager) |
| `DOCKERHUB_USERNAME` | `service-release.yml` | Your Docker Hub username |
| `DOCKERHUB_TOKEN` | `service-release.yml` | Docker Hub → Account Settings → Security → Access Tokens |

GHCR (Helm chart) uses the built-in `GITHUB_TOKEN` — no additional secret required.
