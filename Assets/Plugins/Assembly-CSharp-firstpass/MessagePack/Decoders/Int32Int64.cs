namespace MessagePack.Decoders
{
	internal class Int32Int64 : IInt64Decoder
	{
		internal static readonly IInt64Decoder Instance = new Int32Int64();

		private Int32Int64()
		{
		}

		public long Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return (long)(bytes[offset + 1] << 24) + (long)(bytes[offset + 2] << 16) + (bytes[offset + 3] << 8) + (int)bytes[offset + 4];
		}
	}
}
