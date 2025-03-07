using System;

namespace Sirenix.OdinInspector.Demos
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class NotOneAttribute : Attribute
	{
	}
}
