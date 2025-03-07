namespace MessagePack.Decoders
{
	internal class UInt16UInt64 : IUInt64Decoder
	{
		internal static readonly IUInt64Decoder Instance = new UInt16UInt64();

		private UInt16UInt64()
		{
		}

		public ulong Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (ulong)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
