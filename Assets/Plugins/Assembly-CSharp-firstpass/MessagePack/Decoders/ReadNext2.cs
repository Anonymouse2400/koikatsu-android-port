namespace MessagePack.Decoders
{
	internal class ReadNext2 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext2();

		private ReadNext2()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 2;
		}
	}
}
