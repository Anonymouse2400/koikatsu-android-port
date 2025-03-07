using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class GuideMove : GuideBase, IInitializePotentialDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public enum MoveAxis
		{
			X = 0,
			Y = 1,
			Z = 2,
			XYZ = 3,
			XY = 4,
			YZ = 5,
			XZ = 6
		}

		public enum MoveCalc
		{
			TYPE1 = 0,
			TYPE2 = 1,
			TYPE3 = 2
		}

		public MoveAxis axis;

		[SerializeField]
		private Transform transformRoot;

		public MoveCalc moveCalc;

		public Action onDragAction;

		public Action onEndDragAction;

		private Vector2 oldPos = Vector2.zero;

		private Camera m_Camera;

		private Dictionary<int, Vector3> dicOld;

		private bool isSnap = true;

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

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			oldPos = eventData.pressPosition;
			Dictionary<int, ChangeAmount> selectObjectDictionary = Singleton<GuideObjectManager>.Instance.selectObjectDictionary;
			dicOld = selectObjectDictionary.ToDictionary((KeyValuePair<int, ChangeAmount> v) => v.Key, (KeyValuePair<int, ChangeAmount> v) => v.Value.pos);
			isSnap = true;
		}

		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);
			switch (axis)
			{
			case MoveAxis.XYZ:
			{
				Vector3 value2 = WorldPos(eventData.position) - WorldPos(oldPos);
				GuideObject[] selectObjects2 = Singleton<GuideObjectManager>.Instance.selectObjects;
				foreach (GuideObject guideObject2 in selectObjects2)
				{
					guideObject2.MoveWorld(value2);
				}
				break;
			}
			case MoveAxis.X:
			case MoveAxis.Y:
			case MoveAxis.Z:
			{
				bool _snap = false;
				Vector3 value3 = AxisMove(eventData.delta, ref _snap);
				GuideObject[] selectObjects3 = Singleton<GuideObjectManager>.Instance.selectObjects;
				foreach (GuideObject guideObject3 in selectObjects3)
				{
					guideObject3.MoveLocal(value3, _snap, axis);
				}
				break;
			}
			case MoveAxis.XY:
			case MoveAxis.YZ:
			case MoveAxis.XZ:
			{
				Vector3 value = PlanePos(eventData.position) - PlanePos(oldPos);
				GuideObject[] selectObjects = Singleton<GuideObjectManager>.Instance.selectObjects;
				foreach (GuideObject guideObject in selectObjects)
				{
					guideObject.MoveWorld(value);
				}
				break;
			}
			}
			oldPos = eventData.position;
			if (onDragAction != null)
			{
				onDragAction();
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			GuideCommand.EqualsInfo[] changeAmountInfo = Singleton<GuideObjectManager>.Instance.selectObjectDictionary.Select((KeyValuePair<int, ChangeAmount> v) => new GuideCommand.EqualsInfo
			{
				dicKey = v.Key,
				oldValue = dicOld[v.Key],
				newValue = v.Value.pos
			}).ToArray();
			Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(changeAmountInfo));
			if (onEndDragAction != null)
			{
				onEndDragAction();
			}
		}

		private Vector3 WorldPos(Vector2 _screenPos)
		{
			Plane plane = new Plane(camera.transform.forward * -1f, base.transform.position);
			Ray ray = RectTransformUtility.ScreenPointToRay(camera, _screenPos);
			float enter = 0f;
			return (!plane.Raycast(ray, out enter)) ? base.transform.position : ray.GetPoint(enter);
		}

		private Vector3 PlanePos(Vector2 _screenPos)
		{
			Plane plane = new Plane(base.transform.up, base.transform.position);
			Ray ray = RectTransformUtility.ScreenPointToRay(Camera.main, _screenPos);
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
			if (Input.GetKey(KeyCode.V) && vector[(int)axis] != 0f)
			{
				float num = ((!(vector[(int)axis] < 0f)) ? 1f : (-1f));
				vector = Vector3.zero;
				if (isSnap)
				{
					int num2 = Mathf.Clamp(Studio.optionSystem.snap, 0, 2);
					float[] array = new float[3] { 0.01f, 0.1f, 1f };
					vector[(int)axis] = array[num2] * num;
					isSnap = false;
					Observable.Timer(TimeSpan.FromMilliseconds(50.0)).Subscribe(delegate
					{
						isSnap = true;
					}).AddTo(this);
					_snap = true;
				}
			}
			else
			{
				vector *= Studio.optionSystem.manipuleteSpeed;
			}
			switch (axis)
			{
			case MoveAxis.X:
				vector = ((moveCalc != MoveCalc.TYPE3) ? Vector3.Scale(transformRoot.right, vector) : transformRoot.TransformVector(Vector3.Scale(Vector3.right, transformRoot.InverseTransformVector(vector))));
				break;
			case MoveAxis.Y:
				vector = ((moveCalc != MoveCalc.TYPE3) ? Vector3.Scale(transformRoot.up, vector) : transformRoot.TransformVector(Vector3.Scale(Vector3.up, transformRoot.InverseTransformVector(vector))));
				break;
			case MoveAxis.Z:
				vector = ((moveCalc != MoveCalc.TYPE3) ? Vector3.Scale(transformRoot.forward, vector) : transformRoot.TransformVector(Vector3.Scale(Vector3.forward, transformRoot.InverseTransformVector(vector))));
				break;
			}
			return vector;
		}

		private Vector3 Parse(Vector3 _src)
		{
			string text = string.Format("F{0}", 2 - Studio.optionSystem.snap);
			_src[(int)axis] = float.Parse(_src[(int)axis].ToString(text));
			return _src;
		}

		private void CalcType1(KeyValuePair<ChangeAmount, Transform> _pair, Vector3 _move, bool _snap)
		{
			Vector3 pos = _pair.Key.pos;
			pos += _move;
			_pair.Key.pos = ((!_snap) ? pos : Parse(pos));
		}

		private void CalcType2(KeyValuePair<ChangeAmount, Transform> _pair, Vector3 _move, bool _snap)
		{
			Vector3 pos = _pair.Key.pos;
			pos += _pair.Value.InverseTransformVector(_move);
			_pair.Key.pos = ((!_snap) ? pos : Parse(pos));
		}
	}
}
