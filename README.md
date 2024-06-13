
# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![.NET](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jzills/HmacManager/actions/workflows/dotnet.yml)

- [Summary](#summary)
- [Features](#features)
- [Installation](#installation)
- [Documentation](./src/HmacManager/README.md)
- [Resources](#resources)

## Summary

Integrate Hmac authentication seamlessly into your ASP.NET Core applications, fortifying security measures and ensuring robust authentication protocols.

## Features

- Enables comprehensive configuration of policies, allowing applications to accept a signed Hmac from a variety of policies.
- Each policy has the capability to specify header schemes, dictating the mandatory header values in both the request and the signed Hmac.
    - This includes automatic claims mapping from required header values.
- Dynamic policy creation and deletion at runtime.
- Implements automatic nonce handling to safeguard against replay attacks.
- Seamlessly integrates with authorization policies, providing precise control over authorized Hmac authentication configurations.
- Integration with built-in ASP.NET Core authorization.
- (In-Progress) JavaScript library to facilitate the effortless integration of signatures from client to server.

## Installation

`HmacManager` is available on [NuGet](https://www.nuget.org/packages/HmacManager/). 

    dotnet add package HmacManager

## Resources

- [Documentation](src/HmacManager/README.md)
- [Samples](samples/README.md)
