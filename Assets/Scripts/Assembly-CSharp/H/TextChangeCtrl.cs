using Localize.Translate;
using TMPro;
using UnityEngine;

namespace H
{
	public class TextChangeCtrl : MonoBehaviour
	{
		public string[] drawTexts;

		public TextMeshProUGUI text;

		private void Start()
		{
			if (text != null)
			{
				text = base.gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
			}
		}

		public void OnChangeValue(int _num)
		{
			if (!(text == null) && drawTexts.Length > _num)
			{
				if (_num < 0)
				{
					text.gameObject.SetActive(false);
				}
				else
				{
					text.gameObject.SetActive(true);
					text.text = drawTexts[_num];
				}
				IHTextChangable component = GetComponent<IHTextChangable>();
				if (component != null)
				{
					component.OnChange(_num);
				}
			}
		}
	}
}
