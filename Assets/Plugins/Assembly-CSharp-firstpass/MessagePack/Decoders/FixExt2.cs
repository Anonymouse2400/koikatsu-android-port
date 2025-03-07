namespace MessagePack.Decoders
{
	internal class FixExt2 : IExtDecoder
	{
		internal static readonly IExtDecoder Instance = new FixExt2();

		private FixExt2()
		{
		}

		public ExtensionResult Read(byte[] bytes, int offset, out int readSize)
		{
			readSize = 4;
			sbyte typeCode = (sbyte)bytes[offset + 1];
			byte[] data = new byte[2]
			{
				bytes[offset + 2],
				bytes[offset + 3]
			};
			return new ExtensionResult(typeCode, data);
		}
	}
}
