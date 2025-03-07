using System;

namespace MessagePack.Decoders
{
	internal class Bin16Bytes : IBytesDecoder
	{
		internal static readonly IBytesDecoder Instance = new Bin16Bytes();

		private Bin16Bytes()
		{
		}

		public byte[] Read(byte[] bytes, int offset, out int readSize)
		{
			int num = (bytes[offset + 1] << 8) + bytes[offset + 2];
			byte[] array = new byte[num];
			Buffer.BlockCopy(bytes, offset + 3, array, 0, num);
			readSize = num + 3;
			return array;
		}
	}
}
