namespace MessagePack.Decoders
{
	internal class UInt32UInt64 : IUInt64Decoder
	{
		internal static readonly IUInt64Decoder Instance = new UInt32UInt64();

		private UInt32UInt64()
		{
		}

		public ulong Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return (ulong)((long)((ulong)bytes[offset + 1] << 24) + (long)(bytes[offset + 2] << 16) + (bytes[offset + 3] << 8) + bytes[offset + 4]);
		}
	}
}
