using System;
using UnityEngine;

public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
{
	protected string m_AssetBundleName;

	protected string m_AssetName;

	protected string m_ManifestAssetBundleName;

	protected Type m_Type;

	protected string m_DownloadingError;

	protected AssetBundleRequest m_Request;

	public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, Type type, string manifestAssetBundleName)
	{
		m_AssetBundleName = bundleName;
		m_AssetName = assetName;
		m_Type = type;
		m_ManifestAssetBundleName = manifestAssetBundleName;
	}

	public override bool IsEmpty()
	{
		return m_Request == null || !m_Request.isDone || m_Request.asset == null;
	}

	public override T GetAsset<T>()
	{
		if (m_Request != null && m_Request.isDone)
		{
			return m_Request.asset as T;
		}
		return (T)null;
	}

	public override T[] GetAllAssets<T>()
	{
		if (m_Request != null && m_Request.isDone)
		{
			T[] array = new T[m_Request.allAssets.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = m_Request.allAssets[i] as T;
			}
			return array;
		}
		return null;
	}

	public override bool Update()
	{
		if (m_Request != null)
		{
			return false;
		}
		LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError, m_ManifestAssetBundleName);
		if (loadedAssetBundle != null)
		{
			if ((bool)loadedAssetBundle.m_AssetBundle)
			{
				if (m_AssetName.IsNullOrEmpty())
				{
					m_Request = loadedAssetBundle.m_AssetBundle.LoadAllAssetsAsync(m_Type);
				}
				else
				{
					m_Request = loadedAssetBundle.m_AssetBundle.LoadAssetAsync(m_AssetName, m_Type);
				}
			}
			return false;
		}
		return true;
	}

	public override bool IsDone()
	{
		if (m_Request == null && m_DownloadingError != null)
		{
			return true;
		}
		return m_Request != null && m_Request.isDone;
	}
}
