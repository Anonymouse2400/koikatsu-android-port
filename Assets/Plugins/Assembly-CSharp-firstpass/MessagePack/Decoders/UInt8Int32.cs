namespace MessagePack.Decoders
{
	internal class UInt8Int32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new UInt8Int32();

		private UInt8Int32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return bytes[offset + 1];
		}
	}
}
