using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private Text label;

	private bool _isParcent;

	public bool isParcent
	{
		get
		{
			return _isParcent;
		}
		set
		{
			_isParcent = value;
			UpdateText(this.value);
		}
	}

	public string text
	{
		get
		{
			return (!(label == null)) ? label.text : string.Empty;
		}
		set
		{
			if ((bool)label)
			{
				label.text = value;
			}
		}
	}

	public float value
	{
		get
		{
			return (!(slider == null)) ? slider.value : 0f;
		}
		set
		{
			if ((bool)slider)
			{
				slider.value = value;
			}
		}
	}

	private void UpdateText(float f)
	{
		text = (isParcent ? f.ToString("P0") : (f * 100f).ToString("0"));
	}

	private void Awake()
	{
		if (slider == null)
		{
			slider = base.gameObject.GetComponent<Slider>();
		}
		if (slider != null)
		{
			slider.onValueChanged.AddListener(delegate(float f)
			{
				UpdateText(f);
			});
			UpdateText(slider.value);
		}
		if (label == null)
		{
			label = base.gameObject.transform.GetComponentInChildren<Text>();
		}
	}

	private void OnDestroy()
	{
		if (slider != null)
		{
			slider.onValueChanged.RemoveListener(delegate(float f)
			{
				UpdateText(f);
			});
		}
	}
}
