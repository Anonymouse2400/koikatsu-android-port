using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[AgentType(typeof(IDialogueActor))]
	[Category("Dialogue")]
	[Icon("Dialogue", false)]
	[Description("A random statement will be chosen each time for the actor to say")]
	public class SayRandom : ActionTask
	{
		public List<Statement> statements = new List<Statement>();

		protected override void OnExecute()
		{
			int index = Random.Range(0, statements.Count);
			Statement statement = statements[index];
			Statement statement2 = statement.BlackboardReplace(base.blackboard);
			SubtitlesRequestInfo subtitlesRequestInfo = new SubtitlesRequestInfo((IDialogueActor)base.agent, statement2, base.EndAction);
			DialogueTree.RequestSubtitles(subtitlesRequestInfo);
		}
	}
}
