using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooky.Tests
{
	[TestClass]
	public class RpcClientTests
	{

		#region Constructor Test

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void RpcClient_Constructor_ThrowsOnNullOptions()
		{
			var client = new RpcClient(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void RpcClient_Constructor_ThrowsOnNullSerializer()
		{
			var client = new RpcClient(new RpcClientOptions() { Transport = new Json20.JsonRpcHttpTransport(new Uri("http://somewhere.com"))});
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void RpcClient_Constructor_ThrowsOnNullTransport()
		{
			var client = new RpcClient(new RpcClientOptions() { Serializer = new Json20.JsonRpcSerializer()});
		}

		[TestMethod]
		public void RpcClient_Constructor_ConstructsWithGoodArguments()
		{
			var client = new RpcClient(new RpcClientOptions()
			{
				Transport = new Json20.JsonRpcHttpTransport(new Uri("http://somewhere.com")),
				Serializer = new Json20.JsonRpcSerializer() }
			);
		}

		#endregion

	}
}