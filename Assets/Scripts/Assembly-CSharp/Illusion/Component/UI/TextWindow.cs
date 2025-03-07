using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Component.UI
{
	public class TextWindow : Graphic
	{
		[SerializeField]
		private Text text;

		private float? width;

		public void SetText(string text, float? width = null)
		{
			if (!(this.text == null))
			{
				this.width = width;
				this.text.text = text;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			TextFit();
		}

		private void Update()
		{
			TextFit();
		}

		private void TextFit()
		{
			if (!(text == null))
			{
				float x = ((!width.HasValue) ? text.preferredWidth : width.Value);
				base.rectTransform.sizeDelta = new Vector2(x, text.preferredHeight);
				base.rectTransform.sizeDelta = new Vector2(x, text.preferredHeight);
				Vector2 sizeDelta = base.rectTransform.sizeDelta;
				if (sizeDelta.x < 1f)
				{
					sizeDelta = Vector2.zero;
				}
				base.rectTransform.sizeDelta = sizeDelta;
			}
		}

		[ContextMenu("Setup")]
		private void Setup()
		{
			text = GetComponentInChildren<Text>();
		}
	}
}
