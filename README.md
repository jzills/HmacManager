
<div align="center">
<pre>
╔═════════════════════════════════════════════════╗
║     __  ____  ______   ______                   ║
║    / / / /  |/  /   | / ____/                   ║
║   / /_/ / /|_/ / /| |/ /                        ║
║  / __  / /  / / ___ / /___                      ║
║ /_/ /_/_/  /_/_/  |_\____/                      ║
║     __  ______    _   _____   ________________  ║
║    /  |/  /   |  / | / /   | / ____/ ____/ __ \ ║
║   / /|_/ / /| | /  |/ / /| |/ / __/ __/ / /_/ / ║
║  / /  / / ___ |/ /|  / ___ / /_/ / /___/ _, _/  ║
║ /_/  /_/_/  |_/_/ |_/_/  |_\____/_____/_/ |_|   ║
╚═════════════════════════════════════════════════╝
</pre>
</div>

# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![npm Version](https://img.shields.io/npm/v/hmac-manager?logo=npm&label=npm)](https://www.npmjs.com/package/hmac-manager) [![Docker Image Version](https://img.shields.io/docker/v/zills/hmac-manager?logo=docker&label=docker)](https://hub.docker.com/r/zills/hmac-manager) [![Docker Pulls](https://img.shields.io/docker/pulls/zills/hmac-manager?logo=docker)](https://hub.docker.com/r/zills/hmac-manager) [![Artifact Hub](https://img.shields.io/endpoint?url=https://artifacthub.io/badge/repository/hmac-manager)](https://artifacthub.io/packages/search?repo=hmac-manager) [![.NET](https://github.com/jzills/HmacManager/actions/workflows/pr.yml/badge.svg)](https://github.com/jzills/HmacManager/actions/workflows/pr.yml) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> Secure HMAC request authentication for ASP.NET Core — as a NuGet library, or as a containerized Istio ext-authz service for Kubernetes.

- [Summary](#summary)
- [Features](#features)
- [Installation](#installation)
- [Kubernetes (Istio ext-authz)](#kubernetes-istio-ext-authz)
- [Documentation](./src/README.md)
- [Client Library](./client/)
- [Resources](#resources)
- [Further Reading](#further-reading)

## Summary

Add secure HMAC request authentication to ASP.NET Core APIs with lightweight, configurable middleware.

## Features

- Facilitates detailed policy configuration, enabling applications to sign and verify Hmacs against multiple policy criteria.
- Each policy can optionally define specific schemes, outlining the required header values for a request.
    - Including automatic claims mapping from header values defined in a scheme.
- Supports policy modification at runtime.
    - Including both an option to use a singleton or the preferred approach to use an accessor that can pull policies dynamically from a database or some other data store.
- Incorporates automatic nonce management to protect against replay attacks.
- Integrates with ASP.NET Core authorization mechanisms.

## Installation

`HmacManager` is available on [NuGet](https://www.nuget.org/packages/HmacManager/). 

    dotnet add package HmacManager

## Kubernetes (Istio ext-authz)

Beyond the library, HmacManager ships as a containerized **ext-authz service** for [Istio](https://istio.io/) ambient mode. The waypoint (or ingress gateway) calls the service before forwarding a request, so unsigned or invalid requests are rejected at the mesh — no per-application integration code required.

**Container image** — [`zills/hmac-manager`](https://hub.docker.com/r/zills/hmac-manager) on Docker Hub:

```bash
docker pull zills/hmac-manager:latest
```

**Helm chart** — install from the GitHub Pages HTTP repo:

```bash
helm repo add hmac-manager https://jzills.github.io/HmacManager
helm repo update
helm install hmac-manager hmac-manager/hmac-manager
```

or pull it from GHCR as an OCI artifact:

```bash
helm install hmac-manager oci://ghcr.io/jzills/charts/hmac-manager --version 0.0.1
```

See the [Kubernetes setup guide](kubernetes/service/README.md) for the full walkthrough (Istio install, waypoint enrollment, MeshConfig, request signing), and [RELEASING.md](RELEASING.md) for how the NuGet package, Docker image, and Helm chart are versioned and published.

## Resources

- [Documentation](src/README.md)
- [Samples](samples/README.md)

## Further Reading

- [Hmac](https://en.wikipedia.org/wiki/Hmac)
- [Sign an Http Request](https://learn.microsoft.com/en-us/azure/communication-services/tutorials/Hmac-header-tutorial?pivots=programming-language-csharp)
