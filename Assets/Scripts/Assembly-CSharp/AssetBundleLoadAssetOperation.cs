using UnityEngine;

public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
	public abstract bool IsEmpty();

	public abstract T GetAsset<T>() where T : Object;

	public abstract T[] GetAllAssets<T>() where T : Object;
}
