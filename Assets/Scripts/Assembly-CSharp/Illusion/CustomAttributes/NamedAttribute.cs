using UnityEngine;

namespace Illusion.CustomAttributes
{
	public class NamedAttribute : PropertyAttribute
	{
		public readonly string name;

		public NamedAttribute(string name)
		{
			this.name = name;
		}
	}
}
