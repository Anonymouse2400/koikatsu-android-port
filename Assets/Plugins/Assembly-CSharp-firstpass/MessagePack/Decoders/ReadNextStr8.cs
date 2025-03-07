namespace MessagePack.Decoders
{
	internal class ReadNextStr8 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextStr8();

		private ReadNextStr8()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = bytes[offset + 1];
			return num + 2;
		}
	}
}
