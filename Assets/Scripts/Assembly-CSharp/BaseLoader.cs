using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

public class BaseLoader : MonoBehaviour
{
	[SerializeField]
	protected bool isErase;

	public const string LocalPath = "file://";

	public const string NetWorkPath = "http://";

	protected virtual void Awake()
	{
		Initialize();
		if (isErase)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected void Initialize()
	{
		if (!Singleton<AssetBundleManager>.IsInstance())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Application.dataPath);
			string value = "/../abdata" + '/';
			stringBuilder.Append(value);
			AssetBundleManager.Initialize(stringBuilder.ToString());
		}
	}

	public string GetRelativePath()
	{
		if (Application.isEditor)
		{
			return "file://" + Environment.CurrentDirectory.Replace("\\", "/");
		}
		//if (Application.isWebPlayer)
		//{
		//	return Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
		//}
		if (Application.isMobilePlatform || Application.isConsolePlatform)
		{
			return Application.streamingAssetsPath;
		}
		return "file://" + Application.streamingAssetsPath;
	}

	private static string GetPlatformFolderForAssetBundles(RuntimePlatform platform)
	{
		switch (platform)
		{
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iOS";
		case RuntimePlatform.WindowsPlayer:
			return "Windows";
		case RuntimePlatform.OSXPlayer:
			return "OSX";
		default:
			return null;
		}
	}

	protected T Load<T>(string assetBundleName, string assetName, bool isClone = false, string manifestAssetBundleName = null) where T : UnityEngine.Object
	{
		T val = new AssetBundleManifestData(assetBundleName, assetName, manifestAssetBundleName).GetAsset<T>();
		if (val != null && isClone)
		{
			T val2 = UnityEngine.Object.Instantiate(val);
			val2.name = val.name;
			val = val2;
		}
		return val;
	}

	protected IEnumerator Load_Coroutine<T>(string assetBundleName, string assetName, Action<T> act = null, bool isClone = false, string manifestAssetBundleName = null) where T : UnityEngine.Object
	{
		T asset = (T)null;
		yield return new AssetBundleManifestData(assetBundleName, assetName, manifestAssetBundleName).GetAsset(delegate(T x)
		{
			asset = x;
		});
		if (!(asset == null))
		{
			if (isClone)
			{
				T val = UnityEngine.Object.Instantiate(asset);
				val.name = asset.name;
				asset = val;
			}
			act.Call(asset);
		}
	}

	[Conditional("BASE_LOADER_LOG")]
	private void Log(string str)
	{
	}
}
