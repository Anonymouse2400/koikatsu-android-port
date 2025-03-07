using System;
using DG.Tweening;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Icon("DOTTween", true)]
	[Category("Tween")]
	public class TweenColor : ActionTask<Renderer>
	{
		public BBParameter<Color> color;

		public BBParameter<float> delay = 0f;

		public BBParameter<float> duration = 0.5f;

		public Ease easeType = Ease.Linear;

		public bool waitActionFinish = true;

		private string id;

		protected override void OnExecute()
		{
			Tweener t = base.agent.material.DOColor(color.value, duration.value);
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
