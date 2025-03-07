using UnityEngine;

namespace Illusion.Component.UI
{
	public class RepeatIntervalButton : RepeatButton
	{
		public float interval = 0.5f;

		private float timer;

		protected override void Process(bool push)
		{
			if (push && base.isSelect && (timer == 0f || timer == interval))
			{
				call.Invoke();
			}
			timer = (push ? Mathf.Min(timer + Time.deltaTime, interval) : 0f);
		}
	}
}
