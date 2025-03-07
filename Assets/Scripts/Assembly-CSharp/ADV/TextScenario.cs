using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ADV.Commands.Base;
using Config;
using Illusion.Component.UI;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game.Elements;
using Localize.Translate;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ADV
{
	[RequireComponent(typeof(TextController))]
	[RequireComponent(typeof(CommandController))]
	public class TextScenario : MonoBehaviour
	{
		public interface IVoice
		{
			int personality { get; }

			string bundle { get; }

			string asset { get; }

			void Convert2D();

			AudioSource Play();

			bool Wait();
		}

		public interface IChara
		{
			int no { get; }

			void Play(TextScenario scenario);

			CharaData GetChara(TextScenario scenario);
		}

		public interface IMotion : IChara
		{
		}

		public interface IExpression : IChara
		{
		}

		public interface IExpressionIcon : IChara
		{
		}

		public class CurrentCharaData
		{
			private List<IVoice[]> _karaokePlayer;

			private List<IVoice[]> _voiceList;

			private List<IMotion[]> _motionList;

			private List<IExpression[]> _expressionList;

			private List<IExpressionIcon[]> _expressionIconList;

			private Dictionary<int, string> _bundleVoices;

			public Dictionary<int, string> bundleVoices
			{
				get
				{
					return _bundleVoices;
				}
			}

			public bool isSkip { get; set; }

			public IVoice karaokePlayer
			{
				set
				{
					_karaokePlayer = new List<IVoice[]> { new IVoice[1] { value } };
				}
			}

			public bool isKaraoke
			{
				get
				{
					return _karaokePlayer != null;
				}
			}

			public List<IVoice[]> voiceList
			{
				get
				{
					return _karaokePlayer ?? _voiceList;
				}
			}

			public List<IMotion[]> motionList
			{
				get
				{
					return _motionList;
				}
			}

			public List<IExpression[]> expressionList
			{
				get
				{
					return _expressionList;
				}
			}

			public List<IExpressionIcon[]> expressionIconList
			{
				get
				{
					return _expressionIconList;
				}
			}

			public CurrentCharaData()
			{
				_bundleVoices = new Dictionary<int, string>();
			}

			public void CreateVoiceList()
			{
				if (voiceList == null)
				{
					_voiceList = new List<IVoice[]>();
				}
			}

			public void CreateMotionList()
			{
				if (motionList == null)
				{
					_motionList = new List<IMotion[]>();
				}
			}

			public void CreateExpressionList()
			{
				if (expressionList == null)
				{
					_expressionList = new List<IExpression[]>();
				}
			}

			public void CreateExpressionIconList()
			{
				if (expressionIconList == null)
				{
					_expressionIconList = new List<IExpressionIcon[]>();
				}
			}

			public void Clear()
			{
				_karaokePlayer = null;
				_voiceList = null;
				_motionList = null;
				_expressionList = null;
				_expressionIconList = null;
			}
		}

		public class LoopVoicePack
		{
			public int voiceNo { get; private set; }

			public ChaControl chaCtrl { get; private set; }

			public AudioSource audio { get; private set; }

			public LoopVoicePack(int voiceNo, ChaControl chaCtrl, AudioSource audio)
			{
				this.voiceNo = voiceNo;
				this.chaCtrl = chaCtrl;
				this.audio = audio;
			}

			public bool Set()
			{
				if (chaCtrl == null || audio == null)
				{
					return false;
				}
				chaCtrl.SetVoiceTransform(audio.transform);
				return true;
			}
		}

		protected class NextInfo
		{
			public bool isCompleteDisplayText;

			public bool isNext;

			public bool isSkip;

			public NextInfo(bool isCompleteDisplayText, bool isNext, bool isSkip)
			{
				this.isCompleteDisplayText = isCompleteDisplayText;
				this.isNext = isNext;
				this.isSkip = isSkip;
			}
		}

		[Serializable]
		private sealed class FileOpen
		{
			[SerializeField]
			private List<RootData> fileList = new List<RootData>();

			[SerializeField]
			private List<RootData> rootList = new List<RootData>();

			public List<RootData> FileList
			{
				get
				{
					return fileList;
				}
			}

			public List<RootData> RootList
			{
				get
				{
					return rootList;
				}
			}

			public void Clear()
			{
				fileList.Clear();
				rootList.Clear();
			}
		}

		public Regulate regulate;

		[SerializeField]
		private Info _info = new Info();

		[SerializeField]
		private Camera _AdvCamera;

		private CrossFade _crossFade;

		public CurrentCharaData currentCharaData = new CurrentCharaData();

		private List<Program.Transfer> _transferList;

		private CommandController _commandController;

		private TextController _textController;

		private ADVScene _advScene;

		private BaseCameraControl _baseCamCtrl;

		[SerializeField]
		protected RectTransform messageWindow;

		[SerializeField]
		protected Image nextMarker;

		[SerializeField]
		protected Canvas msgWindowCanvas;

		[SerializeField]
		private ADVFade advFade;

		[SerializeField]
		private BackGroundParam bgParam;

		[SerializeField]
		private Camera backCamera;

		[SerializeField]
		private Transform choices;

		[SerializeField]
		private Transform characters;

		[SerializeField]
		private Transform faceIcons;

		[SerializeField]
		private Image filterImage;

		[SerializeField]
		protected bool _isWindowImage = true;

		[SerializeField]
		protected bool _isSkip;

		[SerializeField]
		protected bool _isAuto;

		[SerializeField]
		protected bool _isAspect;

		[SerializeField]
		protected bool _isWait;

		[SerializeField]
		private string _loadBundleName = string.Empty;

		[SerializeField]
		private string _loadAssetName = string.Empty;

		[NotEditable]
		[SerializeField]
		[Header("Debug表示")]
		protected int currentLine;

		[NotEditable]
		[SerializeField]
		protected float autoWaitTimer;

		[NotEditable]
		[SerializeField]
		protected float autoWaitTime = 3f;

		[NotEditable]
		[SerializeField]
		protected bool _isText;

		[NotEditable]
		[SerializeField]
		protected bool _isChoice;

		[SerializeField]
		[NotEditable]
		private bool _isSceneFadeRegulate = true;

		[SerializeField]
		[NotEditable]
		private bool _isStartRun;

		[SerializeField]
		[NotEditable]
		private bool _isCameraLock;

		protected List<ScenarioData.Param> commandPacks;

		protected Image windowImage;

		private FileOpen fileOpenData;

		private Dictionary<string, ADVFaceIconData.Param> faceIconParamDic;

		protected HashSet<int> textHash = new HashSet<int>();

		private Dictionary<string, ValData> vars = new Dictionary<string, ValData>();

		private Dictionary<string, string> replaces = new Dictionary<string, string>();

		private Illusion.Game.Elements.Single _single = new Illusion.Game.Elements.Single();

		private List<IDisposable> _karaokeList = new List<IDisposable>();

		private IDisposable voicePlayDis;

		private List<LoopVoicePack> _loopVoiceList = new List<LoopVoicePack>();

		public const int VOICE_SET_NO = 1;

		public Info info
		{
			get
			{
				return _info;
			}
		}

		public Camera AdvCamera
		{
			get
			{
				return _AdvCamera;
			}
			set
			{
				if (value == null)
				{
					UnityEngine.Object.Destroy(_baseCamCtrl);
					_crossFade = null;
				}
				else
				{
					_baseCamCtrl = value.GetOrAddComponent<BaseCameraControl>();
					_baseCamCtrl.enabled = false;
					_crossFade = value.GetComponent<CrossFade>();
				}
				_AdvCamera = value;
			}
		}

		public CrossFade crossFade
		{
			get
			{
				return _crossFade;
			}
		}

		public List<Program.Transfer> transferList
		{
			get
			{
				return _transferList;
			}
			set
			{
				_transferList = value;
			}
		}

		public string fontColorKey { get; set; }

		public bool isWindowImage
		{
			get
			{
				return _isWindowImage;
			}
			set
			{
				_isWindowImage = value;
			}
		}

		public bool isSkip
		{
			get
			{
				return _isSkip;
			}
			set
			{
				_isSkip = value;
			}
		}

		public bool isAuto
		{
			get
			{
				return _isAuto;
			}
			set
			{
				_isAuto = value;
			}
		}

		public bool isAspect
		{
			get
			{
				return _isAspect;
			}
			set
			{
				_isAspect = value;
				if (AdvCamera != null)
				{
					AdvCamera.rect = ((!_isAspect) ? new Rect(0f, 0f, 1f, 1f) : MathfEx.AspectRect());
				}
			}
		}

		public bool isWait
		{
			get
			{
				return _isWait;
			}
			set
			{
				_isWait = value;
			}
		}

		public int CurrentLine
		{
			get
			{
				return currentLine - 1;
			}
			set
			{
				currentLine = value;
			}
		}

		public string LoadBundleName
		{
			get
			{
				return _loadBundleName;
			}
			set
			{
				_loadBundleName = value;
			}
		}

		public string LoadAssetName
		{
			get
			{
				return _loadAssetName;
			}
			set
			{
				_loadAssetName = value;
			}
		}

		public bool isSceneFadeRegulate
		{
			get
			{
				return _isSceneFadeRegulate;
			}
			set
			{
				_isSceneFadeRegulate = value;
			}
		}

		public bool isChoice
		{
			get
			{
				return _isChoice;
			}
			set
			{
				_isChoice = value;
			}
		}

		public bool isCameraLock
		{
			get
			{
				return _isCameraLock;
			}
			set
			{
				_isCameraLock = value;
				BaseCameraControl baseCamCtrl = BaseCamCtrl;
				if (baseCamCtrl != null)
				{
					baseCamCtrl.enabled = !_isCameraLock;
					baseCamCtrl.isInit = true;
					if (!_isCameraLock)
					{
						baseCamCtrl.SetCamera(baseCamCtrl.transform.position, baseCamCtrl.transform.eulerAngles, baseCamCtrl.transform.rotation, Vector3.zero - new Vector3(0f, 0f, baseCamCtrl.transform.position.z));
					}
					baseCamCtrl.ZoomCondition = () => false;
					baseCamCtrl.KeyCondition = () => !_isCameraLock;
					baseCamCtrl.NoCtrlCondition = () => _isCameraLock;
				}
			}
		}

		public bool isBackGroundCommanding { get; set; }

		public CommandController commandController
		{
			get
			{
				return this.GetComponentCache(ref _commandController);
			}
		}

		public TextController textController
		{
			get
			{
				return this.GetComponentCache(ref _textController);
			}
		}

		public ADVScene advScene
		{
			get
			{
				return this.GetComponentCache(ref _advScene);
			}
		}

		public List<ScenarioData.Param> CommandPacks
		{
			get
			{
				return commandPacks;
			}
		}

		public bool isBackGroundCommandProcessing
		{
			get
			{
				return backCommandList.Count > 0;
			}
		}

		public virtual RectTransform MessageWindow
		{
			get
			{
				return messageWindow;
			}
		}

		public BackGroundParam BGParam
		{
			get
			{
				return bgParam;
			}
		}

		public Camera BackCamera
		{
			get
			{
				return backCamera;
			}
		}

		public Transform Choices
		{
			get
			{
				return choices;
			}
		}

		public Transform Characters
		{
			get
			{
				return characters;
			}
		}

		public Transform FaceIcons
		{
			get
			{
				return faceIcons;
			}
		}

		public Dictionary<string, ADVFaceIconData.Param> FaceIconParamDic
		{
			get
			{
				return faceIconParamDic;
			}
		}

		public Image FilterImage
		{
			get
			{
				return filterImage;
			}
		}

		public bool isSelectMessageWindow
		{
			get
			{
				return messageWindowSelectUI.isSelect;
			}
		}

		public BaseCameraControl BaseCamCtrl
		{
			get
			{
				return this.GetCacheObject(ref _baseCamCtrl, () => (!(_AdvCamera == null)) ? _AdvCamera.GetComponent<BaseCameraControl>() : null);
			}
		}

		public bool isFadeAllEnd
		{
			get
			{
				if (!Singleton<Manager.Scene>.IsInstance() || advScene == null || advScene.AdvFade == null || _crossFade == null)
				{
					return true;
				}
				return !Singleton<Manager.Scene>.Instance.IsFadeNow && advScene.AdvFade.IsEnd && _crossFade.isEnd;
			}
		}

		protected bool isRequestLine { get; set; }

		private SelectUI messageWindowSelectUI { get; set; }

		public Dictionary<string, ValData> Vars
		{
			get
			{
				return vars;
			}
		}

		public Dictionary<string, string> Replaces
		{
			get
			{
				return replaces;
			}
		}

		public SaveData.Player player
		{
			get
			{
				return Singleton<Game>.Instance.Player;
			}
		}

		public List<SaveData.Heroine> heroineList { get; set; }

		public CharaData currentChara
		{
			get
			{
				return _currentChara;
			}
			set
			{
				_currentChara = value;
			}
		}

		private CharaData _currentChara { get; set; }

		public SaveData.Heroine currentHeroine
		{
			get
			{
				if (currentChara == null)
				{
					return null;
				}
				return currentChara.heroine;
			}
		}

		private BackupPosRot backCameraBackup { get; set; }

		private CommandList nowCommandList
		{
			get
			{
				return _commandController.NowCommandList;
			}
		}

		private CommandList backCommandList
		{
			get
			{
				return _commandController.BackGroundCommandList;
			}
		}

		public IObservable<Unit> OnInitializedAsync
		{
			get
			{
				return _single;
			}
		}

		public List<IDisposable> karaokeList
		{
			get
			{
				return _karaokeList;
			}
		}

		public List<LoopVoicePack> loopVoiceList
		{
			get
			{
				return _loopVoiceList;
			}
		}

		public void CrossFadeStart()
		{
			if (!(_crossFade == null) && isFadeAllEnd)
			{
				_crossFade.FadeStart(info.anime.play.crossFadeTime);
			}
		}

		public bool ChangeCurrentChara(int no)
		{
			currentChara = commandController.GetChara(no);
			return currentChara != null;
		}

		public string ReplaceVars(string arg)
		{
			ValData value;
			return Vars.TryGetValue(arg, out value) ? value.o.ToString() : arg;
		}

		public string ReplaceText(string text, bool useOneceTagCovert)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			try
			{
				foreach (Match item in Regex.Matches(text, "\\[.*?\\]"))
				{
					if (!item.Success)
					{
						continue;
					}
					string key = string.Empty;
					try
					{
						key = Regex.Replace(item.Value, "\\[|\\]", string.Empty);
					}
					catch (Exception)
					{
					}
					string value;
					if (Replaces.TryGetValue(key, out value))
					{
						if (!value.IsNullOrEmpty() && !NameCheck(key))
						{
							value = ReplaceText(value, useOneceTagCovert);
						}
						if (useOneceTagCovert)
						{
							OneceTagCovert(ref key, ref value);
						}
						if (value == null)
						{
							value = item.Value;
						}
					}
					else
					{
						value = item.Value;
					}
					stringBuilder.Append(text.Substring(num, item.Index - num));
					stringBuilder.Append(value);
					num = item.Index + item.Length;
				}
			}
			catch (Exception)
			{
			}
			stringBuilder.Append(text.Substring(num, text.Length - num));
			return stringBuilder.ToString();
		}

		public virtual void Initialize()
		{
			commandPacks = new List<ScenarioData.Param>();
			fileOpenData = new FileOpen();
			regulate = new Regulate(this);
			windowImage = windowImage ?? MessageWindow.GetComponent<Image>();
			messageWindowSelectUI = MessageWindow.GetOrAddComponent<SelectUI>();
			if (backCameraBackup != null && backCamera != null)
			{
				backCameraBackup.Set(backCamera.transform);
			}
			currentLine = 0;
			autoWaitTimer = 0f;
			commandController.Initialize();
			textController.Clear();
			vars.Clear();
			vars["Language"] = new ValData(Localize.Translate.Manager.Language);
			replaces.Clear();
			if (msgWindowCanvas != null)
			{
				msgWindowCanvas.enabled = false;
			}
			if (windowImage != null)
			{
				windowImage.enabled = false;
			}
			if (filterImage != null)
			{
				filterImage.enabled = false;
				filterImage.sprite = null;
			}
			ChoicesInit();
			advFade.SafeProc(delegate(ADVFade p)
			{
				p.Initialize();
			});
			_single.Done();
		}

		protected void MemberInit()
		{
			_isWindowImage = true;
			_isSkip = false;
			_isAuto = false;
			_isWait = false;
			_isChoice = false;
			_isSceneFadeRegulate = true;
			_isStartRun = false;
			_isCameraLock = false;
			textHash.Clear();
		}

		public virtual void Release()
		{
			if (voicePlayDis != null)
			{
				voicePlayDis.Dispose();
			}
			voicePlayDis = null;
			loopVoiceList.ForEach(delegate(LoopVoicePack p)
			{
				if (p.audio != null)
				{
					UnityEngine.Object.Destroy(p.audio.gameObject);
				}
			});
			loopVoiceList.Clear();
			karaokeList.ForEach(delegate(IDisposable p)
			{
				p.Dispose();
			});
			karaokeList.Clear();
			_single = new Illusion.Game.Elements.Single();
			commandController.Release();
			AdvCamera = null;
			info.audio.is2D = false;
			info.audio.isNotMoveMouth = false;
		}

		public void CommandAdd(bool isNext, int line, bool multi, Command command, params string[] args)
		{
			List<string> list = new List<string>();
			list.Add("0");
			list.Add(multi.ToString());
			list.Add(command.ToString());
			list.AddRange(args ?? new string[1] { string.Empty });
			commandPacks.Insert(line, new ScenarioData.Param(list.ToArray()));
			if (isNext)
			{
				RequestNextLine();
			}
		}

		public virtual bool LoadFile(string bundle, string asset, bool isClear = true, bool isClearCheck = true, bool isNext = true)
		{
			if (isClear)
			{
				textController.Clear();
				fileOpenData.Clear();
			}
			if (bundle.IsNullOrEmpty())
			{
				bundle = LoadBundleName;
			}
			if (!isClear && isClearCheck && fileOpenData.FileList.Any((RootData p) => p.bundleName == bundle && p.fileName == asset))
			{
				return false;
			}
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(bundle, asset, typeof(ScenarioData));
			ScenarioData asset2 = assetBundleLoadAssetOperation.GetAsset<ScenarioData>();
			if (!isClear)
			{
				commandPacks.InsertRange(currentLine, asset2.list);
				if (!fileOpenData.FileList.Any((RootData p) => p.bundleName == bundle && p.fileName == asset && p.line == currentLine))
				{
					fileOpenData.FileList.Add(new RootData
					{
						bundleName = bundle,
						fileName = asset,
						line = CurrentLine
					});
				}
			}
			else
			{
				LoadBundleName = bundle;
				LoadAssetName = asset;
				string[] args = new string[5]
				{
					LoadBundleName,
					LoadAssetName,
					bool.FalseString,
					bool.TrueString,
					bool.TrueString
				};
				currentLine = 0;
				commandPacks.Clear();
				CommandAdd(false, currentLine++, false, Command.Open, args);
				commandPacks.AddRange(asset2.list);
			}
			if (isNext)
			{
				RequestNextLine();
			}
			AssetBundleManager.UnloadAssetBundle(bundle, true);
			Resources.UnloadUnusedAssets();
			return true;
		}

		public bool SearchTagJumpOrOpenFile(string jump, int localLine)
		{
			string[] array = jump.Split(':');
			if (array.Length == 1)
			{
				int n;
				if (SearchTag(jump, out n))
				{
					Jump(n);
					return true;
				}
				return false;
			}
			Open open = new Open();
			open.Set(Command.Open);
			string[] argsDefault = open.ArgsDefault;
			for (int i = 0; i < array.Length && i < argsDefault.Length; i++)
			{
				argsDefault[i] = ReplaceVars(array[i]);
			}
			CommandAdd(false, localLine + 1, false, open.command, argsDefault);
			return true;
		}

		public bool SearchTag(string tagName, out int n)
		{
			n = commandPacks.TakeWhile(delegate(ScenarioData.Param p)
			{
				if (p.Command != Command.Tag)
				{
					return true;
				}
				return (ReplaceVars(p.Args[0]) != tagName) ? true : false;
			}).Count();
			return n < commandPacks.Count;
		}

		public void Jump(int n)
		{
			currentLine = n;
			RequestNextLine();
		}

		public void ChangeWindow(UnityEngine.UI.Text nameWindow, UnityEngine.UI.Text messageWindow)
		{
			textController.Change(nameWindow, messageWindow);
			UnityEngine.UI.Text text = MessageWindow.GetComponentsInChildren<UnityEngine.UI.Text>(true).FirstOrDefault((UnityEngine.UI.Text p) => p.name == "Message");
			MessageWindow.gameObject.SetActive(text != null && text == messageWindow);
		}

		public virtual void ConfigProc()
		{
			if (Manager.Config.initialized)
			{
				TextSystem textData = Manager.Config.TextData;
				if (windowImage != null)
				{
					windowImage.color = Manager.Config.TextData.WindowColor;
				}
				textController.FontSpeed = textData.FontSpeed;
				autoWaitTime = textData.AutoWaitTime;
			}
		}

		public virtual void ChoicesInit()
		{
			_isChoice = false;
			if (Choices != null)
			{
				Choices.gameObject.SetActive(false);
			}
			nextMarker.SafeProc(delegate(Image p)
			{
				p.enabled = false;
			});
		}

		public void BackGroundCommandProcessEnd()
		{
			backCommandList.ProcessEnd();
		}

		public void VoicePlay(List<IVoice[]> voices, Action onChange, Action onEnd)
		{
			if (voicePlayDis != null)
			{
				voicePlayDis.Dispose();
			}
			voicePlayDis = null;
			Singleton<Manager.Voice>.Instance.StopAll(false);
			if (voices == null)
			{
				return;
			}
			if (_loopVoiceList.Any())
			{
				HashSet<int> hashSet = new HashSet<int>();
				foreach (IVoice[] voice2 in voices)
				{
					IVoice[] array = voice2;
					foreach (IVoice voice in array)
					{
						hashSet.Add(voice.personality);
					}
				}
				foreach (int item in hashSet)
				{
					foreach (LoopVoicePack loopVoice in _loopVoiceList)
					{
						if (loopVoice.voiceNo == item && loopVoice.audio != null)
						{
							loopVoice.audio.Pause();
						}
					}
				}
			}
			voicePlayDis = Observable.FromCoroutine((IObserver<Unit> observer) => VoicePlayCoroutine(observer, voices)).Subscribe((Action<Unit>)delegate
			{
				onChange.Call();
			}, (Action)delegate
			{
				List<LoopVoicePack> list = new List<LoopVoicePack>();
				foreach (LoopVoicePack loopVoice2 in _loopVoiceList)
				{
					if (!loopVoice2.Set() || loopVoice2.audio == null)
					{
						list.Add(loopVoice2);
					}
					else
					{
						loopVoice2.audio.Play();
					}
				}
				list.ForEach(delegate(LoopVoicePack item)
				{
					_loopVoiceList.Remove(item);
				});
				onEnd.Call();
			});
		}

		private IEnumerator VoicePlayCoroutine(IObserver<Unit> observer, List<IVoice[]> voiceList)
		{
			foreach (IVoice[] voice in voiceList)
			{
				IVoice[] array = voice;
				foreach (IVoice voice2 in array)
				{
					voice2.Play();
				}
				observer.OnNext(Unit.Default);
				while (voice.Any((IVoice p) => p.Wait()))
				{
					yield return null;
				}
			}
			observer.OnCompleted();
		}

		protected virtual bool StartRun()
		{
			if (_isStartRun)
			{
				return false;
			}
			ADVCameraSetting();
			isAspect = isAspect;
			_isStartRun = true;
			int num = 0;
			Program.Transfer transfer = null;
			if (!_transferList.IsNullOrEmpty())
			{
				Program.Transfer transfer2 = _transferList[0];
				if (transfer2.param.Command == Command.SceneFadeRegulate)
				{
					_isSceneFadeRegulate = bool.Parse(transfer2.param.Args[0]);
				}
				transfer = _transferList.Find((Program.Transfer p) => p.param.Command == Command.Open);
				if (transfer != null)
				{
					LoadBundleName = transfer.param.Args[0];
					LoadAssetName = transfer.param.Args[1];
				}
			}
			if (!_transferList.IsNullOrEmpty())
			{
				foreach (Program.Transfer transfer3 in _transferList)
				{
					int line = ((transfer3.line != -1) ? transfer3.line : num);
					CommandAdd(false, line, transfer3.param.Multi, transfer3.param.Command, transfer3.param.Args);
					num++;
				}
			}
			if (transfer == null && !LoadBundleName.IsNullOrEmpty() && !LoadAssetName.IsNullOrEmpty())
			{
				string[] args = new string[5]
				{
					LoadBundleName,
					LoadAssetName,
					bool.FalseString,
					bool.TrueString,
					bool.TrueString
				};
				CommandAdd(true, num, false, Command.Open, args);
			}
			return true;
		}

		protected void RequestNextLine()
		{
			StartCoroutine(_RequestNextLine());
		}

		protected virtual IEnumerator _RequestNextLine()
		{
			isRequestLine = true;
			nowCommandList.ProcessEnd();
			bool isMulti = false;
			ScenarioData.Param pack = null;
			textHash.Clear();
			autoWaitTimer = 0f;
			while (true)
			{
				if (commandController.LoadingCharaList.Any())
				{
					yield return null;
					continue;
				}
				isMulti = false;
				if (currentLine < commandPacks.Count)
				{
					pack = commandPacks[currentLine++];
					if (pack.Command == Command.Log)
					{
						isMulti = true;
					}
					else
					{
						isMulti = ((isBackGroundCommanding && pack.Command != Command.Task) ? backCommandList.Add(pack, CurrentLine) : nowCommandList.Add(pack, CurrentLine));
						Command command = pack.Command;
						if (command == Command.Text)
						{
							textHash.Add(pack.Hash);
						}
					}
				}
				if (!isMulti)
				{
					break;
				}
			}
			isRequestLine = false;
		}

		protected void ADVCameraSetting()
		{
			Camera advCamera = AdvCamera;
			if (advCamera != null)
			{
				Transform transform = advCamera.transform;
				Transform transform2 = BackCamera.transform;
				transform.SetParent(transform2.parent, false);
				transform.SetPositionAndRotation(transform2.position, transform2.rotation);
				transform.localScale = Vector3.one;
				advCamera.fieldOfView = BackCamera.fieldOfView;
				advCamera.farClipPlane = 10000f;
			}
			if (bgParam != null)
			{
				bgParam.visible = false;
			}
			isCameraLock = true;
		}

		protected bool MessageWindowProc(NextInfo nextInfo)
		{
			if (isRequestLine)
			{
				return false;
			}
			backCommandList.Process();
			if (nowCommandList.Process())
			{
				return false;
			}
			if (commandPacks.Count == 0)
			{
				return false;
			}
			if (regulate.control.HasFlag(Regulate.Control.ClickNext))
			{
				nextInfo.isNext = false;
			}
			if (regulate.control.HasFlag(Regulate.Control.Skip))
			{
				nextInfo.isSkip = false;
				isSkip = false;
			}
			if (regulate.control.HasFlag(Regulate.Control.Auto))
			{
				isAuto = false;
			}
			if (regulate.control.HasFlag(Regulate.Control.AutoForce))
			{
				isAuto = true;
			}
			bool isCompleteDisplayText = nextInfo.isCompleteDisplayText;
			bool isNext = nextInfo.isNext;
			bool flag = nextInfo.isSkip;
			if (!isCompleteDisplayText)
			{
				if (isNext || isSkip || flag)
				{
					textController.ForceCompleteDisplayText();
					nowCommandList.ProcessEnd();
				}
				return false;
			}
			autoWaitTimer = Mathf.Min(autoWaitTimer + Time.deltaTime, autoWaitTime);
			bool isProcessing = nowCommandList.Count > 0;
			bool flag2 = textHash.Count > 0;
			bool flag3 = false;
			if (flag || isSkip)
			{
				flag3 = (byte)((flag3 ? 1u : 0u) | 1u) != 0;
			}
			else if (isAuto && flag2)
			{
				flag3 |= autoWaitTimer >= autoWaitTime && !isProcessing;
			}
			nextMarker.SafeProc(delegate(Image p)
			{
				p.enabled = windowImage.enabled && (_isChoice || !isProcessing);
			});
			flag3 = flag3 || isNext;
			if (regulate.control.HasFlag(Regulate.Control.Next))
			{
				flag3 = false;
			}
			flag3 = flag3 || (!flag2 && !isProcessing);
			if (flag3)
			{
				nextMarker.SafeProc(delegate(Image p)
				{
					p.enabled = false;
				});
				currentCharaData.Clear();
				RequestNextLine();
			}
			return flag3;
		}

		protected virtual void UpdateBefore()
		{
			if (msgWindowCanvas != null)
			{
				msgWindowCanvas.enabled = !textController.MessageWindow.text.IsNullOrEmpty();
			}
			if (windowImage != null)
			{
				windowImage.enabled = isWindowImage && !textController.MessageWindow.text.IsNullOrEmpty();
			}
			textController.FontColor = commandController.fontColor[fontColorKey ?? string.Empty];
		}

		protected virtual bool UpdateRegulate()
		{
			if (Manager.Scene.isReturnTitle || Manager.Scene.isGameEnd)
			{
				return true;
			}
			if (!Singleton<Manager.Scene>.IsInstance() || !Singleton<Game>.IsInstance())
			{
				return true;
			}
			if (Singleton<Manager.Scene>.Instance.IsNowLoading)
			{
				return true;
			}
			if (advScene != null && advScene.Map != null && advScene.Map.isMapLoading)
			{
				return true;
			}
			if (_isWait)
			{
				return true;
			}
			StartRun();
			foreach (CharaData value in commandController.Characters.Values)
			{
				if (!value.initialized)
				{
					return true;
				}
			}
			if (_isSceneFadeRegulate && Singleton<Manager.Scene>.Instance.sceneFade.IsFadeNow)
			{
				return true;
			}
			if (Singleton<Manager.Scene>.Instance.IsOverlap)
			{
				return true;
			}
			if (Mathf.Max(0, Singleton<Manager.Scene>.Instance.NowSceneNames.IndexOf("ADV")) > 0)
			{
				return true;
			}
			return false;
		}

		private static bool NameCheck(string text)
		{
			return new string[2] { "P", "H" }.Any((string head) => text.Check(true, (string s) => head + s, SaveData.CharaData.Names) != -1);
		}

		private void OneceTagCovert(ref string key, ref string value)
		{
			string text = null;
			if (key != null)
			{
				if (!(key == "P"))
				{
					if (key == "H" && currentHeroine != null)
					{
						text = key + SaveData.CharaData.Names[(int)currentHeroine.nameType];
					}
				}
				else if (Singleton<Game>.IsInstance() && Singleton<Game>.Instance.Player != null)
				{
					text = key + SaveData.CharaData.Names[(int)Singleton<Game>.Instance.Player.nameType];
				}
			}
			if (text != null)
			{
				key = text;
				if (Replaces.TryGetValue(text, out value))
				{
				}
			}
		}

		protected virtual void Awake()
		{
			if (backCamera != null)
			{
				backCameraBackup = new BackupPosRot(backCamera.transform);
			}
		}

		protected virtual void OnEnable()
		{
			Initialize();
		}

		protected virtual void OnDisable()
		{
			if (!Manager.Scene.isGameEnd)
			{
				Release();
				MemberInit();
			}
		}

		protected virtual void Start()
		{
			faceIconParamDic = new Dictionary<string, ADVFaceIconData.Param>();
			CommonLib.GetAssetBundleNameListFromPath("adv/faceicon/list/", true).ForEach(delegate(string file)
			{
				AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset(file, typeof(ADVFaceIconData));
				ADVFaceIconData[] allAssets = assetBundleLoadAssetOperation.GetAllAssets<ADVFaceIconData>();
				foreach (ADVFaceIconData aDVFaceIconData in allAssets)
				{
					foreach (ADVFaceIconData.Param item in aDVFaceIconData.param)
					{
						faceIconParamDic[item.key] = item;
					}
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
		}

		protected virtual void Update()
		{
			UpdateBefore();
			if (!UpdateRegulate())
			{
				KeyInput.Data data = KeyInput.TextNext(true, isCameraLock);
				MessageWindowProc(new NextInfo(textController.IsCompleteDisplayText, data.isCheck, KeyInput.SkipButton));
			}
		}
	}
}
