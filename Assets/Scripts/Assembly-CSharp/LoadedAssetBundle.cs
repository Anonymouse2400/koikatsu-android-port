using UnityEngine;

public class LoadedAssetBundle
{
	public AssetBundle m_AssetBundle;

	public uint m_ReferencedCount;

	public LoadedAssetBundle(AssetBundle assetBundle)
	{
		m_AssetBundle = assetBundle;
		m_ReferencedCount = 1u;
	}
}
