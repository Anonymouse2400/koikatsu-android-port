using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Studio
{
	public class IKCtrl : MonoBehaviour
	{
		private List<OCIChar.IKInfo> listIKInfo;

		public OCIChar.IKInfo addIKInfo
		{
			set
			{
				listIKInfo.Add(value);
			}
		}

		public int count
		{
			get
			{
				return listIKInfo.Count;
			}
		}

		public void InitTarget()
		{
			StartCoroutine("InitTargetCoroutine");
		}

		public void CopyBone(OIBoneInfo.BoneGroup _target)
		{
			foreach (OCIChar.IKInfo item in listIKInfo.Where((OCIChar.IKInfo l) => (l.boneGroup & _target) != 0))
			{
				item.CopyBone();
			}
		}

		public void CopyBoneRotation(OIBoneInfo.BoneGroup _target)
		{
			foreach (OCIChar.IKInfo item in listIKInfo.Where((OCIChar.IKInfo l) => (l.boneGroup & _target) != 0))
			{
				item.CopyBoneRotation();
			}
		}

		private IEnumerator InitTargetCoroutine()
		{
			yield return null;
			yield return new WaitForEndOfFrame();
			foreach (OCIChar.IKInfo item in listIKInfo)
			{
				item.CopyBone();
			}
		}

		private void Awake()
		{
			listIKInfo = new List<OCIChar.IKInfo>();
		}
	}
}
