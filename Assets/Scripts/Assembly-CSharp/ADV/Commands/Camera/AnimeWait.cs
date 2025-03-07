using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimeWait : CommandBase
	{
		private int layerNo;

		private float time;

		private Animator animator;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "LayerNo", "Time" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "0", "1" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			layerNo = int.Parse(args[num++]);
			time = float.Parse(args[num++]);
			animator = base.scenario.AdvCamera.GetComponent<Animator>();
		}

		public override bool Process()
		{
			base.Process();
			return animator == null || animator.GetCurrentAnimatorStateInfo(layerNo).normalizedTime >= time;
		}
	}
}
