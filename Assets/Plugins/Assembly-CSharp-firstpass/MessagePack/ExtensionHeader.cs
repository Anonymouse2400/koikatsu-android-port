using System.Runtime.InteropServices;

namespace MessagePack
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ExtensionHeader
	{
		public sbyte TypeCode { get; private set; }

		public uint Length { get; private set; }

		public ExtensionHeader(sbyte typeCode, uint length)
		{
			TypeCode = typeCode;
			Length = length;
		}
	}
}
