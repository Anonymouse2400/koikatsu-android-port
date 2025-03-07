using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("✫ Blackboard")]
	public class CheckString : ConditionTask
	{
		[BlackboardOnly]
		public BBParameter<string> valueA;

		public BBParameter<string> valueB;

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
