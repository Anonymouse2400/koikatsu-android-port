using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsNail : MonoBehaviour
	{
		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnNailColor;

		[SerializeField]
		private Image imgNailColor;

		[SerializeField]
		private Slider sldGlossPow;

		[SerializeField]
		private TMP_InputField inpGlossPow;

		[SerializeField]
		private Button btnGlossPow;

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileBody body
		{
			get
			{
				return chaCtrl.chaFile.custom.body;
			}
		}

		public void CalculateUI()
		{
			imgNailColor.color = body.nailColor;
			sldGlossPow.value = body.nailGlossPower;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Nail))
			{
				cvsColor.SetColor(body.nailColor);
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, body.nailGlossPower);
		}

		public void UpdateNailColor(Color color)
		{
			body.nailColor = color;
			imgNailColor.color = color;
			FuncUpdateNailColor();
		}

		public void UpdateNailColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateNailColor);
		}

		public bool FuncUpdateNailColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyColorFlags(false, false, false, false, false, true);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Nail) ?? "爪の色", CvsColor.ConnectColorKind.Nail, body.nailColor, UpdateNailColor, UpdateNailColorHistory, false);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGlossPow);
			Singleton<CustomBase>.Instance.actUpdateCvsNail += UpdateCustomUI;
			btnNailColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Nail)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			sldGlossPow.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.nailGlossPower = value;
				chaCtrl.ChangeSettingNailGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldGlossPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNailGlossPower);
			});
			sldGlossPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGlossPow.value = Mathf.Clamp(sldGlossPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, sldGlossPow.value);
			});
			inpGlossPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGlossPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNailGlossPower);
			});
			btnGlossPow.onClick.AsObservable().Subscribe(delegate
			{
				float nailGlossPower = Singleton<CustomBase>.Instance.defChaInfo.custom.body.nailGlossPower;
				body.nailGlossPower = nailGlossPower;
				chaCtrl.ChangeSettingNailGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, nailGlossPower);
				sldGlossPow.value = nailGlossPower;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNailGlossPower);
			});
			StartCoroutine(SetInputText());
		}
	}
}
