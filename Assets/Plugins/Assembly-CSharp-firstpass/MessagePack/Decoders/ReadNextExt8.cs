namespace MessagePack.Decoders
{
	internal class ReadNextExt8 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextExt8();

		private ReadNextExt8()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			byte b = bytes[offset + 1];
			return b + 3;
		}
	}
}
