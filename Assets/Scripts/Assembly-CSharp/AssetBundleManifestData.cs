using System;
using UnityEngine;

[Serializable]
public class AssetBundleManifestData : AssetBundleData
{
	[SerializeField]
	private string _manifest = string.Empty;

	public string manifest
	{
		get
		{
			return _manifest;
		}
		set
		{
			_manifest = value;
		}
	}

	public new bool isEmpty
	{
		get
		{
			return base.isEmpty || manifest.IsNullOrEmpty();
		}
	}

	public override LoadedAssetBundle LoadedBundle
	{
		get
		{
			string error;
			return AssetBundleManager.GetLoadedAssetBundle(bundle, out error, _manifest);
		}
	}

	public AssetBundleManifestData()
	{
	}

	public AssetBundleManifestData(string bundle, string asset)
		: base(bundle, asset)
	{
	}

	public AssetBundleManifestData(string bundle, string asset, string manifest)
		: base(bundle, asset)
	{
		_manifest = manifest;
	}

	public bool Check(string bundle, string asset, string manifest)
	{
		if (!manifest.IsNullOrEmpty() && _manifest != manifest)
		{
			return true;
		}
		return Check(bundle, asset);
	}

	public override AssetBundleLoadAssetOperation LoadBundle<T>()
	{
		if (!base.isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAsset(this, typeof(T)));
	}

	public override AssetBundleLoadAssetOperation LoadBundleAsync<T>()
	{
		if (!base.isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAssetAsync(this, typeof(T)));
	}

	public override AssetBundleLoadAssetOperation LoadAllBundle<T>()
	{
		if (!base.isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAllAsset(this, typeof(T)));
	}

	public override AssetBundleLoadAssetOperation LoadAllBundleAsync<T>()
	{
		if (!base.isFile)
		{
			return null;
		}
		return request ?? (request = AssetBundleManager.LoadAllAssetAsync(this, typeof(T)));
	}

	public override void UnloadBundle(bool isUnloadForceRefCount = false, bool unloadAllLoadedObjects = false)
	{
		if (request != null)
		{
			AssetBundleManager.UnloadAssetBundle(this, isUnloadForceRefCount, unloadAllLoadedObjects);
		}
		request = null;
	}
}
