using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spooky.Json20;

namespace Spooky.Tests
{
	[TestClass]
	public class JsonRpc20Tests
	{
		// Note, tests use a demo 'Jayrock' server. Jayrock is an awesome 
		// Asp.Net/server implementation of Json RPC. https://github.com/atifaziz/Jayrock
		private const string ServiceAddress = "http://www.raboof.com/projects/jayrock/demo.ashx";

		private JsonRpcHttpClient _Client;

		public JsonRpc20Tests()
		{
			System.Net.ServicePointManager.UseNagleAlgorithm = false;
			System.Net.ServicePointManager.Expect100Continue = false;
			
			_Client = new JsonRpcHttpClient(new Uri(ServiceAddress));
		}

		#region InvokeMethod Tests

		[TestMethod]
		public async Task InvokeMethod_WithNoArgs()
		{
			var answer = await _Client.Invoke<int>("counter").ConfigureAwait(false);
			Assert.IsTrue(answer > 0);

			var answer2 = await _Client.Invoke<int>("counter").ConfigureAwait(false);
			Assert.IsTrue(answer2 > answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithSingleStringArg()
		{
			var value = System.Guid.NewGuid().ToString();
			var echo = await _Client.Invoke<string>("echo", value).ConfigureAwait(false);
			Assert.AreEqual(value, echo);
		}

		[TestMethod]
		public async Task InvokeMethod_WithTwoPositionalArgs()
		{
			var answer = await _Client.Invoke<int>("add", 4, 6).ConfigureAwait(false);
			Assert.AreEqual(10, answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithDictionaryNamedArgs()
		{
			var answer = await _Client.Invoke<int>("add", new Dictionary<string, object>() { { "a", 6 }, { "b", 4 } }).ConfigureAwait(false);
			Assert.AreEqual(10, answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithAlternateDictionaryNamedArgs()
		{
			var args = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
			args.TryAdd("a", 4);
			args.TryAdd("b", 6);
			var answer = await _Client.Invoke<int>("add", args).ConfigureAwait(false);
			Assert.AreEqual(10, answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithAnonymousTypeArgs()
		{
			var answer = await _Client.Invoke<int>("add", new { a = 4, b = 6 }).ConfigureAwait(false);
			Assert.AreEqual(10, answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithKeyValuePairEnumerableNamedArgs()
		{
			var args = new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("a", 6),
				new KeyValuePair<string, object>("b", 4)
			};

			var answer = await _Client.Invoke<int>("add", args).ConfigureAwait(false);
			Assert.AreEqual(10, answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithGuidArg()
		{
			var guid = System.Guid.NewGuid();
			var answer = await _Client.Invoke<Guid>("echoGuid", guid).ConfigureAwait(false);
			Assert.AreEqual(guid, answer);
		}

		[ExpectedException(typeof(RpcException))]
		[TestMethod]
		public async Task InvokeMethod_InvalidRequest()
		{
			try
			{
				var answer = await _Client.Invoke<int>("add").ConfigureAwait(false);
				Assert.AreEqual(10, answer);
			}
			catch (RpcException rpcex)
			{
				Assert.AreEqual("add", rpcex.MethodName);
				Assert.IsNotNull(rpcex.RpcError);
				Assert.IsFalse(String.IsNullOrEmpty(rpcex.RpcError.Message));
				throw;
			}
		}

		#endregion

	}
}
