using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Triggers a boolean variable for 1 frame to True then back to False")]
	[Category("✫ Utility")]
	public class TriggerBoolean : ActionTask
	{
		[BlackboardOnly]
		[RequiredField]
		public BBParameter<bool> variable;

		protected override string info
		{
			get
			{
				return string.Format("Trigger {0}", variable);
			}
		}

		protected override void OnExecute()
		{
			if (!variable.value)
			{
				variable.value = true;
				StartCoroutine(Flip());
			}
			EndAction();
		}

		private IEnumerator Flip()
		{
			yield return null;
			variable.value = false;
		}
	}
}
