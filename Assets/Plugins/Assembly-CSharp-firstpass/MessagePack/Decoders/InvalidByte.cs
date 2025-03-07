using System;

namespace MessagePack.Decoders
{
	internal class InvalidByte : IByteDecoder
	{
		internal static readonly IByteDecoder Instance = new InvalidByte();

		private InvalidByte()
		{
		}

		public byte Read(byte[] bytes, int offset, out int readSize)
		{
			throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", bytes[offset], MessagePackCode.ToFormatName(bytes[offset])));
		}
	}
}
