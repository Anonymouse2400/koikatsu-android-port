using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Gets a lists of game objects that are in the physics overlap sphere at the position of the agent, excluding the agent")]
	[Category("Physics")]
	public class GetOverlapSphereObjects : ActionTask<Transform>
	{
		public LayerMask layerMask = -1;

		public BBParameter<float> radius = 2f;

		[BlackboardOnly]
		public BBParameter<List<GameObject>> saveObjectsAs;

		protected override void OnExecute()
		{
			Collider[] source = Physics.OverlapSphere(base.agent.position, radius.value, layerMask);
			saveObjectsAs.value = source.Select((Collider c) => c.gameObject).ToList();
			saveObjectsAs.value.Remove(base.agent.gameObject);
			if (saveObjectsAs.value.Count == 0)
			{
				EndAction(false);
			}
			else
			{
				EndAction(true);
			}
		}

		public override void OnDrawGizmosSelected()
		{
			if (base.agent != null)
			{
				Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
				Gizmos.DrawSphere(base.agent.position, radius.value);
			}
		}
	}
}
