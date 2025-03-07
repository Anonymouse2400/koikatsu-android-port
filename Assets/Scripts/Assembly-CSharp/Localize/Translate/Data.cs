using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localize.Translate
{
	public class Data : ScriptableObject
	{
		public enum Type
		{
			DestroyObj = -4,
			DisableObj = -3,
			DestroyUI = -2,
			DisableUI = -1,
			Auto = 0,
			Sprite = 1,
			Texture = 2
		}

		[Serializable]
		public class Param
		{
			private string _manifest;

			public int ID = -1;

			public int type;

			public string bundle = string.Empty;

			public string asset = string.Empty;

			public string manifest = string.Empty;

			public string text = string.Empty;

			public string tag = string.Empty;

			public string Bundle
			{
				get
				{
					return (bundle.Length != 0) ? (Manager.LanguageUIPath + bundle) : bundle;
				}
			}

			public void SetBaseManifest(string manifest)
			{
				_manifest = manifest;
			}

			public AssetBundleManifestData ToData()
			{
				return new AssetBundleManifestData(Bundle, asset, manifest.IsNullOrEmpty() ? _manifest : manifest);
			}

			public UnityEngine.Object Load(bool isUnload)
			{
				if (asset.IsNullOrEmpty())
				{
					return null;
				}
				System.Type type = ToType();
				if (type == null)
				{
					return null;
				}
				AssetBundleManifestData assetBundleManifestData = ToData();
				try
				{
					AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleManifestData, type);
					return (!assetBundleLoadAssetOperation.IsEmpty()) ? assetBundleLoadAssetOperation.GetAsset<UnityEngine.Object>() : null;
				}
				catch (Exception)
				{
				}
				finally
				{
					if (isUnload)
					{
						assetBundleManifestData.UnloadBundle();
					}
				}
				return null;
			}

			public T Load<T>(bool isUnload) where T : UnityEngine.Object
			{
				if (asset.IsNullOrEmpty())
				{
					return (T)null;
				}
				AssetBundleManifestData assetBundleManifestData = ToData();
				try
				{
					AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleManifestData, typeof(T));
					return (!assetBundleLoadAssetOperation.IsEmpty()) ? assetBundleLoadAssetOperation.GetAsset<T>() : ((T)null);
				}
				catch (Exception)
				{
				}
				finally
				{
					if (isUnload)
					{
						assetBundleManifestData.UnloadBundle();
					}
				}
				return (T)null;
			}

			public System.Type ToType()
			{
				switch ((Type)type)
				{
				case Type.DestroyObj:
					return null;
				case Type.DisableObj:
					return null;
				case Type.DestroyUI:
					return null;
				case Type.DisableUI:
					return null;
				case Type.Auto:
					return typeof(GameObject);
				case Type.Sprite:
					return typeof(Sprite);
				case Type.Texture:
					return typeof(Texture);
				default:
					return typeof(GameObject);
				}
			}
		}

		public List<Param> param = new List<Param>();

		public const int ERROR_ID = -1;

		public const int FONT_ID = -100;
	}
}
