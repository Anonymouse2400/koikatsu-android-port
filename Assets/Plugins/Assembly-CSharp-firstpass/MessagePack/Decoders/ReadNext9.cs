namespace MessagePack.Decoders
{
	internal class ReadNext9 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNext9();

		private ReadNext9()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			return 9;
		}
	}
}
