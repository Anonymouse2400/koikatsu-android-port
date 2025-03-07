using System;

namespace MessagePack.Decoders
{
	internal class InvalidBoolean : IBooleanDecoder
	{
		internal static IBooleanDecoder Instance = new InvalidBoolean();

		private InvalidBoolean()
		{
		}

		public bool Read()
		{
			throw new InvalidOperationException("code is invalid.");
		}
	}
}
