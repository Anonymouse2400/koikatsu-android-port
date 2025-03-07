using UnityEngine;

namespace MorphAssist
{
	public class TimeProgressCtrl
	{
		private float count;

		private float rate = 1f;

		private float progressTime = 0.15f;

		public void End()
		{
			count = progressTime;
			rate = 1f;
		}

		public void Start()
		{
			count = 0f;
			rate = 0f;
		}

		public float Calculate()
		{
			count += Time.deltaTime;
			if (count < progressTime)
			{
				rate = Mathf.InverseLerp(0f, progressTime, count);
			}
			else
			{
				End();
			}
			return rate;
		}

		public void SetProgressTime(float time)
		{
			progressTime = time;
		}

		public float GetProgressRate()
		{
			return rate;
		}
	}
}
