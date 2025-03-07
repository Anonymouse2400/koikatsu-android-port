namespace MessagePack.Decoders
{
	internal class ReadNextArray : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextArray();

		private ReadNextArray()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = offset;
			int readSize;
			int num2 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
			offset += readSize;
			for (int i = 0; i < num2; i++)
			{
				offset += MessagePackBinary.ReadNext(bytes, offset);
			}
			return offset - num;
		}
	}
}
