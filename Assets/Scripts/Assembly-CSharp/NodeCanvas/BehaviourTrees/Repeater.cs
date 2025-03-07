using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
	[Description("Repeat the child either x times or until it returns the specified status, or forever")]
	[Icon("Repeat", false)]
	[Name("Repeat")]
	[Category("Decorators")]
	public class Repeater : BTDecorator
	{
		public enum RepeaterMode
		{
			RepeatTimes = 0,
			RepeatUntil = 1,
			RepeatForever = 2
		}

		public enum RepeatUntilStatus
		{
			Failure = 0,
			Success = 1
		}

		public RepeaterMode repeaterMode;

		public RepeatUntilStatus repeatUntilStatus = RepeatUntilStatus.Success;

		public BBParameter<int> repeatTimes = 1;

		private int currentIteration = 1;

		protected override Status OnExecute(Component agent, IBlackboard blackboard)
		{
			if (base.decoratedConnection == null)
			{
				return Status.Resting;
			}
			if (base.decoratedConnection.status == Status.Success || base.decoratedConnection.status == Status.Failure)
			{
				base.decoratedConnection.Reset();
			}
			base.status = base.decoratedConnection.Execute(agent, blackboard);
			switch (base.status)
			{
			case Status.Resting:
				return Status.Running;
			case Status.Running:
				return Status.Running;
			default:
				switch (repeaterMode)
				{
				case RepeaterMode.RepeatTimes:
					if (currentIteration >= repeatTimes.value)
					{
						return base.status;
					}
					currentIteration++;
					break;
				case RepeaterMode.RepeatUntil:
					if (base.status == (Status)repeatUntilStatus)
					{
						return base.status;
					}
					break;
				}
				return Status.Running;
			}
		}

		protected override void OnReset()
		{
			currentIteration = 1;
		}
	}
}
