using System;
using UnityEngine;

namespace SceneAssist
{
	public class ContentSizeChange : MonoBehaviour
	{
		[Serializable]
		public class SizeInfo
		{
			public bool use = true;

			public float enableSize;

			public float disableSize;
		}

		[SerializeField]
		private RectTransform target;

		[SerializeField]
		private SizeInfo width = new SizeInfo();

		[SerializeField]
		private SizeInfo height = new SizeInfo();

		private void OnEnable()
		{
			if (!(target == null) && (width.use || height.use))
			{
				Vector2 sizeDelta = target.sizeDelta;
				if (width.use)
				{
					sizeDelta.x = width.enableSize;
				}
				if (height.use)
				{
					sizeDelta.y = height.enableSize;
				}
				target.sizeDelta = sizeDelta;
			}
		}

		private void OnDisable()
		{
			if (!(target == null) && (width.use || height.use))
			{
				Vector2 sizeDelta = target.sizeDelta;
				if (width.use)
				{
					sizeDelta.x = width.disableSize;
				}
				if (height.use)
				{
					sizeDelta.y = height.disableSize;
				}
				target.sizeDelta = sizeDelta;
			}
		}
	}
}
