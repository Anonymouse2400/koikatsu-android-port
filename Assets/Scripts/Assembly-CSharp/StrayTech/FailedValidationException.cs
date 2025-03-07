using System;
using System.Runtime.Serialization;

namespace StrayTech
{
	[Serializable]
	public class FailedValidationException : Exception
	{
		public FailedValidationException()
		{
		}

		public FailedValidationException(string message)
			: base(message)
		{
		}

		public FailedValidationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected FailedValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
