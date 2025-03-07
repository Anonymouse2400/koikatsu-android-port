using System;
using System.Collections.Generic;
using System.Linq;
using CustomUtility;
using FileListUI;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomControl : MonoBehaviour
	{
		public enum MainCameraMode
		{
			Custom = 0,
			View = 1,
			Save = 2
		}

		private MainCameraMode mainCameraMode;

		[SerializeField]
		private CustomCapture _customCap;

		[SerializeField]
		private CvsDrawCtrl _cmpDrawCtrl;

		[SerializeField]
		private CvsEye02 cmpEye02;

		[SerializeField]
		private CustomConfig cmpConfig;

		public Canvas cvsChangeScene;

		[SerializeField]
		private CanvasGroup cvgCustomUI;

		[SerializeField]
		private GameObject objFrontUI;

		[SerializeField]
		private Canvas cvsSpace;

		[SerializeField]
		private Canvas cvsShortcut;

		[SerializeField]
		private Canvas cvsExit;

		[SerializeField]
		private UI_DrawTimer timerSpace;

		[SerializeField]
		private GameObject objBackUIGroup;

		[SerializeField]
		private GameObject objFrontUIGroup;

		[SerializeField]
		private GameObject objCaptureTop;

		[SerializeField]
		private GameObject objCaptureSelect;

		[SerializeField]
		private GameObject objNowCardBack;

		[SerializeField]
		private GameObject objCaptureCaution;

		[SerializeField]
		private Toggle tglCapFace;

		[SerializeField]
		private Button btnCapture;

		[SerializeField]
		private Button btnSave;

		[SerializeField]
		private Button btnBackCustom;

		[SerializeField]
		private CanvasGroup cvgMenuParam;

		public CustomGuideObject[] cmpGuid;

		[SerializeField]
		private BoolReactiveProperty _hideFrontUI = new BoolReactiveProperty(false);

		private BoolReactiveProperty _saveMode = new BoolReactiveProperty(false);

		public bool saveNew;

		public string saveFileName = string.Empty;

		[HideInInspector]
		public bool capFaceOnly;

		public CustomFileListCtrl saveFileListCtrl;

		private bool firstUpdate = true;

		[SerializeField]
		private TMP_Dropdown ddCoordinate;

		[SerializeField]
		private Text textFullName;

		[SerializeField]
		private Toggle tglBGMOnOff;

		[SerializeField]
		private CameraControl_Ver2 camCtrl;

		[SerializeField]
		private GameScreenShot cmpCapture;

		public ChaFileControl backChaFileCtrl;

		public CustomCapture customCap
		{
			get
			{
				return _customCap;
			}
		}

		public CvsDrawCtrl cmpDrawCtrl
		{
			get
			{
				return _cmpDrawCtrl;
			}
		}

		public bool hideFrontUI
		{
			get
			{
				return _hideFrontUI.Value;
			}
			set
			{
				_hideFrontUI.Value = value;
			}
		}

		public bool saveMode
		{
			get
			{
				return _saveMode.Value;
			}
			set
			{
				_saveMode.Value = value;
			}
		}

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return customBase.chaCtrl;
			}
		}

		private ChaFileFace face
		{
			get
			{
				return chaCtrl.chaFile.custom.face;
			}
		}

		private ChaFileHair hair
		{
			get
			{
				return chaCtrl.chaFile.custom.hair;
			}
		}

		private ChaFileControl chaFile
		{
			get
			{
				return chaCtrl.chaFile;
			}
		}

		public void Initialize(int sex, bool _new)
		{
			objCaptureTop.SetActiveIfDifferent(false);
			objCaptureCaution.SetActiveIfDifferent(!_new);
			string assetBunndlePath = "custom/customscenelist.unity3d";
			customBase.customCtrl = this;
			customBase.lstEyebrow = ChaListControl.LoadExcelData(assetBunndlePath, "cus_eb_ptn", 0, 0);
			customBase.lstEye = ChaListControl.LoadExcelData(assetBunndlePath, "cus_e_ptn", 0, 0);
			customBase.lstMouth = ChaListControl.LoadExcelData(assetBunndlePath, "cus_m_ptn", 0, 0);
			List<ExcelData.Param> list = ChaListControl.LoadExcelData(assetBunndlePath, "cus_pose", 1, 0);
			for (int i = 14; i < 99; i++)
			{
				string assetBunndlePath2 = string.Format("custom/customscenelist_{0:00}.unity3d", i);
				List<ExcelData.Param> list2 = ChaListControl.LoadExcelData(assetBunndlePath2, "cus_pose", 1, 0);
				if (list2 == null)
				{
					continue;
				}
				foreach (ExcelData.Param item in list2)
				{
					list.Add(item);
				}
			}
			if (list != null)
			{
				customBase.lstPose = list.Where(delegate(ExcelData.Param param)
				{
					if (sex == 0 && "0" == param.list[5])
					{
						return false;
					}
					return (sex != 1 || !("0" == param.list[6])) ? true : false;
				}).ToList();
			}
			customBase.lstEyesLook = ChaListControl.LoadExcelData(assetBunndlePath, "cus_eyeslook", 1, 0);
			customBase.lstNeckLook = ChaListControl.LoadExcelData(assetBunndlePath, "cus_necklook", 1, 0);
			customBase.lstFileList = ChaListControl.LoadExcelData(assetBunndlePath, "cus_filelist", 2, 0);
			customBase.lstSelectList = ChaListControl.LoadExcelData(assetBunndlePath, "cus_selectlist", 2, 0);
			VoiceInfo.Param[] array = Singleton<Voice>.Instance.voiceInfoDic.Values.Where((VoiceInfo.Param x) => 0 <= x.No).ToArray();
			VoiceInfo.Param[] array2 = array;
			foreach (VoiceInfo.Param param2 in array2)
			{
				customBase.dictPersonality[param2.No] = param2.Personality;
			}
			if (Localize.Translate.Manager.isTranslate)
			{
				Dictionary<int, Dictionary<int, Data.Param>> self = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CUSTOM_LIST2);
				int num = 0;
				Dictionary<int, Data.Param> categorys = self.Get(num++);
				customBase.lstEyebrow = customBase.lstEyebrow.Select(delegate(ExcelData.Param x)
				{
					x.list[1] = categorys.SafeGetText(int.Parse(x.list[0])) ?? x.list[1];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys2 = self.Get(num++);
				customBase.lstEye = customBase.lstEye.Select(delegate(ExcelData.Param x)
				{
					x.list[1] = categorys2.SafeGetText(int.Parse(x.list[0])) ?? x.list[1];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys3 = self.Get(num++);
				customBase.lstMouth = customBase.lstMouth.Select(delegate(ExcelData.Param x)
				{
					x.list[1] = categorys3.SafeGetText(int.Parse(x.list[0])) ?? x.list[1];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys4 = self.Get(num++);
				customBase.lstEyesLook = customBase.lstEyesLook.Select(delegate(ExcelData.Param x)
				{
					x.list[1] = categorys4.SafeGetText(int.Parse(x.list[0])) ?? x.list[1];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys5 = self.Get(num++);
				customBase.lstNeckLook = customBase.lstNeckLook.Select(delegate(ExcelData.Param x)
				{
					x.list[1] = categorys5.SafeGetText(int.Parse(x.list[0])) ?? x.list[1];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys6 = self.Get(num++);
				customBase.lstPose = customBase.lstPose.Select(delegate(ExcelData.Param x)
				{
					x.list[3] = categorys6.SafeGetText(int.Parse(x.list[0])) ?? x.list[3];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys7 = self.Get(num++);
				customBase.lstFileList = customBase.lstFileList.Select(delegate(ExcelData.Param x)
				{
					x.list[2] = categorys7.SafeGetText(int.Parse(x.list[0])) ?? x.list[2];
					return x;
				}).ToList();
				Dictionary<int, Data.Param> categorys8 = self.Get(num++);
				customBase.lstSelectList = customBase.lstSelectList.Select(delegate(ExcelData.Param x)
				{
					x.list[2] = categorys8.SafeGetText(int.Parse(x.list[0])) ?? x.list[2];
					return x;
				}).ToList();
			}
			customBase.saveFrameAssist.CreateSaveFrameToHierarchy(base.transform);
			customBase.drawSaveFrameTop = false;
			customBase.drawSaveFrameBack = true;
			customBase.drawSaveFrameFront = true;
			if ((bool)tglBGMOnOff)
			{
				tglBGMOnOff.isOn = customBase.customSettingSave.bgmOn;
			}
		}

		public void Entry(ChaControl entryChara, bool _new)
		{
			bool flag = entryChara.sex == 0;
			string path = ((!flag) ? "custom/presets_f_00.unity3d" : "custom/presets_m_00.unity3d");
			Localize.Translate.Manager.DefaultData.GetBundlePath(ref path);
			string assetName = ((!flag) ? "ill_Default_Female" : "ill_Default_Male");
			customBase.defChaInfo.LoadFromAssetBundle(path, assetName);
			customBase.modeNew = _new;
			customBase.chaCtrl = entryChara;
			Singleton<CustomHistory>.Instance.Clear();
			Singleton<CustomHistory>.Instance.Add1(entryChara, null);
			Singleton<CustomHistory>.Instance.SetOpenData(entryChara);
			customBase.modeSex = entryChara.sex;
			SetLayoutModeSex();
			SetLayoutModeNew();
			customBase.updateCustomUI = true;
		}

		public void ChangePlayBGM(bool isOn)
		{
			if (isOn)
			{
				if (customBase.initCustomBGM)
				{
					Singleton<Manager.Sound>.Instance.PlayBGM();
				}
				else
				{
					Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.Custom));
					customBase.initCustomBGM = true;
				}
				customBase.customSettingSave.bgmOn = true;
			}
			else
			{
				Singleton<Manager.Sound>.Instance.PauseBGM();
				customBase.customSettingSave.bgmOn = false;
			}
		}

		public void UpdateCharaNameText()
		{
			textFullName.text = chaCtrl.chaFile.parameter.fullname;
		}

		private void SetLayoutModeSex()
		{
			if (customBase.modeSex != 0)
			{
			}
		}

		private void SetLayoutModeNew()
		{
			if (!customBase.modeNew)
			{
			}
		}

		public void ChangeMainCameraRect(MainCameraMode _mode)
		{
			if (!customCap.camMain)
			{
				return;
			}
			float x = 0f;
			float y = 0f;
			float width = 1f;
			float height = 1f;
			switch (_mode)
			{
			case MainCameraMode.Custom:
				x = 0.325f;
				break;
			case MainCameraMode.Save:
				if ((bool)tglCapFace)
				{
					if (tglCapFace.isOn)
					{
						x = 0.3745f;
						y = 0.204f;
						width = 0.251f;
						height = 0.592f;
					}
					else
					{
						x = 0.303125f;
						y = 0.011f;
						width = 63f / 160f;
						height = 0.977f;
					}
				}
				break;
			}
			customCap.camMain.rect = new Rect(x, y, width, height);
		}

		public void ChangeCapCameraRect(bool _capFace)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 1f;
			if (_capFace)
			{
				num = 0.3745f;
				num2 = 0.204f;
				num3 = 0.251f;
				num4 = 0.592f;
			}
			else
			{
				num = 0.303125f;
				num2 = 0.011f;
				num3 = 63f / 160f;
				num4 = 0.977f;
			}
			customCap.camMain.rect = new Rect(num, num2, num3, num4);
		}

		public void ResetCamera()
		{
			camCtrl.Reset(0);
		}

		private void Start()
		{
			Observable.EveryUpdate().Subscribe(delegate
			{
				if ((bool)chaCtrl && (bool)btnSave)
				{
					btnSave.interactable = chaCtrl.chaFile.pngData != null && chaCtrl.chaFile.facePngData != null;
				}
			}).AddTo(this);
			if (null == camCtrl)
			{
				GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
				if ((bool)gameObject)
				{
					camCtrl = gameObject.GetComponent<CameraControl_Ver2>();
				}
			}
			_hideFrontUI.Subscribe(delegate(bool h)
			{
				if (!saveMode)
				{
					if ((bool)objFrontUI)
					{
						objFrontUI.SetActiveIfDifferent(!h);
					}
					ChangeMainCameraRect(h ? MainCameraMode.View : MainCameraMode.Custom);
					cvsSpace.enabled = h;
					if (h)
					{
						timerSpace.Setup(2f, 0.3f);
					}
				}
			});
			if ((bool)tglBGMOnOff)
			{
				tglBGMOnOff.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					ChangePlayBGM(isOn);
				});
			}
			if ((bool)ddCoordinate)
			{
				ddCoordinate.onValueChanged.AddListener(delegate(int dd)
				{
					if (dd != -1)
					{
						customBase.chaCtrl.ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)dd);
						customBase.updateCustomUI = true;
					}
				});
			}
			_saveMode.Subscribe(delegate(bool isSaveMode)
			{
				if (capFaceOnly)
				{
					tglCapFace.isOn = true;
					RectTransform rectTransform = btnCapture.transform as RectTransform;
					rectTransform.anchoredPosition = new Vector2(1384f, -944f);
				}
				else
				{
					RectTransform rectTransform2 = btnCapture.transform as RectTransform;
					rectTransform2.anchoredPosition = new Vector2(1384f, -784f);
				}
				if (null != objCaptureTop)
				{
					objCaptureTop.SetActiveIfDifferent(isSaveMode);
					objCaptureCaution.SetActiveIfDifferent(!capFaceOnly && !Singleton<CustomBase>.Instance.modeNew);
					objCaptureSelect.SetActiveIfDifferent(!capFaceOnly);
					objNowCardBack.SetActiveIfDifferent(!capFaceOnly);
					btnSave.gameObject.SetActiveIfDifferent(!capFaceOnly);
				}
				customBase.drawSaveFrameTop = isSaveMode && !tglCapFace.isOn && !capFaceOnly;
				if (isSaveMode)
				{
					customCap.UpdateFaceImage(chaFile.facePngData);
					if (!capFaceOnly)
					{
						customCap.UpdateCardImage(chaFile.pngData);
					}
				}
				if ((bool)cvgCustomUI)
				{
					cvgCustomUI.Enable(!isSaveMode);
				}
				if ((bool)_cmpDrawCtrl && (bool)_cmpDrawCtrl.objFrameTop && !capFaceOnly)
				{
					_cmpDrawCtrl.objFrameTop.SetActiveIfDifferent(isSaveMode);
				}
				ChangeMainCameraRect(isSaveMode ? MainCameraMode.Save : MainCameraMode.Custom);
			});
			if ((bool)tglCapFace)
			{
				tglCapFace.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (saveMode)
					{
						ChangeCapCameraRect(isOn);
					}
					customBase.drawSaveFrameTop = !isOn;
				});
			}
			if ((bool)btnCapture)
			{
				btnCapture.OnClickAsObservable().Subscribe(delegate
				{
					Illusion.Game.Utils.Sound.Play(SystemSE.photo);
					if ((bool)_customCap && (bool)tglCapFace)
					{
						if (tglCapFace.isOn)
						{
							byte[] facePngData = _customCap.CapCharaFace(true);
							chaFile.facePngData = facePngData;
							customCap.UpdateFaceImage(chaFile.facePngData);
						}
						else
						{
							byte[] pngData = _customCap.CapCharaCard(true, customBase.saveFrameAssist);
							chaFile.pngData = pngData;
							customCap.UpdateCardImage(chaFile.pngData);
						}
					}
				});
			}
			if (btnSave != null)
			{
				btnSave.OnClickAsObservable().Subscribe(delegate
				{
					Illusion.Game.Utils.Sound.Play(SystemSE.ok_s);
					bool flag = chaCtrl.sex == 0;
					DateTime now = DateTime.Now;
					string text = (saveNew ? string.Format("Koikatu_{0}_{1}", (!flag) ? "F" : "M", FolderAssist.TimeStamp(now)) : saveFileName);
					chaCtrl.chaFile.SaveCharaFile(text);
					if (saveFileListCtrl != null)
					{
						if (flag)
						{
							saveFileListCtrl.visibleType = VisibleType.AddHide;
						}
						CustomFileInfo customFileInfo;
						if (!saveNew)
						{
							int[] selectIndex = saveFileListCtrl.GetSelectIndex();
							customFileInfo = saveFileListCtrl.GetFileInfoFromIndex(selectIndex[0]);
						}
						else
						{
							string fullPath = UserData.Path + ((!flag) ? "chara/female/" : "chara/male/") + text + ".png";
							customFileInfo = new CustomFileInfo(new FolderAssist.FileInfo(fullPath, text, null))
							{
								index = saveFileListCtrl.GetNoUseIndex()
							};
						}
						customFileInfo.UpdateInfo(chaCtrl.fileParam, now);
						if (saveNew)
						{
							saveFileListCtrl.Add(customFileInfo);
						}
						else
						{
							customFileInfo.fic.UpdateInfo(null);
							saveFileListCtrl.UpdateSort();
						}
					}
					saveMode = false;
				});
			}
			if ((bool)btnBackCustom)
			{
				btnBackCustom.OnClickAsObservable().Subscribe(delegate
				{
					Illusion.Game.Utils.Sound.Play(SystemSE.cancel);
					capFaceOnly = false;
					saveMode = false;
				});
			}
		}

		private void Update()
		{
			Singleton<CustomBase>.Instance.shortcutConfig.enabled = false;
			if ((bool)camCtrl)
			{
				camCtrl.isConfigTargetTex = Manager.Config.EtcData.Look;
			}
			if (saveMode)
			{
				if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
				{
					if ((bool)camCtrl)
					{
						camCtrl.NoCtrlCondition = () => false;
					}
				}
				else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && Illusion.Utils.uGUI.isMouseHit && (bool)camCtrl)
				{
					camCtrl.NoCtrlCondition = () => true;
				}
			}
			else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
			{
				if ((bool)camCtrl)
				{
					camCtrl.NoCtrlCondition = () => false;
				}
			}
			else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && Illusion.Utils.uGUI.isMouseHit && (bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => true;
			}
			if (cvsShortcut.enabled)
			{
				if (Input.anyKeyDown)
				{
					cvsShortcut.enabled = false;
				}
				return;
			}
			customBase.UpdateIKCalc();
			if (customBase.playSampleVoice && !Singleton<Manager.Sound>.Instance.IsPlay(Manager.Sound.Type.SystemSE))
			{
				chaCtrl.ChangeEyebrowPtn(customBase.backEyebrowPtn);
				chaCtrl.ChangeEyesPtn(customBase.backEyesPtn);
				chaCtrl.HideEyeHighlight(false);
				chaCtrl.ChangeEyesBlinkFlag(customBase.backBlink);
				chaCtrl.ChangeEyesOpenMax(customBase.backEyesOpen);
				chaCtrl.ChangeMouthPtn(customBase.backMouthPtn);
				chaCtrl.ChangeMouthFixed(customBase.backMouthFix);
				chaCtrl.ChangeMouthOpenMax(customBase.backMouthOpen);
				customBase.playSampleVoice = false;
			}
			bool isInputFocused = customBase.IsInputFocused();
			if (!isInputFocused && "CustomScene" == Singleton<Scene>.Instance.NowSceneNames[0])
			{
				if (Input.GetKeyDown(KeyCode.F2))
				{
					if ((bool)cvsShortcut)
					{
						cvsShortcut.enabled = true;
					}
				}
				else if (Input.GetKeyDown(KeyCode.F11))
				{
					if (hideFrontUI && null != cmpCapture)
					{
						cmpCapture.Capture(string.Empty);
					}
				}
				else if (Input.GetKeyDown(KeyCode.Escape))
				{
					Illusion.Game.Utils.Scene.GameEnd();
				}
				else if (Input.GetKeyDown(KeyCode.Space))
				{
					hideFrontUI = !hideFrontUI;
				}
				else if (Input.GetKeyDown(KeyCode.Alpha1))
				{
					Manager.Config.EtcData.Look = !Manager.Config.EtcData.Look;
				}
				else if (Input.GetKeyDown(KeyCode.F1))
				{
					Singleton<CustomBase>.Instance.shortcutConfig.enabled = true;
				}
			}
			if (!firstUpdate && Singleton<CustomBase>.IsInstance() && customBase.updateCustomUI)
			{
				customBase.changeCharaName = true;
				customBase.updateCvsFaceAll = true;
				customBase.updateCvsEar = true;
				customBase.updateCvsChin = true;
				customBase.updateCvsCheek = true;
				customBase.updateCvsEyebrow = true;
				customBase.updateCvsEye01 = true;
				customBase.updateCvsEye02 = true;
				customBase.updateCvsNose = true;
				customBase.updateCvsMouth = true;
				customBase.updateCvsMole = true;
				customBase.updateCvsBaseMakeup = true;
				customBase.updateCvsFaceShapeAll = true;
				customBase.updateCvsBodyAll = true;
				customBase.updateCvsBreast = true;
				customBase.updateCvsBodyUpper = true;
				customBase.updateCvsBodyLower = true;
				customBase.updateCvsArm = true;
				customBase.updateCvsLeg = true;
				customBase.updateCvsNail = true;
				customBase.updateCvsUnderhair = true;
				customBase.updateCvsSunburn = true;
				customBase.updateCvsBodyPaint = true;
				customBase.updateCvsBodyShapeAll = true;
				customBase.updateCvsHairFront = true;
				customBase.updateCvsHairBack = true;
				customBase.updateCvsHairSide = true;
				customBase.updateCvsHairExtension = true;
				customBase.updateCvsHairEtc = true;
				customBase.updateCvsCosTop = true;
				customBase.updateCvsCosBot = true;
				customBase.updateCvsCosBra = true;
				customBase.updateCvsCosShorts = true;
				customBase.updateCvsCosGloves = true;
				customBase.updateCvsCosPanst = true;
				customBase.updateCvsCosSocks = true;
				customBase.updateCvsCosInnerShoes = true;
				customBase.updateCvsCosOuterShoes = true;
				customBase.updateCvsClothesCopy = true;
				for (int i = 0; i < 20; i++)
				{
					customBase.SetUpdateCvsAccessory(i, true);
				}
				customBase.updateCvsAccessoryCopy = true;
				customBase.updateCvsAccessoryChange = true;
				customBase.updateCvsChara = true;
				customBase.updateCvsCharaEx = true;
				customBase.updateCvsH = true;
				customBase.updateCvsQA = true;
				customBase.updateCvsAttribute = true;
				customBase.updateCvsADK = true;
				customBase.updateCvsConfig = true;
				customBase.updateCustomUI = false;
			}
			if ((bool)camCtrl)
			{
				camCtrl.KeyCondition = () => !isInputFocused;
			}
			firstUpdate = false;
		}

		private void LateUpdate()
		{
		}
	}
}
