using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Studio
{
	public class ListNode : PointerAction
	{
		[SerializeField]
		private Button button;

		[SerializeField]
		private Image imageSelect;

		[SerializeField]
		private Text content;

		[SerializeField]
		private TextMeshProUGUI textMesh;

		public bool interactable
		{
			get
			{
				return (bool)button && button.interactable;
			}
			set
			{
				if ((bool)button)
				{
					button.interactable = value;
				}
			}
		}

		public bool select
		{
			get
			{
				return (bool)imageSelect && imageSelect.enabled;
			}
			set
			{
				if ((bool)imageSelect)
				{
					imageSelect.enabled = value;
				}
			}
		}

		public Image image
		{
			get
			{
				return (!button) ? null : button.image;
			}
		}

		public Sprite selectSprite
		{
			set
			{
				if ((bool)imageSelect)
				{
					imageSelect.sprite = value;
				}
			}
		}

		public string text
		{
			get
			{
				return content ? content.text : ((!textMesh) ? string.Empty : textMesh.text);
			}
			set
			{
				if ((bool)content)
				{
					content.text = value;
					content.enabled = false;
					content.enabled = true;
				}
				else if ((bool)textMesh)
				{
					textMesh.text = value;
				}
			}
		}

		private void SetCoverEnabled(bool _enabled)
		{
			if ((bool)button && button.interactable)
			{
			}
		}

		private void PlaySelectSE()
		{
			if ((bool)button && button.interactable)
			{
			}
		}

		public void AddActionToButton(UnityAction _action)
		{
			if ((bool)button)
			{
				button.onClick.AddListener(_action);
			}
		}

		private void Awake()
		{
			listEnterAction.Add(delegate
			{
				SetCoverEnabled(true);
			});
			listEnterAction.Add(PlaySelectSE);
			listExitAction.Add(delegate
			{
				SetCoverEnabled(false);
			});
		}
	}
}
