namespace MessagePack.Decoders
{
	internal class FixExt8 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new FixExt8();

		private FixExt8()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 10;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			byte[] data = new byte[8]
			{
				bytes[offset + 2],
				bytes[offset + 3],
				bytes[offset + 4],
				bytes[offset + 5],
				bytes[offset + 6],
				bytes[offset + 7],
				bytes[offset + 8],
				bytes[offset + 9]
			};
			return new ExtensionResult(typeCode, data);
		}
	}
}
