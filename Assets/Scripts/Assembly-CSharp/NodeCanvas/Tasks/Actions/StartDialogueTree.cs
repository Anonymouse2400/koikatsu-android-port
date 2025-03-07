using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Starts the Dialogue Tree assigned on a Dialogue Tree Controller object with specified agent used for 'Instigator'.")]
	[Category("Dialogue")]
	[AgentType(typeof(IDialogueActor))]
	[Icon("Dialogue", false)]
	public class StartDialogueTree : ActionTask
	{
		[RequiredField]
		public BBParameter<DialogueTreeController> dialogueTreeController;

		public bool waitActionFinish = true;

		protected override string info
		{
			get
			{
				return string.Format("Start Dialogue {0}", dialogueTreeController);
			}
		}

		protected override void OnExecute()
		{
			IDialogueActor instigator = (IDialogueActor)base.agent;
			if (waitActionFinish)
			{
				dialogueTreeController.value.StartDialogue(instigator, delegate(bool success)
				{
					EndAction(success);
				});
			}
			else
			{
				dialogueTreeController.value.StartDialogue(instigator);
				EndAction();
			}
		}
	}
}
