namespace MessagePack.Decoders
{
	internal class Str8String : IStringDecoder
	{
		internal static readonly IStringDecoder Instance = new Str8String();

		private Str8String()
		{
		}

		public string Read(byte[] bytes, int offset, out int readSize)
		{
			int num = bytes[offset + 1];
			readSize = num + 2;
			return StringEncoding.UTF8.GetString(bytes, offset + 2, num);
		}
	}
}
