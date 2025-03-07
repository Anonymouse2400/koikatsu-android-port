using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Illusion;
using Illusion.Component.UI;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ADV
{
	public sealed class MainScenario : TextScenario
	{
		public enum Mode
		{
			Normal = 0,
			WindowNone = 1,
			BackLog = 2,
			Movie = 3
		}

		private class AlreadyReadInfo
		{
			private const string Path = "save";

			private const string FileName = "read.dat";

			private HashSet<int> read = new HashSet<int>();

			public AlreadyReadInfo()
			{
				read.Add(0);
				Load();
			}

			public bool Add(int i)
			{
				return read.Add(i);
			}

			public void Save()
			{
				Utils.File.OpenWrite(UserData.Create("save") + "read.dat", false, delegate(FileStream f)
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(f))
					{
						binaryWriter.Write(read.Count);
						foreach (int item in read)
						{
							binaryWriter.Write(item);
						}
					}
				});
			}

			public bool Load()
			{
				return Utils.File.OpenRead(UserData.Path + "save" + '/' + "read.dat", delegate(FileStream f)
				{
					using (BinaryReader binaryReader = new BinaryReader(f))
					{
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							read.Add(binaryReader.ReadInt32());
						}
					}
				});
			}
		}

		[Serializable]
		public class ModeReactiveProperty : ReactiveProperty<Mode>
		{
			public ModeReactiveProperty()
			{
			}

			public ModeReactiveProperty(Mode initialValue)
				: base(initialValue)
			{
			}
		}

		[SerializeField]
		private BackLog backLog;

		[SerializeField]
		private ADVButton advButton;

		[SerializeField]
		private float movieAutoWaitTime = 3f;

		[SerializeField]
		private ModeReactiveProperty _mode = new ModeReactiveProperty(Mode.Normal);

		[SerializeField]
		private Canvas canvas;

		private CanvasGroup canvasGroup;

		private static AlreadyReadInfo readInfo;

		public BackLog BackLog
		{
			get
			{
				return backLog;
			}
		}

		public Mode mode
		{
			get
			{
				return _mode.Value;
			}
			set
			{
				_mode.Value = value;
			}
		}

		public bool modeChanging { get; private set; }

		public static void LoadReadInfo()
		{
			if (readInfo == null)
			{
				readInfo = new AlreadyReadInfo();
			}
		}

		public static void SaveReadInfo()
		{
			if (readInfo != null)
			{
				readInfo.Save();
			}
		}

		public override void Initialize()
		{
			LoadReadInfo();
			base.Initialize();
			advButton.IsVisible = windowImage.enabled;
			backLog.Clear();
		}

		public override void Release()
		{
			base.Release();
			advButton.IsVisible = windowImage.enabled;
			if (!Singleton<Game>.IsInstance())
			{
				SaveReadInfo();
			}
			_mode.Value = Mode.Normal;
			Lookat_dan component = GetComponent<Lookat_dan>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			HMotionShake component2 = GetComponent<HMotionShake>();
			if (component2 != null)
			{
				UnityEngine.Object.Destroy(component2);
			}
		}

		public void VoicePlay(List<IVoice[]> voices)
		{
			foreach (IVoice[] voice2 in voices)
			{
				IVoice[] array = voice2;
				foreach (IVoice voice in array)
				{
					voice.Convert2D();
				}
			}
			currentCharaData.isSkip = true;
			VoicePlay(voices, null, null);
		}

		protected override void Start()
		{
			base.Start();
			canvasGroup = canvas.GetComponent<CanvasGroup>();
			if (canvasGroup != null)
			{
				this.ObserveEveryValueChanged((MainScenario _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
				{
					canvasGroup.interactable = isOn;
				});
			}
			_mode.Scan(delegate(Mode prev, Mode current)
			{
				switch (prev)
				{
				case Mode.Movie:
					regulate.SetRegulate((Regulate.Control)0);
					base.isWindowImage = true;
					base.textController.nameVisible = true;
					base.textController.MessageWindow.alignment = base.textController.NameWindow.alignment;
					base.textController.isMovie = false;
					break;
				}
				return current;
			}).Subscribe(delegate(Mode mode)
			{
				modeChanging = true;
				switch (mode)
				{
				case Mode.Normal:
					msgWindowCanvas.enabled = true;
					base.Choices.gameObject.SetActive(base.isChoice);
					backLog.Visible = false;
					break;
				case Mode.WindowNone:
					base.isSkip = false;
					base.isAuto = false;
					msgWindowCanvas.enabled = false;
					base.Choices.gameObject.SetActive(false);
					break;
				case Mode.BackLog:
					base.isSkip = false;
					base.isAuto = false;
					msgWindowCanvas.enabled = true;
					base.Choices.gameObject.SetActive(false);
					backLog.Visible = true;
					break;
				case Mode.Movie:
					regulate.SetRegulate((Regulate.Control)20);
					base.isWindowImage = false;
					base.textController.nameVisible = false;
					base.textController.MessageWindow.alignment = TextAnchor.LowerCenter;
					base.textController.isMovie = true;
					break;
				}
			});
			Func<bool> isAddReg = delegate
			{
				if (modeChanging)
				{
					modeChanging = false;
					return false;
				}
				if (base.advScene.startAddSceneName != Singleton<Scene>.Instance.AddSceneName)
				{
					base.isSkip = false;
					base.isAuto = false;
					currentCharaData.isSkip = true;
					return false;
				}
				return true;
			};
			(from _ in this.UpdateAsObservable().Do(delegate
				{
					UpdateBefore();
				})
				where isAddReg()
				where !UpdateRegulate()
				select _).Subscribe(delegate
			{
				bool flag = false;
				bool flag2 = true;
				KeyInput.Data data = KeyInput.TextNext((!base.isCameraLock) ? base.isSelectMessageWindow : flag2, base.isCameraLock);
				if (data.isMouse)
				{
					flag = !Input.GetMouseButtonDown(0) || !advButton.isSelect;
				}
				flag |= data.isKey;
				if (flag && base.isSkip && !base.isChoice)
				{
					base.isSkip = false;
				}
				if (!base.isCameraLock && _mode.Value == Mode.Normal)
				{
					flag2 = base.isSelectMessageWindow;
				}
				bool flag3 = KeyInput.SkipButton;
				bool isCompleteDisplayText = false;
				switch (_mode.Value)
				{
				case Mode.Normal:
					if (advButton.gameObject.activeSelf)
					{
						if (advButton.BackLog.interactable)
						{
							KeyInput.Data data2 = KeyInput.BackLogButton(flag2, base.isCameraLock);
							if (data2.isCheck && (!base.isChoice || !data2.isKey || !Input.GetKeyDown(KeyCode.UpArrow)))
							{
								_mode.Value = Mode.BackLog;
							}
						}
						if (advButton.Close.interactable && KeyInput.WindowNoneButton(flag2, base.isCameraLock).isCheck)
						{
							bool flag4 = true;
							if (advButton.isFocus)
							{
								flag4 = advButton.Close.GetComponent<SelectUI>().isFocus;
							}
							if (flag4)
							{
								_mode.Value = Mode.WindowNone;
							}
						}
					}
					isCompleteDisplayText = base.textController.IsCompleteDisplayText;
					break;
				case Mode.WindowNone:
					if (!advButton.isSelect && !advButton.isFocus && KeyInput.WindowNoneButtonCancel(flag2, base.isCameraLock).isCheck)
					{
						_mode.Value = Mode.Normal;
					}
					isCompleteDisplayText = base.textController.IsCompleteDisplayText;
					flag = false;
					flag3 = false;
					break;
				case Mode.BackLog:
					if (!advButton.isSelect && !advButton.isFocus && KeyInput.BackLogButtonCancel(flag2, base.isCameraLock).isCheck)
					{
						_mode.Value = Mode.Normal;
					}
					isCompleteDisplayText = base.textController.IsCompleteDisplayText;
					flag = false;
					flag3 = false;
					break;
				case Mode.Movie:
					base.textController.ForceCompleteDisplayText();
					isCompleteDisplayText = base.textController.IsCompleteDisplayText;
					autoWaitTime = movieAutoWaitTime;
					flag = false;
					flag3 = false;
					break;
				}
				MessageWindowProc(new NextInfo(isCompleteDisplayText, flag, flag3));
			});
		}

		protected override void UpdateBefore()
		{
			if (_mode.Value != Mode.WindowNone && msgWindowCanvas != null)
			{
				msgWindowCanvas.enabled = !base.textController.MessageWindow.text.IsNullOrEmpty();
			}
			if (windowImage != null)
			{
				windowImage.enabled = base.isWindowImage && !base.textController.MessageWindow.text.IsNullOrEmpty();
			}
			bool flag = base.isWindowImage;
			advButton.IsVisible = flag;
			if (!flag)
			{
				nextMarker.enabled = false;
			}
			base.textController.FontColor = base.commandController.fontColor[base.fontColorKey ?? string.Empty];
		}

		protected override void Update()
		{
		}

		protected override IEnumerator _RequestNextLine()
		{
			yield return _003C_RequestNextLine_003E__BaseCallProxy0();
			foreach (int item in textHash)
			{
				if (readInfo.Add(item) && base.isSkip && Manager.Config.TextData.ReadSkip)
				{
					base.isSkip = false;
				}
			}
		}

		[DebuggerHidden]
		[CompilerGenerated]
		private IEnumerator _003C_RequestNextLine_003E__BaseCallProxy0()
		{
			return base._RequestNextLine();
		}
	}
}
