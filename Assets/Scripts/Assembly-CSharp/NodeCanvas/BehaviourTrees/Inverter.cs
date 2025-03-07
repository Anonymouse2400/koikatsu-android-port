using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Category("Decorators")]
	[Name("Invert")]
	[Description("Inverts Success to Failure and Failure to Success")]
	[Icon("Remap", false)]
	public class Inverter : BTDecorator
	{
		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			if (base.decoratedConnection == null)
			{
				return Status.Resting;
			}
			base.status = base.decoratedConnection.Execute(agent, blackboard);
			switch (base.status)
			{
			case Status.Success:
				return Status.Failure;
			case Status.Failure:
				return Status.Success;
			default:
				return base.status;
			}
		}
	}
}
