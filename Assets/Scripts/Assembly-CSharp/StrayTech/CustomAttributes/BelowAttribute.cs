using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class BelowAttribute : PropertyAttribute
	{
		public readonly float Max;

		public BelowAttribute(float maximumValue)
		{
			Max = maximumValue;
		}
	}
}
