namespace MessagePack.Decoders
{
	internal class ReadNextExt16 : IReadNextDecoder
	{
		internal static readonly IReadNextDecoder Instance = new ReadNextExt16();

		private ReadNextExt16()
		{
		}

		public int Read(byte[] bytes, int offset)
		{
			int num = (ushort)(bytes[offset + 1] << 8) | bytes[offset + 2];
			return num + 4;
		}
	}
}
