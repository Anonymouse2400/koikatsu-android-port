using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Blackboard/Lists")]
	public class InsertElementToList<T> : ActionTask
	{
		[RequiredField]
		[BlackboardOnly]
		public BBParameter<List<T>> targetList;

		public BBParameter<T> targetElement;

		public BBParameter<int> targetIndex;

		protected override string info
		{
			get
			{
				return string.Format("Insert {0} in {1} at {2}", targetElement, targetList, targetIndex);
			}
		}

		protected override void OnExecute()
		{
			int value = targetIndex.value;
			List<T> value2 = targetList.value;
			if (value < 0 || value >= value2.Count)
			{
				EndAction(false);
				return;
			}
			value2.Insert(value, targetElement.value);
			EndAction(true);
		}
	}
}
