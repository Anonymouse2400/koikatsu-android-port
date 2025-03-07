using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Category("Mutators (beta)")]
	[Description("Switch the root node of the behaviour tree to a new one defined by tag\nBeta Feature!")]
	[Name("Root Switcher")]
	[DoNotList]
	public class RootSwitcher : BTNode
	{
		public string targetNodeTag;

		private Node targetNode;

		public override void OnGraphStarted()
		{
			targetNode = base.graph.GetNodeWithTag<Node>(targetNodeTag);
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			if (string.IsNullOrEmpty(targetNodeTag))
			{
				return Status.Failure;
			}
			if (targetNode == null)
			{
				return Status.Failure;
			}
			if (base.graph.primeNode != targetNode)
			{
				base.graph.primeNode = targetNode;
			}
			return Status.Success;
		}
	}
}
