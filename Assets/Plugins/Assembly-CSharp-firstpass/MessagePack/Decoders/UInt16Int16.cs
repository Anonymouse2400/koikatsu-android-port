namespace MessagePack.Decoders
{
	internal class UInt16Int16 : IInt16Decoder
	{
		internal static readonly IInt16Decoder Instance = new UInt16Int16();

		private UInt16Int16()
		{
		}

		public short Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return checked((short)((bytes[offset + 1] << 8) + bytes[offset + 2]));
		}
	}
}
