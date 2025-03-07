namespace MessagePack.Decoders
{
	internal class Int8Int16 : IInt16Decoder
	{
		internal static readonly IInt16Decoder Instance = new Int8Int16();

		private Int8Int16()
		{
		}

		public short Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return (sbyte)bytes[offset + 1];
		}
	}
}
