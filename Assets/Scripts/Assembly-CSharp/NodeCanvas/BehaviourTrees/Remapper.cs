using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Icon("Remap", false)]
	[Name("Remap")]
	[Category("Decorators")]
	[Description("Remap the child node's status to another status. Used to either invert the child's return status or to always return a specific status.")]
	public class Remapper : BTDecorator
	{
		public enum RemapStatus
		{
			Failure = 0,
			Success = 1
		}

		public RemapStatus successRemap = RemapStatus.Success;

		public RemapStatus failureRemap;

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
				return (Status)successRemap;
			case Status.Failure:
				return (Status)failureRemap;
			default:
				return base.status;
			}
		}
	}
}
