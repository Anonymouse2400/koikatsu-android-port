namespace MessagePack.Decoders
{
	internal class Int16Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new Int16Int64();

		private Int16Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (short)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
