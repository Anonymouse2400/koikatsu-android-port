using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class EnumToggleButtonsExamples : MonoBehaviour
	{
		public MyBitmaskEnum DefaultEnumBitmask;

		[EnumToggleButtons]
		public MyEnum MyEnumField;

		[EnumToggleButtons]
		[HideLabel]
		[Title("Wide mode", null, TitleAlignments.Left, false, false)]
		public MyEnum WideEnumField;

		[EnumToggleButtons]
		[HideLabel]
		[Title("Wide mode", null, TitleAlignments.Left, false, false)]
		public MyBitmaskEnum BitmaskEnumField;

		[EnumToggleButtons]
		public MyBitmaskEnum[] BitmaskArray;
	}
}
