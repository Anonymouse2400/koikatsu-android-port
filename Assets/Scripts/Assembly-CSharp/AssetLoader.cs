using System.Collections;
using UnityEngine;

public class AssetLoader : BaseLoader
{
	public string assetBundleName;

	public string assetName;

	public bool isAsync = true;

	public bool isBundleUnload = true;

	public bool isUnloadForceRefCount;

	public bool unloadAllLoadedObjects;

	public string manifestFileName;

	[SerializeField]
	protected bool isLoad = true;

	[SerializeField]
	protected bool isClone;

	public Object loadObject { get; protected set; }

	public bool isLoadEnd { get; private set; }

	public bool initialized { get; protected set; }

	public void Init()
	{
		StartCoroutine(_Init());
	}

	public virtual IEnumerator _Init()
	{
		if (initialized)
		{
			yield break;
		}
		initialized = true;
		if (isLoad)
		{
			if (isAsync)
			{
				yield return StartCoroutine(Load_Coroutine(assetBundleName, assetName, delegate(Object o)
				{
					loadObject = o;
				}, isClone, manifestFileName));
			}
			else
			{
				loadObject = Load<Object>(assetBundleName, assetName, isClone, manifestFileName);
			}
		}
		isLoadEnd = true;
	}

	protected virtual IEnumerator Start()
	{
		if (!initialized)
		{
			yield return StartCoroutine(_Init());
		}
	}

	protected virtual void OnDestroy()
	{
		if (isLoadEnd && isBundleUnload && Singleton<AssetBundleManager>.IsInstance())
		{
			AssetBundleManager.UnloadAssetBundle(assetBundleName, isUnloadForceRefCount, manifestFileName, unloadAllLoadedObjects);
		}
	}
}
