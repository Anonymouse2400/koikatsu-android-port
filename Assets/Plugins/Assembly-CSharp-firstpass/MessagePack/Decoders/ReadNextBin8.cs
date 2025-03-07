namespace MessagePack.Decoders
{
	internal class ReadNextBin8 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextBin8();

		private ReadNextBin8()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			byte b = bytes[offset + 1];
			return b + 2;
		}
	}
}
