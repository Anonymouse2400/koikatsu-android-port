using System;

namespace MessagePack.Decoders
{
	internal class Ext32 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new Ext32();

		private Ext32()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			uint num = (uint)((bytes[offset + 1] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 8) | bytes[offset + 4]);
			sbyte typeCode = (sbyte)bytes[offset + 5];
			byte[] array = new byte[num];
			checked
			{
				readSize = (int)num + 6;
				Buffer.BlockCopy(bytes, offset + 6, array, 0, (int)num);
				return new ExtensionResult(typeCode, array);
			}
		}
	}
}
