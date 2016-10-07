using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spooky.Tests
{
	[TestClass]
	public class RpcExceptionTests
	{

		#region Constructor Test

		[TestMethod]
		public void RpcException_Constructor_DefaultConstructorSetsErrorMessage()
		{
			var ex = new RpcException();
			Assert.AreEqual("Unknown RPC error", ex.Message);
		}

		[TestMethod]
		public void RpcException_Constructor_SetsMethodName()
		{
			var ex = new RpcException("add", new RpcError() { Code = Json20.JsonRpcErrorCodes.MethodNotFound, Message = "Method not found." });
			Assert.AreEqual("add", ex.MethodName);
		}

		[TestMethod]
		public void RpcException_Constructor_SetsRpcError()
		{
			var ex = new RpcException("add", new RpcError() { Code = Json20.JsonRpcErrorCodes.MethodNotFound, Message = "Method not found." });
			Assert.IsNotNull(ex.RpcError);
			Assert.AreEqual(Json20.JsonRpcErrorCodes.MethodNotFound, ex.RpcError.Code);
			Assert.AreEqual("Method not found.", ex.RpcError.Message);
		}

		[TestMethod]
		public void RpcException_Constructor_SetsErrorCode()
		{
			var ex = new RpcException("add", new RpcError() { Code = Json20.JsonRpcErrorCodes.MethodNotFound, Message = "Method not found." });
			Assert.AreEqual(Json20.JsonRpcErrorCodes.MethodNotFound, ex.ErrorCode);
		}

		[TestMethod]
		public void RpcException_Constructor_SetsErrorMessage()
		{
			var ex = new RpcException("add", new RpcError() { Code = Json20.JsonRpcErrorCodes.MethodNotFound, Message = "Method not found." });
			Assert.AreEqual("Method not found.", ex.Message);
		}

		#endregion

	}
}