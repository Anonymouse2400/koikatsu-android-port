using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Effect
{
	public class Fade : CommandBase
	{
		private Color color = Color.clear;

		private float time;

		private float timer;

		private bool fadeIn = true;

		private bool isFront = true;

		private bool isInitColorSet;

		private ADVFade advFade;

		private EMTransition emtFade;

		private bool isEMT;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Fade", "Time", "Color", "Type", "isInit" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					"in",
					"0",
					"clear",
					string.Empty,
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			timer = 0f;
			int num = 0;
			fadeIn = args[num++].Compare("in", true);
			time = float.Parse(args[num++]);
			this.color = args[num++].GetColor();
			string self = args[num++];
			if (!self.IsNullOrEmpty())
			{
				isFront = self.Compare("front", true);
			}
			else
			{
				isEMT = true;
			}
			if (isEMT)
			{
				emtFade = base.scenario.advScene.EMTFade;
				if (this.color != Color.clear)
				{
					emtFade.SetColor(this.color);
				}
				emtFade.duration = time;
				bool flag = !(emtFade.curve.Evaluate(0f) > 0.5f);
				if ((fadeIn && !flag) || (!fadeIn && flag))
				{
					emtFade.FlipAnimationCurve();
				}
			}
			else
			{
				advFade = base.scenario.advScene.AdvFade;
				advFade.enabled = true;
				isInitColorSet = bool.Parse(args[num++]);
				if (isInitColorSet)
				{
					float a = ((!fadeIn) ? 1 : 0);
					Color color = this.color;
					color.a = a;
					advFade.SetColor(isFront, color);
				}
			}
			if (isEMT)
			{
				emtFade.Play();
			}
			else
			{
				FadeStart(time);
			}
		}

		public override bool Process()
		{
			base.Process();
			timer = Mathf.Min(timer + Time.deltaTime, time);
			if (isEMT)
			{
				return (!fadeIn) ? (emtFade.threshold <= 0f) : (emtFade.threshold >= 1f);
			}
			return timer >= time;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (!processEnd)
			{
				if (isEMT)
				{
					emtFade.Stop();
					emtFade.Set(1f);
				}
				else
				{
					FadeStart(0f);
				}
			}
		}

		private void FadeStart(float time)
		{
			float num = (fadeIn ? 1 : 0);
			if (color == Color.clear)
			{
				advFade.CrossFadeAlpha(isFront, num, time, false);
				return;
			}
			color.a = num;
			advFade.CrossFadeColor(isFront, color, time, true);
		}
	}
}
