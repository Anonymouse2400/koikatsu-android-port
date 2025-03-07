using System.Collections;
using System.Collections.Generic;
using ActionGame.Chara;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectMenuScene : BaseLoader
{
	public enum ResultType
	{
		None = 0,
		EnterMapMove = 1,
		Wait = 2
	}

	public enum VisibleType
	{
		None = 0,
		Route = 1,
		FreeH = 2
	}

	private ResultType _result;

	private ReactiveProperty<MapInfo.Param> _mapInfo = new ReactiveProperty<MapInfo.Param>();

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Button enterButton;

	[SerializeField]
	private Button returnButton;

	[SerializeField]
	private Color enterColor = Color.yellow;

	[SerializeField]
	private VisibleType _visibleType;

	[SerializeField]
	private GameObject nodeFrame;

	public ResultType result
	{
		get
		{
			return _result;
		}
		set
		{
			_result = value;
		}
	}

	public MapInfo.Param mapInfo { get; private set; }

	public VisibleType visibleType
	{
		set
		{
			_visibleType = value;
		}
	}

	public BaseMap baseMap { get; set; }

	private IEnumerator Start()
	{
		base.enabled = false;
		_mapInfo.Select((MapInfo.Param p) => p != null).SubscribeToInteractable(enterButton);
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((MapSelectMenuScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		yield return new WaitWhile(() => baseMap == null);
		Dictionary<int, Data.Param> thumbnailLT = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.MAP).Get(0);
		int childCount = nodeFrame.transform.childCount;
		GameObject currentFrame = null;
		ReactiveProperty<Button> enterMapColor = new ReactiveProperty<Button>();
		enterMapColor.Scan(delegate(Button prev, Button current)
		{
			if (prev != null)
			{
				prev.colors = current.colors;
			}
			ColorBlock colors = current.colors;
			colors.normalColor = enterColor;
			colors.highlightedColor = enterColor;
			current.colors = colors;
			return current;
		}).Subscribe();
		int i = 0;
		foreach (MapInfo.Param mapInfo in baseMap.infoDic.Values)
		{
			switch (_visibleType)
			{
			case VisibleType.Route:
				if (!mapInfo.isGate)
				{
					continue;
				}
				break;
			case VisibleType.FreeH:
				if (!mapInfo.isFreeH)
				{
					continue;
				}
				break;
			}
			int num = i++ % childCount;
			if (num == 0)
			{
				currentFrame = Object.Instantiate(nodeFrame, nodeFrame.transform.parent, false);
				currentFrame.SetActive(true);
			}
			GameObject gameObject = currentFrame.transform.GetChild(num).gameObject;
			gameObject.SetActive(true);
			Button button = gameObject.GetComponent<Button>();
			string bundle = mapInfo.ThumbnailBundle;
			string asset = mapInfo.ThumbnailAsset;
			thumbnailLT.SafeGet(mapInfo.No).SafeProc(delegate(Data.Param param)
			{
				bundle = param.Bundle;
				asset = param.asset;
			});
			Utils.Bundle.LoadSprite(bundle, asset, button.GetComponent<Image>(), true);
			(from _ in button.OnClickAsObservable()
				select mapInfo).Subscribe(delegate(MapInfo.Param info)
			{
				enterMapColor.Value = button;
				Utils.Sound.Play(SystemSE.sel);
				_mapInfo.Value = info;
			});
		}
		yield return new WaitWhile(() => _result == ResultType.None);
		enterButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			this.mapInfo = _mapInfo.Value;
			ResultType resultType = result;
			switch (resultType)
			{
			case ResultType.EnterMapMove:
			{
				ActionScene actScene = Singleton<Game>.Instance.actScene;
				Player player = actScene.Player;
				actScene.Map.PlayerMapWarp(this.mapInfo.No, delegate
				{
					if (player.chaser != null && player.chaser.mapNo == player.mapNo)
					{
						player.chaser.mapNo = this.mapInfo.No;
					}
					Singleton<Scene>.Instance.UnLoad();
				});
				break;
			}
			case ResultType.Wait:
				Singleton<Scene>.Instance.UnLoad();
				break;
			}
		});
		returnButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			Singleton<Scene>.Instance.UnLoad();
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where Singleton<Scene>.Instance.NowSceneNames[0] == "MapSelectMenu"
			select _).Subscribe(delegate
		{
			Singleton<Scene>.Instance.UnLoad();
		});
		base.enabled = true;
	}
}
