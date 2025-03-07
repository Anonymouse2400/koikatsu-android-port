namespace MessagePack.Decoders
{
	internal class ReadNext6 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext6();

		private ReadNext6()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 6;
		}
	}
}
