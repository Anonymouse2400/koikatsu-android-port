using System;
using System.Collections;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsHair : MonoBehaviour
	{
		public enum HairType
		{
			Back = 0,
			Front = 1,
			Side = 2,
			Extension = 3
		}

		public HairType typeHair;

		[SerializeField]
		private Toggle tglSameSetting;

		[SerializeField]
		private Toggle tglHairKind;

		[SerializeField]
		private Image imgHairKind;

		[SerializeField]
		private TextMeshProUGUI textHairKind;

		[SerializeField]
		private CustomSelectKind customHair;

		[SerializeField]
		private CanvasGroup cgHairWin;

		[SerializeField]
		private Slider sldHairLength;

		[SerializeField]
		private TMP_InputField inpHairLength;

		[SerializeField]
		private Button btnHairLength;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnBaseColor;

		[SerializeField]
		private Image imgBaseColor;

		[SerializeField]
		private Toggle tglAutoSetting;

		[SerializeField]
		private Button btnStartColor;

		[SerializeField]
		private Image imgStartColor;

		[SerializeField]
		private Button btnEndColor;

		[SerializeField]
		private Image imgEndColor;

		[SerializeField]
		private Toggle tglChangeOutline;

		[SerializeField]
		private Button btnOutlineColor;

		[SerializeField]
		private Image imgOutlineColor;

		[SerializeField]
		private GameObject objAcsColor01;

		[SerializeField]
		private Button btnAcsColor01;

		[SerializeField]
		private Image imgAcsColor01;

		[SerializeField]
		private GameObject objAcsColor02;

		[SerializeField]
		private Button btnAcsColor02;

		[SerializeField]
		private Image imgAcsColor02;

		[SerializeField]
		private GameObject objAcsColor03;

		[SerializeField]
		private Button btnAcsColor03;

		[SerializeField]
		private Image imgAcsColor03;

		[SerializeField]
		private GameObject objInitAcsColor;

		[SerializeField]
		private Button btnInitAcsColor;

		[SerializeField]
		private GameObject objAcsSeparate;

		[SerializeField]
		private Button btnReflectColor;

		[SerializeField]
		private Button btnEyebrowColor;

		[SerializeField]
		private Button btnUnderhairColor;

		private int hairType
		{
			get
			{
				return (int)typeHair;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileFace face
		{
			get
			{
				return chaCtrl.chaFile.custom.face;
			}
		}

		private ChaFileBody body
		{
			get
			{
				return chaCtrl.chaFile.custom.body;
			}
		}

		private ChaFileHair hair
		{
			get
			{
				return chaCtrl.chaFile.custom.hair;
			}
		}

		public void CalculateUI()
		{
			if ((bool)sldHairLength)
			{
				sldHairLength.value = hair.parts[hairType].length;
			}
			imgBaseColor.color = hair.parts[hairType].baseColor;
			imgStartColor.color = hair.parts[hairType].startColor;
			imgEndColor.color = hair.parts[hairType].endColor;
			imgAcsColor01.color = hair.parts[hairType].acsColor[0];
			imgAcsColor02.color = hair.parts[hairType].acsColor[1];
			imgAcsColor03.color = hair.parts[hairType].acsColor[2];
			imgOutlineColor.color = hair.parts[hairType].outlineColor;
			tglSameSetting.isOn = Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting;
			tglAutoSetting.isOn = !Singleton<CustomBase>.Instance.customSettingSave.hairAutoSetting;
			tglChangeOutline.isOn = !Singleton<CustomBase>.Instance.customSettingSave.hairOutlineSetting;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customHair)
			{
				customHair.UpdateCustomUI();
			}
			switch (typeHair)
			{
			case HairType.Front:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_Base))
				{
					cvsColor.SetColor(hair.parts[hairType].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_Start))
				{
					cvsColor.SetColor(hair.parts[hairType].startColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_End))
				{
					cvsColor.SetColor(hair.parts[hairType].endColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_Acs01))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[0]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_Acs02))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[1]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_Acs03))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[2]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairF_Outline))
				{
					cvsColor.SetColor(hair.parts[hairType].outlineColor);
				}
				break;
			case HairType.Back:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_Base))
				{
					cvsColor.SetColor(hair.parts[hairType].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_Start))
				{
					cvsColor.SetColor(hair.parts[hairType].startColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_End))
				{
					cvsColor.SetColor(hair.parts[hairType].endColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_Acs01))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[0]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_Acs02))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[1]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_Acs03))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[2]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairB_Outline))
				{
					cvsColor.SetColor(hair.parts[hairType].outlineColor);
				}
				break;
			case HairType.Side:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_Base))
				{
					cvsColor.SetColor(hair.parts[hairType].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_Start))
				{
					cvsColor.SetColor(hair.parts[hairType].startColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_End))
				{
					cvsColor.SetColor(hair.parts[hairType].endColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_Acs01))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[0]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_Acs02))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[1]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_Acs03))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[2]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairS_Outline))
				{
					cvsColor.SetColor(hair.parts[hairType].outlineColor);
				}
				break;
			case HairType.Extension:
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_Base))
				{
					cvsColor.SetColor(hair.parts[hairType].baseColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_Start))
				{
					cvsColor.SetColor(hair.parts[hairType].startColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_End))
				{
					cvsColor.SetColor(hair.parts[hairType].endColor);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_Acs01))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[0]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_Acs02))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[1]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_Acs03))
				{
					cvsColor.SetColor(hair.parts[hairType].acsColor[2]);
				}
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.HairO_Outline))
				{
					cvsColor.SetColor(hair.parts[hairType].outlineColor);
				}
				break;
			}
			ChangeHairAcsVisible();
		}

		public void UpdateAnotherHairTypeUI()
		{
			switch (typeHair)
			{
			case HairType.Back:
				Singleton<CustomBase>.Instance.updateCvsHairFront = true;
				Singleton<CustomBase>.Instance.updateCvsHairSide = true;
				Singleton<CustomBase>.Instance.updateCvsHairExtension = true;
				break;
			case HairType.Front:
				Singleton<CustomBase>.Instance.updateCvsHairBack = true;
				Singleton<CustomBase>.Instance.updateCvsHairSide = true;
				Singleton<CustomBase>.Instance.updateCvsHairExtension = true;
				break;
			case HairType.Side:
				Singleton<CustomBase>.Instance.updateCvsHairBack = true;
				Singleton<CustomBase>.Instance.updateCvsHairFront = true;
				Singleton<CustomBase>.Instance.updateCvsHairExtension = true;
				break;
			case HairType.Extension:
				Singleton<CustomBase>.Instance.updateCvsHairBack = true;
				Singleton<CustomBase>.Instance.updateCvsHairFront = true;
				Singleton<CustomBase>.Instance.updateCvsHairSide = true;
				break;
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			if ((bool)inpHairLength)
			{
				inpHairLength.text = CustomBase.ConvertTextFromRate(0, 100, hair.parts[hairType].length);
			}
		}

		public void ChangeHairAcsVisible()
		{
			int hairAcsColorNum = chaCtrl.GetHairAcsColorNum(hairType);
			bool[,] array = new bool[4, 5]
			{
				{ false, false, false, false, false },
				{ true, false, false, true, true },
				{ true, true, false, true, true },
				{ true, true, true, true, true }
			};
			objAcsColor01.SetActiveIfDifferent(array[hairAcsColorNum, 0]);
			objAcsColor02.SetActiveIfDifferent(array[hairAcsColorNum, 1]);
			objAcsColor03.SetActiveIfDifferent(array[hairAcsColorNum, 2]);
			objInitAcsColor.SetActiveIfDifferent(array[hairAcsColorNum, 3]);
			objAcsSeparate.SetActiveIfDifferent(array[hairAcsColorNum, 4]);
		}

		public void UpdateSelectHair(string name, Sprite sp, int index)
		{
			if ((bool)textHairKind)
			{
				textHairKind.text = name;
			}
			if ((bool)imgHairKind)
			{
				imgHairKind.sprite = sp;
			}
			if (hair.parts[hairType].id != index)
			{
				hair.parts[hairType].id = index;
				switch (typeHair)
				{
				case HairType.Front:
					chaCtrl.ChangeHairFront(true);
					Singleton<CustomHistory>.Instance.Add2(chaCtrl, chaCtrl.ChangeHairFront, true);
					break;
				case HairType.Back:
					chaCtrl.ChangeHairBack(true);
					Singleton<CustomHistory>.Instance.Add2(chaCtrl, chaCtrl.ChangeHairBack, true);
					break;
				case HairType.Side:
					chaCtrl.ChangeHairSide(true);
					Singleton<CustomHistory>.Instance.Add2(chaCtrl, chaCtrl.ChangeHairSide, true);
					break;
				case HairType.Extension:
					chaCtrl.ChangeHairOption(true);
					Singleton<CustomHistory>.Instance.Add2(chaCtrl, chaCtrl.ChangeHairOption, true);
					break;
				}
			}
			ChangeHairAcsVisible();
		}

		public void UpdateHairBaseColor(Color color)
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairAutoSetting)
			{
				float H;
				float S;
				float V;
				Color.RGBToHSV(color, out H, out S, out V);
				Color color2 = Color.HSVToRGB(H, S, Mathf.Max(V - 0.3f, 0f));
				Color color3 = Color.HSVToRGB(H, S, Mathf.Min(V + 0.15f, 1f));
				if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
				{
					int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
					for (int i = 0; i < num; i++)
					{
						hair.parts[i].baseColor = color;
						hair.parts[i].startColor = color2;
						hair.parts[i].endColor = color3;
					}
					imgBaseColor.color = color;
					imgStartColor.color = color2;
					imgEndColor.color = color3;
					FuncUpdateAllHairAllColor();
				}
				else
				{
					hair.parts[hairType].baseColor = color;
					imgBaseColor.color = color;
					hair.parts[hairType].startColor = color2;
					imgStartColor.color = color2;
					hair.parts[hairType].endColor = color3;
					imgEndColor.color = color3;
					FuncUpdateHairAllColor();
				}
			}
			else if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				int num2 = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
				for (int j = 0; j < num2; j++)
				{
					hair.parts[j].baseColor = color;
				}
				imgBaseColor.color = color;
				FuncUpdateAllHairBaseColor();
			}
			else
			{
				hair.parts[hairType].baseColor = color;
				imgBaseColor.color = color;
				FuncUpdateHairBaseColor();
			}
			if (Singleton<CustomBase>.Instance.customSettingSave.hairOutlineSetting)
			{
				float H2;
				float S2;
				float V2;
				Color.RGBToHSV(color, out H2, out S2, out V2);
				Color color4 = Color.HSVToRGB(H2, S2, Mathf.Max(V2 - 0.4f, 0f));
				if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
				{
					int num3 = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
					for (int k = 0; k < num3; k++)
					{
						hair.parts[k].outlineColor = color4;
					}
					imgOutlineColor.color = color4;
					FuncUpdateAllHairOutlineColor();
				}
				else
				{
					hair.parts[hairType].outlineColor = color4;
					imgOutlineColor.color = color4;
					FuncUpdateHairOutlineColor();
				}
			}
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				UpdateAnotherHairTypeUI();
			}
		}

		public bool FuncUpdateHairBaseColor()
		{
			chaCtrl.ChangeSettingHairColor(hairType, true, false, false);
			return true;
		}

		public bool FuncUpdateAllHairBaseColor()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, true, false, false);
			}
			return true;
		}

		public bool FuncUpdateHairBaseColorAndOutline()
		{
			chaCtrl.ChangeSettingHairColor(hairType, true, false, false);
			chaCtrl.ChangeSettingHairOutlineColor(hairType);
			return true;
		}

		public bool FuncUpdateAllHairBaseColorAndOutline()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, true, false, false);
				chaCtrl.ChangeSettingHairOutlineColor(i);
			}
			return true;
		}

		public bool FuncUpdateHairAllColor()
		{
			chaCtrl.ChangeSettingHairColor(hairType, true, true, true);
			HairAcsUpdate();
			return true;
		}

		public bool FuncUpdateAllHairAllColor()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, true, true, true);
			}
			HairAcsUpdate();
			return true;
		}

		public bool FuncUpdateHairAllColorAndOutline()
		{
			chaCtrl.ChangeSettingHairColor(hairType, true, true, true);
			chaCtrl.ChangeSettingHairOutlineColor(hairType);
			HairAcsUpdate();
			return true;
		}

		public bool FuncUpdateAllHairAllColorAndOutline()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, true, true, true);
				chaCtrl.ChangeSettingHairOutlineColor(i);
			}
			HairAcsUpdate();
			return true;
		}

		public void HairAcsUpdate()
		{
			foreach (var item in chaCtrl.cusAcsCmp.Select((ChaAccessoryComponent val, int idx) => new { val, idx }))
			{
				if (!(null == item.val) && item.val.rendHair != null && item.val.rendHair.Length != 0)
				{
					chaCtrl.ChangeAccessoryColor(item.idx);
				}
			}
		}

		public void UpdateHairBaseColorHistory()
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairAutoSetting)
			{
				if (Singleton<CustomBase>.Instance.customSettingSave.hairOutlineSetting)
				{
					if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
					{
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairAllColorAndOutline);
					}
					else
					{
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairAllColorAndOutline);
					}
				}
				else if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairAllColor);
				}
				else
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairAllColor);
				}
			}
			else if (Singleton<CustomBase>.Instance.customSettingSave.hairOutlineSetting)
			{
				if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairBaseColorAndOutline);
				}
				else
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairBaseColorAndOutline);
				}
			}
			else if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairBaseColor);
			}
			else
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairBaseColor);
			}
		}

		public void UpdateHairStartColor(Color color)
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
				for (int i = 0; i < num; i++)
				{
					hair.parts[i].startColor = color;
				}
				imgStartColor.color = color;
				FuncUpdateAllHairStartColor();
				UpdateAnotherHairTypeUI();
			}
			else
			{
				hair.parts[hairType].startColor = color;
				imgStartColor.color = color;
				FuncUpdateHairStartColor();
			}
		}

		public bool FuncUpdateHairStartColor()
		{
			chaCtrl.ChangeSettingHairColor(hairType, false, true, false);
			foreach (var item in chaCtrl.cusAcsCmp.Select((ChaAccessoryComponent val, int idx) => new { val, idx }))
			{
				if (!(null == item.val) && item.val.rendHair != null && item.val.rendHair.Length != 0)
				{
					chaCtrl.ChangeAccessoryColor(item.idx);
				}
			}
			return true;
		}

		public bool FuncUpdateAllHairStartColor()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, false, true, false);
			}
			foreach (var item in chaCtrl.cusAcsCmp.Select((ChaAccessoryComponent val, int idx) => new { val, idx }))
			{
				if (!(null == item.val) && item.val.rendHair != null && item.val.rendHair.Length != 0)
				{
					chaCtrl.ChangeAccessoryColor(item.idx);
				}
			}
			return true;
		}

		public void UpdateHairStartColorHistory()
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairStartColor);
			}
			else
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairStartColor);
			}
		}

		public void UpdateHairEndColor(Color color)
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
				for (int i = 0; i < num; i++)
				{
					hair.parts[i].endColor = color;
				}
				imgEndColor.color = color;
				FuncUpdateAllHairEndColor();
				UpdateAnotherHairTypeUI();
			}
			else
			{
				hair.parts[hairType].endColor = color;
				imgEndColor.color = color;
				FuncUpdateHairEndColor();
			}
		}

		public bool FuncUpdateHairEndColor()
		{
			chaCtrl.ChangeSettingHairColor(hairType, false, false, true);
			return true;
		}

		public bool FuncUpdateAllHairEndColor()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, false, false, true);
			}
			return true;
		}

		public void UpdateHairEndColorHistory()
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairEndColor);
			}
			else
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairEndColor);
			}
		}

		public void UpdateHairAcs01Color(Color color)
		{
			hair.parts[hairType].acsColor[0] = color;
			imgAcsColor01.color = color;
			FuncUpdateHairAcsColor();
		}

		public void UpdateHairAcs02Color(Color color)
		{
			hair.parts[hairType].acsColor[1] = color;
			imgAcsColor02.color = color;
			FuncUpdateHairAcsColor();
		}

		public void UpdateHairAcs03Color(Color color)
		{
			hair.parts[hairType].acsColor[2] = color;
			imgAcsColor03.color = color;
			FuncUpdateHairAcsColor();
		}

		public bool FuncUpdateHairAcsColor()
		{
			chaCtrl.ChangeSettingHairAcsColor(hairType);
			return true;
		}

		public void UpdateHairAcsColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairAcsColor);
		}

		public void UpdateHairOutlineColor(Color color)
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
				for (int i = 0; i < num; i++)
				{
					hair.parts[i].outlineColor = color;
				}
				imgOutlineColor.color = color;
				FuncUpdateAllHairOutlineColor();
				UpdateAnotherHairTypeUI();
			}
			else
			{
				hair.parts[hairType].outlineColor = color;
				imgOutlineColor.color = color;
				FuncUpdateHairOutlineColor();
			}
		}

		public bool FuncUpdateHairOutlineColor()
		{
			chaCtrl.ChangeSettingHairOutlineColor(hairType);
			return true;
		}

		public bool FuncUpdateAllHairOutlineColor()
		{
			int num = Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length;
			for (int i = 0; i < num; i++)
			{
				chaCtrl.ChangeSettingHairOutlineColor(i);
			}
			return true;
		}

		public void UpdateHairOutlineColorHistory()
		{
			if (Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting)
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairOutlineColor);
			}
			else
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairOutlineColor);
			}
		}

		public bool UpdateEyebrowAndUnderhair()
		{
			chaCtrl.ChangeSettingEyebrowColor();
			chaCtrl.ChangeSettingUnderhairColor();
			return true;
		}

		public void SetDefaultColorWindow(int no)
		{
			CvsColor.ConnectColorKind kind = CvsColor.ConnectColorKind.HairF_Base;
			string winBaseTitle = string.Empty;
			switch ((HairType)no)
			{
			case HairType.Front:
				kind = CvsColor.ConnectColorKind.HairF_Base;
				winBaseTitle = "前髪の基本の色";
				break;
			case HairType.Back:
				kind = CvsColor.ConnectColorKind.HairB_Base;
				winBaseTitle = "後ろ髪の基本の色";
				break;
			case HairType.Side:
				kind = CvsColor.ConnectColorKind.HairS_Base;
				winBaseTitle = "横髪の基本の色";
				break;
			case HairType.Extension:
				kind = CvsColor.ConnectColorKind.HairO_Base;
				winBaseTitle = "エクステの基本の色";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(kind).SafeProc(delegate(string text)
			{
				winBaseTitle = text;
			});
			cvsColor.Setup(winBaseTitle, kind, hair.parts[no].baseColor, UpdateHairBaseColor, UpdateHairBaseColorHistory, false);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			if ((bool)inpHairLength)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpHairLength);
			}
			switch (typeHair)
			{
			case HairType.Back:
				Singleton<CustomBase>.Instance.actUpdateCvsHairBack += UpdateCustomUI;
				break;
			case HairType.Front:
				Singleton<CustomBase>.Instance.actUpdateCvsHairFront += UpdateCustomUI;
				break;
			case HairType.Side:
				Singleton<CustomBase>.Instance.actUpdateCvsHairSide += UpdateCustomUI;
				break;
			case HairType.Extension:
				Singleton<CustomBase>.Instance.actUpdateCvsHairExtension += UpdateCustomUI;
				break;
			}
			tglSameSetting.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				Singleton<CustomBase>.Instance.customSettingSave.hairSameSetting = isOn;
				if (!isOn)
				{
				}
			});
			tglHairKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgHairWin)
				{
					bool flag = ((cgHairWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgHairWin.Enable(isOn);
					}
				}
			});
			if (typeHair == HairType.Front)
			{
				sldHairLength.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					hair.parts[hairType].length = value;
					chaCtrl.ChangeSettingHairFrontLength();
					inpHairLength.text = CustomBase.ConvertTextFromRate(0, 100, value);
				});
				sldHairLength.OnPointerUpAsObservable().Subscribe(delegate
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingHairFrontLength);
				});
				sldHairLength.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					sldHairLength.value = Mathf.Clamp(sldHairLength.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
					inpHairLength.text = CustomBase.ConvertTextFromRate(0, 100, sldHairLength.value);
				});
				inpHairLength.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					sldHairLength.value = CustomBase.ConvertRateFromText(0, 100, value);
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingHairFrontLength);
				});
				btnHairLength.onClick.AsObservable().Subscribe(delegate
				{
					float length = Singleton<CustomBase>.Instance.defChaInfo.custom.hair.parts[hairType].length;
					hair.parts[hairType].length = length;
					inpHairLength.text = CustomBase.ConvertTextFromRate(0, 100, length);
					sldHairLength.value = length;
					chaCtrl.ChangeSettingHairFrontLength();
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingHairFrontLength);
				});
			}
			CvsColor.ConnectColorKind colorBaseKind = CvsColor.ConnectColorKind.HairF_Base;
			string winBaseTitle = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorBaseKind = CvsColor.ConnectColorKind.HairF_Base;
				winBaseTitle = "前髪の基本の色";
				break;
			case HairType.Back:
				colorBaseKind = CvsColor.ConnectColorKind.HairB_Base;
				winBaseTitle = "後ろ髪の基本の色";
				break;
			case HairType.Side:
				colorBaseKind = CvsColor.ConnectColorKind.HairS_Base;
				winBaseTitle = "横髪の基本の色";
				break;
			case HairType.Extension:
				colorBaseKind = CvsColor.ConnectColorKind.HairO_Base;
				winBaseTitle = "エクステの基本の色";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorBaseKind).SafeProc(delegate(string text)
			{
				winBaseTitle = text;
			});
			btnBaseColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorBaseKind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winBaseTitle, colorBaseKind, hair.parts[hairType].baseColor, UpdateHairBaseColor, UpdateHairBaseColorHistory, false);
				}
			});
			tglAutoSetting.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				Singleton<CustomBase>.Instance.customSettingSave.hairAutoSetting = !isOn;
				btnStartColor.transform.parent.gameObject.SetActiveIfDifferent(isOn);
				btnEndColor.transform.parent.gameObject.SetActiveIfDifferent(isOn);
				if (!isOn && (cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairF_Start || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairB_Start || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairS_Start || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairO_Start || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairF_End || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairB_End || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairS_End || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairO_End))
				{
					cvsColor.Close();
				}
			});
			CvsColor.ConnectColorKind colorStartKind = CvsColor.ConnectColorKind.HairF_Start;
			string winStartTitle = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorStartKind = CvsColor.ConnectColorKind.HairF_Start;
				winStartTitle = "前髪の根本の色";
				break;
			case HairType.Back:
				colorStartKind = CvsColor.ConnectColorKind.HairB_Start;
				winStartTitle = "後ろ髪の根本の色";
				break;
			case HairType.Side:
				colorStartKind = CvsColor.ConnectColorKind.HairS_Start;
				winStartTitle = "横髪の根本の色";
				break;
			case HairType.Extension:
				colorStartKind = CvsColor.ConnectColorKind.HairO_Start;
				winStartTitle = "エクステの根本の色";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorStartKind).SafeProc(delegate(string text)
			{
				winStartTitle = text;
			});
			btnStartColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorStartKind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winStartTitle, colorStartKind, hair.parts[hairType].startColor, UpdateHairStartColor, UpdateHairStartColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorEndKind = CvsColor.ConnectColorKind.HairF_End;
			string winEndTitle = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorEndKind = CvsColor.ConnectColorKind.HairF_End;
				winEndTitle = "前髪の毛先の色";
				break;
			case HairType.Back:
				colorEndKind = CvsColor.ConnectColorKind.HairB_End;
				winEndTitle = "後ろ髪の毛先の色";
				break;
			case HairType.Side:
				colorEndKind = CvsColor.ConnectColorKind.HairS_End;
				winEndTitle = "横髪の毛先の色";
				break;
			case HairType.Extension:
				colorEndKind = CvsColor.ConnectColorKind.HairO_End;
				winEndTitle = "エクステの毛先の色";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorEndKind).SafeProc(delegate(string text)
			{
				winEndTitle = text;
			});
			btnEndColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorEndKind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winEndTitle, colorEndKind, hair.parts[hairType].endColor, UpdateHairEndColor, UpdateHairEndColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorAcs01Kind = CvsColor.ConnectColorKind.HairF_Acs01;
			string winAcs01Title = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorAcs01Kind = CvsColor.ConnectColorKind.HairF_Acs01;
				winAcs01Title = "前髪の装飾の色０１";
				break;
			case HairType.Back:
				colorAcs01Kind = CvsColor.ConnectColorKind.HairB_Acs01;
				winAcs01Title = "後ろ髪の装飾の色０１";
				break;
			case HairType.Side:
				colorAcs01Kind = CvsColor.ConnectColorKind.HairS_Acs01;
				winAcs01Title = "横髪の装飾の色０１";
				break;
			case HairType.Extension:
				colorAcs01Kind = CvsColor.ConnectColorKind.HairO_Acs01;
				winAcs01Title = "エクステの装飾の色０１";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorAcs01Kind).SafeProc(delegate(string text)
			{
				winAcs01Title = text;
			});
			btnAcsColor01.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorAcs01Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winAcs01Title, colorAcs01Kind, hair.parts[hairType].acsColor[0], UpdateHairAcs01Color, UpdateHairAcsColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorAcs02Kind = CvsColor.ConnectColorKind.HairF_Acs02;
			string winAcs02Title = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorAcs02Kind = CvsColor.ConnectColorKind.HairF_Acs02;
				winAcs02Title = "前髪の装飾の色０２";
				break;
			case HairType.Back:
				colorAcs02Kind = CvsColor.ConnectColorKind.HairB_Acs02;
				winAcs02Title = "後ろ髪の装飾の色０２";
				break;
			case HairType.Side:
				colorAcs02Kind = CvsColor.ConnectColorKind.HairS_Acs02;
				winAcs02Title = "横髪の装飾の色０２";
				break;
			case HairType.Extension:
				colorAcs02Kind = CvsColor.ConnectColorKind.HairO_Acs02;
				winAcs02Title = "エクステの装飾の色０２";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorAcs02Kind).SafeProc(delegate(string text)
			{
				winAcs02Title = text;
			});
			btnAcsColor02.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorAcs02Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winAcs02Title, colorAcs02Kind, hair.parts[hairType].acsColor[1], UpdateHairAcs02Color, UpdateHairAcsColorHistory, false);
				}
			});
			CvsColor.ConnectColorKind colorAcs03Kind = CvsColor.ConnectColorKind.HairF_Acs03;
			string winAcs03Title = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorAcs03Kind = CvsColor.ConnectColorKind.HairF_Acs03;
				winAcs03Title = "前髪の装飾の色０３";
				break;
			case HairType.Back:
				colorAcs03Kind = CvsColor.ConnectColorKind.HairB_Acs03;
				winAcs03Title = "後ろ髪の装飾の色０３";
				break;
			case HairType.Side:
				colorAcs03Kind = CvsColor.ConnectColorKind.HairS_Acs03;
				winAcs03Title = "横髪の装飾の色０３";
				break;
			case HairType.Extension:
				colorAcs03Kind = CvsColor.ConnectColorKind.HairO_Acs03;
				winAcs03Title = "エクステの装飾の色０３";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorAcs03Kind).SafeProc(delegate(string text)
			{
				winAcs03Title = text;
			});
			btnAcsColor03.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorAcs03Kind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winAcs03Title, colorAcs03Kind, hair.parts[hairType].acsColor[2], UpdateHairAcs03Color, UpdateHairAcsColorHistory, false);
				}
			});
			btnInitAcsColor.onClick.AsObservable().Subscribe(delegate
			{
				chaCtrl.SetAcsDefaultColorParameterOnly(hairType);
				chaCtrl.ChangeSettingHairAcsColor(hairType);
				UpdateCustomUI();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateHairAcsColor);
			});
			CvsColor.ConnectColorKind colorOutlineKind = CvsColor.ConnectColorKind.HairF_Outline;
			string winOutlineTitle = string.Empty;
			switch (typeHair)
			{
			case HairType.Front:
				colorOutlineKind = CvsColor.ConnectColorKind.HairF_Outline;
				winOutlineTitle = "前髪のアウトラインの色";
				break;
			case HairType.Back:
				colorOutlineKind = CvsColor.ConnectColorKind.HairB_Outline;
				winOutlineTitle = "後ろ髪のアウトラインの色";
				break;
			case HairType.Side:
				colorOutlineKind = CvsColor.ConnectColorKind.HairS_Outline;
				winOutlineTitle = "横髪のアウトラインの色";
				break;
			case HairType.Extension:
				colorOutlineKind = CvsColor.ConnectColorKind.HairO_Outline;
				winOutlineTitle = "エクステのアウトラインの色";
				break;
			}
			Singleton<CustomBase>.Instance.TranslateColorWindowTitle(colorOutlineKind).SafeProc(delegate(string text)
			{
				winOutlineTitle = text;
			});
			tglChangeOutline.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				Singleton<CustomBase>.Instance.customSettingSave.hairOutlineSetting = !isOn;
				btnOutlineColor.transform.parent.gameObject.SetActiveIfDifferent(isOn);
				if (!isOn && (cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairF_Outline || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairB_Outline || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairS_Outline || cvsColor.connectColorKind == CvsColor.ConnectColorKind.HairO_Outline))
				{
					cvsColor.Close();
				}
			});
			btnOutlineColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == colorOutlineKind)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(winOutlineTitle, colorOutlineKind, hair.parts[hairType].outlineColor, UpdateHairOutlineColor, UpdateHairOutlineColorHistory, false);
				}
			});
			btnReflectColor.OnClickAsObservable().Subscribe(delegate
			{
				Color baseColor = hair.parts[0].baseColor;
				face.eyebrowColor = baseColor;
				body.underhairColor = baseColor;
				UpdateEyebrowAndUnderhair();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, UpdateEyebrowAndUnderhair);
				Singleton<CustomBase>.Instance.updateCvsEyebrow = true;
				Singleton<CustomBase>.Instance.updateCvsUnderhair = true;
			});
			btnEyebrowColor.OnClickAsObservable().Subscribe(delegate
			{
				Color eyebrowColor = face.eyebrowColor;
				float H;
				float S;
				float V;
				Color.RGBToHSV(eyebrowColor, out H, out S, out V);
				Color startColor = Color.HSVToRGB(H, S, Mathf.Max(V - 0.2f, 0f));
				Color endColor = Color.HSVToRGB(H, S, Mathf.Min(V + 0.1f, 1f));
				ChaFileHair.PartsInfo[] parts = hair.parts;
				foreach (ChaFileHair.PartsInfo partsInfo in parts)
				{
					partsInfo.baseColor = eyebrowColor;
					partsInfo.startColor = startColor;
					partsInfo.endColor = endColor;
				}
				FuncUpdateAllHairAllColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairAllColor);
				UpdateCustomUI();
			});
			btnUnderhairColor.OnClickAsObservable().Subscribe(delegate
			{
				Color underhairColor = body.underhairColor;
				float H2;
				float S2;
				float V2;
				Color.RGBToHSV(underhairColor, out H2, out S2, out V2);
				Color startColor2 = Color.HSVToRGB(H2, S2, Mathf.Max(V2 - 0.2f, 0f));
				Color endColor2 = Color.HSVToRGB(H2, S2, Mathf.Min(V2 + 0.1f, 1f));
				ChaFileHair.PartsInfo[] parts2 = hair.parts;
				foreach (ChaFileHair.PartsInfo partsInfo2 in parts2)
				{
					partsInfo2.baseColor = underhairColor;
					partsInfo2.startColor = startColor2;
					partsInfo2.endColor = endColor2;
				}
				FuncUpdateAllHairAllColor();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateAllHairAllColor);
				UpdateCustomUI();
			});
			StartCoroutine(SetInputText());
		}
	}
}
