using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
	public abstract class DTNode : Node
	{
		[SerializeField]
		private string _actorName = "INSTIGATOR";

		[SerializeField]
		private string _actorParameterID;

		public override string name
		{
			get
			{
				if (DLGTree.definedActorParameterNames.Contains(actorName))
				{
					return string.Format("#{0} {1}", base.ID, actorName);
				}
				return string.Format("#{0} <color=#d63e3e>* {1} *</color>", base.ID, _actorName);
			}
		}

		public override int maxInConnections
		{
			get
			{
				return -1;
			}
		}

		public override int maxOutConnections
		{
			get
			{
				return 1;
			}
		}

		public sealed override Type outConnectionType
		{
			get
			{
				return typeof(DTConnection);
			}
		}

		public sealed override bool allowAsPrime
		{
			get
			{
				return true;
			}
		}

		public sealed override bool showCommentsBottom
		{
			get
			{
				return false;
			}
		}

		protected DialogueTree DLGTree
		{
			get
			{
				return (DialogueTree)base.graph;
			}
		}

		protected string actorName
		{
			get
			{
				DialogueTree.ActorParameter parameterByID = DLGTree.GetParameterByID(_actorParameterID);
				return (parameterByID == null) ? _actorName : parameterByID.name;
			}
			set
			{
				if (_actorName != value && !string.IsNullOrEmpty(value))
				{
					_actorName = value;
					DialogueTree.ActorParameter parameterByName = DLGTree.GetParameterByName(value);
					_actorParameterID = ((parameterByName == null) ? null : parameterByName.ID);
				}
			}
		}

		protected IDialogueActor finalActor
		{
			get
			{
				IDialogueActor actorReferenceByID = DLGTree.GetActorReferenceByID(_actorParameterID);
				return (actorReferenceByID == null) ? DLGTree.GetActorReferenceByName(_actorName) : actorReferenceByID;
			}
		}
	}
}
