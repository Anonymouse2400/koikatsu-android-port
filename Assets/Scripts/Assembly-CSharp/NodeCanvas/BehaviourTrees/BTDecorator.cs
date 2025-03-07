using NodeCanvas.Framework;

namespace NodeCanvas.BehaviourTrees
{
	public abstract class BTDecorator : BTNode
	{
		public sealed override int maxOutConnections
		{
			get
			{
				return 1;
			}
		}

		public sealed override bool showCommentsBottom
		{
			get
			{
				return false;
			}
		}

		protected Connection decoratedConnection
		{
			get
			{
				try
				{
					return base.outConnections[0];
				}
				catch
				{
					return null;
				}
			}
		}

		protected Node decoratedNode
		{
			get
			{
				try
				{
					return base.outConnections[0].targetNode;
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
