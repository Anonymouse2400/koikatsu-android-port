using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomConfig : MonoBehaviour
	{
		[SerializeField]
		private TMP_Dropdown ddRamp;

		[SerializeField]
		private Slider sldShadowD;

		[SerializeField]
		private TMP_InputField inpShadowD;

		[SerializeField]
		private Button btnShadowD;

		[SerializeField]
		private Slider sldOutlineD;

		[SerializeField]
		private TMP_InputField inpOutlineD;

		[SerializeField]
		private Button btnOutlineD;

		[SerializeField]
		private Slider sldOutlineW;

		[SerializeField]
		private TMP_InputField inpOutlineW;

		[SerializeField]
		private Button btnOutlineW;

		[SerializeField]
		private Slider sldBGM;

		[SerializeField]
		private TMP_InputField inpBGM;

		[SerializeField]
		private Button btnBGM;

		[SerializeField]
		private Slider sldSE;

		[SerializeField]
		private TMP_InputField inpSE;

		[SerializeField]
		private Button btnSE;

		public Toggle tglCenter;

		[SerializeField]
		private TextMeshProUGUI textMenuName;

		private List<int> lstRampId = new List<int>();

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		public void CalculateUI()
		{
			ddRamp.value = lstRampId.FindIndex((int find) => find == Manager.Config.EtcData.rampId);
			sldShadowD.value = 1f - Manager.Config.EtcData.shadowDepth;
			sldOutlineD.value = Manager.Config.EtcData.lineDepth;
			sldOutlineW.value = Manager.Config.EtcData.lineWidth;
			sldBGM.value = Mathf.InverseLerp(0f, 100f, Manager.Config.SoundData.BGM.Volume);
			sldSE.value = Mathf.InverseLerp(0f, 100f, Manager.Config.SoundData.SystemSE.Volume);
			tglCenter.isOn = Manager.Config.EtcData.Look;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			inpShadowD.text = CustomBase.ConvertTextFromRate(0, 100, 1f - Manager.Config.EtcData.shadowDepth);
			inpOutlineD.text = CustomBase.ConvertTextFromRate(0, 100, Manager.Config.EtcData.lineDepth);
			inpOutlineW.text = CustomBase.ConvertTextFromRate(0, 100, Manager.Config.EtcData.lineWidth);
			inpBGM.text = Manager.Config.SoundData.BGM.Volume.ToString();
			inpSE.text = Manager.Config.SoundData.SystemSE.Volume.ToString();
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			if ((bool)sldBGM)
			{
				sldBGM.transform.parent.gameObject.SetActiveIfDifferent(false);
			}
			if ((bool)sldSE)
			{
				sldSE.transform.parent.gameObject.SetActiveIfDifferent(false);
			}
			if ((bool)tglCenter)
			{
				tglCenter.transform.parent.gameObject.SetActiveIfDifferent(false);
			}
			Dictionary<int, ListInfoBase> dictInfo = chaCtrl.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_ramp);
			lstRampId = dictInfo.Select((KeyValuePair<int, ListInfoBase> dict) => dict.Value.Id).ToList();
			List<string> options = lstRampId.Select((int id) => dictInfo[id].Name).ToList();
			ddRamp.ClearOptions();
			ddRamp.AddOptions(options);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpShadowD);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpOutlineD);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpOutlineW);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpBGM);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpSE);
			Singleton<CustomBase>.Instance.actUpdateCvsConfig += UpdateCustomUI;
			ddRamp.onValueChanged.AddListener(delegate(int idx)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI)
				{
					Manager.Config.EtcData.rampId = lstRampId[idx];
				}
			});
			sldShadowD.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				Manager.Config.EtcData.shadowDepth = 1f - value;
				inpShadowD.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldShadowD.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldShadowD.value = Mathf.Clamp(sldShadowD.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpShadowD.text = CustomBase.ConvertTextFromRate(0, 100, sldShadowD.value);
			});
			inpShadowD.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldShadowD.value = CustomBase.ConvertRateFromText(0, 100, value);
			});
			btnShadowD.onClick.AsObservable().Subscribe(delegate
			{
				Manager.Config.EtcData.shadowDepth = 0.26f;
				inpShadowD.text = CustomBase.ConvertTextFromRate(0, 100, 1f - Manager.Config.EtcData.shadowDepth);
				sldShadowD.value = 1f - Manager.Config.EtcData.shadowDepth;
			});
			sldOutlineD.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				Manager.Config.EtcData.lineDepth = value;
				inpOutlineD.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldOutlineD.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldOutlineD.value = Mathf.Clamp(sldOutlineD.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpOutlineD.text = CustomBase.ConvertTextFromRate(0, 100, sldOutlineD.value);
			});
			inpOutlineD.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldOutlineD.value = CustomBase.ConvertRateFromText(0, 100, value);
			});
			btnOutlineD.onClick.AsObservable().Subscribe(delegate
			{
				Manager.Config.EtcData.lineDepth = 1f;
				inpOutlineD.text = CustomBase.ConvertTextFromRate(0, 100, Manager.Config.EtcData.lineDepth);
				sldOutlineD.value = Manager.Config.EtcData.lineDepth;
			});
			sldOutlineW.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				Manager.Config.EtcData.lineWidth = value;
				inpOutlineW.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldOutlineW.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldOutlineW.value = Mathf.Clamp(sldOutlineW.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpOutlineW.text = CustomBase.ConvertTextFromRate(0, 100, sldOutlineW.value);
			});
			inpOutlineW.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldOutlineW.value = CustomBase.ConvertRateFromText(0, 100, value);
			});
			btnOutlineW.onClick.AsObservable().Subscribe(delegate
			{
				Manager.Config.EtcData.lineWidth = 0.307f;
				inpOutlineW.text = CustomBase.ConvertTextFromRate(0, 100, Manager.Config.EtcData.lineWidth);
				sldOutlineW.value = Manager.Config.EtcData.lineWidth;
			});
			sldBGM.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				Manager.Config.SoundData.BGM.Volume = (int)Mathf.Lerp(0f, 100f, value);
				inpBGM.text = Manager.Config.SoundData.BGM.Volume.ToString();
			});
			sldBGM.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldBGM.value = Mathf.Clamp(sldBGM.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpBGM.text = CustomBase.ConvertTextFromRate(0, 100, sldBGM.value);
			});
			inpBGM.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldBGM.value = CustomBase.ConvertRateFromText(0, 100, value);
			});
			btnBGM.onClick.AsObservable().Subscribe(delegate
			{
				Manager.Config.SoundData.BGM.Volume = 20;
				inpBGM.text = "20";
				sldBGM.value = Mathf.InverseLerp(0f, 100f, 20f);
			});
			sldSE.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				Manager.Config.SoundData.SystemSE.Volume = (int)Mathf.Lerp(0f, 100f, value);
				inpSE.text = Manager.Config.SoundData.SystemSE.Volume.ToString();
			});
			sldSE.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldSE.value = Mathf.Clamp(sldSE.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpSE.text = CustomBase.ConvertTextFromRate(0, 100, sldSE.value);
			});
			inpSE.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldSE.value = CustomBase.ConvertRateFromText(0, 100, value);
			});
			btnSE.onClick.AsObservable().Subscribe(delegate
			{
				Manager.Config.SoundData.SystemSE.Volume = 30;
				inpSE.text = "30";
				sldSE.value = Mathf.InverseLerp(0f, 100f, 30f);
			});
			tglCenter.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI)
				{
					Manager.Config.EtcData.Look = isOn;
				}
			});
			StartCoroutine(SetInputText());
		}
	}
}
