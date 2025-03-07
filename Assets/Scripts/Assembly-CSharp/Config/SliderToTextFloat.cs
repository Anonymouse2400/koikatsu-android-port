using UnityEngine;
using UnityEngine.UI;

namespace Config
{
	public class SliderToTextFloat : MonoBehaviour
	{
		[SerializeField]
		private Slider silder;

		[SerializeField]
		private Text text;

		public void Start()
		{
			OnValueChanged();
		}

		public void OnValueChanged()
		{
			if (!(silder == null) && !(text == null))
			{
				text.text = silder.value.ToString("0.00");
			}
		}
	}
}
