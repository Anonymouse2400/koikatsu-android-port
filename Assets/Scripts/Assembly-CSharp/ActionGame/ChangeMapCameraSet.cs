using System.Collections;
using ADV;
using Manager;
using UnityEngine;

namespace ActionGame
{
	public class ChangeMapCameraSet : MonoBehaviour
	{
		private IEnumerator Start()
		{
			if (Singleton<Game>.IsInstance() && !Program.isADVProcessing)
			{
				Camera.main.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
			yield break;
		}
	}
}
