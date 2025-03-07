using System.IO;
using Manager;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class MobCreate : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "No", "Bundle", "Asset", "hiPoly" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					int.MaxValue.ToString(),
					string.Empty,
					string.Empty,
					bool.FalseString
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
			bool hiPoly = false;
			args.SafeProc(num++, delegate(string s)
			{
				bool.TryParse(s, out hiPoly);
			});
			if (!Path.HasExtension(text))
			{
				text += ".unity3d";
			}
			SaveData.Heroine heroine = new SaveData.Heroine(false);
			TextAsset asset = AssetBundleManager.LoadAsset(text, assetName, typeof(TextAsset)).GetAsset<TextAsset>();
			Manager.Game.LoadFromTextAsset(-100, heroine, asset);
			AssetBundleManager.UnloadAssetBundle(text, false);
			base.scenario.commandController.AddChara(no, heroine, null, null, hiPoly);
		}
	}
}
