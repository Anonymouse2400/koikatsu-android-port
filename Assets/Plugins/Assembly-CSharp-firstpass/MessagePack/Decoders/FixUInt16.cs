namespace MessagePack.Decoders
{
	internal class FixUInt16 : IUInt16Decoder
	{
		internal static readonly IUInt16Decoder Instance = new FixUInt16();

		private FixUInt16()
		{
		}

		public ushort Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return bytes[offset];
		}
	}
}
