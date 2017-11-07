using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spooky.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooky.Tests
{
	[TestClass]
	public class XmlRpcClientTests
	{

		[TestMethod]
		public void XmlRpcClient_Constructor_ConstructsWithServiceAddress()
		{
			var client = new XmlRpcHttpClient(new Uri("http://cookcomputing.com/xmlrpcsamples/math.rem"));
		}

		[TestMethod]
		public void XmlRpcClient_Constructor_ConstructsWithServiceAddressAndClient()
		{
			var client = new XmlRpcHttpClient(new Uri("http://cookcomputing.com/xmlrpcsamples/math.rem"), new System.Net.Http.HttpClient());
		}

	}
}