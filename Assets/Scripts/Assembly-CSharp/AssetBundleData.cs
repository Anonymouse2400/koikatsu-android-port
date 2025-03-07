using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class AssetBundleData
{
	public string bundle = string.Empty;

	public string asset = string.Empty;

	protected AssetBundleLoadAssetOperation request;

	public bool isEmpty
	{
		get
		{
			return bundle.IsNullOrEmpty() || asset.IsNullOrEmpty();
		}
	}

	public virtual LoadedAssetBundle LoadedBundle
	{
		get
		{
			string error;
			return AssetBundleManager.GetLoadedAssetBundle(bundle, out error);
		}
	}

	public bool isFile
	{
		get
		{
			if (LoadedBundle != null)
			{
				return true;
			}
			if (File.Exists(AssetBundleManager.BaseDownloadingURL + bundle))
			{
				return true;
			}
			return false;
		}
	}

	public virtual string[] AllAssetNames
	{
		get
		{
			string[] array = null;
			LoadedAssetBundle loadedBundle = LoadedBundle;
			AssetBundle assetBundle = null;
			assetBundle = ((loadedBundle == null) ? AssetBundle.LoadFromFile(AssetBundleManager.BaseDownloadingURL + bundle) : loadedBundle.m_AssetBundle);
			array = assetBundle.GetAllAssetNames().Select(Path.GetFileNameWithoutExtension).ToArray();
			if (loadedBundle == null)
			{
				assetBundle.Unload(true);
			}
			return array;
		}
	}

	protected static bool isSimulation
	{
		get
		{
			return false;
		}
	}

	public AssetBundleData()
	{
	}

	public AssetBundleData(string bundle, string asset)
	{
		this.bundle = bundle;
		this.asset = asset;
	}

	public bool Check(string bundle, string asset)
	{
		if (!asset.IsNullOrEmpty() && this.asset != asset)
		{
			return true;
		}
		if (!bundle.IsNullOrEmpty() && this.bundle != bundle)
		{
			return true;
		}
		return false;
	}

	public static List<string> GetAssetBundleNameListFromPath(string path, bool subdirCheck = false)
	{
		List<string> result = new List<string>();
		string basePath = AssetBundleManager.BaseDownloadingURL;
		string path2 = basePath + path;
		if (!Directory.Exists(path2))
		{
			return result;
		}
		string[] source = ((!subdirCheck) ? Directory.GetFiles(path2, "*.unity3d") : Directory.GetFiles(path2, "*.unity3d", SearchOption.AllDirectories));
		return source.Select((string s) => s.Replace(basePath, string.Empty)).ToList();
	}

	public void ClearRequest()
	{
		request = null;
	}

	public virtual AssetBundleLoadAssetOperation LoadBundle<T>() where T : UnityEngine.Object
	{
		if (!isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAsset(this, typeof(T)));
	}

	public virtual AssetBundleLoadAssetOperation LoadBundleAsync<T>() where T : UnityEngine.Object
	{
		if (!isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAssetAsync(this, typeof(T)));
	}

	public virtual AssetBundleLoadAssetOperation LoadAllBundle<T>() where T : UnityEngine.Object
	{
		if (!isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAllAsset(this, typeof(T)));
	}

	public virtual AssetBundleLoadAssetOperation LoadAllBundleAsync<T>() where T : UnityEngine.Object
	{
		if (!isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAllAssetAsync(this, typeof(T)));
	}

	public virtual T GetAsset<T>() where T : UnityEngine.Object
	{
		if (request == null)
		{
			request = LoadBundle<T>();
		}
		if (request == null)
		{
			return (T)null;
		}
		return request.GetAsset<T>();
	}

	public virtual T[] GetAllAssets<T>() where T : UnityEngine.Object
	{
		if (request == null)
		{
			request = LoadAllBundle<T>();
		}
		if (request == null)
		{
			return null;
		}
		return request.GetAllAssets<T>();
	}

	public IEnumerator GetAsset<T>(Action<T> act) where T : UnityEngine.Object
	{
		if (request == null)
		{
			request = LoadBundleAsync<T>();
		}
		if (request != null)
		{
			yield return request;
			if (!request.IsEmpty())
			{
				act.Call(request.GetAsset<T>());
			}
		}
	}

	public IEnumerator GetAllAssets<T>(Action<T[]> act) where T : UnityEngine.Object
	{
		if (request == null)
		{
			request = LoadBundleAsync<T>();
		}
		if (request != null)
		{
			yield return request;
			if (!request.IsEmpty())
			{
				act.Call(request.GetAllAssets<T>());
			}
		}
	}

	public virtual void UnloadBundle(bool isUnloadForceRefCount = false, bool unloadAllLoadedObjects = false)
	{
		if (request != null)
		{
			AssetBundleManager.UnloadAssetBundle(this, isUnloadForceRefCount, unloadAllLoadedObjects);
		}
		request = null;
	}

	[Conditional("BASE_LOADER_LOG")]
	private void LogError(string str)
	{
	}
}
