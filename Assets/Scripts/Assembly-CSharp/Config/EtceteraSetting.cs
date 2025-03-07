using System.Collections.Generic;
using System.Linq;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class EtceteraSetting : BaseSetting
	{
		[Header("注視点の表示")]
		[SerializeField]
		private Toggle lookToggle;

		[Header("眉の最前面表示")]
		[SerializeField]
		private Toggle foregroundEyebrowToggle;

		[SerializeField]
		[Header("目の最前面表示")]
		private Toggle foregroundEyesToggle;

		[SerializeField]
		[Header("頬赤の表示")]
		private Toggle hohoAkaToggle;

		[SerializeField]
		[Header("アクセサリ表示[0=>無し, 1=>頭の一部, 2=>全て]")]
		private Toggle[] accessoryTypeToggles;

		[Header("マウスクリックロック移動")]
		[SerializeField]
		private Toggle moveLockToggle;

		[SerializeField]
		[Header("登場キャラ人数")]
		private Slider maxCharaNumSlider;

		[SerializeField]
		[Header("TPS時の注視点位置Y")]
		private Slider tpsOffsetYSlider;

		[SerializeField]
		[Header("TPS時のマウス感度X")]
		private Slider tpsSensitivityXSlider;

		[SerializeField]
		[Header("TPS時のマウス感度Y")]
		private Slider tpsSensitivityYSlider;

		[SerializeField]
		[Header("TPS時のスムース移動時間")]
		private Slider tpsSmoothMoveTimeSlider;

		[SerializeField]
		[Header("FPS時のマウス感度X")]
		private Slider fpsSensitivityXSlider;

		[SerializeField]
		[Header("FPS時のマウス感度Y")]
		private Slider fpsSensitivityYSlider;

		[SerializeField]
		[Header("FPS時のスムース移動時間")]
		private Slider fpsSmoothMoveTimeSlider;

		[SerializeField]
		[Header("カメラ移動Xの反転")]
		private Toggle invertMoveXToggle;

		[Header("カメラ移動Yの反転")]
		[SerializeField]
		private Toggle invertMoveYToggle;

		[Header("しゃがみをコントロールキー")]
		[SerializeField]
		private Toggle crouchCtrlKeyToggle;

		[SerializeField]
		[Header("トイレで一人称")]
		private Toggle toiletTPSToggle;

		[SerializeField]
		[Header("TPS時の注視点位置Yリセット")]
		private Button tpsOffsetYResetButton;

		[SerializeField]
		[Header("TPS時のマウス感度Xリセット")]
		private Button tpsSensitivityXResetButton;

		[Header("TPS時のマウス感度Yリセット")]
		[SerializeField]
		private Button tpsSensitivityYResetButton;

		[SerializeField]
		[Header("TPS時のスムース移動時間リセット")]
		private Button tpsSmoothMoveTimeResetButton;

		[SerializeField]
		[Header("FPS時のマウス感度Xリセット")]
		private Button fpsSensitivityXResetButton;

		[Header("FPS時のマウス感度Yリセット")]
		[SerializeField]
		private Button fpsSensitivityYResetButton;

		[SerializeField]
		[Header("FPS時のスムース移動時間リセット")]
		private Button fpsSmoothMoveTimeResetButton;

		public override void Init()
		{
			EtceteraSystem data = Manager.Config.EtcData;
			LinkToggle(lookToggle, delegate(bool isOn)
			{
				data.Look = isOn;
			});
			LinkToggle(foregroundEyebrowToggle, delegate(bool isOn)
			{
				data.ForegroundEyebrow = isOn;
			});
			LinkToggle(foregroundEyesToggle, delegate(bool isOn)
			{
				data.ForegroundEyes = isOn;
			});
			LinkToggle(hohoAkaToggle, delegate(bool isOn)
			{
				data.hohoAka = isOn;
			});
			ReadOnlyReactiveProperty<int> source = (from list in accessoryTypeToggles.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).ToReadOnlyReactiveProperty();
			source.Subscribe(delegate(int i)
			{
				data.loadHeadAccessory = i == 1;
				data.loadAllAccessory = i == 2;
			});
			source.Skip(1).Subscribe(delegate
			{
				EnterSE();
			});
			ActionSystem actData = Manager.Config.ActData;
			LinkToggle(moveLockToggle, delegate(bool isOn)
			{
				actData.MoveLook = isOn;
			});
			maxCharaNumSlider.maxValue = 38f;
			LinkSlider(maxCharaNumSlider, delegate(float value)
			{
				actData.MaxCharaNum = (int)value;
			});
			LinkSlider(tpsOffsetYSlider, delegate(float value)
			{
				actData.TPSOffsetY = value;
			});
			LinkSlider(tpsSensitivityXSlider, delegate(float value)
			{
				actData.TPSSensitivityX = (int)value;
			});
			LinkSlider(tpsSensitivityYSlider, delegate(float value)
			{
				actData.TPSSensitivityY = (int)value;
			});
			LinkSlider(tpsSmoothMoveTimeSlider, delegate(float value)
			{
				actData.TPSSmoothMoveTime = (int)value;
			});
			LinkSlider(fpsSensitivityXSlider, delegate(float value)
			{
				actData.FPSSensitivityX = (int)value;
			});
			LinkSlider(fpsSensitivityYSlider, delegate(float value)
			{
				actData.FPSSensitivityY = (int)value;
			});
			LinkSlider(fpsSmoothMoveTimeSlider, delegate(float value)
			{
				actData.FPSSmoothMoveTime = (int)value;
			});
			LinkToggle(invertMoveXToggle, delegate(bool isOn)
			{
				actData.InvertMoveX = isOn;
			});
			LinkToggle(invertMoveYToggle, delegate(bool isOn)
			{
				actData.InvertMoveY = isOn;
			});
			LinkToggle(crouchCtrlKeyToggle, delegate(bool isOn)
			{
				actData.CrouchCtrlKey = isOn;
			});
			LinkToggle(toiletTPSToggle, delegate(bool isOn)
			{
				actData.ToiletTPS = isOn;
			});
			tpsOffsetYResetButton.OnClickAsObservable().Subscribe(delegate
			{
				tpsOffsetYSlider.value = 0f;
			});
			tpsSensitivityXResetButton.OnClickAsObservable().Subscribe(delegate
			{
				tpsSensitivityXSlider.value = 50f;
			});
			tpsSensitivityYResetButton.OnClickAsObservable().Subscribe(delegate
			{
				tpsSensitivityYSlider.value = 50f;
			});
			tpsSmoothMoveTimeResetButton.OnClickAsObservable().Subscribe(delegate
			{
				tpsSmoothMoveTimeSlider.value = 30f;
			});
			fpsSensitivityXResetButton.OnClickAsObservable().Subscribe(delegate
			{
				fpsSensitivityXSlider.value = 50f;
			});
			fpsSensitivityYResetButton.OnClickAsObservable().Subscribe(delegate
			{
				fpsSensitivityYSlider.value = 50f;
			});
			fpsSmoothMoveTimeResetButton.OnClickAsObservable().Subscribe(delegate
			{
				fpsSmoothMoveTimeSlider.value = 20f;
			});
			Observable.Merge(new IObservable<Unit>[7]
			{
				tpsOffsetYResetButton.OnClickAsObservable(),
				tpsSensitivityXResetButton.OnClickAsObservable(),
				tpsSensitivityYResetButton.OnClickAsObservable(),
				tpsSmoothMoveTimeResetButton.OnClickAsObservable(),
				fpsSensitivityXResetButton.OnClickAsObservable(),
				fpsSensitivityYResetButton.OnClickAsObservable(),
				fpsSmoothMoveTimeResetButton.OnClickAsObservable()
			}).Subscribe(delegate
			{
				EnterSE();
			});
		}

		protected override void ValueToUI()
		{
			EtceteraSystem etcData = Manager.Config.EtcData;
			lookToggle.isOn = etcData.Look;
			foregroundEyebrowToggle.isOn = etcData.ForegroundEyebrow;
			foregroundEyesToggle.isOn = etcData.ForegroundEyes;
			hohoAkaToggle.isOn = etcData.hohoAka;
			foreach (var item in accessoryTypeToggles.Select((Toggle tgl, int index) => new { tgl, index }))
			{
				if (etcData.loadHeadAccessory)
				{
					item.tgl.isOn = item.index == 1;
				}
				else if (etcData.loadAllAccessory)
				{
					item.tgl.isOn = item.index == 2;
				}
				else
				{
					item.tgl.isOn = item.index == 0;
				}
			}
			ActionSystem actData = Manager.Config.ActData;
			moveLockToggle.isOn = actData.MoveLook;
			maxCharaNumSlider.value = actData.MaxCharaNum;
			tpsOffsetYSlider.value = actData.TPSOffsetY;
			tpsSensitivityXSlider.value = actData.TPSSensitivityX;
			tpsSensitivityYSlider.value = actData.TPSSensitivityY;
			tpsSmoothMoveTimeSlider.value = actData.TPSSmoothMoveTime;
			fpsSensitivityXSlider.value = actData.FPSSensitivityX;
			fpsSensitivityYSlider.value = actData.FPSSensitivityY;
			fpsSmoothMoveTimeSlider.value = actData.FPSSmoothMoveTime;
			invertMoveXToggle.isOn = actData.InvertMoveX;
			invertMoveYToggle.isOn = actData.InvertMoveY;
			crouchCtrlKeyToggle.isOn = actData.CrouchCtrlKey;
			toiletTPSToggle.isOn = actData.ToiletTPS;
		}
	}
}
