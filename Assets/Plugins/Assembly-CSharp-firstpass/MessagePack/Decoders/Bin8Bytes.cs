using System;

namespace MessagePack.Decoders
{
	internal class Bin8Bytes : IBytesDecoder
	{
		internal static readonly IBytesDecoder Instance = new Bin8Bytes();

		private Bin8Bytes()
		{
		}

		public byte[] Read(byte[] bytes, int offset, out int readSize)
		{
			byte b = bytes[offset + 1];
			byte[] array = new byte[b];
			Buffer.BlockCopy(bytes, offset + 2, array, 0, b);
			readSize = b + 2;
			return array;
		}
	}
}
