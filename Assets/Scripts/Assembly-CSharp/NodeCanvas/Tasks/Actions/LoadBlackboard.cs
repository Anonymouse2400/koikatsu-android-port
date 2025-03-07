using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Loads the blackboard variables previously saved in the provided PlayerPrefs key if at all. Returns false if no saves found or load was failed")]
	[Category("✫ Blackboard")]
	public class LoadBlackboard : ActionTask<Blackboard>
	{
		[RequiredField]
		public BBParameter<string> saveKey;

		protected override string info
		{
			get
			{
				return string.Format("Load Blackboard [{0}]", saveKey.ToString());
			}
		}

		protected override void OnExecute()
		{
			EndAction(base.agent.Load(saveKey.value));
		}
	}
}
