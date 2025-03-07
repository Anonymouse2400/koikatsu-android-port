using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Animator")]
	[Name("Set Parameter Bool")]
	[Description("You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
	public class MecanimSetBool : ActionTask<Animator>
	{
		public BBParameter<string> parameter;

		public BBParameter<int> parameterHashID;

		public BBParameter<bool> setTo;

		protected override string info
		{
			get
			{
				return string.Format("Mec.SetBool {0} to {1}", (!string.IsNullOrEmpty(parameter.value)) ? parameter.ToString() : parameterHashID.ToString(), setTo);
			}
		}

		protected override void OnExecute()
		{
			if (!string.IsNullOrEmpty(parameter.value))
			{
				base.agent.SetBool(parameter.value, setTo.value);
			}
			else
			{
				base.agent.SetBool(parameterHashID.value, setTo.value);
			}
			EndAction(true);
		}
	}
}
