namespace MessagePack.Decoders
{
	internal class FixExt4 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new FixExt4();

		private FixExt4()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 6;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			byte[] data = new byte[4]
			{
				bytes[offset + 2],
				bytes[offset + 3],
				bytes[offset + 4],
				bytes[offset + 5]
			};
			return new ExtensionResult(typeCode, data);
		}
	}
}
