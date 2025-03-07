using System;
using DG.Tweening;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Icon("DOTTween", true)]
	[Category("Tween")]
	public class TweenLookAt : ActionTask<Transform>
	{
		public BBParameter<Vector3> vector;

		public BBParameter<float> delay = 0f;

		public BBParameter<float> duration = 0.5f;

		public Ease easeType = Ease.Linear;

		public bool waitActionFinish = true;

		private string id;

		protected override void OnExecute()
		{
			Tweener t = base.agent.DOLookAt(vector.value, duration.value, AxisConstraint.None, null);
			t.SetDelay(delay.value);
			t.SetEase(easeType);
			id = Guid.NewGuid().ToString();
			t.SetId(id);
			if (!waitActionFinish)
			{
				EndAction();
			}
		}

		protected override void OnUpdate()
		{
			if (base.elapsedTime >= duration.value + delay.value)
			{
				EndAction();
			}
		}

		protected override void OnStop()
		{
			if (waitActionFinish)
			{
				DOTween.Kill(id);
			}
		}
	}
}
