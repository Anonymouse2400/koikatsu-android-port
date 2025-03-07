using System;

namespace Illusion.Extensions
{
	public static class MulticastDelegateExtensions
	{
		public static int GetLength(this MulticastDelegate self)
		{
			if ((object)self == null || self.GetInvocationList() == null)
			{
				return 0;
			}
			return self.GetInvocationList().Length;
		}
	}
}
