using UnityEngine;

namespace ActionGame
{
	public class LookAtTarget : MonoBehaviour
	{
		public Transform target;

		public bool isPositionY;

		protected virtual void Update()
		{
			if (!(target == null))
			{
				Vector3 position = target.position;
				if (isPositionY)
				{
					position.y = base.transform.position.y;
				}
				base.transform.LookAt(position);
			}
		}
	}
}
