using System;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Chara2D
{
	public class StandPosition : CommandBase
	{
		public enum Type
		{
			Center = 0,
			Left = 1,
			Right = 2
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Stand" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"0",
					Type.Center.ToString()
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int key = int.Parse(args[num++]);
			string self = args[num++];
			int num2 = self.Check(true, Enum.GetNames(typeof(Type)));
			RectTransform rectTransform = base.scenario.commandController.Characters2D[key].rectTransform;
			switch ((Type)num2)
			{
			case Type.Center:
				rectTransform.localPosition = Vector2.zero;
				break;
			case Type.Left:
				rectTransform.localPosition = Vector2.left * rectTransform.sizeDelta.x;
				break;
			case Type.Right:
				rectTransform.localPosition = Vector2.right * rectTransform.sizeDelta.x;
				break;
			}
		}
	}
}
