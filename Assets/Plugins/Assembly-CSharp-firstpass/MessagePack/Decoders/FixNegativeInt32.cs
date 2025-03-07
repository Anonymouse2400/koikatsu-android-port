namespace MessagePack.Decoders
{
	internal class FixNegativeInt32 : IInt32Decoder
	{
		internal static readonly IInt32Decoder Instance = new FixNegativeInt32();

		private FixNegativeInt32()
		{
		}

		public int Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (sbyte)bytes[offset];
		}
	}
}
