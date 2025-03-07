using System.Collections.Generic;
using IllusionUtility.GetUtility;
using UnityEngine;

namespace Studio
{
	public class ItemFKCtrl : MonoBehaviour
	{
		private class TargetInfo
		{
			public GameObject gameObject;

			private Transform m_Transform;

			public ChangeAmount changeAmount;

			private Vector3 baseRot = Vector3.zero;

			public Transform transform
			{
				get
				{
					if (m_Transform == null)
					{
						m_Transform = gameObject.transform;
					}
					return m_Transform;
				}
			}

			public TargetInfo(GameObject _gameObject, ChangeAmount _changeAmount, bool _new)
			{
				gameObject = _gameObject;
				changeAmount = _changeAmount;
				if (_new)
				{
					CopyBone();
				}
				baseRot = transform.localEulerAngles;
			}

			public void CopyBone()
			{
				changeAmount.rot = transform.localEulerAngles;
			}

			public void CopyBase()
			{
				transform.localEulerAngles = baseRot;
			}

			public void Update()
			{
				transform.localRotation = Quaternion.Euler(changeAmount.rot);
			}
		}

		private List<TargetInfo> listBones = new List<TargetInfo>();

		private int count { get; set; }

		public void InitBone(OCIItem _ociItem, Info.ItemLoadInfo _loadInfo, bool _isNew)
		{
			Transform transform = _ociItem.objectItem.transform;
			_ociItem.listBones = new List<OCIChar.BoneInfo>();
			foreach (string bone in _loadInfo.bones)
			{
				GameObject gameObject = transform.FindLoop(bone);
				if (!(gameObject == null))
				{
					OIBoneInfo value = null;
					if (!_ociItem.itemInfo.bones.TryGetValue(bone, out value))
					{
						value = new OIBoneInfo(Studio.GetNewIndex());
						_ociItem.itemInfo.bones.Add(bone, value);
					}
					GuideObject guideObject = Singleton<GuideObjectManager>.Instance.Add(gameObject.transform, value.dicKey);
					guideObject.enablePos = false;
					guideObject.enableScale = false;
					guideObject.enableMaluti = false;
					guideObject.calcScale = false;
					guideObject.scaleRate = 0.5f;
					guideObject.scaleRot = 0.025f;
					guideObject.scaleSelect = 0.05f;
					guideObject.parentGuide = _ociItem.guideObject;
					_ociItem.listBones.Add(new OCIChar.BoneInfo(guideObject, value, -1));
					guideObject.SetActive(false);
					listBones.Add(new TargetInfo(gameObject, value.changeAmount, _isNew));
				}
			}
			count = listBones.Count;
		}

		private void OnDisable()
		{
			for (int i = 0; i < count; i++)
			{
				listBones[i].CopyBase();
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < count; i++)
			{
				listBones[i].Update();
			}
		}
	}
}
