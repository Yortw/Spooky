using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Spooky.XmlRpc;

namespace Spooky.Tests
{
	[TestClass]
	public class XmlRpcIntegrationTests
	{
		private const string MathServiceAddress = "http://cookcomputing.com/xmlrpcsamples/math.rem";
		private const string StateServiceAddress = "http://cookcomputing.com/xmlrpcsamples/RPC2.ashx";

		private XmlRpcHttpClient _MathClient;
		private XmlRpcHttpClient _StateClient;

		public XmlRpcIntegrationTests()
		{
			System.Net.ServicePointManager.UseNagleAlgorithm = false;
			System.Net.ServicePointManager.Expect100Continue = false;

			_MathClient = new XmlRpcHttpClient(new Uri(MathServiceAddress));
			_StateClient = new XmlRpcHttpClient(new Uri(StateServiceAddress));
		}

		#region InvokeMethod Tests

		[ExpectedException(typeof(Spooky.RpcException))]
		[TestMethod]
		public async Task InvokeMethod_WithErrorResponse()
		{
			try
			{
				var answer = await _MathClient.Invoke<int>("notavalidmethod", 4, 6).ConfigureAwait(false);
			}
			catch (Spooky.RpcException ex)
			{
				Assert.IsNotNull(ex.RpcError);
				Assert.AreEqual(0, ex.RpcError.Code);
				Assert.AreEqual("unsupported method called: notavalidmethod", ex.RpcError.Message);
				throw;
			}
		}

		[TestMethod]
		public async Task InvokeMethod_WithTwoPositionalIntegerArgsAndIntegerResponse()
		{
			var answer = await _MathClient.Invoke<int>("math.Add", 4, 6).ConfigureAwait(false);
			Assert.AreEqual(10, answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithIntegerArgAndStringResponse()
		{
			var answer = await _StateClient.Invoke<string>("examples.getStateName", 25).ConfigureAwait(false);
			Assert.IsNotNull(answer);
			Assert.AreEqual("Missouri", answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithStructArgAndStringResponse()
		{
			var answer = await _StateClient.Invoke<string>("examples.getStateNames", new { State1 = 1, State2 = 25, State3 = 50 }).ConfigureAwait(false);
			Assert.IsNotNull(answer);
			Assert.AreEqual("Alabama,Missouri,Wyoming", answer);
		}

		[TestMethod]
		public async Task InvokeMethod_WithStructArgAndResponse()
		{
			var answer = await _StateClient.Invoke<StateResponse>("examples.getStateStructResponse", new { State1 = 1, State2 = 25, State3 = 50 }).ConfigureAwait(false);

			Assert.IsNotNull(answer);
			Assert.AreEqual("Alabama", answer.State1);
			Assert.AreEqual("Missouri", answer.State2);
			Assert.AreEqual("Wyoming", answer.State3);
		}

		[TestMethod]
		public async Task InvokeMethod_WithDictionaryNamedArgs()
		{
			var answer = await _StateClient.Invoke<StateResponse>("examples.getStateStructResponse", new Dictionary<string, object>() { { "State1", 1 }, { "State2", 25 }, { "State3", 50 } }).ConfigureAwait(false);
			Assert.IsNotNull(answer);
			Assert.AreEqual("Alabama", answer.State1);
			Assert.AreEqual("Missouri", answer.State2);
			Assert.AreEqual("Wyoming", answer.State3);
		}

		//[TestMethod]
		//public async Task InvokeMethod_WithKeyValuePairEnumerableNamedArgs()
		//{
		//	var args = new KeyValuePair<string, object>[]
		//	{
		//		new KeyValuePair<string, object>("a", 6),
		//		new KeyValuePair<string, object>("b", 4)
		//	};

		//	var answer = await _Client.Invoke<int>("add", args).ConfigureAwait(false);
		//	Assert.AreEqual(10, answer);
		//}

		#endregion

		internal class StateResponse
		{
			public string State1 { get; set; }
			public string State2 { get; set; }
			public string State3 { get; set; }
		}

	}
}