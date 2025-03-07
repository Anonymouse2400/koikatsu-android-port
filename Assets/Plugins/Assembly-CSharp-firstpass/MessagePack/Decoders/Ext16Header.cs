namespace MessagePack.Decoders
{
	internal class Ext16Header : IExtHeaderDecoder
	{
		internal static readonly IExtHeaderDecoder Instance = new Ext16Header();

		private Ext16Header()
		{
		}

		public ExtensionHeader Read(byte[] bytes, int offset, out int readSize)
		{
			uint length = (uint)((ushort)(bytes[offset + 1] << 8) | bytes[offset + 2]);
			sbyte typeCode = (sbyte)bytes[offset + 3];
			readSize = 4;
			return new ExtensionHeader(typeCode, length);
		}
	}
}
