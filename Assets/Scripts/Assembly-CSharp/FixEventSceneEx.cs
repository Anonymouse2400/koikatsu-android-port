using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame;
using CharaFiles;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixEventSceneEx : BaseLoader
{
	private class ADVParam : SceneParameter
	{
		public bool isInVisibleChara = true;

		public ADVParam(MonoBehaviour scene)
			: base(scene)
		{
		}

		public override void Init(ADV.Data data)
		{
			ADVScene aDVScene = SceneParameter.advScene;
			TextScenario scenario = aDVScene.Scenario;
			scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
			scenario.LoadBundleName = data.bundleName;
			scenario.LoadAssetName = data.assetName;
			aDVScene.Stand.SetPositionAndRotation(data.position, data.rotation);
			scenario.heroineList = data.heroineList;
			scenario.transferList = data.transferList;
			if (!data.heroineList.IsNullOrEmpty())
			{
				scenario.currentChara = new CharaData(data.heroineList[0], scenario, null);
			}
			if (data.camera != null)
			{
				scenario.BackCamera.transform.SetPositionAndRotation(data.camera.position, data.camera.rotation);
			}
			float fadeInTime = data.fadeInTime;
			if (fadeInTime > 0f)
			{
				aDVScene.fadeTime = fadeInTime;
			}
			else
			{
				isInVisibleChara = false;
			}
		}

		public override void Release()
		{
			ADVScene aDVScene = SceneParameter.advScene;
			aDVScene.gameObject.SetActive(false);
		}

		public override void WaitEndProc()
		{
		}
	}

	[SerializeField]
	private BGM bgm = BGM.Memories;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private CanvasGroup cgFixEvent;

	[SerializeField]
	private GameObject[] objCharaLoad;

	[SerializeField]
	private ChaFileListCtrl[] cmpChaFileListCtrl;

	[SerializeField]
	private Button[] btnChaLoad;

	[SerializeField]
	private Button[] btnCancel;

	private int editNo;

	[SerializeField]
	private Toggle[] tglMainMenu;

	[SerializeField]
	private GameObject[] objMenuTarget;

	[SerializeField]
	private Button[] btnCharaInit;

	[SerializeField]
	private Button[] btnEdit;

	[SerializeField]
	private GameObject[] objCard;

	private RawImage[] imgCard;

	[SerializeField]
	private GameObject[] objStand;

	[SerializeField]
	private TextMeshProUGUI[] textTitle;

	[SerializeField]
	private Toggle[] tglCha00Event;

	[SerializeField]
	private Toggle[] tglCha01Event;

	[SerializeField]
	private Toggle[] tglCha02Event;

	[SerializeField]
	private Toggle[] tglCha03Event;

	[SerializeField]
	private Button btnBack;

	[SerializeField]
	private Button btnOK;

	protected override void Awake()
	{
		base.Awake();
		ParameterList.Add(new ADVParam(this));
	}

	private void OnDestroy()
	{
		ParameterList.Remove(this);
	}

	private void Start()
	{
		cgFixEvent.blocksRaycasts = true;
		Utils.Sound.Play(new Utils.Sound.SettingBGM(bgm));
		List<SaveData.Heroine> fixInitCharaDataList = Game.CreateFixCharaList();
		MemoriesData memData = new MemoriesData();
		memData.Initialize();
		memData.Load();
		memData.CreateFixHeroines();
		CreateCharaList();
		Dictionary<int, Dictionary<int, Localize.Translate.Data.Param>> translater = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.EXTRA_EVENT);
		Dictionary<int, List<EventInfo.Param>> evInfos = new Dictionary<int, List<EventInfo.Param>>();
		CommonLib.GetAssetBundleNameListFromPath("action/list/event/", true).ForEach(delegate(string file)
		{
			EventInfo[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(EventInfo)).GetAllAssets<EventInfo>();
			foreach (EventInfo eventInfo in allAssets)
			{
				int result;
				int.TryParse(eventInfo.name, out result);
				List<EventInfo.Param> value;
				if (!evInfos.TryGetValue(result, out value))
				{
					value = (evInfos[result] = new List<EventInfo.Param>());
				}
				Dictionary<int, Localize.Translate.Data.Param> categorys = translater.Get(result);
				value.AddRange(eventInfo.param.Select(delegate(EventInfo.Param p)
				{
					categorys.SafeGetText(p.ID).SafeProc(delegate(string text)
					{
						p.Name = text;
					});
					return p;
				}));
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
		HashSet<int> fixCharaTaked = Singleton<Game>.Instance.glSaveData.fixCharaTaked;
		for (int k = 0; k < tglMainMenu.Length - 1; k++)
		{
			tglMainMenu[k].interactable = fixCharaTaked.Contains(ConvertIDtoFixID(k));
		}
		IntReactiveProperty mainPage = new IntReactiveProperty(tglMainMenu.Check((Toggle tgl) => tgl.interactable));
		for (int l = 0; l < tglMainMenu.Length; l++)
		{
			objMenuTarget[l].SetActive(false);
			tglMainMenu[l].isOn = l == mainPage.Value;
		}
		mainPage.Skip(1).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
		});
		mainPage.Scan(delegate(int prev, int current)
		{
			objMenuTarget[prev].SetActive(false);
			return current;
		}).Subscribe(delegate(int i)
		{
			objMenuTarget[i].SetActive(true);
		});
		(from list in tglMainMenu.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
			select list.IndexOf(true) into i
			where i >= 0
			select i).Subscribe(delegate(int i)
		{
			mainPage.Value = i;
		});
		Toggle[][] subToggles = new Toggle[4][] { tglCha00Event, tglCha01Event, tglCha02Event, tglCha03Event };
		IntReactiveProperty subPage = new IntReactiveProperty(-1);
		bool isPlaySE = true;
		(from _ in subPage.Skip(1)
			where isPlaySE
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
		});
		mainPage.Where((int i) => i < subToggles.Length).Subscribe(delegate(int i)
		{
			isPlaySE = false;
			subPage.Value = subToggles[i].Check((Toggle t) => t.isOn);
			isPlaySE = true;
		});
		Toggle[][] array = subToggles;
		foreach (Toggle[] source in array)
		{
			(from list in source.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).Subscribe(delegate(int i)
			{
				subPage.Value = i;
			});
		}
		(from page in subPage.Select((int _) => mainPage.Value).Merge(mainPage)
			where page < 4
			select page).Subscribe(delegate(int page)
		{
			SaveData.Heroine heroine = ConvertIDtoChara(page, memData) as SaveData.Heroine;
			EventInfo.Param param = evInfos[heroine.fixCharaID][subPage.Value];
			textTitle[page].text = param.Name;
		});
		imgCard = new RawImage[objCard.Length];
		for (int n = 0; n < objCard.Length; n++)
		{
			imgCard[n] = objCard[n].GetComponent<RawImage>();
			byte[] array2 = null;
			if (n == 7)
			{
				array2 = memData.etcData[-100].pngData;
			}
			else
			{
				SaveData.Heroine heroine2 = ConvertIDtoChara(n, memData) as SaveData.Heroine;
				array2 = memData.etcData[heroine2.fixCharaID].pngData;
			}
			objCard[n].SetActiveIfDifferent(null != array2);
			objStand[n].SetActiveIfDifferent(null == array2);
			if (array2 != null)
			{
				if ((bool)imgCard[n].texture)
				{
					UnityEngine.Object.Destroy(imgCard[n].texture);
				}
				Texture2D texture2D = new Texture2D(0, 0);
				texture2D.LoadImage(array2);
				imgCard[n].texture = texture2D;
			}
		}
		btnCharaInit.Select((Button p, int i) => new
		{
			btn = p,
			index = i
		}).ToList().ForEach(p =>
		{
			p.btn.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
				CheckScene.Parameter param2 = new CheckScene.Parameter();
				param2.Yes = delegate
				{
					SaveData.CharaData charaData = ConvertIDtoChara(p.index, memData);
					if (charaData is SaveData.Heroine)
					{
						SaveData.Heroine heroine3 = charaData as SaveData.Heroine;
						SaveData.Heroine heroine4 = fixInitCharaDataList.FirstOrDefault((SaveData.Heroine c) => c.fixCharaID == heroine3.fixCharaID);
						ChaFile.CopyChaFile(heroine3.charFile, heroine4.charFile);
						memData.etcData[heroine3.fixCharaID].initialized = true;
						memData.etcData[heroine3.fixCharaID].pngData = null;
					}
					else
					{
						memData.CreatePlayer();
						memData.etcData[-100].initialized = true;
						memData.etcData[-100].pngData = null;
					}
					memData.Save();
					Singleton<Scene>.Instance.UnLoad();
					objStand[p.index].SetActiveIfDifferent(true);
					objCard[p.index].SetActiveIfDifferent(false);
				};
				param2.No = delegate
				{
					Singleton<Scene>.Instance.UnLoad();
				};
				param2.Title = translater.Get(0).Values.FindTagText("InitCheck") ?? "容姿を初期化しますか？";
				Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param2, observer)).StartAsCoroutine();
			});
		});
		btnEdit.Select((Button p, int i) => new
		{
			btn = p,
			index = i
		}).ToList().ForEach(p =>
		{
			p.btn.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
				objCharaLoad[0].SetActiveIfDifferent(7 == p.index);
				objCharaLoad[1].SetActiveIfDifferent(7 != p.index);
				editNo = p.index;
			});
		});
		Action close = delegate
		{
			cgFixEvent.blocksRaycasts = false;
			Utils.Sound.Play(SystemSE.cancel);
			Singleton<Scene>.Instance.UnLoad();
		};
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where !Singleton<Scene>.Instance.IsNowLoadingFade
			where !objCharaLoad[0].activeInHierarchy && !objCharaLoad[1].activeInHierarchy
			where Singleton<Scene>.Instance.NowSceneNames[0] == "FixEventSceneEx"
			select _).Take(1).Subscribe(delegate
		{
			close();
		});
		btnBack.OnClickAsObservable().Take(1).Subscribe(delegate
		{
			close();
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where !Singleton<Scene>.Instance.IsNowLoadingFade
			where objCharaLoad[0].activeInHierarchy || objCharaLoad[1].activeInHierarchy
			where Singleton<Scene>.Instance.NowSceneNames[0] == "FixEventSceneEx"
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			objCharaLoad[0].SetActiveIfDifferent(false);
			objCharaLoad[1].SetActiveIfDifferent(false);
		});
		mainPage.Select((int i) => i < subToggles.Length).SubscribeToInteractable(btnOK);
		btnOK.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			Observable.FromCoroutine((CancellationToken __) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In)).Subscribe(delegate
			{
				EventSystem.current.SetSelectedGameObject(null);
				SaveData.Heroine heroine5 = ConvertIDtoChara(mainPage.Value, memData) as SaveData.Heroine;
				EventInfo.Param param3 = evInfos[heroine5.fixCharaID][subPage.Value];
				Singleton<Game>.Instance.HeroineList.Clear();
				Singleton<Game>.Instance.HeroineList.AddRange(memData.heroineDic.Values);
				List<Program.Transfer> list3 = Program.Transfer.NewList();
				Program.SetParam(heroine5, list3);
				SaveData.Player player = Singleton<Game>.Instance.Player;
				player.SetCharFile(memData.player.charFile);
				Program.SetParam(player, list3);
				list3.Add(Program.Transfer.Create(true, Command.CameraLock, bool.FalseString));
				list3.Add(Program.Transfer.Open(param3.Bundle, param3.Asset));
				StartCoroutine(Program.Open(new ADV.Data
				{
					position = Vector3.zero,
					rotation = Quaternion.identity,
					scene = this,
					transferList = list3,
					heroineList = new List<SaveData.Heroine> { heroine5 }
				}, new Program.OpenDataProc
				{
					onLoad = delegate
					{
						StartCoroutine(Wait());
					}
				}));
			});
		});
		btnChaLoad.Select((Button p, int i) => new
		{
			btn = p,
			index = i
		}).ToList().ForEach(p =>
		{
			p.btn.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.ok_s);
				objCharaLoad[0].SetActiveIfDifferent(false);
				objCharaLoad[1].SetActiveIfDifferent(false);
				ChaFileInfoComponent selectTopItem = cmpChaFileListCtrl[p.index].GetSelectTopItem();
				if (null != selectTopItem)
				{
					ChaFileControl chaFileControl = new ChaFileControl();
					chaFileControl.LoadCharaFile(selectTopItem.info.FullPath, (byte)p.index);
					SaveData.CharaData charaData2 = ConvertIDtoChara(editNo, memData);
					charaData2.charFile.pngData = chaFileControl.pngData;
					charaData2.charFile.SetCustomBytes(chaFileControl.GetCustomBytes(), ChaFileDefine.ChaFileCustomVersion);
					charaData2.charFile.SetCoordinateBytes(chaFileControl.GetCoordinateBytes(), ChaFileDefine.ChaFileCoordinateVersion);
					charaData2.charFile.parameter.firstname = chaFileControl.parameter.firstname;
					charaData2.charFile.parameter.lastname = chaFileControl.parameter.lastname;
					if ((bool)imgCard[editNo].texture)
					{
						UnityEngine.Object.Destroy(imgCard[editNo].texture);
					}
					Texture2D texture2D2 = new Texture2D(180, 240);
					texture2D2.LoadImage(charaData2.charFile.pngData);
					imgCard[editNo].texture = texture2D2;
					if (p.index == 0)
					{
						memData.etcData[-100].initialized = false;
						memData.etcData[-100].pngData = charaData2.charFile.pngData;
					}
					else
					{
						SaveData.Heroine heroine6 = ConvertIDtoChara(editNo, memData) as SaveData.Heroine;
						memData.etcData[heroine6.fixCharaID].initialized = false;
						memData.etcData[heroine6.fixCharaID].pngData = charaData2.charFile.pngData;
					}
					memData.Save();
					objStand[editNo].SetActiveIfDifferent(false);
					objCard[editNo].SetActiveIfDifferent(true);
				}
			});
		});
		btnCancel.Select((Button p, int i) => new
		{
			btn = p,
			index = i
		}).ToList().ForEach(p =>
		{
			p.btn.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
				objCharaLoad[0].SetActiveIfDifferent(false);
				objCharaLoad[1].SetActiveIfDifferent(false);
			});
		});
		Observable.EveryUpdate().Subscribe(delegate
		{
			for (int num = 0; num < 2; num++)
			{
				bool interactable = !(null == cmpChaFileListCtrl[num].GetSelectTopItem());
				btnChaLoad[num].interactable = interactable;
			}
		}).AddTo(this);
	}

	private void CreateCharaList()
	{
		for (int i = 0; i < 2; i++)
		{
			foreach (var item in Localize.Translate.Manager.CreateChaFileInfo(i, true).Select((Localize.Translate.Manager.ChaFileInfo p, int index) => new { p, index }))
			{
				cmpChaFileListCtrl[i].AddList(new ChaFileInfo(item.p.chaFile, item.p.info)
				{
					index = item.index
				});
			}
			cmpChaFileListCtrl[i].Create(null);
		}
	}

	private  IEnumerator Wait()
	{
		canvas.enabled = false;
		yield return null;
        yield return new WaitWhile(() => Program.isADVScene);
        if (!Scene.isReturnTitle && !Scene.isGameEnd)
		{
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			canvas.enabled = true;
			Utils.Sound.Play(new Utils.Sound.SettingBGM(bgm));
		}
	}

	private void InitToggles(Toggle[] toggles, int n)
	{
		for (int i = 0; i < toggles.Length; i++)
		{
			toggles[i].isOn = i == n;
		}
	}

	private SaveData.CharaData ConvertIDtoChara(int editNo, MemoriesData memData)
	{
		int num = ConvertIDtoFixID(editNo);
		if (num == int.MaxValue)
		{
			return memData.player;
		}
		return memData.heroineDic[num];
	}

	private int ConvertIDtoFixID(int editNo)
	{
		int result = int.MaxValue;
		switch (editNo)
		{
		case 0:
			result = -5;
			break;
		case 1:
			result = -8;
			break;
		case 2:
			result = -9;
			break;
		case 3:
			result = -10;
			break;
		case 4:
			result = -1;
			break;
		case 5:
			result = -4;
			break;
		case 6:
			result = -2;
			break;
		}
		return result;
	}
}
