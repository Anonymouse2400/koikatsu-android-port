using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Effect
{
	public class FilterSet : CommandBase
	{
		private const string front = "front";

		private Color initColor = Color.clear;

		private Color color = Color.clear;

		private float time;

		private float timer;

		private bool isFront = true;

		private ADVFade advFade;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "From", "To", "Time", "Type" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4] { "clear", "clear", "0", "front" };
			}
		}

		public override void Do()
		{
			base.Do();
			timer = 0f;
			int num = 0;
			initColor = args[num++].GetColor();
			color = args[num++].GetColor();
			time = float.Parse(args[num++]);
			isFront = args[num++].Compare("front", true);
			advFade = base.scenario.advScene.AdvFade;
			advFade.enabled = false;
		}

		public override bool Process()
		{
			base.Process();
			timer = Mathf.Min(timer + Time.deltaTime, time);
			float t = ((time != 0f) ? Mathf.InverseLerp(0f, time, timer) : 1f);
			advFade.SetColor(isFront, Color.Lerp(initColor, color, t));
			return timer >= time;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (!processEnd)
			{
				advFade.SetColor(isFront, color);
			}
		}
	}
}
