using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spooky.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Spooky.Tests
{
	[TestClass]
	public class XmlRpcSerializerTests
	{

		[TestMethod]
		public void XmlRpcSerializer_Constructor_ConstructsOk()
		{
			var serializer = new XmlRpcSerializer();
		}

		[TestMethod]
		public void XmlRpcSerializer_Constructor_ConstructsOkWithNullSettings()
		{
			var serializer = new XmlRpcSerializer(null);
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializeRequest_DoesNotWriteBomByDefault()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { 1 };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				Assert.AreEqual(155, stream.Length);

				stream.Seek(0, System.IO.SeekOrigin.Begin);
				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><i4>1</i4></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesInt32Argument()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { 1 };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><i4>1</i4></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesStringArgument()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><string>testValue</string></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesBooleanArgument()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { true };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><boolean>true</boolean></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesDoubleArgument()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { 3.1459D };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><double>3.1459</double></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesDateTimeArgument()
		{
			var d = DateTime.Now;
			var serializer = new XmlRpcSerializer();
			var args = new object[] { d };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual($"<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><dateTime.iso8601>{d.ToString("s")}</dateTime.iso8601></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesByteArrayArgument()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { new byte[] { 65, 66, 67 } };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><base64>QUJD</base64></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesEnumerableBytesArgument()
		{
			var serializer = new XmlRpcSerializer();
			var bytes = new List<byte>(4) { 65, 66, 67 };
			var args = new object[] { bytes };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><base64>QUJD</base64></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesStreamBytesArgument()
		{
			var serializer = new XmlRpcSerializer();
			var ms = new System.IO.MemoryStream(4);
			ms.Write(new byte[] { 65, 66, 67 }, 0, 3);
			ms.Seek(0, System.IO.SeekOrigin.Begin);

			var args = new object[] { ms };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><base64>QUJD</base64></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesWithoutParamsNodeWhenArgsNull()
		{
			var serializer = new XmlRpcSerializer();
			var ms = new System.IO.MemoryStream(4);
			ms.Write(new byte[] { 65, 66, 67 }, 0, 3);
			ms.Seek(0, System.IO.SeekOrigin.Begin);

			var args = new object[] { ms };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = null,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesWithEmptyParamsNodeWhenArgsEmpty()
		{
			var serializer = new XmlRpcSerializer();
			var ms = new System.IO.MemoryStream(4);
			ms.Write(new byte[] { 65, 66, 67 }, 0, 3);
			ms.Seek(0, System.IO.SeekOrigin.Begin);

			var args = new object[] { ms };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new object[] { },
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params /></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesObjectAsStruct()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var created = DateTime.Parse("2017-11-06T21:06:25");
			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new
				{
					Name = "Rod Galloglass",
					Age = 32,
					Class = "Wizard",
					Created = created
				}
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><struct><member><name>Name</name><value><string>Rod Galloglass</string></value></member><member><name>Age</name><value><i4>32</i4></value></member><member><name>Class</name><value><string>Wizard</string></value></member><member><name>Created</name><value><dateTime.iso8601>2017-11-06T21:06:25</dateTime.iso8601></value></member></struct></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesObjectWithNestedObjectAndArrayAsStruct()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var created = DateTime.Parse("2017-11-06T21:06:25");
			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new Relationship
				{
					RelationshipType = "Marriage",
					Character1 = new Character()
					{
						Name = "Rod Galloglass",
						Age = 32,
						Created = created
					},
					Character2 = new Character()
					{
						Name = "Gwen Galloglass",
						Age = 29,
						Created = created
					},
					RandomTestData = new object[]
					{
						4,
						true,
						"test"
					}
				}
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><struct><member><name>RelationshipType</name><value><string>Marriage</string></value></member><member><name>Character1</name><value><struct><member><name>Name</name><value><string>Rod Galloglass</string></value></member><member><name>Age</name><value><i4>32</i4></value></member><member><name>Created</name><value><dateTime.iso8601>2017-11-06T21:06:25</dateTime.iso8601></value></member><member><name>IsTrue</name><value><boolean>false</boolean></value></member><member><name>Score</name><value><double>0</double></value></member><member><name>ProfileData</name><value /></member><member><name>ProfileStream</name><value /></member><member><name>APropertyThatIsNotDeserialised</name><value /></member></struct></value></member><member><name>Character2</name><value><struct><member><name>Name</name><value><string>Gwen Galloglass</string></value></member><member><name>Age</name><value><i4>29</i4></value></member><member><name>Created</name><value><dateTime.iso8601>2017-11-06T21:06:25</dateTime.iso8601></value></member><member><name>IsTrue</name><value><boolean>false</boolean></value></member><member><name>Score</name><value><double>0</double></value></member><member><name>ProfileData</name><value /></member><member><name>ProfileStream</name><value /></member><member><name>APropertyThatIsNotDeserialised</name><value /></member></struct></value></member><member><name>RandomTestData</name><value><array><data><value><i4>4</i4></value><value><boolean>true</boolean></value><value><string>test</string></value></data></array></value></member></struct></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesChildArray()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var created = DateTime.Parse("2017-11-06T21:06:25");
			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new object[] { new object[]
				{
					"Rod Galloglass",
					32,
					"Wizard",
					created
				}
				}
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual($"<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><array><data><value><string>Rod Galloglass</string></value><value><i4>32</i4></value><value><string>Wizard</string></value><value><dateTime.iso8601>{created.ToString("s")}</dateTime.iso8601></value></data></array></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_SerializesChildObject()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var created = DateTime.Parse("2017-11-06T21:06:25");
			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new object[]
				{
					new
					{
						Name = "Rod Galloglass",
						Age = 32,
						Class = "Wizard",
						Created = created
					}
				}
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual($"<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><struct><member><name>Name</name><value><string>Rod Galloglass</string></value></member><member><name>Age</name><value><i4>32</i4></value></member><member><name>Class</name><value><string>Wizard</string></value></member><member><name>Created</name><value><dateTime.iso8601>{created.ToString("s")}</dateTime.iso8601></value></member></struct></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_Serialize_SkipsNullArgument()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var created = DateTime.Parse("2017-11-06T21:06:25");
			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = null
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_Serialize_SkipsNullArgumentInArray()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new object[] { 4, null, 5 }
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><i4>4</i4></value></param><param><value><i4>5</i4></value></param></params></methodCall>", actual);
				}
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_Serialize_SkipsNullArgumentInObject()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new { Arg1 = 4, Arg2 = (int?)null, Arg3 = 5 }
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
				stream.Seek(0, System.IO.SeekOrigin.Begin);

				using (var reader = new System.IO.StreamReader(stream))
				{
					var actual = reader.ReadToEnd();
					Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><methodCall><methodName>testmethod</methodName><params><param><value><struct><member><name>Arg1</name><value><i4>4</i4></value></member><member><name>Arg2</name><value /></member><member><name>Arg3</name><value><i4>5</i4></value></member></struct></value></param></params></methodCall>", actual);
				}
			}
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void XmlRpcSerializer_Serialize_ThrowsOnNullRequest()
		{
			var serializer = new XmlRpcSerializer();

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(null, stream);
			}
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void XmlRpcSerializer_Serialize_ThrowsOnNullStream()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { "testValue" };

			var created = DateTime.Parse("2017-11-06T21:06:25");
			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = new object[] { new object[]
				{
					"Rod Galloglass",
					32,
					"Wizard",
					created
				}
				}
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, null);
			}
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void XmlRpcSerializer_Serialize_ThrowsOnUnsupportedDataType()
		{
			var serializer = new XmlRpcSerializer();
			var args = new object[] { 3.1459M };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
			}
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void XmlRpcSerializer_Serialize_ThrowsOnUnsupportedChildDataType()
		{
			var serializer = new XmlRpcSerializer();
			var args = new { Value = 3.1459M };

			var request = new RpcRequest()
			{
				MethodName = "testmethod",
				Arguments = args,
			};

			using (var stream = new System.IO.MemoryStream())
			{
				serializer.Serialize(request, stream);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesXDocumentAsRawResponse()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("i4", 4)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<System.Xml.Linq.XDocument>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("<methodResponse><params><param><value><i4>4</i4></value></param></params></methodResponse>", response.Result.ToString(SaveOptions.DisableFormatting));
			}
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void XmlRpcSerializer_ThrowsOnUnsupportedType()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("i4", 4)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<decimal>(ms);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesIntegerResponse()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("i4", 4)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<int>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(4, response.Result);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesStringResponse()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("string", "test string")
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<string>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string", response.Result);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesBooleanResponse()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("boolean", "True")
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<bool>(ms);
				Assert.IsNull(response.Error);
				Assert.IsTrue(response.Result);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesDoubleResponse()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("double", 1.54321D)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<double>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(1.54321D, response.Result);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesDateTimeResponse()
		{
			var now = DateTime.Parse(DateTime.Now.ToString("s"));

			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("dateTime.iso8601", now.ToString("s"))
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<DateTime>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(now, response.Result);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesBase64ResponseAsByteArray()
		{
			var now = DateTime.Parse(DateTime.Now.ToString("s"));

			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("base64", "QUJD")
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<byte[]>(ms);
				Assert.IsNotNull(response.Result);
				Assert.AreEqual(3, response.Result.Length);
				Assert.AreEqual(65, response.Result[0]);
				Assert.AreEqual(66, response.Result[1]);
				Assert.AreEqual(67, response.Result[2]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesBase64ResponseAsStream()
		{
			var now = DateTime.Parse(DateTime.Now.ToString("s"));

			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("base64", "QUJD")
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<System.IO.Stream>(ms);
				Assert.IsNotNull(response.Result);
				Assert.AreEqual(3, response.Result.Length);
				var buffer = new byte[3];
				response.Result.Read(buffer, 0, 3);
				Assert.AreEqual(65, buffer[0]);
				Assert.AreEqual(66, buffer[1]);
				Assert.AreEqual(67, buffer[2]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesBase64ResponseAsEnumerableBytes()
		{
			var now = DateTime.Now;

			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("base64", "QUJD")
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<IEnumerable<byte>>(ms);
				var bytes = response.Result.ToList();

				Assert.IsNull(response.Error);
				Assert.AreEqual(65, bytes[0]);
				Assert.AreEqual(66, bytes[1]);
				Assert.AreEqual(67, bytes[2]);
			}
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void XmlRpcSerializer_Deserialize_ThrowsOnUnsupportedType()
		{
			var now = DateTime.Parse(DateTime.Now.ToString("s"));

			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("double", 3.1459)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<decimal>(ms);
			}
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayThrowsOnUnsupportedType()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("string", "test string 1"),
										new XElement("string", "test string 2"),
										new XElement("string", "test string 3"),
										new XElement("string", "test string 4")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<Stack<object>>(ms);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsStringArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("string", "test string 1"),
										new XElement("string", "test string 2"),
										new XElement("string", "test string 3"),
										new XElement("string", "test string 4")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<string[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string 1", response.Result[0]);
				Assert.AreEqual("test string 2", response.Result[1]);
				Assert.AreEqual("test string 3", response.Result[2]);
				Assert.AreEqual("test string 4", response.Result[3]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsDoubleArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("double", 1.5D),
										new XElement("double", 0.8D),
										new XElement("double", 2.1D),
										new XElement("double", 12.5D)
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<double[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(1.5D, response.Result[0]);
				Assert.AreEqual(0.8D, response.Result[1]);
				Assert.AreEqual(2.1D, response.Result[2]);
				Assert.AreEqual(12.5D, response.Result[3]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsIntegerArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("i4", 1),
										new XElement("i4", 2),
										new XElement("i4", 3),
										new XElement("i4", 4)
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<double[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(1, response.Result[0]);
				Assert.AreEqual(2, response.Result[1]);
				Assert.AreEqual(3, response.Result[2]);
				Assert.AreEqual(4, response.Result[3]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsByteArrayArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("base64", "QUJD"),
										new XElement("base64", "QUJD"),
										new XElement("base64", "QUJD"),
										new XElement("base64", "QUJD")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<byte[][]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(3, response.Result[0].Length);
				Assert.AreEqual(3, response.Result[1].Length);
				Assert.AreEqual(3, response.Result[2].Length);
				Assert.AreEqual(3, response.Result[3].Length);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsStreamArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("base64", "QUJD"),
										new XElement("base64", "QUJD"),
										new XElement("base64", "QUJD"),
										new XElement("base64", "QUJD")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<System.IO.Stream[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(3, response.Result[0].Length);
				Assert.AreEqual(3, response.Result[1].Length);
				Assert.AreEqual(3, response.Result[2].Length);
				Assert.AreEqual(3, response.Result[3].Length);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsBooleanArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("boolean", true),
										new XElement("boolean", true),
										new XElement("boolean", false),
										new XElement("boolean", true)
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<bool[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(true, response.Result[0]);
				Assert.AreEqual(true, response.Result[1]);
				Assert.AreEqual(false, response.Result[2]);
				Assert.AreEqual(true, response.Result[3]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsDateTimeArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("dateTime.iso8601", new DateTime(1970, 01, 01)),
										new XElement("dateTime.iso8601", new DateTime(1980, 12, 25)),
										new XElement("dateTime.iso8601", new DateTime(2017, 11, 07)),
										new XElement("dateTime.iso8601", new DateTime(2000, 01, 01))
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<DateTime[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual(DateTime.Parse(new DateTime(1970, 01, 01).ToString("s")), response.Result[0]);
				Assert.AreEqual(DateTime.Parse(new DateTime(1980, 12, 25).ToString("s")), response.Result[1]);
				Assert.AreEqual(DateTime.Parse(new DateTime(2017, 11, 07).ToString("s")), response.Result[2]);
				Assert.AreEqual(DateTime.Parse(new DateTime(2000, 01, 01).ToString("s")), response.Result[3]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsTypedList()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("string", "test string 1"),
										new XElement("string", "test string 2"),
										new XElement("string", "test string 3"),
										new XElement("string", "test string 4")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<List<string>>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string 1", response.Result[0]);
				Assert.AreEqual("test string 2", response.Result[1]);
				Assert.AreEqual("test string 3", response.Result[2]);
				Assert.AreEqual("test string 4", response.Result[3]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsObjectList()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("string", "test string 1"),
										new XElement("i4", 1),
										new XElement("dateTime.iso8601", new DateTime(2017, 11, 7)),
										new XElement("double", 3.1459),
										new XElement("boolean", true),
										new XElement("base64", "QUJD")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<List<object>>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string 1", response.Result[0]);
				Assert.AreEqual(1, response.Result[1]);
				Assert.AreEqual(DateTime.Parse(new DateTime(2017, 11, 7).ToString("s")), response.Result[2]);
				Assert.AreEqual(3.1459, response.Result[3]);
				Assert.AreEqual(true, response.Result[4]);
				Assert.IsNotNull(response.Result[5]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesArrayAsObjectArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("array",
									new XElement("data",
										new XElement("string", "test string 1"),
										new XElement("i4", 42),
										new XElement("dateTime.iso8601", new DateTime(2017, 11, 06, 23, 00, 02)),
										new XElement("boolean", true),
										new XElement("double", 3.1459),
										new XElement("base64", "QUJD")
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<object[]>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string 1", response.Result[0]);
				Assert.AreEqual(42, response.Result[1]);
				Assert.AreEqual(DateTime.Parse(new DateTime(2017, 11, 06, 23, 00, 02).ToString("s")), response.Result[2]);
				Assert.AreEqual(true, response.Result[3]);
				Assert.AreEqual(3.1459D, response.Result[4]);
				Assert.IsNotNull(response.Result[5]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesStructAsObjectDictionary()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("struct",
									new XElement
									(
										"member",
										new XElement("name", "name"),
										new XElement("value", new XElement("string", "test string 1"))
									),
									new XElement
									(
										"member",
										new XElement("name", "age"),
										new XElement("value", new XElement("i4", 42))
									),
									new XElement
									(
										"member",
										new XElement("name", "IsTrue"),
										new XElement("value", new XElement("boolean", true))
									),
									new XElement
									(
										"member",
										new XElement("name", "created"),
										new XElement("value", new XElement("dateTime.iso8601", new DateTime(2017, 11, 06, 23, 00, 02)))
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<Dictionary<string, object>>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string 1", response.Result["name"]);
				Assert.AreEqual(42, response.Result["age"]);
				Assert.AreEqual(DateTime.Parse(new DateTime(2017, 11, 06, 23, 00, 02).ToString("s")), response.Result["created"]);
				Assert.AreEqual(true, response.Result["IsTrue"]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesStructAsTypedDictionary()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("struct",
									new XElement
									(
										"member",
										new XElement("name", "name"),
										new XElement("value", new XElement("string", "Rod Galloglass"))
									),
									new XElement
									(
										"member",
										new XElement("name", "alias"),
										new XElement("value", new XElement("string", "The Warlock"))
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<Dictionary<string, string>>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("Rod Galloglass", response.Result["name"]);
				Assert.AreEqual("The Warlock", response.Result["alias"]);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesStructAsObjectInstance()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("struct",
									new XElement
									(
										"member",
										new XElement("name", "Name"),
										new XElement("value", new XElement("string", "test string 1"))
									),
									new XElement
									(
										"member",
										new XElement("name", "Age"),
										new XElement("value", new XElement("i4", 42))
									),
									new XElement
									(
										"member",
										new XElement("name", "IsTrue"),
										new XElement("value", new XElement("boolean", true))
									),
									new XElement
									(
										"member",
										new XElement("name", "Score"),
										new XElement("value", new XElement("double", 125.6D))
									),
									new XElement
									(
										"member",
										new XElement("name", "ProfileData"),
										new XElement("value", new XElement("base64", System.Convert.ToBase64String(new byte[] { 65, 66, 67 })))
									),
									new XElement
									(
										"member",
										new XElement("name", "ProfileStream"),
										new XElement("value", new XElement("base64", System.Convert.ToBase64String(new byte[] { 65, 66, 67 })))
									),
									new XElement
									(
										"member",
										new XElement("name", "Created"),
										new XElement("value", new XElement("dateTime.iso8601", new DateTime(2017, 11, 06, 23, 00, 02)))
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<Character>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("test string 1", response.Result.Name);
				Assert.AreEqual(42, response.Result.Age);
				Assert.AreEqual(DateTime.Parse(new DateTime(2017, 11, 06, 23, 00, 02).ToString("s")), response.Result.Created);
				Assert.AreEqual(125.6D, response.Result.Score);
				Assert.IsNotNull(response.Result.ProfileData);
				Assert.IsNotNull(response.Result.ProfileStream);
				Assert.IsNull(response.Result.APropertyThatIsNotDeserialised);
				Assert.AreEqual(true, response.Result.IsTrue);
			}
		}

		[TestMethod]
		public void XmlRpcSerializer_DeserializesStructWithSubStructAndArray()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param",
							new XElement("value",
								new XElement("struct",
									new XElement
									(
										"member",
										new XElement("name", "RelationshipType"),
										new XElement("value", new XElement("string", "Marriage"))
									),
									new XElement
									(
										"member",
										new XElement("name", "Character1"),
										new XElement("value",
											new XElement("struct",
												new XElement("member",
													new XElement("name", "Name"),
													new XElement("value", new XElement("string", "Rod Gallowglass"))
												),
												new XElement("member",
													new XElement("name", "Age"),
													new XElement("value", new XElement("i4", 32))
												)
											)
										)
									),
									new XElement
									(
										"member",
										new XElement("name", "Character2"),
										new XElement("value", 
											new XElement("struct",
												new XElement("member",
													new XElement("name", "Name"),
													new XElement("value", new XElement("string", "Gwen Gallowglass"))
												),
												new XElement("member",
													new XElement("name", "Age"),
													new XElement("value", new XElement("i4", 29))
												)
											)
										)
									),
									new XElement("member",
										new XElement("name", "RandomTestData"),
										new XElement("value",
											new XElement("array",
												new XElement("data",
													new XElement("i4", 4),
													new XElement("boolean", true),
													new XElement("string", "test")
												)
											)
										)
									)
								)
							)
						)
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<Relationship>(ms);
				Assert.IsNull(response.Error);
				Assert.AreEqual("Marriage", response.Result.RelationshipType);
				Assert.AreEqual("Rod Gallowglass", response.Result.Character1.Name);
				Assert.AreEqual(32, response.Result.Character1.Age);
				Assert.AreEqual("Gwen Gallowglass", response.Result.Character2.Name);
				Assert.AreEqual(29, response.Result.Character2.Age);

				Assert.AreEqual(4, response.Result.RandomTestData[0]);
				Assert.AreEqual(true, response.Result.RandomTestData[1]);
				Assert.AreEqual("test", response.Result.RandomTestData[2]);
			}
		}

		[ExpectedException(typeof(RpcException))]
		[TestMethod]
		public void XmlRpcSerializer_Deserialize_ThrowsOnNoValueOrFault()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse")
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<bool>(ms);
			}
		}

		[ExpectedException(typeof(RpcException))]
		[TestMethod]
		public void XmlRpcSerializer_Deserialize_ThrowsOnEmptyValue()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params",
						new XElement("param")
					)
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<bool>(ms);
			}
		}

		[ExpectedException(typeof(RpcException))]
		[TestMethod]
		public void XmlRpcSerializer_Deserialize_ThrowsOnEmptyParams()
		{
			var responseDoc = new XDocument
			(
				new XElement("methodResponse",
					new XElement("params")
				)
			);
			var responseString = responseDoc.ToString();

			var serializer = new XmlRpcSerializer();
			using (var ms = new System.IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(responseString)))
			{
				var response = serializer.Deserialize<bool>(ms);
			}
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void XmlRpcSerializer_ThrowsOnNullData()
		{
			var serializer = new XmlRpcSerializer();
			var response = serializer.Deserialize<Character>(null);
		}


		private class Character
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public DateTime Created { get; set; }
			public bool IsTrue { get; set; }
			public double Score { get; set; }
			public byte[] ProfileData { get; set; }
			public System.IO.Stream ProfileStream { get; set; }

			public object APropertyThatIsNotDeserialised { get; set; }
		}

		private class Relationship
		{
			public string RelationshipType { get; set; }
			public Character Character1 { get; set; }
			public Character Character2 { get; set; }

			public object[] RandomTestData { get; set; }
		}
	}

}