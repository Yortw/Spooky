using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Spooky.XmlRpc
{
	/// <summary>
	/// An HTTP transport for XML-RPC requests, using an <see cref="HttpClient"/> to make requests.
	/// </summary>
	/// <remarks>
	/// <para>You can inject your own <see cref="HttpClient"/> instance via the constructors to control the HTTP pipeline and add features such as authorisation etc.</para>
	/// <para>If no <see cref="HttpClient"/> is injected the system creates a new instance with GZIP and Deflate compression support enabled.</para>
	/// </remarks>
	public class XmlRpcHttpTransport : HttpClientTransport
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcHttpTransport"/> class.
		/// </summary>
		/// <remarks>
		/// <para>Creates an instance using a new <see cref="HttpClient"/> instance with compression enabled and <see cref="System.Text.UTF8Encoding"/> encoding.</para>
		/// </remarks>
		/// <param name="serviceAddress">The service address.</param>
		public XmlRpcHttpTransport(Uri serviceAddress) : 
			this
			(
				serviceAddress,
				CreateDefaultXmlRpcHttpClient()
			)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcHttpTransport"/> class.
		/// </summary>
		/// <remarks>
		/// <para>Creates an instance using a new <see cref="HttpClient"/> instance with compression enabled and <see cref="System.Text.UTF8Encoding"/> encoding.</para>
		/// </remarks>
		/// <param name="serviceAddress">The service address.</param>
		/// <param name="httpClient">An <see cref="System.Net.Http.HttpClient"/> to use when making HTTP requests.</param>
		public XmlRpcHttpTransport(Uri serviceAddress, HttpClient httpClient) :
			base
			(
				serviceAddress, XmlRpcMediaTypes.TextXml,
				System.Text.UTF8Encoding.UTF8.WebName.ToLower(),
				httpClient
			)
		{
		}

		private static HttpClient CreateDefaultXmlRpcHttpClient()
		{
			var handler = new HttpClientHandler();
			if (handler.SupportsAutomaticDecompression)
				handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

			var client = new HttpClient(handler);
			client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("SpookyXmlRpc", typeof(XmlRpcHttpTransport).GetTypeInfo().Assembly.GetName().Version.ToString()));
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(XmlRpcMediaTypes.TextXml));

			return client;
		}
	}
}