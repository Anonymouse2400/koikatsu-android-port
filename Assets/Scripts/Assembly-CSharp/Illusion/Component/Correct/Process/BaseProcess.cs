using System.Collections;
using UnityEngine;

namespace Illusion.Component.Correct.Process
{
	[RequireComponent(typeof(BaseData))]
	public abstract class BaseProcess : MonoBehaviour
	{
		public enum Type
		{
			Target = 0,
			Sync = 1
		}

		private BaseData _data;

		public Type type;

		public bool noRestore;

		private ChaControl _chaCtrl;

		public BaseData data
		{
			get
			{
				return this.GetComponentCache(ref _data);
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return this.GetCacheObject(ref _chaCtrl, () => GetComponentInParent<ChaControl>());
			}
		}

		protected virtual void LateUpdate()
		{
			Transform bone = data.bone;
			if (bone == null)
			{
				return;
			}
			switch (type)
			{
			case Type.Target:
			{
				Vector3 localPosition = bone.localPosition;
				Quaternion localRotation = bone.localRotation;
				bone.localPosition = localPosition + data.pos;
				bone.localRotation = localRotation * data.rot;
				if (!noRestore)
				{
					StartCoroutine(Restore(bone, localPosition, localRotation));
				}
				break;
			}
			case Type.Sync:
				if (chaCtrl != null)
				{
					base.transform.SetPositionAndRotation(bone.position + chaCtrl.objBodyBone.transform.TransformDirection(data.pos), bone.rotation * data.rot);
				}
				break;
			}
		}

		protected IEnumerator Restore(Transform t, Vector3 pos, Quaternion rot)
		{
			yield return new WaitForEndOfFrame();
			t.localPosition = pos;
			t.localRotation = rot;
		}
	}
}
