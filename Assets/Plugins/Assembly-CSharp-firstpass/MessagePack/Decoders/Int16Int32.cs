namespace MessagePack.Decoders
{
	internal class Int16Int32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new Int16Int32();

		private Int16Int32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 3;
			return (short)((bytes[offset + 1] << 8) | bytes[offset + 2]);
		}
	}
}
