using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimeRelease : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return null;
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void Do()
		{
			base.Do();
			UnityEngine.Camera advCamera = base.scenario.AdvCamera;
			if (!(advCamera == null))
			{
				UnityEngine.Object.Destroy(advCamera.GetComponent<Animator>());
			}
		}
	}
}
