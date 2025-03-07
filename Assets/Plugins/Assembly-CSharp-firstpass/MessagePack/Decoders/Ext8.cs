using System;

namespace MessagePack.Decoders
{
	internal class Ext8 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new Ext8();

		private Ext8()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			byte b = bytes[offset + 1];
			sbyte typeCode = (sbyte)bytes[offset + 2];
			byte[] array = new byte[b];
			readSize = b + 3;
			Buffer.BlockCopy(bytes, offset + 3, array, 0, b);
			return new ExtensionResult(typeCode, array);
		}
	}
}
