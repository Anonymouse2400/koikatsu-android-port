using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Extensions;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class NickNameSettingScene : BaseLoader
{
	private SaveData.CharaData _charaData;

	[Label("キャンバス")]
	[SerializeField]
	private Canvas Canvas;

	[SerializeField]
	[Label("個別用女情報")]
	private GameObject ObjFemaleInfo;

	[Label("学生証コンポーネント")]
	[SerializeField]
	private StudentCardControlComponent cmpStudentCard;

	[Label("姓表示")]
	[SerializeField]
	private Text LastNameText;

	[SerializeField]
	[Label("名表示")]
	private Text FirstNameText;

	[Label("あだ名表示")]
	[SerializeField]
	private TextMeshProUGUI NickNameText;

	[SerializeField]
	[Label("コピー元ボタン")]
	private Toggle NickNameButton;

	[Label("あだ名(50音頭文字)")]
	[SerializeField]
	private RectTransform NickNameInitialContent;

	[Label("あだ名ウィンドウ")]
	[SerializeField]
	private RectTransform NickNameWindow;

	[Label("あだ名用レイアウトコピー元")]
	[SerializeField]
	private RectTransform NickNameBaseNode;

	[Label("クリアボタン")]
	[SerializeField]
	private Button Clear;

	[Label("決定ボタン")]
	[SerializeField]
	private Button Enter;

	[SerializeField]
	[Label("戻るボタン")]
	private Button Return;

	private HashSet<string> loadBundles = new HashSet<string>();

	public SaveData.CharaData charaData
	{
		get
		{
			return _charaData;
		}
		set
		{
			_charaData = value;
		}
	}

	public event Action onEnter = delegate
	{
	};

	public event Action onCancel = delegate
	{
	};

	public event Action onClear = delegate
	{
	};

	private void Start()
	{
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		CanvasGroup canvasGroup = Canvas.GetComponent<CanvasGroup>();
		this.ObserveEveryValueChanged((NickNameSettingScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
		{
			canvasGroup.interactable = isOn;
		});
		SaveData.Heroine heroine = _charaData as SaveData.Heroine;
		if (heroine == null)
		{
			if ((bool)ObjFemaleInfo)
			{
				ObjFemaleInfo.SetActiveIfDifferent(false);
			}
			if ((bool)Clear)
			{
				Clear.gameObject.SetActiveIfDifferent(false);
			}
		}
		else if ((bool)cmpStudentCard)
		{
			Localize.Translate.Manager.Convert(base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ID_CARD).Get(0).Get(0)
				.Load(true)).SafeProcObject(delegate(Sprite sprite)
			{
				cmpStudentCard.BaseImage.sprite = sprite;
			});
			cmpStudentCard.SetCharaInfo(heroine.charFile, 0, Singleton<Game>.Instance.saveData.accademyName, string.Empty);
		}
		SaveData.Player player = Singleton<Game>.Instance.Player;
		if ((bool)LastNameText)
		{
			LastNameText.text = player.lastname;
		}
		if ((bool)FirstNameText)
		{
			FirstNameText.text = player.firstname;
		}
		this.SelectSEAdd(Enter, Return);
		List<NickName.Param> callNameList = SaveData.GetCallNameList(_charaData);
		Dictionary<int, List<NickName.Param>> dic = callNameList.ToLookup((NickName.Param v) => v.Category, (NickName.Param v) => v).ToDictionary((IGrouping<int, NickName.Param> v) => v.Key, Enumerable.ToList);
		List<NickName.Param> value;
		Func<int, List<NickName.Param>> getList = (int category) => (!dic.TryGetValue(category, out value)) ? new List<NickName.Param>() : value;
		int callID = _charaData.callMyID;
		StringReactiveProperty callName = new StringReactiveProperty(callNameList.Find((NickName.Param p) => p.ID == callID).Name);
		callName.SubscribeWithState(NickNameText, delegate(string text, TextMeshProUGUI t)
		{
			t.text = text;
		});
		List<GameObject> destroyNickNameList = new List<GameObject>();
		Transform nickNameBaseNodeParent = NickNameBaseNode.parent;
		Action<Tuple<string, int>[]> createNickNameButton = delegate(Tuple<string, int>[] categoryData)
		{
			destroyNickNameList.ForEach(delegate(GameObject o)
			{
				UnityEngine.Object.Destroy(o);
			});
			destroyNickNameList.Clear();
			foreach (var item in categoryData.Select((Tuple<string, int> p) => new
			{
				text = p.Item1,
				index = p.Item2
			}))
			{
				List<NickName.Param> list = getList(item.index);
				if (list.Any())
				{
					RectTransform rectTransform = UnityEngine.Object.Instantiate(NickNameBaseNode, nickNameBaseNodeParent, false);
					GameObject gameObject = rectTransform.gameObject;
					gameObject.SetActive(true);
					destroyNickNameList.Add(gameObject);
					Transform child = rectTransform.GetChild(0);
					TextMeshProUGUI componentInChildren = child.GetComponentInChildren<TextMeshProUGUI>();
					bool isOther = item.index == 0;
					if (isOther)
					{
						Localize.Translate.Manager.BindFont(componentInChildren);
					}
					componentInChildren.text = item.text;
					Button nickBaseNameButton = rectTransform.GetComponentInChildren<Button>(true);
					Transform nickBaseNameButtonParent = nickBaseNameButton.transform.parent;
					list.ForEach(delegate(NickName.Param nick)
					{
						Button button = UnityEngine.Object.Instantiate(nickBaseNameButton, nickBaseNameButtonParent, false);
						button.gameObject.SetActive(true);
						TextMeshProUGUI componentInChildren2 = button.GetComponentInChildren<TextMeshProUGUI>();
						if (isOther)
						{
							Localize.Translate.Manager.BindFont(componentInChildren2);
						}
						componentInChildren2.text = nick.Name;
						button.OnClickAsObservable().Subscribe(delegate
						{
							callID = nick.ID;
							callName.Value = nick.Name;
							if (_charaData is SaveData.Heroine)
							{
								SaveData.CallFileData callFileData = SaveData.FindCallFileData(_charaData.personality, nick.ID);
								loadBundles.Add(callFileData.bundle);
								Singleton<Voice>.Instance.OnecePlayChara(_charaData.personality, callFileData.bundle, callFileData.GetFileName(0), _charaData.voicePitch, 0f, 0f, false, null, Voice.Type.PCM, -1, true, false);
							}
						});
					});
				}
			}
		};
		string[][] _50sounds = new string[11][]
		{
			new string[1] { "特殊" },
			new string[5] { "あ", "い", "う", "え", "お" },
			new string[5] { "か", "き", "く", "け", "こ" },
			new string[5] { "さ", "し", "す", "せ", "そ" },
			new string[5] { "た", "ち", "つ", "て", "と" },
			new string[5] { "な", "に", "ぬ", "ね", "の" },
			new string[5] { "は", "ひ", "ふ", "へ", "ほ" },
			new string[5] { "ま", "み", "む", "め", "も" },
			new string[3] { "や", "ゆ", "よ" },
			new string[5] { "ら", "り", "る", "れ", "ろ" },
			new string[3] { "わ", "を", "ん" }
		};
		base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.NICK_NAME).Get(0).Values.FindTagText("Other").SafeProc(delegate(string text)
		{
			_50sounds[0][0] = text;
		});
		bool flag = false;
		int cnt = 0;
		Vector2 nickNameWindowResetPos = NickNameWindow.anchoredPosition;
		if (Localize.Translate.Manager.isTranslate)
		{
			_50sounds = _50sounds.Take(1).ToArray();
		}
		bool flag2 = _50sounds.Length <= 1;
		foreach (var item2 in _50sounds.Select((string[] p) => p.Select((string text, int index) => new
		{
			text = text,
			index = cnt++
		}).ToArray()))
		{
			var anon = item2[0];
			Tuple<string, int>[] categoryData2 = item2.Select(p => Tuple.Create(p.text, p.index)).ToArray();
			Toggle toggle = null;
			if (!flag2)
			{
				toggle = UnityEngine.Object.Instantiate(NickNameButton, NickNameInitialContent, false);
				toggle.gameObject.SetActive(true);
			}
			TextMeshProUGUI textMeshProUGUI = null;
			if (toggle != null)
			{
				textMeshProUGUI = toggle.GetComponentInChildren<TextMeshProUGUI>();
				textMeshProUGUI.text = anon.text;
				(from isOn in toggle.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					NickNameWindow.anchoredPosition = nickNameWindowResetPos;
					createNickNameButton(categoryData2);
				});
			}
			if (anon.index == 0)
			{
				Localize.Translate.Manager.BindFont(textMeshProUGUI);
			}
			if (!flag)
			{
				if (toggle != null)
				{
					toggle.isOn = true;
				}
				createNickNameButton(categoryData2);
				flag = true;
			}
		}
		Enter.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			_charaData.callID = callID;
			_charaData.callName = callName.Value;
			SaveData.CallNormalize(_charaData);
			foreach (string loadBundle in loadBundles)
			{
				AssetBundleManager.UnloadAssetBundle(loadBundle, true);
			}
			this.onEnter();
		});
		Clear.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			NickName.Param callName2 = SaveData.GetCallName(Singleton<Game>.Instance.Player);
			callID = callName2.ID;
			callName.Value = callName2.Name;
			this.onClear();
		});
		Return.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			foreach (string loadBundle2 in loadBundles)
			{
				AssetBundleManager.UnloadAssetBundle(loadBundle2, true);
			}
			this.onCancel();
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1) && Return.interactable
			where Singleton<Scene>.Instance.AddSceneName == "NickNameSetting"
			select _).Take(1).Subscribe(delegate
		{
			foreach (string loadBundle3 in loadBundles)
			{
				AssetBundleManager.UnloadAssetBundle(loadBundle3, true);
			}
			this.onCancel();
		});
	}
}
