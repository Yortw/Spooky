# Spooky
## What is Spooky?
Short description: A Json RPC 2.0 client library for .Net.

Long Description: Spooky is two things;

1. A portable, async-await based, .Net abstraction for RPC clients.
2. An implementation of the Json RPC 2.0 specification over HTTP (using HttpClient) fitting that abstraction. 

Basically Spooky lets you make Json RPC calls, and could theoretically be extended to work with XML-RPC, future Json RPC versions and other similar systems without altering dependant systems.
Because the Json RPC implementation uses HttpClient for transport, it supports injecting message handlers into the HTTP pipeline. This allows support for HTTP based authentication/authorisation, compression, retry logic etc. 
(assuming the server is also compatible with those features).

The transport, serialization and client layers are all separate, so while Json RPC over HTTP is supported out of the box, implementing it over sockets or another network protocol should be relatively simple and self-contained.
By relying on the abstraction, clients are protected from changing requirements at the network or serialization layers, and could even choose those at runtime.

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
[![Build status](https://ci.appveyor.com/api/projects/status/o2m5qx499ctm58qg?svg=true)](https://ci.appveyor.com/project/Yortw/spooky)

## How do I use Spooky?
*We got your samples right here*

Install the relevant Nuget package. 

If you just want the abstraction layer to write your own implementation against, try;
```powershell
PM> Install-Package Spooky
```

If you want to do Json RPC (2.0) do this;
```powershell
PM> Install-Package Spooky.Json20
```

Once the package is installed, add a using for the Spooky.Json20 namespace;
```c#
using Spooky.Json20;
```

Then create an client and call the *Invoke method* to start making remote calls. The following sample calls a remote *add* procedure passing two arguments by name, *a* and *b*. Both named and 
positional arguments are supported. For positional arguments use an object array or the overload that takes params object[]. For named arguments pass a dictionary as shown in this sample.
```c#
    var client = new Spooky.Json20.JsonRpcHttpClient(new Uri("http://www.myserver.com/mathservice"));
    var answer = await client.Invoke<int>
    (
	    "add", 
	    new Dictionary<string, object>()
	    {
    		{ "a", 4 },
	    	{ "b", 6 }
	    }
    ).ConfigureAwait(false);
```

That's it, you've made your first successful RPC call!

[![NuGet Badge](https://buildstats.info/nuget/Spooky)](https://www.nuget.org/packages/Spooky/)

## Server Implementation
Spooky doesn't even attempt to provide a server implementation. If you want to implement a server in .Net, try the awesome [Jayrock](https://github.com/atifaziz/Jayrock) library.
