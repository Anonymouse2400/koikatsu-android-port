using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class bl_IconItem : MonoBehaviour
	{
		private BoolReactiveProperty _visible = new BoolReactiveProperty(false);

		private StringReactiveProperty _text = new StringReactiveProperty();

		[Separator("REFERENCES")]
		[SerializeField]
		private Image _graphic;

		[SerializeField]
		private Text _infoText;

		[SerializeField]
		private Animator _animator;

		private CanvasGroup _canvasGroup;

		private float delay = 0.1f;

		private static int OpenHash = Animator.StringToHash("Open");

		private bool open = true;

		public bool visible
		{
			get
			{
				return _visible.Value;
			}
			set
			{
				_visible.Value = value;
			}
		}

		public string text
		{
			set
			{
				_text.Value = value;
			}
		}

		public Image graphic
		{
			get
			{
				return _graphic;
			}
		}

		public CanvasGroup canvasGroup
		{
			get
			{
				return _canvasGroup;
			}
		}

		public Animator animator
		{
			get
			{
				return _animator;
			}
		}

		private void Awake()
		{
			_canvasGroup = this.GetOrAddComponent<CanvasGroup>();
			_canvasGroup.ignoreParentGroups = true;
			_canvasGroup.alpha = 0f;
			_visible.Subscribe(delegate(bool isOn)
			{
				_graphic.enabled = isOn;
			});
			if (_infoText != null)
			{
				_visible.Subscribe(delegate(bool isOn)
				{
					_infoText.enabled = isOn;
				});
				_text.SubscribeToText(_infoText);
			}
			if (_animator != null)
			{
				_animator.enabled = false;
			}
		}

		public void DestroyIcon()
		{
			Object.Destroy(base.gameObject);
		}

		public void SetIcon(Sprite ico)
		{
			_graphic.sprite = ico;
		}

		private IEnumerator FadeIcon()
		{
			yield return new WaitForSeconds(delay);
			while (_canvasGroup.alpha < 1f)
			{
				_canvasGroup.alpha += Time.deltaTime * 2f;
				yield return null;
			}
			if (_animator != null)
			{
				_animator.enabled = true;
			}
			InfoItem();
		}

		public void SetVisibleAlpha()
		{
			_canvasGroup.alpha = 1f;
		}

		public void InfoItem()
		{
			open = !open;
			if (_animator != null)
			{
				_animator.SetBool(OpenHash, open);
			}
			else if (!(_infoText == null))
			{
				Color color = _infoText.color;
				color.a = (open ? 1 : 0);
				_infoText.color = color;
				_infoText.rectTransform.localScale = Vector3.one;
			}
		}

		public void DelayStart(float v)
		{
			delay = v;
			StartCoroutine(FadeIcon());
		}
	}
}
