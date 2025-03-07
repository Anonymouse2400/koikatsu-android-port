using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsADK : MonoBehaviour
	{
		[SerializeField]
		private Toggle[] tglAggressive;

		[SerializeField]
		private Toggle[] tglDilligence;

		[SerializeField]
		private Toggle[] tglKindness;

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
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
			int num = param.aggressive + 5;
			for (int i = 0; i < tglAggressive.Length; i++)
			{
				tglAggressive[i].isOn = i == num;
			}
			num = param.diligence + 5;
			for (int j = 0; j < tglDilligence.Length; j++)
			{
				tglDilligence[j].isOn = j == num;
			}
			num = param.kindness + 5;
			for (int k = 0; k < tglKindness.Length; k++)
			{
				tglKindness[k].isOn = k == num;
			}
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.actUpdateCvsADK += UpdateCustomUI;
			tglAggressive.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
					{
						int num = p.index - 5;
						if (num != param.aggressive)
						{
							param.aggressive = num;
							Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
						}
					}
				});
			});
			tglDilligence.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
					{
						int num2 = p.index - 5;
						if (num2 != param.diligence)
						{
							param.diligence = num2;
							Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
						}
					}
				});
			});
			tglKindness.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
					{
						int num3 = p.index - 5;
						if (num3 != param.kindness)
						{
							param.kindness = num3;
							Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
						}
					}
				});
			});
		}
	}
}
