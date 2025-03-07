using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
	public class BundlePack
	{
		private string[] m_Variants = new string[0];

		private AssetBundleManifest m_AssetBundleManifest;

		private Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();

		private Dictionary<string, AssetBundleCreate> m_CreateAssetBundles = new Dictionary<string, AssetBundleCreate>();

		private Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();

		private List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();

		private List<LoadedAssetBundleDependencies> m_Dependencies = new List<LoadedAssetBundleDependencies>();

		public string[] Variants
		{
			get
			{
				return m_Variants;
			}
			set
			{
				m_Variants = value;
			}
		}

		public AssetBundleManifest AssetBundleManifest
		{
			get
			{
				return m_AssetBundleManifest;
			}
			set
			{
				m_AssetBundleManifest = value;
			}
		}

		public Dictionary<string, LoadedAssetBundle> LoadedAssetBundles
		{
			get
			{
				return m_LoadedAssetBundles;
			}
			set
			{
				m_LoadedAssetBundles = value;
			}
		}

		public Dictionary<string, AssetBundleCreate> CreateAssetBundles
		{
			get
			{
				return m_CreateAssetBundles;
			}
			set
			{
				m_CreateAssetBundles = value;
			}
		}

		public Dictionary<string, string> DownloadingErrors
		{
			get
			{
				return m_DownloadingErrors;
			}
			set
			{
				m_DownloadingErrors = value;
			}
		}

		public List<AssetBundleLoadOperation> InProgressOperations
		{
			get
			{
				return m_InProgressOperations;
			}
			set
			{
				m_InProgressOperations = value;
			}
		}

		public List<LoadedAssetBundleDependencies> Dependencies
		{
			get
			{
				return m_Dependencies;
			}
			set
			{
				m_Dependencies = value;
			}
		}
	}

	public const string MAIN_MANIFEST_NAME = "abdata";

	public const string Extension = ".unity3d";

	private static HashSet<string> m_AllLoadedAssetBundleNames = new HashSet<string>();

	private static BundlePack MainBundle = null;

	private static Dictionary<string, BundlePack> m_ManifestBundlePack = new Dictionary<string, BundlePack>();

	private static string m_BaseDownloadingURL = string.Empty;

	private static bool isInitialized = false;

	private List<string> keysToRemove = new List<string>();

	public static string BaseDownloadingURL
	{
		get
		{
			return m_BaseDownloadingURL;
		}
	}

	public static string[] Variants
	{
		get
		{
			return MainBundle.Variants;
		}
		set
		{
			MainBundle.Variants = value;
		}
	}

	public static HashSet<string> AllLoadedAssetBundleNames
	{
		get
		{
			return m_AllLoadedAssetBundleNames;
		}
	}

	public static Dictionary<string, BundlePack> ManifestBundlePack
	{
		get
		{
			return m_ManifestBundlePack;
		}
	}

	public static float Progress
	{
		get
		{
			int num = MainBundle.LoadedAssetBundles.Count;
			float num2 = num;
			foreach (AssetBundleCreate value in MainBundle.CreateAssetBundles.Values)
			{
				num++;
				num2 += value.m_CreateRequest.progress;
			}
			return (num != 0) ? (num2 / (float)num) : 1f;
		}
	}

	public static BundlePack ManifestAdd(string manifestAssetBundleName)
	{
		if (m_ManifestBundlePack.ContainsKey(manifestAssetBundleName))
		{
			return null;
		}
		BundlePack bundlePack = new BundlePack();
		m_ManifestBundlePack.Add(manifestAssetBundleName, bundlePack);
		LoadedAssetBundle loadedAssetBundle = LoadAssetBundle(manifestAssetBundleName, false, manifestAssetBundleName);
		if (loadedAssetBundle == null)
		{
			m_ManifestBundlePack.Remove(manifestAssetBundleName);
			return null;
		}
		AssetBundleLoadAssetOperationSimulation assetBundleLoadAssetOperationSimulation = new AssetBundleLoadAssetOperationSimulation(loadedAssetBundle.m_AssetBundle.LoadAsset("AssetBundleManifest", typeof(AssetBundleManifest)));
		if (assetBundleLoadAssetOperationSimulation.IsEmpty())
		{
			m_ManifestBundlePack.Remove(manifestAssetBundleName);
			return null;
		}
		bundlePack.AssetBundleManifest = assetBundleLoadAssetOperationSimulation.GetAsset<AssetBundleManifest>();
		return bundlePack;
	}

	public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error, string manifestAssetBundleName = null)
	{
		assetBundleName = assetBundleName ?? string.Empty;
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		if (bundlePack.DownloadingErrors.TryGetValue(assetBundleName, out error))
		{
			return null;
		}
		LoadedAssetBundle value = null;
		bundlePack.LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value == null)
		{
			return null;
		}
		LoadedAssetBundleDependencies loadedAssetBundleDependencies = bundlePack.Dependencies.Find((LoadedAssetBundleDependencies p) => p.m_Key == assetBundleName);
		if (loadedAssetBundleDependencies == null)
		{
			return value;
		}
		string[] bundleNames = loadedAssetBundleDependencies.m_BundleNames;
		foreach (string key in bundleNames)
		{
			if (bundlePack.DownloadingErrors.TryGetValue(assetBundleName, out error))
			{
				return value;
			}
			LoadedAssetBundle value2;
			bundlePack.LoadedAssetBundles.TryGetValue(key, out value2);
			if (value2 == null)
			{
				return null;
			}
		}
		return value;
	}

	public static void Initialize(string basePath)
	{
		if (isInitialized)
		{
			return;
		}
		m_BaseDownloadingURL = basePath;
		GameObject gameObject = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		if (MainBundle == null)
		{
			MainBundle = ManifestAdd("abdata");
		}
		if (Directory.Exists(basePath))
		{
			foreach (string item in from s in (from s in Directory.GetFiles(basePath, "*.*", SearchOption.TopDirectoryOnly)
					where Path.GetExtension(s).IsNullOrEmpty()
					select s).Select(Path.GetFileNameWithoutExtension)
				where s != "abdata"
				select s)
			{
				ManifestAdd(item);
			}
		}
		isInitialized = true;
		InitAddComponent.AddComponents(gameObject);
	}

	public static LoadedAssetBundle LoadAssetBundle(string assetBundleName, bool isAsync, string manifestAssetBundleName = null)
	{
		bool flag = assetBundleName == manifestAssetBundleName;
		if (!flag)
		{
			assetBundleName = RemapVariantName(assetBundleName, manifestAssetBundleName);
		}
		if (!LoadAssetBundleInternal(assetBundleName, isAsync, manifestAssetBundleName) && !flag)
		{
			LoadDependencies(assetBundleName, isAsync, manifestAssetBundleName);
		}
		string error;
		return GetLoadedAssetBundle(assetBundleName, out error, manifestAssetBundleName);
	}

	protected static string RemapVariantName(string assetBundleName, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		string[] allAssetBundlesWithVariant = bundlePack.AssetBundleManifest.GetAllAssetBundlesWithVariant();
		if (Array.IndexOf(allAssetBundlesWithVariant, assetBundleName) < 0)
		{
			return assetBundleName;
		}
		string[] array = assetBundleName.Split('.');
		int num = int.MaxValue;
		int num2 = -1;
		for (int i = 0; i < allAssetBundlesWithVariant.Length; i++)
		{
			string[] array2 = allAssetBundlesWithVariant[i].Split('.');
			if (!(array2[0] != array[0]))
			{
				int num3 = Array.IndexOf(bundlePack.Variants, array2[1]);
				if (num3 != -1 && num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		if (num2 != -1)
		{
			return allAssetBundlesWithVariant[num2];
		}
		return assetBundleName;
	}

	public static bool LoadAssetBundleInternal(string assetBundleName, bool isAsync, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		LoadedAssetBundle value = null;
		bundlePack.LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value != null)
		{
			value.m_ReferencedCount++;
			return true;
		}
		AssetBundleCreate value2 = null;
		bundlePack.CreateAssetBundles.TryGetValue(assetBundleName, out value2);
		if (value2 != null)
		{
			value2.m_ReferencedCount++;
			return true;
		}
		if (!m_AllLoadedAssetBundleNames.Add(assetBundleName))
		{
			return true;
		}
		string path = BaseDownloadingURL + assetBundleName;
		if (!isAsync)
		{
			bundlePack.LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(AssetBundle.LoadFromFile(path)));
		}
		else
		{
			bundlePack.CreateAssetBundles.Add(assetBundleName, new AssetBundleCreate(AssetBundle.LoadFromFileAsync(path)));
		}
		return false;
	}

	protected static void LoadDependencies(string assetBundleName, bool isAsync, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		if (object.ReferenceEquals(bundlePack.AssetBundleManifest, null))
		{
			return;
		}
		string[] allDependencies = bundlePack.AssetBundleManifest.GetAllDependencies(assetBundleName);
		if (allDependencies.Length != 0)
		{
			for (int i = 0; i < allDependencies.Length; i++)
			{
				allDependencies[i] = RemapVariantName(allDependencies[i], manifestAssetBundleName);
			}
			LoadedAssetBundleDependencies loadedAssetBundleDependencies = bundlePack.Dependencies.Find((LoadedAssetBundleDependencies p) => p.m_Key == assetBundleName);
			if (loadedAssetBundleDependencies != null)
			{
				loadedAssetBundleDependencies.m_ReferencedCount++;
			}
			else
			{
				bundlePack.Dependencies.Add(new LoadedAssetBundleDependencies(assetBundleName, allDependencies));
			}
			for (int j = 0; j < allDependencies.Length; j++)
			{
				LoadAssetBundleInternal(allDependencies[j], isAsync, manifestAssetBundleName);
			}
		}
	}

	public static void UnloadAssetBundle(AssetBundleData data, bool isUnloadForceRefCount, bool unloadAllLoadedObjects = false)
	{
		UnloadAssetBundle(data.bundle, isUnloadForceRefCount, null, unloadAllLoadedObjects);
	}

	public static void UnloadAssetBundle(AssetBundleManifestData data, bool isUnloadForceRefCount, bool unloadAllLoadedObjects = false)
	{
		UnloadAssetBundle(data.bundle, isUnloadForceRefCount, data.manifest, unloadAllLoadedObjects);
	}

	public static void UnloadAssetBundle(string assetBundleName, bool isUnloadForceRefCount, string manifestAssetBundleName = null, bool unloadAllLoadedObjects = false)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		while (UnloadBundleAndDependencies(assetBundleName, manifestAssetBundleName, unloadAllLoadedObjects) && isUnloadForceRefCount)
		{
		}
	}

	private static bool UnloadBundleAndDependencies(string assetBundleName, string manifestAssetBundleName, bool unloadAllLoadedObjects)
	{
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		bool flag = UnloadBundle(assetBundleName, bundlePack, unloadAllLoadedObjects);
		if (flag)
		{
			LoadedAssetBundleDependencies loadedAssetBundleDependencies = bundlePack.Dependencies.Find((LoadedAssetBundleDependencies p) => p.m_Key == assetBundleName);
			if (loadedAssetBundleDependencies != null && --loadedAssetBundleDependencies.m_ReferencedCount == 0)
			{
				string[] bundleNames = loadedAssetBundleDependencies.m_BundleNames;
				foreach (string assetBundleName2 in bundleNames)
				{
					UnloadBundle(assetBundleName2, bundlePack, unloadAllLoadedObjects);
				}
				bundlePack.Dependencies.Remove(loadedAssetBundleDependencies);
			}
		}
		return flag;
	}

	private static bool UnloadBundle(string assetBundleName, BundlePack targetPack, bool unloadAllLoadedObjects)
	{
		assetBundleName = assetBundleName ?? string.Empty;
		string value;
		if (targetPack.DownloadingErrors.TryGetValue(assetBundleName, out value))
		{
			return false;
		}
		LoadedAssetBundle value2 = null;
		if (!targetPack.LoadedAssetBundles.TryGetValue(assetBundleName, out value2))
		{
			return false;
		}
		if (--value2.m_ReferencedCount == 0)
		{
			if ((bool)value2.m_AssetBundle)
			{
				value2.m_AssetBundle.Unload(unloadAllLoadedObjects);
			}
			targetPack.LoadedAssetBundles.Remove(assetBundleName);
			m_AllLoadedAssetBundleNames.Remove(assetBundleName);
		}
		return true;
	}

	private void Update()
	{
		foreach (KeyValuePair<string, BundlePack> item in m_ManifestBundlePack)
		{
			BundlePack value = item.Value;
			foreach (KeyValuePair<string, AssetBundleCreate> createAssetBundle in value.CreateAssetBundles)
			{
				AssetBundleCreateRequest createRequest = createAssetBundle.Value.m_CreateRequest;
				if (createRequest.isDone)
				{
					LoadedAssetBundle loadedAssetBundle = new LoadedAssetBundle(createRequest.assetBundle);
					loadedAssetBundle.m_ReferencedCount = createAssetBundle.Value.m_ReferencedCount;
					value.LoadedAssetBundles.Add(createAssetBundle.Key, loadedAssetBundle);
					keysToRemove.Add(createAssetBundle.Key);
				}
			}
			foreach (string item2 in keysToRemove)
			{
				value.CreateAssetBundles.Remove(item2);
			}
			int num = 0;
			while (num < value.InProgressOperations.Count)
			{
				if (!value.InProgressOperations[num].Update())
				{
					value.InProgressOperations.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			keysToRemove.Clear();
		}
	}

	public static AssetBundleLoadAssetOperation LoadAsset(AssetBundleData data, Type type)
	{
		return LoadAsset(data.bundle, data.asset, type);
	}

	public static AssetBundleLoadAssetOperation LoadAsset(AssetBundleManifestData data, Type type)
	{
		return LoadAsset(data.bundle, data.asset, type, data.manifest);
	}

	public static AssetBundleLoadAssetOperation LoadAsset(string assetBundleName, string assetName, Type type, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		if (assetBundleLoadAssetOperation == null)
		{
			LoadAssetBundle(assetBundleName, false, manifestAssetBundleName);
			assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationSimulation(bundlePack.LoadedAssetBundles[assetBundleName].m_AssetBundle.LoadAsset(assetName, type));
		}
		return assetBundleLoadAssetOperation;
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(AssetBundleData data, Type type)
	{
		return LoadAssetAsync(data.bundle, data.asset, type);
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(AssetBundleManifestData data, Type type)
	{
		return LoadAssetAsync(data.bundle, data.asset, type, data.manifest);
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, Type type, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		if (assetBundleLoadAssetOperation == null)
		{
			LoadAssetBundle(assetBundleName, true, manifestAssetBundleName);
			assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type, manifestAssetBundleName);
			bundlePack.InProgressOperations.Add(assetBundleLoadAssetOperation);
		}
		return assetBundleLoadAssetOperation;
	}

	public static AssetBundleLoadAssetOperation LoadAllAsset(AssetBundleData data, Type type)
	{
		return LoadAllAsset(data.bundle, type);
	}

	public static AssetBundleLoadAssetOperation LoadAllAsset(AssetBundleManifestData data, Type type)
	{
		return LoadAllAsset(data.bundle, type, data.manifest);
	}

	public static AssetBundleLoadAssetOperation LoadAllAsset(string assetBundleName, Type type, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		if (assetBundleLoadAssetOperation == null)
		{
			LoadAssetBundle(assetBundleName, false, manifestAssetBundleName);
			assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationSimulation(bundlePack.LoadedAssetBundles[assetBundleName].m_AssetBundle.LoadAllAssets(type));
		}
		return assetBundleLoadAssetOperation;
	}

	public static AssetBundleLoadAssetOperation LoadAllAssetAsync(AssetBundleData data, Type type)
	{
		return LoadAllAssetAsync(data.bundle, type);
	}

	public static AssetBundleLoadAssetOperation LoadAllAssetAsync(AssetBundleManifestData data, Type type)
	{
		return LoadAllAssetAsync(data.bundle, type, data.manifest);
	}

	public static AssetBundleLoadAssetOperation LoadAllAssetAsync(string assetBundleName, Type type, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		if (assetBundleLoadAssetOperation == null)
		{
			LoadAssetBundle(assetBundleName, true, manifestAssetBundleName);
			assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationFull(assetBundleName, null, type, manifestAssetBundleName);
			bundlePack.InProgressOperations.Add(assetBundleLoadAssetOperation);
		}
		return assetBundleLoadAssetOperation;
	}

	public static AssetBundleLoadOperation LoadLevel(AssetBundleData data, bool isAdditive)
	{
		return LoadLevel(data.bundle, data.asset, isAdditive);
	}

	public static AssetBundleLoadOperation LoadLevel(AssetBundleManifestData data, bool isAdditive)
	{
		return LoadLevel(data.bundle, data.asset, isAdditive, data.manifest);
	}

	public static AssetBundleLoadOperation LoadLevel(string assetBundleName, string levelName, bool isAdditive, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		AssetBundleLoadOperation assetBundleLoadOperation = null;
		if (assetBundleLoadOperation == null)
		{
			LoadAssetBundle(assetBundleName, false, manifestAssetBundleName);
			SceneManager.LoadScene(levelName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			assetBundleLoadOperation = new AssetBundleLoadLevelSimulationOperation();
		}
		return assetBundleLoadOperation;
	}

	public static AssetBundleLoadOperation LoadLevelAsync(AssetBundleData data, bool isAdditive)
	{
		return LoadLevelAsync(data.bundle, data.asset, isAdditive);
	}

	public static AssetBundleLoadOperation LoadLevelAsync(AssetBundleManifestData data, bool isAdditive)
	{
		return LoadLevelAsync(data.bundle, data.asset, isAdditive, data.manifest);
	}

	public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive, string manifestAssetBundleName = null)
	{
		if (manifestAssetBundleName.IsNullOrEmpty())
		{
			manifestAssetBundleName = "abdata";
		}
		BundlePack bundlePack = m_ManifestBundlePack[manifestAssetBundleName];
		AssetBundleLoadOperation assetBundleLoadOperation = null;
		if (assetBundleLoadOperation == null)
		{
			LoadAssetBundle(assetBundleName, true, manifestAssetBundleName);
			assetBundleLoadOperation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive, manifestAssetBundleName);
			bundlePack.InProgressOperations.Add(assetBundleLoadOperation);
		}
		return assetBundleLoadOperation;
	}
}
