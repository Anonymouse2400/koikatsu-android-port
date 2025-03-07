namespace MessagePack.Decoders
{
	internal class UInt8Byte : IByteDecoder
	{
		internal static readonly IByteDecoder Instance = new UInt8Byte();

		private UInt8Byte()
		{
		}

		public byte Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return bytes[offset + 1];
		}
	}
}
