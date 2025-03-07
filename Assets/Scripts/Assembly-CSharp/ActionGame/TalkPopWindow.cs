using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame
{
	public class TalkPopWindow : MonoBehaviour
	{
		public Camera targetCamera;

		public Transform target;

		public Vector3 offsetPos;

		public Vector3 offsetAngle;

		public bool angleLockPitch;

		public bool angleLockYaw;

		public bool angleLockRoll;

		public bool reverse;

		[SerializeField]
		private RectTransform tail;

		[SerializeField]
		private RectTransform window;

		private void Start()
		{
			(from _ in this.UpdateAsObservable()
				where target != null
				select _).Take(1).Subscribe(delegate
			{
				target.OnBecameVisibleAsObservable().Subscribe(delegate
				{
					base.gameObject.SetActive(true);
				}).AddTo(this);
				target.OnBecameInvisibleAsObservable().Subscribe(delegate
				{
					base.gameObject.SetActive(false);
				}).AddTo(this);
			});
			(from _ in this.UpdateAsObservable()
				where target != null && targetCamera != null
				where base.isActiveAndEnabled
				select RectTransformUtility.WorldToScreenPoint(targetCamera, target.position + offsetPos)).Subscribe(delegate(Vector2 drawPos)
			{
				Vector2 vector = ClipPos(tail, drawPos);
				Vector2 normalized = (reverse ? (tail.anchoredPosition - vector) : (vector - tail.anchoredPosition)).normalized;
				Vector2 vector2 = new Vector2(Screen.width, Screen.height) * 0.5f;
				Vector2 normalized2 = (vector2 - vector).normalized;
				vector += OffsetCalc(tail.sizeDelta, normalized2);
				vector = ClipPos(tail, vector);
				tail.anchoredPosition = vector;
				Quaternion rotation = tail.rotation;
				Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.up, normalized).eulerAngles;
				bool[] array = new bool[3] { angleLockPitch, angleLockYaw, angleLockRoll };
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i])
					{
						eulerAngles[i] = rotation.eulerAngles[i];
					}
				}
				tail.rotation = Quaternion.Euler(eulerAngles);
				tail.Rotate(offsetAngle);
				vector += OffsetCalc(window.sizeDelta, normalized2);
				window.anchoredPosition = ClipPos(window, vector);
			});
		}

		private static Vector2 ClipPos(RectTransform rt, Vector2 pos)
		{
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			for (int i = 0; i < 2; i++)
			{
				pos[i] = Mathf.Clamp(pos[i], rt.sizeDelta[i] * rt.pivot[i], vector[i] - rt.sizeDelta[i] * (1f - rt.pivot[i]));
			}
			return pos;
		}

		private static Vector2 OffsetCalc(Vector2 size, Vector2 v)
		{
			return new Vector2(size.x * (float)((!(Vector2.Dot(Vector2.right, v) < 0f)) ? 1 : (-1)), size.y * (float)((!(Vector2.Dot(Vector2.up, v) < 0f)) ? 1 : (-1))) * 0.5f;
		}
	}
}
