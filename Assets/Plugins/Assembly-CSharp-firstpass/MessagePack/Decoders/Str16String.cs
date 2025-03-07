namespace MessagePack.Decoders
{
	internal class Str16String : IStringDecoder
	{
		internal static readonly IStringDecoder Instance = new Str16String();

		private Str16String()
		{
		}

		public string Read(byte[] bytes, int offset, out int readSize)
		{
			int num = (bytes[offset + 1] << 8) + bytes[offset + 2];
			readSize = num + 3;
			return StringEncoding.UTF8.GetString(bytes, offset + 3, num);
		}
	}
}
