namespace MessagePack.Decoders
{
	internal class ReadNext18 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext18();

		private ReadNext18()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 18;
		}
	}
}
