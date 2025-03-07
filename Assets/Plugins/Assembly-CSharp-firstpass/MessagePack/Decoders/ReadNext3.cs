namespace MessagePack.Decoders
{
	internal class ReadNext3 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext3();

		private ReadNext3()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 3;
		}
	}
}
