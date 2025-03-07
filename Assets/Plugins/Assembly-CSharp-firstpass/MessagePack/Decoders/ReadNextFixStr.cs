namespace MessagePack.Decoders
{
	internal class ReadNextFixStr : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextFixStr();

		private ReadNextFixStr()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = bytes[offset] & 0x1F;
			return num + 1;
		}
	}
}
