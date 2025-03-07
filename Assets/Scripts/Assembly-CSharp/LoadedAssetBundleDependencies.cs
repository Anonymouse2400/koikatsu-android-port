public class LoadedAssetBundleDependencies
{
	public string m_Key;

	public int m_ReferencedCount;

	public string[] m_BundleNames;

	public LoadedAssetBundleDependencies(string key, string[] bundleNames)
	{
		m_Key = key;
		m_BundleNames = bundleNames;
		m_ReferencedCount = 1;
	}
}
