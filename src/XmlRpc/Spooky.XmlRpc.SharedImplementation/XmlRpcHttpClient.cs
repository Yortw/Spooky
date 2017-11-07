using System;
using System.Net.Http;

namespace Spooky.XmlRpc
{
	// http://www.cookcomputing.com/xmlrpcsamples/math.rem#math.Add

	/// <summary>
	/// An <see cref="IRpcClient"/> implementation for making XML-RPC calls.
	/// </summary>
	public class XmlRpcHttpClient : RpcClient
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcHttpClient"/> class.
		/// </summary>
		/// <remarks>
		/// <para>See http://xmlrpc.scripting.com/spec.html for details of the XML-RPC spec and common questions.</para>
		/// </remarks>
		/// <param name="serviceAddress">The url of the XML-RPC service this client accesses.</param>
		public XmlRpcHttpClient(Uri serviceAddress) 
			: base
			(
				new RpcClientOptions()
				{
					Serializer = new XmlRpcSerializer(),
					Transport = new XmlRpcHttpTransport(serviceAddress)
				}
			)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcHttpClient"/> class.
		/// </summary>
		/// <param name="serviceAddress">The url of the XML-RPC service this client accesses.</param>
		/// <param name="httpClient">An <see cref="System.Net.Http.HttpClient"/> to use when making HTTP requests.</param>
		public XmlRpcHttpClient(Uri serviceAddress, HttpClient httpClient)
			: base
			(
				new RpcClientOptions()
				{
					Serializer = new XmlRpcSerializer(),
					Transport = new XmlRpcHttpTransport(serviceAddress, httpClient)
				}
			)
		{
		}

	}
}