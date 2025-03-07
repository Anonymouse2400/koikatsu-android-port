using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
	public sealed class EnumFlags2Attribute : PropertyAttribute
	{
		public string label;

		public int line;

		public EnumFlags2Attribute(string label, int _oneline = -1)
		{
			this.label = label;
			line = _oneline;
		}
	}
}
