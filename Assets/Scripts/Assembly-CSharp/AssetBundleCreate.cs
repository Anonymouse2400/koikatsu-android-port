using UnityEngine;

public class AssetBundleCreate
{
	public AssetBundleCreateRequest m_CreateRequest;

	public uint m_ReferencedCount;

	public AssetBundleCreate(AssetBundleCreateRequest request)
	{
		m_CreateRequest = request;
		m_ReferencedCount = 1u;
	}
}
