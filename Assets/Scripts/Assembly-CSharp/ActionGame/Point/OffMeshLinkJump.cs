using ActionGame.Chara.Mover;
using Illusion.Component;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Point
{
	public class OffMeshLinkJump : TriggerEnterExitEvent
	{
		private OffMeshLink oml;

		private void HitAction(Collider other)
		{
			Main main = other.GetComponent<Base>().NowState as Main;
			if (main != null)
			{
				if (!main.isOnNavMeshLink)
				{
					main.NavMeshLinkIn(oml.endTransform.position);
				}
				else
				{
					main.NavMeshLinkOut();
				}
			}
		}

		private void Start()
		{
			oml = GetComponent<OffMeshLink>();
			_tagNames = new string[1] { "Player" };
			base.onTriggerEnter += HitAction;
		}

		private void OnDestroy()
		{
			base.onTriggerEnter -= HitAction;
		}
	}
}
