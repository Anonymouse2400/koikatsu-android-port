namespace Illusion.Extensions
{
	public static class IntExtensions
	{
		public static string MinusThroughToString(this int self, string format)
		{
			return (self < 0) ? self.ToString() : self.ToString(format);
		}

		public static int ValueRound(this int self, int add)
		{
			if (add == 0)
			{
				return self;
			}
			int num = self;
			self += add;
			if (add > 0 && self < num)
			{
				self = int.MaxValue;
			}
			else if (add < 0 && self > num)
			{
				self = int.MinValue;
			}
			return self;
		}
	}
}
