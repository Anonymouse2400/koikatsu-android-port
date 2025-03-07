using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class GuideObjectManager : Singleton<GuideObjectManager>
	{
		[SerializeField]
		private GameObject objectOriginal;

		[SerializeField]
		private GuideInput m_GuideInput;

		[SerializeField]
		private Transform transformWorkplace;

		[SerializeField]
		private DrawLightLine m_DrawLightLine;

		private HashSet<GuideObject> hashSelectObject = new HashSet<GuideObject>();

		private int m_Mode;

		private Dictionary<Transform, GuideObject> dicGuideObject = new Dictionary<Transform, GuideObject>();

		private Dictionary<Transform, Light> dicTransLight = new Dictionary<Transform, Light>();

		private Dictionary<GuideObject, Light> dicGuideLight = new Dictionary<GuideObject, Light>();

		public GuideInput guideInput
		{
			get
			{
				return m_GuideInput;
			}
		}

		public DrawLightLine drawLightLine
		{
			get
			{
				return m_DrawLightLine;
			}
		}

		public GuideObject selectObject
		{
			get
			{
				return (hashSelectObject.Count == 0) ? null : hashSelectObject.ToArray()[0];
			}
			set
			{
				SetSelectObject(value);
			}
		}

		public GuideObject deselectObject
		{
			set
			{
				SetDeselectObject(value);
			}
		}

		public GuideObject[] selectObjects
		{
			get
			{
				return (hashSelectObject.Count == 0) ? null : hashSelectObject.ToArray();
			}
		}

		public ChangeAmount[] selectObjectChangeAmount
		{
			get
			{
				return hashSelectObject.Select((GuideObject v) => v.changeAmount).ToArray();
			}
		}

		public int[] selectObjectKey
		{
			get
			{
				return hashSelectObject.Select((GuideObject v) => v.dicKey).ToArray();
			}
		}

		public Dictionary<int, ChangeAmount> selectObjectDictionary
		{
			get
			{
				return hashSelectObject.ToDictionary((GuideObject v) => v.dicKey, (GuideObject v) => v.changeAmount);
			}
		}

		public GuideObject operationTarget { get; set; }

		public bool isOperationTarget
		{
			get
			{
				return operationTarget != null;
			}
		}

		public int mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				if (Utility.SetStruct(ref m_Mode, value))
				{
					SetMode(m_Mode);
					if (this.ModeChangeEvent != null)
					{
						this.ModeChangeEvent(this, EventArgs.Empty);
					}
				}
			}
		}

		public event EventHandler ModeChangeEvent;

		public static int GetMode()
		{
			if (!Singleton<GuideObjectManager>.IsInstance())
			{
				return 0;
			}
			return Singleton<GuideObjectManager>.Instance.mode;
		}

		public GuideObject Add(Transform _target, int _dicKey)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(objectOriginal);
			gameObject.transform.SetParent(transformWorkplace);
			GuideObject component = gameObject.GetComponent<GuideObject>();
			component.transformTarget = _target;
			component.dicKey = _dicKey;
			dicGuideObject.Add(_target, component);
			Light component2 = _target.GetComponent<Light>();
			if ((bool)component2 && component2.type != LightType.Directional)
			{
				dicTransLight.Add(_target, component2);
			}
			return component;
		}

		public void Delete(GuideObject _object, bool _destroy = true)
		{
			if (!(_object == null))
			{
				if (hashSelectObject.Contains(_object))
				{
					SetSelectObject(null, false);
				}
				dicGuideObject.Remove(_object.transformTarget);
				dicTransLight.Remove(_object.transformTarget);
				dicGuideLight.Remove(_object);
				if (_destroy)
				{
					UnityEngine.Object.DestroyImmediate(_object.gameObject);
				}
				if (operationTarget == _object)
				{
					operationTarget = null;
				}
			}
		}

		public void DeleteAll()
		{
			hashSelectObject.Clear();
			operationTarget = null;
			GameObject[] array = (from v in dicGuideObject
				where v.Value != null
				select v.Value.gameObject).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if ((bool)array[i])
				{
					UnityEngine.Object.DestroyImmediate(array[i]);
				}
			}
			dicGuideObject.Clear();
			dicTransLight.Clear();
			dicGuideLight.Clear();
			drawLightLine.Clear();
			guideInput.Stop();
		}

		public void AddSelectMultiple(GuideObject _object)
		{
			if (!(_object == null) && !hashSelectObject.Contains(_object) && (hashSelectObject.Count == 0 || _object.enableMaluti))
			{
				AddObject(_object, hashSelectObject.Count == 0);
				guideInput.AddSelectMultiple(_object);
			}
		}

		public void SetScale()
		{
			foreach (KeyValuePair<Transform, GuideObject> item in dicGuideObject)
			{
				item.Value.SetScale();
			}
		}

		public void SetVisibleTranslation()
		{
			bool visibleAxisTranslation = Singleton<Studio>.Instance.workInfo.visibleAxisTranslation;
			foreach (KeyValuePair<Transform, GuideObject> item in dicGuideObject)
			{
				item.Value.visibleTranslation = visibleAxisTranslation;
			}
		}

		public void SetVisibleCenter()
		{
			bool visibleAxisCenter = Singleton<Studio>.Instance.workInfo.visibleAxisCenter;
			foreach (KeyValuePair<Transform, GuideObject> item in dicGuideObject)
			{
				item.Value.visibleCenter = visibleAxisCenter;
			}
		}

		private void SetMode(int _mode)
		{
			foreach (GuideObject item in hashSelectObject)
			{
				item.SetMode(_mode);
			}
		}

		private void SetSelectObject(GuideObject _object, bool _multiple = true)
		{
			bool flag = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			bool key = Input.GetKey(KeyCode.X);
			if (_multiple && flag && !key)
			{
				if (_object == null || hashSelectObject.Contains(_object) || (hashSelectObject.Count != 0 && !_object.enableMaluti))
				{
					return;
				}
				AddObject(_object, hashSelectObject.Count == 0);
			}
			else
			{
				switch (Studio.optionSystem.selectedState)
				{
				case 0:
					StopSelectObject();
					break;
				case 1:
				{
					GuideObject guideObject = selectObject;
					if (guideObject == null)
					{
						break;
					}
					if (!guideObject.isChild)
					{
						if ((bool)_object && _object.isChild)
						{
							guideObject.SetActive(false, false);
						}
						else
						{
							StopSelectObject();
						}
					}
					else
					{
						guideObject.SetActive(false, false);
					}
					break;
				}
				}
				hashSelectObject.Clear();
				if ((bool)_object && !_object.enables[m_Mode])
				{
					for (int i = 0; i < 3; i++)
					{
						if (_object.enables[i])
						{
							mode = i;
							break;
						}
					}
				}
				AddObject(_object);
			}
			guideInput.guideObject = _object;
		}

		private void SetDeselectObject(GuideObject _object)
		{
			if (!(_object == null))
			{
				bool isActive = _object.isActive;
				_object.isActive = false;
				Light value = null;
				if (dicTransLight.TryGetValue(_object.transformTarget, out value))
				{
					drawLightLine.Remove(value);
					dicGuideLight.Remove(_object);
				}
				hashSelectObject.Remove(_object);
				guideInput.deselectObject = _object;
				if (hashSelectObject.Count > 0 && isActive)
				{
					selectObject.isActive = true;
				}
			}
		}

		private void StopSelectObject()
		{
			foreach (GuideObject item in hashSelectObject)
			{
				item.isActive = false;
				Light value = null;
				if (dicGuideLight.TryGetValue(item, out value))
				{
					drawLightLine.Remove(value);
					dicGuideLight.Remove(item);
				}
			}
		}

		private void AddObject(GuideObject _object, bool _active = true)
		{
			if (!(_object == null))
			{
				if (_active)
				{
					_object.isActive = true;
				}
				Light value = null;
				if (dicTransLight.TryGetValue(_object.transformTarget, out value))
				{
					drawLightLine.Add(value);
					dicGuideLight.Add(_object, value);
				}
				hashSelectObject.Add(_object);
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				if (transformWorkplace == null)
				{
					transformWorkplace = base.transform;
				}
				operationTarget = null;
			}
		}
	}
}
