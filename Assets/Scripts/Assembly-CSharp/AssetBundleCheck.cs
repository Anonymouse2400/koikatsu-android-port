using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class AssetBundleCheck
{
	public static bool IsSimulation
	{
		get
		{
			return false;
		}
	}

	public static bool IsFile(string assetBundleName, string fileName = "")
	{
		if (!File.Exists(AssetBundleManager.BaseDownloadingURL + assetBundleName))
		{
			return false;
		}
		return true;
	}

	public static bool IsManifest(string manifest)
	{
		return AssetBundleManager.ManifestBundlePack.ContainsKey(manifest);
	}

	public static bool IsManifestOrBundle(string bundle)
	{
		return AssetBundleManager.ManifestBundlePack.ContainsKey(bundle) || IsFile(bundle, string.Empty);
	}

	public static string[] GetAllAssetName(string assetBundleName, bool _WithExtension = true, string manifestAssetBundleName = null, bool isAllCheck = false)
	{
		if (manifestAssetBundleName == null && isAllCheck && AssetBundleManager.AllLoadedAssetBundleNames.Contains(assetBundleName))
		{
			foreach (KeyValuePair<string, AssetBundleManager.BundlePack> item in AssetBundleManager.ManifestBundlePack)
			{
				LoadedAssetBundle value;
				if (item.Value.LoadedAssetBundles.TryGetValue(assetBundleName, out value))
				{
					if (_WithExtension)
					{
						return value.m_AssetBundle.GetAllAssetNames().Select(Path.GetFileName).ToArray();
					}
					return value.m_AssetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension).ToArray();
				}
			}
		}
		string error;
		LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out error, manifestAssetBundleName);
		AssetBundle assetBundle = ((loadedAssetBundle == null) ? AssetBundle.LoadFromFile(AssetBundleManager.BaseDownloadingURL + assetBundleName) : loadedAssetBundle.m_AssetBundle);
		string[] array = null;
		array = ((!_WithExtension) ? assetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension).ToArray() : assetBundle.GetAllAssetNames().Select(Path.GetFileName).ToArray());
		if (loadedAssetBundle == null)
		{
			assetBundle.Unload(true);
		}
		return array;
	}
}
