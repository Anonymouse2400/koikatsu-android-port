using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Use this to set a variable on any blackboard by overriding the agent")]
	[Category("✫ Blackboard")]
	public class SetOtherBlackboardVariable : ActionTask<Blackboard>
	{
		[RequiredField]
		public BBParameter<string> targetVariableName;

		public BBObjectParameter newValue;

		protected override string info
		{
			get
			{
				return string.Format("<b>{0}</b> = {1}", targetVariableName.ToString(), (newValue == null) ? string.Empty : newValue.ToString());
			}
		}

		protected override void OnExecute()
		{
			base.agent.SetValue(targetVariableName.value, newValue.value);
			EndAction();
		}
	}
}
