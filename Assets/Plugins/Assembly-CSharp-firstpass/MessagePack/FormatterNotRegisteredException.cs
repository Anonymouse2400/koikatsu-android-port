using System;

namespace MessagePack
{
	public class FormatterNotRegisteredException : Exception
	{
		public FormatterNotRegisteredException(string message)
			: base(message)
		{
		}
	}
}
