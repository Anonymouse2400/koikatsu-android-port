using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class RangeTupleAttribute : PropertyAttribute
	{
		public readonly float Min;

		public readonly float Max;

		public bool ConstrainToIntegralValues;

		public bool AllArgsValid = true;

		public RangeTupleAttribute(float min, float max, bool constrainToInts = false)
		{
			if (min >= max)
			{
				AllArgsValid = false;
			}
			Min = min;
			Max = max;
			ConstrainToIntegralValues = constrainToInts;
		}
	}
}
