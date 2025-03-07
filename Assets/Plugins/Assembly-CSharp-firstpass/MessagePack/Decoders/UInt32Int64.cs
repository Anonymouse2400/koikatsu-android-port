namespace MessagePack.Decoders
{
	internal class UInt32Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new UInt32Int64();

		private UInt32Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return (uint)((bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4]);
		}
	}
}
