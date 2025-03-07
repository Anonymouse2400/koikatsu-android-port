using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ADV;
using ActionGame;
using ActionGame.Chara;
using Illusion.Extensions;
using IllusionUtility.GetUtility;
using StrayTech;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Manager
{
	public class PlayerAction : Singleton<PlayerAction>
	{
		private class WaitTime
		{
			private float nextFrameTime;

			public bool isOver
			{
				get
				{
					return Time.realtimeSinceStartup >= nextFrameTime;
				}
			}

			public WaitTime()
			{
				Next();
			}

			public void Next()
			{
				nextFrameTime = Time.realtimeSinceStartup + 0.03f;
			}
		}

		private class AdditionValue
		{
			public int min { get; private set; }

			public int max { get; private set; }

			public int value
			{
				get
				{
					return UnityEngine.Random.Range(min, max + 1);
				}
			}

			public AdditionValue(string _str)
			{
				string[] array = _str.Split(',');
				min = int.Parse(array[0]);
				max = int.Parse(array[1]);
			}
		}

		private class ObjectInfo
		{
			public string bundle { get; private set; }

			public string file { get; private set; }

			public string manifest { get; private set; }

			public ObjectInfo(string _bundle, string _file, string _manifest)
			{
				bundle = _bundle;
				file = _file;
				manifest = _manifest;
			}
		}

		private class ActionInfo
		{
			public string name { get; private set; }

			public AdditionValue[] add { get; private set; }

			public string text { get; private set; }

			public bool isDark { get; private set; }

			public int drop { get; private set; }

			public string state { get; private set; }

			public string target { get; private set; }

			public Vector3 move { get; private set; }

			public Vector3 rot { get; private set; }

			public List<ObjectInfo> items { get; private set; }

			public int kind
			{
				get
				{
					int key = add.Max((AdditionValue v) => v.max);
					return Array.FindIndex(add, (AdditionValue v) => v.max == key);
				}
			}

			public ActionInfo(List<string> _list)
			{
				int num = 1;
				name = _list[num++];
				add = new AdditionValue[3];
				for (int i = 0; i < 3; i++)
				{
					add[i] = new AdditionValue(_list[num++]);
				}
				this.text = _list[num++];
				isDark = bool.Parse(_list[num++]);
				drop = int.Parse(_list[num++]);
				state = _list[num++];
				target = _list.SafeGet(num++);
				move = GetVector3(_list, num);
				num += 3;
				rot = GetVector3(_list, num);
				num += 3;
				items = new List<ObjectInfo>();
				while (num + 2 <= _list.Count)
				{
					string text = _list.SafeGet(num++);
					string text2 = _list.SafeGet(num++);
					string manifest = _list.SafeGet(num++);
					if (!text.IsNullOrEmpty() && !text2.IsNullOrEmpty())
					{
						items.Add(new ObjectInfo(text, text2, manifest));
					}
				}
			}

			private Vector3 GetVector3(List<string> _list, int _startIdx)
			{
				float result = 0f;
				float result2 = 0f;
				float result3 = 0f;
				float.TryParse(_list.SafeGet(_startIdx), out result);
				float.TryParse(_list.SafeGet(_startIdx + 1), out result2);
				float.TryParse(_list.SafeGet(_startIdx + 2), out result3);
				return new Vector3(result, result2, result3);
			}
		}

		private class PointInfo
		{
			public string name { get; private set; }

			public int mapNo { get; private set; }

			public int[] action { get; private set; }

			public int actionNo
			{
				get
				{
					return action.OrderBy((int i) => Guid.NewGuid()).First();
				}
			}

			public PointInfo(List<string> _list)
			{
				int num = 1;
				name = _list[num++];
				mapNo = int.Parse(_list[num++]);
				string text = _list[num++];
				action = text.Split(',').Select(int.Parse).ToArray();
			}
		}

		private class ObjectTrans
		{
			private Transform target;

			private Vector3 backupPosition = Vector3.zero;

			private Quaternion backupRotation = Quaternion.identity;

			public ObjectTrans(Transform _target)
			{
				target = _target;
				backupPosition = target.position;
				backupRotation = target.rotation;
			}

			public void Restore()
			{
				if (target != null)
				{
					target.SetPositionAndRotation(backupPosition, backupRotation);
				}
			}
		}

		private class DropInfo
		{
			private int _hNo = -1;

			private int _conditions = -1;

			public string name { get; private set; }

			public int hNo
			{
				get
				{
					return _hNo;
				}
			}

			public int conditions
			{
				get
				{
					return _conditions;
				}
			}

			public DropInfo(List<string> _list)
			{
				name = _list.SafeGet(1);
				int.TryParse(_list.SafeGet(2), out _hNo);
				int.TryParse(_list.SafeGet(3), out _conditions);
			}
		}

		public class ADVParam : SceneParameter
		{
			public bool isInVisibleChara = true;

			public ADVParam(MonoBehaviour scene)
				: base(scene)
			{
			}

			public override void Init(Data data)
			{
				ADVScene aDVScene = SceneParameter.advScene;
				TextScenario scenario = aDVScene.Scenario;
				scenario.heroineList = data.heroineList;
				scenario.transferList = data.transferList;
				scenario.LoadBundleName = string.Empty;
				scenario.LoadAssetName = string.Empty;
				aDVScene.Stand.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
				scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
				scenario.BackCamera.transform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
				float fadeInTime = data.fadeInTime;
				if (fadeInTime > 0f)
				{
					aDVScene.fadeTime = fadeInTime;
				}
				else
				{
					isInVisibleChara = false;
				}
			}

			public override void Release()
			{
				ADVScene aDVScene = SceneParameter.advScene;
				aDVScene.gameObject.SetActive(false);
			}

			public override void WaitEndProc()
			{
			}
		}

		private WaitTime waitTime;

		private Dictionary<int, ActionInfo> dicActionInfo;

		private Dictionary<int, PointInfo> dicPointInfo;

		private Dictionary<int, ObjectInfo> dicPointLoadInfo;

		private Dictionary<int, DropInfo> dicDropInfo;

		private ActionInfo nowActionInfo;

		private Dictionary<int, bool> dicAlive;

		private GameObject objPoint;

		private List<ObjectTrans> lstRestore;

		private List<GameObject> lstItem = new List<GameObject>();

		private bool isOpenADV;

		private CrossFade crossFade;

		public bool isInit { get; private set; }

		public bool isNext { get; set; }

		public static bool isNextStatic
		{
			get
			{
				return Singleton<PlayerAction>.IsInstance() && Singleton<PlayerAction>.Instance.isNext;
			}
			set
			{
				if (Singleton<PlayerAction>.IsInstance())
				{
					Singleton<PlayerAction>.Instance.isNext = value;
				}
			}
		}

		public void LoadPoint(int _mapNo = -1)
		{
			ReleasePoint();
			_mapNo = ((_mapNo != -1) ? _mapNo : ((!(Singleton<Game>.Instance.actScene != null)) ? (-1) : Singleton<Game>.Instance.actScene.Map.no));
			ObjectInfo value = null;
			if (dicPointLoadInfo.TryGetValue(_mapNo, out value))
			{
				objPoint = CommonLib.LoadAsset<GameObject>(value.bundle, value.file, true, value.manifest);
			}
		}

		public void ReleasePoint()
		{
			if ((bool)objPoint)
			{
				UnityEngine.Object.Destroy(objPoint);
			}
			objPoint = null;
		}

		public void NextTime(int _aliveNum = 5)
		{
			dicAlive = dicPointInfo.ToDictionary((KeyValuePair<int, PointInfo> v) => v.Key, (KeyValuePair<int, PointInfo> v) => false);
			int[] array = dicPointInfo.Keys.OrderBy((int key) => Guid.NewGuid()).ToArray();
			List<int> list = new List<int>();
			int[] array2 = array;
			foreach (int key2 in array2)
			{
				if (!list.Contains(dicPointInfo[key2].mapNo))
				{
					dicAlive[key2] = true;
					list.Add(dicPointInfo[key2].mapNo);
				}
			}
			isNext = true;
		}

		public bool CheckAlive(int _no)
		{
			return dicAlive == null || (dicAlive.ContainsKey(_no) && dicAlive[_no]);
		}

		public void Action(Transform _trans, int _no)
		{
			PointInfo value = null;
			if (!dicPointInfo.TryGetValue(_no, out value))
			{
				return;
			}
			SaveData.Player player = Singleton<Game>.Instance.Player;
			player.actionCount--;
			if (dicAlive != null && dicAlive.Count != 0)
			{
				if (player.actionCount <= 0)
				{
					dicAlive = dicAlive.ToDictionary((KeyValuePair<int, bool> v) => v.Key, (KeyValuePair<int, bool> v) => false);
				}
				else
				{
					dicAlive[_no] = false;
				}
			}
			int actionNo = value.actionNo;
			ActionInfo info = null;
			if (!dicActionInfo.TryGetValue(actionNo, out info))
			{
				return;
			}
			nowActionInfo = info;
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			Player player2 = actScene.Player;
			player2.agent.enabled = false;
			player2.isActionNow = true;
			if (info.isDark)
			{
				Dark(info.text);
				return;
			}
			player2.motion.state = info.state;
			player2.motion.Play(player2.animator);
			if (!info.items.IsNullOrEmpty())
			{
				lstItem = new List<GameObject>();
				foreach (ObjectInfo item in info.items)
				{
					GameObject gameObject = CommonLib.LoadAsset<GameObject>(item.bundle, item.file, true, item.manifest);
					if (gameObject == null)
					{
					}
					gameObject.transform.SetPositionAndRotation(_trans.position, _trans.rotation);
					lstItem.Add(gameObject);
				}
			}
			crossFade = Camera.main.GetComponent<CrossFade>();
			crossFade.FadeStart();
			lstRestore = new List<ObjectTrans>();
			lstRestore.Add(new ObjectTrans(player2.transform));
			player2.SetPositionAndRotation(_trans);
			FirstPersonActionCamera fPSCamera = actScene.CameraState.FPSCamera;
			bool isFPS = fPSCamera != null;
			if (isFPS)
			{
				fPSCamera.Angle = player2.eulerAngles;
				actScene.CameraState.ModeChangeForce(CameraMode.TPS, false);
				(from _ in this.UpdateAsObservable()
					select actScene.CameraState.TPSCamera into tpsCam
					where tpsCam != null
					select tpsCam).Take(1).Subscribe(delegate(ThirdPersonActionCamera tpsCam)
				{
					tpsCam.Angle = Quaternion.LookRotation(-player2.cachedTransform.forward).eulerAngles;
				});
			}
			if (!info.target.IsNullOrEmpty())
			{
				GameObject gameObject2 = actScene.Map.mapRoot.transform.FindLoop(info.target);
				if (gameObject2 != null)
				{
					lstRestore.Add(new ObjectTrans(gameObject2.transform));
					gameObject2.transform.position = info.move;
					gameObject2.transform.eulerAngles = info.rot;
				}
			}
			(from _ in this.UpdateAsObservable()
				where !player2.isActive || player2.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f
				select _).Take(1).Subscribe(delegate
			{
				if (isFPS)
				{
					actScene.CameraState.ModeChangeForce(CameraMode.FPS, false);
				}
				if (player2.isActive)
				{
					player2.agent.enabled = true;
					player2.isActionNow = false;
				}
				crossFade.FadeStart();
				player2.PlayAnimation();
				if (!info.items.IsNullOrEmpty())
				{
					for (int i = 0; i < lstItem.Count; i++)
					{
						UnityEngine.Object.Destroy(lstItem[i]);
					}
					lstItem.Clear();
				}
				foreach (ObjectTrans item2 in lstRestore)
				{
					item2.Restore();
				}
				lstRestore.Clear();
				ParameterList.Remove(this);
				AddParam();
			});
		}

		private void Dark(string _text)
		{
			ParameterList.Add(new ADVParam(this));
			List<Program.Transfer> list = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, list);
			list.Add(Program.Transfer.Create(false, Command.Fade, "in", "0.5", "black", "back", "TRUE"));
			list.Add(Program.Transfer.Text(string.Empty, _text));
			list.Add(Program.Transfer.Create(false, Command.TextClear));
			list.Add(Program.Transfer.Create(false, Command.WindowActive, "FALSE", "0"));
			list.Add(Program.Transfer.Create(false, Command.Fade, "out", "0.5", "black", "back", "TRUE"));
			list.Add(Program.Transfer.Close());
			isOpenADV = false;
			StartCoroutine(Program.Open(new Data
			{
				scene = this,
				transferList = list
			}, new Program.OpenDataProc
			{
				onLoad = delegate
				{
					isOpenADV = true;
				}
			}));
			StartCoroutine("WaitDark");
		}

		private IEnumerator WaitDark()
		{
			yield return null;
			yield return new WaitUntil(() => isOpenADV);
			yield return Program.ADVProcessingCheck();
			Player player = Singleton<Game>.Instance.actScene.Player;
			player.agent.enabled = true;
			player.isActionNow = false;
			ParameterList.Remove(this);
			AddParam();
		}

		private void AddParam()
		{
			SaveData.Player pl = Singleton<Game>.Instance.Player;
			int[] array = nowActionInfo.add.Select((AdditionValue v) => v.value).ToArray();
			bool flag = Singleton<Game>.Instance.actScene.Player.chaser != null;
			if (flag)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] < 0)
					{
						array[i] = 0;
					}
				}
			}
			pl.intellect = Mathf.Clamp(pl.intellect + array[0], 0, 100);
			pl.physical = Mathf.Clamp(pl.physical + array[1], 0, 100);
			pl.hentai = Mathf.Clamp(pl.hentai + array[2], 0, 100);
			GameObject gameObject = CommonLib.LoadAsset<GameObject>("action/playeraction/00.unity3d", "paramupdate", true, string.Empty);
			ParamUpdateUI component = gameObject.GetComponent<ParamUpdateUI>();
			component.physical = array[1];
			component.intellect = array[0];
			component.hentai = array[2];
			if (flag && nowActionInfo.drop != 0)
			{
				SaveData saveData = Singleton<Game>.Instance.saveData;
				if (!saveData.clubContents.ContainsKey(1))
				{
					saveData.clubContents[1] = new HashSet<int>();
				}
				HashSet<int> clubContents = saveData.clubContents[1];
				KeyValuePair<int, DropInfo> value = (from v in dicDropInfo
					where !clubContents.Contains(v.Value.hNo + 1000)
					where pl.hentai >= v.Value.conditions
					select v).Shuffle().FirstOrDefault();
				if (!value.IsDefault())
				{
					clubContents.Add(value.Value.hNo + 1000);
					component.drop = 1;
				}
			}
			switch (nowActionInfo.kind)
			{
			case 0:
				Singleton<Game>.Instance.rankSaveData.intellectCount++;
				break;
			case 1:
				Singleton<Game>.Instance.rankSaveData.physicalCount++;
				break;
			case 2:
				Singleton<Game>.Instance.rankSaveData.hentaiCount++;
				break;
			}
		}

		private IEnumerator LoadInfo()
		{
			waitTime = new WaitTime();
			isInit = false;
			List<string> pathList = (from s in CommonLib.GetAssetBundleNameListFromPath("action/playeraction/", true)
				where Regex.Match(Path.GetFileNameWithoutExtension(s), "(\\d*)").Success
				select s).ToList();
			pathList.Sort();
			if (waitTime.isOver)
			{
				yield return null;
				waitTime.Next();
			}
			foreach (string path in pathList)
			{
				LoadActionInfo(path);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadPointInfo(path);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadPointLoadInfo(path);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
				LoadDropInfo(path);
				if (waitTime.isOver)
				{
					yield return null;
					waitTime.Next();
				}
			}
			waitTime = null;
			isInit = true;
		}

		private void LoadActionInfo(string _path)
		{
			string assetName = "action_" + Path.GetFileNameWithoutExtension(_path);
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, assetName, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(3)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item[0], out result))
				{
					dicActionInfo.Add(result, new ActionInfo(item));
				}
			}
		}

		private void LoadPointInfo(string _path)
		{
			string assetName = "point_" + Path.GetFileNameWithoutExtension(_path);
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, assetName, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item[0], out result))
				{
					if (dicPointInfo.ContainsKey(result))
					{
						dicPointInfo[result] = new PointInfo(item);
					}
					else
					{
						dicPointInfo.Add(result, new PointInfo(item));
					}
				}
			}
		}

		private void LoadPointLoadInfo(string _path)
		{
			string assetName = "load_" + Path.GetFileNameWithoutExtension(_path);
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, assetName, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item[0], out result))
				{
					dicPointLoadInfo.Add(result, new ObjectInfo(item[1], item[2], item.SafeGet(3)));
				}
			}
		}

		private void LoadDropInfo(string _path)
		{
			string assetName = "drop_" + Path.GetFileNameWithoutExtension(_path);
			ExcelData excelData = CommonLib.LoadAsset<ExcelData>(_path, assetName, false, string.Empty);
			if (excelData == null)
			{
				return;
			}
			foreach (List<string> item in from p in excelData.list.Skip(1)
				select p.list)
			{
				int result = -1;
				if (int.TryParse(item[0], out result))
				{
					dicDropInfo[result] = new DropInfo(item);
				}
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				dicActionInfo = new Dictionary<int, ActionInfo>();
				dicPointInfo = new Dictionary<int, PointInfo>();
				dicPointLoadInfo = new Dictionary<int, ObjectInfo>();
				dicDropInfo = new Dictionary<int, DropInfo>();
				StartCoroutine("LoadInfo");
			}
		}
	}
}
