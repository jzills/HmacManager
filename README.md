
# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![.NET](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml)

- [Summary](#summary)
- [Features](#features)
- [Installation](#installation)
- [Documentation](./src/README.md)
- [Resources](#resources)
- [Further Reading](#further-reading)

## Summary

Integrate Hmac authentication seamlessly into your ASP.NET Core applications, fortifying security measures and ensuring robust authentication protocols.

## Features

- Facilitates detailed policy configuration, enabling applications to sign and verify Hmacs against multiple policy criteria.
- Each policy can define specific schemes, outlining the required header values for a request.
    - This includes automatic claims mapping from header values defined in a scheme.
- Supports policy creation and deletion at runtime.
- Incorporates automatic nonce management to protect against replay attacks.
- Integrates with ASP.NET Core authorization mechanisms.

## Installation

`HmacManager` is available on [NuGet](https://www.nuget.org/packages/HmacManager/). 

    dotnet add package HmacManager

## Resources

- [Documentation](src/README.md)
- [Samples](samples/README.md)

## Further Reading

- [Hmac](https://en.wikipedia.org/wiki/Hmac)
- [Sign an Http Request](https://learn.microsoft.com/en-us/azure/communication-services/tutorials/Hmac-header-tutorial?pivots=programming-language-csharp)