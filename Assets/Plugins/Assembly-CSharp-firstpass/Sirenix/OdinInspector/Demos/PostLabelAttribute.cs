using System;

namespace Sirenix.OdinInspector.Demos
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class PostLabelAttribute : Attribute
	{
		public string Name { get; private set; }

		public PostLabelAttribute(string name)
		{
			Name = name;
		}
	}
}
