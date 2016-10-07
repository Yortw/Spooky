using System;
using System.Collections.Generic;
using System.Text;

namespace Spooky.Json20
{
	public class JsonRpcHttpClient : RpcClient
	{
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