namespace MessagePack.Decoders
{
	internal class ReadNextStr32 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextStr32();

		private ReadNextStr32()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = (bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4];
			return num + 5;
		}
	}
}
