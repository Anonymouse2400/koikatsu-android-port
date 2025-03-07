using Manager;
using UnityEngine;

namespace ADV.Commands.Sound.BGM
{
	public class Stop : CommandBase
	{
		private float stopTime;

		private float fadeTime;

		private float timer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Time", "Fade" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "0", "0.8" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			stopTime = float.Parse(args[num++]);
			fadeTime = float.Parse(args[num++]);
		}

		public override bool Process()
		{
			base.Process();
			if (timer >= stopTime)
			{
				return true;
			}
			timer += Time.deltaTime;
			return false;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			Singleton<Manager.Sound>.Instance.StopBGM(fadeTime);
		}
	}
}
