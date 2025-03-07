using System;
using Illusion.Component.UI.ColorPicker;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class AdditionalFunctionsSetting : BaseSetting
	{
		[Serializable]
		private class SetData
		{
			public Toggle toggle;

			public Slider slider;
		}

		[Serializable]
		private class SampleImage
		{
			public Image image;

			public PickerSlider picker;
		}

		[SerializeField]
		private SetData expH;

		[SerializeField]
		[Header("初回H緩和")]
		private Toggle tglFirstHEasy;

		[Header("イキそうボタン追加")]
		[SerializeField]
		private Toggle tglImmediatelyFinish;

		[SerializeField]
		[Header("ADVイベント省略しない")]
		private Toggle tglADVEventNotOmission;

		[SerializeField]
		[Header("会話時間のないキャラを立ち止まらせない")]
		private Toggle tglTalkTimeNoneWalkStop;

		[SerializeField]
		[Header("AI行動プレイヤーを対象しない")]
		private Toggle tglAINotPlayerTarget;

		[Header("AI行動プレイヤーを対象後コミュニケーションに入らせない")]
		[SerializeField]
		private Toggle tglAINotPlayerTargetCommunication;

		[Header("モブの表示")]
		[SerializeField]
		private Toggle tglMobVisible;

		[SerializeField]
		[Header("モブ色")]
		private SampleImage mobColor;

		[SerializeField]
		[Header("他クラスの登録人数を最大にする")]
		private Toggle tglOtherClassRegisterMax;

		[SerializeField]
		[Header("マップ選択デートに入らない")]
		private Toggle tglDateMapSelectNoneEvent;

		[SerializeField]
		[Header("ＡＩ行動のＨしたい（主人公）の欲求が溜まる量を補正する")]
		private Slider AIActionCorrectionHSlider;

		[Header("ＡＩ行動の会話したい（主人公）の欲求が溜まる量を補正する")]
		[SerializeField]
		private Slider AIActionCorrectionTalkSlider;

		[Header("コミュニケーションでヒロインから誘う行動の発生率を補正する")]
		[SerializeField]
		private Slider CommunicationCorrectionHeroineActionSlider;

		private void InitSet(SetData _data, AdditionalFunctionsSystem.CheatPropertyInt _property)
		{
			_data.toggle.isOn = _property.isON;
			_data.slider.value = _property.property;
		}

		private void InitLinkInt(SetData _data, AdditionalFunctionsSystem.CheatPropertyInt _property)
		{
			LinkToggle(_data.toggle, delegate(bool isOn)
			{
				_property.isON = isOn;
			});
			_data.toggle.OnValueChangedAsObservable().SubscribeToInteractable(_data.slider);
			LinkSlider(_data.slider, delegate(float value)
			{
				_property.property = (int)value;
			});
		}

		public override void Init()
		{
			AdditionalFunctionsSystem data = Manager.Config.AddData;
			InitLinkInt(expH, data.expH);
			LinkToggle(tglFirstHEasy, delegate(bool isOn)
			{
				data.firstHEasy = isOn;
			});
			LinkToggle(tglImmediatelyFinish, delegate(bool isOn)
			{
				data.immediatelyFinish = isOn;
			});
			LinkToggle(tglADVEventNotOmission, delegate(bool isOn)
			{
				data.ADVEventNotOmission = isOn;
			});
			LinkToggle(tglTalkTimeNoneWalkStop, delegate(bool isOn)
			{
				data.TalkTimeNoneWalkStop = isOn;
			});
			LinkToggle(tglAINotPlayerTarget, delegate(bool isOn)
			{
				data.AINotPlayerTarget = isOn;
			});
			LinkToggle(tglAINotPlayerTargetCommunication, delegate(bool isOn)
			{
				data.AINotPlayerTargetCommunication = isOn;
			});
			if (tglMobVisible != null)
			{
				LinkToggle(tglMobVisible, delegate(bool isOn)
				{
					data.mobVisible = isOn;
				});
			}
			if (mobColor != null && mobColor.picker != null)
			{
				mobColor.picker.updateColorAction += delegate(Color color)
				{
					data.mobColor = color;
					if (mobColor.image != null)
					{
						mobColor.image.color = color;
					}
				};
			}
			LinkToggle(tglOtherClassRegisterMax, delegate(bool isOn)
			{
				data.OtherClassRegisterMax = isOn;
			});
			if (tglDateMapSelectNoneEvent != null)
			{
				LinkToggle(tglDateMapSelectNoneEvent, delegate(bool isOn)
				{
					data.DateMapSelectNoneEvent = isOn;
				});
			}
			LinkSlider(AIActionCorrectionHSlider, delegate(float value)
			{
				data.AIActionCorrectionH = (int)value * 10;
			});
			LinkSlider(AIActionCorrectionTalkSlider, delegate(float value)
			{
				data.AIActionCorrectionTalk = (int)value * 10;
			});
			LinkSlider(CommunicationCorrectionHeroineActionSlider, delegate(float value)
			{
				data.CommunicationCorrectionHeroineAction = (int)value;
			});
		}

		protected override void ValueToUI()
		{
			AdditionalFunctionsSystem addData = Manager.Config.AddData;
			InitSet(expH, addData.expH);
			tglFirstHEasy.isOn = addData.firstHEasy;
			tglImmediatelyFinish.isOn = addData.immediatelyFinish;
			tglADVEventNotOmission.isOn = addData.ADVEventNotOmission;
			tglTalkTimeNoneWalkStop.isOn = addData.TalkTimeNoneWalkStop;
			tglAINotPlayerTarget.isOn = addData.AINotPlayerTarget;
			tglAINotPlayerTargetCommunication.isOn = addData.AINotPlayerTargetCommunication;
			if (tglMobVisible != null)
			{
				tglMobVisible.isOn = addData.mobVisible;
			}
			if (mobColor.picker != null)
			{
				mobColor.picker.SetColor(addData.mobColor);
			}
			tglOtherClassRegisterMax.isOn = addData.OtherClassRegisterMax;
			if (tglDateMapSelectNoneEvent != null)
			{
				tglDateMapSelectNoneEvent.isOn = addData.DateMapSelectNoneEvent;
			}
			AIActionCorrectionHSlider.value = (float)addData.AIActionCorrectionH * 0.1f;
			AIActionCorrectionTalkSlider.value = (float)addData.AIActionCorrectionTalk * 0.1f;
			CommunicationCorrectionHeroineActionSlider.value = addData.CommunicationCorrectionHeroineAction;
		}
	}
}
