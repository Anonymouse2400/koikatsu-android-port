namespace MessagePack.Decoders
{
	internal class UInt64UInt64 : IUInt64Decoder
	{
		internal static readonly IUInt64Decoder Instance = new UInt64UInt64();

		private UInt64UInt64()
		{
		}

		public ulong Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 9;
			return ((ulong)bytes[offset + 1] << 56) | ((ulong)bytes[offset + 2] << 48) | ((ulong)bytes[offset + 3] << 40) | ((ulong)bytes[offset + 4] << 32) | ((ulong)bytes[offset + 5] << 24) | ((ulong)bytes[offset + 6] << 16) | ((ulong)bytes[offset + 7] << 8) | bytes[offset + 8];
		}
	}
}
