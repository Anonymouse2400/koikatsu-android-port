namespace MessagePack.Decoders
{
	internal class Float32Single : ISingleDecoder
	{
		internal static readonly ISingleDecoder Instance = new Float32Single();

		private Float32Single()
		{
		}

		public float Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return new Float32Bits(bytes, offset + 1).Value;
		}
	}
}
