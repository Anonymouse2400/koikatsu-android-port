using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("✫ Utility")]
	[Description("Return true or false based on the probability settings. Outcome is calculated EACH time this is checked")]
	public class Probability : ConditionTask
	{
		public BBParameter<float> probability = 0.5f;

		public BBParameter<float> maxValue = 1f;

		protected override string info
		{
			get
			{
				return probability.value / maxValue.value * 100f + "%";
			}
		}

		protected override bool OnCheck()
		{
			return Random.Range(0f, maxValue.value) <= probability.value;
		}
	}
}
