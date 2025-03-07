using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsUnderhair : MonoBehaviour
	{
		[SerializeField]
		private Toggle tglUnderhairKind;

		[SerializeField]
		private Image imgUnderhairKind;

		[SerializeField]
		private TextMeshProUGUI textUnderhairKind;

		[SerializeField]
		private CustomSelectKind customUnderhair;

		[SerializeField]
		private CanvasGroup cgUnderhairWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnUnderhairColor;

		[SerializeField]
		private Image imgUnderhairColor;

		[SerializeField]
		private Button btnReflectColor;

		[SerializeField]
		private Button btnEyebrowColor;

		[SerializeField]
		private Button btnHairColor;

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
			imgUnderhairColor.color = body.skinMainColor;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customUnderhair)
			{
				customUnderhair.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Underhair))
			{
				cvsColor.SetColor(body.underhairColor);
			}
		}

		public void UpdateSelectUnderhairKind(string name, Sprite sp, int index)
		{
			if ((bool)textUnderhairKind)
			{
				textUnderhairKind.text = name;
			}
			if ((bool)imgUnderhairKind)
			{
				imgUnderhairKind.sprite = sp;
			}
			if (body.underhairId != index)
			{
				body.underhairId = index;
				chaCtrl.ChangeSettingUnderhair();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingUnderhair);
			}
		}

		public bool UpdateEyebrowAndHair()
		{
			chaCtrl.ChangeSettingEyebrowColor();
			for (int i = 0; i < hair.parts.Length; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, true, true, true);
			}
			return true;
		}

		public void UpdateUnderhairColor(Color color)
		{
			body.underhairColor = color;
			imgUnderhairColor.color = color;
			chaCtrl.ChangeSettingUnderhairColor();
		}

		public void UpdateUnderhairColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingUnderhairColor);
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Underhair) ?? "陰毛の色", CvsColor.ConnectColorKind.Underhair, body.underhairColor, UpdateUnderhairColor, UpdateUnderhairColorHistory, true);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.actUpdateCvsUnderhair += UpdateCustomUI;
			tglUnderhairKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgUnderhairWin)
				{
					bool flag = ((cgUnderhairWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgUnderhairWin.Enable(isOn);
					}
				}
			});
			btnUnderhairColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Underhair)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			btnReflectColor.OnClickAsObservable().Subscribe(delegate
			{
				Color underhairColor = body.underhairColor;
				float H;
				float S;
				float V;
				Color.RGBToHSV(underhairColor, out H, out S, out V);
				Color startColor = Color.HSVToRGB(H, S, Mathf.Max(V - 0.2f, 0f));
				Color endColor = Color.HSVToRGB(H, S, Mathf.Min(V + 0.1f, 1f));
				ChaFileHair.PartsInfo[] parts = hair.parts;
				foreach (ChaFileHair.PartsInfo partsInfo in parts)
				{
					partsInfo.baseColor = underhairColor;
					partsInfo.startColor = startColor;
					partsInfo.endColor = endColor;
				}
				face.eyebrowColor = underhairColor;
				UpdateEyebrowAndHair();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, UpdateEyebrowAndHair);
				Singleton<CustomBase>.Instance.updateCvsEyebrow = true;
				Singleton<CustomBase>.Instance.updateCvsHairBack = true;
				Singleton<CustomBase>.Instance.updateCvsHairFront = true;
				Singleton<CustomBase>.Instance.updateCvsHairSide = true;
				Singleton<CustomBase>.Instance.updateCvsHairExtension = true;
			});
			btnEyebrowColor.OnClickAsObservable().Subscribe(delegate
			{
				Color eyebrowColor = face.eyebrowColor;
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Underhair))
				{
					cvsColor.SetColor(eyebrowColor);
				}
				UpdateUnderhairColor(eyebrowColor);
				UpdateUnderhairColorHistory();
			});
			btnHairColor.OnClickAsObservable().Subscribe(delegate
			{
				Color baseColor = hair.parts[0].baseColor;
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Underhair))
				{
					cvsColor.SetColor(baseColor);
				}
				UpdateUnderhairColor(baseColor);
				UpdateUnderhairColorHistory();
			});
		}
	}
}
