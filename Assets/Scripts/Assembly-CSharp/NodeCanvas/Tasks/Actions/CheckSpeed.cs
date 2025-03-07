using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Checks the current speed of the agent against a value based on it's Rigidbody velocity")]
	[Category("GameObject")]
	public class CheckSpeed : ConditionTask<Rigidbody>
	{
		public CompareMethod checkType;

		public BBParameter<float> value;

		[SliderField(0f, 0.1f)]
		public float differenceThreshold = 0.05f;

		protected override string info
		{
			get
			{
				return "Speed" + OperationTools.GetCompareString(checkType) + value;
			}
		}

		protected override bool OnCheck()
		{
			float magnitude = base.agent.velocity.magnitude;
			return OperationTools.Compare(magnitude, value.value, checkType, differenceThreshold);
		}
	}
}
