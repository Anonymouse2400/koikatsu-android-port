using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADV;
using ActionGame.Chara;
using ActionGame.MapObject;
using ActionGame.Place;
using ActionGame.Point;
using Config;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Manager;
using StrayTech;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace ActionGame
{
	public class ActionMap : BaseMap
	{
		public class NavigationInfo
		{
			public float distance;

			public int[] IDs;
		}

		public const string GATE_OBJECT_ASSET_NAME = "GatePoint";

		public const string GATE_ROOT_NAME = "GateRoot";

		private const string MAP_OBJECT_GROUP = "MapObjectGroup";

		private const string MAP_MOB_ROOT = "MapMobGroup";

		[SerializeField]
		private int _gateLinkID = -1;

		[SerializeField]
		private Cycle.Type _nowCycle;

		private bool isMobVisible = true;

		private SunLightInfo _sunLightInfo;

		private GameObject _gateObject;

		private ActionScene actScene;

		private IDisposable _subscriber;

		public MapInfo.Param Info
		{
			get
			{
				return base.infoDic[base.no];
			}
		}

		public int gateLinkID
		{
			get
			{
				return _gateLinkID;
			}
			set
			{
				_gateLinkID = value;
			}
		}

		public Cycle.Type nowCycle
		{
			get
			{
				if (actScene != null)
				{
					_nowCycle = actScene.Cycle.nowType;
				}
				return _nowCycle;
			}
			set
			{
				_nowCycle = value;
			}
		}

		public Dictionary<int, List<GateInfo>> calcGateDic { get; private set; }

		public Dictionary<int, GateInfo> gateInfoDic { get; private set; }

		public GateGroup gateGroup { get; private set; }

		public MapVisibleList mapVisibleList { get; private set; }

		public Transform mapObjectGroup { get; private set; }

		public Kind[] mapObjects { get; private set; }

		public GameObject mapMobGroup { get; private set; }

		public bool IsMobVisible
		{
			get
			{
				return isMobVisible;
			}
			set
			{
				isMobVisible = value;
			}
		}

		public SunLightInfo sunLightInfo
		{
			get
			{
				if (base.mapRoot == null)
				{
					return null;
				}
				return base.mapRoot.GetComponentCache(ref _sunLightInfo);
			}
		}

		private GameObject gateObject
		{
			get
			{
				return this.GetCacheObject(ref _gateObject, delegate
				{
					GameObject asset = AssetBundleManager.LoadAsset("map/object/00.unity3d", "GatePoint", typeof(GameObject)).GetAsset<GameObject>();
					AssetBundleManager.UnloadAssetBundle("map/object/00.unity3d", false);
					return asset;
				});
			}
		}

		public Transform warpPointTransform
		{
			get
			{
				if (base.mapRoot == null)
				{
					return null;
				}
				foreach (Transform item in base.mapRoot.transform)
				{
					if (item.CompareTag("Action/WarpPoint"))
					{
						return item;
					}
				}
				return null;
			}
		}

		public Vector3 warpPoint
		{
			get
			{
				Transform transform = warpPointTransform;
				return (!(transform == null)) ? transform.position : Vector3.zero;
			}
		}

		private Dictionary<int, Dictionary<int, List<NavigationInfo>>> navDic { get; set; }

		private Dictionary<int, List<GameObject>> bestPlaceAreas { get; set; }

		public static SunLightInfo.Info.Type CycleToType(Cycle.Type cycle)
		{
			switch (cycle)
			{
			case Cycle.Type.WakeUp:
			case Cycle.Type.Morning:
			case Cycle.Type.GotoSchool:
			case Cycle.Type.HR1:
			case Cycle.Type.Lesson1:
			case Cycle.Type.LunchTime:
			case Cycle.Type.Lesson2:
			case Cycle.Type.HR2:
			case Cycle.Type.StaffTime:
				return SunLightInfo.Info.Type.DayTime;
			case Cycle.Type.AfterSchool:
			case Cycle.Type.GotoMyHouse:
				return SunLightInfo.Info.Type.Evening;
			case Cycle.Type.MyHouse:
				return SunLightInfo.Info.Type.Night;
			default:
				return SunLightInfo.Info.Type.DayTime;
			}
		}

		public void UpdateCameraFog()
		{
			CameraEffector cameraEffector = Singleton<Game>.Instance.cameraEffector;
			cameraEffector.config.SetFog(RenderSettings.fog);
		}

		public bool IsPop(Base chara)
		{
			MapInfo.Param value;
			base.infoDic.TryGetValue(base.no, out value);
			return value != null && value.isGate && chara.isPopOK && base.no == chara.mapNo;
		}

		public override void Change(int no, Manager.Scene.Data.FadeType fadeType = Manager.Scene.Data.FadeType.InOut)
		{
			if (base.no == no)
			{
				return;
			}
			base.isMapLoading = true;
			base.prevNo = base.no;
			base.no = no;
			MapInfo.Param info = Info;
			Singleton<Manager.Scene>.Instance.LoadBaseScene(new Manager.Scene.Data
			{
				assetBundleName = info.AssetBundleName,
				levelName = info.AssetName,
				fadeType = fadeType,
				onFadeIn = FadeIn,
				onLoad = delegate
				{
					OnLoad(fadeType);
				}
			});
			if (actScene != null)
			{
				actScene.npcList.ForEach(delegate(NPC p)
				{
					p.SetActive(false);
				});
				if (actScene.fixChara != null)
				{
					actScene.fixChara.SetActive(false);
				}
				Player player = actScene.Player;
				if (player != null)
				{
					player.SetActive(false);
				}
			}
		}

		private IEnumerator FadeIn()
		{
			if (!(actScene == null))
			{
				ChangeCameraMapState();
			}
			yield break;
		}

		private IEnumerator FadeOut()
		{
			yield break;
		}

		private void OnLoad(Manager.Scene.Data.FadeType fadeType)
		{
			if (fadeType == Manager.Scene.Data.FadeType.None)
			{
				ChangeCameraMapState();
			}
		}

		private void ChangeCameraMapState()
		{
			if (actScene == null)
			{
				return;
			}
			if (MonoBehaviourSingleton<CameraSystem>.Instance != null)
			{
				CameraStateDefinitionChange cameraState = actScene.CameraState;
				if (cameraState != null && cameraState.Mode == CameraMode.Other)
				{
					MonoBehaviourSingleton<CameraSystem>.Instance.UnregisterCameraState(MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCameraStateDefinition);
				}
			}
			switch (base.no)
			{
			case 14:
			case 15:
			case 16:
			case 18:
				if (Manager.Config.ActData.ToiletTPS)
				{
					actScene.CameraState.ModeChangeForce(CameraMode.FPS);
				}
				break;
			default:
				actScene.CameraState.ModeChangePrev();
				break;
			}
		}

		public void SetCycleToSunLight()
		{
			bool flag = false;
			if (sunLightInfo != null)
			{
				flag = sunLightInfo.Set(CycleToType(nowCycle), Camera.main);
			}
			if (!flag)
			{
			}
			UpdateCameraFog();
		}

		public NavigationInfo SearchRoute(int firstMapNo, int lastMapNo, Vector3? pos = null, HashSet<int> _not = null)
		{
			return GatePositionDistanceCheck(FindNavigateList(firstMapNo, lastMapNo), pos, _not);
		}

		public void PlayerMapWarp(int mapNo, Action act = null)
		{
			Player player = actScene.Player;
			int mapNo2 = player.mapNo;
			GateInfo gateInfo = NearGate(mapNo2, mapNo, player.position);
			if (gateInfo != null)
			{
				_gateLinkID = gateInfo.linkID;
			}
			else
			{
				NavigationInfo navigationInfo = SearchRoute(mapNo2, mapNo, player.position);
				if (navigationInfo != null && !navigationInfo.IDs.IsNullOrEmpty())
				{
					_gateLinkID = gateInfoDic[navigationInfo.IDs.Last()].linkID;
				}
			}
			act.Call();
			player.mapNo = mapNo;
			Change(mapNo);
		}

		public GateInfo NearGate(int s, int e, Vector3? pos = null)
		{
			NavigationInfo navigationInfo = SearchRoute(s, e, pos);
			if (navigationInfo != null && !navigationInfo.IDs.IsNullOrEmpty())
			{
				return gateInfoDic[navigationInfo.IDs.Last()];
			}
			return null;
		}

		public void PlayerMapNearWarp(Base target, bool isNearGate = true)
		{
			Player player = actScene.Player;
			isNearGate &= target.mapNo != player.mapNo;
			GateInfo gateInfo = (isNearGate ? NearGate(target.mapNo, player.mapNo, null) : null);
			if (gateInfo != null)
			{
				target.mapNo = gateInfoDic[gateInfo.linkID].mapNo;
				target.position = gateInfo.pos;
			}
			else
			{
				GateInfo gateInfo2 = calcGateDic[player.mapNo].Shuffle().First();
				target.mapNo = gateInfo2.mapNo;
				target.position = gateInfoDic[gateInfo2.linkID].pos;
			}
		}

		public List<NavigationInfo> FindNavigateList(int mapNo, int lastMapNo)
		{
			if (mapNo == lastMapNo)
			{
				return new List<NavigationInfo>();
			}
			return navDic[mapNo][lastMapNo];
		}

		public NavigationInfo GatePositionDistanceCheck(List<NavigationInfo> routes, Vector3? position, HashSet<int> _not)
		{
			return GatePositionDistanceCheckList(routes, position, _not).FirstOrDefault();
		}

		public List<NavigationInfo> GatePositionDistanceCheckList(List<NavigationInfo> routes, Vector3? position, HashSet<int> _not)
		{
			if (!routes.Any())
			{
				return routes;
			}
			List<NavigationInfo> list = new List<NavigationInfo>();
			List<NavigationInfo> list2 = ((_not == null) ? routes : routes.Where((NavigationInfo v) => !v.IDs.Any((int id) => _not.Contains(id))).ToList());
			if (list2.IsNullOrEmpty())
			{
				list2 = routes;
			}
			foreach (NavigationInfo item in list2)
			{
				float num = item.distance;
				if (position.HasValue)
				{
					num += (gateInfoDic[item.IDs[0]].pos - position.Value).magnitude;
				}
				list.Add(new NavigationInfo
				{
					distance = num,
					IDs = item.IDs
				});
			}
			return list.OrderBy((NavigationInfo p) => p.distance).ToList();
		}

		public Vector3[] MapCalcPosition(int mapNo, int gateID, Vector3 pos, int? prevID)
		{
			List<GateInfo> list = calcGateDic[mapNo];
			GateInfo gateInfo = (prevID.HasValue ? list.Find((GateInfo gate) => gate.ID == prevID.Value) : null);
			Vector3[] value;
			if (gateInfo != null && gateInfo.calc.TryGetValue(gateID, out value))
			{
				int num = Illusion.Utils.Math.MinDistanceRouteIndex(value, pos);
				if (num != -1)
				{
					return value.Skip(num).ToArray();
				}
			}
			foreach (GateInfo item in list)
			{
				Vector3[] value2;
				if (item.calc.TryGetValue(gateID, out value2))
				{
					int num2 = Illusion.Utils.Math.MinDistanceRouteIndex(value2, pos);
					if (num2 != -1)
					{
						return value2.Skip(num2).ToArray();
					}
				}
			}
			return null;
		}

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
			gateInfoDic = new Dictionary<int, GateInfo>();
			actScene = GetComponent<ActionScene>();
			LoadCalcGate();
			LoadNavigationInfo();
			LoadBestPlaceArea();
			Resources.UnloadUnusedAssets();
		}

		protected override void Reserve(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
		{
			base.Reserve(oldScene, newScene);
			if (base.mapRoot == null || base.isMapLoading)
			{
				return;
			}
			SetCycleToSunLight();
			MapInfo.Param info = Info;
			mapVisibleList = base.mapRoot.GetComponent<MapVisibleList>();
			mapObjectGroup = base.mapRoot.transform.Find("MapObjectGroup");
			mapObjects = ((!(mapObjectGroup == null)) ? mapObjectGroup.GetComponentsInChildren<Kind>(true) : null);
			isMobVisible = true;
			mapMobGroup = base.mapRoot.transform.FindLoop("MapMobGroup");
			if (mapMobGroup != null)
			{
				Renderer[] rendereMobs = mapMobGroup.GetComponentsInChildren<Renderer>();
				if (!rendereMobs.IsNullOrEmpty())
				{
					AdditionalFunctionsSystem config = Manager.Config.AddData;
					MaterialPropertyBlock mpbMob = new MaterialPropertyBlock();
					base.mapRoot.UpdateAsObservable().Subscribe(delegate
					{
						mapMobGroup.SetActiveIfDifferent(isMobVisible && config.mobVisible);
						mpbMob.SetColor(ChaShader._Color, config.mobColor);
						Renderer[] array = rendereMobs;
						foreach (Renderer renderer in array)
						{
							renderer.SetPropertyBlock(mpbMob);
						}
					});
				}
			}
			int num = base.no;
			switch (num)
			{
			case 14:
			case 15:
			case 16:
			case 45:
			{
				List<Tuple<OffMeshLink, NavMeshObstacle>> list = new List<Tuple<OffMeshLink, NavMeshObstacle>>();
				foreach (GameObject item in from p in base.mapRoot.Children()
					where p.name.IndexOf("DoorCloseEvent") != -1
					select p)
				{
					Door door = item.GetComponent<Door>();
					if (door == null)
					{
						continue;
					}
					(from _ in door.UpdateAsObservable().DistinctUntilChanged((Unit _) => door.isHit)
						where !door.isHit
						select _).Subscribe(delegate
					{
						door.OpenForce();
					});
					NavMeshObstacle obstacle = door.GetObstacle();
					if (!(obstacle == null))
					{
						OffMeshLink component = item.GetComponent<OffMeshLink>();
						if (!(component == null))
						{
							list.Add(Tuple.Create(component, obstacle));
						}
					}
				}
				list.Select((Tuple<OffMeshLink, NavMeshObstacle> p) => from _ in p.Item1.UpdateAsObservable().DistinctUntilChanged((Unit _) => p.Item2.enabled)
					select p).Merge().Subscribe(delegate(Tuple<OffMeshLink, NavMeshObstacle> item)
				{
					if (!(item.Item2 == null))
					{
						float carvingTimeToStationary = item.Item2.carvingTimeToStationary;
						Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(carvingTimeToStationary + 0.1f)).Subscribe(delegate
						{
							list.ForEach(delegate(Tuple<OffMeshLink, NavMeshObstacle> p)
							{
								if (!(p.Item1 == null))
								{
									p.Item1.enabled = false;
									p.Item1.enabled = true;
								}
							});
						});
					}
				})
					.AddTo(base.mapRoot);
				break;
			}
			}
			gateGroup = base.mapRoot.GetComponent<GateGroup>();
			if (gateGroup == null)
			{
				return;
			}
			CreateGate(info);
			CreateActionPoint(info);
			CharaSetting(info);
			foreach (KeyValuePair<int, List<GameObject>> bestPlaceArea in bestPlaceAreas)
			{
				int key = bestPlaceArea.Key;
				foreach (GameObject item2 in bestPlaceArea.Value)
				{
					item2.SetActiveIfDifferent(key == base.no);
				}
			}
			Singleton<PlayerAction>.Instance.LoadPoint();
			if (_subscriber != null)
			{
				_subscriber.Dispose();
			}
			_subscriber = (from _ in Observable.EveryUpdate()
				where !Singleton<Manager.Scene>.Instance.IsNowLoadingFade
				select _).Take(1).Subscribe(delegate
			{
				ResetSoundComponents();
			});
			Illusion.Game.Utils.Sound.InitMapSoundInfo();
		}

		private void CreateGate(MapInfo.Param param)
		{
			List<GateInfo> value;
			if (!calcGateDic.TryGetValue(param.No, out value))
			{
				return;
			}
			Transform transform = new GameObject("GateRoot").transform;
			transform.SetParent(gateGroup.transform, false);
			List<Gate> gateList = gateGroup.gateList;
			foreach (GateInfo item in value)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gateObject, transform, false);
				Gate component = gameObject.GetComponent<Gate>();
				component.SetData(item);
				gateList.Add(component);
			}
		}

		private void CreateActionPoint(MapInfo.Param param)
		{
			Transform rootAction = new GameObject("ActionPoints").transform;
			rootAction.SetParent(base.mapRoot.transform, false);
			CommonLib.GetAssetBundleNameListFromPath("map/actionpoint/", true).ForEach(delegate(string file)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				int result = 0;
				if (int.TryParse(fileNameWithoutExtension, out result) && result == param.No)
				{
					GameObject gameObject = CommonLib.LoadAsset<GameObject>(file, fileNameWithoutExtension, true, string.Empty);
					gameObject.transform.SetParent(rootAction, false);
					AssetBundleManager.UnloadAssetBundle(file, true);
				}
			});
		}

		private void CharaSetting(MapInfo.Param info)
		{
			if (actScene == null)
			{
				return;
			}
			Player player = actScene.Player;
			player.isLesMotionPlay = true;
			Gate gate = gateGroup.gateList.Find((Gate p) => p.ID == _gateLinkID);
			Transform playerTrans;
			if (gate != null)
			{
				playerTrans = gate.playerTrans;
			}
			else
			{
				player.HitGateReset();
				playerTrans = warpPointTransform;
			}
			if (playerTrans != null)
			{
				player.SetPositionAndRotation(playerTrans);
				actScene.CameraState.SetAngle(playerTrans.eulerAngles);
				if (player.chaser != null && player.chaser.mapNo == player.mapNo)
				{
					float num = 0.5f;
					Vector3 vector = player.rotation * Vector3.right * num;
					player.chaser.position = playerTrans.position + vector;
				}
			}
			if (!Program.isADVProcessing && !Singleton<Manager.Scene>.Instance.NowSceneNames.Contains("H"))
			{
				actScene.npcList.ForEach(delegate(NPC p)
				{
					p.SetActive(IsPop(p));
				});
				if (actScene.fixChara != null)
				{
					actScene.fixChara.SetActive(IsPop(actScene.fixChara));
				}
				player.SetActive(IsPop(player));
			}
		}

		private void LoadCalcGate()
		{
			calcGateDic = new Dictionary<int, List<GateInfo>>();
			CommonLib.GetAssetBundleNameListFromPath("map/list/calcgateinfo/", true).ForEach(delegate(string file)
			{
				ExcelData[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(ExcelData)).GetAllAssets<ExcelData>();
				foreach (ExcelData excelData in allAssets)
				{
					List<GateInfo> list = GateInfo.Create(excelData.list);
					list.ForEach(delegate(GateInfo info)
					{
						gateInfoDic[info.ID] = info;
					});
					calcGateDic[int.Parse(excelData.name)] = list;
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
		}

		private void LoadNavigationInfo()
		{
			navDic = new Dictionary<int, Dictionary<int, List<NavigationInfo>>>();
			CommonLib.GetAssetBundleNameListFromPath("map/list/navigationinfo/", true).ForEach(delegate(string file)
			{
				ExcelData[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(ExcelData)).GetAllAssets<ExcelData>();
				foreach (ExcelData excelData in allAssets)
				{
					int key = int.Parse(excelData.name);
					Dictionary<int, List<NavigationInfo>> value;
					if (!navDic.TryGetValue(key, out value))
					{
						Dictionary<int, List<NavigationInfo>> dictionary = new Dictionary<int, List<NavigationInfo>>();
						navDic[key] = dictionary;
						value = dictionary;
					}
					foreach (ExcelData.Param item in excelData.list)
					{
						int count = 0;
						List<string> list = item.list;
						int lastMapID = -1;
						list.SafeProc(count++, delegate(string s)
						{
							lastMapID = int.Parse(s);
						});
						List<NavigationInfo> value2;
						if (!value.TryGetValue(lastMapID, out value2))
						{
							List<NavigationInfo> list2 = new List<NavigationInfo>();
							value[lastMapID] = list2;
							value2 = list2;
						}
						foreach (string item2 in item.list.Skip(count))
						{
							NavigationInfo navigationInfo = new NavigationInfo();
							string[] array = item2.Split(':');
							navigationInfo.distance = float.Parse(array[0]);
							navigationInfo.IDs = array[1].Split(',').Select(int.Parse).ToArray();
							value2.Add(navigationInfo);
						}
					}
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
		}

		private void LoadBestPlaceArea()
		{
			bestPlaceAreas = new Dictionary<int, List<GameObject>>();
			if (actScene == null)
			{
				return;
			}
			string text = (from s in CommonLib.GetAssetBundleNameListFromPath("map/placearea/", true)
				orderby s descending
				select s).FirstOrDefault();
			if (text.IsNullOrEmpty())
			{
				return;
			}
			Transform transform = new GameObject("PlaceAreaRoot").transform;
			transform.SetParent(actScene.transform, false);
			GameObject[] allAssets = AssetBundleManager.LoadAllAsset(text, typeof(GameObject)).GetAllAssets<GameObject>();
			foreach (GameObject gameObject in allAssets)
			{
				Area component = gameObject.GetComponent<Area>();
				if (!(component == null))
				{
					int mapNo = component.mapNo;
					List<GameObject> value;
					if (!bestPlaceAreas.TryGetValue(mapNo, out value))
					{
						value = (bestPlaceAreas[mapNo] = new List<GameObject>());
					}
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform, false);
					gameObject2.name = gameObject.name;
					value.Add(gameObject2);
				}
			}
			AssetBundleManager.UnloadAssetBundle(text, false);
		}

		private void ResetSoundComponents()
		{
			if (base.no == 37 || base.no == 45)
			{
				Illusion.Game.Utils.Sound.FootStepAreaDefaultType = FootStepSE.Pool;
			}
			else if (base.no == 28)
			{
				Illusion.Game.Utils.Sound.FootStepAreaDefaultType = FootStepSE.Ground;
			}
			else if (base.no == 32)
			{
				Illusion.Game.Utils.Sound.FootStepAreaDefaultType = FootStepSE.Gymnasium;
			}
			else
			{
				Illusion.Game.Utils.Sound.FootStepAreaDefaultType = FootStepSE.Default;
			}
			int[] array = Illusion.Game.Utils.Sound.FootStepAreaTypes.Keys.ToArray();
			int[] array2 = array;
			foreach (int key in array2)
			{
				Illusion.Game.Utils.Sound.FootStepAreaTypes[key] = Illusion.Game.Utils.Sound.FootStepAreaDefaultType;
			}
			FootStepSEArea[] componentsInChildren = base.mapRoot.GetComponentsInChildren<FootStepSEArea>();
			FootStepSEArea[] array3 = componentsInChildren;
			foreach (FootStepSEArea footStepSEArea in array3)
			{
				footStepSEArea.ForceUpdate();
			}
			BGMArea.PlayInfo value;
			bool flag = Illusion.Game.Utils.Sound.MapBGMTable.TryGetValue(base.no, out value);
			Action<BGM> action = delegate(BGM bgmID)
			{
				GameObject gameObject = new GameObject("BGM");
				gameObject.transform.SetParent(base.mapRoot.transform, false);
				BGMArea bGMArea = gameObject.AddComponent<BGMArea>();
				bGMArea.BGM = bgmID;
			};
			if (actScene != null && !actScene.IsInChargeBGM && actScene.Cycle.isAction)
			{
				BGM obj = BGM.Daytime;
				bool flag2 = false;
				if (flag)
				{
					if (value.BGMID > -1)
					{
						obj = (BGM)value.BGMID;
						flag2 = true;
					}
				}
				else
				{
					switch (nowCycle)
					{
					case Cycle.Type.LunchTime:
					case Cycle.Type.StaffTime:
						obj = BGM.MapMoveDay;
						flag2 = true;
						break;
					case Cycle.Type.AfterSchool:
						obj = BGM.MapMoveEve;
						flag2 = true;
						break;
					}
				}
				if (actScene.npcList != null)
				{
					foreach (NPC npc in actScene.npcList)
					{
						if (npc.mapNo != base.no || (!npc.isOnanism && !npc.isLesbian))
						{
							continue;
						}
						obj = BGM.HScenePeep;
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					action(obj);
				}
			}
			Action<float> action2 = delegate(float volume)
			{
				GameObject gameObject2 = new GameObject("BGMVolume");
				gameObject2.transform.SetParent(base.mapRoot.transform, false);
				BGMVolume bGMVolume = gameObject2.AddComponent<BGMVolume>();
				bGMVolume.Volume = volume;
			};
			if (actScene != null && actScene.Cycle.isAction)
			{
				if (flag)
				{
					if (value.EnableVolumeModification)
					{
						action2(value.Volume);
					}
				}
				else
				{
					action2(1f);
				}
			}
			string key2 = string.Format("EnvArea_{0:00}", base.no);
			GameObject value2;
			if (Illusion.Game.Utils.Sound.EnvAreaTable.TryGetValue(key2, out value2))
			{
				UnityEngine.Object.Instantiate(value2, base.mapRoot.transform);
			}
		}
	}
}
