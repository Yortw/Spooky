using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using McStreamy;
using System.Xml;

namespace Spooky.XmlRpc
{
	/// <summary>
	/// Serializer for XML-RPC request and responses.
	/// </summary>
	/// <remarks>
	/// <para>Uses <see cref="System.Text.UTF8Encoding"/> for the text encoding if none or null specified.</para>
	/// <para>Due to limitations of the XML-RPC protocol, only the following data types are supported for arguments and return values.</para>
	/// <list type="Bullet">
	/// <item><see cref="Int32"/></item>
	/// <item><see cref="System.Boolean"/></item>
	/// <item><see cref="System.DateTime"/></item>
	/// <item><see cref="System.String"/></item>
	/// <item><see cref="System.Double"/></item>
	/// <item>Byte arrays/<see cref="IEnumerable{T}"/> of <see cref="System.Byte"/>, or <see cref="System.IO.Stream"/> derived types (as base64 value in XML-RPC protocol).</item>
	/// <item>Arrays/<see cref="IEnumerable{T}"/> where the contained items are classes or structs with properties using only the above data types.</item>
	/// <item>Classes or structs with properties using only the above data types.</item>
	/// </list>
	/// A <see cref="System.InvalidOperationException"/> will be thrown if any other data type is used.
	/// </remarks>
	public class XmlRpcSerializer : IRpcSerializer 
	{

		#region Constants & Fields

		private static readonly object[] EmptyOrdinalArguments = new object[] { };

		private System.Text.Encoding _TextEncoding;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcSerializer"/> class.
		/// </summary>
		public XmlRpcSerializer() : this(new System.Text.UTF8Encoding(false))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRpcSerializer"/> class with a custom text encoding.
		/// </summary>
		/// <param name="textEncoding">The text encoding to use. If null, <see cref="System.Text.UTF8Encoding"/> is used.</param>
		public XmlRpcSerializer(System.Text.Encoding textEncoding)
		{
			_TextEncoding = textEncoding ?? System.Text.UTF8Encoding.UTF8;
		}

		#endregion

		#region IRpcSerializer Members

		/// <summary>
		/// Deserializes a response from the server.
		/// </summary>
		/// <typeparam name="T">The type for the payment of the response.</typeparam>
		/// <param name="serializedData">A stream containing the serialized data from the serve.</param>
		/// <remarks>
		/// <para>If the type {T} is <see cref="System.Xml.Linq.XDocument"/> then the raw response from the server is returned.</para>
		/// <para>XML-RPC 'struct' data types can be deserialised as <see cref="System.Collections.Generic.Dictionary{string, object}"/>, or into a POCO class with read/write properties whose names match exactly those of the members in the XML.</para>
		/// <para>XML-RPC 'array' data types can be deserialised into arrays of objects.</para>
		/// <para><see cref="System.Int32"/>, <see cref="System.String"/>, <see cref="System.DateTime"/>, <see cref="System.Double"/> and <see cref="System.Boolean"/> can all be used as types for deserialising primitive responses.</para>
		/// <para>XML-RPC 'base64' values can be decoded into byte arrays or as <see cref="System.IO.Stream"/>.</para>
		/// </remarks>
		/// <returns>A <see cref="RpcResponse{T}"/> where the result is the payload from the server.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="serializedData"/> is null.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public RpcResponse<T> Deserialize<T>(Stream serializedData) 
		{
			if (serializedData == null) throw new ArgumentNullException(nameof(serializedData));

			var doc = System.Xml.Linq.XDocument.Load(serializedData);
			var responseNode = doc.Descendants("methodResponse");

			var paramNodes = responseNode.Descendants("params").Descendants("param");
			var valueNode = paramNodes.FirstOrDefault()?.Descendants("value")?.FirstOrDefault();

			if (valueNode == null)
			{
				var faultNode = responseNode.Descendants("fault").Descendants("value").Descendants("struct").FirstOrDefault() ??
					throw new RpcException("XML-RPC response is neither a fault nor a value.");

				return DeserializeFaultResponse<T>(faultNode);
			}

			if (typeof(T) == typeof(System.Xml.Linq.XDocument)) return (RpcResponse<T>)(Object)new RpcResponse<XDocument>() { Result = doc };

			var firstDescendant = valueNode.Descendants().First();
			if (firstDescendant.Name == "struct")
				return DeserialiseXmlRpcStruct<T>(firstDescendant);
			else if (firstDescendant.Name == "array")
				return DeserialiseXmlRpcArray<T>(firstDescendant);
			else if (typeof(T) == typeof(string))
				return (RpcResponse<T>)(Object)(new RpcResponse<string>() { Result = valueNode.Value });
			else if (typeof(T) == typeof(Int32))
				return (RpcResponse<T>)(Object)DeserializeIntegerResponse(valueNode);
			else if (typeof(T) == typeof(Boolean))
				return (RpcResponse<T>)(Object)DeserializeBooleanResponse(valueNode);
			else if (typeof(T) == typeof(Double))
				return (RpcResponse<T>)(Object)DeserializeDoubleResponse(valueNode);
			else if (typeof(T) == typeof(DateTime))
				return (RpcResponse<T>)(Object)DeserializeDateTimeResponse(valueNode);
			else if (typeof(T) == typeof(byte[]))
				return (RpcResponse<T>)(Object)new RpcResponse<byte[]>() { Result = DeserializeByteArrayResponse(valueNode) };
			else if (typeof(T) == typeof(IEnumerable<byte>))
				return (RpcResponse<T>)(Object)new RpcResponse<IEnumerable<byte>>() { Result = DeserializeByteArrayResponse(valueNode) };
			else if (typeof(T) == typeof(System.IO.Stream))
				return (RpcResponse<T>)(Object)DeserializeStreamResponse(valueNode);

			throw new InvalidOperationException($"Data type {typeof(T).FullName} is not supported by XML-RPC.");
		}

		/// <summary>
		/// Serializes a <see cref="RpcRequest"/> for transmission to the server.
		/// </summary>
		/// <param name="request">The <see cref="RpcRequest"/> to serialize.</param>
		/// <param name="outputStream">A <see cref="Stream"/> to write the serialized output to.</param>
		/// <remarks>
		/// <para>The <see cref="RpcRequest.Arguments"/> must be one of the following types;
		/// <list type="Bullet">
		/// <item>Dictionary&gt;string, object&lt;</item>
		/// <item>object[]</item>
		/// <item>IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt;</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <returns>A <see cref="Stream"/> containing the serialized content.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public void Serialize(RpcRequest request, System.IO.Stream outputStream)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));

			using (var nonClosingStream = new McStreamy.NonClosingStreamAdapter(outputStream))
			using (var textWriter = new System.IO.StreamWriter(nonClosingStream, _TextEncoding))
			using (var xmlWriter = System.Xml.XmlWriter.Create(textWriter))
			{
				WriteRequest(xmlWriter, request);
				textWriter.Flush();
			}
		}

		#endregion

		#region Private Methods

		#region Serialisation

		private static void WriteRequest(XmlWriter writer, RpcRequest request)
		{
			writer.WriteStartDocument();

			writer.WriteStartElement("methodCall");

			writer.WriteStartElement("methodName");
			writer.WriteValue(request.MethodName);
			writer.WriteEndElement();

			if (request.Arguments != null)
			{
				writer.WriteStartElement("params");

				WriteRequestArgs(writer, request.Arguments);

				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.WriteEndDocument();
		}

		private static void WriteRequestArgs(XmlWriter writer, object arguments)
		{
			var args = arguments as IEnumerable<object>;
			if (args != null)
			{
				foreach (var arg in args)
				{
					if (arg == null) continue;

					writer.WriteStartElement("param");

					writer.WriteStartElement("value");

					WriteArgWithType(writer, arg);

					writer.WriteEndElement();

					writer.WriteEndElement();
				}
			}
			else if (arguments != null)
			{
				writer.WriteStartElement("param");
				writer.WriteStartElement("value");

				var type = arguments.GetType();

				if (typeof(IEnumerable<KeyValuePair<string, object>>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					WriteStruct(writer, (IEnumerable<KeyValuePair<string, object>>)arguments);
				else
					WriteStruct(writer, arguments, type);

				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		private static void WriteArgWithType(XmlWriter writer, object arg)
		{
			switch (arg)
			{
				case Int32 v:
					writer.WriteStartElement("i4");
					writer.WriteValue(arg);
					break;

				case Boolean v:
					writer.WriteStartElement("boolean");
					writer.WriteValue(arg);
					break;

				case Double v:
					writer.WriteStartElement("double");
					writer.WriteValue(arg);
					break;

				case DateTime v:
					writer.WriteStartElement("dateTime.iso8601");
					writer.WriteValue(((DateTime)arg).ToString("s")); // Must be IS08601 format - s
					break;

				case Byte[] v:
					writer.WriteStartElement("base64");
					writer.WriteValue(System.Convert.ToBase64String(v));
					break;

				case IEnumerable<Byte> v:
					writer.WriteStartElement("base64");
					writer.WriteValue(System.Convert.ToBase64String(v.ToArray()));
					break;

				case System.IO.Stream v:
					writer.WriteStartElement("base64");
					using (var ncs = new NonClosingStreamAdapter(v))
					{
						writer.WriteValue(System.Convert.ToBase64String(ncs.ReadAllBytes()));
					}
					break;

				case string v:
					writer.WriteStartElement("string");
					writer.WriteValue(arg);
					break;

				case IEnumerable<object> v:
					WriteArray(writer, v);
					return;

				default:
					if (arg == null) return;

					var argType = arg.GetType();
					if (IsInvalidPrimitive(argType)) throw new InvalidOperationException($"{argType.FullName} is not a valid type for XML-RPC.");

					WriteStruct(writer, arg, argType);
					return;
			}

			writer.WriteEndElement();
		}

		private static void WriteStruct(XmlWriter writer, object arg, Type argType)
		{
			if (arg == null) return;

			writer.WriteStartElement("struct");

			foreach (var prop in argType.GetTypeInfo().DeclaredProperties)
			{
				writer.WriteStartElement("member");

				writer.WriteStartElement("name");
				writer.WriteValue(prop.Name);
				writer.WriteEndElement();

				writer.WriteStartElement("value");
				WriteArgWithType(writer, prop.GetValue(arg));
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		private static void WriteStruct(XmlWriter writer, IEnumerable<KeyValuePair<string, object>> arguments)
		{
			if (arguments == null) return;

			writer.WriteStartElement("struct");

			foreach (var kvp in arguments)
			{
				writer.WriteStartElement("member");

				writer.WriteStartElement("name");
				writer.WriteValue(kvp.Key);
				writer.WriteEndElement();

				writer.WriteStartElement("value");
				WriteArgWithType(writer, kvp.Value);
				writer.WriteEndElement();

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		private static void WriteArray(XmlWriter writer, IEnumerable<object> v)
		{
			writer.WriteStartElement("array");

			writer.WriteStartElement("data");

			foreach (var o in v)
			{
				if (o == null) continue;

				writer.WriteStartElement("value");
				WriteArgWithType(writer, o);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		private static bool IsInvalidPrimitive(Type argType)
		{
			return argType == typeof(decimal)
				|| argType == typeof(DateTimeOffset)
				|| argType == typeof(Int16)
				|| argType == typeof(byte)
				|| argType == typeof(float)
				|| argType == typeof(Int64)
				|| argType == typeof(Char);
		}

		#endregion

		private static RpcResponse<T> DeserialiseXmlRpcArray<T>(XElement arrayNode)
		{
			var elementType = typeof(T).GetElementType() ?? typeof(T).GenericTypeArguments.First();
			var elementsAreLoosleyTyped = elementType == typeof(object);
			var dataNode = arrayNode.Descendants("data");

			if (IsList(typeof(T)))
			{
				System.Collections.IList list = (System.Collections.IList)typeof(T).GetTypeInfo().DeclaredConstructors.First().Invoke(null);
				foreach (var n in dataNode.Descendants())
				{
					if (elementsAreLoosleyTyped)
						list.Add(DeserialiseArgumentValue(n.Value, n.Name.LocalName));
					else
						list.Add(DeserialiseArgumentValue(n.Value, elementType));
				}
				return new RpcResponse<T>()
				{
					Result = (T)(object)list
				};
			}
			else if (typeof(T).IsArray || typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>) || typeof(T) == typeof(System.Collections.IEnumerable))
			{
				var nodes = dataNode.Descendants().ToList();
				Array array = Array.CreateInstance(elementType, nodes.Count);
				for (int index = 0; index < nodes.Count; index++)
				{
					var n = nodes[index];
					if (elementsAreLoosleyTyped)
						array.SetValue(DeserialiseArgumentValue(n.Value, n.Name.LocalName), index);
					else
						array.SetValue(DeserialiseArgumentValue(n.Value, elementType), index);
				}
				return new RpcResponse<T>()
				{
					Result = (T)(object)array
				};
			}

			throw new InvalidOperationException($"Cannot convert XML-RPC array to {typeof(T).FullName}.");
		}

		private static object DeserialiseArgumentValue(string value, string xmlRpcType)
		{
			switch (xmlRpcType)
			{
				case "i4":
				case "integer":
					return DeserialiseArgumentValue<int>(value);

				case "boolean":
					return DeserialiseArgumentValue<bool>(value);

				case "dateTime.iso8601":
					return DeserialiseArgumentValue<DateTime>(value);

				case "double":
					return DeserialiseArgumentValue<double>(value);

				case "base64":
					return DeserialiseArgumentValue<byte[]>(value);

				default:
					return value;
			}
		}

		private static bool IsList(Type type)
		{
			return !type.IsArray &&
				(
					from i
					in type.GetTypeInfo().ImplementedInterfaces
					where (i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>))
					select i
				).Any();
		}

		private static bool IsDictionary(Type type)
		{
			return (from i
				in type.GetTypeInfo().ImplementedInterfaces
				where i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>)
				select i
			).Any();
		}

		private static RpcResponse<T> DeserialiseXmlRpcStruct<T>(XElement structNode)
		{
			if (IsDictionary(typeof(T)))
			{
				return new RpcResponse<T>() { Result = (T)(Object)DeserialiseXmlRpcStructAsDictionary(structNode, typeof(T)) };
			}

			var returnType = typeof(T).GetTypeInfo();
			var retVal = (T)returnType.DeclaredConstructors.FirstOrDefault().Invoke(null);
			var properties = returnType.DeclaredProperties;

			foreach (var property in properties)
			{
				var memberNode = (from n in structNode.Descendants("member") where n.Element("name").Value == property.Name select n).FirstOrDefault();
				if (memberNode == null) continue;

				var name = memberNode.Element("name").Value;
				var valueNode = memberNode.Element("value");
				if (valueNode.Name == "struct")
				{
					//TODO:
				}
				else if (valueNode.Name == "array")
				{
					//TODO:
				}
				else
					returnType.GetDeclaredProperty(name).SetValue(retVal, DeserialiseArgumentValue(valueNode.Value, property.PropertyType));
			}

			return new RpcResponse<T>() { Result = retVal };
		}

		private static System.Collections.IDictionary DeserialiseXmlRpcStructAsDictionary(XElement structNode, Type dictionaryType)
		{
			System.Collections.IDictionary retVal = (System.Collections.IDictionary)dictionaryType.GetTypeInfo().DeclaredConstructors.First().Invoke(null);

			foreach (var node in structNode.Elements("member"))
			{
				var valueNode = node.Element("value").Elements().First();
				retVal.Add(node.Element("name").Value, DeserialiseArgumentValue(valueNode.Value, valueNode.Name.LocalName));
			}
			return retVal;
		}

		private static object DeserialiseArgumentValue(string value, Type propertyType)
		{
			if (propertyType == typeof(string))
				return value;
			else if (propertyType == typeof(Int32))
				return Int32.Parse(value);
			else if (propertyType == typeof(Boolean))
				return Boolean.Parse(value);
			else if (propertyType == typeof(DateTime))
				return DateTime.Parse(value);
			else if (propertyType == typeof(double))
				return Double.Parse(value);
			else if (propertyType == typeof(byte[]) || propertyType == typeof(IEnumerable<byte>))
				return System.Convert.FromBase64String(value);
			else if (typeof(System.IO.Stream).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()))
				return System.Convert.FromBase64String(value).ToStream();

			throw new InvalidOperationException($"{propertyType.FullName} is not a supported type for XML-RPC.");
		}

		private static RpcResponse<T> DeserializeFaultResponse<T>(XElement faultNode)
		{
			return new RpcResponse<T>()
			{
				Error = new RpcError()
				{
					Code =
					(
						from m
						in faultNode.Descendants("member")
						where m.Descendants("name").FirstOrDefault().Value == "faultCode"
						select DeserialiseArgumentValue<int>(m.Descendants("value").Descendants("i4").FirstOrDefault().Value)
					).FirstOrDefault(),
					Message =
					(
						from m
						in faultNode.Descendants("member")
						where m.Descendants("name").FirstOrDefault().Value == "faultString"
						select DeserialiseArgumentValue<string>(m.Descendants("value").Descendants("string").FirstOrDefault().Value)
					).FirstOrDefault()
				}
			};
		}

		private static RpcResponse<int> DeserializeIntegerResponse(XElement node)
		{
			return new RpcResponse<int>()
			{
				Result = Int32.Parse(node.Value)
			};
		}

		private static RpcResponse<bool> DeserializeBooleanResponse(XElement node)
		{
			return new RpcResponse<bool>()
			{
				Result = Boolean.Parse(node.Value)
			};
		}

		private static RpcResponse<double> DeserializeDoubleResponse(XElement node)
		{
			return new RpcResponse<double>()
			{
				Result = Double.Parse(node.Value)
			};
		}

		private static RpcResponse<DateTime> DeserializeDateTimeResponse(XElement node)
		{
			return new RpcResponse<DateTime>()
			{
				Result = DateTime.Parse(node.Value)
			};
		}

		private static byte[] DeserializeByteArrayResponse(XElement valueNode)
		{
			return System.Convert.FromBase64String(valueNode.Value);
		}

		private static RpcResponse<System.IO.Stream> DeserializeStreamResponse(XElement valueNode)
		{
			return new RpcResponse<System.IO.Stream>()
			{
				Result = DeserializeByteArrayResponse(valueNode).ToStream()
			};
		}

		private static T DeserialiseArgumentValue<T>(string value)
		{
			if (typeof(T) == typeof(string)) return (T)(object)value;

			else if (typeof(T) == typeof(Int32))
			{
				Int32.TryParse(value, out int iRetVal);
				return (T)(object)iRetVal;
			}
			else if (typeof(T) == typeof(bool))
			{
				Boolean.TryParse(value, out bool iRetVal);
				return (T)(object)iRetVal;
			}
			else if (typeof(T) == typeof(double))
			{
				Double.TryParse(value, out double iRetVal);
				return (T)(object)iRetVal;
			}
			else if (typeof(T) == typeof(DateTime))
			{
				DateTime.TryParse(value, out DateTime iRetVal);
				return (T)(object)iRetVal;
			}
			else if (typeof(T) == typeof(byte[]))
			{
				return (T)(object)System.Convert.FromBase64String(value);
			}

			throw new InvalidOperationException($"The type {typeof(T).FullName} is not supported in this context.");
		}

		#endregion

	}
}
 