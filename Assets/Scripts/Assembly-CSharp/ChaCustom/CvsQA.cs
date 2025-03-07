using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsQA : MonoBehaviour
	{
		[SerializeField]
		private Toggle[] tglAnimal;

		[SerializeField]
		private Toggle[] tglEat;

		[SerializeField]
		private Toggle[] tglCook;

		[SerializeField]
		private Toggle[] tglExercise;

		[SerializeField]
		private Toggle[] tglStudy;

		[SerializeField]
		private Toggle[] tglFashionable;

		[SerializeField]
		private Toggle[] tglBlackCoffee;

		[SerializeField]
		private Toggle[] tglSpicy;

		[SerializeField]
		private Toggle[] tglSweet;

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
			tglAnimal[0].isOn = param.awnser.animal;
			tglAnimal[1].isOn = !param.awnser.animal;
			tglEat[0].isOn = param.awnser.eat;
			tglEat[1].isOn = !param.awnser.eat;
			tglCook[0].isOn = param.awnser.cook;
			tglCook[1].isOn = !param.awnser.cook;
			tglExercise[0].isOn = param.awnser.exercise;
			tglExercise[1].isOn = !param.awnser.exercise;
			tglStudy[0].isOn = param.awnser.study;
			tglStudy[1].isOn = !param.awnser.study;
			tglFashionable[0].isOn = param.awnser.fashionable;
			tglFashionable[1].isOn = !param.awnser.fashionable;
			tglBlackCoffee[0].isOn = param.awnser.blackCoffee;
			tglBlackCoffee[1].isOn = !param.awnser.blackCoffee;
			tglSpicy[0].isOn = param.awnser.spicy;
			tglSpicy[1].isOn = !param.awnser.spicy;
			tglSweet[0].isOn = param.awnser.sweet;
			tglSweet[1].isOn = !param.awnser.sweet;
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
			Singleton<CustomBase>.Instance.actUpdateCvsQA += UpdateCustomUI;
			tglAnimal[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.animal != isOn)
				{
					param.awnser.animal = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglEat[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.eat != isOn)
				{
					param.awnser.eat = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglCook[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.cook != isOn)
				{
					param.awnser.cook = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglExercise[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.exercise != isOn)
				{
					param.awnser.exercise = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglStudy[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.study != isOn)
				{
					param.awnser.study = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglFashionable[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.fashionable != isOn)
				{
					param.awnser.fashionable = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglBlackCoffee[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.blackCoffee != isOn)
				{
					param.awnser.blackCoffee = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglSpicy[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.spicy != isOn)
				{
					param.awnser.spicy = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
			tglSweet[0].OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && param.awnser.sweet != isOn)
				{
					param.awnser.sweet = isOn;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
				}
			});
		}
	}
}
