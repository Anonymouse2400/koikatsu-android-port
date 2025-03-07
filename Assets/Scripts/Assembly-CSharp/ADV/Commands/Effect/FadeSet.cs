using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ADV.Commands.Effect
{
	public class FadeSet : CommandBase
	{
		private float timer;

		private float valueFrom;

		private float valueTo = 1f;

		private float time;

		private bool isFront = true;

		private bool isProc;

		private ADVFade advFade;

		private Color color = Color.clear;

		private EMTransition emtFade;

		private static int EMT_SiblingIndex = -1;

		private Color startColor = Color.clear;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[6] { "From", "To", "Time", "Color", "Type", "isProc" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[6]
				{
					"0",
					"1",
					"0",
					"clear",
					string.Empty,
					bool.FalseString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			timer = 0f;
			int num = 0;
			valueFrom = Mathf.Clamp01(float.Parse(args[num++]));
			valueTo = Mathf.Clamp01(float.Parse(args[num++]));
			time = float.Parse(args[num++]);
			color = args[num++].GetColor();
			string self = args.SafeGet(num++);
			if (!self.IsNullOrEmpty())
			{
				isFront = self.Compare("front", true);
			}
			args.SafeProc(num++, delegate(string s)
			{
				isProc = bool.Parse(s);
			});
			emtFade = base.scenario.advScene.EMTFade;
			advFade = base.scenario.advScene.AdvFade;
			if (color != Color.clear)
			{
				emtFade.SetColor(color);
			}
			RawImage image = emtFade.image;
			if (EMT_SiblingIndex == -1)
			{
				EMT_SiblingIndex = image.rectTransform.GetSiblingIndex();
			}
			if (isFront)
			{
				image.rectTransform.SetSiblingIndex(advFade.FrontIndex + 1);
			}
			else
			{
				image.rectTransform.SetSiblingIndex(advFade.BackIndex + 1);
			}
			startColor = emtFade.image.color;
		}

		public override bool Process()
		{
			base.Process();
			timer += Time.deltaTime;
			timer = Mathf.Min(timer, time);
			float t = ((time != 0f) ? Mathf.InverseLerp(0f, time, timer) : 1f);
			float t2 = Mathf.Lerp(valueFrom, valueTo, t);
			Calc(t2);
			return timer >= time;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			Calc(valueTo);
			emtFade.image.rectTransform.SetSiblingIndex(EMT_SiblingIndex);
		}

		private void Calc(float t)
		{
			if (!isProc)
			{
				emtFade.threshold = t;
				emtFade.Set();
			}
			else
			{
				emtFade.image.color = Color.Lerp(startColor, color, t);
			}
		}
	}
}
