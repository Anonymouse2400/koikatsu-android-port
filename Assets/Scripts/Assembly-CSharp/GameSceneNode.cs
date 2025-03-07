using SceneAssist;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameSceneNode : PointerAction
{
	private enum ClickSound
	{
		NoSound = 0,
		OK = 1,
		Cancel = 2
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private Image imageCover;

	[SerializeField]
	private Image imageSelect;

	[SerializeField]
	private Text content;

	[Tooltip("クリックされた時にかぶせを解除する")]
	[SerializeField]
	private bool isAddCoverDisabled;

	[SerializeField]
	private ClickSound clickSound;

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
			if (!value && (bool)imageCover)
			{
				imageCover.enabled = false;
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
			return (!content) ? string.Empty : content.text;
		}
		set
		{
			if ((bool)content)
			{
				content.text = value;
				content.enabled = false;
				content.enabled = true;
			}
		}
	}

	private void SetCoverEnabled(bool _enabled)
	{
		if ((!button || button.interactable) && (bool)imageCover)
		{
			imageCover.enabled = _enabled;
		}
	}

	private void PlaySelectSE()
	{
		if (!button || button.interactable)
		{
			GlobalMethod.PlaySelectSE();
		}
	}

	public void AddActionToButton(UnityAction _action)
	{
		if ((bool)button)
		{
			button.onClick.AddListener(_action);
		}
	}

	public void AddCoverDisabled()
	{
		if (!button)
		{
			return;
		}
		button.onClick.AddListener(delegate
		{
			if ((bool)imageCover)
			{
				imageCover.enabled = false;
			}
		});
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	public void SetActive(bool _value)
	{
		base.gameObject.SetActive(_value);
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
		if (isAddCoverDisabled)
		{
			AddCoverDisabled();
		}
		switch (clickSound)
		{
		case ClickSound.OK:
			AddActionToButton(delegate
			{
				GlobalMethod.PlayDecisionSE();
			});
			break;
		case ClickSound.Cancel:
			AddActionToButton(delegate
			{
				GlobalMethod.PlayCancelSE();
			});
			break;
		}
	}
}
