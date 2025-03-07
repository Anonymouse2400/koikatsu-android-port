using System;
using System.Collections;
using System.Linq;
using ADV;
using ActionGame.Chara;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame.Point
{
	public class Gate : MonoBehaviour
	{
		public int ID = -1;

		public int mapNo = -1;

		public int linkID = -1;

		[SerializeField]
		private Transform _playerTrans;

		[SerializeField]
		private BoxCollider _playerHitBox;

		[SerializeField]
		private BoxCollider _heroineHitBox;

		[Header("0=>クリックで移動,1=>当たって移動")]
		public int moveType;

		public int seType = -1;

		[Header("Icon")]
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private BoxCollider _iconHitBox;

		[SerializeField]
		private bl_MiniMapItem miniMapIcon;

		private string infoText;

		public Transform playerTrans
		{
			get
			{
				return _playerTrans;
			}
		}

		public BoxCollider playerHitBox
		{
			get
			{
				return _playerHitBox;
			}
		}

		public BoxCollider heroineHitBox
		{
			get
			{
				return _heroineHitBox;
			}
		}

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
		}

		public BoxCollider iconHitBox
		{
			get
			{
				return _iconHitBox;
			}
		}

		public void SetData(GateInfo info)
		{
			ID = info.ID;
			mapNo = info.mapNo;
			linkID = info.linkID;
			base.name = info.Name;
			base.transform.SetPositionAndRotation(info.pos, Quaternion.Euler(info.ang));
			_playerTrans.localPosition = info.playerPos;
			_playerTrans.localEulerAngles = info.playerAng;
			_playerHitBox.center = info.playerHitPos;
			_playerHitBox.size = info.playerHitSize;
			_heroineHitBox.center = info.heroineHitPos;
			_heroineHitBox.size = info.heroineHitSize;
			moveType = info.moveType;
			seType = info.seType;
			_canvas.GetComponent<RectTransform>().anchoredPosition3D = info.iconPos;
			_iconHitBox.center = info.iconHitPos;
			_iconHitBox.size = info.iconHitSize;
		}

		public void SetMiniMap(string infoText)
		{
			this.infoText = infoText;
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			_playerHitBox.enabled = false;
			_heroineHitBox.enabled = false;
			_iconHitBox.enabled = false;
			canvas.enabled = false;
			if (!Singleton<Game>.IsInstance())
			{
				yield break;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene == null)
			{
				yield break;
			}
			BoolReactiveProperty canvasEnabled = new BoolReactiveProperty();
			canvasEnabled.Subscribe(delegate(bool isOn)
			{
				canvas.enabled = isOn;
			});
			this.UpdateAsObservable().Subscribe(delegate
			{
				canvasEnabled.Value = !Singleton<Game>.Instance.IsRegulate(true) && !Program.isADVActionActive && actScene.Player.hitGateID == ID;
			});
			ActionMap actMap = actScene.Map;
			if (actMap == null)
			{
				yield break;
			}
			yield return new WaitWhile(() => actMap.isMapLoading);
			(from other in _heroineHitBox.OnTriggerEnterAsObservable()
				select other.GetComponent<NPC>() into npc
				where npc != null
				select npc).Subscribe(delegate(NPC npc)
			{
				npc.hitGateID = ID;
			});
			(from other in _heroineHitBox.OnTriggerExitAsObservable()
				select other.GetComponent<NPC>() into npc
				where npc != null
				select npc).Subscribe(delegate(NPC npc)
			{
				npc.HitGateReset();
			});
			(from other in _playerHitBox.OnTriggerExitAsObservable()
				select other.GetComponent<Player>() into player
				where player != null
				select player).Subscribe(delegate(Player player)
			{
				player.HitGateReset();
			});
			IDisposable linkDisposable = null;
			Func<Player, bool> linkReceive = delegate(Player player)
			{
				if (linkDisposable != null)
				{
					linkDisposable.Dispose();
				}
				if (actMap.gateLinkID == ID)
				{
					actMap.gateLinkID = -1;
					player.isActionNow = false;
					return true;
				}
				return false;
			};
			linkDisposable = this.UpdateAsObservable().Subscribe(delegate
			{
				Player player2 = actScene.Player;
				if (linkReceive(player2))
				{
					player2.HitGateReset();
				}
			});
			Action<Player> linkJump = delegate(Player player)
			{
				actMap.gateLinkID = linkID;
				player.isActionNow = true;
				if (player.chaser != null && player.chaser.mapNo == player.mapNo)
				{
					player.chaser.mapNo = mapNo;
				}
				player.mapNo = mapNo;
				actMap.Change(mapNo);
			};
			(from _ in _playerHitBox.OnTriggerStayAsObservable().TakeUntilDestroy(actScene)
				where ActionInput.isAction
				where !Singleton<Game>.Instance.IsRegulate(true)
				select _ into other
				select other.GetComponent<Player>() into player
				where player != null && !player.isActionNow
				select player).Subscribe(delegate(Player player)
			{
				if (!player.isActionPointToPlayerHit && (!(player.actionTarget != null) || player.actionTarget.heroine.fixCharaID == 0) && !actScene.npcList.Any((NPC p) => p.isLesH))
				{
					if (player.chaser != null && actMap.infoDic[mapNo].isWarning && player.isChaseCancel(mapNo))
					{
						actScene.SceneEvent(player.chaser);
					}
					else
					{
						linkJump(player);
					}
				}
			});
			(from other in _playerHitBox.OnTriggerEnterAsObservable()
				select other.GetComponent<Player>() into player
				where player != null
				select player).Subscribe(delegate(Player player)
			{
				player.hitGateID = ID;
				if (moveType == 0)
				{
					linkReceive(player);
				}
				else if (!linkReceive(player))
				{
					linkJump(player);
				}
			});
			base.enabled = true;
			_playerHitBox.enabled = true;
			_heroineHitBox.enabled = true;
			yield return new WaitWhile(() => miniMapIcon.iconItem == null);
			miniMapIcon.iconItem.text = infoText;
			miniMapIcon.SetVisible(true);
		}
	}
}
