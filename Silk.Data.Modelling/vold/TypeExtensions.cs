﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling
{
	internal static class TypeExtensions
	{
		private static Type _enumerableCompareType = typeof(IEnumerable<>);

		public static (Type dataType, Type enumerableBaseType) GetDataAndEnumerableType(this Type type)
		{
			//  string is an enumerable of chars but we prefer to use it as a string
			if (type == typeof(string))
				return (type, null);

			var enumerableInterfaceType = type.GetTypeInfo().ImplementedInterfaces
				.FirstOrDefault(q => q.GetTypeInfo().IsGenericType && q.GetGenericTypeDefinition() == _enumerableCompareType);
			if (enumerableInterfaceType == null && type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == _enumerableCompareType)
				enumerableInterfaceType = type;
			if (enumerableInterfaceType == null)
				return (type, null);
			return (enumerableInterfaceType.GetTypeInfo().GenericTypeArguments[0], type);
		}
	}
}