using System;

namespace MessagePack.Decoders
{
	internal class Bin32Bytes : IBytesDecoder
	{
		internal static readonly IBytesDecoder Instance = new Bin32Bytes();

		private Bin32Bytes()
		{
		}

		public byte[] Read(byte[] bytes, int offset, out int readSize)
		{
			int num = (bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4];
			byte[] array = new byte[num];
			Buffer.BlockCopy(bytes, offset + 5, array, 0, num);
			readSize = num + 5;
			return array;
		}
	}
}
