using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class AboveAttribute : PropertyAttribute
	{
		public readonly float Min;

		public AboveAttribute(float minimumValue)
		{
			Min = minimumValue;
		}
	}
}
