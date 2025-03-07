using System;

namespace Sirenix.OdinInspector.Demos
{
	[Flags]
	public enum MyBitmaskEnum
	{
		A = 2,
		B = 4,
		C = 8,
		ALL = 0xE
	}
}
