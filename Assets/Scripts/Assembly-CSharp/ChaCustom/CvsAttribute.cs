using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsAttribute : MonoBehaviour
	{
		[SerializeField]
		private Toggle[] tglAttribute;

		[SerializeField]
		private Button btnRandom;

		[SerializeField]
		private Button btnAllOn;

		[SerializeField]
		private Button btnAllOff;

		private bool noUpdateHistory;

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

		private ChaFileParameter param
		{
			get
			{
				return chaCtrl.chaFile.parameter;
			}
		}

		public void CalculateUI()
		{
			bool[] attribute = chaCtrl.GetAttribute();
			for (int i = 0; i < tglAttribute.Length; i++)
			{
				tglAttribute[i].isOn = attribute[i];
			}
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		public void SetAttribute(int idx, bool flag)
		{
			switch (idx)
			{
			case 0:
				param.attribute.hinnyo = flag;
				break;
			case 1:
				param.attribute.harapeko = flag;
				break;
			case 2:
				param.attribute.donkan = flag;
				break;
			case 3:
				param.attribute.choroi = flag;
				break;
			case 4:
				param.attribute.bitch = flag;
				break;
			case 5:
				param.attribute.mutturi = flag;
				break;
			case 6:
				param.attribute.dokusyo = flag;
				break;
			case 7:
				param.attribute.ongaku = flag;
				break;
			case 8:
				param.attribute.kappatu = flag;
				break;
			case 9:
				param.attribute.ukemi = flag;
				break;
			case 10:
				param.attribute.friendly = flag;
				break;
			case 11:
				param.attribute.kireizuki = flag;
				break;
			case 12:
				param.attribute.taida = flag;
				break;
			case 13:
				param.attribute.sinsyutu = flag;
				break;
			case 14:
				param.attribute.hitori = flag;
				break;
			case 15:
				param.attribute.undo = flag;
				break;
			case 16:
				param.attribute.majime = flag;
				break;
			case 17:
				param.attribute.likeGirls = flag;
				break;
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			customBase.actUpdateCvsAttribute += UpdateCustomUI;
			tglAttribute.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!customBase.updateCustomUI)
					{
						bool attribute = chaCtrl.GetAttribute(p.index);
						if (attribute != isOn)
						{
							SetAttribute(p.index, isOn);
							if (!noUpdateHistory)
							{
								Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
							}
						}
					}
				});
			});
			btnRandom.OnClickAsObservable().Subscribe(delegate
			{
				bool[] attribute2 = chaCtrl.GetAttribute();
				noUpdateHistory = true;
				for (int i = 0; i < tglAttribute.Length; i++)
				{
					tglAttribute[i].isOn = ((Random.Range(0, 2) != 0) ? true : false);
				}
				noUpdateHistory = false;
				bool[] attribute3 = chaCtrl.GetAttribute();
				for (int j = 0; j < attribute2.Length; j++)
				{
					if (attribute2[j] != attribute3[j])
					{
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
						break;
					}
				}
			});
			btnAllOn.OnClickAsObservable().Subscribe(delegate
			{
				bool[] attribute4 = chaCtrl.GetAttribute();
				noUpdateHistory = true;
				for (int k = 0; k < tglAttribute.Length; k++)
				{
					tglAttribute[k].isOn = true;
				}
				noUpdateHistory = false;
				bool[] attribute5 = chaCtrl.GetAttribute();
				for (int l = 0; l < attribute4.Length; l++)
				{
					if (attribute4[l] != attribute5[l])
					{
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
						break;
					}
				}
			});
			btnAllOff.OnClickAsObservable().Subscribe(delegate
			{
				bool[] attribute6 = chaCtrl.GetAttribute();
				noUpdateHistory = true;
				for (int m = 0; m < tglAttribute.Length; m++)
				{
					tglAttribute[m].isOn = false;
				}
				noUpdateHistory = false;
				bool[] attribute7 = chaCtrl.GetAttribute();
				for (int n = 0; n < attribute6.Length; n++)
				{
					if (attribute6[n] != attribute7[n])
					{
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
						break;
					}
				}
			});
		}
	}
}
