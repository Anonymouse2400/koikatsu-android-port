using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
	public class DialogueTreeController : GraphOwner<DialogueTree>, IDialogueActor
	{
		string IDialogueActor.name
		{
			get
			{
				return base.name;
			}
		}

		Texture2D IDialogueActor.portrait
		{
			get
			{
				return null;
			}
		}

		Sprite IDialogueActor.portraitSprite
		{
			get
			{
				return null;
			}
		}

		Color IDialogueActor.dialogueColor
		{
			get
			{
				return Color.white;
			}
		}

		Vector3 IDialogueActor.dialoguePosition
		{
			get
			{
				return Vector3.zero;
			}
		}

		Transform IDialogueActor.transform
		{
			get
			{
				return base.transform;
			}
		}

		public void StartDialogue()
		{
			graph = GetInstance(graph);
			graph.StartGraph(this, blackboard, true);
		}

		public void StartDialogue(IDialogueActor instigator)
		{
			graph = GetInstance(graph);
			graph.StartGraph((!(instigator is Component)) ? instigator.transform : ((Component)instigator), blackboard, true);
		}

		public void StartDialogue(IDialogueActor instigator, Action<bool> callback)
		{
			graph = GetInstance(graph);
			graph.StartGraph((!(instigator is Component)) ? instigator.transform : ((Component)instigator), blackboard, true, callback);
		}

		public void StartDialogue(Action<bool> callback)
		{
			graph = GetInstance(graph);
			graph.StartGraph(this, blackboard, true, callback);
		}
	}
}
