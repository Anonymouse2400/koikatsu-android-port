using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Blackboard/Lists")]
	public class GetIndexOfElement<T> : ActionTask
	{
		[BlackboardOnly]
		[RequiredField]
		public BBParameter<List<T>> targetList;

		public BBParameter<T> targetElement;

		[BlackboardOnly]
		public BBParameter<int> saveIndexAs;

		protected override void OnExecute()
		{
			saveIndexAs.value = targetList.value.IndexOf(targetElement.value);
			EndAction(true);
		}
	}
}
