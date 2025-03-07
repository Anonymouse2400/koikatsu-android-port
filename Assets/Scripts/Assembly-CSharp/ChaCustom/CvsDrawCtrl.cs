using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Localize.Translate;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsDrawCtrl : MonoBehaviour
	{
		[SerializeField]
		private GameObject objLight;

		[SerializeField]
		private Camera backGroundCamera;

		[SerializeField]
		private GameObject cvsBackGroundImage;

		[SerializeField]
		private RawImage imgBackground;

		[SerializeField]
		private Toggle[] tglClothesState;

		private int capBackClothesStateNo;

		public Toggle[] tglShoesType;

		[SerializeField]
		private Toggle[] tglShowAccessory;

		[SerializeField]
		private TMP_Dropdown ddEyebrowPtn;

		[SerializeField]
		private Button[] btnEyebrowPtn;

		[SerializeField]
		private TMP_Dropdown ddEyesPtn;

		[SerializeField]
		private Button[] btnEyesPtn;

		[SerializeField]
		private Slider sldEyesOpen;

		[SerializeField]
		private Toggle tglBlink;

		[SerializeField]
		private TMP_Dropdown ddMouthPtn;

		[SerializeField]
		private Button[] btnMouthPtn;

		[SerializeField]
		private Slider sldMouthOpen;

		[SerializeField]
		private TMP_Dropdown ddEyesLook;

		[SerializeField]
		private Button[] btnEyesLookPtn;

		[SerializeField]
		private GameObject objEyesLookRate;

		[SerializeField]
		private Slider sldEyesLookRate;

		[SerializeField]
		private TMP_Dropdown ddNeckLook;

		[SerializeField]
		private Button[] btnNeckLookPtn;

		[SerializeField]
		private GameObject objNeckLookRate;

		[SerializeField]
		private Slider sldNeckLookRate;

		public TMP_Dropdown ddPose;

		[SerializeField]
		private Button[] btnPose;

		[SerializeField]
		private Slider sldLightingX;

		[SerializeField]
		private Slider sldLightingY;

		[SerializeField]
		private Button btnLightingInit;

		[SerializeField]
		private Toggle[] tglBackType;

		[SerializeField]
		private Button btnBackImageWin;

		[SerializeField]
		private Button[] btnBackImage;

		[SerializeField]
		private TextMeshProUGUI textBackImage;

		[SerializeField]
		private BackColorCtrl backColorCtrl;

		[SerializeField]
		private Button btnBackColor;

		[SerializeField]
		private Image imgBackColor;

		public GameObject objFrameTop;

		[SerializeField]
		private Toggle tglFrontFrame;

		[SerializeField]
		private Button btnFrontFrameWin;

		[SerializeField]
		private Button[] btnFrontFrame;

		[SerializeField]
		private TextMeshProUGUI textFrontFrame;

		[SerializeField]
		private Toggle tglBackFrame;

		[SerializeField]
		private Button btnBackFrameWin;

		[SerializeField]
		private Button[] btnBackFrame;

		[SerializeField]
		private TextMeshProUGUI textBackFrame;

		private Localize.Translate.Manager.FileInfo[] _faBackgrounds;

		private int backgroundNo;

		private Localize.Translate.Manager.FileInfo[] faBackgrounds
		{
			get
			{
				return this.GetCache(ref _faBackgrounds, delegate
				{
					string[] filters = new string[1] { "*.png" };
					Localize.Translate.Manager.FileInfo[] source = Localize.Translate.Manager.DefaultData.UserDataCommonAssist("bg", filters);
					IOrderedEnumerable<Localize.Translate.Manager.FileInfo> first = from p in source
						where p.isDefault
						orderby p.info.FileName
						select p;
					IOrderedEnumerable<Localize.Translate.Manager.FileInfo> second = from p in source
						where !p.isDefault
						orderby p.info.FileName
						select p;
					return first.Concat(second).ToArray();
				});
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

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			ChangeBackgroundNo(0);
			if (tglClothesState.Any())
			{
				(from item in tglClothesState.Select((Toggle tgl, int idx) => new { tgl, idx })
					where item.tgl != null
					select item).ToList().ForEach(item =>
				{
					CvsDrawCtrl cvsDrawCtrl = this;
					(from isOn in item.tgl.OnValueChangedAsObservable()
						where isOn
						select isOn).Subscribe(delegate
					{
						cvsDrawCtrl.customBase.ChangeClothesState(item.idx);
					});
				});
			}
			if (tglShoesType.Any())
			{
				(from item in tglShoesType.Select((Toggle tgl, int idx) => new
					{
						tgl = tgl,
						type = (byte)idx
					})
					where item.tgl != null
					select item).ToList().ForEach(item =>
				{
					CvsDrawCtrl cvsDrawCtrl2 = this;
					(from isOn in item.tgl.OnValueChangedAsObservable()
						where isOn
						select isOn).Subscribe(delegate
					{
						if ((bool)cvsDrawCtrl2.chaCtrl)
						{
							cvsDrawCtrl2.chaCtrl.fileStatus.shoesType = item.type;
						}
					});
				});
			}
			if (tglShowAccessory.Any())
			{
				(from item in tglShowAccessory.Select((Toggle tgl, int idx) => new { tgl, idx })
					where item.tgl != null
					select item).ToList().ForEach(item =>
				{
					CvsDrawCtrl cvsDrawCtrl3 = this;
					item.tgl.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
					{
						if ((bool)cvsDrawCtrl3.chaCtrl)
						{
							if (item.idx == 0)
							{
								cvsDrawCtrl3.chaCtrl.SetAccessoryStateCategory(0, isOn);
							}
							else
							{
								cvsDrawCtrl3.chaCtrl.SetAccessoryStateCategory(1, isOn);
							}
						}
					});
				});
			}
			List<ExcelData.Param> lstEyebrow = customBase.lstEyebrow;
			List<string> options = lstEyebrow.Select((ExcelData.Param x) => x.list[1]).ToList();
			List<int> lstIndex = lstEyebrow.Select((ExcelData.Param x) => int.Parse(x.list[0])).ToList();
			ddEyebrowPtn.ClearOptions();
			ddEyebrowPtn.AddOptions(options);
			ddEyebrowPtn.onValueChanged.AddListener(delegate(int idx)
			{
				chaCtrl.ChangeEyebrowPtn(lstIndex[idx]);
			});
			btnEyebrowPtn[0].OnClickAsObservable().Subscribe(delegate
			{
				int value2 = ddEyebrowPtn.value;
				int count = lstIndex.Count;
				value2 = (value2 + count - 1) % count;
				ddEyebrowPtn.value = value2;
			});
			btnEyebrowPtn[1].OnClickAsObservable().Subscribe(delegate
			{
				int value3 = ddEyebrowPtn.value;
				int count2 = lstIndex.Count;
				value3 = (value3 + 1) % count2;
				ddEyebrowPtn.value = value3;
			});
			List<ExcelData.Param> lstEye = customBase.lstEye;
			List<string> options2 = lstEye.Select((ExcelData.Param x) => x.list[1]).ToList();
			List<int> lstIndex2 = lstEye.Select((ExcelData.Param x) => int.Parse(x.list[0])).ToList();
			ddEyesPtn.ClearOptions();
			ddEyesPtn.AddOptions(options2);
			ddEyesPtn.onValueChanged.AddListener(delegate(int idx)
			{
				chaCtrl.ChangeEyesPtn(lstIndex2[idx]);
			});
			btnEyesPtn[0].OnClickAsObservable().Subscribe(delegate
			{
				int value4 = ddEyesPtn.value;
				int count3 = lstIndex2.Count;
				value4 = (value4 + count3 - 1) % count3;
				ddEyesPtn.value = value4;
			});
			btnEyesPtn[1].OnClickAsObservable().Subscribe(delegate
			{
				int value5 = ddEyesPtn.value;
				int count4 = lstIndex2.Count;
				value5 = (value5 + 1) % count4;
				ddEyesPtn.value = value5;
			});
			tglBlink.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				chaCtrl.ChangeEyesBlinkFlag(!isOn);
			});
			sldEyesOpen.OnValueChangedAsObservable().Subscribe(delegate(float v)
			{
				chaCtrl.ChangeEyesOpenMax(v);
			});
			sldEyesOpen.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldEyesOpen.value = Mathf.Clamp(sldEyesOpen.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
			});
			List<ExcelData.Param> lstMouth = customBase.lstMouth;
			List<string> options3 = lstMouth.Select((ExcelData.Param x) => x.list[1]).ToList();
			List<int> lstIndex3 = lstMouth.Select((ExcelData.Param x) => int.Parse(x.list[0])).ToList();
			ddMouthPtn.ClearOptions();
			ddMouthPtn.AddOptions(options3);
			ddMouthPtn.onValueChanged.AddListener(delegate(int idx)
			{
				chaCtrl.ChangeMouthPtn(lstIndex3[idx]);
			});
			btnMouthPtn[0].OnClickAsObservable().Subscribe(delegate
			{
				int value6 = ddMouthPtn.value;
				int count5 = lstIndex3.Count;
				value6 = (value6 + count5 - 1) % count5;
				ddMouthPtn.value = value6;
			});
			btnMouthPtn[1].OnClickAsObservable().Subscribe(delegate
			{
				int value7 = ddMouthPtn.value;
				int count6 = lstIndex3.Count;
				value7 = (value7 + 1) % count6;
				ddMouthPtn.value = value7;
			});
			sldMouthOpen.OnValueChangedAsObservable().Subscribe(delegate(float v)
			{
				chaCtrl.fileStatus.mouthFixed = true;
				chaCtrl.ChangeMouthOpenMax(v);
			});
			sldMouthOpen.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldMouthOpen.value = Mathf.Clamp(sldMouthOpen.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
			});
			List<ExcelData.Param> lstEyesLook = customBase.lstEyesLook;
			List<string> options4 = lstEyesLook.Select((ExcelData.Param x) => x.list[1]).ToList();
			List<int> lstIndex4 = lstEyesLook.Select((ExcelData.Param x) => int.Parse(x.list[0])).ToList();
			ddEyesLook.ClearOptions();
			ddEyesLook.AddOptions(options4);
			int ptn = int.Parse(lstEyesLook[0].list[2]);
			int targetType = int.Parse(lstEyesLook[0].list[3]);
			bool active = 1 == int.Parse(lstEyesLook[0].list[4]);
			chaCtrl.ChangeLookEyesPtn(ptn);
			chaCtrl.ChangeLookEyesTarget(targetType, null, sldEyesLookRate.value);
			objEyesLookRate.SetActiveIfDifferent(active);
			ddEyesLook.onValueChanged.AddListener(delegate(int idx)
			{
				int ptn2 = int.Parse(lstEyesLook[idx].list[2]);
				int targetType2 = int.Parse(lstEyesLook[idx].list[3]);
				bool active2 = 1 == int.Parse(lstEyesLook[idx].list[4]);
				chaCtrl.ChangeLookEyesPtn(ptn2);
				chaCtrl.ChangeLookEyesTarget(targetType2, null, sldEyesLookRate.value);
				objEyesLookRate.SetActiveIfDifferent(active2);
			});
			btnEyesLookPtn[0].OnClickAsObservable().Subscribe(delegate
			{
				int value8 = ddEyesLook.value;
				int count7 = lstIndex4.Count;
				value8 = (value8 + count7 - 1) % count7;
				ddEyesLook.value = value8;
			});
			btnEyesLookPtn[1].OnClickAsObservable().Subscribe(delegate
			{
				int value9 = ddEyesLook.value;
				int count8 = lstIndex4.Count;
				value9 = (value9 + 1) % count8;
				ddEyesLook.value = value9;
			});
			sldEyesLookRate.OnValueChangedAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeLookEyesTarget(-1, null, sldEyesLookRate.value);
			});
			sldEyesLookRate.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldEyesLookRate.value = Mathf.Clamp(sldEyesLookRate.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
			});
			List<ExcelData.Param> lstNeckLook = customBase.lstNeckLook;
			List<string> options5 = lstNeckLook.Select((ExcelData.Param x) => x.list[1]).ToList();
			List<int> lstIndex5 = lstNeckLook.Select((ExcelData.Param x) => int.Parse(x.list[0])).ToList();
			ddNeckLook.ClearOptions();
			ddNeckLook.AddOptions(options5);
			int ptn3 = int.Parse(lstNeckLook[0].list[2]);
			int targetType3 = int.Parse(lstNeckLook[0].list[3]);
			bool active3 = 1 == int.Parse(lstNeckLook[0].list[4]);
			chaCtrl.ChangeLookNeckPtn(ptn3);
			chaCtrl.ChangeLookNeckTarget(targetType3, null, sldNeckLookRate.value);
			objNeckLookRate.SetActiveIfDifferent(active3);
			ddNeckLook.onValueChanged.AddListener(delegate(int idx)
			{
				int ptn4 = int.Parse(lstNeckLook[idx].list[2]);
				int targetType4 = int.Parse(lstNeckLook[idx].list[3]);
				bool active4 = 1 == int.Parse(lstNeckLook[idx].list[4]);
				chaCtrl.ChangeLookNeckPtn(ptn4);
				chaCtrl.ChangeLookNeckTarget(targetType4, null, sldNeckLookRate.value);
				objNeckLookRate.SetActiveIfDifferent(active4);
			});
			btnNeckLookPtn[0].OnClickAsObservable().Subscribe(delegate
			{
				int value10 = ddNeckLook.value;
				int count9 = lstIndex5.Count;
				value10 = (value10 + count9 - 1) % count9;
				ddNeckLook.value = value10;
			});
			btnNeckLookPtn[1].OnClickAsObservable().Subscribe(delegate
			{
				int value11 = ddNeckLook.value;
				int count10 = lstIndex5.Count;
				value11 = (value11 + 1) % count10;
				ddNeckLook.value = value11;
			});
			sldNeckLookRate.OnValueChangedAsObservable().Subscribe(delegate
			{
				chaCtrl.ChangeLookNeckTarget(-1, null, sldNeckLookRate.value);
			});
			sldNeckLookRate.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldNeckLookRate.value = Mathf.Clamp(sldNeckLookRate.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
			});
			List<string> options6 = customBase.lstPose.Select((ExcelData.Param x) => x.list[3]).ToList();
			List<int> lstIndex6 = customBase.lstPose.Select((ExcelData.Param x) => int.Parse(x.list[0])).ToList();
			ddPose.ClearOptions();
			ddPose.AddOptions(options6);
			ddPose.onValueChanged.AddListener(delegate(int idx)
			{
				if (customBase.animeAssetBundleName != customBase.lstPose[idx].list[1] || customBase.animeAssetName != customBase.lstPose[idx].list[2])
				{
					customBase.animeAssetBundleName = customBase.lstPose[idx].list[1];
					customBase.animeAssetName = customBase.lstPose[idx].list[2];
					customBase.LoadAnimation(customBase.animeAssetBundleName, customBase.animeAssetName);
				}
				customBase.PlayAnimation(customBase.lstPose[idx].list[4], -1f);
			});
			btnPose[0].OnClickAsObservable().Subscribe(delegate
			{
				int value12 = ddPose.value;
				int count11 = lstIndex6.Count;
				value12 = (value12 + count11 - 1) % count11;
				ddPose.value = value12;
			});
			btnPose[1].OnClickAsObservable().Subscribe(delegate
			{
				int value13 = ddPose.value;
				int count12 = lstIndex6.Count;
				value13 = (value13 + 1) % count12;
				ddPose.value = value13;
			});
			sldLightingX.OnValueChangedAsObservable().Subscribe(delegate(float value)
			{
				if ((bool)objLight)
				{
					objLight.transform.localEulerAngles = new Vector3(Mathf.Lerp(-89f, 89f, value), objLight.transform.localEulerAngles.y, 0f);
				}
			});
			sldLightingX.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldLightingX.value = Mathf.Clamp(sldLightingX.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
			});
			sldLightingY.OnValueChangedAsObservable().Subscribe(delegate(float value)
			{
				if ((bool)objLight)
				{
					objLight.transform.localEulerAngles = new Vector3(objLight.transform.localEulerAngles.x, Mathf.Lerp(-180f, 180f, value), 0f);
				}
			});
			sldLightingY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldLightingY.value = Mathf.Clamp(sldLightingY.value + scl.scrollDelta.y * customBase.sliderWheelSensitive, 0f, 100f);
			});
			btnLightingInit.OnClickAsObservable().Subscribe(delegate
			{
				sldLightingX.value = 0.5f;
				sldLightingY.value = 0.5f;
				if ((bool)objLight)
				{
					objLight.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				}
			});
			(from item in tglBackType.Select((Toggle tgl, int idx) => new { tgl, idx })
				where item.tgl != null
				select item).ToList().ForEach(item =>
			{
				CvsDrawCtrl cvsDrawCtrl4 = this;
				(from isOn in item.tgl.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					bool flag = 0 == item.idx;
					cvsDrawCtrl4.btnBackImageWin.transform.parent.gameObject.SetActiveIfDifferent(flag);
					cvsDrawCtrl4.btnBackColor.transform.parent.gameObject.SetActiveIfDifferent(!flag);
					cvsDrawCtrl4.cvsBackGroundImage.SetActiveIfDifferent(flag);
					if (flag && cvsDrawCtrl4.backColorCtrl.isOpen)
					{
						cvsDrawCtrl4.backColorCtrl.Close();
					}
				});
			});
			if ((bool)backGroundCamera)
			{
				backGroundCamera.backgroundColor = customBase.customSettingSave.backColor;
			}
			imgBackColor.color = customBase.customSettingSave.backColor;
			btnBackColor.OnClickAsObservable().Subscribe(delegate
			{
				if (backColorCtrl.isOpen)
				{
					backColorCtrl.Close();
				}
				else
				{
					backColorCtrl.Setup(customBase.customSettingSave.backColor, UpdateBackColor);
				}
			});
			btnBackImage[0].OnClickAsObservable().Subscribe(delegate
			{
				ChangeBackgroundImagePrev();
			});
			btnBackImage[1].OnClickAsObservable().Subscribe(delegate
			{
				ChangeBackgroundImageNext();
			});
			textBackFrame.text = customBase.saveFrameAssist.GetNowPositionStringBack();
			tglBackFrame.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				customBase.drawSaveFrameBack = isOn;
			});
			btnBackFrame[0].OnClickAsObservable().Subscribe(delegate
			{
				customBase.saveFrameAssist.ChangeSaveFrameBack(1);
				textBackFrame.text = customBase.saveFrameAssist.GetNowPositionStringBack();
			});
			btnBackFrame[1].OnClickAsObservable().Subscribe(delegate
			{
				customBase.saveFrameAssist.ChangeSaveFrameBack(0);
				textBackFrame.text = customBase.saveFrameAssist.GetNowPositionStringBack();
			});
			textFrontFrame.text = customBase.saveFrameAssist.GetNowPositionStringFront();
			tglFrontFrame.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				customBase.drawSaveFrameFront = isOn;
			});
			btnFrontFrame[0].OnClickAsObservable().Subscribe(delegate
			{
				customBase.saveFrameAssist.ChangeSaveFrameFront(1);
				textFrontFrame.text = customBase.saveFrameAssist.GetNowPositionStringFront();
			});
			btnFrontFrame[1].OnClickAsObservable().Subscribe(delegate
			{
				customBase.saveFrameAssist.ChangeSaveFrameFront(0);
				textFrontFrame.text = customBase.saveFrameAssist.GetNowPositionStringFront();
			});
			yield return null;
		}

		public void UpdateAccessoryDraw()
		{
			if (!(null == chaCtrl))
			{
				for (int i = 0; i < tglShowAccessory.Length; i++)
				{
					chaCtrl.SetAccessoryStateCategory(0, tglShowAccessory[0].isOn);
					chaCtrl.SetAccessoryStateCategory(1, tglShowAccessory[1].isOn);
				}
			}
		}

		public void ChangeClothesStateForCapture(bool capture)
		{
			if (capture)
			{
				capBackClothesStateNo = customBase.clothesStateNo;
				tglClothesState[1].isOn = false;
				tglClothesState[2].isOn = false;
				tglClothesState[0].isOn = true;
				return;
			}
			for (int i = 0; i < tglClothesState.Length; i++)
			{
				if (i != capBackClothesStateNo)
				{
					tglClothesState[i].isOn = false;
				}
			}
			tglClothesState[capBackClothesStateNo].isOn = true;
		}

		public void ChangePoseDefault()
		{
			ddPose.value = 0;
		}

		public void ChangeEyesPtnDefault()
		{
			ddEyesPtn.value = 0;
		}

		public void ChangeAnimationForce(int idx, float posTime)
		{
			if (customBase.animeAssetBundleName != customBase.lstPose[idx].list[1] || customBase.animeAssetName != customBase.lstPose[idx].list[2])
			{
				customBase.animeAssetBundleName = customBase.lstPose[idx].list[1];
				customBase.animeAssetName = customBase.lstPose[idx].list[2];
				customBase.LoadAnimation(customBase.animeAssetBundleName, customBase.animeAssetName);
			}
			customBase.PlayAnimation(customBase.lstPose[idx].list[4], posTime);
		}

		public void UpdateBackColor(Color color)
		{
			customBase.customSettingSave.backColor = color;
			imgBackColor.color = color;
			if ((bool)backGroundCamera)
			{
				backGroundCamera.backgroundColor = color;
			}
		}

		public void SetShoesType(int type)
		{
			if (tglShoesType != null && tglShoesType.Length == 2)
			{
				tglShoesType[0].isOn = 0 == type;
				tglShoesType[1].isOn = 1 == type;
			}
		}

		public byte GetShoesType()
		{
			if (tglShoesType.Any())
			{
				var anon = (from item in tglShoesType.Select((Toggle tgl, int idx) => new
					{
						tgl = tgl,
						type = (byte)idx
					})
					where null != item.tgl && item.tgl.isOn
					select item).FirstOrDefault();
				if (anon != null)
				{
					return anon.type;
				}
			}
			return 0;
		}

		public bool GetShowAccessory(int index)
		{
			if (tglShowAccessory == null || tglShowAccessory.Length != 2)
			{
				return true;
			}
			return tglShowAccessory[index].isOn;
		}

		public void ChangeBackgroundImagePrev()
		{
			int num = faBackgrounds.Length;
			if (num != 0)
			{
				backgroundNo = (backgroundNo + num - 1) % num;
				ChangeBackgroundNo(backgroundNo);
			}
		}

		public void ChangeBackgroundImageNext()
		{
			int num = faBackgrounds.Length;
			if (num != 0)
			{
				backgroundNo = (backgroundNo + 1) % num;
				ChangeBackgroundNo(backgroundNo);
			}
		}

		public void ChangeBackgroundNo(int index)
		{
			int num = faBackgrounds.Length;
			if (num == 0)
			{
				if (Localize.Translate.Manager.isTranslate)
				{
					textBackImage.text = "NoData";
				}
				else
				{
					textBackImage.text = "ファイルがありません";
				}
				return;
			}
			textBackImage.text = string.Format("{0:000} / {1:000}", index + 1, num);
			Texture2D texture = PngAssist.LoadTexture(faBackgrounds[index].info.FullPath);
			if ((bool)imgBackground)
			{
				if ((bool)imgBackground.texture)
				{
					Object.Destroy(imgBackground.texture);
				}
				imgBackground.texture = texture;
			}
		}
	}
}
