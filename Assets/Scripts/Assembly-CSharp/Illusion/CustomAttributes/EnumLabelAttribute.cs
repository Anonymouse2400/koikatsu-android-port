using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
	public class EnumLabelAttribute : PropertyAttribute
	{
		public string label;

		public EnumLabelAttribute(string label)
		{
			this.label = label;
		}
	}
}
