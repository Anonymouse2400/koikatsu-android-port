using System.IO;
using System.Linq;
using Manager;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class FixCreateDummy : CommandBase
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
			string text = args[num++];
			SaveData.Heroine heroine = null;
			int fixID;
			if (!int.TryParse(text, out fixID))
			{
				string text2 = args[num++];
				if (!Path.HasExtension(text))
				{
					text += ".unity3d";
				}
				fixID = int.Parse(Path.GetFileNameWithoutExtension(text2).Substring(1));
				heroine = new SaveData.Heroine(false);
				TextAsset asset = AssetBundleManager.LoadAsset(text, text2, typeof(TextAsset)).GetAsset<TextAsset>();
				Manager.Game.LoadFromTextAsset(fixID, heroine, asset);
				AssetBundleManager.UnloadAssetBundle(text, false);
			}
			else
			{
				heroine = Singleton<Manager.Game>.Instance.HeroineList.Find((SaveData.Heroine p) => p.fixCharaID == fixID);
				if (heroine == null)
				{
					heroine = Manager.Game.CreateFixCharaList((from p in Singleton<Manager.Game>.Instance.HeroineList
						select p.fixCharaID into id
						where id < 0
						select id).ToArray()).Find((SaveData.Heroine p) => p.fixCharaID == fixID);
				}
			}
			base.scenario.commandController.AddChara(no, new CharaData(heroine, base.scenario));
		}
	}
}
