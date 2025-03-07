using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class RangeLabelAttribute : PropertyAttribute
	{
		public string label;

		public float min;

		public float max;

		public RangeLabelAttribute(string label, float min, float max)
		{
			this.label = label;
			this.min = min;
			this.max = max;
		}
	}
}
