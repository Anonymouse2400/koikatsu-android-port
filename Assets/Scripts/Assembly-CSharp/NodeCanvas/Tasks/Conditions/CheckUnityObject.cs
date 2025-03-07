using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("✫ Blackboard")]
	public class CheckUnityObject : ConditionTask
	{
		[BlackboardOnly]
		public BBParameter<Object> valueA;

		public BBParameter<Object> valueB;

		protected override string info
		{
			get
			{
				return string.Concat(valueA, " == ", valueB);
			}
		}

		protected override bool OnCheck()
		{
			return valueA.value == valueB.value;
		}
	}
}
