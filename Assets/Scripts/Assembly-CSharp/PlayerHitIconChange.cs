using System.Collections;
using Illusion.Component;
using Manager;
using UnityEngine;

public class PlayerHitIconChange : TriggerEnterExitEvent
{
	[SerializeField]
	private SpriteRenderer rendererIcon;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Sprite[] spriteIcon;

	private bool isVisible
	{
		get
		{
			if (!Singleton<Game>.IsInstance())
			{
				return false;
			}
			if (Singleton<Game>.Instance.IsRegulate(true))
			{
				return false;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene == null)
			{
				return false;
			}
			if (actScene.regulate != 0)
			{
				return false;
			}
			if (actScene.AdvScene != null && actScene.AdvScene.gameObject.activeSelf)
			{
				return false;
			}
			return true;
		}
	}

	private IEnumerator Start()
	{
		if (rendererIcon == null)
		{
			rendererIcon = GetComponentInChildren<SpriteRenderer>();
		}
		yield return new WaitWhile(() => !PlayerAction.isNextStatic);
		base.gameObject.layer = LayerMask.NameToLayer("Action/ActionPoint");
		base.onTriggerEnter += delegate(Collider _c)
		{
			if (_c.tag.StartsWith("Player"))
			{
				animator.Play("icon_action");
				Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, "sound/data/systemse/00.unity3d", "sse_player_point");
				rendererIcon.sprite = spriteIcon[1];
			}
		};
		base.onTriggerExit += delegate(Collider _c)
		{
			if (_c.tag.StartsWith("Player"))
			{
				animator.Play("icon_stop");
				rendererIcon.sprite = spriteIcon[0];
			}
		};
	}
}
