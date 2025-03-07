using System;
using MessagePack.LZ4;

namespace MessagePack.Internal
{
	internal static class LZ4MemoryPool
	{
		[ThreadStatic]
		private static byte[] lz4buffer;

		public static byte[] GetBuffer()
		{
			if (lz4buffer == null)
			{
				lz4buffer = new byte[LZ4Codec.MaximumOutputLength(65536)];
			}
			return lz4buffer;
		}
	}
}
