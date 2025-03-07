using System.Collections.Generic;
using System.Linq;
using IllusionUtility.GetUtility;
using UniRx;
using UnityEngine;

namespace Studio
{
	public class FKCtrl : MonoBehaviour
	{
		private class TargetInfo
		{
			public GameObject gameObject;

			private Transform m_Transform;

			public ChangeAmount changeAmount;

			private BoolReactiveProperty _enable = new BoolReactiveProperty(true);

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

			public OIBoneInfo.BoneGroup group { get; private set; }

			public int level { get; private set; }

			public bool enable
			{
				get
				{
					return _enable.Value;
				}
				set
				{
					_enable.Value = value;
				}
			}

			public TargetInfo(GameObject _gameObject, ChangeAmount _changeAmount, OIBoneInfo.BoneGroup _group, int _level)
			{
				gameObject = _gameObject;
				changeAmount = _changeAmount;
				group = _group;
				level = _level;
				if ((group & OIBoneInfo.BoneGroup.Hair) == 0 && (group & OIBoneInfo.BoneGroup.Skirt) == 0 && (group & OIBoneInfo.BoneGroup.Body) == 0)
				{
					return;
				}
				_enable.Subscribe(delegate(bool _b)
				{
					if (!_b)
					{
						transform.localRotation = Quaternion.identity;
					}
				});
			}

			public void CopyBone()
			{
				changeAmount.rot = transform.localEulerAngles;
			}

			public void Update()
			{
				if (enable)
				{
					transform.localRotation = Quaternion.Euler(changeAmount.rot);
				}
			}
		}

		public static OIBoneInfo.BoneGroup[] parts = new OIBoneInfo.BoneGroup[7]
		{
			OIBoneInfo.BoneGroup.Hair,
			OIBoneInfo.BoneGroup.Neck,
			OIBoneInfo.BoneGroup.Breast,
			OIBoneInfo.BoneGroup.Body,
			OIBoneInfo.BoneGroup.RightHand,
			OIBoneInfo.BoneGroup.LeftHand,
			OIBoneInfo.BoneGroup.Skirt
		};

		private Transform m_Transform;

		private List<TargetInfo> listBones = new List<TargetInfo>();

		private new Transform transform
		{
			get
			{
				if (m_Transform == null)
				{
					m_Transform = base.transform;
				}
				return m_Transform;
			}
		}

		private int count { get; set; }

		public void InitBones(OICharInfo _info, ChaReference _charReference)
		{
			if (_info == null)
			{
				return;
			}
			listBones.Clear();
			foreach (KeyValuePair<int, Info.BoneInfo> item in Singleton<Info>.Instance.dicBoneInfo)
			{
				GameObject gameObject = null;
				switch (item.Value.group)
				{
				case 7:
				case 8:
				case 9:
				{
					GameObject referenceInfo = _charReference.GetReferenceInfo(ChaReference.RefObjKey.HairParent);
					gameObject = referenceInfo.transform.FindLoop(item.Value.bone);
					break;
				}
				default:
					gameObject = transform.FindLoop(item.Value.bone);
					if (!(gameObject == null))
					{
					}
					break;
				}
				if (gameObject == null)
				{
					continue;
				}
				OIBoneInfo value = null;
				if (_info.bones.TryGetValue(item.Key, out value))
				{
					OIBoneInfo.BoneGroup boneGroup = OIBoneInfo.BoneGroup.Body;
					switch (item.Value.group)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						boneGroup = OIBoneInfo.BoneGroup.Body;
						break;
					case 7:
					case 8:
					case 9:
						boneGroup = OIBoneInfo.BoneGroup.Hair;
						break;
					case 10:
						boneGroup = OIBoneInfo.BoneGroup.Neck;
						break;
					case 11:
					case 12:
						boneGroup = OIBoneInfo.BoneGroup.Breast;
						break;
					case 13:
						boneGroup = OIBoneInfo.BoneGroup.Skirt;
						break;
					default:
						boneGroup = (OIBoneInfo.BoneGroup)(1 << item.Value.group);
						break;
					}
					listBones.Add(new TargetInfo(gameObject, value.changeAmount, boneGroup, item.Value.level));
				}
			}
			count = listBones.Count;
		}

		public void CopyBone()
		{
			foreach (TargetInfo listBone in listBones)
			{
				listBone.CopyBone();
			}
		}

		public void CopyBone(OIBoneInfo.BoneGroup _target)
		{
			foreach (TargetInfo item in listBones.Where((TargetInfo l) => (l.group & _target) != 0))
			{
				item.CopyBone();
			}
		}

		public void SetEnable(OIBoneInfo.BoneGroup _group, bool _enable)
		{
			foreach (TargetInfo item in listBones.Where((TargetInfo l) => (l.group & _group) != 0))
			{
				item.enable = _enable;
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
