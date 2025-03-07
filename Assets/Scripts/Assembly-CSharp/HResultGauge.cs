using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HResultGauge : MonoBehaviour
{
	public Image image;

	public TextMeshProUGUI text;

	public float nowGauge;

	public float addPoint;

	public float endTime = 2f;

	public float waitTime = 1f;

	private float time;

	private bool end = true;

	private float wait;

	private bool start;

	private void Update()
	{
		if (end || Singleton<Scene>.Instance.IsNowLoadingFade)
		{
			return;
		}
		wait = Mathf.Min(wait + Time.deltaTime, waitTime);
		if (Mathf.Approximately(wait, waitTime))
		{
			if (!start)
			{
				start = true;
			}
			time = Mathf.Min(time + Time.deltaTime, endTime);
			float num = time / endTime;
			float num2 = Mathf.Min(100f, nowGauge + Mathf.Lerp(0f, addPoint, num));
			image.fillAmount = num2 * 0.01f;
			if ((bool)text)
			{
				text.text = ((int)num2).ToString("000") + "%";
			}
			if (Mathf.Approximately(num, 1f))
			{
				end = true;
				start = false;
			}
		}
	}

	public void SetStart(float _now, float _add)
	{
		nowGauge = _now;
		addPoint = _add;
		time = 0f;
		end = _add == 0f;
		wait = 0f;
		start = false;
		image.fillAmount = nowGauge * 0.01f;
		if ((bool)text)
		{
			text.text = ((int)nowGauge).ToString("000") + "%";
		}
	}

	public bool IsEnd()
	{
		return end;
	}

	public bool IsStart()
	{
		return start;
	}

	public void SetFinish()
	{
		end = true;
		start = false;
		float num = Mathf.Min(100f, nowGauge + addPoint);
		image.fillAmount = num * 0.01f;
		if ((bool)text)
		{
			text.text = ((int)num).ToString("000") + "%";
		}
	}

	public void SetImageVisible(bool _visible)
	{
		image.enabled = _visible;
	}

	public void SetImageFillAmout(float _amout)
	{
		image.fillAmount = _amout;
	}
}
