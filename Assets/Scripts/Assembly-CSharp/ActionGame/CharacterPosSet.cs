using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using UnityEngine;

namespace ActionGame
{
	public class CharacterPosSet
	{
		public Dictionary<int, List<WaitPoint>> waitPointDic { get; private set; }

		public Dictionary<int, List<NonActiveWaitPointInfo.Param>> nonActiveWaitPointDic { get; private set; }

		public void Initialize()
		{
			List<WaitPoint> wpList = new List<WaitPoint>();
			Transform root = new GameObject("WaitPoints").transform;
			root.SetParent(Singleton<Game>.Instance.actScene.transform, false);
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("map/waitpoint/", true);
			assetBundleNameListFromPath.Sort();
			assetBundleNameListFromPath.ForEach(delegate(string file)
			{
				foreach (GameObject item2 in from p in AssetBundleManager.LoadAllAsset(file, typeof(GameObject)).GetAllAssets<GameObject>()
					orderby p.name
					select p)
				{
					GameObject gameObject = Object.Instantiate(item2);
					gameObject.transform.Children().ForEach(delegate(Transform t)
					{
						t.SetParent(root, false);
						WaitPoint[] componentsInChildren = t.GetComponentsInChildren<WaitPoint>();
						Dictionary<int, List<WaitPoint>> dictionary = wpList.ToLookup((WaitPoint v) => v.MapNo, (WaitPoint v) => v).ToDictionary((IGrouping<int, WaitPoint> v) => v.Key, Enumerable.ToList);
						List<WaitPoint> list = new List<WaitPoint>();
						List<WaitPoint> list2 = new List<WaitPoint>();
						WaitPoint[] array = componentsInChildren;
						foreach (WaitPoint item in array)
						{
							List<WaitPoint> value;
							if (dictionary.TryGetValue(item.MapNo, out value))
							{
								WaitPoint waitPoint = value.Find((WaitPoint p) => p.name == item.name);
								if (waitPoint != null)
								{
									waitPoint.kindList.AddRange(item.kindList);
									waitPoint.transform.SetPositionAndRotation(item.transform.position, item.transform.rotation);
									waitPoint.offsetHPos = item.offsetHPos;
									waitPoint.offsetHAngle = item.offsetHAngle;
									waitPoint.navMeshOffsetPoint = item.navMeshOffsetPoint;
									waitPoint.parameterList.AddRange(item.parameterList);
									list2.Add(item);
									continue;
								}
							}
							list.Add(item);
						}
						foreach (WaitPoint item3 in list2)
						{
							Object.Destroy(item3.gameObject);
						}
						wpList.AddRange(list);
					});
					Object.Destroy(gameObject);
				}
				AssetBundleManager.UnloadAssetBundle(file, true);
			});
			waitPointDic = wpList.ToLookup((WaitPoint v) => v.MapNo, (WaitPoint v) => v).ToDictionary((IGrouping<int, WaitPoint> v) => v.Key, Enumerable.ToList);
			List<NonActiveWaitPointInfo.Param> list3 = new List<NonActiveWaitPointInfo.Param>();
			CommonLib.GetAssetBundleNameListFromPath("action/list/waitpoint/non/", true).ForEach(delegate(string file)
			{
				NonActiveWaitPointInfo[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(NonActiveWaitPointInfo)).GetAllAssets<NonActiveWaitPointInfo>();
				foreach (NonActiveWaitPointInfo nonActiveWaitPointInfo in allAssets)
				{
					list3.AddRange(nonActiveWaitPointInfo.param);
				}
				AssetBundleManager.UnloadAssetBundle(file, true);
			});
			nonActiveWaitPointDic = list3.ToLookup((NonActiveWaitPointInfo.Param v) => v.MapNo, (NonActiveWaitPointInfo.Param v) => v).ToDictionary((IGrouping<int, NonActiveWaitPointInfo.Param> v) => v.Key, Enumerable.ToList);
		}
	}
}
