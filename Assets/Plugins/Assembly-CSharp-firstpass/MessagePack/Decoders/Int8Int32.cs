namespace MessagePack.Decoders
{
	internal class Int8Int32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new Int8Int32();

		private Int8Int32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 2;
			return (sbyte)bytes[offset + 1];
		}
	}
}
