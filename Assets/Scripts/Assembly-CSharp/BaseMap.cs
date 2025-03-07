using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseMap : MonoBehaviour
{
	public const string MAP_ROOT_NAME = "Map";

	public GameObject mapRoot { get; private set; }

	public bool isMapLoading { get; protected set; }

	public int no { get; protected set; }

	public int prevNo { get; protected set; }

	public Dictionary<int, MapInfo.Param> infoDic { get; private set; }

	public virtual void Change(int no, Manager.Scene.Data.FadeType fadeType = Manager.Scene.Data.FadeType.InOut)
	{
		if (this.no != no)
		{
			isMapLoading = true;
			prevNo = this.no;
			this.no = no;
			MapInfo.Param param = infoDic[no];
			Singleton<Manager.Scene>.Instance.LoadBaseScene(new Manager.Scene.Data
			{
				assetBundleName = param.AssetBundleName,
				levelName = param.AssetName,
				fadeType = fadeType
			});
		}
	}

	public void Change(string mapName, Manager.Scene.Data.FadeType fadeType = Manager.Scene.Data.FadeType.InOut)
	{
		Change(ConvertMapNo(mapName), fadeType);
	}

	public MapInfo.Param GetParam(int no)
	{
		MapInfo.Param value;
		infoDic.TryGetValue(no, out value);
		return value;
	}

	public MapInfo.Param GetParam(string mapName)
	{
		return infoDic.Values.FirstOrDefault((MapInfo.Param p) => p.MapName == mapName);
	}

	public int ConvertMapNo(string mapName)
	{
		MapInfo.Param param = GetParam(mapName);
		return (param == null) ? (-1) : param.No;
	}

	public string ConvertMapName(int no)
	{
		MapInfo.Param param = GetParam(no);
		return (param == null) ? string.Empty : param.MapName;
	}

	private void LoadMapInfo()
	{
		infoDic = new Dictionary<int, MapInfo.Param>();
		CommonLib.GetAssetBundleNameListFromPath("map/list/mapinfo/", true).ForEach(delegate(string file)
		{
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset(file, typeof(MapInfo));
			foreach (List<MapInfo.Param> item in from p in assetBundleLoadAssetOperation.GetAllAssets<MapInfo>()
				select p.param)
			{
				foreach (MapInfo.Param item2 in item)
				{
					infoDic[item2.No] = item2;
				}
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
	}

	protected virtual void Awake()
	{
		isMapLoading = false;
		no = -1;
		SceneManager.activeSceneChanged += Reserve;
	}

	protected virtual void OnDestroy()
	{
		SceneManager.activeSceneChanged -= Reserve;
	}

	protected virtual void Start()
	{
		LoadMapInfo();
	}

	protected virtual void Reserve(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
	{
		mapRoot = newScene.GetRootGameObjects().FirstOrDefault((GameObject p) => p.name == "Map");
		if (!(mapRoot == null))
		{
			isMapLoading = false;
		}
	}
}
