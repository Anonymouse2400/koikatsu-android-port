namespace MessagePack.Decoders
{
	internal class FixInt16 : IInt16Decoder
	{
		internal static readonly IInt16Decoder Instance = new FixInt16();

		private FixInt16()
		{
		}

		public short Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return bytes[offset];
		}
	}
}
