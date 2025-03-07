using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class AutoLayoutCtrl : MonoBehaviour
	{
		private enum Func
		{
			Disabled = 0,
			Delete = 1
		}

		[SerializeField]
		private HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup;

		[SerializeField]
		private ContentSizeFitter contentSizeFitter;

		[SerializeField]
		private Func func = Func.Delete;

		private IEnumerator WaitFuncCoroutine()
		{
			yield return new WaitForEndOfFrame();
			switch (func)
			{
			case Func.Disabled:
				if ((bool)horizontalOrVerticalLayoutGroup)
				{
					horizontalOrVerticalLayoutGroup.enabled = false;
				}
				if ((bool)contentSizeFitter)
				{
					contentSizeFitter.enabled = false;
				}
				break;
			case Func.Delete:
				if ((bool)horizontalOrVerticalLayoutGroup)
				{
					Object.Destroy(horizontalOrVerticalLayoutGroup);
				}
				if ((bool)contentSizeFitter)
				{
					Object.Destroy(contentSizeFitter);
				}
				break;
			}
			Object.Destroy(this);
		}

		private void Start()
		{
			StartCoroutine(WaitFuncCoroutine());
		}
	}
}
