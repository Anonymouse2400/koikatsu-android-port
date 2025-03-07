using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("SendMessage to the agent, optionaly with an argument")]
	[Category("✫ Script Control/Common")]
	public class SendMessage<T> : ActionTask<Transform>
	{
		[RequiredField]
		public BBParameter<string> methodName;

		public BBParameter<T> argument;

		protected override string info
		{
			get
			{
				return string.Format("Message {0}({1})", methodName, argument.ToString());
			}
		}

		protected override void OnExecute()
		{
			if (argument.isNull)
			{
				base.agent.SendMessage(methodName.value);
			}
			else
			{
				base.agent.SendMessage(methodName.value, argument.value);
			}
			EndAction();
		}
	}
}
