namespace MessagePack.Decoders
{
	internal class FixArrayHeader : IArrayHeaderDecoder
	{
		internal static readonly IArrayHeaderDecoder Instance = new FixArrayHeader();

		private FixArrayHeader()
		{
		}

		public uint Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (uint)(bytes[offset] & 0xF);
		}
	}
}
