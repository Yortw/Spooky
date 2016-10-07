using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Spooky.Json20
{
	public class JsonRpcHttpTransport : HttpClientTransport
	{
		public JsonRpcHttpTransport(Uri serviceAddress) : 
			base
			(
				serviceAddress, JsonRpcMediaTypes.ApplicationJson, 
				System.Text.UTF8Encoding.UTF8.WebName.ToLower(), 
				CreateDefaultJsonRpcHttpClient()
			)
		{
		}

		private static HttpClient CreateDefaultJsonRpcHttpClient()
		{
			var handler = new HttpClientHandler();
			if (handler.SupportsAutomaticDecompression)
				handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

			var client = new HttpClient(handler);
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(JsonRpcMediaTypes.ApplicationJson));
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(JsonRpcMediaTypes.ApplicationJsonRequest));
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(JsonRpcMediaTypes.ApplicationJson));

			return client;
		}
	}
}