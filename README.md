# Spooky
## What is Spooky?
Short description: A Json RPC 2.0 client library for .Net.

Long Description: Spooky is two things;

1. A portable, async-await based, .Net abstraction for RPC clients
2. An implementation of the Json RPC 2.0 specification over HTTP (using HttpClient) fitting that abstraction. 

Basically Spooky lets you make Json RPC calls, and could theoretically be extended to work with XML-RPC, future Json RPC versions and other similar systems without altering dependant systems.
Because the Json RPC implementation uses HttpClient for transport, it supports injecting message handlers into the HTTP pipeline. This allows support for HTTP based authentication/authorisation, compression, retry logic etc. 
(assuming the server is also compatible with those features)

[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/Yortw/Spooky/blob/master/LICENSE.md) 

## Supported Platforms
Currently;

* .Net Framework 4.0+
* Net Standard 1.1
* Xamarin.iOS
* Xamarin.Android
* WinRT (Windows Store Apps 8.1)
* UWP 10+ (Windows 10 Universal Programs)

## Build Status
[![Build status](https://ci.appveyor.com/api/projects/status/88t4uo6hvxfiqbe0?svg=true)](https://ci.appveyor.com/project/Yortw/Spooky)

## How do I use Spooky?
*We got your samples right here*

Install the relevant Nuget package. If you want to do Json RPC (2.0) do this;

```powershell
    PM> Install-Package Spooky.Json20
```

If you just want the abstraction layer to write your own implementation against, try
```powershell
    PM> Install-Package Spooky
```

[![NuGet Badge](https://buildstats.info/nuget/Spooky)](https://www.nuget.org/packages/Spooky/)


