namespace Sirenix.OdinInspector.Demos
{
	public class HideReferenceObjectPickerExamples : SerializedMonoBehaviour
	{
		public class MyCustomReferenceType
		{
			public int A;

			public int B;

			public int C;
		}

		[Title("Hidden Object Pickers", null, TitleAlignments.Left, true, true)]
		[HideReferenceObjectPicker]
		public MyCustomReferenceType OdinSerializedProperty1;

		[HideReferenceObjectPicker]
		public MyCustomReferenceType OdinSerializedProperty2;

		[Title("Shown Object Pickers", null, TitleAlignments.Left, true, true)]
		public MyCustomReferenceType OdinSerializedProperty3;

		public MyCustomReferenceType OdinSerializedProperty4;
	}
}
