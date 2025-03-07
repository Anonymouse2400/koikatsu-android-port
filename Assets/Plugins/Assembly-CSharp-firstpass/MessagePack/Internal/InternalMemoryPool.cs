using System;

namespace MessagePack.Internal
{
	internal static class InternalMemoryPool
	{
		[ThreadStatic]
		private static byte[] buffer;

		public static byte[] GetBuffer()
		{
			if (buffer == null)
			{
				buffer = new byte[65536];
			}
			return buffer;
		}
	}
}
