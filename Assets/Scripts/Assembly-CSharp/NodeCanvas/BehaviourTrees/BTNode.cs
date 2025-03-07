using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;

namespace NodeCanvas.BehaviourTrees
{
	public abstract class BTNode : Node
	{
		public sealed override Type outConnectionType
		{
			get
			{
				return typeof(BTConnection);
			}
		}

		public sealed override int maxInConnections
		{
			get
			{
				return 1;
			}
		}

		public override int maxOutConnections
		{
			get
			{
				return 0;
			}
		}

		public sealed override bool allowAsPrime
		{
			get
			{
				return true;
			}
		}

		public override bool showCommentsBottom
		{
			get
			{
				return true;
			}
		}

		public T AddChild<T>(int childIndex) where T : BTNode
		{
			if (base.outConnections.Count >= maxOutConnections && maxOutConnections != -1)
			{
				return (T)null;
			}
			T val = base.graph.AddNode<T>();
			base.graph.ConnectNodes(this, val, childIndex);
			return val;
		}

		public T AddChild<T>() where T : BTNode
		{
			if (base.outConnections.Count >= maxOutConnections && maxOutConnections != -1)
			{
				return (T)null;
			}
			T val = base.graph.AddNode<T>();
			base.graph.ConnectNodes(this, val);
			return val;
		}

		public List<BTNode> GetAllChildNodesRecursively(bool includeThis)
		{
			List<BTNode> list = new List<BTNode>();
			if (includeThis)
			{
				list.Add(this);
			}
			foreach (BTNode item in base.outConnections.Select((Connection c) => c.targetNode))
			{
				list.AddRange(item.GetAllChildNodesRecursively(true));
			}
			return list;
		}

		public Dictionary<BTNode, int> GetAllChildNodesWithDepthRecursively(bool includeThis, int startIndex)
		{
			Dictionary<BTNode, int> dictionary = new Dictionary<BTNode, int>();
			if (includeThis)
			{
				dictionary[this] = startIndex;
			}
			foreach (BTNode item in base.outConnections.Select((Connection c) => c.targetNode))
			{
				foreach (KeyValuePair<BTNode, int> item2 in item.GetAllChildNodesWithDepthRecursively(true, startIndex + 1))
				{
					dictionary[item2.Key] = item2.Value;
				}
			}
			return dictionary;
		}
	}
}
