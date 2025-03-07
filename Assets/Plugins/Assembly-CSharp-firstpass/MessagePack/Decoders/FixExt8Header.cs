namespace MessagePack.Decoders
{
	internal class FixExt8Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new FixExt8Header();

		private FixExt8Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			return new ExtensionHeader(typeCode, 8u);
		}
	}
}
