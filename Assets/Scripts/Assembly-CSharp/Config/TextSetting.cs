using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Component.UI.ColorPicker;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class TextSetting : BaseSetting
	{
		[SerializeField]
		[Header("既読スキップ")]
		private Toggle[] readSkipToggles;

		[Header("次のテキスト表示時に音声を停止")]
		[SerializeField]
		private Toggle nextVoiceStopToggle;

		[Header("選択肢でもスキップ継続")]
		[SerializeField]
		private Toggle choiceSkipToggle;

		[SerializeField]
		[Header("選択肢でもオート継続")]
		private Toggle choiceAutoToggle;

		[SerializeField]
		[Header("文字表示速度")]
		private Slider fontSpeedSlider;

		[Header("自動送り待ち時間")]
		[SerializeField]
		private Slider autoWaitTimeSlider;

		[SerializeField]
		[Header("文字表示サンプル")]
		private TypefaceAnimatorEx[] ta;

		[SerializeField]
		[Header("文字表示背景色サンプル")]
		private Image sampleWindow;

		[SerializeField]
		[Header("背景色+文字色変更")]
		private PickerSlider[] pickers;

		private IDisposable cancel;

		private void OnDestroy()
		{
			Release();
		}

		private void Release()
		{
			if (cancel != null)
			{
				cancel.Dispose();
			}
		}

		public override void Init()
		{
			TextSystem data = Manager.Config.TextData;
			ReadOnlyReactiveProperty<int> source = (from list in readSkipToggles.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).ToReadOnlyReactiveProperty();
			source.Subscribe(delegate(int i)
			{
				data.ReadSkip = i == 0;
			});
			source.Skip(1).Subscribe(delegate
			{
				EnterSE();
			});
			LinkToggle(nextVoiceStopToggle, delegate(bool isOn)
			{
				data.NextVoiceStop = isOn;
			});
			LinkToggle(choiceSkipToggle, delegate(bool isOn)
			{
				data.ChoicesSkip = isOn;
			});
			LinkToggle(choiceAutoToggle, delegate(bool isOn)
			{
				data.ChoicesAuto = isOn;
			});
			LinkSlider(fontSpeedSlider, delegate(float value)
			{
				data.FontSpeed = (int)value;
			});
			(from value in fontSpeedSlider.OnValueChangedAsObservable()
				select (int)value).Subscribe(delegate(int value)
			{
				TypefaceAnimatorEx[] array = ta;
				foreach (TypefaceAnimatorEx typefaceAnimatorEx in array)
				{
					typefaceAnimatorEx.isNoWait = value == 100;
					if (!typefaceAnimatorEx.isNoWait)
					{
						typefaceAnimatorEx.timeMode = TypefaceAnimatorEx.TimeMode.Speed;
						typefaceAnimatorEx.speed = value;
					}
				}
			});
			LinkSlider(autoWaitTimeSlider, delegate(float value)
			{
				data.AutoWaitTime = value;
			});
			autoWaitTimeSlider.OnValueChangedAsObservable().Subscribe(delegate
			{
				if (cancel != null)
				{
					cancel.Dispose();
				}
				TypefaceAnimatorEx[] array2 = ta;
				foreach (TypefaceAnimatorEx typefaceAnimatorEx2 in array2)
				{
					typefaceAnimatorEx2.Play();
				}
			});
			(from isPlaying in (from _ in this.UpdateAsObservable()
					select ta[0].isPlaying).DistinctUntilChanged()
				where !isPlaying
				select isPlaying).Subscribe(delegate
			{
				if (cancel != null)
				{
					cancel.Dispose();
				}
				float autoWaitTimer = 0f;
				cancel = Observable.FromCoroutine((CancellationToken __) => new WaitWhile(delegate
				{
					float autoWaitTime = data.AutoWaitTime;
					autoWaitTimer = Mathf.Min(autoWaitTimer + Time.deltaTime, autoWaitTime);
					return autoWaitTimer < autoWaitTime;
				})).Subscribe(delegate
				{
					TypefaceAnimatorEx[] array3 = ta;
					foreach (TypefaceAnimatorEx typefaceAnimatorEx3 in array3)
					{
						typefaceAnimatorEx3.Play();
					}
				});
			});
			Text textM = ta[0].GetComponent<Text>();
			Text textF = ta[1].GetComponent<Text>();
			for (int m = 0; m < pickers.Length; m++)
			{
				switch (m)
				{
				case 0:
					pickers[m].updateColorAction += delegate(Color color)
					{
						data.WindowColor = color;
						sampleWindow.color = color;
					};
					break;
				case 1:
					pickers[m].updateColorAction += delegate(Color color)
					{
						data.Font0Color = color;
						textM.color = color;
					};
					break;
				default:
					pickers[m].updateColorAction += delegate(Color color)
					{
						data.Font1Color = color;
						textF.color = color;
					};
					break;
				}
			}
		}

		protected override void ValueToUI()
		{
			TextSystem textData = Manager.Config.TextData;
			foreach (var item in readSkipToggles.Select((Toggle tgl, int index) => new { tgl, index }))
			{
				item.tgl.isOn = ((item.index != 0) ? (!textData.ReadSkip) : textData.ReadSkip);
			}
			nextVoiceStopToggle.isOn = textData.NextVoiceStop;
			choiceSkipToggle.isOn = textData.ChoicesSkip;
			choiceAutoToggle.isOn = textData.ChoicesAuto;
			fontSpeedSlider.value = textData.FontSpeed;
			autoWaitTimeSlider.value = textData.AutoWaitTime;
			for (int i = 0; i < pickers.Length; i++)
			{
				Color color;
				switch (i)
				{
				case 0:
					color = textData.WindowColor;
					break;
				case 1:
					color = textData.Font0Color;
					break;
				default:
					color = textData.Font1Color;
					break;
				}
				pickers[i].SetColor(color);
			}
			ta[0].GetComponent<Text>().color = textData.Font0Color;
			ta[1].GetComponent<Text>().color = textData.Font1Color;
		}
	}
}
