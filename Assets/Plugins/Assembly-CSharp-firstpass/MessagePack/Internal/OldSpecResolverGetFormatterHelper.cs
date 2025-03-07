using System;
using MessagePack.Formatters;

namespace MessagePack.Internal
{
	internal static class OldSpecResolverGetFormatterHelper
	{
		internal static object GetFormatter(Type t)
		{
			if (t == typeof(string))
			{
				return OldSpecStringFormatter.Instance;
			}
			if (t == typeof(string[]))
			{
				return new ArrayFormatter<string>();
			}
			if (t == typeof(byte[]))
			{
				return OldSpecBinaryFormatter.Instance;
			}
			return null;
		}
	}
}
