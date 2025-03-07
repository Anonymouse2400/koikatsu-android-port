namespace MessagePack.Decoders
{
	internal class ReadNext1 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext1();

		private ReadNext1()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 1;
		}
	}
}
