namespace MessagePack.Decoders
{
	internal class Map16Header : IMapHeaderDecoder
	{
		internal static readonly IMapHeaderDecoder Instance = new Map16Header();

		private Map16Header()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (uint)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
