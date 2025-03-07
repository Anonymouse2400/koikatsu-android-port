using System;
using UnityEngine;

[Serializable]
public class FBSBlinkControl
{
	private byte fixedFlags;

	[Range(0f, 255f)]
	public byte BlinkFrequency = 30;

	private sbyte blinkMode;

	[Range(0f, 0.5f)]
	public float BaseSpeed = 0.15f;

	private float calcSpeed;

	private float blinkTime;

	private int count;

	private float openRate = 1f;

	public void SetFixedFlags(byte flags)
	{
		fixedFlags = flags;
	}

	public byte GetFixedFlags()
	{
		return fixedFlags;
	}

	public void SetFrequency(byte value)
	{
		BlinkFrequency = value;
		if (blinkMode == 0)
		{
			int num = UnityEngine.Random.Range(0, BlinkFrequency);
			float t = Mathf.InverseLerp(0f, (int)BlinkFrequency, num);
			t = Mathf.Lerp(0f, (int)BlinkFrequency, t);
			blinkTime = Time.time + 0.2f * t;
		}
	}

	public void SetSpeed(float value)
	{
		BaseSpeed = Mathf.Max(1f, value);
	}

	public void SetForceOpen()
	{
		calcSpeed = BaseSpeed + UnityEngine.Random.Range(0f, 0.05f);
		blinkTime = Time.time + calcSpeed;
		blinkMode = -1;
	}

	public void SetForceClose()
	{
		calcSpeed = BaseSpeed + UnityEngine.Random.Range(0f, 0.05f);
		blinkTime = Time.time + calcSpeed;
		count = UnityEngine.Random.Range(0, 3) + 1;
		blinkMode = 1;
	}

	public void CalcBlink()
	{
		float num = 0f;
		float num2 = Mathf.Max(0f, blinkTime - Time.time);
		switch (blinkMode)
		{
		case 0:
			num = 1f;
			break;
		case 1:
			num = Mathf.Clamp(num2 / calcSpeed, 0f, 1f);
			break;
		default:
			num = Mathf.Clamp(1f - num2 / calcSpeed, 0f, 1f);
			break;
		}
		if (fixedFlags == 0)
		{
			openRate = num;
		}
		if (fixedFlags != 0 || Time.time <= blinkTime)
		{
			return;
		}
		switch (blinkMode)
		{
		case 0:
			SetForceClose();
			break;
		case 1:
			count--;
			if (0 >= count)
			{
				SetForceOpen();
			}
			break;
		case -1:
		{
			int num3 = UnityEngine.Random.Range(0, BlinkFrequency);
			float t = Mathf.InverseLerp(0f, (int)BlinkFrequency, num3);
			t = Mathf.Lerp(0f, (int)BlinkFrequency, t);
			blinkTime = Time.time + 0.2f * t;
			blinkMode = 0;
			break;
		}
		}
	}

	public float GetOpenRate()
	{
		return openRate;
	}
}
