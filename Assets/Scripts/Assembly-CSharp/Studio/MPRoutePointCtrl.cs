using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MPRoutePointCtrl : MonoBehaviour
	{
		[Serializable]
		private class InputCombination
		{
			public TMP_InputField input;

			public Slider slider;

			public bool interactable
			{
				set
				{
					input.interactable = value;
					slider.interactable = value;
				}
			}

			public string text
			{
				get
				{
					return input.text;
				}
				set
				{
					input.text = value;
					slider.value = Utility.StringToFloat(value);
				}
			}

			public float value
			{
				get
				{
					return slider.value;
				}
				set
				{
					slider.value = value;
					input.text = value.ToString("0.0");
				}
			}

			public float min
			{
				get
				{
					return slider.minValue;
				}
			}

			public float max
			{
				get
				{
					return slider.maxValue;
				}
			}
		}

		[Serializable]
		private class ToggleGroup
		{
			[SerializeField]
			private Toggle[] toggle;

			public int isOn
			{
				get
				{
					return Array.FindIndex(toggle, (Toggle _t) => _t.isOn);
				}
				set
				{
					for (int i = 0; i < toggle.Length; i++)
					{
						toggle[i].isOn = i == value;
					}
				}
			}

			public bool interactable
			{
				set
				{
					Toggle[] array = this.toggle;
					foreach (Toggle toggle in array)
					{
						toggle.interactable = value;
					}
				}
			}

			public Toggle this[int _idx]
			{
				get
				{
					return toggle[_idx];
				}
			}

			public Action<bool, int> action
			{
				set
				{
					for (int i = 0; i < toggle.Length; i++)
					{
						int no = i;
						toggle[i].onValueChanged.AddListener(delegate(bool _b)
						{
							value(_b, no);
						});
					}
				}
			}
		}

		private class EaseInfo
		{
			public string name { get; private set; }

			public StudioTween.EaseType ease { get; private set; }

			public EaseInfo(string _name, StudioTween.EaseType _ease)
			{
				name = _name;
				ease = _ease;
			}
		}

		[SerializeField]
		private InputCombination inputSpeed = new InputCombination();

		[SerializeField]
		private Dropdown dropdownEase;

		[SerializeField]
		private ToggleGroup toggleConnection = new ToggleGroup();

		[SerializeField]
		private Toggle toggleLink;

		private OCIRoutePoint m_OCIRoutePoint;

		private bool m_Active;

		private List<EaseInfo> listEase;

		private bool isUpdateInfo;

		public OCIRoutePoint ociRoutePoint
		{
			get
			{
				return m_OCIRoutePoint;
			}
			set
			{
				m_OCIRoutePoint = value;
				UpdateInfo();
			}
		}

		public bool active
		{
			get
			{
				return m_Active;
			}
			set
			{
				m_Active = value;
				base.gameObject.SetActive(m_Active && m_OCIRoutePoint != null);
			}
		}

		public bool Deselect(OCIRoutePoint _ociRoutePoint)
		{
			if (m_OCIRoutePoint != _ociRoutePoint)
			{
				return false;
			}
			ociRoutePoint = null;
			active = false;
			return true;
		}

		public void UpdateInteractable(OCIRoute _route)
		{
			if (_route == null || !_route.listPoint.Contains(m_OCIRoutePoint))
			{
				return;
			}
			bool interactable = !_route.isPlay;
			inputSpeed.interactable = interactable;
			dropdownEase.interactable = interactable;
			toggleConnection.interactable = interactable;
			toggleLink.interactable = interactable;
			if (m_OCIRoutePoint.connection == OIRoutePointInfo.Connection.Curve && m_OCIRoutePoint.link)
			{
				int index = _route.listPoint.FindIndex((OCIRoutePoint p) => p == m_OCIRoutePoint) - 1;
				OCIRoutePoint oCIRoutePoint = _route.listPoint.SafeGet(index);
				if (oCIRoutePoint != null && oCIRoutePoint.connection == OIRoutePointInfo.Connection.Curve)
				{
					inputSpeed.interactable = false;
					dropdownEase.interactable = false;
				}
			}
		}

		private void UpdateInfo()
		{
			if (m_OCIRoutePoint != null)
			{
				isUpdateInfo = true;
				inputSpeed.value = m_OCIRoutePoint.routePointInfo.speed;
				int num = listEase.FindIndex((EaseInfo e) => e.ease == m_OCIRoutePoint.easeType);
				dropdownEase.value = ((num >= 0) ? num : 0);
				toggleConnection.isOn = (int)m_OCIRoutePoint.connection;
				toggleLink.isOn = m_OCIRoutePoint.link;
				isUpdateInfo = false;
				UpdateInteractable(m_OCIRoutePoint.route);
			}
		}

		private void OnValueChangeSpeed(float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			IEnumerable<OCIRoutePoint> enumerable = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 6
				select v as OCIRoutePoint;
			foreach (OCIRoutePoint item in enumerable)
			{
				item.speed = _value;
			}
			inputSpeed.value = _value;
		}

		private void OnEndEditSpeed(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), inputSpeed.min, inputSpeed.max);
			IEnumerable<OCIRoutePoint> enumerable = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 6
				select v as OCIRoutePoint;
			foreach (OCIRoutePoint item in enumerable)
			{
				item.speed = num;
			}
			inputSpeed.value = num;
		}

		private void OnValueChangedEase(int _value)
		{
			if (m_OCIRoutePoint == null)
			{
				return;
			}
			IEnumerable<OCIRoutePoint> enumerable = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 6
				select v as OCIRoutePoint;
			StudioTween.EaseType ease = listEase[_value].ease;
			foreach (OCIRoutePoint item in enumerable)
			{
				item.easeType = ease;
			}
		}

		private void OnValueChangedConnection(bool _value, int _idx)
		{
			if (isUpdateInfo || m_OCIRoutePoint == null || !_value)
			{
				return;
			}
			toggleConnection.isOn = _idx;
			IEnumerable<OCIRoutePoint> enumerable = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 6
				select v as OCIRoutePoint;
			HashSet<OCIRoute> hashSet = new HashSet<OCIRoute>();
			foreach (OCIRoutePoint item in enumerable)
			{
				item.connection = (OIRoutePointInfo.Connection)_idx;
				hashSet.Add(item.route);
			}
			foreach (OCIRoute item2 in hashSet)
			{
				item2.ForceUpdateLine();
			}
			UpdateInteractable(m_OCIRoutePoint.route);
		}

		private void OnValueChangedLink(bool _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			IEnumerable<OCIRoutePoint> enumerable = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 6
				select v as OCIRoutePoint;
			HashSet<OCIRoute> hashSet = new HashSet<OCIRoute>();
			foreach (OCIRoutePoint item in enumerable)
			{
				item.link = _value;
				hashSet.Add(item.route);
			}
			foreach (OCIRoute item2 in hashSet)
			{
				item2.ForceUpdateLine();
			}
			UpdateInteractable(m_OCIRoutePoint.route);
		}

		private void Awake()
		{
			listEase = new List<EaseInfo>();
			listEase.Add(new EaseInfo("直線的", StudioTween.EaseType.linear));
			listEase.Add(new EaseInfo("徐々に早く", StudioTween.EaseType.easeInQuad));
			listEase.Add(new EaseInfo("徐々に遅く", StudioTween.EaseType.easeOutQuad));
			listEase.Add(new EaseInfo("急に早く", StudioTween.EaseType.easeInQuart));
			listEase.Add(new EaseInfo("急に遅く", StudioTween.EaseType.easeOutQuart));
			listEase.Add(new EaseInfo("バウンド", StudioTween.EaseType.easeOutBounce));
			dropdownEase.options = listEase.Select((EaseInfo v) => new Dropdown.OptionData(v.name)).ToList();
			inputSpeed.input.onEndEdit.AddListener(OnEndEditSpeed);
			inputSpeed.slider.onValueChanged.AddListener(OnValueChangeSpeed);
			dropdownEase.onValueChanged.AddListener(OnValueChangedEase);
			toggleConnection.action = OnValueChangedConnection;
			toggleLink.onValueChanged.AddListener(OnValueChangedLink);
		}
	}
}
