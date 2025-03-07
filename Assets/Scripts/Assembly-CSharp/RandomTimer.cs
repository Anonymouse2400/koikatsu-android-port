using UnityEngine;

public class RandomTimer
{
	private float randMin = 1000f;

	private float randMax = 1000f;

	private float next;

	private float cnt;

	public void Init(float min, float max)
	{
		randMin = min;
		randMax = max;
		next = Random.Range(randMin, randMax);
	}

	public bool Check()
	{
		cnt += Time.deltaTime;
		if (cnt >= next)
		{
			next = Random.Range(randMin, randMax);
			cnt = 0f;
			return true;
		}
		return false;
	}
}
