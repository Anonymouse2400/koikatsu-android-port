namespace MessagePack.Decoders
{
	internal class FixString : IStringDecoder
	{
		internal static readonly IStringDecoder Instance = new FixString();

		private FixString()
		{
		}

		public string Read(byte[] bytes, int offset, out int readSize)
		{
			int num = bytes[offset] & 0x1F;
			readSize = num + 1;
			return StringEncoding.UTF8.GetString(bytes, offset + 1, num);
		}
	}
}
