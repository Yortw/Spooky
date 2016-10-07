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

	}
}