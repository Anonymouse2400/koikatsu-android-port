using System;
using Studio;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Localize.Translate
{
	[RequireComponent(typeof(TextSlideEffect))]
	public class TextSlideEffectCtrl : MonoBehaviour, IDisposable, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
	{
		[SerializeField]
		private TextSlideEffect textSlideEffect;

		[SerializeField]
		private RectTransform transBase;

		[SerializeField]
		private Text text;

		[SerializeField]
		private TextMeshProUGUI textMesh;

		[SerializeField]
		private float _speed = 50f;

		[SerializeField]
		private bool _assist;

		private bool isUnityText;

		private RectTransform textTransform;

		public float speed
		{
			set
			{
				_speed = value;
			}
		}

		private bool _isPlay { get; set; }

		private float _preferredWidth { get; set; }

		public void Dispose()
		{
			UnityEngine.Object.Destroy(this);
			UnityEngine.Object.Destroy(textSlideEffect);
		}

		private void MoveText()
		{
			float x = transBase.sizeDelta.x;
			float preferredWidth = _preferredWidth;
			if (x >= preferredWidth)
			{
				return;
			}
			if (isUnityText)
			{
				float num = textSlideEffect.subPos;
				if (num > preferredWidth)
				{
					num -= preferredWidth + x;
				}
				num += _speed * Time.deltaTime;
				textSlideEffect.subPos = num;
			}
			else
			{
				Vector4 margin = textMesh.margin;
				if (Mathf.Abs(margin.x) > preferredWidth)
				{
					margin.x += preferredWidth + x;
				}
				margin.x -= _speed * Time.deltaTime;
				textMesh.margin = margin;
			}
		}

		private void Check()
		{
			if (isUnityText)
			{
				text.alignment = TextAnchor.MiddleLeft;
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				text.raycastTarget = true;
			}
			else
			{
				textMesh.alignment = TextAlignmentOptions.MidlineLeft;
				textMesh.overflowMode = TextOverflowModes.Ellipsis;
				textMesh.enableWordWrapping = false;
			}
			float x = transBase.sizeDelta.x;
			float preferredWidth = _preferredWidth;
			if (x >= preferredWidth)
			{
				ObservableLateUpdateTrigger component = GetComponent<ObservableLateUpdateTrigger>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				Dispose();
			}
			else
			{
				AddFunc();
			}
		}

		private void AddFunc()
		{
			(from _ in this.UpdateAsObservable()
				where _isPlay
				select _).Subscribe(delegate
			{
				MoveText();
			}).AddTo(this);
		}

		private void Awake()
		{
			isUnityText = text != null;
			float num = 0f;
			if (isUnityText)
			{
				textTransform = text.rectTransform;
				num = text.preferredWidth;
			}
			else
			{
				if (!(textMesh != null))
				{
					Dispose();
					return;
				}
				textTransform = textMesh.rectTransform;
				num = textMesh.preferredWidth;
			}
			float x = textTransform.sizeDelta.x;
			if (x < 0f)
			{
				num += Mathf.Abs(x);
			}
			_preferredWidth = num;
		}

		private void Start()
		{
			if (_assist)
			{
				this.LateUpdateAsObservable().Take(1).Subscribe(delegate
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
			_isPlay = true;
			if (textMesh != null)
			{
				textMesh.overflowMode = TextOverflowModes.Overflow;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_isPlay = false;
			textSlideEffect.subPos = 0f;
			if (textMesh != null)
			{
				textMesh.margin = Vector4.zero;
				textMesh.overflowMode = TextOverflowModes.Ellipsis;
			}
		}
	}
}
