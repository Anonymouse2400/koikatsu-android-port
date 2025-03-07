namespace MessagePack.Decoders
{
	internal class Ext8Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new Ext8Header();

		private Ext8Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			byte length = bytes[offset + 1];
			sbyte typeCode = (sbyte)bytes[offset + 2];
			readSize = 3;
			return new ExtensionHeader(typeCode, length);
		}
	}
}
