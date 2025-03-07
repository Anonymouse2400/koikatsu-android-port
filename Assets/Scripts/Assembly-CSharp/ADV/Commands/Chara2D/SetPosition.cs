using UnityEngine;

namespace ADV.Commands.Chara2D
{
	public class SetPosition : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "X", "Y" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					"0",
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int key = int.Parse(args[num++]);
			RectTransform rectTransform = base.scenario.commandController.Characters2D[key].rectTransform;
			Vector2 pos = rectTransform.anchoredPosition;
			for (int i = 0; i < 2; i++)
			{
				args.SafeProc(num++, delegate(string s)
				{
					pos[i] = float.Parse(s);
				});
			}
			rectTransform.anchoredPosition = pos;
		}
	}
}
