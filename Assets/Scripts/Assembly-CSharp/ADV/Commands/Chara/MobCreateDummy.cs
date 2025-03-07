using System.IO;
using Manager;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class MobCreateDummy : CommandBase
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
					"-100",
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			string text = args[num++].ToLower();
			string assetName = args[num++];
			if (!Path.HasExtension(text))
			{
				text += ".unity3d";
			}
			SaveData.Heroine heroine = new SaveData.Heroine(false);
			TextAsset asset = AssetBundleManager.LoadAsset(text, assetName, typeof(TextAsset)).GetAsset<TextAsset>();
			Manager.Game.LoadFromTextAsset(-100, heroine, asset);
			AssetBundleManager.UnloadAssetBundle(text, false);
			base.scenario.commandController.AddChara(no, new CharaData(heroine, base.scenario));
		}
	}
}
