using UnityEngine;

namespace ActionGame
{
	public class LookAtMainCamera : LookAtTarget
	{
		protected override void Update()
		{
			if (Camera.main != null)
			{
				target = Camera.main.transform;
			}
			base.Update();
		}
	}
}
