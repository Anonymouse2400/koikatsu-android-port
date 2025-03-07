using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	[RequireComponent(typeof(TextSlideEffect))]
	public class TextSlideEffectCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
	{
		[SerializeField]
		private TextSlideEffect textSlideEffect;

		[SerializeField]
		private RectTransform transBase;

		[SerializeField]
		private Text text;

		[SerializeField]
		private TextMeshProUGUI textMesh;

		public float speed = 50f;

		public bool assist;

		private bool isPlay;

		private float preferredWidth
		{
			get
			{
				return text ? text.preferredWidth : ((!textMesh) ? 0f : textMesh.preferredWidth);
			}
		}

		private void MoveText()
		{
			float x = transBase.sizeDelta.x;
			float num = preferredWidth;
			if (x >= num)
			{
				return;
			}
			if ((bool)text)
			{
				float num2 = textSlideEffect.subPos;
				if (num2 > num)
				{
					num2 -= num + x;
				}
				num2 += speed * Time.deltaTime;
				textSlideEffect.subPos = num2;
			}
			else if ((bool)textMesh)
			{
				Vector4 margin = textMesh.margin;
				if (Mathf.Abs(margin.x) > num)
				{
					margin.x += num + x;
				}
				margin.x -= speed * Time.deltaTime;
				textMesh.margin = margin;
			}
		}

		private void Check()
		{
			float x = transBase.sizeDelta.x;
			float num = preferredWidth;
			if (x >= num)
			{
				ObservableLateUpdateTrigger component = GetComponent<ObservableLateUpdateTrigger>();
				if (component != null)
				{
					Object.Destroy(component);
				}
				Object.Destroy(this);
				Object.Destroy(textSlideEffect);
				return;
			}
			if ((bool)text)
			{
				text.alignment = TextAnchor.MiddleLeft;
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				text.raycastTarget = true;
			}
			else if ((bool)textMesh)
			{
				textMesh.alignment = TextAlignmentOptions.MidlineLeft;
				textMesh.overflowMode = TextOverflowModes.Ellipsis;
				textMesh.enableWordWrapping = false;
			}
			AddFunc();
		}

		private void AddFunc()
		{
			(from _ in this.UpdateAsObservable()
				where isPlay
				select _).Subscribe(delegate
			{
				MoveText();
			}).AddTo(this);
		}

		private void Start()
		{
			if (assist)
			{
				this.LateUpdateAsObservable().First().Subscribe(delegate
				{
					Check();
				})
					.AddTo(this);
			}
			else
			{
				AddFunc();
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			isPlay = true;
			if ((bool)textMesh)
			{
				textMesh.overflowMode = TextOverflowModes.Overflow;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			isPlay = false;
			textSlideEffect.subPos = 0f;
			if ((bool)textMesh)
			{
				textMesh.margin = Vector4.zero;
				textMesh.overflowMode = TextOverflowModes.Ellipsis;
			}
		}
	}
}
