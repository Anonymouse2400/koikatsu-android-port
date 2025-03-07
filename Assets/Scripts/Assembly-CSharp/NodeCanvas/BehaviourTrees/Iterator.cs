using System.Collections;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Icon("List", false)]
	[Name("Iterate")]
	[Category("Decorators")]
	[Description("Iterate any type of list and execute the child node for each element in the list. Keeps iterating until the Termination Condition is met or the whole list is iterated and return the child node status")]
	public class Iterator : BTDecorator
	{
		public enum TerminationConditions
		{
			None = 0,
			FirstSuccess = 1,
			FirstFailure = 2
		}

		[RequiredField]
		[BlackboardOnly]
		public BBParameter<IList> targetList;

		[BlackboardOnly]
		public BBObjectParameter current;

		[BlackboardOnly]
		public BBParameter<int> storeIndex;

		public BBParameter<int> maxIteration = -1;

		public TerminationConditions terminationCondition;

		public bool resetIndex = true;

		private int currentIndex;

		private IList list
		{
			get
			{
				return (targetList == null) ? null : targetList.value;
			}
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			if (base.decoratedConnection == null)
			{
				return Status.Resting;
			}
			if (list == null || list.Count == 0)
			{
				return Status.Failure;
			}
			for (int i = currentIndex; i < list.Count; i++)
			{
				current.value = list[i];
				storeIndex.value = i;
				base.status = base.decoratedConnection.Execute(agent, blackboard);
				if (base.status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess)
				{
					return Status.Success;
				}
				if (base.status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure)
				{
					return Status.Failure;
				}
				if (base.status == Status.Running)
				{
					currentIndex = i;
					return Status.Running;
				}
				if (currentIndex == list.Count - 1 || currentIndex == maxIteration.value - 1)
				{
					return base.status;
				}
				base.decoratedConnection.Reset();
				currentIndex++;
			}
			return Status.Running;
		}

		protected override void OnReset()
		{
			if (resetIndex)
			{
				currentIndex = 0;
			}
		}
	}
}
