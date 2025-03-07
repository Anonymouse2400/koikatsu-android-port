namespace MessagePack.Decoders
{
	internal class FixExt1Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new FixExt1Header();

		private FixExt1Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			return new ExtensionHeader(typeCode, 1u);
		}
	}
}
