using System;

namespace Sirenix.OdinInspector.Demos
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class OnClickMethodAttribute : Attribute
	{
		public string MethodName { get; private set; }

		public OnClickMethodAttribute(string methodName)
		{
			MethodName = methodName;
		}
	}
}
