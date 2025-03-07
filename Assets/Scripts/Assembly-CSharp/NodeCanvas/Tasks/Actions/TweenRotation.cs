using System;
using DG.Tweening;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Tween")]
	[Icon("DOTTween", true)]
	public class TweenRotation : ActionTask<Transform>
	{
		public BBParameter<Vector3> vector;

		public BBParameter<float> delay = 0f;

		public BBParameter<float> duration = 0.5f;

		public Ease easeType = Ease.Linear;

		public bool relative;

		public bool waitActionFinish = true;

		private string id;

		protected override void OnExecute()
		{
			if (!relative && base.agent.eulerAngles == vector.value)
			{
				EndAction();
				return;
			}
			Tweener t = base.agent.DORotate(vector.value, duration.value);
			t.SetDelay(delay.value);
			t.SetEase(easeType);
			id = Guid.NewGuid().ToString();
			t.SetId(id);
			if (relative)
			{
				t.SetRelative();
			}
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
