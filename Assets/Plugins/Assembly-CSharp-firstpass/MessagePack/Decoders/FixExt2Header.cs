namespace MessagePack.Decoders
{
	internal class FixExt2Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new FixExt2Header();

		private FixExt2Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			return new ExtensionHeader(typeCode, 2u);
		}
	}
}
