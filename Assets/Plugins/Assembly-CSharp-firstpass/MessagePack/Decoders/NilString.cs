namespace MessagePack.Decoders
{
	internal class NilString : IStringDecoder
	{
		internal static readonly IStringDecoder Instance = new NilString();

		private NilString()
		{
		}

		public string Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return null;
		}
	}
}
