using System.IO;
using UnityEngine;

namespace SaveAssist
{
	public abstract class AssetBundleAssist
	{
		protected string savePath = string.Empty;

		protected string assetBundleName = string.Empty;

		protected string assetName = string.Empty;

		public AssetBundleAssist(string _savePath, string _assetBundleName, string _assetName)
		{
			savePath = _savePath;
			assetBundleName = _assetBundleName;
			assetName = _assetName;
		}

		public void Save()
		{
			using (FileStream output = new FileStream(savePath, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter bw = new BinaryWriter(output))
				{
					SaveFunc(bw);
				}
			}
		}

		public abstract void SaveFunc(BinaryWriter bw);

		public void Load()
		{
			TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
			if (null != textAsset)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					memoryStream.Write(textAsset.bytes, 0, textAsset.bytes.Length);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (BinaryReader br = new BinaryReader(memoryStream))
					{
						LoadFunc(br);
					}
				}
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		}

		public abstract void LoadFunc(BinaryReader br);
	}
}
