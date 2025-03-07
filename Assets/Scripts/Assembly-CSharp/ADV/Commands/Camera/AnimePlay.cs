using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimePlay : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "State" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { string.Empty };
			}
		}

		public override void Do()
		{
			base.Do();
			Animator component = base.scenario.AdvCamera.GetComponent<Animator>();
			if (component != null)
			{
				component.Play(args[0]);
			}
		}
	}
}
