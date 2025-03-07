namespace MessagePack.Decoders
{
	internal class FixExt16Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new FixExt16Header();

		private FixExt16Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			return new ExtensionHeader(typeCode, 16u);
		}
	}
}
