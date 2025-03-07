using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimationLerpSet : LerpSet
	{
		private Animator animator;

		private float normalizedTime;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Time", "X,Y,Z", "Pitch,Yaw,Roll", "Dir", "No,normalizedTime" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		protected override void Analytics(string[] args, TextScenario scenario)
		{
			base.Analytics(args, scenario);
			animator = null;
			normalizedTime = 0f;
			string[] array = args[4].Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				switch (i)
				{
				case 0:
				{
					int result;
					if (int.TryParse(array[i], out result))
					{
						animator = scenario.commandController.GetChara(result).chaCtrl.animBody;
					}
					else
					{
						animator = GameObject.Find(array[i]).GetComponent<Animator>();
					}
					break;
				}
				case 1:
					float.TryParse(array[i], out normalizedTime);
					break;
				default:
					return;
				}
			}
		}

		public override bool Process()
		{
			if (animator == null)
			{
				return true;
			}
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizedTime)
			{
				return base.Process();
			}
			return false;
		}
	}
}
