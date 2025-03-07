namespace MessagePack.Decoders
{
	internal class UInt8Int16 : IInt16Decoder
	{
		internal static readonly IInt16Decoder Instance = new UInt8Int16();

		private UInt8Int16()
		{
		}

		public short Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return bytes[offset + 1];
		}
	}
}
