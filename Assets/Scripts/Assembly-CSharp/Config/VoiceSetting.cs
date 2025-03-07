using System.Collections.Generic;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class VoiceSetting : BaseSetting
	{
		private class SetData
		{
			public SoundData sd;

			public Toggle toggle;

			public Slider slider;
		}

		[SerializeField]
		private GameObject charaVoice;

		[SerializeField]
		private RectTransform node;

		private Dictionary<Transform, SetData> dic = new Dictionary<Transform, SetData>();

		public override void Init()
		{
			Dictionary<int, VoiceSystem.Voice> chara = Singleton<Voice>.Instance._Config.chara;
			int childCount = node.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = node.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					Add(Singleton<Voice>.Instance._Config.PCM, child as RectTransform);
				}
			}
			int num = childCount + ((childCount % 2 != 0) ? 1 : 0);
			foreach (KeyValuePair<int, VoiceInfo.Param> item in Singleton<Voice>.Instance.voiceInfoDic)
			{
				if (chara.ContainsKey(item.Key))
				{
					Create(num++, item.Key, item.Value.Personality);
				}
			}
		}

		protected override void ValueToUI()
		{
			foreach (SetData value in dic.Values)
			{
				value.toggle.isOn = value.sd.Switch;
				value.slider.value = value.sd.Volume;
			}
		}

		private void Create(int num, int key, string name)
		{
			GameObject gameObject = Object.Instantiate(charaVoice, node.transform, false);
			gameObject.SetActive(true);
			gameObject.name = key.ToString();
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = name;
			Add(Singleton<Voice>.Instance._Config.chara[key].sound, gameObject.GetComponent<RectTransform>());
		}

		private bool Add(SoundData sd, Transform trans)
		{
			if (dic.ContainsKey(trans))
			{
				return false;
			}
			Slider componentInChildren = trans.GetComponentInChildren<Slider>();
			if (componentInChildren == null)
			{
				return false;
			}
			Toggle componentInChildren2 = trans.GetComponentInChildren<Toggle>();
			if (componentInChildren2 == null)
			{
				return false;
			}
			SetData setData = new SetData();
			setData.sd = sd;
			setData.slider = componentInChildren;
			setData.toggle = componentInChildren2;
			SetData setData2 = setData;
			AddEvent(setData2);
			dic.Add(trans, setData2);
			return true;
		}

		private void AddEvent(SetData data)
		{
			data.toggle.onValueChanged.AsObservable().Subscribe(delegate(bool isOn)
			{
				data.sd.Switch = isOn;
			});
			data.toggle.OnValueChangedAsObservable().SubscribeToInteractable(data.slider);
			(from value in data.slider.onValueChanged.AsObservable()
				select (int)value).Subscribe(delegate(int value)
			{
				data.sd.Volume = value;
			});
		}
	}
}
