using Illusion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace ADV.Commands.Chara2D
{
	public class Create : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Bundle", "Asset" };
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
			string assetBundleName = args[num++];
			string text = args[num++];
			GameObject gameObject = new GameObject(text);
			Image image = gameObject.AddComponent<Image>();
			Utils.Bundle.LoadSprite(assetBundleName, text, image, true);
			gameObject.transform.SetParent(base.scenario.commandController.Character2D, false);
			base.scenario.commandController.Characters2D.Add(key, new CharaData2D(image));
		}
	}
}
