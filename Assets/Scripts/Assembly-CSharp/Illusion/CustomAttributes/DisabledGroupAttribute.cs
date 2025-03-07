using UnityEngine;

namespace Illusion.CustomAttributes
{
	public class DisabledGroupAttribute : PropertyAttribute
	{
		public string label;

		public DisabledGroupAttribute(string label)
		{
			this.label = label;
		}
	}
}
