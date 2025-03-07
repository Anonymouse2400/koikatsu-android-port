using UnityEngine;

namespace FBSAssist
{
	public class TimeProgressCtrlRandom : TimeProgressCtrl
	{
		private float minTime = 0.1f;

		private float maxTime = 0.2f;

		public void Init(float min, float max)
		{
			minTime = min;
			maxTime = max;
			SetProgressTime(Random.Range(minTime, maxTime));
			Start();
		}

		public new float Calculate()
		{
			float num = base.Calculate();
			if (num == 1f)
			{
				SetProgressTime(Random.Range(minTime, maxTime));
				Start();
			}
			return num;
		}

		public float Calculate(float _minTime, float _maxTime)
		{
			minTime = _minTime;
			maxTime = _maxTime;
			return Calculate();
		}
	}
}
