namespace MessagePack.Decoders
{
	internal class ReadNext10 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext10();

		private ReadNext10()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 10;
		}
	}
}
