namespace MessagePack.Decoders
{
	internal class False : IBooleanDecoder
	{
		internal static IBooleanDecoder Instance = new False();

		private False()
		{
		}

		public bool Read()
		{
			return false;
		}
	}
}
