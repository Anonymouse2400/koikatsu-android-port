using ActionGame;
using Illusion.CustomAttributes;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class TimeZoenCutIn : MonoBehaviour
{
	private enum Fade
	{
		In = 0,
		Wait = 1,
		Out = 2
	}

	[SerializeField]
	private Texture2D[] texTimeZone;

	[SerializeField]
	[Label("設定Image")]
	private RawImage rawImage;

	[Label("フェード時間")]
	[SerializeField]
	private float timeFade = 3f;

	[SerializeField]
	[Label("停止時間")]
	private float timeWait = 5f;

	private float time;

	private Fade fade;

	private bool isProc;

	private void Update()
	{
		if (!isProc)
		{
			return;
		}
		float a = 0f;
		switch (fade)
		{
		case Fade.In:
			time += Time.deltaTime;
			a = time / timeFade;
			if (!(timeFade > time))
			{
				a = 1f;
				fade = Fade.Wait;
				time = 0f;
			}
			break;
		case Fade.Wait:
			time += Time.deltaTime;
			a = 1f;
			if (!(timeWait > time))
			{
				fade = Fade.Out;
				time = 0f;
			}
			break;
		case Fade.Out:
			time += Time.deltaTime;
			a = 1f - time / timeFade;
			if (!(timeFade > time))
			{
				a = 0f;
				isProc = false;
				base.gameObject.SetActive(isProc);
			}
			break;
		}
		Color color = rawImage.color;
		color.a = a;
		rawImage.color = color;
	}

	public void FadeStart(Cycle.Type _cycleType)
	{
		switch (_cycleType)
		{
		case Cycle.Type.LunchTime:
			rawImage.texture = ((Singleton<Game>.Instance.actScene.Cycle.nowWeek != Cycle.Week.Saturday) ? texTimeZone[0] : texTimeZone[3]);
			break;
		case Cycle.Type.StaffTime:
			rawImage.texture = ((Singleton<Game>.Instance.actScene.Cycle.nowWeek != Cycle.Week.Saturday) ? texTimeZone[1] : texTimeZone[4]);
			break;
		case Cycle.Type.AfterSchool:
			rawImage.texture = ((Singleton<Game>.Instance.actScene.Cycle.nowWeek != Cycle.Week.Saturday) ? texTimeZone[2] : texTimeZone[5]);
			break;
		}
		time = 0f;
		fade = Fade.In;
		isProc = true;
		base.gameObject.SetActive(isProc);
	}

	public void FadeStart(Cycle.Type _cycleType, Texture2D texture)
	{
		if (texture == null)
		{
			FadeStart(_cycleType);
			return;
		}
		rawImage.texture = texture;
		time = 0f;
		fade = Fade.In;
		isProc = true;
		base.gameObject.SetActive(isProc);
	}

	public void Stop()
	{
		if (isProc)
		{
			Color color = rawImage.color;
			color.a = 0f;
			rawImage.color = color;
			time = 0f;
			fade = Fade.In;
			isProc = false;
			base.gameObject.SetActive(isProc);
		}
	}
}
