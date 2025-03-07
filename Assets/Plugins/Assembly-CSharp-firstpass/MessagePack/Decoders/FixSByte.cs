namespace MessagePack.Decoders
{
	internal class FixSByte : ISByteDecoder
	{
		internal static readonly ISByteDecoder Instance = new FixSByte();

		private FixSByte()
		{
		}

		public sbyte Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (sbyte)bytes[offset];
		}
	}
}
