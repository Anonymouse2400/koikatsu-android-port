using UnityEngine;

namespace Illusion.Extensions
{
	public static class CanvasGroupExtensions
	{
		public static void Enable(this CanvasGroup canvasGroup, bool enable, bool ignoreParentGroups = false)
		{
			canvasGroup.alpha = ((!enable) ? 0f : 1f);
			canvasGroup.interactable = enable;
			canvasGroup.blocksRaycasts = enable;
		}

		public static void Set(this CanvasGroup canvasGroup, float alpha, bool interactable, bool blocksRaycasts, bool ignoreParentGroups = false)
		{
			canvasGroup.alpha = alpha;
			canvasGroup.interactable = interactable;
			canvasGroup.blocksRaycasts = blocksRaycasts;
			canvasGroup.ignoreParentGroups = ignoreParentGroups;
		}
	}
}
