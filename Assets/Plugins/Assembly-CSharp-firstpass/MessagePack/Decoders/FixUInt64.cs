namespace MessagePack.Decoders
{
	internal class FixUInt64 : IUInt64Decoder
	{
		internal static readonly IUInt64Decoder Instance = new FixUInt64();

		private FixUInt64()
		{
		}

		public ulong Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return bytes[offset];
		}
	}
}
