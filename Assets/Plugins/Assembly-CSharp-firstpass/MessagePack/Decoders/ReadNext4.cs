namespace MessagePack.Decoders
{
	internal class ReadNext4 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext4();

		private ReadNext4()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 4;
		}
	}
}
