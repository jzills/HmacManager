# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

All commands run from the respective project directory.

```bash
# Build the library
cd src && dotnet build

# Run all unit tests
cd test/Unit && dotnet test

# Run a single test class
cd test/Unit && dotnet test --filter "ClassName=Test_InMemory_ReplayAttack"

# Run a single test method
cd test/Unit && dotnet test --filter "FullyQualifiedName~MethodName"

# Run with coverage
cd test/Unit && dotnet test --collect:"XPlat Code Coverage"
```

The library targets `net8.0` and `net10.0`. Integration tests (`test/Integration`) require a running Redis instance on the default port.

## Architecture Overview

HmacManager is an ASP.NET Core HMAC authentication library. It supports both server-side request verification and client-side request signing via `HttpClient`. The two primary integration paths are:

1. **Server (verifying)**: Register via `services.AddHmacManager(...)` + `builder.AddHmac(...)`. Requests are validated by `HmacAuthenticationHandler` (in `src/Mvc/`).
2. **Client (signing)**: Register via `httpClientBuilder.AddHmacHttpMessageHandler(...)`. `HmacDelegatingHandler` signs outgoing requests automatically.

### Core signing flow

`IHmacManager` (`src/Components/HmacManager.cs`) is the central type. `SignAsync(HttpRequestMessage)` creates an `Hmac`, computes a signature, and attaches headers. `VerifyAsync(HttpRequestMessage)` parses incoming headers, recomputes the signature, and compares.

Signature computation lives in `HmacSignatureProvider` → `HmacFactory` → hash generators in `src/Components/Hashing/Generators/`. The signing content string (method + URI + date + public key + content hash + scheme header values + nonce) is built by implementations of `ISigningContentBuilder`.

### Policies and schemes

- **`HmacPolicy`** (`src/Policies/`) is the top-level configuration unit: it holds `KeyCredentials`, hash `Algorithms`, an optional `Nonce` cache config, and a `SchemeCollection`.
- **`Scheme`** (`src/Schemes/`) is a named set of required HTTP headers whose values are included in the signature, enabling scoped authentication contexts.
- Policies are stored in `IHmacPolicyCollection`, which is registered as either singleton (static) or scoped (dynamic/DB-driven).

### Nonce caching

`INonceCache` (`src/Caching/`) prevents replay attacks by storing used nonces with a TTL. Two implementations exist: `MemoryNonceCache` (in-process) and `DistributedNonceCache` (Redis). The `Nonce` config on a policy selects which to use.

### DI wiring

`IServiceCollectionExtensions.AddHmacManager()` (`src/Mvc/Extensions/`) registers all internal services. `IHmacManagerFactory` is the DI-resolvable entry point to obtain an `IHmacManager` for a named policy at runtime.

### Key types at a glance

| Type | Location | Purpose |
|---|---|---|
| `IHmacManager` | `src/Components/Interfaces/` | Sign / verify requests |
| `IHmacManagerFactory` | `src/Components/Interfaces/` | Resolve manager by policy name |
| `HmacPolicy` | `src/Policies/` | Top-level auth configuration |
| `Scheme` | `src/Schemes/` | Named header set included in signature |
| `HmacResult` | `src/Components/` | Result of sign/verify (success + `Hmac` snapshot) |
| `HmacDelegatingHandler` | `src/Mvc/` | Auto-signs outgoing `HttpClient` requests |
| `HmacAuthenticationHandler` | `src/Mvc/` | ASP.NET Core auth scheme handler |
| `HmacEvents` | `src/Mvc/` | Hooks: `OnValidateKeys`, `OnAuthSuccess`, `OnAuthFailure` |

### Test layout

Tests mirror the source structure under `test/Unit/`. Shared test data and helpers are in `test/Unit/Common/`. The `src` project exposes internals to the `Unit` project via `InternalsVisibleTo`.
