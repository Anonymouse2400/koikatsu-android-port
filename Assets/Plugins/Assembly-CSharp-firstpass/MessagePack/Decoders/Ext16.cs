using System;

namespace MessagePack.Decoders
{
	internal class Ext16 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new Ext16();

		private Ext16()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			int num = (ushort)(bytes[offset + 1] << 8) | bytes[offset + 2];
			sbyte typeCode = (sbyte)bytes[offset + 3];
			byte[] array = new byte[num];
			readSize = num + 4;
			Buffer.BlockCopy(bytes, offset + 4, array, 0, num);
			return new ExtensionResult(typeCode, array);
		}
	}
}
