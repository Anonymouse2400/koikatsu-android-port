namespace MessagePack.Decoders
{
	internal class FixNegativeInt16 : IInt16Decoder
	{
		internal static readonly IInt16Decoder Instance = new FixNegativeInt16();

		private FixNegativeInt16()
		{
		}

		public short Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 1;
			return (sbyte)bytes[offset];
		}
	}
}
