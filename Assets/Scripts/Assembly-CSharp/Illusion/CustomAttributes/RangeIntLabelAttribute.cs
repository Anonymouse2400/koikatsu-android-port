using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class RangeIntLabelAttribute : PropertyAttribute
	{
		public string label;

		public int min;

		public int max;

		public RangeIntLabelAttribute(string label, int min, int max)
		{
			this.label = label;
			this.min = min;
			this.max = max;
		}
	}
}
