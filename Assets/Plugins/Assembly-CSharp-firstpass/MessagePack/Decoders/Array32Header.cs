namespace MessagePack.Decoders
{
	internal class Array32Header : IArrayHeaderDecoder
	{
		internal static readonly IArrayHeaderDecoder Instance = new Array32Header();

		private Array32Header()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 5;
			return (uint)((bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4]);
		}
	}
}
