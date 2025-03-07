namespace MessagePack.Decoders
{
	internal class ReadNextBin16 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextBin16();

		private ReadNextBin16()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = (bytes[offset + 1] << 8) | bytes[offset + 2];
			return num + 3;
		}
	}
}
