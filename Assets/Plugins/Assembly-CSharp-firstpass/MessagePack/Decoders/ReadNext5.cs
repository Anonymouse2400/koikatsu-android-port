namespace MessagePack.Decoders
{
	internal class ReadNext5 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext5();

		private ReadNext5()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 5;
		}
	}
}
