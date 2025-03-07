using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Name("Set Visibility")]
	[Category("GameObject")]
	public class SetObjectActive : ActionTask<Transform>
	{
		public enum SetActiveMode
		{
			Deactivate = 0,
			Activate = 1,
			Toggle = 2
		}

		public SetActiveMode setTo = SetActiveMode.Toggle;

		protected override string info
		{
			get
			{
				return string.Format("{0} {1}", setTo, base.agentInfo);
			}
		}

		protected override void OnExecute()
		{
			bool active = ((setTo != SetActiveMode.Toggle) ? (setTo == SetActiveMode.Activate) : (!base.agent.gameObject.activeSelf));
			base.agent.gameObject.SetActive(active);
			EndAction();
		}
	}
}
