using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class GuideRotation : GuideBase, IInitializePotentialDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public enum RotationAxis
		{
			X = 0,
			Y = 1,
			Z = 2,
			XYZ = 3
		}

		public RotationAxis axis;

		private Vector2 prevScreenPos = Vector2.zero;

		private Vector3 prevPlanePos = Vector3.zero;

		private Dictionary<int, Vector3> dicOldRot;

		private Dictionary<int, ChangeAmount> dicChangeAmount;

		private Camera m_Camera;

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
			prevScreenPos = _eventData.position;
			prevPlanePos = PlanePos(_eventData.position);
			dicChangeAmount = Singleton<GuideObjectManager>.Instance.selectObjectDictionary;
			dicOldRot = dicChangeAmount.ToDictionary((KeyValuePair<int, ChangeAmount> v) => v.Key, (KeyValuePair<int, ChangeAmount> v) => v.Value.rot);
		}

		public override void OnDrag(PointerEventData _eventData)
		{
			base.OnDrag(_eventData);
			RotationAxis rotationAxis = axis;
			if (rotationAxis == RotationAxis.XYZ)
			{
				GuideObject[] selectObjects = Singleton<GuideObjectManager>.Instance.selectObjects;
				foreach (GuideObject guideObject in selectObjects)
				{
					guideObject.Rotation(camera.transform.up, 0f - _eventData.delta.x);
					guideObject.Rotation(camera.transform.right, _eventData.delta.y);
				}
				return;
			}
			Vector3 zero = Vector3.zero;
			float f = Vector3.Dot(camera.transform.forward, base.transform.right);
			if (Mathf.Abs(f) > 0.1f)
			{
				Vector3 position = PlanePos(_eventData.position);
				Vector3 vector = Quaternion.Euler(0f, 90f, 0f) * base.transform.InverseTransformPoint(prevPlanePos);
				Vector3 vector2 = Quaternion.Euler(0f, 90f, 0f) * base.transform.InverseTransformPoint(position);
				float value = VectorToAngle(new Vector2(vector.x, vector.y), new Vector2(vector2.x, vector2.y));
				zero[(int)axis] = value;
				prevPlanePos = position;
			}
			else
			{
				Vector3 position2 = _eventData.position;
				position2.z = Vector3.Distance(prevPlanePos, camera.transform.position);
				Vector3 position3 = prevScreenPos;
				position3.z = Vector3.Distance(prevPlanePos, camera.transform.position);
				Vector3 vector3 = camera.ScreenToWorldPoint(position2) - camera.ScreenToWorldPoint(position3);
				Vector3 position4 = prevPlanePos + vector3;
				Vector3 vector4 = Quaternion.Euler(0f, 90f, 0f) * base.transform.InverseTransformPoint(prevPlanePos);
				Vector3 vector5 = Quaternion.Euler(0f, 90f, 0f) * base.transform.InverseTransformPoint(position4);
				prevPlanePos = position4;
				float value2 = VectorToAngle(new Vector2(vector4.x, vector4.y), new Vector2(vector5.x, vector5.y));
				zero[(int)axis] = value2;
				prevPlanePos = position4;
			}
			prevScreenPos = _eventData.position;
			foreach (KeyValuePair<int, ChangeAmount> item in dicChangeAmount)
			{
				Vector3 eulerAngles = (Quaternion.Euler(item.Value.rot) * Quaternion.Euler(zero)).eulerAngles;
				eulerAngles.x %= 360f;
				eulerAngles.y %= 360f;
				eulerAngles.z %= 360f;
				item.Value.rot = eulerAngles;
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
				oldValue = dicOldRot[v],
				newValue = dicChangeAmount[v].rot
			}).ToArray();
			Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.RotationEqualsCommand(changeAmountInfo));
		}

		private Vector3 PlanePos(Vector2 _screenPos)
		{
			Plane plane = new Plane(base.transform.right, base.transform.position);
			if (!plane.GetSide(camera.transform.position))
			{
				plane.SetNormalAndPosition(base.transform.right * -1f, base.transform.position);
			}
			Ray ray = RectTransformUtility.ScreenPointToRay(camera, _screenPos);
			float enter = 0f;
			return (!plane.Raycast(ray, out enter)) ? base.transform.position : ray.GetPoint(enter);
		}

		private float VectorToAngle(Vector2 _v1, Vector2 _v2)
		{
			float current = Mathf.Atan2(_v1.x, _v1.y) * 57.29578f;
			float target = Mathf.Atan2(_v2.x, _v2.y) * 57.29578f;
			return Mathf.DeltaAngle(current, target);
		}
	}
}
