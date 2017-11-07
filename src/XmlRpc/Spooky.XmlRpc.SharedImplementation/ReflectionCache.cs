using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Spooky.XmlRpc
{
	internal static class ReflectionCache
	{
		private static System.Collections.Concurrent.ConcurrentDictionary<Type, CachedTypeInformation> _Cache = new System.Collections.Concurrent.ConcurrentDictionary<Type, CachedTypeInformation>();

		public static CachedTypeInformation GetTypeInfo(Type type)
		{
			CachedTypeInformation retVal = null;
			if (!_Cache.TryGetValue(type, out retVal))
			{
				retVal = new CachedTypeInformation(type.GetTypeInfo());
				_Cache.TryAdd(type, retVal);
			}

			return retVal;
		}
	}

	internal class CachedTypeInformation
	{

		private IEnumerable<ConstructorInfo> _Constructors;
		private IEnumerable<PropertyInfo> _Properties;
		private IEnumerable<Type> _ImplementedInterfaces;
		private ConstructorInfo _DefaultConstructor;

		public CachedTypeInformation(TypeInfo typeInfo)
		{
			this.TypeInfo = typeInfo;
		}

		public TypeInfo TypeInfo { get; private set; }

		public IEnumerable<ConstructorInfo> Constructors
		{
			get { return _Constructors ?? (_Constructors = TypeInfo.DeclaredConstructors); }
		}

		public IEnumerable<PropertyInfo> Properties
		{
			get { return _Properties ?? (_Properties = TypeInfo.DeclaredProperties); }
		}

		public IEnumerable<Type> ImplementedInterfaces
		{
			get { return _ImplementedInterfaces ?? (_ImplementedInterfaces = TypeInfo.ImplementedInterfaces); }
		}

		public ConstructorInfo DefaultConstructor()
		{
			if (_DefaultConstructor != null) return _DefaultConstructor;

			_DefaultConstructor = (from c in Constructors where (c.GetParameters()?.Length ?? 0) == 0 select c).FirstOrDefault();
			if (_DefaultConstructor == null) throw new InvalidOperationException($"Type {TypeInfo.FullName} must have a parameterless constructor to be used with Spooky XML-RPC.");

			return _DefaultConstructor;
		}

	}
}