using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Will sort the gameobjects in the target list by their distance to the agent (closer first) and save that list to the blackboard")]
	[Category("✫ Blackboard/Lists")]
	public class SortGameObjectListByDistance : ActionTask<Transform>
	{
		[RequiredField]
		[BlackboardOnly]
		public BBParameter<List<GameObject>> targetList;

		[BlackboardOnly]
		public BBParameter<List<GameObject>> saveAs;

		public bool reverse;

		protected override string info
		{
			get
			{
				return string.Concat("Sort ", targetList, " by distance as ", saveAs);
			}
		}

		protected override void OnExecute()
		{
			saveAs.value = targetList.value.OrderBy((GameObject go) => Vector3.Distance(go.transform.position, base.agent.position)).ToList();
			if (reverse)
			{
				saveAs.value.Reverse();
			}
			EndAction();
		}
	}
}
