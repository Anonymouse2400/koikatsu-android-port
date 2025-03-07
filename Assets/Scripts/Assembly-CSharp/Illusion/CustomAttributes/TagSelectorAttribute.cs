using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class TagSelectorAttribute : PropertyAttribute
	{
		public bool AllowUntagged;
	}
}
