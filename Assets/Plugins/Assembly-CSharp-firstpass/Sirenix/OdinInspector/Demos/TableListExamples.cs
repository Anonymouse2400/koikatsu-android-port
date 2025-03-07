using System.Collections.Generic;

namespace Sirenix.OdinInspector.Demos
{
	public class TableListExamples : SerializedMonoBehaviour
	{
		[TableList]
		public List<A> TableList = new List<A>();
	}
}
