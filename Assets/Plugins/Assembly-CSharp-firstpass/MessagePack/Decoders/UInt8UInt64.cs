namespace MessagePack.Decoders
{
	internal class UInt8UInt64 : IUInt64Decoder
	{
		internal static readonly IUInt64Decoder Instance = new UInt8UInt64();

		private UInt8UInt64()
		{
		}

		public ulong Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return bytes[offset + 1];
		}
	}
}
