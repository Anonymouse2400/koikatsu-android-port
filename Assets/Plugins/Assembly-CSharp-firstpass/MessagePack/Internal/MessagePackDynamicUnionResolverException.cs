using System;

namespace MessagePack.Internal
{
	internal class MessagePackDynamicUnionResolverException : Exception
	{
		public MessagePackDynamicUnionResolverException(string message)
			: base(message)
		{
		}
	}
}
