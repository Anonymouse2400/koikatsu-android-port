using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class CallSelectScene : BaseLoader
	{
		[SerializeField]
		private GameObject objAddress;

		[SerializeField]
		private GameObject objCheck;

		[SerializeField]
		private GameObject objGirlFriendTmp;

		[SerializeField]
		private Button btnCall;

		[SerializeField]
		private Button btnEnd;

		[SerializeField]
		private RawImage imgChara;

		[SerializeField]
		private Text textFullName;

		[SerializeField]
		private Button btnYes;

		[SerializeField]
		private Button btnNo;

		private void Start()
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			Transform parent = objGirlFriendTmp.transform.parent;
			Tuple<SaveData.Heroine, Toggle>[] infos = (from npc in actScene.npcList
				select npc.heroine into p
				where p.isGirlfriend
				select p).Select(delegate(SaveData.Heroine item)
			{
				GameObject gameObject = Object.Instantiate(objGirlFriendTmp, parent, false);
				Toggle component = gameObject.GetComponent<Toggle>();
				component.isOn = false;
				Transform transform = gameObject.transform.Find("FullName");
				Text text = null;
				if (transform != null)
				{
					text = transform.GetComponent<Text>();
					if (text != null)
					{
						text.text = item.parameter.fullname;
					}
				}
				if (item.isAnger || item.talkTime <= 0)
				{
					component.interactable = false;
					if ((bool)text)
					{
						text.color = Color.gray;
					}
				}
				gameObject.SetActiveIfDifferent(true);
				return Tuple.Create(item, component);
			}).ToArray();
			ReactiveProperty<SaveData.Heroine> selHeroine = new ReactiveProperty<SaveData.Heroine>();
			(from list in infos.Select((Tuple<SaveData.Heroine, Toggle> p) => p.Item2.OnValueChangedAsObservable()).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).Subscribe(delegate(int i)
			{
				selHeroine.Value = infos[i].Item1;
			});
			btnEnd.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
				Singleton<Scene>.Instance.UnLoad();
			});
			selHeroine.Select((SaveData.Heroine heroine) => heroine != null).SubscribeToInteractable(btnCall);
			btnCall.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
				SaveData.Heroine value = selHeroine.Value;
				textFullName.text = value.parameter.fullname;
				if ((bool)imgChara.texture)
				{
					Object.Destroy(imgChara.texture);
				}
				Texture2D texture2D = new Texture2D(180, 240);
				texture2D.LoadImage(value.charFile.facePngData);
				imgChara.texture = texture2D;
				objAddress.SetActiveIfDifferent(false);
				objCheck.SetActiveIfDifferent(true);
			});
			btnNo.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
				objAddress.SetActiveIfDifferent(true);
				objCheck.SetActiveIfDifferent(false);
			});
			btnYes.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
				NPC.Summon(selHeroine.Value, actScene, null);
				Singleton<Scene>.Instance.UnLoad();
			});
			(from _ in this.UpdateAsObservable()
				where Input.GetMouseButtonDown(1)
				where Singleton<Scene>.Instance.NowSceneNames[0] == "CallSelect"
				select _).Subscribe(delegate
			{
				if (objCheck.activeSelf)
				{
					btnNo.onClick.Invoke();
				}
				else
				{
					Singleton<Scene>.Instance.UnLoad();
				}
			});
		}
	}
}
