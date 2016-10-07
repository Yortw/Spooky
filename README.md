# Spooky
Short description: A Json RPC 2.0 client library for .Net.

Long Description: Spooky is two things;

1. A portable, async-await based, .Net abstraction for RPC clients
2. An implementation of the Json RPC 2.0 specification over HTTP (using HttpClient) fitting that abstraction. 

Basically Spooky lets you make Json RPC calls, and could theoretically be extended to work with XML-RPC, future Json RPC versions and other similar systems without altering existing systems.
Because the Json RPC implementation uses HttpClient for transport, it supports injecting message handlers in the HTTP pipeline to support HTTP based authentication/authorisation, compression, retry logic etc. 
assuming the server is also compatible with those features.

