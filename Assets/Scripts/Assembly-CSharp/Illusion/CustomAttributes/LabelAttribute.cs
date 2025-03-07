using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class LabelAttribute : PropertyAttribute
	{
		public string label;

		public LabelAttribute(string label)
		{
			this.label = label;
		}
	}
}
