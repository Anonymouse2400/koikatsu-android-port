using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsH : MonoBehaviour
	{
		[SerializeField]
		private TMP_Dropdown ddWeakpoint;

		[SerializeField]
		private Toggle[] tglKiss;

		[SerializeField]
		private Toggle[] tglAibu;

		[SerializeField]
		private Toggle[] tglAnal;

		[SerializeField]
		private Toggle[] tglMassage;

		[SerializeField]
		private Toggle[] tglNotCondom;

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
			ddWeakpoint.value = param.weakPoint + 1;
			tglKiss[0].isOn = param.denial.kiss;
			tglKiss[1].isOn = !param.denial.kiss;
			tglAibu[0].isOn = param.denial.aibu;
			tglAibu[1].isOn = !param.denial.aibu;
			tglAnal[0].isOn = param.denial.anal;
			tglAnal[1].isOn = !param.denial.anal;
			tglMassage[0].isOn = param.denial.massage;
			tglMassage[1].isOn = !param.denial.massage;
			tglNotCondom[0].isOn = param.denial.notCondom;
			tglNotCondom[1].isOn = !param.denial.notCondom;
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
			Singleton<CustomBase>.Instance.actUpdateCvsH += UpdateCustomUI;
			ddWeakpoint.onValueChanged.AddListener(delegate(int idx)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && idx - 1 != param.weakPoint)
				{
					param.weakPoint = idx - 1;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglKiss[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.denial.kiss != isOn)
				{
					param.denial.kiss = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglAibu[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.denial.aibu != isOn)
				{
					param.denial.aibu = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglAnal[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.denial.anal != isOn)
				{
					param.denial.anal = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglMassage[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.denial.massage != isOn)
				{
					param.denial.massage = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglNotCondom[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.denial.notCondom != isOn)
				{
					param.denial.notCondom = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
		}
	}
}
