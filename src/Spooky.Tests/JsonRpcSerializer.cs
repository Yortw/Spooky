using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooky.Tests
{
	[TestClass]
	public class JsonRpcSerializerTests
	{

		[TestMethod]
		public void JsonRpcSerializer_Constructor_ConstructsOkWithNullEncoding()
		{
			var serializer = new Json20.JsonRpcSerializer(null);
		}

		[TestMethod]
		public void JsonRpcSerializer_Constructor_ConstructsOkWithNullSettings()
		{
			var serializer = new Json20.JsonRpcSerializer(null, null);
		}

		[TestMethod]
		public void JsonRpcSerializer_SerializesRequest()
		{
			var serializer = new Json20.JsonRpcSerializer();
			var args = new Dictionary<String, object>();
			args.Add("TestArg", 1);

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			Json20.JsonRpcRequestIdGenerator.ResetId();
			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"testmethod\",\"params\":{\"TestArg\":1}}", reader.ReadToEnd());
				}
			}
		}
		
		[TestMethod]
		public void JsonRpcSerializer_SerializeRequest_DoesNotWriteBomByDefault()
		{
			var serializer = new Json20.JsonRpcSerializer();
			var args = new Dictionary<String, object>();
			args.Add("TestArg", 1);

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			Json20.JsonRpcRequestIdGenerator.ResetId();
			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				Assert.AreEqual(69, stream.Length);

				Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"testmethod\",\"params\":{\"TestArg\":1}}", System.Text.UTF8Encoding.UTF8.GetString(stream.ToArray()));
			}
		}

		[TestMethod]
		public void JsonRpcSerializer_DeserializesResponse()
		{
			var response = new RpcResponse<int>()
			{
				Result = 1
			};
			var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);

			var serializer = new Json20.JsonRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var deserializedResponse = serializer.Deserialize<int>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(1, response.Result);
			}
		}

	}
}