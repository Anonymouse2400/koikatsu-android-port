namespace MessagePack.Decoders
{
	internal class Int32Int32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new Int32Int32();

		private Int32Int32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return (bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4];
		}
	}
}
