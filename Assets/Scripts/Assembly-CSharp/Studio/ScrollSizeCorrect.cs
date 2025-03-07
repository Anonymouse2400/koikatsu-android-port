using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ScrollSizeCorrect : MonoBehaviour
	{
		[SerializeField]
		private RectTransform transBase;

		[SerializeField]
		private RectTransform transTarget;

		[SerializeField]
		private Scrollbar scrollbar;

		private void OnRectTransformDimensionsChange()
		{
			float size = Mathf.Min(1f, transBase.rect.height / transTarget.rect.height);
			scrollbar.size = size;
			if (scrollbar.value == 0f)
			{
				scrollbar.value = 0.1f;
				scrollbar.value = 0f;
			}
		}
	}
}
