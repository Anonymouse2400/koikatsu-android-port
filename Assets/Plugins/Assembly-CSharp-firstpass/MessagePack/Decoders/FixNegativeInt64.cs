namespace MessagePack.Decoders
{
	internal class FixNegativeInt64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new FixNegativeInt64();

		private FixNegativeInt64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (sbyte)bytes[offset];
		}
	}
}
