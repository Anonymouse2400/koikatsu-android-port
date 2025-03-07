using System;

namespace MessagePack.Internal
{
	internal class MessagePackDynamicObjectResolverException : Exception
	{
		public MessagePackDynamicObjectResolverException(string message)
			: base(message)
		{
		}
	}
}
