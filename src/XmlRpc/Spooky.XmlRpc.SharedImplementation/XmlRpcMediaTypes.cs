using System;
using System.Collections.Generic;
using System.Text;

namespace Spooky.XmlRpc
{
	/// <summary>
	/// Provides constants for HTTP content media types for XML-RPC requests.
	/// </summary>
	public static class XmlRpcMediaTypes
	{
		/// <summary>
		/// The preferred media type string for http content containing an XML-RPC request or response.
		/// </summary>
		/// <remarks>
		/// <para>This is the old valid media type as specified by; http://xmlrpc.scripting.com/spec.html</para>
		/// </remarks>
		public const string TextXml = "text/xml";
	}
}