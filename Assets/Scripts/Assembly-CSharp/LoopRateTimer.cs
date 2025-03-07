using System;
using UnityEngine;

public class LoopRateTimer
{
	private float loop;

	private float cnt;

	public void Init(float looptime)
	{
		loop = looptime;
	}

	public float Check()
	{
		if (loop <= 0f)
		{
			return 0f;
		}
		cnt += Time.deltaTime * (180f / loop);
		while (cnt >= 180f)
		{
			cnt -= 180f;
		}
		return Mathf.Sin(cnt * ((float)Math.PI / 180f));
	}
}
