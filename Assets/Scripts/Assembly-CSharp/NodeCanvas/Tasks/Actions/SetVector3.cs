using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Set a blackboard Vector3 variable")]
	[Category("✫ Blackboard")]
	public class SetVector3 : ActionTask
	{
		[BlackboardOnly]
		public BBParameter<Vector3> valueA;

		public OperationMethod operation;

		public BBParameter<Vector3> valueB;

		protected override string info
		{
			get
			{
				return string.Concat(valueA, OperationTools.GetOperationString(operation), valueB);
			}
		}

		protected override void OnExecute()
		{
			valueA.value = OperationTools.Operate(valueA.value, valueB.value, operation);
			EndAction();
		}
	}
}
