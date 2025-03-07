using System;
using System.Runtime.InteropServices;

namespace MessagePack
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Nil : IEquatable<Nil>
	{
		public static readonly Nil Default = default(Nil);

		public override bool Equals(object obj)
		{
			return obj is Nil;
		}

		public bool Equals(Nil other)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return "()";
		}
	}
}
