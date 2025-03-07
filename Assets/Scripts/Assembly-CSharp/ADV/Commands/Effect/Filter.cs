using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Effect
{
	public class Filter : CommandBase
	{
		private const string front = "front";

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Type", "Color", "Time" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3] { "front", "clear", "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			bool isFront = args[num++].Compare("front", true);
			Color color = args[num++].GetColor();
			float time = float.Parse(args[num++]);
			ADVFade advFade = base.scenario.advScene.AdvFade;
			advFade.enabled = true;
			advFade.CrossFadeColor(isFront, color, time, true);
		}
	}
}
