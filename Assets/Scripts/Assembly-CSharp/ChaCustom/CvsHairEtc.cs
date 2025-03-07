using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsHairEtc : MonoBehaviour
	{
		[SerializeField]
		private Toggle tglGlossKind;

		[SerializeField]
		private Image imgGlossKind;

		[SerializeField]
		private TextMeshProUGUI textGlossKind;

		[SerializeField]
		private CustomSelectKind customGloss;

		[SerializeField]
		private CanvasGroup cgGlossWin;

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileHair hair
		{
			get
			{
				return chaCtrl.chaFile.custom.hair;
			}
		}

		public virtual void UpdateCustomUI()
		{
			if (null != customGloss)
			{
				customGloss.UpdateCustomUI();
			}
		}

		public void UpdateSelectGloss(string name, Sprite sp, int index)
		{
			if ((bool)textGlossKind)
			{
				textGlossKind.text = name;
			}
			if ((bool)imgGlossKind)
			{
				imgGlossKind.sprite = sp;
			}
			if (hair.glossId != index)
			{
				hair.glossId = index;
				FuncChangeGlossMask();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncChangeGlossMask);
			}
		}

		public bool FuncChangeGlossMask()
		{
			chaCtrl.LoadHairGlossMask();
			chaCtrl.ChangeSettingHairGlossMaskAll();
			return true;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.actUpdateCvsHairEtc += UpdateCustomUI;
			tglGlossKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgGlossWin)
				{
					bool flag = ((cgGlossWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgGlossWin.Enable(isOn);
					}
				}
			});
		}
	}
}
