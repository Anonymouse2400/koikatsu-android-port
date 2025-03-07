namespace MessagePack.Decoders
{
	internal class Int8SByte : ISByteDecoder
	{
		internal static readonly ISByteDecoder Instance = new Int8SByte();

		private Int8SByte()
		{
		}

		public sbyte Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return (sbyte)bytes[offset + 1];
		}
	}
}
