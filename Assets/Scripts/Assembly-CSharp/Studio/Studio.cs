using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion.Elements.Xml;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public sealed class Studio : Singleton<Studio>
	{
		private class DuplicateParentInfo
		{
			public ObjectCtrlInfo info;

			public TreeNodeObject node;

			public DuplicateParentInfo(ObjectCtrlInfo _info, TreeNodeObject _node)
			{
				info = _info;
				node = _node;
			}
		}

		public const string savePath = "studio/scene";

		[SerializeField]
		private TreeNodeCtrl m_TreeNodeCtrl;

		[SerializeField]
		private RootButtonCtrl m_RootButtonCtrl;

		[SerializeField]
		private ManipulatePanelCtrl _manipulatePanelCtrl;

		[SerializeField]
		private CameraControl m_CameraCtrl;

		[SerializeField]
		private SystemButtonCtrl m_SystemButtonCtrl;

		[SerializeField]
		private CameraLightCtrl m_CameraLightCtrl;

		[SerializeField]
		private MapList _mapList;

		[SerializeField]
		private ColorPalette _colorPalette;

		[SerializeField]
		private WorkspaceCtrl m_WorkspaceCtrl;

		[SerializeField]
		private BackgroundCtrl m_BackgroundCtrl;

		[SerializeField]
		private BackgroundList m_BackgroundList;

		[SerializeField]
		private PatternSelectListCtrl _patternSelectListCtrl;

		[SerializeField]
		private GameScreenShot _gameScreenShot;

		[SerializeField]
		private FrameCtrl _frameCtrl;

		[SerializeField]
		private FrameList _frameList;

		[SerializeField]
		private LogoList _logoList;

		[SerializeField]
		private InputField _inputFieldNow;

		[SerializeField]
		private TMP_InputField _inputFieldTMPNow;

		[SerializeField]
		[Space]
		private RectTransform rectName;

		[SerializeField]
		private TextMeshProUGUI textName;

		private SingleAssignmentDisposable disposableName;

		[SerializeField]
		[Space]
		private Sprite spriteLight;

		[SerializeField]
		[Space]
		private Image imageCamera;

		[SerializeField]
		private TextMeshProUGUI textCamera;

		[SerializeField]
		private CameraSelector _cameraSelector;

		[Space]
		[SerializeField]
		private Texture _textureLine;

		[SerializeField]
		private RouteControl _routeControl;

		public Dictionary<TreeNodeObject, ObjectCtrlInfo> dicInfo = new Dictionary<TreeNodeObject, ObjectCtrlInfo>();

		public Dictionary<int, ObjectCtrlInfo> dicObjectCtrl = new Dictionary<int, ObjectCtrlInfo>();

		public Dictionary<int, ChangeAmount> dicChangeAmount = new Dictionary<int, ChangeAmount>();

		private const string UserPath = "studio";

		private const string FileName = "option.xml";

		private const string RootName = "Option";

		private Control xmlCtrl;

		private OCICamera _ociCamera;

		public Action<ObjectCtrlInfo> onDelete;

		public Action onChangeMap;

		public WorkInfo workInfo = new WorkInfo();

		public TreeNodeCtrl treeNodeCtrl
		{
			get
			{
				return m_TreeNodeCtrl;
			}
		}

		public RootButtonCtrl rootButtonCtrl
		{
			get
			{
				return m_RootButtonCtrl;
			}
		}

		public ManipulatePanelCtrl manipulatePanelCtrl
		{
			get
			{
				return _manipulatePanelCtrl;
			}
		}

		public CameraControl cameraCtrl
		{
			get
			{
				return m_CameraCtrl;
			}
		}

		public SystemButtonCtrl systemButtonCtrl
		{
			get
			{
				return m_SystemButtonCtrl;
			}
		}

		public BGMCtrl bgmCtrl
		{
			get
			{
				return sceneInfo.bgmCtrl;
			}
		}

		public ENVCtrl envCtrl
		{
			get
			{
				return sceneInfo.envCtrl;
			}
		}

		public OutsideSoundCtrl outsideSoundCtrl
		{
			get
			{
				return sceneInfo.outsideSoundCtrl;
			}
		}

		public CameraLightCtrl cameraLightCtrl
		{
			get
			{
				return m_CameraLightCtrl;
			}
		}

		public MapList mapList
		{
			get
			{
				return _mapList;
			}
		}

		public ColorPalette colorPalette
		{
			get
			{
				return _colorPalette;
			}
		}

		public PatternSelectListCtrl patternSelectListCtrl
		{
			get
			{
				return _patternSelectListCtrl;
			}
		}

		public GameScreenShot gameScreenShot
		{
			get
			{
				return _gameScreenShot;
			}
		}

		public FrameCtrl frameCtrl
		{
			get
			{
				return _frameCtrl;
			}
		}

		public LogoList logoList
		{
			get
			{
				return _logoList;
			}
		}

		public bool isInputNow
		{
			get
			{
				return _inputFieldNow ? _inputFieldNow.isFocused : ((bool)_inputFieldTMPNow && _inputFieldTMPNow.isFocused);
			}
		}

		public CameraSelector cameraSelector
		{
			get
			{
				return _cameraSelector;
			}
		}

		public Texture textureLine
		{
			get
			{
				return _textureLine;
			}
		}

		public RouteControl routeControl
		{
			get
			{
				return _routeControl;
			}
		}

		public SceneInfo sceneInfo { get; private set; }

		public static OptionSystem optionSystem { get; private set; }

		public OCICamera ociCamera
		{
			get
			{
				return _ociCamera;
			}
			private set
			{
				_ociCamera = value;
			}
		}

		public int cameraCount { get; private set; }

		public bool isVRMode { get; private set; }

		public void AddFemale(string _path)
		{
			OCICharFemale oCICharFemale = AddObjectFemale.Add(_path);
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoHide)
			{
				rootButtonCtrl.OnClick(-1);
			}
			if (optionSystem.autoSelect && oCICharFemale != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCICharFemale.treeNodeObject);
			}
		}

		private IEnumerator AddFemaleCoroutine(string _path)
		{
			AddObjectFemale.NecessaryInfo ni = new AddObjectFemale.NecessaryInfo(_path);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioWait",
				isAdd = true
			}, false);
			yield return null;
			yield return AddObjectFemale.AddCoroutine(ni);
			Singleton<Scene>.Instance.UnLoad();
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoHide)
			{
				rootButtonCtrl.OnClick(-1);
			}
			if (optionSystem.autoSelect && ni.ocicf != null)
			{
				m_TreeNodeCtrl.SelectSingle(ni.ocicf.treeNodeObject);
			}
		}

		public void AddMale(string _path)
		{
			OCICharMale oCICharMale = AddObjectMale.Add(_path);
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoHide)
			{
				rootButtonCtrl.OnClick(-1);
			}
			if (optionSystem.autoSelect && oCICharMale != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCICharMale.treeNodeObject);
			}
		}

		public void AddMap(int _no, bool _close = true, bool _wait = true, bool _coroutine = true)
		{
			if (_coroutine)
			{
				StartCoroutine(AddMapCoroutine(_no, _close, _wait));
				return;
			}
			Singleton<Map>.Instance.LoadMap(_no);
			SetupMap(_no, _close);
		}

		private IEnumerator AddMapCoroutine(int _no, bool _close, bool _wait)
		{
			yield return Singleton<Map>.Instance.LoadMapCoroutine(_no, _wait);
			SetupMap(_no, _close);
		}

		private void SetupMap(int _no, bool _close)
		{
			sceneInfo.map = _no;
			sceneInfo.caMap.Reset();
			if (onChangeMap != null)
			{
				onChangeMap();
			}
			m_CameraCtrl.CloerListCollider();
			Info.MapLoadInfo value = null;
			if (Singleton<Info>.Instance.dicMapLoadInfo.TryGetValue(Singleton<Map>.Instance.no, out value))
			{
				m_CameraCtrl.LoadVanish(value.vanish.bundlePath, value.vanish.fileName, Singleton<Map>.Instance.mapRoot);
			}
			if (_close)
			{
				rootButtonCtrl.OnClick(-1);
			}
		}

		public void AddItem(int _group, int _category, int _no)
		{
			OCIItem oCIItem = AddObjectItem.Add(_group, _category, _no);
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoHide)
			{
				rootButtonCtrl.OnClick(-1);
			}
			if (optionSystem.autoSelect && oCIItem != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCIItem.treeNodeObject);
			}
		}

		public void AddLight(int _no)
		{
			if (!sceneInfo.isLightCheck)
			{
				NotificationScene.spriteMessage = spriteLight;
				NotificationScene.waitTime = 1f;
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = "StudioNotification",
					isAdd = true
				}, false);
				return;
			}
			OCILight oCILight = AddObjectLight.Add(_no);
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoHide)
			{
				rootButtonCtrl.OnClick(-1);
			}
			if (optionSystem.autoSelect && oCILight != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCILight.treeNodeObject);
			}
		}

		public void AddFolder()
		{
			OCIFolder oCIFolder = AddObjectFolder.Add();
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoSelect && oCIFolder != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCIFolder.treeNodeObject);
			}
		}

		public void AddCamera()
		{
			if (cameraCount != int.MaxValue)
			{
				cameraCount++;
			}
			OCICamera oCICamera = AddObjectCamera.Add();
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoSelect && oCICamera != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCICamera.treeNodeObject);
			}
			_cameraSelector.Init();
		}

		public void ChangeCamera(OCICamera _ociCamera, bool _active, bool _force = false)
		{
			if (_active)
			{
				if (ociCamera != null && ociCamera != _ociCamera)
				{
					ociCamera.SetActive(false);
					ociCamera = null;
				}
				if (_ociCamera != null)
				{
					_ociCamera.SetActive(true);
					ociCamera = _ociCamera;
				}
			}
			else if (_force)
			{
				if (ociCamera != null)
				{
					ociCamera.SetActive(false);
				}
				if (_ociCamera != null)
				{
					_ociCamera.SetActive(false);
				}
				ociCamera = null;
			}
			else if (ociCamera == _ociCamera)
			{
				if (_ociCamera != null)
				{
					_ociCamera.SetActive(false);
				}
				ociCamera = null;
			}
			Singleton<Studio>.Instance.cameraCtrl.enabled = ociCamera == null;
			textCamera.text = ((ociCamera != null) ? ociCamera.cameraInfo.name : "-");
			_cameraSelector.SetCamera(ociCamera);
		}

		public void ChangeCamera(OCICamera _ociCamera)
		{
			ChangeCamera(_ociCamera, ociCamera != _ociCamera);
		}

		public void DeleteCamera(OCICamera _ociCamera)
		{
			if (ociCamera != _ociCamera)
			{
				_cameraSelector.Init();
				return;
			}
			ociCamera.SetActive(false);
			ociCamera = null;
			Singleton<Studio>.Instance.cameraCtrl.enabled = true;
			textCamera.text = "-";
			_cameraSelector.Init();
		}

		public void AddRoute()
		{
			OCIRoute oCIRoute = AddObjectRoute.Add();
			if (_routeControl.visible)
			{
				_routeControl.Init();
			}
			Singleton<UndoRedoManager>.Instance.Clear();
			if (optionSystem.autoSelect && oCIRoute != null)
			{
				m_TreeNodeCtrl.SelectSingle(oCIRoute.treeNodeObject);
			}
		}

		public void SetACE(int _no)
		{
			m_SystemButtonCtrl.SetACE(_no);
		}

		public void SetSunCaster(int _key)
		{
			m_SystemButtonCtrl.SetSunCaster(_key);
		}

		public void UpdateCharaFKColor()
		{
			foreach (KeyValuePair<int, ObjectCtrlInfo> item in dicObjectCtrl.Where((KeyValuePair<int, ObjectCtrlInfo> v) => v.Value is OCIChar))
			{
				(item.Value as OCIChar).UpdateFKColor(FKCtrl.parts);
			}
		}

		public void UpdateItemFKColor()
		{
			foreach (KeyValuePair<int, ObjectCtrlInfo> item in dicObjectCtrl.Where((KeyValuePair<int, ObjectCtrlInfo> v) => v.Value is OCIItem))
			{
				(item.Value as OCIItem).UpdateFKColor();
			}
		}

		public void Duplicate()
		{
			Dictionary<int, ObjectInfo> dictionary = new Dictionary<int, ObjectInfo>();
			Dictionary<int, DuplicateParentInfo> dictionary2 = new Dictionary<int, DuplicateParentInfo>();
			TreeNodeObject[] selectNodes = treeNodeCtrl.selectNodes;
			for (int i = 0; i < selectNodes.Length; i++)
			{
				SavePreprocessingLoop(selectNodes[i]);
				ObjectCtrlInfo value = null;
				if (dicInfo.TryGetValue(selectNodes[i], out value))
				{
					dictionary.Add(value.objectInfo.dicKey, value.objectInfo);
					if (value.parentInfo != null)
					{
						dictionary2.Add(value.objectInfo.dicKey, new DuplicateParentInfo(value.parentInfo, value.treeNodeObject.parent));
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return;
			}
			byte[] buffer = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					sceneInfo.Save(writer, dictionary);
					buffer = memoryStream.ToArray();
				}
			}
			using (MemoryStream input = new MemoryStream(buffer))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					sceneInfo.Import(reader, sceneInfo.version);
				}
			}
			foreach (KeyValuePair<int, ObjectInfo> item in sceneInfo.dicImport)
			{
				DuplicateParentInfo value2 = null;
				if (dictionary2.TryGetValue(sceneInfo.dicChangeKey[item.Key], out value2))
				{
					AddObjectAssist.LoadChild(item.Value, value2.info, value2.node);
				}
				else
				{
					AddObjectAssist.LoadChild(item.Value);
				}
			}
			if (_routeControl.visible)
			{
				_routeControl.Init();
			}
			treeNodeCtrl.RefreshHierachy();
			_cameraSelector.Init();
		}

		public void SaveScene()
		{
			foreach (KeyValuePair<int, ObjectCtrlInfo> item in dicObjectCtrl)
			{
				item.Value.OnSavePreprocessing();
			}
			sceneInfo.cameraSaveData = m_CameraCtrl.Export();
			DateTime now = DateTime.Now;
			string text = string.Format("{0}_{1:00}{2:00}_{3:00}{4:00}_{5:00}_{6:000}.png", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
			string path = UserData.Create("studio/scene") + text;
			sceneInfo.Save(path);
		}

		public bool LoadScene(string _path)
		{
			if (!File.Exists(_path))
			{
				return false;
			}
			InitScene(false);
			sceneInfo = new SceneInfo();
			if (!sceneInfo.Load(_path))
			{
				return false;
			}
			AddObjectAssist.LoadChild(sceneInfo.dicObject);
			ChangeAmount source = sceneInfo.caMap.Clone();
			AddMap(sceneInfo.map, false, false);
			mapList.UpdateInfo();
			sceneInfo.caMap.Copy(source);
			Singleton<MapCtrl>.Instance.Reflect();
			bgmCtrl.Play(bgmCtrl.no);
			envCtrl.Play(envCtrl.no);
			outsideSoundCtrl.Play(outsideSoundCtrl.fileName);
			m_BackgroundCtrl.Load(sceneInfo.background);
			m_BackgroundList.UpdateUI();
			_frameCtrl.Load(sceneInfo.frame);
			_frameList.UpdateUI();
			m_SystemButtonCtrl.UpdateInfo();
			treeNodeCtrl.RefreshHierachy();
			if (sceneInfo.cameraSaveData != null)
			{
				m_CameraCtrl.Import(sceneInfo.cameraSaveData);
			}
			cameraLightCtrl.Reflect();
			_cameraSelector.Init();
			sceneInfo.dataVersion = sceneInfo.version;
			rootButtonCtrl.OnClick(-1);
			return true;
		}

		public IEnumerator LoadSceneCoroutine(string _path)
		{
			if (!File.Exists(_path))
			{
				yield break;
			}
			InitScene(false);
			yield return null;
			sceneInfo = new SceneInfo();
			if (sceneInfo.Load(_path))
			{
				AddObjectAssist.LoadChild(sceneInfo.dicObject);
				ChangeAmount ca = sceneInfo.caMap.Clone();
				yield return AddMapCoroutine(sceneInfo.map, false, false);
				mapList.UpdateInfo();
				sceneInfo.caMap.Copy(ca);
				Singleton<MapCtrl>.Instance.Reflect();
				bgmCtrl.Play(bgmCtrl.no);
				envCtrl.Play(envCtrl.no);
				outsideSoundCtrl.Play(outsideSoundCtrl.fileName);
				m_BackgroundCtrl.Load(sceneInfo.background);
				m_BackgroundList.UpdateUI();
				_frameCtrl.Load(sceneInfo.frame);
				_frameList.UpdateUI();
				m_SystemButtonCtrl.UpdateInfo();
				treeNodeCtrl.RefreshHierachy();
				if (sceneInfo.cameraSaveData != null)
				{
					m_CameraCtrl.Import(sceneInfo.cameraSaveData);
				}
				cameraLightCtrl.Reflect();
				_cameraSelector.Init();
				rootButtonCtrl.OnClick(-1);
			}
		}

		public bool ImportScene(string _path)
		{
			if (!File.Exists(_path))
			{
				return false;
			}
			if (!sceneInfo.Import(_path))
			{
				return false;
			}
			AddObjectAssist.LoadChild(sceneInfo.dicImport);
			treeNodeCtrl.RefreshHierachy();
			_cameraSelector.Init();
			return true;
		}

		public void InitScene(bool _close = true)
		{
			ChangeCamera(null, false, true);
			cameraCount = 0;
			treeNodeCtrl.DeleteAllNode();
			Singleton<GuideObjectManager>.Instance.DeleteAll();
			m_RootButtonCtrl.OnClick(-1);
			m_RootButtonCtrl.objectCtrlInfo = null;
			foreach (KeyValuePair<TreeNodeObject, ObjectCtrlInfo> item in dicInfo)
			{
				switch (item.Value.kind)
				{
				case 0:
				{
					OCIChar oCIChar = item.Value as OCIChar;
					if (oCIChar != null)
					{
						oCIChar.StopVoice();
					}
					break;
				}
				case 4:
				{
					OCIRoute oCIRoute = item.Value as OCIRoute;
					oCIRoute.DeleteLine();
					break;
				}
				}
				UnityEngine.Object.Destroy(item.Value.guideObject.transformTarget.gameObject);
			}
			Singleton<Character>.Instance.DeleteCharaAll();
			dicInfo.Clear();
			dicChangeAmount.Clear();
			dicObjectCtrl.Clear();
			Singleton<Map>.Instance.ReleaseMap();
			cameraCtrl.CloerListCollider();
			bgmCtrl.Stop();
			envCtrl.Stop();
			outsideSoundCtrl.Stop();
			sceneInfo.Init();
			m_SystemButtonCtrl.UpdateInfo();
			cameraCtrl.Reset(0);
			cameraLightCtrl.Reflect();
			_cameraSelector.Init();
			mapList.UpdateInfo();
			if (onChangeMap != null)
			{
				onChangeMap();
			}
			m_BackgroundCtrl.Load(sceneInfo.background);
			m_BackgroundList.UpdateUI();
			_frameCtrl.Load(sceneInfo.frame);
			_frameList.UpdateUI();
			m_WorkspaceCtrl.UpdateUI();
			Singleton<UndoRedoManager>.Instance.Clear();
			if (_close)
			{
				rootButtonCtrl.OnClick(-1);
			}
		}

		public void OnDeleteNode(TreeNodeObject _node)
		{
			ObjectCtrlInfo value = null;
			if (dicInfo.TryGetValue(_node, out value))
			{
				if (onDelete != null)
				{
					onDelete(value);
				}
				value.OnDelete();
				dicInfo.Remove(_node);
			}
		}

		public void OnParentage(TreeNodeObject _parent, TreeNodeObject _child)
		{
			if ((bool)_parent)
			{
				ObjectCtrlInfo objectCtrlInfo = FindLoop(_parent);
				if (objectCtrlInfo != null)
				{
					objectCtrlInfo.OnAttach(_parent, dicInfo[_child]);
				}
			}
			else
			{
				dicInfo[_child].OnDetach();
			}
		}

		public void ResetOption()
		{
			if (xmlCtrl != null)
			{
				xmlCtrl.Init();
			}
		}

		public void LoadOption()
		{
			if (xmlCtrl != null)
			{
				xmlCtrl.Read();
			}
		}

		public void SaveOption()
		{
			if (xmlCtrl != null)
			{
				xmlCtrl.Write();
			}
		}

		public static void AddInfo(ObjectInfo _info, ObjectCtrlInfo _ctrlInfo)
		{
			if (Singleton<Studio>.IsInstance() && _info != null && _ctrlInfo != null)
			{
				Singleton<Studio>.Instance.sceneInfo.dicObject.Add(_info.dicKey, _info);
				Singleton<Studio>.Instance.dicObjectCtrl[_info.dicKey] = _ctrlInfo;
			}
		}

		public static void DeleteInfo(ObjectInfo _info, bool _delKey = true)
		{
			if (!Singleton<Studio>.IsInstance() || _info == null)
			{
				return;
			}
			if (Singleton<Studio>.Instance.sceneInfo.dicObject.ContainsKey(_info.dicKey))
			{
				Singleton<Studio>.Instance.sceneInfo.dicObject.Remove(_info.dicKey);
			}
			if (_delKey)
			{
				Singleton<Studio>.Instance.dicObjectCtrl.Remove(_info.dicKey);
				_info.DeleteKey();
				if (Singleton<Studio>.Instance.sceneInfo.sunCaster == _info.dicKey)
				{
					Singleton<Studio>.Instance.SetSunCaster(-1);
				}
			}
		}

		public static ObjectInfo GetInfo(int _key)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return null;
			}
			ObjectInfo value = null;
			return (!Singleton<Studio>.Instance.sceneInfo.dicObject.TryGetValue(_key, out value)) ? null : value;
		}

		public static void AddObjectCtrlInfo(ObjectCtrlInfo _ctrlInfo)
		{
			if (Singleton<Studio>.IsInstance() && _ctrlInfo != null)
			{
				Singleton<Studio>.Instance.dicObjectCtrl[_ctrlInfo.objectInfo.dicKey] = _ctrlInfo;
			}
		}

		public static ObjectCtrlInfo GetCtrlInfo(int _key)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return null;
			}
			ObjectCtrlInfo value = null;
			return (!Singleton<Studio>.Instance.dicObjectCtrl.TryGetValue(_key, out value)) ? null : value;
		}

		public static TreeNodeObject AddNode(string _name, TreeNodeObject _parent = null)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return null;
			}
			return Singleton<Studio>.Instance.treeNodeCtrl.AddNode(_name, _parent);
		}

		public static void DeleteNode(TreeNodeObject _node)
		{
			if (Singleton<Studio>.IsInstance())
			{
				Singleton<Studio>.Instance.treeNodeCtrl.DeleteNode(_node);
			}
		}

		public static void AddCtrlInfo(ObjectCtrlInfo _info)
		{
			if (Singleton<Studio>.IsInstance() && _info != null)
			{
				Singleton<Studio>.Instance.dicInfo.Add(_info.treeNodeObject, _info);
			}
		}

		public static ObjectCtrlInfo GetCtrlInfo(TreeNodeObject _node)
		{
			if (!Singleton<Studio>.IsInstance() || _node == null)
			{
				return null;
			}
			ObjectCtrlInfo value = null;
			return (!Singleton<Studio>.Instance.dicInfo.TryGetValue(_node, out value)) ? null : value;
		}

		public static int GetNewIndex()
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return -1;
			}
			return Singleton<Studio>.Instance.sceneInfo.GetNewIndex();
		}

		public static int SetNewIndex(int _index)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return _index;
			}
			return (!Singleton<Studio>.Instance.sceneInfo.SetNewIndex(_index)) ? Singleton<Studio>.Instance.sceneInfo.GetNewIndex() : _index;
		}

		public static bool DeleteIndex(int _index)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return false;
			}
			bool flag = Singleton<Studio>.Instance.sceneInfo.DeleteIndex(_index);
			return DeleteChangeAmount(_index) || flag;
		}

		public static bool IsLightCheck()
		{
			return Singleton<Studio>.IsInstance() && Singleton<Studio>.Instance.sceneInfo.isLightCheck;
		}

		public static void AddLight()
		{
			if (Singleton<Studio>.IsInstance())
			{
				Singleton<Studio>.Instance.sceneInfo.AddLight();
			}
		}

		public static void DeleteLight()
		{
			if (Singleton<Studio>.IsInstance())
			{
				Singleton<Studio>.Instance.sceneInfo.DeleteLight();
			}
		}

		public static void AddChangeAmount(int _key, ChangeAmount _ca)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return;
			}
			try
			{
				Singleton<Studio>.Instance.dicChangeAmount.Add(_key, _ca);
			}
			catch (Exception)
			{
			}
		}

		public static bool DeleteChangeAmount(int _key)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return false;
			}
			return Singleton<Studio>.Instance.dicChangeAmount.Remove(_key);
		}

		public static ChangeAmount GetChangeAmount(int _key)
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return null;
			}
			ChangeAmount value = null;
			return (!Singleton<Studio>.Instance.dicChangeAmount.TryGetValue(_key, out value)) ? null : value;
		}

		public static ObjectCtrlInfo[] GetSelectObjectCtrl()
		{
			if (!Singleton<Studio>.IsInstance())
			{
				return null;
			}
			return Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl;
		}

		public void Init()
		{
			sceneInfo = new SceneInfo();
			cameraLightCtrl.Init();
			systemButtonCtrl.Init();
			mapList.Init();
			logoList.Init();
			_inputFieldNow = null;
			_inputFieldTMPNow = null;
			TreeNodeCtrl obj = treeNodeCtrl;
			obj.onDelete = (Action<TreeNodeObject>)Delegate.Combine(obj.onDelete, new Action<TreeNodeObject>(OnDeleteNode));
			TreeNodeCtrl obj2 = treeNodeCtrl;
			obj2.onParentage = (Action<TreeNodeObject, TreeNodeObject>)Delegate.Combine(obj2.onParentage, new Action<TreeNodeObject, TreeNodeObject>(OnParentage));
		}

		public void SelectInputField(InputField _input, TMP_InputField _inputTMP)
		{
			_inputFieldNow = _input;
			_inputFieldTMPNow = _inputTMP;
		}

		public void DeselectInputField(InputField _input, TMP_InputField _inputTMP)
		{
			if (_inputFieldNow == _input)
			{
				_inputFieldNow = null;
			}
			if (_inputFieldTMPNow == _inputTMP)
			{
				_inputFieldTMPNow = null;
			}
		}

		public void ShowName(Transform _transform, string _name)
		{
			rectName.gameObject.SetActive(true);
			rectName.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _transform.position);
			textName.text = _name;
			if (disposableName != null)
			{
				disposableName.Dispose();
			}
			disposableName = new SingleAssignmentDisposable();
			IObservable<long> other = Observable.Timer(TimeSpan.FromSeconds(2.0));
			disposableName.Disposable = Observable.EveryUpdate().TakeUntil(other).Subscribe((Action<long>)delegate
			{
				if (_transform != null)
				{
					rectName.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _transform.position);
				}
			}, (Action)delegate
			{
				rectName.gameObject.SetActive(false);
			});
		}

		private ObjectCtrlInfo FindLoop(TreeNodeObject _node)
		{
			if (_node == null)
			{
				return null;
			}
			ObjectCtrlInfo value = null;
			if (dicInfo.TryGetValue(_node, out value))
			{
				return value;
			}
			return FindLoop(_node.parent);
		}

		private void SavePreprocessingLoop(TreeNodeObject _node)
		{
			if (_node == null)
			{
				return;
			}
			ObjectCtrlInfo value = null;
			if (dicInfo.TryGetValue(_node, out value))
			{
				value.OnSavePreprocessing();
			}
			if (_node.child.IsNullOrEmpty())
			{
				return;
			}
			foreach (TreeNodeObject item in _node.child)
			{
				SavePreprocessingLoop(item);
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				imageCamera.gameObject.SetActive(false);
				optionSystem = new OptionSystem("Option");
				xmlCtrl = new Control("studio", "option.xml", "Option", optionSystem);
				LoadOption();
				_logoList.UpdateInfo();
				if (workInfo == null)
				{
					workInfo = new WorkInfo();
				}
				workInfo.Load();
			}
		}

		private void OnApplicationQuit()
		{
			SaveOption();
			workInfo.Save();
		}
	}
}
