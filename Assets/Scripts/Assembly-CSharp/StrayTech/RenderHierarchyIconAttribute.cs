using System;

namespace StrayTech
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class RenderHierarchyIconAttribute : Attribute
	{
		public readonly string _iconAssetPath;

		public RenderHierarchyIconAttribute(string iconAssetPath)
		{
			_iconAssetPath = iconAssetPath;
		}
	}
}
