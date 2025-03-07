using UnityEngine;

namespace ADV.Commands.Base
{
	public class Wait : CommandBase
	{
		private float time;

		private float timer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Time" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			timer = 0f;
			float.TryParse(args[0], out time);
		}

		public override bool Process()
		{
			base.Process();
			timer += Time.deltaTime;
			return timer >= time;
		}
	}
}
