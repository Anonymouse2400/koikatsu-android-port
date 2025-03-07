using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HExpGauge : MonoBehaviour
{
	public Image image;

	public Image imageAdd;

	public TextMeshProUGUI text;

	public void Set(float _now, float _add)
	{
		image.fillAmount = _now * 0.01f;
		imageAdd.fillAmount = _add * 0.01f;
		if ((bool)text)
		{
			text.text = ((int)_add).ToString("000") + "%";
		}
	}
}
