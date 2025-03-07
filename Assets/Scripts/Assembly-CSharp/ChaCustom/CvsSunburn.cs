using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsSunburn : MonoBehaviour
	{
		[SerializeField]
		private Toggle tglSunburnKind;

		[SerializeField]
		private Image imgSunburnKind;

		[SerializeField]
		private TextMeshProUGUI textSunburnKind;

		[SerializeField]
		private CustomSelectKind customSunburn;

		[SerializeField]
		private CanvasGroup cgSunburnWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnSunburnColor;

		[SerializeField]
		private Image imgSunburnColor;

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
			imgSunburnColor.color = body.sunburnColor;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customSunburn)
			{
				customSunburn.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Sunburn))
			{
				cvsColor.SetColor(body.sunburnColor);
			}
		}

		public void UpdateSelectSunburnKind(string name, Sprite sp, int index)
		{
			if ((bool)textSunburnKind)
			{
				textSunburnKind.text = name;
			}
			if ((bool)imgSunburnKind)
			{
				imgSunburnKind.sprite = sp;
			}
			if (body.sunburnId != index)
			{
				body.sunburnId = index;
				FuncUpdateSunburnKind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSunburnKind);
			}
		}

		public void UpdateSunburnColor(Color color)
		{
			body.sunburnColor = color;
			imgSunburnColor.color = color;
			FuncUpdateSunburnColor();
		}

		public void UpdateSunburnColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSunburnColor);
		}

		public bool FuncUpdateSunburnKind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyTexFlags(false, false, false, false, true);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public bool FuncUpdateSunburnColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyColorFlags(false, false, false, false, true, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Sunburn) ?? "日焼け跡の色", CvsColor.ConnectColorKind.Sunburn, body.sunburnColor, UpdateSunburnColor, UpdateSunburnColorHistory, true);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.actUpdateCvsSunburn += UpdateCustomUI;
			tglSunburnKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgSunburnWin)
				{
					bool flag = ((cgSunburnWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgSunburnWin.Enable(isOn);
					}
				}
			});
			btnSunburnColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Sunburn)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
		}
	}
}
