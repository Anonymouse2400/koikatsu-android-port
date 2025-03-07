using System;
using Illusion.Game;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Config
{
	public abstract class BaseSetting : MonoBehaviour
	{
		private bool _isPlaySE = true;

		public bool isPlaySE
		{
			get
			{
				return _isPlaySE;
			}
			set
			{
				_isPlaySE = value;
			}
		}

		protected void EnterSE()
		{
			if (_isPlaySE)
			{
				Utils.Sound.Play(SystemSE.sel);
			}
		}

		protected void LinkToggle(Toggle toggle, Action<bool> act)
		{
			toggle.onValueChanged.AsObservable().Subscribe(delegate(bool isOn)
			{
				EnterSE();
				act(isOn);
			});
		}

		protected void LinkSlider(Slider slider, Action<float> act)
		{
			slider.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				act(value);
			});
			(from _ in slider.OnPointerDownAsObservable()
				where Input.GetMouseButtonDown(0)
				select _).Subscribe(delegate
			{
				EnterSE();
			});
		}

		protected void LinkTmpDropdown(TMP_Dropdown dropdown, Action<float> act)
		{
			dropdown.onValueChanged.AsObservable().Subscribe(delegate(int value)
			{
				act(value);
			});
			(from _ in dropdown.OnPointerDownAsObservable()
				where Input.GetMouseButtonDown(0)
				select _).Subscribe(delegate
			{
				EnterSE();
			});
		}

		public abstract void Init();

		protected abstract void ValueToUI();

		public void UIPresenter()
		{
			bool flag = _isPlaySE;
			_isPlaySE = false;
			ValueToUI();
			_isPlaySE = flag;
		}
	}
}
