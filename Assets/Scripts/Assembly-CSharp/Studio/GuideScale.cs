using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class GuideScale : GuideBase, IInitializePotentialDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public enum ScaleAxis
		{
			X = 0,
			Y = 1,
			Z = 2,
			XYZ = 3
		}

		public ScaleAxis axis;

		private float speed = 0.001f;

		[SerializeField]
		private Transform transformRoot;

		private Vector2 prevPos = Vector2.zero;

		private Camera m_Camera;

		private Dictionary<int, Vector3> dicOldScale;

		private Dictionary<int, ChangeAmount> dicChangeAmount;

		private Camera camera
		{
			get
			{
				if (m_Camera == null)
				{
					m_Camera = Camera.main;
				}
				return m_Camera;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnInitializePotentialDrag(PointerEventData _eventData)
		{
			prevPos = _eventData.position;
			dicChangeAmount = Singleton<GuideObjectManager>.Instance.selectObjectDictionary;
			dicOldScale = dicChangeAmount.ToDictionary((KeyValuePair<int, ChangeAmount> v) => v.Key, (KeyValuePair<int, ChangeAmount> v) => v.Value.scale);
		}

		public override void OnDrag(PointerEventData _eventData)
		{
			base.OnDrag(_eventData);
			Vector3 zero = Vector3.zero;
			if (axis == ScaleAxis.XYZ)
			{
				Vector2 delta = _eventData.delta;
				float num = (delta.x + delta.y) * speed;
				zero = Vector3.one * num;
			}
			else
			{
				zero = AxisMove(_eventData.delta);
			}
			foreach (KeyValuePair<int, ChangeAmount> item in dicChangeAmount)
			{
				Vector3 scale = item.Value.scale;
				scale += zero;
				scale.x = Mathf.Max(scale.x, 0.01f);
				scale.y = Mathf.Max(scale.y, 0.01f);
				scale.z = Mathf.Max(scale.z, 0.01f);
				item.Value.scale = scale;
			}
		}

		public void OnPointerUp(PointerEventData _eventData)
		{
		}

		public override void OnEndDrag(PointerEventData _eventData)
		{
			base.OnEndDrag(_eventData);
			GuideCommand.EqualsInfo[] changeAmountInfo = Singleton<GuideObjectManager>.Instance.selectObjectKey.Select((int v) => new GuideCommand.EqualsInfo
			{
				dicKey = v,
				oldValue = dicOldScale[v],
				newValue = dicChangeAmount[v].scale
			}).ToArray();
			Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.ScaleEqualsCommand(changeAmountInfo));
		}

		private Vector3 AxisPos(Vector2 _screenPos)
		{
			Vector3 position = base.transform.position;
			Plane plane = new Plane(camera.transform.forward * -1f, position);
			Ray ray = RectTransformUtility.ScreenPointToRay(camera, _screenPos);
			float enter = 0f;
			Vector3 vector = ((!plane.Raycast(ray, out enter)) ? position : ray.GetPoint(enter));
			Vector3 vector2 = vector - position;
			Vector3 onNormal = base.transform.up;
			switch (axis)
			{
			case ScaleAxis.X:
				onNormal = Vector3.right;
				break;
			case ScaleAxis.Y:
				onNormal = Vector3.up;
				break;
			case ScaleAxis.Z:
				onNormal = Vector3.forward;
				break;
			}
			return Vector3.Project(vector2, onNormal);
		}

		private Vector3 AxisMove(Vector2 _delta)
		{
			Vector3 vector = camera.transform.TransformVector(_delta.x * 0.005f, _delta.y * 0.005f, 0f);
			vector *= Studio.optionSystem.manipuleteSpeed;
			vector = base.transform.InverseTransformVector(vector);
			switch (axis)
			{
			case ScaleAxis.X:
				vector = Vector3.Scale(vector, Vector3.right);
				break;
			case ScaleAxis.Y:
				vector = Vector3.Scale(vector, Vector3.up);
				break;
			case ScaleAxis.Z:
				vector = Vector3.Scale(vector, Vector3.forward);
				break;
			}
			return vector;
		}
	}
}
