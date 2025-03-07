using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Set a blackboard integer variable at random between min and max value")]
	[Name("Set Integer Random")]
	[Category("✫ Blackboard")]
	public class SetIntRandom : ActionTask
	{
		public BBParameter<int> minValue;

		public BBParameter<int> maxValue;

		[BlackboardOnly]
		public BBParameter<int> intVariable;

		protected override string info
		{
			get
			{
				return string.Concat("Set ", intVariable, " Random(", minValue, ", ", maxValue, ")");
			}
		}

		protected override void OnExecute()
		{
			intVariable.value = Random.Range(minValue.value, maxValue.value + 1);
			EndAction();
		}
	}
}
