using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TextMeshSlideEffectCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
	{
		[SerializeField]
		private RectTransform transBase;

		[SerializeField]
		private TextMeshProUGUI textMesh;

		public float speed = 50f;

		public bool assist;

		private bool isPlay;

		private float preferredWidth
		{
			get
			{
				return (!textMesh) ? 0f : textMesh.preferredWidth;
			}
		}

		private void MoveText()
		{
			float x = transBase.sizeDelta.x;
			float num = preferredWidth;
			if (!(x >= num))
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
			}
			else
			{
				textMesh.alignment = TextAlignmentOptions.MidlineLeft;
				textMesh.overflowMode = TextOverflowModes.Ellipsis;
				textMesh.enableWordWrapping = false;
				AddFunc();
			}
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
			textMesh.overflowMode = TextOverflowModes.Overflow;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			isPlay = false;
			textMesh.margin = Vector4.zero;
			textMesh.overflowMode = TextOverflowModes.Ellipsis;
		}
	}
}
