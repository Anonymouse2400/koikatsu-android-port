namespace MessagePack.Decoders
{
	internal class Ext32Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new Ext32Header();

		private Ext32Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			uint length = (uint)((bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4]);
			sbyte typeCode = (sbyte)bytes[offset + 5];
			readSize = 6;
			return new ExtensionHeader(typeCode, length);
		}
	}
}
