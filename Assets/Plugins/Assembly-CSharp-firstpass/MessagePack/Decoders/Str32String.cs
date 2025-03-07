namespace MessagePack.Decoders
{
	internal class Str32String : IStringDecoder
	{
		internal static readonly IStringDecoder Instance = new Str32String();

		private Str32String()
		{
		}

		public string Read(byte[] bytes, int offset, out int readSize)
		{
			int num = (bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4];
			readSize = num + 5;
			return StringEncoding.UTF8.GetString(bytes, offset + 5, num);
		}
	}
}
