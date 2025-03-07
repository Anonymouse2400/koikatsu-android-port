namespace MessagePack.Decoders
{
	internal class True : IBooleanDecoder
	{
		internal static IBooleanDecoder Instance = new True();

		private True()
		{
		}

		public bool Read()
		{
			return true;
		}
	}
}
