using System;
using System.Collections.Generic;
using System.Text;

namespace Spooky.Json20
{
	/// <summary>
	/// An <see cref="IRpcClient"/> implementation for making Json RPC 2.0 calls.
	/// </summary>
	public class JsonRpcHttpClient : RpcClient
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonRpcHttpClient"/> class.
		/// </summary>
		/// <param name="serviceAddress">The url of the Json RPC service this client accesses.</param>
		public JsonRpcHttpClient(Uri serviceAddress) 
			: base
			(
				new RpcClientOptions()
				{
					Serializer = new JsonRpcSerializer(),
					Transport = new JsonRpcHttpTransport(serviceAddress)
				}
			)
		{
		}
	}
}