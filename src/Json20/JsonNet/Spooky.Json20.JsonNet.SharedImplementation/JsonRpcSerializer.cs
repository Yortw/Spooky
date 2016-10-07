using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spooky.Json20
{
	/// <summary>
	/// Serializer for Json RPC 2.0 request and responses.
	/// </summary>
	/// <remarks>
	/// <para>Uses <see cref="System.Text.UTF8Encoding"/> for the text encoding if none or null specified.</para>
	/// </remarks>
	public class JsonRpcSerializer : IRpcSerializer
	{

		private static readonly object[] EmptyOrdinalArguments = new object[] { };

		private System.Text.Encoding _TextEncoding;
		private Newtonsoft.Json.JsonSerializerSettings _Settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonRpcSerializer"/> class.
		/// </summary>
		public JsonRpcSerializer() : this(System.Text.UTF8Encoding.UTF8)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonRpcSerializer"/> class with a custom text encoding.
		/// </summary>
		/// <param name="textEncoding">The text encoding to use. If null, <see cref="System.Text.UTF8Encoding"/> is used.</param>
		public JsonRpcSerializer(System.Text.Encoding textEncoding) : this(textEncoding, null)
		{
			_TextEncoding = textEncoding ?? System.Text.UTF8Encoding.UTF8;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonRpcSerializer"/> class with a custom text encoding.
		/// </summary>
		/// <param name="textEncoding">The text encoding to use. If null, <see cref="System.Text.UTF8Encoding"/> is used.</param>
		/// <param name="settings">Settings for the Json.Net serialiser, allows controlling things like date formats. Provide null for default settings.</param>
		public JsonRpcSerializer(System.Text.Encoding textEncoding, Newtonsoft.Json.JsonSerializerSettings settings) 
		{
			_TextEncoding = textEncoding ?? System.Text.UTF8Encoding.UTF8;
			_Settings = settings;
		}
		

		/// <summary>
		/// Deserializes a response from the server.
		/// </summary>
		/// <typeparam name="T">The type for the payment of the response.</typeparam>
		/// <param name="serializedData">A stream containing the serialized data from the serve.</param>
		/// <returns>A <see cref="RpcResponse{T}"/> where the result is the payload from the server.</returns>
		public RpcResponse<T> Deserialize<T>(Stream serializedData)
		{
			using (var reader = new System.IO.StreamReader(serializedData))
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<RpcResponse<T>>(reader.ReadToEnd());
			}
		}

		/// <summary>
		/// Serializes a <see cref="RpcRequest"/> for transmission to the server.
		/// </summary>
		/// <param name="request">The <see cref="RpcRequest"/> to serialize.</param>
		/// <remarks>
		/// <para>The <see cref="RpcRequest.Arguments"/> must be one of the following types;
		/// <list type="Bullet">
		/// <item>Dictionary&gt;string, object&lt;</item>
		/// <item>object[]</item>
		/// <item>IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt;</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <returns>A <see cref="System.IO.Stream"/> containing the serialized content.</returns>
		public Stream Serialize(RpcRequest request)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var argumentsType = request.Arguments?.GetType();

			object jsonRpcRequest = null;
			if (argumentsType == null || argumentsType == typeof(object[]))
			{
				jsonRpcRequest = new JsonRpcRequest<object[]>()
				{
					MethodName = request.MethodName,
					Arguments = (object[])request.Arguments ?? EmptyOrdinalArguments
				};
			}
			else if (argumentsType == typeof(Dictionary<string, object>))
			{
				jsonRpcRequest = new JsonRpcRequest<IEnumerable<KeyValuePair<string, object>>>()
				{
					MethodName = request.MethodName,
					Arguments = (IEnumerable<KeyValuePair<string, object>>)request.Arguments
				};
			}
			else if (typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(argumentsType))
			{
				var argsDict = new Dictionary<string, object>();
				foreach (var kvp in (IEnumerable<KeyValuePair<string, object>>)request.Arguments)
				{
					argsDict.Add(kvp.Key, kvp.Value);
				}

				jsonRpcRequest = new JsonRpcRequest<IEnumerable<KeyValuePair<string, object>>>()
				{
					MethodName = request.MethodName,
					Arguments = argsDict
				};
			}
			else
				throw new ArgumentException(nameof(RpcRequest) + "." + nameof(RpcRequest.Arguments) + " must be a supported type, usually object[] or Dictionary<string, object>. Check the documentation.");

			var encoding = _TextEncoding ?? System.Text.UTF8Encoding.UTF8;
			System.IO.Stream retVal = null;

			try
			{
				if (_Settings == null)
					retVal = new System.IO.MemoryStream(encoding.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpcRequest)));
				else
					retVal = new System.IO.MemoryStream(encoding.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpcRequest, _Settings)));

				retVal.Seek(0, SeekOrigin.Begin);
				return retVal;
			}
			catch
			{
				retVal?.Dispose();
				throw;
			}
		}

	}
}