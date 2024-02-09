
# HmacManager

[![NuGet Version](https://img.shields.io/nuget/v/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/) [![NuGet Downloads](https://img.shields.io/nuget/dt/HmacManager.svg)](https://www.nuget.org/packages/HmacManager/)

## Summary

Integrate HMAC (Hash-based Message Authentication Code) authentication seamlessly into your .NET Core applications.

## Features

- Supports fully configurable policies, i.e. an application can accept a signed HMAC from one of many policies
- Each policy can define header schemes that determine required headers values that must be present in both the request and signed HMAC
- Automatic nonce handling to ensure replay attacks do not occur
- (In Progress) A client library to easily integrate signatures from client to server

## Installation
`HmacManager` is available on [NuGet](https://www.nuget.org/packages/HmacManager/). 

    dotnet add package HmacManager

## Docs

[HmacManager](Source/README.md)