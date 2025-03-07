using System.Collections;
using System.Linq;
using ActionGame;
using ActionGame.Chara;
using Illusion.Component;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerActionPoint : TriggerEnterExitEvent
{
	[SerializeField]
	private int no = -1;

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
		this.UpdateAsObservable().Subscribe(delegate
		{
			bool flag = isVisible;
			if (rendererIcon.enabled != flag)
			{
				rendererIcon.enabled = flag;
			}
		}).AddTo(this);
		yield return new WaitWhile(() => !PlayerAction.isNextStatic);
		if (!Singleton<PlayerAction>.Instance.CheckAlive(no))
		{
			Object.Destroy(base.gameObject);
			yield break;
		}
		base.gameObject.layer = LayerMask.NameToLayer("Action/ActionPoint");
		(from _ in this.UpdateAsObservable()
			where base.hitList.Any((Collider p) => p.CompareTag("Player"))
			where !Singleton<Game>.Instance.actScene.Player.isActionNow
			where ActionInput.isAction
			select _).Subscribe(delegate
		{
			Singleton<PlayerAction>.Instance.Action(base.transform, no);
			Object.Destroy(base.gameObject);
		}).AddTo(this);
		base.onTriggerEnter += delegate(Collider _c)
		{
			if (_c.CompareTag("Player"))
			{
				animator.Play("icon_action");
				Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, "sound/data/systemse/00.unity3d", "sse_player_point");
				rendererIcon.sprite = spriteIcon[1];
				_c.GetComponent<Player>().actionPointList.Add(this);
			}
		};
		base.onTriggerExit += delegate(Collider _c)
		{
			if (_c.CompareTag("Player"))
			{
				animator.Play("icon_stop");
				rendererIcon.sprite = spriteIcon[0];
				_c.GetComponent<Player>().actionPointList.Remove(this);
			}
		};
	}
}
