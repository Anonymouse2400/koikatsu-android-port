using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using Illusion.CustomAttributes;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class StaffRoomMenuScene : BaseLoader
{
	[SerializeField]
	[Label("キャンバス")]
	private Canvas canvas;

	[SerializeField]
	[Label("学生証")]
	private StudentCardControlComponent card;

	[Label("マップ画像")]
	[SerializeField]
	private Image mapImage;

	[SerializeField]
	[Label("決定")]
	private Button enterButton;

	[SerializeField]
	[Label("戻る")]
	private Button returnButton;

	private ReactiveProperty<SaveData.Heroine> resultHeroine = new ReactiveProperty<SaveData.Heroine>();

	private ReactiveProperty<MapInfo.Param> resultMap = new ReactiveProperty<MapInfo.Param>();

	public BaseMap baseMap;

	public void SetHeroine(SaveData.Heroine _heroine)
	{
		resultHeroine.Value = _heroine;
	}

	public void SetMapInfo(MapInfo.Param _mapParam)
	{
		resultMap.Value = _mapParam;
	}

	private void AddScene(string assetBundleName, string levelName, Action onLoad)
	{
		Singleton<Scene>.Instance.LoadReserve(new Scene.Data
		{
			assetBundleName = assetBundleName,
			levelName = levelName,
			isAdd = true,
			onLoad = onLoad
		}, false);
	}

	private IEnumerator Start()
	{
		ActionScene actScene = Singleton<Game>.Instance.actScene;
		baseMap = actScene.Map;
		if (Utils.Scene.OpenTutorial(6))
		{
			yield return new WaitWhile(Utils.Scene.IsTutorial);
		}
		Localize.Translate.Manager.Convert(base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ID_CARD).Get(0).Get(0)
			.Load(true)).SafeProcObject(delegate(Sprite sprite)
		{
			card.BaseImage.sprite = sprite;
		});
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((StaffRoomMenuScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		resultHeroine.Subscribe(delegate(SaveData.Heroine heroine)
		{
			card.gameObject.SetActive(heroine != null);
			SaveData saveData = Singleton<Game>.Instance.saveData;
			if (heroine != null)
			{
				card.SetCharaInfo(heroine.charFile, saveData.emblemID, saveData.accademyName, string.Empty);
			}
		});
		Dictionary<int, Data.Param> thumbnailLT = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.MAP).Get(0);
		resultMap.Subscribe(delegate(MapInfo.Param info)
		{
			mapImage.gameObject.SetActive(info != null);
			if (info != null)
			{
				string bundle = info.ThumbnailBundle;
				string asset = info.ThumbnailAsset;
				thumbnailLT.SafeGet(info.No).SafeProc(delegate(Data.Param param)
				{
					bundle = param.Bundle;
					asset = param.asset;
				});
				Utils.Bundle.LoadSprite(bundle, asset, mapImage, true);
			}
		});
		(from list in Observable.CombineLatest<bool>(resultHeroine.Select((SaveData.Heroine p) => p != null), resultMap.Select((MapInfo.Param p) => p != null))
			select list.All((bool result) => result)).SubscribeToInteractable(enterButton);
		enterButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			NPC.Summon(resultHeroine.Value, actScene, resultMap.Value.No);
			Singleton<Scene>.Instance.UnLoad();
		});
		returnButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			Singleton<Scene>.Instance.UnLoad();
		});
		(from _ in this.UpdateAsObservable()
			where canvas.enabled
			where Singleton<Scene>.Instance.NowSceneNames[0] == "StaffRoomMenu"
			where Input.GetMouseButtonDown(1)
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			Singleton<Scene>.Instance.UnLoad();
		});
		yield return null;
	}
}
