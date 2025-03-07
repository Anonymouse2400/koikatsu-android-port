using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MPRouteCtrl : MonoBehaviour
	{
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

		[SerializeField]
		private TMP_InputField inputName;

		[SerializeField]
		private Button buttonAddPoint;

		[SerializeField]
		private ToggleGroup toggleOrient;

		[SerializeField]
		private Toggle toggleLoop;

		[SerializeField]
		private Toggle toggleLine;

		[SerializeField]
		private Button buttonColor;

		[SerializeField]
		private RouteControl routeControl;

		private OCIRoute m_OCIRoute;

		private bool m_Active;

		private bool isUpdateInfo;

		private bool isColorFunc;

		public OCIRoute ociRoute
		{
			get
			{
				return m_OCIRoute;
			}
			set
			{
				m_OCIRoute = value;
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
				base.gameObject.SetActive(m_Active && m_OCIRoute != null);
				routeControl.visible = value;
				if (isColorFunc && !value)
				{
					Singleton<Studio>.Instance.colorPalette.visible = false;
				}
			}
		}

		public bool Deselect(OCIRoute _ociRoute)
		{
			if (m_OCIRoute != _ociRoute)
			{
				return false;
			}
			ociRoute = null;
			active = false;
			return true;
		}

		public void UpdateInteractable(OCIRoute _route)
		{
			if (m_OCIRoute == _route)
			{
				bool interactable = !m_OCIRoute.isPlay;
				buttonAddPoint.interactable = interactable;
				toggleOrient.interactable = interactable;
				toggleLoop.interactable = interactable;
			}
		}

		private void UpdateInfo()
		{
			if (m_OCIRoute != null)
			{
				isUpdateInfo = true;
				inputName.text = m_OCIRoute.name;
				toggleLoop.isOn = m_OCIRoute.routeInfo.loop;
				toggleLine.isOn = m_OCIRoute.visibleLine;
				toggleOrient.isOn = (int)m_OCIRoute.routeInfo.orient;
				buttonColor.image.color = m_OCIRoute.routeInfo.color;
				isUpdateInfo = false;
				UpdateInteractable(m_OCIRoute);
			}
		}

		private void OnEndEditName(string _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIRoute.name = _value;
			}
		}

		private void OnClickAddPoint()
		{
			if (m_OCIRoute != null)
			{
				OCIRoutePoint oCIRoutePoint = m_OCIRoute.AddPoint();
				if (Studio.optionSystem.autoSelect && oCIRoutePoint != null)
				{
					Singleton<Studio>.Instance.treeNodeCtrl.SelectSingle(oCIRoutePoint.treeNodeObject);
				}
			}
		}

		private void OnValueChangedPlay(bool _value)
		{
			if (m_OCIRoute != null)
			{
				if (_value)
				{
					m_OCIRoute.Play();
				}
				else
				{
					m_OCIRoute.Stop();
				}
			}
		}

		private void OnValueChangedLoop(bool _value)
		{
			List<OCIRoute> list = (from v in Studio.GetSelectObjectCtrl()
				where v.kind == 4
				select v as OCIRoute).ToList();
			list.Add(m_OCIRoute);
			HashSet<OCIRoute> hashSet = new HashSet<OCIRoute>();
			foreach (OCIRoute item in list)
			{
				item.routeInfo.loop = _value;
				hashSet.Add(item);
			}
			foreach (OCIRoute item2 in hashSet)
			{
				item2.ForceUpdateLine();
			}
		}

		private void OnValueChangedLine(bool _value)
		{
			List<OCIRoute> list = (from v in Studio.GetSelectObjectCtrl()
				where v.kind == 4
				select v as OCIRoute).ToList();
			list.Add(m_OCIRoute);
			foreach (OCIRoute item in list)
			{
				item.visibleLine = _value;
			}
		}

		private void OnValueChangedOrient(bool _value, int _idx)
		{
			if (isUpdateInfo || !_value)
			{
				return;
			}
			List<OCIRoute> list = (from v in Studio.GetSelectObjectCtrl()
				where v.kind == 4
				select v as OCIRoute).ToList();
			list.Add(m_OCIRoute);
			foreach (OCIRoute item in list)
			{
				item.routeInfo.orient = (OIRouteInfo.Orient)_idx;
			}
			toggleOrient.isOn = _idx;
		}

		private void OnClickColor()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("ルートのラインカラー"))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			List<OCIRoute> array = (from v in Studio.GetSelectObjectCtrl()
				where v.kind == 4
				select v as OCIRoute).ToList();
			array.Add(m_OCIRoute);
			Singleton<Studio>.Instance.colorPalette.Setup("ルートのラインカラー", m_OCIRoute.routeInfo.color, delegate(Color _c)
			{
				foreach (OCIRoute item in array)
				{
					item.routeInfo.color = _c;
					item.SetLineColor(_c);
				}
				buttonColor.image.color = _c;
			}, false);
			isColorFunc = true;
		}

		private void Start()
		{
			inputName.onEndEdit.AddListener(OnEndEditName);
			buttonAddPoint.onClick.AddListener(OnClickAddPoint);
			toggleLoop.onValueChanged.AddListener(OnValueChangedLoop);
			toggleLine.onValueChanged.AddListener(OnValueChangedLine);
			toggleOrient.action = OnValueChangedOrient;
			buttonColor.onClick.AddListener(OnClickColor);
		}
	}
}
