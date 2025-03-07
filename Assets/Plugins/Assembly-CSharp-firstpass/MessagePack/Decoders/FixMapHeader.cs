namespace MessagePack.Decoders
{
	internal class FixMapHeader : IMapHeaderDecoder
	{
		internal static readonly IMapHeaderDecoder Instance = new FixMapHeader();

		private FixMapHeader()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (uint)(bytes[offset] & 0xF);
		}
	}
}
