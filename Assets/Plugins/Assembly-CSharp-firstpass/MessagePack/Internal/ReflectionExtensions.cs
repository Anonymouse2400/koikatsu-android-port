using System;
using System.Reflection;

namespace MessagePack.Internal
{
	internal static class ReflectionExtensions
	{
		public static bool IsNullable(this TypeInfo type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static bool IsPublic(this TypeInfo type)
		{
			return type.IsPublic;
		}
	}
}
