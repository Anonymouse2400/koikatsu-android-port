using System;
using DG.Tweening;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Tween")]
	[Icon("DOTTween", true)]
	public class TweenPunchRotation : ActionTask<Transform>
	{
		public BBParameter<Vector3> ammount;

		public BBParameter<float> delay = 0f;

		public BBParameter<float> duration = 0.5f;

		public Ease easeType = Ease.Linear;

		public int vibrato = 10;

		public float elasticity = 1f;

		public bool waitActionFinish = true;

		private string id;

		protected override void OnExecute()
		{
			Tweener t = base.agent.DOPunchRotation(ammount.value, duration.value, vibrato, elasticity);
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
