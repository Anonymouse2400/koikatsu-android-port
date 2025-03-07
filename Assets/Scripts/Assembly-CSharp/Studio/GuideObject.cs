using System;
using System.Linq;
using Manager;
using UniRx;
using UnityEngine;

namespace Studio
{
	public class GuideObject : MonoBehaviour
	{
		public delegate void IsActiveFunc(bool _active);

		public enum Mode
		{
			Local = 0,
			LocalIK = 1,
			World = 2
		}

		public Transform transformTarget;

		private int m_DicKey = -1;

		protected ChangeAmount m_ChangeAmount;

		[SerializeField]
		protected GameObject[] roots = new GameObject[3];

		[SerializeField]
		protected GameObject objectSelect;

		[SerializeField]
		private GuideBase[] guide;

		[SerializeField]
		private GameObject m_objCenter;

		[SerializeField]
		private MeshRenderer rendererCenter;

		[SerializeField]
		protected bool[] m_Enables = new bool[4] { true, true, true, true };

		[SerializeField]
		private bool _calcScale = true;

		public IsActiveFunc isActiveFunc;

		protected bool m_IsActive;

		protected float m_ScaleRate = 1f;

		protected float m_ScaleRot = 1f;

		protected float m_ScaleSelect = 1f;

		public GuideObject parentGuide;

		protected BoolReactiveProperty _visible = new BoolReactiveProperty(true);

		private BoolReactiveProperty _visibleOutside = new BoolReactiveProperty(true);

		public Mode mode;

		public Transform parent;

		private GuideMove.MoveCalc _moveCalc;

		public bool nonconnect;

		public int dicKey
		{
			get
			{
				return m_DicKey;
			}
			set
			{
				if (Utility.SetStruct(ref m_DicKey, value))
				{
					changeAmount = Studio.GetChangeAmount(m_DicKey);
				}
			}
		}

		public ChangeAmount changeAmount
		{
			get
			{
				return m_ChangeAmount;
			}
			private set
			{
				m_ChangeAmount = value;
				if (m_ChangeAmount != null)
				{
					ChangeAmount obj = m_ChangeAmount;
					obj.onChangePos = (Action)Delegate.Combine(obj.onChangePos, new Action(CalcPosition));
					ChangeAmount obj2 = m_ChangeAmount;
					obj2.onChangeRot = (Action)Delegate.Combine(obj2.onChangeRot, new Action(CalcRotation));
					ChangeAmount obj3 = m_ChangeAmount;
					obj3.onChangeScale = (Action<Vector3>)Delegate.Combine(obj3.onChangeScale, new Action<Vector3>(CalcScale));
				}
			}
		}

		public GuideMove[] guideMove
		{
			get
			{
				return (from g in guide.Skip(1).Take(3)
					select g as GuideMove).ToArray();
			}
		}

		public GuideSelect guideSelect
		{
			get
			{
				return guide[11] as GuideSelect;
			}
		}

		public GameObject objCenter
		{
			get
			{
				return m_objCenter;
			}
		}

		public bool[] enables
		{
			get
			{
				return m_Enables;
			}
		}

		public bool enablePos
		{
			get
			{
				return m_Enables[0];
			}
			set
			{
				if (Utility.SetStruct(ref m_Enables[0], value))
				{
					roots[0].SetActive(isActive && m_Enables[0]);
				}
			}
		}

		public bool enableRot
		{
			get
			{
				return m_Enables[1];
			}
			set
			{
				if (Utility.SetStruct(ref m_Enables[1], value))
				{
					roots[1].SetActive(isActive && m_Enables[1]);
				}
			}
		}

		public bool enableScale
		{
			get
			{
				return m_Enables[2];
			}
			set
			{
				if (Utility.SetStruct(ref m_Enables[2], value))
				{
					roots[2].SetActive(isActive && m_Enables[2]);
				}
			}
		}

		public bool calcScale
		{
			get
			{
				return _calcScale;
			}
			set
			{
				_calcScale = value;
			}
		}

		public bool enableMaluti { get; set; }

		public bool isActive
		{
			get
			{
				return m_IsActive;
			}
			set
			{
				if (Utility.SetStruct(ref m_IsActive, value))
				{
					SetMode(GuideObjectManager.GetMode());
				}
			}
		}

		public float scaleRate
		{
			get
			{
				return m_ScaleRate;
			}
			set
			{
				if (Utility.SetStruct(ref m_ScaleRate, value))
				{
					SetScale();
				}
			}
		}

		public float scaleRot
		{
			get
			{
				return m_ScaleRot;
			}
			set
			{
				if (Utility.SetStruct(ref m_ScaleRot, value))
				{
					SetScale();
				}
			}
		}

		public float scaleSelect
		{
			get
			{
				return m_ScaleSelect;
			}
			set
			{
				if (Utility.SetStruct(ref m_ScaleSelect, value))
				{
					SetScale();
				}
			}
		}

		public bool isChild
		{
			get
			{
				return parentGuide != null;
			}
		}

		public int layer
		{
			get
			{
				return (!isChild) ? base.gameObject.layer : parentGuide.gameObject.layer;
			}
		}

		public bool visible
		{
			get
			{
				return _visible.Value;
			}
			set
			{
				_visible.Value = value;
			}
		}

		public bool visibleOutside
		{
			get
			{
				return _visibleOutside.Value;
			}
			set
			{
				_visibleOutside.Value = value;
			}
		}

		public bool visibleCenter
		{
			get
			{
				return rendererCenter.enabled;
			}
			set
			{
				rendererCenter.enabled = value;
			}
		}

		public bool visibleTranslation
		{
			set
			{
				foreach (GuideBase item in guide.Skip(13))
				{
					item.gameObject.SetActive(value);
				}
			}
		}

		public GuideMove.MoveCalc moveCalc
		{
			get
			{
				return _moveCalc;
			}
			set
			{
				_moveCalc = value;
				GuideMove[] array = this.guideMove;
				foreach (GuideMove guideMove in array)
				{
					guideMove.moveCalc = value;
				}
			}
		}

		private bool isQuit { get; set; }

		public void SetMode(int _mode, bool _layer = true)
		{
			for (int i = 0; i < 3; i++)
			{
				if (!(roots[i] == null))
				{
					roots[i].SetActive(isActive && m_Enables[i] && _mode == i);
				}
			}
			bool flag = (!isActive && m_Enables.Any((bool b) => b)) | (isActive && !m_Enables[_mode]);
			objectSelect.SetActive(flag);
			if (_layer)
			{
				SetLayer(base.gameObject, (!isChild) ? LayerMask.NameToLayer((!flag) ? "Studio/Select" : "Studio/Col") : layer);
				if (isActiveFunc != null)
				{
					isActiveFunc(flag);
				}
			}
		}

		public void SetActive(bool _active, bool _layer = true)
		{
			m_IsActive = _active;
			SetMode(GuideObjectManager.GetMode(), _layer);
		}

		private void CalcPosition()
		{
			if (m_Enables[0] && (bool)transformTarget)
			{
				if ((bool)parent && nonconnect)
				{
					transformTarget.position = parent.TransformPoint(changeAmount.pos);
				}
				else
				{
					transformTarget.localPosition = changeAmount.pos;
				}
			}
		}

		private void CalcRotation()
		{
			if (m_Enables[1] && (bool)transformTarget)
			{
				if ((bool)parent && nonconnect)
				{
					transformTarget.rotation = parent.rotation * Quaternion.Euler(changeAmount.rot);
				}
				else
				{
					transformTarget.localRotation = Quaternion.Euler(changeAmount.rot);
				}
			}
		}

		private void CalcScale(Vector3 _value)
		{
			if ((bool)transformTarget && (bool)parent && nonconnect)
			{
				transformTarget.localScale = changeAmount.scale;
			}
		}

		public void SetScale()
		{
			roots[0].transform.localScale = Vector3.one * m_ScaleRate * Studio.optionSystem.manipulateSize;
			roots[1].transform.localScale = Vector3.one * 15f * m_ScaleRate * 1.1f * m_ScaleRot * Studio.optionSystem.manipulateSize;
			roots[2].transform.localScale = Vector3.one * m_ScaleRate * Studio.optionSystem.manipulateSize;
			objectSelect.transform.localScale = Vector3.one * m_ScaleRate * m_ScaleSelect * Studio.optionSystem.manipulateSize;
			m_objCenter.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f) * m_ScaleRate * Studio.optionSystem.manipulateSize;
		}

		public void SetLayer(GameObject _object, int _layer)
		{
			if (!(_object == null))
			{
				_object.layer = _layer;
				Transform transform = _object.transform;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					SetLayer(transform.GetChild(i).gameObject, _layer);
				}
			}
		}

		public GuideCommand.EqualsInfo SetWorldPos(Vector3 _pos)
		{
			Vector3 pos = m_ChangeAmount.pos;
			if ((bool)parent && nonconnect)
			{
				m_ChangeAmount.pos = parent.InverseTransformPoint(_pos);
			}
			else
			{
				transformTarget.position = _pos;
				m_ChangeAmount.pos = transformTarget.localPosition;
			}
			GuideCommand.EqualsInfo equalsInfo = new GuideCommand.EqualsInfo();
			equalsInfo.dicKey = dicKey;
			equalsInfo.oldValue = pos;
			equalsInfo.newValue = m_ChangeAmount.pos;
			return equalsInfo;
		}

		public void MoveWorld(Vector3 _value)
		{
			if ((bool)parent && nonconnect)
			{
				Vector3 pos = m_ChangeAmount.pos;
				pos += parent.InverseTransformVector(_value);
				m_ChangeAmount.pos = pos;
			}
			else
			{
				Vector3 position = transformTarget.position;
				position += _value;
				transformTarget.position = position;
				m_ChangeAmount.pos = transformTarget.localPosition;
			}
		}

		public void MoveLocal(Vector3 _value, bool _snap, GuideMove.MoveAxis _axis)
		{
			switch (moveCalc)
			{
			case GuideMove.MoveCalc.TYPE1:
			{
				Vector3 pos2 = m_ChangeAmount.pos;
				pos2 += _value;
				m_ChangeAmount.pos = ((!_snap) ? pos2 : Parse(pos2, _axis));
				break;
			}
			case GuideMove.MoveCalc.TYPE2:
			case GuideMove.MoveCalc.TYPE3:
				if ((bool)parent && nonconnect)
				{
					Vector3 pos = m_ChangeAmount.pos;
					pos += parent.InverseTransformVector(_value);
					m_ChangeAmount.pos = ((!_snap) ? pos : Parse(pos, _axis));
				}
				else
				{
					Vector3 position = transformTarget.position;
					position += _value;
					transformTarget.position = ((!_snap) ? position : Parse(position, _axis));
					m_ChangeAmount.pos = transformTarget.localPosition;
				}
				break;
			}
		}

		private Vector3 Parse(Vector3 _src, GuideMove.MoveAxis _axis)
		{
			string text = string.Format("F{0}", 2 - Studio.optionSystem.snap);
			_src[(int)_axis] = float.Parse(_src[(int)_axis].ToString(text));
			return _src;
		}

		public void MoveLocal(Vector3 _value)
		{
			Vector3 pos = m_ChangeAmount.pos;
			pos += base.transform.InverseTransformVector(_value);
			m_ChangeAmount.pos = pos;
		}

		public void Rotation(Vector3 _axis, float _angle)
		{
			transformTarget.Rotate(_axis, _angle, Space.World);
			m_ChangeAmount.rot = transformTarget.localEulerAngles;
		}

		public void ForceUpdate()
		{
			CalcPosition();
			CalcRotation();
		}

		public void SetEnable(int _pos = -1, int _rot = -1, int _scale = -1)
		{
			if (_pos != -1)
			{
				m_Enables[0] = _pos == 1;
			}
			if (_rot != -1)
			{
				m_Enables[1] = _rot == 1;
			}
			if (_scale != -1)
			{
				m_Enables[2] = _scale == 1;
			}
			SetMode(GuideObjectManager.GetMode());
		}

		public void SetVisibleCenter(bool _value)
		{
			m_objCenter.SetActive(_value);
		}

		private void Awake()
		{
			m_DicKey = -1;
			isActiveFunc = null;
			parentGuide = null;
			enableMaluti = true;
			calcScale = true;
			visibleTranslation = Singleton<Studio>.Instance.workInfo.visibleAxisTranslation;
			visibleCenter = Singleton<Studio>.Instance.workInfo.visibleAxisCenter;
			SetVisibleCenter(false);
			Renderer component = objectSelect.GetComponent<Renderer>();
			if ((bool)component)
			{
				component.material.renderQueue = 3500;
			}
			_visible.Subscribe(delegate(bool _b)
			{
				GuideBase[] array = guide;
				foreach (GuideBase guideBase in array)
				{
					guideBase.draw = _b & visibleOutside;
				}
			});
			_visibleOutside.Subscribe(delegate(bool _b)
			{
				GuideBase[] array2 = guide;
				foreach (GuideBase guideBase2 in array2)
				{
					guideBase2.draw = _b & visible;
				}
			});
			for (int k = 0; k < guide.Length; k++)
			{
				guide[k].guideObject = this;
			}
		}

		private void Start()
		{
			isQuit = false;
			_visible.Value = true;
		}

		private void LateUpdate()
		{
			if ((bool)parent && nonconnect)
			{
				CalcPosition();
				CalcRotation();
			}
			base.transform.position = transformTarget.position;
			base.transform.rotation = transformTarget.rotation;
			switch (mode)
			{
			case Mode.Local:
				roots[0].transform.eulerAngles = ((!parent) ? Vector3.zero : parent.eulerAngles);
				break;
			case Mode.LocalIK:
				roots[0].transform.localEulerAngles = Vector3.zero;
				break;
			case Mode.World:
				roots[0].transform.eulerAngles = Vector3.zero;
				break;
			}
			if (calcScale)
			{
				Vector3 localScale = transformTarget.localScale;
				Vector3 lossyScale = transformTarget.lossyScale;
				Vector3 vector = ((!enableScale) ? Vector3.one : changeAmount.scale);
				transformTarget.localScale = new Vector3(localScale.x / lossyScale.x * vector.x, localScale.y / lossyScale.y * vector.y, localScale.z / lossyScale.z * vector.z);
			}
		}

		private void OnApplicationQuit()
		{
			isQuit = true;
		}

		private void OnDestroy()
		{
			if (!isQuit && !Scene.isGameEnd && Singleton<GuideObjectManager>.IsInstance())
			{
				Singleton<GuideObjectManager>.Instance.Delete(this, false);
			}
		}
	}
}
