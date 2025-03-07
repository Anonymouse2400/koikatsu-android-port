using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Name("SubTree")]
	[Icon("BT", false)]
	[Category("Nested")]
	[Description("SubTree Node can be assigned an entire Sub BehaviorTree. The root node of that behaviour will be considered child node of this node and will return whatever it returns.\nThe SubTree can also be parametrized using Blackboard variables as normal.")]
	public class SubTree : BTNode, IGraphAssignable
	{
		[SerializeField]
		private BBParameter<BehaviourTree> _subTree;

		private Dictionary<BehaviourTree, BehaviourTree> instances = new Dictionary<BehaviourTree, BehaviourTree>();

		Graph IGraphAssignable.nestedGraph
		{
			get
			{
				return subTree;
			}
			set
			{
				subTree = (BehaviourTree)value;
			}
		}

		public override string name
		{
			get
			{
				return base.name.ToUpper();
			}
		}

		public BehaviourTree subTree
		{
			get
			{
				return _subTree.value;
			}
			set
			{
				_subTree.value = value;
			}
		}

		Graph[] IGraphAssignable.GetInstances()
		{
			return instances.Values.ToArray();
		}

		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			if (subTree == null || subTree.primeNode == null)
			{
				return Status.Failure;
			}
			if (base.status == Status.Resting)
			{
				CheckInstance();
			}
			return subTree.Tick(agent, blackboard);
		}

		protected override void OnReset()
		{
			if (IsInstance(subTree) && subTree.primeNode != null)
			{
				subTree.primeNode.Reset();
			}
		}

		public override void OnGraphStoped()
		{
			if (IsInstance(subTree))
			{
				for (int i = 0; i < subTree.allNodes.Count; i++)
				{
					subTree.allNodes[i].OnGraphStoped();
				}
			}
		}

		public override void OnGraphPaused()
		{
			if (IsInstance(subTree))
			{
				for (int i = 0; i < subTree.allNodes.Count; i++)
				{
					subTree.allNodes[i].OnGraphPaused();
				}
			}
		}

		private bool IsInstance(BehaviourTree bt)
		{
			return instances.Values.Contains(bt);
		}

		private void CheckInstance()
		{
			if (IsInstance(subTree))
			{
				return;
			}
			BehaviourTree value = null;
			if (!instances.TryGetValue(subTree, out value))
			{
				value = Graph.Clone(subTree);
				instances[subTree] = value;
				for (int i = 0; i < value.allNodes.Count; i++)
				{
					value.allNodes[i].OnGraphStarted();
				}
			}
			value.agent = base.graphAgent;
			value.blackboard = base.graphBlackboard;
			value.UpdateReferences();
			subTree = value;
		}
	}
}
