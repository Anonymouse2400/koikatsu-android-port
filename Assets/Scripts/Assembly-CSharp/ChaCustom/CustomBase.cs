using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion.Component;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomBase : Singleton<CustomBase>
	{
		public class CustomSettingSave
		{
			private const string strSave = "custom/customscene.dat";

			public Version version = CustomDefine.CustomSettingVersion;

			public Color backColor = Color.gray;

			public bool hairSameSetting = true;

			public bool hairAutoSetting = true;

			public bool hairOutlineSetting = true;

			public bool acsTakeOverParent = true;

			public bool acsTakeOverColor = true;

			public int[] acsCorrectPosRate = new int[2];

			public int[] acsCorrectRotRate = new int[2];

			public int[] acsCorrectSclRate = new int[2];

			public bool bgmOn = true;

			public bool[] drawController = new bool[2];

			public int[] controllerType = new int[2];

			public float[] controllerSpeed = new float[2] { 0.1f, 0.1f };

			public float[] controllerScale = new float[2] { 1.5f, 1.5f };

			public void Save()
			{
				string path = UserData.Path + "custom/customscene.dat";
				string directoryName = Path.GetDirectoryName(path);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(output))
					{
						binaryWriter.Write(version.ToString());
						binaryWriter.Write(backColor.r);
						binaryWriter.Write(backColor.g);
						binaryWriter.Write(backColor.b);
						binaryWriter.Write(hairSameSetting);
						binaryWriter.Write(hairAutoSetting);
						binaryWriter.Write(hairOutlineSetting);
						binaryWriter.Write(acsTakeOverParent);
						binaryWriter.Write(acsTakeOverColor);
						binaryWriter.Write(acsCorrectPosRate[0]);
						binaryWriter.Write(acsCorrectPosRate[1]);
						binaryWriter.Write(acsCorrectRotRate[0]);
						binaryWriter.Write(acsCorrectRotRate[1]);
						binaryWriter.Write(acsCorrectSclRate[0]);
						binaryWriter.Write(acsCorrectSclRate[1]);
						binaryWriter.Write(bgmOn);
						for (int i = 0; i < 2; i++)
						{
							binaryWriter.Write(drawController[i]);
						}
						for (int j = 0; j < 2; j++)
						{
							binaryWriter.Write(controllerType[j]);
						}
						for (int k = 0; k < 2; k++)
						{
							binaryWriter.Write(controllerSpeed[k]);
						}
						for (int l = 0; l < 2; l++)
						{
							binaryWriter.Write(controllerScale[l]);
						}
					}
				}
			}

			public void Load()
			{
				string path = UserData.Path + "custom/customscene.dat";
				if (!File.Exists(path))
				{
					return;
				}
				using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						version = new Version(binaryReader.ReadString());
						backColor.r = binaryReader.ReadSingle();
						backColor.g = binaryReader.ReadSingle();
						backColor.b = binaryReader.ReadSingle();
						hairSameSetting = binaryReader.ReadBoolean();
						hairAutoSetting = binaryReader.ReadBoolean();
						hairOutlineSetting = binaryReader.ReadBoolean();
						if (version.CompareTo(new Version("0.0.1")) == -1)
						{
							return;
						}
						acsTakeOverParent = binaryReader.ReadBoolean();
						acsTakeOverColor = binaryReader.ReadBoolean();
						if (version.CompareTo(new Version("0.0.2")) == -1)
						{
							return;
						}
						acsCorrectPosRate[0] = binaryReader.ReadInt32();
						acsCorrectPosRate[1] = binaryReader.ReadInt32();
						acsCorrectRotRate[0] = binaryReader.ReadInt32();
						acsCorrectRotRate[1] = binaryReader.ReadInt32();
						acsCorrectSclRate[0] = binaryReader.ReadInt32();
						acsCorrectSclRate[1] = binaryReader.ReadInt32();
						if (version.CompareTo(new Version("0.0.3")) == -1)
						{
							return;
						}
						bgmOn = binaryReader.ReadBoolean();
						if (version.CompareTo(new Version("0.0.4")) != -1)
						{
							for (int i = 0; i < 2; i++)
							{
								drawController[i] = binaryReader.ReadBoolean();
							}
							for (int j = 0; j < 2; j++)
							{
								controllerType[j] = binaryReader.ReadInt32();
							}
							for (int k = 0; k < 2; k++)
							{
								controllerSpeed[k] = binaryReader.ReadSingle();
							}
							for (int l = 0; l < 2; l++)
							{
								controllerScale[l] = binaryReader.ReadSingle();
							}
						}
					}
				}
			}
		}

		[HideInInspector]
		public SaveFrameAssist saveFrameAssist = new SaveFrameAssist();

		[HideInInspector]
		public Vector3[] vecAcsClipBord = new Vector3[3];

		[HideInInspector]
		public bool modeNew = true;

		[HideInInspector]
		public int modeSex = 1;

		[HideInInspector]
		public ChaControl chaCtrl;

		[HideInInspector]
		public bool autoClothesState = true;

		[HideInInspector]
		public int autoClothesStateNo;

		[HideInInspector]
		public int clothesStateNo;

		[HideInInspector]
		public List<ExcelData.Param> lstEyebrow;

		[HideInInspector]
		public List<ExcelData.Param> lstEye;

		[HideInInspector]
		public List<ExcelData.Param> lstMouth;

		[HideInInspector]
		public List<ExcelData.Param> lstPose;

		[HideInInspector]
		public List<ExcelData.Param> lstEyesLook;

		[HideInInspector]
		public List<ExcelData.Param> lstNeckLook;

		[HideInInspector]
		public List<ExcelData.Param> lstFileList;

		[HideInInspector]
		public List<ExcelData.Param> lstSelectList;

		[HideInInspector]
		public string animeAssetBundleName = string.Empty;

		[HideInInspector]
		public string animeAssetName = string.Empty;

		[HideInInspector]
		public string animeStateName = string.Empty;

		[HideInInspector]
		public MotionIK motionIK;

		[HideInInspector]
		public bool initCustomBGM;

		public Dictionary<int, string> dictPersonality = new Dictionary<int, string>();

		[HideInInspector]
		public float sliderWheelSensitive = -0.01f;

		public CustomControl customCtrl;

		public ChaFileControl defChaInfo = new ChaFileControl();

		[HideInInspector]
		public List<TMP_InputField> lstTmpInputField = new List<TMP_InputField>();

		[HideInInspector]
		public List<InputField> lstInputField = new List<InputField>();

		public CustomSettingSave customSettingSave = new CustomSettingSave();

		[HideInInspector]
		public bool playSampleVoice;

		[HideInInspector]
		public int backEyebrowPtn;

		[HideInInspector]
		public int backEyesPtn;

		[HideInInspector]
		public bool backBlink = true;

		[HideInInspector]
		public float backEyesOpen = 1f;

		[HideInInspector]
		public int backMouthPtn;

		[HideInInspector]
		public bool backMouthFix = true;

		[HideInInspector]
		public float backMouthOpen;

		private BoolReactiveProperty _drawSaveFrameTop = new BoolReactiveProperty(false);

		private BoolReactiveProperty _drawSaveFrameBack = new BoolReactiveProperty(false);

		private BoolReactiveProperty _drawSaveFrameFront = new BoolReactiveProperty(false);

		private BoolReactiveProperty _changeCharaName = new BoolReactiveProperty(false);

		public ShortcutKey.Proc shortcutConfig;

		[HideInInspector]
		public bool updateCustomUI;

		private BoolReactiveProperty _updateCvsFaceAll = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsEar = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsChin = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCheek = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsEyebrow = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsEye01 = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsEye02 = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsNose = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsMouth = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsMole = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBaseMakeup = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsFaceShapeAll = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBodyAll = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBreast = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBodyUpper = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBodyLower = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsArm = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsLeg = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsNail = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsUnderhair = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsSunburn = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBodyPaint = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsBodyShapeAll = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsHairFront = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsHairBack = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsHairSide = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsHairExtension = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsHairEtc = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosTop = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosBot = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosBra = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosShorts = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosGloves = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosPanst = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosSocks = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosInnerShoes = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCosOuterShoes = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsClothesCopy = new BoolReactiveProperty(false);

		public Action[] actUpdateCvsAccessory = new Action[20];

		public Action[] actUpdateAcsSlotName = new Action[20];

		private BoolReactiveProperty[] _updateCvsAccessory = new BoolReactiveProperty[20];

		public int selectSlot;

		private BoolReactiveProperty _updateCvsAccessoryCopy = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsAccessoryChange = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsChara = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsCharaEx = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsH = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsQA = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsAttribute = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsADK = new BoolReactiveProperty(false);

		private BoolReactiveProperty _updateCvsConfig = new BoolReactiveProperty(false);

		private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

		public bool drawSaveFrameTop
		{
			get
			{
				return _drawSaveFrameTop.Value;
			}
			set
			{
				_drawSaveFrameTop.Value = value;
			}
		}

		public bool drawSaveFrameBack
		{
			get
			{
				return _drawSaveFrameBack.Value;
			}
			set
			{
				_drawSaveFrameBack.Value = value;
			}
		}

		public bool drawSaveFrameFront
		{
			get
			{
				return _drawSaveFrameFront.Value;
			}
			set
			{
				_drawSaveFrameFront.Value = value;
			}
		}

		public bool changeCharaName
		{
			get
			{
				return _changeCharaName.Value;
			}
			set
			{
				_changeCharaName.Value = value;
			}
		}

		public bool updateCvsFaceAll
		{
			get
			{
				return _updateCvsFaceAll.Value;
			}
			set
			{
				_updateCvsFaceAll.Value = value;
			}
		}

		public bool updateCvsEar
		{
			get
			{
				return _updateCvsEar.Value;
			}
			set
			{
				_updateCvsEar.Value = value;
			}
		}

		public bool updateCvsChin
		{
			get
			{
				return _updateCvsChin.Value;
			}
			set
			{
				_updateCvsChin.Value = value;
			}
		}

		public bool updateCvsCheek
		{
			get
			{
				return _updateCvsCheek.Value;
			}
			set
			{
				_updateCvsCheek.Value = value;
			}
		}

		public bool updateCvsEyebrow
		{
			get
			{
				return _updateCvsEyebrow.Value;
			}
			set
			{
				_updateCvsEyebrow.Value = value;
			}
		}

		public bool updateCvsEye01
		{
			get
			{
				return _updateCvsEye01.Value;
			}
			set
			{
				_updateCvsEye01.Value = value;
			}
		}

		public bool updateCvsEye02
		{
			get
			{
				return _updateCvsEye02.Value;
			}
			set
			{
				_updateCvsEye02.Value = value;
			}
		}

		public bool updateCvsNose
		{
			get
			{
				return _updateCvsNose.Value;
			}
			set
			{
				_updateCvsNose.Value = value;
			}
		}

		public bool updateCvsMouth
		{
			get
			{
				return _updateCvsMouth.Value;
			}
			set
			{
				_updateCvsMouth.Value = value;
			}
		}

		public bool updateCvsMole
		{
			get
			{
				return _updateCvsMole.Value;
			}
			set
			{
				_updateCvsMole.Value = value;
			}
		}

		public bool updateCvsBaseMakeup
		{
			get
			{
				return _updateCvsBaseMakeup.Value;
			}
			set
			{
				_updateCvsBaseMakeup.Value = value;
			}
		}

		public bool updateCvsFaceShapeAll
		{
			get
			{
				return _updateCvsFaceShapeAll.Value;
			}
			set
			{
				_updateCvsFaceShapeAll.Value = value;
			}
		}

		public bool updateCvsBodyAll
		{
			get
			{
				return _updateCvsBodyAll.Value;
			}
			set
			{
				_updateCvsBodyAll.Value = value;
			}
		}

		public bool updateCvsBreast
		{
			get
			{
				return _updateCvsBreast.Value;
			}
			set
			{
				_updateCvsBreast.Value = value;
			}
		}

		public bool updateCvsBodyUpper
		{
			get
			{
				return _updateCvsBodyUpper.Value;
			}
			set
			{
				_updateCvsBodyUpper.Value = value;
			}
		}

		public bool updateCvsBodyLower
		{
			get
			{
				return _updateCvsBodyLower.Value;
			}
			set
			{
				_updateCvsBodyLower.Value = value;
			}
		}

		public bool updateCvsArm
		{
			get
			{
				return _updateCvsArm.Value;
			}
			set
			{
				_updateCvsArm.Value = value;
			}
		}

		public bool updateCvsLeg
		{
			get
			{
				return _updateCvsLeg.Value;
			}
			set
			{
				_updateCvsLeg.Value = value;
			}
		}

		public bool updateCvsNail
		{
			get
			{
				return _updateCvsNail.Value;
			}
			set
			{
				_updateCvsNail.Value = value;
			}
		}

		public bool updateCvsUnderhair
		{
			get
			{
				return _updateCvsUnderhair.Value;
			}
			set
			{
				_updateCvsUnderhair.Value = value;
			}
		}

		public bool updateCvsSunburn
		{
			get
			{
				return _updateCvsSunburn.Value;
			}
			set
			{
				_updateCvsSunburn.Value = value;
			}
		}

		public bool updateCvsBodyPaint
		{
			get
			{
				return _updateCvsBodyPaint.Value;
			}
			set
			{
				_updateCvsBodyPaint.Value = value;
			}
		}

		public bool updateCvsBodyShapeAll
		{
			get
			{
				return _updateCvsBodyShapeAll.Value;
			}
			set
			{
				_updateCvsBodyShapeAll.Value = value;
			}
		}

		public bool updateCvsHairFront
		{
			get
			{
				return _updateCvsHairFront.Value;
			}
			set
			{
				_updateCvsHairFront.Value = value;
			}
		}

		public bool updateCvsHairBack
		{
			get
			{
				return _updateCvsHairBack.Value;
			}
			set
			{
				_updateCvsHairBack.Value = value;
			}
		}

		public bool updateCvsHairSide
		{
			get
			{
				return _updateCvsHairSide.Value;
			}
			set
			{
				_updateCvsHairSide.Value = value;
			}
		}

		public bool updateCvsHairExtension
		{
			get
			{
				return _updateCvsHairExtension.Value;
			}
			set
			{
				_updateCvsHairExtension.Value = value;
			}
		}

		public bool updateCvsHairEtc
		{
			get
			{
				return _updateCvsHairEtc.Value;
			}
			set
			{
				_updateCvsHairEtc.Value = value;
			}
		}

		public bool updateCvsCosTop
		{
			get
			{
				return _updateCvsCosTop.Value;
			}
			set
			{
				_updateCvsCosTop.Value = value;
			}
		}

		public bool updateCvsCosBot
		{
			get
			{
				return _updateCvsCosBot.Value;
			}
			set
			{
				_updateCvsCosBot.Value = value;
			}
		}

		public bool updateCvsCosBra
		{
			get
			{
				return _updateCvsCosBra.Value;
			}
			set
			{
				_updateCvsCosBra.Value = value;
			}
		}

		public bool updateCvsCosShorts
		{
			get
			{
				return _updateCvsCosShorts.Value;
			}
			set
			{
				_updateCvsCosShorts.Value = value;
			}
		}

		public bool updateCvsCosGloves
		{
			get
			{
				return _updateCvsCosGloves.Value;
			}
			set
			{
				_updateCvsCosGloves.Value = value;
			}
		}

		public bool updateCvsCosPanst
		{
			get
			{
				return _updateCvsCosPanst.Value;
			}
			set
			{
				_updateCvsCosPanst.Value = value;
			}
		}

		public bool updateCvsCosSocks
		{
			get
			{
				return _updateCvsCosSocks.Value;
			}
			set
			{
				_updateCvsCosSocks.Value = value;
			}
		}

		public bool updateCvsCosInnerShoes
		{
			get
			{
				return _updateCvsCosInnerShoes.Value;
			}
			set
			{
				_updateCvsCosInnerShoes.Value = value;
			}
		}

		public bool updateCvsCosOuterShoes
		{
			get
			{
				return _updateCvsCosOuterShoes.Value;
			}
			set
			{
				_updateCvsCosOuterShoes.Value = value;
			}
		}

		public bool updateCvsClothesCopy
		{
			get
			{
				return _updateCvsClothesCopy.Value;
			}
			set
			{
				_updateCvsClothesCopy.Value = value;
			}
		}

		public bool updateCvsAccessoryCopy
		{
			get
			{
				return _updateCvsAccessoryCopy.Value;
			}
			set
			{
				_updateCvsAccessoryCopy.Value = value;
			}
		}

		public bool updateCvsAccessoryChange
		{
			get
			{
				return _updateCvsAccessoryChange.Value;
			}
			set
			{
				_updateCvsAccessoryChange.Value = value;
			}
		}

		public bool updateCvsChara
		{
			get
			{
				return _updateCvsChara.Value;
			}
			set
			{
				_updateCvsChara.Value = value;
			}
		}

		public bool updateCvsCharaEx
		{
			get
			{
				return _updateCvsCharaEx.Value;
			}
			set
			{
				_updateCvsCharaEx.Value = value;
			}
		}

		public bool updateCvsH
		{
			get
			{
				return _updateCvsH.Value;
			}
			set
			{
				_updateCvsH.Value = value;
			}
		}

		public bool updateCvsQA
		{
			get
			{
				return _updateCvsQA.Value;
			}
			set
			{
				_updateCvsQA.Value = value;
			}
		}

		public bool updateCvsAttribute
		{
			get
			{
				return _updateCvsAttribute.Value;
			}
			set
			{
				_updateCvsAttribute.Value = value;
			}
		}

		public bool updateCvsADK
		{
			get
			{
				return _updateCvsADK.Value;
			}
			set
			{
				_updateCvsADK.Value = value;
			}
		}

		public bool updateCvsConfig
		{
			get
			{
				return _updateCvsConfig.Value;
			}
			set
			{
				_updateCvsConfig.Value = value;
			}
		}

		private Dictionary<int, Data.Param> translateSlotTitle
		{
			get
			{
				return uiTranslater.Get(996);
			}
		}

		public Dictionary<int, Data.Param> translateQuestionTitle
		{
			get
			{
				return uiTranslater.Get(995);
			}
		}

		public Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
		{
			get
			{
				return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CUSTOM_UI));
			}
		}

		public event Action actUpdateCvsFaceAll;

		public event Action actUpdateCvsEar;

		public event Action actUpdateCvsChin;

		public event Action actUpdateCvsCheek;

		public event Action actUpdateCvsEyebrow;

		public event Action actUpdateCvsEye01;

		public event Action actUpdateCvsEye02;

		public event Action actUpdateCvsNose;

		public event Action actUpdateCvsMouth;

		public event Action actUpdateCvsMole;

		public event Action actUpdateCvsBaseMakeup;

		public event Action actUpdateCvsFaceShapeAll;

		public event Action actUpdateCvsBodyAll;

		public event Action actUpdateCvsBreast;

		public event Action actUpdateCvsBodyUpper;

		public event Action actUpdateCvsBodyLower;

		public event Action actUpdateCvsArm;

		public event Action actUpdateCvsLeg;

		public event Action actUpdateCvsNail;

		public event Action actUpdateCvsUnderhair;

		public event Action actUpdateCvsSunburn;

		public event Action actUpdateCvsBodyPaint;

		public event Action actUpdateCvsBodyShapeAll;

		public event Action actUpdateCvsHairFront;

		public event Action actUpdateCvsHairBack;

		public event Action actUpdateCvsHairSide;

		public event Action actUpdateCvsHairExtension;

		public event Action actUpdateCvsHairEtc;

		public event Action actUpdateCvsCosTop;

		public event Action actUpdateCvsCosBot;

		public event Action actUpdateCvsCosBra;

		public event Action actUpdateCvsCosShorts;

		public event Action actUpdateCvsCosGloves;

		public event Action actUpdateCvsCosPanst;

		public event Action actUpdateCvsCosSocks;

		public event Action actUpdateCvsCosInnerShoes;

		public event Action actUpdateCvsCosOuterShoes;

		public event Action actUpdateCvsClothesCopy;

		public event Action actUpdateCvsAccessoryCopy;

		public event Action actUpdateCvsAccessoryChange;

		public event Action actUpdateCvsChara;

		public event Action actUpdateCvsCharaEx;

		public event Action actUpdateCvsH;

		public event Action actUpdateCvsQA;

		public event Action actUpdateCvsAttribute;

		public event Action actUpdateCvsADK;

		public event Action actUpdateCvsConfig;

		public bool GetUpdateCvsAccessory(int slotNo)
		{
			return _updateCvsAccessory[slotNo] != null && _updateCvsAccessory[slotNo].Value;
		}

		public void SetUpdateCvsAccessory(int slotNo, bool value)
		{
			if (_updateCvsAccessory[slotNo] != null)
			{
				_updateCvsAccessory[slotNo].Value = value;
			}
		}

		public bool IsInputFocused()
		{
			foreach (InputField item in lstInputField)
			{
				if (item.isFocused)
				{
					return true;
				}
			}
			foreach (TMP_InputField item2 in lstTmpInputField)
			{
				if (item2.isFocused)
				{
					return true;
				}
			}
			return false;
		}

		public static string ConvertTextFromRate(int min, int max, float value)
		{
			return ((int)Mathf.Lerp(min, max, value)/*cast due to .constrained prefix*/).ToString();
		}

		public static float ConvertRateFromText(int min, int max, string buf)
		{
			if (buf.IsNullOrEmpty())
			{
				return 0f;
			}
			int result;
			if (!int.TryParse(buf, out result))
			{
				return 0f;
			}
			return Mathf.InverseLerp(min, max, result);
		}

		public static float ConvertValueFromTextLimit(float min, float max, int digit, string buf)
		{
			if (buf.IsNullOrEmpty())
			{
				return 0f;
			}
			if (!MathfEx.RangeEqualOn(0, digit, 4))
			{
				return 0f;
			}
			float result = 0f;
			float.TryParse(buf, out result);
			string[] array = new string[5] { "f0", "f1", "f2", "f3", "f4" };
			result = float.Parse(result.ToString(array[digit]));
			return Mathf.Clamp(result, min, max);
		}

		public bool IsMaleCoordinateBra()
		{
			Dictionary<int, ListInfoBase> categoryInfo = Singleton<Character>.Instance.chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_bra);
			int[] array = (from n in categoryInfo.Where(delegate(KeyValuePair<int, ListInfoBase> n)
				{
					int infoInt = n.Value.GetInfoInt(ChaListDefine.KeyType.Sex);
					return (infoInt != 3 && infoInt != 99) ? true : false;
				})
				select n.Key).ToArray();
			return 0 != array.Length;
		}

		public void ChangeClothesStateAuto(int stateNo)
		{
			autoClothesStateNo = (byte)stateNo;
			if (autoClothesState)
			{
				ChangeClothesState(0);
			}
		}

		public void ChangeClothesState(int stateNo)
		{
			byte[,] array = new byte[3, 9]
			{
				{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
				{ 3, 3, 0, 0, 0, 0, 0, 3, 3 },
				{ 3, 3, 3, 3, 3, 3, 3, 3, 3 }
			};
			if (stateNo == 0)
			{
				autoClothesState = true;
				clothesStateNo = autoClothesStateNo;
			}
			else
			{
				autoClothesState = false;
				clothesStateNo = stateNo - 1;
			}
			if ((bool)chaCtrl)
			{
				int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
				for (int i = 0; i < num; i++)
				{
					chaCtrl.SetClothesState(i, array[clothesStateNo, i]);
				}
			}
		}

		public void LoadAnimation(string assetBundleName, string assetName)
		{
			if (null == chaCtrl)
			{
				return;
			}
			chaCtrl.LoadAnimation(animeAssetBundleName, animeAssetName, string.Empty);
			if (motionIK != null)
			{
				TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(animeAssetBundleName, animeAssetName, false, string.Empty);
				if ((bool)textAsset)
				{
					motionIK.LoadData(textAsset);
				}
			}
		}

		public void PlayAnimation(string stateName, float pos)
		{
			if (!(null == chaCtrl))
			{
				animeStateName = stateName;
				if (0f > pos)
				{
					chaCtrl.AnimPlay(stateName);
				}
				else
				{
					chaCtrl.syncPlay(stateName, 0, pos);
				}
				chaCtrl.resetDynamicBoneAll = true;
			}
		}

		public void UpdateIKCalc()
		{
			if (motionIK != null)
			{
				motionIK.Calc(animeStateName);
			}
		}

		private void OnDestroy()
		{
			customSettingSave.Save();
		}

		protected override void Awake()
		{
			lstTmpInputField.Clear();
			lstInputField.Clear();
			customSettingSave.Load();
			for (int i = 0; i < _updateCvsAccessory.Length; i++)
			{
				_updateCvsAccessory[i] = new BoolReactiveProperty(false);
			}
		}

		private void Start()
		{
			_drawSaveFrameTop.Subscribe(delegate(bool draw)
			{
				if (saveFrameAssist != null)
				{
					saveFrameAssist.SetActiveSaveFrameTop(draw);
				}
			});
			_drawSaveFrameBack.Subscribe(delegate(bool draw)
			{
				if (saveFrameAssist != null)
				{
					saveFrameAssist.ShowSaveFrameBack(draw);
				}
			});
			_drawSaveFrameFront.Subscribe(delegate(bool draw)
			{
				if (saveFrameAssist != null)
				{
					saveFrameAssist.ShowSaveFrameFront(draw);
				}
			});
			_changeCharaName.Where((bool f) => f).Subscribe(delegate
			{
				customCtrl.UpdateCharaNameText();
				changeCharaName = false;
			});
			_updateCvsFaceAll.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsFaceAll.Call();
				updateCvsFaceAll = false;
			});
			_updateCvsEar.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsEar.Call();
				updateCvsEar = false;
			});
			_updateCvsChin.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsChin.Call();
				updateCvsChin = false;
			});
			_updateCvsCheek.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCheek.Call();
				updateCvsCheek = false;
			});
			_updateCvsEyebrow.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsEyebrow.Call();
				updateCvsEyebrow = false;
			});
			_updateCvsEye01.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsEye01.Call();
				updateCvsEye01 = false;
			});
			_updateCvsEye02.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsEye02.Call();
				updateCvsEye02 = false;
			});
			_updateCvsNose.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsNose.Call();
				updateCvsNose = false;
			});
			_updateCvsMouth.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsMouth.Call();
				updateCvsMouth = false;
			});
			_updateCvsMole.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsMole.Call();
				updateCvsMole = false;
			});
			_updateCvsBaseMakeup.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBaseMakeup.Call();
				updateCvsBaseMakeup = false;
			});
			_updateCvsFaceShapeAll.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsFaceShapeAll.Call();
				updateCvsFaceShapeAll = false;
			});
			_updateCvsBodyAll.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBodyAll.Call();
				updateCvsBodyAll = false;
			});
			_updateCvsBreast.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBreast.Call();
				updateCvsBreast = false;
			});
			_updateCvsBodyUpper.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBodyUpper.Call();
				updateCvsBodyUpper = false;
			});
			_updateCvsBodyLower.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBodyLower.Call();
				updateCvsBodyLower = false;
			});
			_updateCvsArm.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsArm.Call();
				updateCvsArm = false;
			});
			_updateCvsLeg.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsLeg.Call();
				updateCvsLeg = false;
			});
			_updateCvsNail.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsNail.Call();
				updateCvsNail = false;
			});
			_updateCvsUnderhair.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsUnderhair.Call();
				updateCvsUnderhair = false;
			});
			_updateCvsSunburn.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsSunburn.Call();
				updateCvsSunburn = false;
			});
			_updateCvsBodyPaint.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBodyPaint.Call();
				updateCvsBodyPaint = false;
			});
			_updateCvsBodyShapeAll.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsBodyShapeAll.Call();
				updateCvsBodyShapeAll = false;
			});
			_updateCvsHairFront.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsHairFront.Call();
				updateCvsHairFront = false;
			});
			_updateCvsHairBack.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsHairBack.Call();
				updateCvsHairBack = false;
			});
			_updateCvsHairSide.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsHairSide.Call();
				updateCvsHairSide = false;
			});
			_updateCvsHairExtension.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsHairExtension.Call();
				updateCvsHairExtension = false;
			});
			_updateCvsHairEtc.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsHairEtc.Call();
				updateCvsHairEtc = false;
			});
			_updateCvsCosTop.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosTop.Call();
				updateCvsCosTop = false;
			});
			_updateCvsCosBot.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosBot.Call();
				updateCvsCosBot = false;
			});
			_updateCvsCosBra.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosBra.Call();
				updateCvsCosBra = false;
			});
			_updateCvsCosShorts.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosShorts.Call();
				updateCvsCosShorts = false;
			});
			_updateCvsCosGloves.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosGloves.Call();
				updateCvsCosGloves = false;
			});
			_updateCvsCosPanst.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosPanst.Call();
				updateCvsCosPanst = false;
			});
			_updateCvsCosSocks.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosSocks.Call();
				updateCvsCosSocks = false;
			});
			_updateCvsCosInnerShoes.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosInnerShoes.Call();
				updateCvsCosInnerShoes = false;
			});
			_updateCvsCosOuterShoes.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCosOuterShoes.Call();
				updateCvsCosOuterShoes = false;
			});
			_updateCvsClothesCopy.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsClothesCopy.Call();
				updateCvsClothesCopy = false;
			});
			_updateCvsAccessory.Select((BoolReactiveProperty p, int idx) => new
			{
				flag = p,
				index = idx
			}).ToList().ForEach(p =>
			{
				p.flag.Where((bool f) => f).Subscribe(delegate
				{
					if (p.index == selectSlot)
					{
						actUpdateCvsAccessory[p.index].Call();
					}
					actUpdateAcsSlotName[p.index].Call();
					_updateCvsAccessory[p.index].Value = false;
				});
			});
			_updateCvsAccessoryCopy.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsAccessoryCopy.Call();
				updateCvsAccessoryCopy = false;
			});
			_updateCvsAccessoryChange.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsAccessoryChange.Call();
				updateCvsAccessoryChange = false;
			});
			_updateCvsChara.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsChara.Call();
				updateCvsChara = false;
			});
			_updateCvsCharaEx.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsCharaEx.Call();
				updateCvsCharaEx = false;
			});
			_updateCvsH.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsH.Call();
				updateCvsH = false;
			});
			_updateCvsQA.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsQA.Call();
				updateCvsQA = false;
			});
			_updateCvsAttribute.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsAttribute.Call();
				updateCvsAttribute = false;
			});
			_updateCvsADK.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsADK.Call();
				updateCvsADK = false;
			});
			_updateCvsConfig.Where((bool f) => f).Subscribe(delegate
			{
				this.actUpdateCvsConfig.Call();
				updateCvsConfig = false;
			});
		}

		public string TranslateSetPositionTitle(int id)
		{
			return uiTranslater.Get(999).SafeGetText(id);
		}

		public string TranslateColorWindowTitle(CvsColor.ConnectColorKind kind)
		{
			return uiTranslater.Get(997).SafeGetText((int)kind);
		}

		public string TranslateSlotTitle(int id)
		{
			return translateSlotTitle.SafeGetText(id);
		}

		public void FontBind(TMP_Text text)
		{
			text.GetOrAddComponent<UIFontBinder>();
		}

		public void FontBind(Text text)
		{
			text.GetOrAddComponent<UIFontBinder>();
		}

		public string TranslateSlotTitleWithColor(int id)
		{
			return TranslateSlotTitleColors().SafeGet(id);
		}

		private string[] TranslateSlotTitleColors()
		{
			return translateSlotTitle.Values.ToArray("Colors");
		}
	}
}
