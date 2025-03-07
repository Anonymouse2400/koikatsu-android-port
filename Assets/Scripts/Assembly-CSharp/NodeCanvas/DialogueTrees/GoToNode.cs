using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
	[Description("Jump to another Dialogue node. Usefull if that other node is far away to connect, but otherwise it's exactly the same")]
	[Color("00b9e8")]
	[Name("Go To")]
	[Category("Flow Control")]
	public class GoToNode : DTNode
	{
		[SerializeField]
		private DTNode _targetNode;

		public override int maxOutConnections
		{
			get
			{
				return 0;
			}
		}

		public override string name
		{
			get
			{
				return "<GO TO>";
			}
		}

		protected override Status OnExecute(Component agent, IBlackboard bb)
		{
			if (_targetNode == null)
			{
				return Error("Target node of GOTO node is null");
			}
			base.DLGTree.EnterNode(_targetNode);
			return Status.Success;
		}
	}
}
