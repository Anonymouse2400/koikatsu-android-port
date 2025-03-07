namespace MessagePack.Decoders
{
	internal class UInt8Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new UInt8Int64();

		private UInt8Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return (int)bytes[offset + 1];
		}
	}
}
