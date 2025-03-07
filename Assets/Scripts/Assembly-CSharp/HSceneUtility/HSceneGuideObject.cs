using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HSceneUtility
{
	public class HSceneGuideObject : MonoBehaviour
	{
		public class Amount
		{
			public Vector3 position = Vector3.zero;

			public Vector3 rotation = Vector3.zero;
		}

		public Amount amount = new Amount();

		public bool isDrag;

		[Range(0.01f, 3f)]
		public float scaleAxis = 2f;

		[Range(0.01f, 3f)]
		public float speedMove = 1f;

		public HFlag flags;

		[SerializeField]
		protected HSceneSpriteObjectCategory roots;

		protected float m_ScaleRate = 0.5f;

		protected float m_ScaleRot = 0.05f;

		protected float m_ScaleSelect = 0.1f;

		public float scaleRate
		{
			get
			{
				return m_ScaleRate;
			}
			set
			{
				if (m_ScaleRate != value)
				{
					m_ScaleRate = value;
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
				if (m_ScaleRot != value)
				{
					m_ScaleRot = value;
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
				if (m_ScaleSelect != value)
				{
					m_ScaleSelect = value;
					SetScale();
				}
			}
		}

		public void SetMode(int _mode)
		{
			roots.SetActiveToggle(_mode);
		}

		public void SetScale()
		{
			roots.GetObject(0).transform.localScale = Vector3.one * m_ScaleRate * scaleAxis;
			roots.GetObject(1).transform.localScale = Vector3.one * 15f * m_ScaleRate * 1.1f * m_ScaleRot * scaleAxis;
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

		private void Awake()
		{
			List<HSceneGuideBase> list = new List<HSceneGuideBase>();
			for (int i = 0; i < 2; i++)
			{
				list.AddRange(roots.GetObject(i).GetComponentsInChildren<HSceneGuideBase>().ToList());
			}
			HSceneGuideBase[] array = list.ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				array[j].guideObject = this;
			}
			SetMode(0);
			SetScale();
		}

		private void LateUpdate()
		{
			base.transform.localPosition = base.transform.InverseTransformVector(amount.position);
			roots.GetObject(1).SafeProcObject(delegate(GameObject o)
			{
				o.transform.localRotation = Quaternion.Euler(amount.rotation);
			});
		}
	}
}
