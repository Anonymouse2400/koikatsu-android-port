namespace MessagePack.Decoders
{
	internal class ReadNextStr16 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextStr16();

		private ReadNextStr16()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = (bytes[offset + 1] << 8) | bytes[offset + 2];
			return num + 3;
		}
	}
}
