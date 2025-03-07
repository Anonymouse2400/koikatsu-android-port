using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Name("Set Look At")]
	[Category("Animator")]
	[EventReceiver(new string[] { "OnAnimatorIK" })]
	public class MecanimSetLookAt : ActionTask<Animator>
	{
		public BBParameter<GameObject> targetPosition;

		public BBParameter<float> targetWeight;

		protected override string info
		{
			get
			{
				return "Mec.SetLookAt " + targetPosition;
			}
		}

		public void OnAnimatorIK()
		{
			base.agent.SetLookAtPosition(targetPosition.value.transform.position);
			base.agent.SetLookAtWeight(targetWeight.value);
			EndAction();
		}
	}
}
