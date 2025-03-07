using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;

namespace NodeCanvas.Framework
{
	[DoNotList]
	public class ConditionList : ConditionTask
	{
		public enum ConditionsCheckMode
		{
			AllTrueRequired = 0,
			AnyTrueSuffice = 1
		}

		public ConditionsCheckMode checkMode;

		public List<ConditionTask> conditions = new List<ConditionTask>();

		private List<ConditionTask> initialActiveConditions;

		private bool allTrueRequired
		{
			get
			{
				return checkMode == ConditionsCheckMode.AllTrueRequired;
			}
		}

		protected override string info
		{
			get
			{
				string text = ((conditions.Count == 0) ? "No Conditions" : string.Empty);
				if (conditions.Count > 1)
				{
					text = text + "<b>(" + ((!allTrueRequired) ? "ANY True" : "ALL True") + ")</b>\n";
				}
				for (int i = 0; i < conditions.Count; i++)
				{
					if (conditions[i] != null && (conditions[i].isActive || (initialActiveConditions != null && initialActiveConditions.Contains(conditions[i]))))
					{
						text = text + conditions[i].summaryInfo + ((i != conditions.Count - 1) ? "\n" : string.Empty);
					}
				}
				return text;
			}
		}

		public override Task Duplicate(ITaskSystem newOwnerSystem)
		{
			ConditionList conditionList = (ConditionList)base.Duplicate(newOwnerSystem);
			conditionList.conditions.Clear();
			foreach (ConditionTask condition in conditions)
			{
				conditionList.AddCondition((ConditionTask)condition.Duplicate(newOwnerSystem));
			}
			return conditionList;
		}

		protected override void OnEnable()
		{
			if (initialActiveConditions == null)
			{
				initialActiveConditions = conditions.Where((ConditionTask c) => c.isActive).ToList();
			}
			for (int i = 0; i < initialActiveConditions.Count; i++)
			{
				initialActiveConditions[i].Enable(base.agent, base.blackboard);
			}
		}

		protected override void OnDisable()
		{
			for (int i = 0; i < initialActiveConditions.Count; i++)
			{
				initialActiveConditions[i].Disable();
			}
		}

		protected override bool OnCheck()
		{
			int num = 0;
			for (int i = 0; i < conditions.Count; i++)
			{
				if (!conditions[i].isActive)
				{
					num++;
				}
				else if (conditions[i].CheckCondition(base.agent, base.blackboard))
				{
					if (!allTrueRequired)
					{
						return true;
					}
					num++;
				}
				else if (allTrueRequired)
				{
					return false;
				}
			}
			return num == conditions.Count;
		}

		public override void OnDrawGizmos()
		{
			foreach (ConditionTask condition in conditions)
			{
				condition.OnDrawGizmos();
			}
		}

		public override void OnDrawGizmosSelected()
		{
			foreach (ConditionTask condition in conditions)
			{
				condition.OnDrawGizmosSelected();
			}
		}

		public void AddCondition(ConditionTask condition)
		{
			if (!(condition is ConditionList))
			{
				conditions.Add(condition);
				condition.SetOwnerSystem(base.ownerSystem);
			}
		}
	}
}
