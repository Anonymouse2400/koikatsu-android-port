using System;
using System.Collections;
using UnityEngine;

namespace Illusion.Component.Correct
{
	public class FingerCorrect : MonoBehaviour
	{
		[Serializable]
		private class Info
		{
			public bool enable;

			public Transform trfBone;

			public Vector3 position = Vector3.zero;

			public Vector3 rotation = Vector3.zero;
		}

		[SerializeField]
		private bool enableAll = true;

		[SerializeField]
		private Info[] info;

		private void LateUpdate()
		{
			if (!enableAll || this.info == null)
			{
				return;
			}
			Info[] array = this.info;
			foreach (Info info in array)
			{
				if (info.enable && !(null == info.trfBone))
				{
					Vector3 position = info.trfBone.position;
					info.trfBone.position = new Vector3(info.trfBone.position.x + info.position.x, info.trfBone.position.y + info.position.y, info.trfBone.position.z + info.position.z);
					Vector3 eulerAngles = info.trfBone.rotation.eulerAngles;
					info.trfBone.rotation = Quaternion.Euler(info.trfBone.rotation.eulerAngles.x + info.rotation.x, info.trfBone.rotation.eulerAngles.y + info.rotation.y, info.trfBone.rotation.eulerAngles.z + info.rotation.z);
					StartCoroutine(RestoreTransform(info.trfBone, position, eulerAngles));
				}
			}
		}

		private IEnumerator RestoreTransform(Transform trfTarget, Vector3 pos, Vector3 rot)
		{
			yield return new WaitForEndOfFrame();
			trfTarget.position = pos;
			trfTarget.rotation = Quaternion.Euler(rot);
		}
	}
}
