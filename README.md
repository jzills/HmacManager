
# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/)

## Summary

Integrate Hmac (Hash-based Message Authentication Code) authentication seamlessly into your .NET applications.

## Features

- Enables comprehensive configuration of policies, allowing applications to accept a signed Hmac from a variety of policies.
- Each policy has the capability to specify header schemes, dictating the mandatory header values in both the request and the signed Hmac.
- Implements automatic nonce handling to safeguard against replay attacks.
- Seamlessly integrates with authorization policies, providing precise control over authorized Hmac authentication configurations.
- (Currently in progress) JavaScript library to facilitate the effortless integration of signatures from the client to the server.

## Installation
`HmacManager` is available on [NuGet](https://www.nuget.org/packages/HmacManager/). 

    dotnet add package HmacManager

## Docs

[HmacManager](src/HmacManager/README.md)
