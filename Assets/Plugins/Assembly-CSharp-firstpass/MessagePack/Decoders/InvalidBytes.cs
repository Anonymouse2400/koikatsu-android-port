using System;

namespace MessagePack.Decoders
{
	internal class InvalidBytes : IBytesDecoder
	{
		internal static readonly IBytesDecoder Instance = new InvalidBytes();

		private InvalidBytes()
		{
		}

		public byte[] Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
