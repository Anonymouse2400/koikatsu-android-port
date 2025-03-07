namespace MessagePack.Decoders
{
	internal class UInt8UInt16 : IUInt16Decoder
	{
		internal static readonly IUInt16Decoder Instance = new UInt8UInt16();

		private UInt8UInt16()
		{
		}

		public ushort Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return bytes[offset + 1];
		}
	}
}
