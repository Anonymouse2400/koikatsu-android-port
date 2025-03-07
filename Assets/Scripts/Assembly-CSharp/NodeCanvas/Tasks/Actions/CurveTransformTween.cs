using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Category("Tween")]
	[Name("Curve Tween")]
	public class CurveTransformTween : ActionTask<Transform>
	{
		public enum TransformMode
		{
			Position = 0,
			Rotation = 1,
			Scale = 2
		}

		public enum TweenMode
		{
			Absolute = 0,
			Additive = 1
		}

		public enum PlayMode
		{
			Normal = 0,
			PingPong = 1
		}

		public TransformMode transformMode;

		public TweenMode mode;

		public PlayMode playMode;

		public BBParameter<Vector3> targetPosition;

		public BBParameter<AnimationCurve> curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public BBParameter<float> time = 0.5f;

		private Vector3 original;

		private Vector3 final;

		private bool ponging;

		protected override void OnExecute()
		{
			if (ponging)
			{
				final = original;
			}
			if (transformMode == TransformMode.Position)
			{
				original = base.agent.localPosition;
			}
			if (transformMode == TransformMode.Rotation)
			{
				original = base.agent.localEulerAngles;
			}
			if (transformMode == TransformMode.Scale)
			{
				original = base.agent.localScale;
			}
			if (!ponging)
			{
				final = targetPosition.value + ((mode != TweenMode.Additive) ? Vector3.zero : original);
			}
			ponging = playMode == PlayMode.PingPong;
			if ((original - final).magnitude < 0.1f)
			{
				EndAction();
			}
		}

		protected override void OnUpdate()
		{
			Vector3 vector = Vector3.Lerp(original, final, curve.value.Evaluate(base.elapsedTime / time.value));
			if (transformMode == TransformMode.Position)
			{
				base.agent.localPosition = vector;
			}
			if (transformMode == TransformMode.Rotation)
			{
				base.agent.localEulerAngles = vector;
			}
			if (transformMode == TransformMode.Scale)
			{
				base.agent.localScale = vector;
			}
			if (base.elapsedTime >= time.value)
			{
				EndAction(true);
			}
		}
	}
}
