using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion.CustomAttributes;
using Illusion.Game;
using Illusion.Game.Extensions;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectScene : BaseLoader
{
	private class SendHeroineData
	{
		public SaveData.Heroine heroine;

		public Button button;
	}

	[SerializeField]
	[Label("キャンバス")]
	private Canvas Canvas;

	[SerializeField]
	[Label("決定ボタン")]
	private Button EnterButton;

	[SerializeField]
	[Label("戻るボタン")]
	private Button ReturnButton;

	[SerializeField]
	[Label("キャラのコンテンツ")]
	private GameObject charaContent;

	[SerializeField]
	[Label("キャラのノード")]
	private GameObject charaNode;

	[SerializeField]
	[Label("選択ボタン色")]
	private Color selectColor = new Color(1f, 1f, 0f);

	[Label("ロード開始")]
	public bool isLoadStart;

	private Color baseColor;

	private CanvasGroup canvasGroup;

	public Action<SaveData.Heroine> EnterProc;

	public Action ReturnProc;

	public Func<SaveData.Heroine, bool> LoadCondition;

	protected override void Awake()
	{
		base.Awake();
		EnterButton.gameObject.SetActive(false);
		ReturnButton.gameObject.SetActive(false);
	}

	private IEnumerator Start()
	{
		canvasGroup = Canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((CharacterSelectScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		yield return new WaitWhile(() => !isLoadStart);
		if (LoadCondition.IsNullOrEmpty())
		{
			LoadCondition = (SaveData.Heroine _) => true;
		}
		ColorBlock baseColors = charaNode.GetComponentInChildren<Button>().colors;
		Func<ColorBlock, ColorBlock> bkColorFunc = delegate(ColorBlock sc)
		{
			sc.normalColor = baseColors.normalColor;
			sc.highlightedColor = baseColors.highlightedColor;
			return sc;
		};
		Func<Color, ColorBlock, ColorBlock> selectColorFunc = delegate(Color sc, ColorBlock src)
		{
			Color highlightedColor = (src.normalColor = sc);
			src.highlightedColor = highlightedColor;
			return src;
		};
		List<SaveData.Heroine> addCharaList = new List<SaveData.Heroine>();
		if (Game.SaveFileName == string.Empty)
		{
			string[] allFile = Directory.GetFiles(UserData.Path + "chara/female/", "*.png");
			string[] array = allFile;
			foreach (string path in array)
			{
				SaveData.Heroine heroine2 = new SaveData.Heroine(true);
				heroine2.charFile.LoadCharaFile(path, 1, true);
				heroine2.fixCharaID = 0;
				addCharaList.Add(heroine2);
				if (!Singleton<Scene>.Instance.sceneFade.canvas.gameObject.activeSelf)
				{
					Singleton<Scene>.Instance.sceneFade.canvas.gameObject.SetActive(true);
				}
				Singleton<Scene>.Instance.DrawImageAndProgress((float)addCharaList.Count / (float)allFile.Length, 1f);
				yield return null;
			}
		}
		else
		{
			Singleton<Scene>.Instance.DrawImageAndProgress(-1f, 1f);
			addCharaList.AddRange(Singleton<Game>.Instance.saveData.heroineList);
		}
		int index = -1;
		var filterHeroineList = addCharaList.Where((SaveData.Heroine heroine) => LoadCondition(heroine)).Select((SaveData.Heroine p, int i) => new { p, i }).ToList();
		SendHeroineData[] contactArray = new SendHeroineData[filterHeroineList.Count];
		filterHeroineList.ForEach(h =>
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(charaNode);
			gameObject.SetActive(true);
			gameObject.transform.SetParent(charaContent.transform, false);
			Button bt = gameObject.GetComponentInChildren<Button>();
			contactArray[h.i] = new SendHeroineData
			{
				heroine = h.p,
				button = bt
			};
			bt.GetComponentInChildren<Text>().text = h.p.Name;
			(from _ in bt.OnClickAsObservable()
				select bt).Subscribe(delegate(Button sel)
			{
				if ((uint)index < contactArray.Length)
				{
					contactArray[index].button.colors = bkColorFunc(contactArray[index].button.colors);
				}
				index = h.i;
				sel.colors = selectColorFunc(selectColor, sel.colors);
			});
		});
		Singleton<Scene>.Instance.DrawImageAndProgress(-1f, -1f, false);
		Singleton<Scene>.Instance.sceneFade.canvas.gameObject.SetActive(false);
		EnterButton.gameObject.SetActive(true);
		EnterButton.OnClickAsObservable().Subscribe(delegate
		{
			if ((uint)index < contactArray.Length)
			{
				EnterProc.Call(contactArray[index].heroine);
			}
		});
		if (!ReturnProc.IsNullOrEmpty())
		{
			ReturnButton.OnClickAsObservable().Subscribe(delegate
			{
				ReturnProc();
			});
			ReturnButton.gameObject.SetActive(true);
		}
		if (!ReturnButton.onClick.IsNullOrEmpty())
		{
			ReturnButton.gameObject.SetActive(true);
		}
		List<Button> list = new List<Button>(contactArray.Select((SendHeroineData p) => p.button));
		list.ForEach(delegate(Button p)
		{
			p.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
			});
		});
		list.AddRange(new Button[2] { EnterButton, ReturnButton }.Where((Button p) => p.gameObject.activeSelf));
		this.SelectSEAdd(list.ToArray());
		if (EnterButton.gameObject.activeSelf)
		{
			EnterButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
			});
		}
		if (ReturnButton.gameObject.activeSelf)
		{
			ReturnButton.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
			});
		}
	}
}
