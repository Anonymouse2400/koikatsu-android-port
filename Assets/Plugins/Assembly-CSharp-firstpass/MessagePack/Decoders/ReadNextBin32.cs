namespace MessagePack.Decoders
{
	internal class ReadNextBin32 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextBin32();

		private ReadNextBin32()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = (bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4];
			return num + 5;
		}
	}
}
