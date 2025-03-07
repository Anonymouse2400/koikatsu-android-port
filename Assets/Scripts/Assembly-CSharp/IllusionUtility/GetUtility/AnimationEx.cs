using UnityEngine;

namespace IllusionUtility.GetUtility
{
	public static class AnimationEx
	{
		public static AnimationClip GetPlayingClip(this Animation animation)
		{
			AnimationClip result = null;
			float num = 0f;
			foreach (AnimationState item in animation)
			{
				if (item.enabled && num < item.weight)
				{
					num = item.weight;
					result = item.clip;
				}
			}
			return result;
		}
	}
}
