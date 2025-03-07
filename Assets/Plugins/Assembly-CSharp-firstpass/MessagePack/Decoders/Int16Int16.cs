namespace MessagePack.Decoders
{
	internal class Int16Int16 : IInt16Decoder
	{
		internal static readonly IInt16Decoder Instance = new Int16Int16();

		private Int16Int16()
		{
		}

		public short Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (short)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
