using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooky.Tests
{
	[TestClass]
	public class HttpClientTransportTests
	{

		#region Constructor Tests

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void HttpClientTransport_Constructor_ThrowsOnNullServiceAddress()
		{
			var client = new HttpClientTransport(null, Json20.JsonRpcMediaTypes.ApplicationJsonRpc, "utf-8");
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void HttpClientTransport_Constructor_ThrowsOnNullClient()
		{
			var client = new HttpClientTransport(new Uri("http://somewhere.com"), Json20.JsonRpcMediaTypes.ApplicationJsonRpc, "utf-8", null);
		}

		[TestMethod]
		public void HttpClientTransport_Constructor_AllowsNullMediaType()
		{
			var client = new HttpClientTransport(new Uri("http://somewhere.com"), null, "utf-8");
		}

		[TestMethod]
		public void HttpClientTransport_Constructor_AllowsNullCharSetType()
		{
			var client = new HttpClientTransport(new Uri("http://somewhere.com"), Json20.JsonRpcMediaTypes.ApplicationJsonRpc, null);
		}

		#endregion

	}
}