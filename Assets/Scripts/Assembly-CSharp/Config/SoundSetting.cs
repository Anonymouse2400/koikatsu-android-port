using System;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Config
{
	public class SoundSetting : BaseSetting
	{
		[Serializable]
		private class SoundGroup
		{
			public Toggle toggle;

			public Slider slider;
		}

		[SerializeField]
		private SoundGroup Master;

		[SerializeField]
		private SoundGroup BGM;

		[SerializeField]
		private SoundGroup ENV;

		[SerializeField]
		private SoundGroup SystemSE;

		[SerializeField]
		private SoundGroup GameSE;

		private void InitSet(SoundGroup sg, SoundData sd)
		{
			sg.toggle.isOn = sd.Switch;
			sg.slider.value = sd.Volume;
		}

		private void InitLink(SoundGroup sg, SoundData sd, bool isSliderEvent)
		{
			LinkToggle(sg.toggle, delegate(bool isOn)
			{
				sd.Switch = isOn;
			});
			sg.toggle.OnValueChangedAsObservable().SubscribeToInteractable(sg.slider);
			if (isSliderEvent)
			{
				LinkSlider(sg.slider, delegate(float value)
				{
					sd.Volume = (int)value;
				});
				return;
			}
			(from _ in sg.slider.OnPointerDownAsObservable()
				where Input.GetMouseButtonDown(0)
				select _).Subscribe(delegate
			{
				EnterSE();
			});
		}

		public override void Init()
		{
			SoundSystem soundData = Manager.Config.SoundData;
			InitLink(Master, soundData.Master, true);
			InitLink(ENV, soundData.ENV, true);
			InitLink(SystemSE, soundData.SystemSE, true);
			InitLink(GameSE, soundData.GameSE, true);
			InitLink(BGM, soundData.BGM, true);
		}

		protected override void ValueToUI()
		{
			SoundSystem soundData = Manager.Config.SoundData;
			InitSet(Master, soundData.Master);
			InitSet(BGM, soundData.BGM);
			InitSet(ENV, soundData.ENV);
			InitSet(SystemSE, soundData.SystemSE);
			InitSet(GameSE, soundData.GameSE);
		}
	}
}
