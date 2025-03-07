using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class Item
	{
		[Serializable]
		public class Data : AssetBundleData
		{
			public enum Type
			{
				None = 0,
				Head = 1,
				Neck = 2,
				LeftHand = 3,
				RightHand = 4,
				LeftFoot = 5,
				RightFoot = 6,
				a_n_headside = 7,
				k_f_handL_00 = 8,
				k_f_handR_00 = 9,
				chara = 10,
				k_f_shoulderL_00 = 11,
				k_f_shoulderR_00 = 12
			}

			public Type type;

			public Vector3 offsetPos = Vector3.zero;

			public Vector3 offsetAngle = Vector3.zero;

			public Motion motion = new Motion();

			public static Transform GetParent(Type type, ChaControl chaCtrl)
			{
				switch (type)
				{
				case Type.Head:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.HeadParent).transform;
				case Type.Neck:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.a_n_neck).transform;
				case Type.LeftHand:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.a_n_hand_L).transform;
				case Type.RightHand:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.a_n_hand_R).transform;
				case Type.LeftFoot:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.a_n_knee_L).transform;
				case Type.RightFoot:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.a_n_knee_R).transform;
				case Type.a_n_headside:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.a_n_headside).transform;
				case Type.k_f_handL_00:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.k_f_handL_00).transform;
				case Type.k_f_handR_00:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.k_f_handR_00).transform;
				case Type.chara:
					return chaCtrl.objTop.transform;
				case Type.k_f_shoulderL_00:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.k_f_shoulderL_00).transform;
				case Type.k_f_shoulderR_00:
					return chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.k_f_shoulderR_00).transform;
				default:
					return null;
				}
			}

			public GameObject Load(ChaControl chaCtrl)
			{
				GameObject gameObject = LoadModel(chaCtrl);
				Animator component = gameObject.GetComponent<Animator>();
				if (component != null)
				{
					motion.LoadAnimator(component);
				}
				return gameObject;
			}

			private GameObject LoadModel(ChaControl chaCtrl)
			{
				Transform transform = GetParent(type, chaCtrl);
				if (transform == null)
				{
					transform = chaCtrl.transform.root;
				}
				GameObject gameObject = GetAsset<GameObject>();
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform, false);
				gameObject2.transform.localPosition += offsetPos;
				gameObject2.transform.localEulerAngles += offsetAngle;
				gameObject2.name = gameObject.name;
				UnloadBundle();
				return gameObject2;
			}
		}

		public Data[] data = new Data[0];

		[SerializeField]
		private List<GameObject> _itemObjectList = new List<GameObject>();

		public List<GameObject> itemObjectList
		{
			get
			{
				return _itemObjectList;
			}
		}

		public void Visible(bool visible)
		{
			_itemObjectList.ForEach(delegate(GameObject item)
			{
				item.SetActive(visible);
			});
		}

		public void Setting(ChaControl chaCtrl, bool isItemClear = true)
		{
			if (isItemClear)
			{
				_itemObjectList.ForEach(delegate(GameObject item)
				{
					UnityEngine.Object.Destroy(item);
				});
				_itemObjectList.Clear();
			}
			_itemObjectList.AddRange(data.Select((Data item) => item.Load(chaCtrl)));
		}
	}
}
