namespace Sirenix.OdinInspector.Demos
{
	public class A
	{
		[TableColumnWidth(50)]
		public bool Toggle;

		public string Message;

		[TableColumnWidth(160)]
		[HorizontalGroup("Actions", 0f, 0, 0, 0)]
		public void Test1()
		{
		}

		[HorizontalGroup("Actions", 0f, 0, 0, 0)]
		public void Test2()
		{
		}
	}
}
