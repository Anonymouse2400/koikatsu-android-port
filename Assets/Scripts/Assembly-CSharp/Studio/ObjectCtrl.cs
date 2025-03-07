using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class ObjectCtrl : Singleton<ObjectCtrl>
	{
		[SerializeField]
		private MapDragButton[] mapDragButton;

		[SerializeField]
		private Transform transformBase;

		private Dictionary<int, Vector3> dicOld;

		public bool active
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				base.gameObject.SetActive(value);
			}
		}

		private void OnBeginDragTrans()
		{
			Dictionary<int, ChangeAmount> selectObjectDictionary = Singleton<GuideObjectManager>.Instance.selectObjectDictionary;
			dicOld = selectObjectDictionary.ToDictionary((KeyValuePair<int, ChangeAmount> v) => v.Key, (KeyValuePair<int, ChangeAmount> v) => v.Value.pos);
		}

		private void OnEndDragTrans()
		{
			GuideCommand.EqualsInfo[] changeAmountInfo = Singleton<GuideObjectManager>.Instance.selectObjectDictionary.Select((KeyValuePair<int, ChangeAmount> v) => new GuideCommand.EqualsInfo
			{
				dicKey = v.Key,
				oldValue = dicOld[v.Key],
				newValue = v.Value.pos
			}).ToArray();
			Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(changeAmountInfo));
		}

		private void OnDragTransXZ()
		{
			Vector3 vector = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));
			vector *= 0.005f;
			vector = Camera.main.transform.TransformVector(vector);
			vector.y = 0f;
			foreach (GuideObject item in Singleton<GuideObjectManager>.Instance.selectObjects.Where((GuideObject v) => v.enablePos))
			{
				item.MoveLocal(vector);
			}
		}

		private void OnDragTransY()
		{
			Vector3 vector = new Vector3(0f, Input.GetAxis("Mouse Y"), 0f);
			vector *= 0.005f;
			vector = Camera.main.transform.TransformVector(vector);
			vector.x = 0f;
			vector.z = 0f;
			foreach (GuideObject item in Singleton<GuideObjectManager>.Instance.selectObjects.Where((GuideObject v) => v.enablePos))
			{
				item.MoveLocal(vector);
			}
		}

		private void OnBeginDragRot()
		{
			Dictionary<int, ChangeAmount> selectObjectDictionary = Singleton<GuideObjectManager>.Instance.selectObjectDictionary;
			dicOld = selectObjectDictionary.ToDictionary((KeyValuePair<int, ChangeAmount> v) => v.Key, (KeyValuePair<int, ChangeAmount> v) => v.Value.rot);
		}

		private void OnEndDragRot()
		{
			GuideCommand.EqualsInfo[] changeAmountInfo = Singleton<GuideObjectManager>.Instance.selectObjectDictionary.Select((KeyValuePair<int, ChangeAmount> v) => new GuideCommand.EqualsInfo
			{
				dicKey = v.Key,
				oldValue = dicOld[v.Key],
				newValue = v.Value.rot
			}).ToArray();
			Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.RotationEqualsCommand(changeAmountInfo));
		}

		private void OnDragRotX()
		{
			float axis = Input.GetAxis("Mouse Y");
			Vector3 right = transformBase.right;
			foreach (GuideObject item in Singleton<GuideObjectManager>.Instance.selectObjects.Where((GuideObject v) => v.enableRot))
			{
				item.Rotation(right, axis);
			}
		}

		private void OnDragRotY()
		{
			float angle = Input.GetAxis("Mouse X") * -1f;
			Vector3 up = transformBase.up;
			foreach (GuideObject item in Singleton<GuideObjectManager>.Instance.selectObjects.Where((GuideObject v) => v.enableRot))
			{
				item.Rotation(up, angle);
			}
		}

		private void OnDragRotZ()
		{
			float angle = Input.GetAxis("Mouse X") * -1f;
			Vector3 forward = transformBase.forward;
			foreach (GuideObject item in Singleton<GuideObjectManager>.Instance.selectObjects.Where((GuideObject v) => v.enableRot))
			{
				item.Rotation(forward, angle);
			}
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				MapDragButton obj = mapDragButton[0];
				obj.onBeginDragFunc = (Action)Delegate.Combine(obj.onBeginDragFunc, new Action(OnBeginDragTrans));
				MapDragButton obj2 = mapDragButton[0];
				obj2.onDragFunc = (Action)Delegate.Combine(obj2.onDragFunc, new Action(OnDragTransXZ));
				MapDragButton obj3 = mapDragButton[0];
				obj3.onEndDragFunc = (Action)Delegate.Combine(obj3.onEndDragFunc, new Action(OnEndDragTrans));
				MapDragButton obj4 = mapDragButton[1];
				obj4.onBeginDragFunc = (Action)Delegate.Combine(obj4.onBeginDragFunc, new Action(OnBeginDragTrans));
				MapDragButton obj5 = mapDragButton[1];
				obj5.onDragFunc = (Action)Delegate.Combine(obj5.onDragFunc, new Action(OnDragTransY));
				MapDragButton obj6 = mapDragButton[1];
				obj6.onEndDragFunc = (Action)Delegate.Combine(obj6.onEndDragFunc, new Action(OnEndDragTrans));
				for (int i = 0; i < 3; i++)
				{
					MapDragButton obj7 = mapDragButton[2 + i];
					obj7.onBeginDragFunc = (Action)Delegate.Combine(obj7.onBeginDragFunc, new Action(OnBeginDragRot));
					MapDragButton obj8 = mapDragButton[2 + i];
					obj8.onEndDragFunc = (Action)Delegate.Combine(obj8.onEndDragFunc, new Action(OnEndDragRot));
				}
				MapDragButton obj9 = mapDragButton[2];
				obj9.onDragFunc = (Action)Delegate.Combine(obj9.onDragFunc, new Action(OnDragRotX));
				MapDragButton obj10 = mapDragButton[3];
				obj10.onDragFunc = (Action)Delegate.Combine(obj10.onDragFunc, new Action(OnDragRotY));
				MapDragButton obj11 = mapDragButton[4];
				obj11.onDragFunc = (Action)Delegate.Combine(obj11.onDragFunc, new Action(OnDragRotZ));
			}
		}
	}
}
