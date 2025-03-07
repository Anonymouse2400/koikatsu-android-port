namespace MessagePack.Decoders
{
	internal class UInt16Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new UInt16Int64();

		private UInt16Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (bytes[offset + 1] << 8) | bytes[offset + 2];
		}
	}
}
