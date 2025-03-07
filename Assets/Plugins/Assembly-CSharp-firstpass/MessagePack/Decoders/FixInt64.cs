namespace MessagePack.Decoders
{
	internal class FixInt64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new FixInt64();

		private FixInt64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (int)bytes[offset];
		}
	}
}
