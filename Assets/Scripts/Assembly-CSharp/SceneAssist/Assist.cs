using Manager;
using UnityEngine;

namespace SceneAssist
{
	public class Assist
	{
		public static string AssetBundleSystemSE
		{
			get
			{
				return "sound/data/systemse.unity3d";
			}
		}

		public static Transform PlayDecisionSE()
		{
			return Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, AssetBundleSystemSE, "sse_00_02");
		}

		public static Transform PlayCancelSE()
		{
			return Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, AssetBundleSystemSE, "sse_00_04");
		}
	}
}
