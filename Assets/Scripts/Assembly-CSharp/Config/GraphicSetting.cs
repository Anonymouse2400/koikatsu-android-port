using System.Collections.Generic;
using System.Linq;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class GraphicSetting : BaseSetting
	{
		[Header("セルフシャドウ")]
		[SerializeField]
		private Toggle selfShadowToggle;

		[SerializeField]
		[Header("被写界深度")]
		private Toggle depthOfFieldToggle;

		[SerializeField]
		[Header("フォグ")]
		private Toggle fogToggle;

		[SerializeField]
		[Header("ブルーム")]
		private Toggle bloomToggle;

		[Header("ビグネット")]
		[SerializeField]
		private Toggle vignetteToggle;

		[SerializeField]
		[Header("サンシャフト")]
		private Toggle sunShaftsToggle;

		[Header("オクリュージョン")]
		[SerializeField]
		private Toggle amplifyOcclusToggle;

		[Header("軽い")]
		[SerializeField]
		private Button lowQualityButton;

		[Header("通常")]
		[SerializeField]
		private Button midQualityButton;

		[Header("綺麗")]
		[SerializeField]
		private Button highQualityButton;

		[SerializeField]
		[Header("ランプテクスチャID")]
		private TMP_Dropdown rampIDDropdown;

		[SerializeField]
		[Header("影の濃さ")]
		private Slider shadowDepthSlider;

		[Header("ラインの濃さ")]
		[SerializeField]
		private Slider lineDepthSlider;

		[Header("ライン幅")]
		[SerializeField]
		private Slider lineWidthSlider;

		private List<int> lstRampId = new List<int>();

		[SerializeField]
		private RawImage _rampRawImage;

		public RawImage rampRawImage
		{
			get
			{
				return _rampRawImage;
			}
		}

		public override void Init()
		{
			EtceteraSystem data = Manager.Config.EtcData;
			LinkToggle(selfShadowToggle, delegate(bool isOn)
			{
				int qualityLevel = QualitySettings.GetQualityLevel() / 2 * 2 + ((!isOn) ? 1 : 0);
				QualitySettings.SetQualityLevel(qualityLevel);
				data.SelfShadow = isOn;
			});
			LinkToggle(depthOfFieldToggle, delegate(bool isOn)
			{
				data.DepthOfField = isOn;
			});
			LinkToggle(fogToggle, delegate(bool isOn)
			{
				data.Fog = isOn;
			});
			LinkToggle(bloomToggle, delegate(bool isOn)
			{
				data.Bloom = isOn;
			});
			LinkToggle(vignetteToggle, delegate(bool isOn)
			{
				data.Vignette = isOn;
			});
			LinkToggle(sunShaftsToggle, delegate(bool isOn)
			{
				data.SunShafts = isOn;
			});
			LinkToggle(amplifyOcclusToggle, delegate(bool isOn)
			{
				data.AmplifyOcclus = isOn;
			});
			lowQualityButton.OnClickAsObservable().Subscribe(delegate
			{
				EnterSE();
				data.AmplifyOcclus = false;
				data.DepthOfField = false;
				data.Vignette = false;
				data.Fog = false;
				data.Bloom = false;
				data.SunShafts = false;
				data.SelfShadow = true;
				UIPresenter();
			});
			midQualityButton.OnClickAsObservable().Subscribe(delegate
			{
				EnterSE();
				data.AmplifyOcclus = false;
				data.DepthOfField = false;
				data.Vignette = true;
				data.Fog = true;
				data.Bloom = true;
				data.SunShafts = true;
				data.SelfShadow = true;
				UIPresenter();
			});
			highQualityButton.OnClickAsObservable().Subscribe(delegate
			{
				EnterSE();
				data.AmplifyOcclus = true;
				data.DepthOfField = true;
				data.Vignette = true;
				data.Fog = true;
				data.Bloom = true;
				data.SunShafts = true;
				data.SelfShadow = true;
				UIPresenter();
			});
			ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
			Dictionary<int, ListInfoBase> dictInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_ramp);
			lstRampId = dictInfo.Select((KeyValuePair<int, ListInfoBase> dict) => dict.Value.Id).ToList();
			List<string> options = lstRampId.Select((int id) => dictInfo[id].Name).ToList();
			rampIDDropdown.ClearOptions();
			rampIDDropdown.AddOptions(options);
			LinkTmpDropdown(rampIDDropdown, delegate(float value)
			{
				data.rampId = lstRampId[(int)value];
			});
			LinkSlider(shadowDepthSlider, delegate(float value)
			{
				data.shadowDepth = 1f - value;
			});
			LinkSlider(lineDepthSlider, delegate(float value)
			{
				data.lineDepth = value;
			});
			LinkSlider(lineWidthSlider, delegate(float value)
			{
				data.lineWidth = value;
			});
		}

		protected override void ValueToUI()
		{
			EtceteraSystem data = Manager.Config.EtcData;
			selfShadowToggle.isOn = data.SelfShadow;
			depthOfFieldToggle.isOn = data.DepthOfField;
			fogToggle.isOn = data.Fog;
			bloomToggle.isOn = data.Bloom;
			vignetteToggle.isOn = data.Vignette;
			sunShaftsToggle.isOn = data.SunShafts;
			amplifyOcclusToggle.isOn = data.AmplifyOcclus;
			rampIDDropdown.value = lstRampId.FindIndex((int find) => find == data.rampId);
			shadowDepthSlider.value = 1f - data.shadowDepth;
			lineDepthSlider.value = data.lineDepth;
			lineWidthSlider.value = data.lineWidth;
		}
	}
}
