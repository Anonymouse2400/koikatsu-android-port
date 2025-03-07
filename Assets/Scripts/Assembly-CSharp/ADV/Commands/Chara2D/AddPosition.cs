using UnityEngine;

namespace ADV.Commands.Chara2D
{
	public class AddPosition : CommandBase
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
					int.MaxValue.ToString(),
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
			Vector2 move = Vector2.zero;
			for (int i = 0; i < 2; i++)
			{
				args.SafeProc(num++, delegate(string s)
				{
					move[i] = float.Parse(s);
				});
			}
			rectTransform.anchoredPosition += move;
		}
	}
}
