namespace MessagePack.Decoders
{
	internal class ReadNextMap : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextMap();

		private ReadNextMap()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = offset;
			int readSize;
			int num2 = MessagePackBinary.ReadMapHeader(bytes, offset, out readSize);
			offset += readSize;
			for (int i = 0; i < num2; i++)
			{
				offset += MessagePackBinary.ReadNext(bytes, offset);
				offset += MessagePackBinary.ReadNext(bytes, offset);
			}
			return offset - num;
		}
	}
}
