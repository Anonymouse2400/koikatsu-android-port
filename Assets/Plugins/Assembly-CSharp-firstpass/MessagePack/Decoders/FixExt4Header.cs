namespace MessagePack.Decoders
{
	internal class FixExt4Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new FixExt4Header();

		private FixExt4Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			return new ExtensionHeader(typeCode, 4u);
		}
	}
}
