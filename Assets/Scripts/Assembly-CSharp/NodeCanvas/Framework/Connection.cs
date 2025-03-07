using System;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace NodeCanvas.Framework
{
	[SpoofAOT]
	public abstract class Connection
	{
		[SerializeField]
		private Node _sourceNode;

		[SerializeField]
		private Node _targetNode;

		[SerializeField]
		private bool _isDisabled;

		[NonSerialized]
		private Status _status = Status.Resting;

		public Node sourceNode
		{
			get
			{
				return _sourceNode;
			}
			protected set
			{
				_sourceNode = value;
			}
		}

		public Node targetNode
		{
			get
			{
				return _targetNode;
			}
			protected set
			{
				_targetNode = value;
			}
		}

		public bool isActive
		{
			get
			{
				return !_isDisabled;
			}
			set
			{
				if (!_isDisabled && !value)
				{
					Reset();
				}
				_isDisabled = !value;
			}
		}

		public Status status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}

		protected Graph graph
		{
			get
			{
				return sourceNode.graph;
			}
		}

		public Connection()
		{
		}

		public static Connection Create(Node source, Node target, int sourceIndex)
		{
			if (source == null || target == null)
			{
				return null;
			}
			if (source is MissingNode)
			{
				return null;
			}
			Connection connection = (Connection)Activator.CreateInstance(source.outConnectionType);
			connection.sourceNode = source;
			connection.targetNode = target;
			source.outConnections.Insert(sourceIndex, connection);
			target.inConnections.Add(connection);
			connection.OnValidate(sourceIndex, target.inConnections.IndexOf(connection));
			return connection;
		}

		public Connection Duplicate(Node newSource, Node newTarget)
		{
			if (newSource == null || newTarget == null)
			{
				return null;
			}
			Connection connection = JSONSerializer.Deserialize<Connection>(JSONSerializer.Serialize(typeof(Connection), this));
			connection.SetSource(newSource, false);
			connection.SetTarget(newTarget, false);
			ITaskAssignable taskAssignable = this as ITaskAssignable;
			if (taskAssignable != null && taskAssignable.task != null)
			{
				(connection as ITaskAssignable).task = taskAssignable.task.Duplicate(newSource.graph);
			}
			connection.OnValidate(newSource.outConnections.IndexOf(connection), newTarget.inConnections.IndexOf(connection));
			return connection;
		}

		public virtual void OnValidate(int sourceIndex, int targetIndex)
		{
		}

		public virtual void OnDestroy()
		{
		}

		public void SetSource(Node newSource, bool isRelink = true)
		{
			if (isRelink)
			{
				int connectionIndex = sourceNode.outConnections.IndexOf(this);
				sourceNode.OnChildDisconnected(connectionIndex);
				newSource.OnChildConnected(connectionIndex);
				sourceNode.outConnections.Remove(this);
			}
			newSource.outConnections.Add(this);
			sourceNode = newSource;
		}

		public void SetTarget(Node newTarget, bool isRelink = true)
		{
			if (isRelink)
			{
				int connectionIndex = targetNode.inConnections.IndexOf(this);
				targetNode.OnParentDisconnected(connectionIndex);
				newTarget.OnParentConnected(connectionIndex);
				targetNode.inConnections.Remove(this);
			}
			newTarget.inConnections.Add(this);
			targetNode = newTarget;
		}

		public Status Execute(Component agent, IBlackboard blackboard)
		{
			if (!isActive)
			{
				return Status.Resting;
			}
			status = targetNode.Execute(agent, blackboard);
			return status;
		}

		public void Reset(bool recursively = true)
		{
			if (status != Status.Resting)
			{
				status = Status.Resting;
				if (recursively)
				{
					targetNode.Reset(recursively);
				}
			}
		}
	}
}
