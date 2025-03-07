namespace MessagePack.Decoders
{
	internal class Int8Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new Int8Int64();

		private Int8Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return (sbyte)bytes[offset + 1];
		}
	}
}
