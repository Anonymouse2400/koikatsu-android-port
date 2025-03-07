namespace MessagePack.Decoders
{
	internal class UInt16UInt16 : IUInt16Decoder
	{
		internal static readonly IUInt16Decoder Instance = new UInt16UInt16();

		private UInt16UInt16()
		{
		}

		public ushort Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (ushort)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
