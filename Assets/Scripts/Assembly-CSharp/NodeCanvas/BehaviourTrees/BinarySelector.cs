using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Category("Composites")]
	[Description("Quick way to execute the left, or the right child node based on a Condition Task evaluation.")]
	[Icon("Condition", false)]
	[Color("b3ff7f")]
	public class BinarySelector : BTNode, ITaskAssignable<ConditionTask>, ITaskAssignable
	{
		public bool dynamic;

		[SerializeField]
		private ConditionTask _condition;

		private int succeedIndex;

		public override int maxOutConnections
		{
			get
			{
				return 2;
			}
		}

		public override bool showCommentsBottom
		{
			get
			{
				return false;
			}
		}

		public override string name
		{
			get
			{
				return base.name.ToUpper();
			}
		}

		public Task task
		{
			get
			{
				return condition;
			}
			set
			{
				condition = (ConditionTask)value;
			}
		}

		private ConditionTask condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			if (condition == null || base.outConnections.Count < 2)
			{
				return Status.Failure;
			}
			if (dynamic || base.status == Status.Resting)
			{
				int num = succeedIndex;
				succeedIndex = ((!condition.CheckCondition(agent, blackboard)) ? 1 : 0);
				if (succeedIndex != num)
				{
					base.outConnections[num].Reset();
				}
			}
			return base.outConnections[succeedIndex].Execute(agent, blackboard);
		}
	}
}
