namespace MessagePack.Decoders
{
	internal class UInt64Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new UInt64Int64();

		private UInt64Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 9;
			return ((long)(int)bytes[checked(offset + 1)] << 56) | ((long)(int)bytes[checked(offset + 2)] << 48) | ((long)(int)bytes[checked(offset + 3)] << 40) | ((long)(int)bytes[checked(offset + 4)] << 32) | ((long)(int)bytes[checked(offset + 5)] << 24) | ((long)(int)bytes[checked(offset + 6)] << 16) | ((long)(int)bytes[checked(offset + 7)] << 8) | (int)bytes[checked(offset + 8)];
		}
	}
}
