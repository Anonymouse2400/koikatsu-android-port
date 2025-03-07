using UnityEngine;
using UnityEngine.EventSystems;

namespace HSceneUtility
{
	public class HSceneGuideMove : HSceneGuideBase, IInitializePotentialDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public enum MoveAxis
		{
			X = 0,
			Y = 1,
			Z = 2,
			XYZ = 3
		}

		public MoveAxis axis;

		private Vector2 oldPos = Vector2.zero;

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

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (!GlobalMethod.IsCameraActionFlag(base.guideObject.flags.ctrlCamera))
			{
				oldPos = eventData.pressPosition;
			}
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (!GlobalMethod.IsCameraActionFlag(base.guideObject.flags.ctrlCamera))
			{
				base.OnDrag(eventData);
				bool _snap = false;
				Vector3 vector = ((axis != MoveAxis.XYZ) ? AxisMove(eventData.delta, ref _snap) : (WorldPos(eventData.position) - WorldPos(oldPos)));
				Vector3 position = base.guideObject.amount.position;
				position += vector;
				base.guideObject.amount.position = ((axis == MoveAxis.XYZ) ? position : ((!_snap) ? position : Parse(position)));
				oldPos = eventData.position;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!GlobalMethod.IsCameraActionFlag(base.guideObject.flags.ctrlCamera))
			{
				GlobalMethod.SetCameraMoveFlag(base.guideObject.flags.ctrlCamera, false);
				base.guideObject.isDrag = true;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			GlobalMethod.SetCameraMoveFlag(base.guideObject.flags.ctrlCamera, true);
			base.guideObject.isDrag = false;
		}

		private Vector3 WorldPos(Vector2 _screenPos)
		{
			Plane plane = new Plane(camera.transform.forward * -1f, base.transform.position);
			Ray ray = RectTransformUtility.ScreenPointToRay(camera, _screenPos);
			float enter = 0f;
			return (!plane.Raycast(ray, out enter)) ? base.transform.position : ray.GetPoint(enter);
		}

		private Vector3 AxisPos(Vector2 _screenPos)
		{
			Vector3 position = base.transform.position;
			Plane plane = new Plane(base.transform.forward, position);
			if (!plane.GetSide(camera.transform.position))
			{
				plane = new Plane(base.transform.forward * -1f, position);
			}
			Vector3 up = base.transform.up;
			Ray ray = RectTransformUtility.ScreenPointToRay(camera, _screenPos);
			float enter = 0f;
			return (!plane.Raycast(ray, out enter)) ? Vector3.Project(position, up) : Vector3.Project(ray.GetPoint(enter), up);
		}

		private Vector3 AxisMove(Vector2 _delta, ref bool _snap)
		{
			Vector3 vector = camera.transform.TransformVector(_delta.x * 0.005f, _delta.y * 0.005f, 0f);
			Vector3 up = base.transform.up;
			return up * vector.magnitude * base.guideObject.speedMove * Vector3.Dot(vector.normalized, up);
		}

		private Vector3 Parse(Vector3 _src)
		{
			return _src;
		}
	}
}
