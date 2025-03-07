namespace MessagePack.Decoders
{
	internal class Int64Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new Int64Int64();

		private Int64Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 9;
			return ((long)(int)bytes[offset + 1] << 56) | ((long)(int)bytes[offset + 2] << 48) | ((long)(int)bytes[offset + 3] << 40) | ((long)(int)bytes[offset + 4] << 32) | ((long)(int)bytes[offset + 5] << 24) | ((long)(int)bytes[offset + 6] << 16) | ((long)(int)bytes[offset + 7] << 8) | (int)bytes[offset + 8];
		}
	}
}
