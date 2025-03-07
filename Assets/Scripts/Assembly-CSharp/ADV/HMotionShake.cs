using Illusion.CustomAttributes;
using UnityEngine;

namespace ADV
{
	public class HMotionShake : MonoBehaviour
	{
		[RangeLabel("モーション揺らぎ", 0f, 1f)]
		[SerializeField]
		[Header("---< モーション関連 >---")]
		private float motion;

		[Label("揺らぎモーション発動までの最小時間")]
		[SerializeField]
		private float timeAutoMotionMin = 3f;

		[Label("揺らぎモーション発動までの最大時間")]
		[SerializeField]
		private float timeAutoMotionMax = 5f;

		[Label("揺らぎモーションを変更している最小時間")]
		[SerializeField]
		private float timeMotionMin = 2f;

		[Label("揺らぎモーションを変更している最大時間")]
		[SerializeField]
		private float timeMotionMax = 3f;

		[Label("揺らぎモーションを最低でもこんだけ進める")]
		[SerializeField]
		private float rateMotionMin = 0.1f;

		[Label("揺らぎモーション変更しているときのリープアニメーション")]
		[SerializeField]
		private AnimationCurve curveMotion = new AnimationCurve(default(Keyframe), new Keyframe
		{
			time = 1f,
			value = 1f
		});

		[SerializeField]
		[NotEditable]
		[Header("---< デバッグ表示 >---")]
		private bool allowMotion;

		[SerializeField]
		[NotEditable]
		private bool enableMotion;

		[SerializeField]
		[NotEditable]
		private float timeMotionCalc;

		[SerializeField]
		[NotEditable]
		private float timeMotion;

		[SerializeField]
		[NotEditable]
		private float timeAutoMotionCalc;

		[SerializeField]
		[NotEditable]
		private float timeAutoMotion;

		[SerializeField]
		[NotEditable]
		private Vector2 lerpMotion;

		private ChaControl[] chaCtrls;

		public void SetCharas(params ChaControl[] ctrls)
		{
			chaCtrls = ctrls;
		}

		private void Update()
		{
			if (chaCtrls == null)
			{
				return;
			}
			if (enableMotion)
			{
				timeMotionCalc = Mathf.Clamp(timeMotionCalc + Time.deltaTime, 0f, timeMotion);
				float num = curveMotion.Evaluate(Mathf.Clamp01(timeMotionCalc / timeMotion));
				motion = Mathf.Lerp(lerpMotion.x, lerpMotion.y, num);
				if (num >= 1f)
				{
					enableMotion = false;
				}
			}
			else
			{
				timeAutoMotionCalc = Mathf.Min(timeAutoMotionCalc + Time.deltaTime, timeAutoMotion);
				if (timeAutoMotion > timeAutoMotionCalc)
				{
					return;
				}
				timeAutoMotion = Random.Range(timeAutoMotionMin, timeAutoMotionMax);
				timeAutoMotionCalc = 0f;
				timeMotionCalc = 0f;
				float num2 = (allowMotion ? (1f - motion) : motion);
				if (num2 <= rateMotionMin)
				{
					num2 = (allowMotion ? 1 : 0);
				}
				else
				{
					float num3 = Random.Range(rateMotionMin, num2);
					num2 = motion + (allowMotion ? num3 : (0f - num3));
				}
				if (num2 >= 1f)
				{
					allowMotion = false;
				}
				else if (num2 <= 0f)
				{
					allowMotion = true;
				}
				lerpMotion = new Vector2(motion, num2);
				timeMotion = Random.Range(timeMotionMin, timeMotionMax);
				enableMotion = true;
			}
			ChaControl[] array = chaCtrls;
			foreach (ChaControl chaControl in array)
			{
				chaControl.setAnimatorParamFloat("motion", motion);
			}
		}
	}
}
