using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionGame.H;
using FreeH;
using Illusion.CustomAttributes;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class FreeHScene : BaseLoader
{
	public class Member
	{
		public BaseMap map;

		public ReactiveProperty<SaveData.Heroine> resultHeroine;

		public ReactiveProperty<SaveData.Heroine> resultPartner;

		public ReactiveProperty<SaveData.Player> resultPlayer;

		public ReactiveProperty<MapInfo.Param> resultMapInfo;

		public IntReactiveProperty resultTimeZone;

		public IntReactiveProperty resultStage1;

		public IntReactiveProperty resultStage2;

		public IntReactiveProperty resultStatus;

		public BoolReactiveProperty resultDiscovery;
	}

	[SerializeField]
	[Label("キャンバス")]
	private Canvas canvas;

	[Label("行動選択オブジェクト")]
	[SerializeField]
	[Header("3Pあり")]
	private GameObject objAcionSelect3P;

	[SerializeField]
	[Label("通常タブ")]
	private Toggle tglNormal3P;

	[SerializeField]
	[Label("オナニータブ")]
	private Toggle tglMasturbation3P;

	[SerializeField]
	[Label("レズタブ")]
	private Toggle tglLesbian3P;

	[SerializeField]
	[Label("3Pタブ")]
	private Toggle tgl3P;

	[Header("3Pなし")]
	[SerializeField]
	[Label("行動選択オブジェクト")]
	private GameObject objAcionSelect;

	[SerializeField]
	[Label("通常タブ")]
	private Toggle tglNormal;

	[Label("オナニータブ")]
	[SerializeField]
	private Toggle tglMasturbation;

	[SerializeField]
	[Label("レズタブ")]
	private Toggle tglLesbian;

	[Header("通常")]
	[SerializeField]
	[Label("通常親")]
	private GameObject objNormal;

	[SerializeField]
	[Label("女学生証")]
	private CharaHInfoComponent cardFemaleNormal;

	[Label("男学生証")]
	[SerializeField]
	private CharaHInfoComponent cardMaleNormal;

	[SerializeField]
	[Label("マップ画像")]
	private Image mapImageNormal;

	[SerializeField]
	[Label("女選択")]
	private Button femaleSelectButtonNormal;

	[Label("男選択")]
	[SerializeField]
	private Button maleSelectButtonNormal;

	[SerializeField]
	[Label("マップ選択")]
	private Button mapSelectButtonNormal;

	[Label("決定")]
	[SerializeField]
	private Button enterButton;

	[SerializeField]
	[Label("戻る")]
	private Button returnButton;

	[SerializeField]
	[Label("昼")]
	private Toggle tglNoonNormal;

	[SerializeField]
	[Label("夕方")]
	private Toggle tglEveningNormal;

	[SerializeField]
	[Label("夜")]
	private Toggle tglNightNormal;

	[SerializeField]
	[Label("初めて")]
	private Toggle tglFirstNormal;

	[SerializeField]
	[Label("不慣れ")]
	private Toggle tglInexperienceNormal;

	[SerializeField]
	[Label("慣れ")]
	private Toggle tglExprienceNormal;

	[SerializeField]
	[Label("淫乱")]
	private Toggle tglLewdnessNormal;

	[SerializeField]
	[Label("安全日")]
	private Toggle tglSafeNormal;

	[SerializeField]
	[Label("危険日")]
	private Toggle tglDangerNormal;

	[SerializeField]
	[Label("オナニー親")]
	[Header("オナニー")]
	private GameObject objMasturbation;

	[SerializeField]
	[Label("女学生証")]
	private CharaHInfoComponent cardFemaleMasturbation;

	[SerializeField]
	[Label("マップ画像")]
	private Image mapImageMasturbation;

	[SerializeField]
	[Label("女選択")]
	private Button femaleSelectButtonMasturbation;

	[SerializeField]
	[Label("マップ選択")]
	private Button mapSelectButtonMasturbation;

	[SerializeField]
	[Label("昼")]
	private Toggle tglNoonMasturbation;

	[SerializeField]
	[Label("夕方")]
	private Toggle tglEveningMasturbation;

	[SerializeField]
	[Label("夜")]
	private Toggle tglNightMasturbation;

	[SerializeField]
	[Label("慣れ")]
	private Toggle tglExprienceMasturbation;

	[SerializeField]
	[Label("淫乱")]
	private Toggle tglLewdnessMasturbation;

	[SerializeField]
	[Label("発見されていない")]
	private Toggle tglDiscoverySafeMasturbation;

	[SerializeField]
	[Label("発見されている")]
	private Toggle tglDiscoveryOutMasturbation;

	[SerializeField]
	[Label("レズ親")]
	[Header("レズ")]
	private GameObject objLesbian;

	[SerializeField]
	[Label("女学生証")]
	private CharaHInfoComponent cardFemaleLesbian;

	[SerializeField]
	[Label("相手学生証")]
	private CharaHInfoComponent cardPartnerLesbian;

	[Label("マップ画像")]
	[SerializeField]
	private Image mapImageLesbian;

	[SerializeField]
	[Label("女選択")]
	private Button femaleSelectButtonLesbian;

	[SerializeField]
	[Label("相手選択")]
	private Button partnerSelectButtonLesbian;

	[SerializeField]
	[Label("マップ選択")]
	private Button mapSelectButtonLesbian;

	[SerializeField]
	[Label("昼")]
	private Toggle tglNoonLesbian;

	[SerializeField]
	[Label("夕方")]
	private Toggle tglEveningLesbian;

	[SerializeField]
	[Label("夜")]
	private Toggle tglNightLesbian;

	[SerializeField]
	[Label("3P親")]
	[Header("3P")]
	private GameObject obj3P;

	[SerializeField]
	[Label("1人目女の子選択タブ")]
	private Toggle tglFemal3P1;

	[SerializeField]
	[Label("1人目女の子名前")]
	private TextMeshProUGUI textFemaleName1;

	[SerializeField]
	[Label("1人目女の子性格")]
	private TextMeshProUGUI textFemalePersonal1;

	[SerializeField]
	[Label("2人目女の子選択タブ")]
	private Toggle tglFemal3P2;

	[SerializeField]
	[Label("2人目女の子名前")]
	private TextMeshProUGUI textFemaleName2;

	[SerializeField]
	[Label("2人目女の子性格")]
	private TextMeshProUGUI textFemalePersonal2;

	[SerializeField]
	[Label("女学生証")]
	private CharaHInfoComponent cardFemale3P;

	[SerializeField]
	[Label("男学生証")]
	private CharaHInfoComponent cardMale3P;

	[SerializeField]
	[Label("マップ画像")]
	private Image mapImage3P;

	[SerializeField]
	[Label("女選択")]
	private Button femaleSelectButton3P;

	[SerializeField]
	[Label("男選択")]
	private Button maleSelectButton3P;

	[Label("マップ選択")]
	[SerializeField]
	private Button mapSelectButton3P;

	[Label("昼")]
	[SerializeField]
	private Toggle tglNoon3P;

	[SerializeField]
	[Label("夕方")]
	private Toggle tglEvening3P;

	[Label("夜")]
	[SerializeField]
	private Toggle tglNight3P;

	[Label("女の子1人目H段階親")]
	[SerializeField]
	private GameObject objStage3P1;

	[Label("女の子1人目慣れ")]
	[SerializeField]
	private Toggle tglExprience3P1;

	[SerializeField]
	[Label("女の子1人目淫乱")]
	private Toggle tglLewdness3P1;

	[SerializeField]
	[Label("女の子2人目H段階親")]
	private GameObject objStage3P2;

	[Label("女の子2人目慣れ")]
	[SerializeField]
	private Toggle tglExprience3P2;

	[SerializeField]
	[Label("女の子2人目淫乱")]
	private Toggle tglLewdness3P2;

	[SerializeField]
	[Label("ステータス親")]
	private GameObject objStatus;

	[SerializeField]
	[Label("安全日")]
	private Toggle tglSafe3P;

	[SerializeField]
	[Label("危険日")]
	private Toggle tglDanger3P;

	private SaveData.Heroine heroine;

	private SaveData.Heroine partner;

	private SaveData.Player player;

	private int mapNo;

	private int timeZone;

	private int stageH1;

	private int stageH2;

	private int statusH;

	private bool discovery;

	private Member member = new Member();

	private int mode;

	private bool isInit;

	private bool isStageMasturbationChange = true;

	private bool isStage3PChange = true;

	private bool isTimeZoneChange = true;

	private Dictionary<int, Data.Param> _mapDataLT;

	private Dictionary<int, Data.Param> mapDataLT
	{
		get
		{
			return this.GetCache(ref _mapDataLT, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.MAP).Get(0));
		}
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
		Utils.Sound.Play(new Utils.Sound.SettingBGM(BGM.Title));
		bool isAdd20 = Game.isAdd20;
		objAcionSelect3P.SetActive(isAdd20);
		objAcionSelect.SetActive(!isAdd20);
		List<int> lstBackCategory = new List<int>();
		if (Singleton<Scene>.IsInstance())
		{
			GameObject commonSpace = Singleton<Scene>.Instance.commonSpace;
			if (commonSpace != null)
			{
				FreeHBackData component = commonSpace.GetComponent<FreeHBackData>();
				if (component != null)
				{
					heroine = component.heroine;
					partner = component.partner;
					player = component.player;
					mapNo = component.map;
					timeZone = component.timeZone;
					stageH1 = component.stageH1;
					stageH2 = component.stageH2;
					statusH = component.statusH;
					discovery = component.discovery;
					lstBackCategory = component.categorys;
				}
				UnityEngine.Object.Destroy(component);
			}
		}
		member.map = base.gameObject.AddComponent<BaseMap>();
		member.resultHeroine = new ReactiveProperty<SaveData.Heroine>(heroine);
		member.resultPartner = new ReactiveProperty<SaveData.Heroine>(partner);
		member.resultPlayer = new ReactiveProperty<SaveData.Player>(player);
		member.resultMapInfo = new ReactiveProperty<MapInfo.Param>();
		member.resultTimeZone = new IntReactiveProperty(timeZone);
		member.resultStage1 = new IntReactiveProperty(stageH1);
		member.resultStage2 = new IntReactiveProperty(stageH2);
		member.resultStatus = new IntReactiveProperty(statusH);
		member.resultDiscovery = new BoolReactiveProperty(discovery);
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((FreeHScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		yield return new WaitUntil(() => member.map.infoDic != null);
		int modeFirst = 0;
		if (lstBackCategory.Any((int c) => MathfEx.IsRange(1010, c, 1099, true)))
		{
			modeFirst = 1;
		}
		else if (lstBackCategory.Any((int c) => MathfEx.IsRange(1100, c, 1199, true)))
		{
			modeFirst = 2;
		}
		else if (lstBackCategory.Any((int c) => MathfEx.IsRange(3000, c, 3099, true)))
		{
			modeFirst = 3;
		}
		if (isAdd20)
		{
			Toggle[] array = new Toggle[4] { tglNormal3P, tglMasturbation3P, tglLesbian3P, tgl3P };
			array[modeFirst].isOn = true;
			tglNormal3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 0;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			tglMasturbation3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 1;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			tglLesbian3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 2;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			tgl3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 3;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			array.ToList().ForEach(delegate(Toggle bt)
			{
				bt.OnPointerClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.sel);
				});
			});
		}
		else
		{
			Toggle[] array2 = new Toggle[3] { tglNormal, tglMasturbation, tglLesbian };
			array2[modeFirst].isOn = true;
			tglNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 0;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			tglMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 1;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			tglLesbian.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
			{
				if (ison)
				{
					mode = 2;
					SetMainCanvasObject(mode);
					isStageMasturbationChange = true;
					isStage3PChange = true;
				}
			});
			array2.ToList().ForEach(delegate(Toggle bt)
			{
				bt.OnPointerClickAsObservable().Subscribe(delegate
				{
					Utils.Sound.Play(SystemSE.sel);
				});
			});
		}
		NormalSetup();
		MasturbationSetup();
		LesbianSetup();
		Same3PSetup();
		member.resultHeroine.Where((SaveData.Heroine heroine) => heroine != null).Subscribe(delegate(SaveData.Heroine heroine)
		{
			cardFemaleNormal.SetCharaInfo(heroine.charFile);
			cardFemaleMasturbation.SetCharaInfo(heroine.charFile);
			cardFemaleLesbian.SetCharaInfo(heroine.charFile);
			if (tglFemal3P1.isOn)
			{
				cardFemale3P.SetCharaInfo(heroine.charFile);
			}
			textFemaleName1.text = heroine.charFile.parameter.fullname;
			textFemalePersonal1.text = GetPersonality(heroine.charFile.parameter);
		});
		member.resultPartner.Where((SaveData.Heroine heroine) => heroine != null).Subscribe(delegate(SaveData.Heroine heroine)
		{
			cardPartnerLesbian.SetCharaInfo(heroine.charFile);
			if (tglFemal3P2.isOn)
			{
				cardFemale3P.SetCharaInfo(heroine.charFile);
			}
			textFemaleName2.text = heroine.charFile.parameter.fullname;
			textFemalePersonal2.text = GetPersonality(heroine.charFile.parameter);
		});
		member.resultPlayer.Where((SaveData.Player player) => player != null).Subscribe(delegate(SaveData.Player player)
		{
			cardMaleNormal.SetCharaInfo(player.charFile);
			cardMale3P.SetCharaInfo(player.charFile);
		});
		member.resultMapInfo.Where((MapInfo.Param mapInfo) => mapInfo != null).Subscribe(delegate(MapInfo.Param mapInfo)
		{
			SetMapSprite(mapInfo);
		});
		enterButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
		});
		IObservable<int> modeUpdate = from _ in this.UpdateAsObservable()
			select mode;
		(from list in Observable.CombineLatest<bool>(member.resultHeroine.Select((SaveData.Heroine p) => p != null), modeUpdate.Select((int n) => n != 2 || member.resultPartner.Value != null), modeUpdate.Select((int n) => n != 0 || member.resultPlayer.Value != null), modeUpdate.Select((int n) => n != 3 || (member.resultPlayer.Value != null && member.resultPartner.Value != null)), member.resultMapInfo.Select((MapInfo.Param mapInfo) => mapInfo != null))
			select list.All((bool result) => result)).SubscribeToInteractable(enterButton);
		enterButton.OnClickAsObservable().Subscribe(delegate
		{
			int[] array3 = new int[4] { 0, 1012, 1100, 3000 };
			OpenHData orAddComponent = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenHData>();
			orAddComponent.data = new OpenHData.Data
			{
				lstFemale = new List<SaveData.Heroine> { member.resultHeroine.Value },
				player = member.resultPlayer.Value,
				isFreeH = true,
				mapNoFreeH = member.resultMapInfo.Value.No,
				peepCategory = new List<int> { array3[mode] },
				timezoneFreeH = member.resultTimeZone.Value,
				statusFreeH = member.resultStatus.Value,
				isFound = member.resultDiscovery.Value
			};
			switch (mode)
			{
			case 0:
				(orAddComponent.data as OpenHData.Data).stageFreeH1 = member.resultStage1.Value;
				(orAddComponent.data as OpenHData.Data).stageFreeH2 = 0;
				break;
			case 1:
				(orAddComponent.data as OpenHData.Data).stageFreeH1 = ((member.resultStage1.Value <= 1) ? 2 : member.resultStage1.Value);
				(orAddComponent.data as OpenHData.Data).stageFreeH2 = 0;
				break;
			case 2:
				(orAddComponent.data as OpenHData.Data).stageFreeH1 = 0;
				(orAddComponent.data as OpenHData.Data).stageFreeH2 = 0;
				break;
			case 3:
				(orAddComponent.data as OpenHData.Data).stageFreeH1 = ((member.resultStage1.Value <= 1) ? 2 : member.resultStage1.Value);
				(orAddComponent.data as OpenHData.Data).stageFreeH2 = member.resultStage2.Value;
				break;
			}
			if ((mode == 2 || mode == 3) && member.resultPartner.Value != null)
			{
				(orAddComponent.data as OpenHData.Data).lstFemale.Add(member.resultPartner.Value);
			}
			orAddComponent.isLoad = true;
			orAddComponent.isAsync = false;
			orAddComponent.isFade = false;
			orAddComponent.fadeType = Scene.Data.FadeType.In;
		});
		Action returnProc = delegate
		{
			Observable.NextFrame().Subscribe(delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			});
		};
		returnButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			returnProc();
		});
		(from _ in this.UpdateAsObservable()
			where canvas.enabled
			where Singleton<Scene>.Instance.NowSceneNames[0] == "FreeH" && !Singleton<Scene>.Instance.IsNowLoadingFade
			where Input.GetMouseButtonDown(1)
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			returnProc();
		});
		isInit = true;
	}

	private void SetMainCanvasObject(int _mode)
	{
		GameObject[] array = new GameObject[4] { objNormal, objMasturbation, objLesbian, obj3P };
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(i == _mode);
		}
	}

	private void SetTimeZone(int _mode, int _time)
	{
		List<Toggle>[] array = new List<Toggle>[4]
		{
			new List<Toggle> { tglNoonNormal, tglEveningNormal, tglNightNormal },
			new List<Toggle> { tglNoonMasturbation, tglEveningMasturbation, tglNightMasturbation },
			new List<Toggle> { tglNoonLesbian, tglEveningLesbian, tglNightLesbian },
			new List<Toggle> { tglNoon3P, tglEvening3P, tglNight3P }
		};
		for (int i = 0; i < array[_mode].Count; i++)
		{
			array[_mode][i].isOn = i == _time;
		}
	}

	private void SetMapSprite(MapInfo.Param _mapInfo)
	{
		if (_mapInfo == null)
		{
			return;
		}
		bool flag = _mapInfo.No == 65 && member.resultTimeZone.Value != 2;
		bool flag2 = (_mapInfo.No == 63 || _mapInfo.No == 64) && member.resultTimeZone.Value != 0;
		if (!flag && !flag2)
		{
			string bundle = _mapInfo.ThumbnailBundle;
			string asset = _mapInfo.ThumbnailAsset;
			mapDataLT.SafeGet(_mapInfo.No).SafeProc(delegate(Data.Param param)
			{
				bundle = param.Bundle;
				asset = param.asset;
			});
			StringBuilder stringBuilder = new StringBuilder(asset);
			stringBuilder = stringBuilder.Remove(stringBuilder.Length - 2, 2);
			stringBuilder.Append(member.resultTimeZone.Value.ToString("00"));
			Utils.Bundle.LoadSprite(bundle, stringBuilder.ToString(), mapImageNormal, false);
			Utils.Bundle.LoadSprite(bundle, stringBuilder.ToString(), mapImageMasturbation, false);
			Utils.Bundle.LoadSprite(bundle, stringBuilder.ToString(), mapImageLesbian, false);
			Utils.Bundle.LoadSprite(bundle, stringBuilder.ToString(), mapImage3P, false);
		}
	}

	private void SetMapTimeZone(MapInfo.Param _mapInfo)
	{
		if (_mapInfo != null)
		{
			Toggle[] array = new Toggle[3] { tglNoonNormal, tglEveningNormal, tglNightNormal };
			Toggle[] array2 = new Toggle[3] { tglNoonMasturbation, tglEveningMasturbation, tglNightMasturbation };
			Toggle[] array3 = new Toggle[3] { tglNoonLesbian, tglEveningLesbian, tglNightLesbian };
			Toggle[] array4 = new Toggle[3] { tglNoon3P, tglEvening3P, tglNight3P };
			bool flag = _mapInfo.No == 65;
			bool flag2 = _mapInfo.No == 63 || _mapInfo.No == 64;
			bool[] array5 = new bool[3]
			{
				!flag,
				!flag2 && !flag,
				!flag2
			};
			for (int i = 0; i < array.Length; i++)
			{
				bool interactable = array5[i];
				array[i].interactable = interactable;
				array2[i].interactable = interactable;
				array3[i].interactable = interactable;
				array4[i].interactable = interactable;
			}
			int time = member.resultTimeZone.Value;
			if (flag2)
			{
				time = 0;
			}
			else if (flag)
			{
				time = 2;
			}
			SetTimeZone(0, time);
		}
	}

	private void NormalSetup()
	{
		femaleSelectButtonNormal.OnClickAsObservable().Subscribe(delegate
		{
			string levelName = "FreeHCharaSelectFemale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName, delegate
			{
				FreeHCharaSelect rootComponent = Scene.GetRootComponent<FreeHCharaSelect>(levelName);
				if (!(rootComponent == null))
				{
					(from heroine in rootComponent.ObserveEveryValueChanged((FreeHCharaSelect p) => p.heroine)
						where heroine != null
						select heroine).Subscribe(delegate(SaveData.Heroine heroine)
					{
						member.resultHeroine.Value = heroine;
					});
				}
			});
		});
		maleSelectButtonNormal.OnClickAsObservable().Subscribe(delegate
		{
			string levelName2 = "FreeHCharaSelectMale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName2, delegate
			{
				FreeHCharaSelect rootComponent2 = Scene.GetRootComponent<FreeHCharaSelect>(levelName2);
				if (!(rootComponent2 == null))
				{
					(from player in rootComponent2.ObserveEveryValueChanged((FreeHCharaSelect p) => p.player)
						where player != null
						select player).Subscribe(delegate(SaveData.Player player)
					{
						member.resultPlayer.Value = player;
					});
				}
			});
		});
		member.resultMapInfo.Value = member.map.infoDic[mapNo];
		SetMapTimeZone(member.resultMapInfo.Value);
		mapSelectButtonNormal.OnClickAsObservable().Subscribe(delegate
		{
			string levelName3 = "MapSelectMenu";
			AddScene("action/menu/mapselect.unity3d", levelName3, delegate
			{
				MapSelectMenuScene rootComponent3 = Scene.GetRootComponent<MapSelectMenuScene>(levelName3);
				if (!(rootComponent3 == null))
				{
					rootComponent3.visibleType = MapSelectMenuScene.VisibleType.FreeH;
					rootComponent3.baseMap = member.map;
					rootComponent3.result = MapSelectMenuScene.ResultType.Wait;
					(from mapInfo in rootComponent3.ObserveEveryValueChanged((MapSelectMenuScene p) => p.mapInfo)
						where mapInfo != null
						select mapInfo).Subscribe(delegate(MapInfo.Param mapInfo)
					{
						SetMapTimeZone(mapInfo);
						member.resultMapInfo.Value = mapInfo;
					});
				}
			});
		});
		Toggle[] array = new Toggle[3] { tglNoonNormal, tglEveningNormal, tglNightNormal };
		array[timeZone].isOn = true;
		tglNoonNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				isTimeZoneChange = false;
				member.resultTimeZone.Value = 0;
				SetTimeZone(1, 0);
				SetTimeZone(2, 0);
				SetTimeZone(3, 0);
				isTimeZoneChange = true;
				SetMapSprite(member.resultMapInfo.Value);
			}
		});
		tglEveningNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				isTimeZoneChange = false;
				member.resultTimeZone.Value = 1;
				SetTimeZone(1, 1);
				SetTimeZone(2, 1);
				SetTimeZone(3, 1);
				isTimeZoneChange = true;
				SetMapSprite(member.resultMapInfo.Value);
			}
		});
		tglNightNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				isTimeZoneChange = false;
				member.resultTimeZone.Value = 2;
				SetTimeZone(1, 2);
				SetTimeZone(2, 2);
				SetTimeZone(3, 2);
				isTimeZoneChange = true;
				SetMapSprite(member.resultMapInfo.Value);
			}
		});
		Toggle[] array2 = new Toggle[4] { tglFirstNormal, tglInexperienceNormal, tglExprienceNormal, tglLewdnessNormal };
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].isOn = stageH1 == i;
		}
		tglFirstNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				isStageMasturbationChange = false;
				isStage3PChange = false;
				tglExprienceMasturbation.isOn = true;
				tglLewdnessMasturbation.isOn = false;
				tglExprience3P1.isOn = true;
				tglLewdness3P1.isOn = false;
				member.resultStage1.Value = 0;
			}
		});
		tglInexperienceNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				isStageMasturbationChange = false;
				isStage3PChange = false;
				tglExprienceMasturbation.isOn = true;
				tglLewdnessMasturbation.isOn = false;
				tglExprience3P1.isOn = true;
				tglLewdness3P1.isOn = false;
				member.resultStage1.Value = 1;
			}
		});
		tglExprienceNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				tglExprienceMasturbation.isOn = true;
				tglLewdnessMasturbation.isOn = false;
				tglExprience3P1.isOn = true;
				tglLewdness3P1.isOn = false;
				member.resultStage1.Value = 2;
			}
		});
		tglLewdnessNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				tglExprienceMasturbation.isOn = false;
				tglLewdnessMasturbation.isOn = true;
				tglExprience3P1.isOn = false;
				tglLewdness3P1.isOn = true;
				member.resultStage1.Value = 3;
			}
		});
		Toggle[] array3 = new Toggle[2] { tglSafeNormal, tglDangerNormal };
		array3[statusH].isOn = true;
		tglSafeNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultStatus.Value = 0;
				tglSafe3P.isOn = true;
				tglDanger3P.isOn = false;
			}
		});
		tglDangerNormal.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultStatus.Value = 1;
				tglSafe3P.isOn = false;
				tglDanger3P.isOn = true;
			}
		});
		Toggle[] source = new Toggle[9] { tglNoonNormal, tglEveningNormal, tglNightNormal, tglFirstNormal, tglInexperienceNormal, tglExprienceNormal, tglLewdnessNormal, tglSafeNormal, tglDangerNormal };
		source.ToList().ForEach(delegate(Toggle bt)
		{
			bt.OnPointerClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		});
		Button[] source2 = new Button[3] { femaleSelectButtonNormal, maleSelectButtonNormal, mapSelectButtonNormal };
		source2.ToList().ForEach(delegate(Button bt)
		{
			bt.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
			});
		});
	}

	private void MasturbationSetup()
	{
		femaleSelectButtonMasturbation.OnClickAsObservable().Subscribe(delegate
		{
			string levelName = "FreeHCharaSelectFemale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName, delegate
			{
				FreeHCharaSelect rootComponent = Scene.GetRootComponent<FreeHCharaSelect>(levelName);
				if (!(rootComponent == null))
				{
					(from heroine in rootComponent.ObserveEveryValueChanged((FreeHCharaSelect p) => p.heroine)
						where heroine != null
						select heroine).Subscribe(delegate(SaveData.Heroine heroine)
					{
						member.resultHeroine.Value = heroine;
					});
				}
			});
		});
		mapSelectButtonMasturbation.OnClickAsObservable().Subscribe(delegate
		{
			string levelName2 = "MapSelectMenu";
			AddScene("action/menu/mapselect.unity3d", levelName2, delegate
			{
				MapSelectMenuScene rootComponent2 = Scene.GetRootComponent<MapSelectMenuScene>(levelName2);
				if (!(rootComponent2 == null))
				{
					rootComponent2.visibleType = MapSelectMenuScene.VisibleType.FreeH;
					rootComponent2.baseMap = member.map;
					rootComponent2.result = MapSelectMenuScene.ResultType.Wait;
					(from mapInfo in rootComponent2.ObserveEveryValueChanged((MapSelectMenuScene p) => p.mapInfo)
						where mapInfo != null
						select mapInfo).Subscribe(delegate(MapInfo.Param mapInfo)
					{
						SetMapTimeZone(mapInfo);
						member.resultMapInfo.Value = mapInfo;
					});
				}
			});
		});
		tglNoonMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison && isInit)
			{
				member.resultTimeZone.Value = 0;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 0);
				}
			}
		});
		tglEveningMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison && isInit)
			{
				member.resultTimeZone.Value = 1;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 1);
				}
			}
		});
		tglNightMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison && isInit)
			{
				member.resultTimeZone.Value = 2;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 2);
				}
			}
		});
		Toggle[] array = new Toggle[4] { null, null, tglExprienceMasturbation, tglLewdnessMasturbation };
		if (array[stageH1] == null)
		{
			array[2].isOn = true;
			array[3].isOn = false;
		}
		else
		{
			array[stageH1].isOn = true;
			array[(stageH1 != 3) ? 3 : 2].isOn = false;
		}
		tglExprienceMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				if (isInit)
				{
					member.resultStage1.Value = 2;
					if (isStageMasturbationChange)
					{
						tglFirstNormal.isOn = false;
						tglInexperienceNormal.isOn = false;
						tglExprienceNormal.isOn = true;
						tglLewdnessNormal.isOn = false;
						if (isStage3PChange)
						{
							tglExprience3P1.isOn = true;
							tglLewdness3P1.isOn = false;
						}
					}
				}
				isStageMasturbationChange = true;
			}
		});
		tglLewdnessMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				if (isInit)
				{
					member.resultStage1.Value = 3;
					if (isStageMasturbationChange)
					{
						tglFirstNormal.isOn = false;
						tglInexperienceNormal.isOn = false;
						tglExprienceNormal.isOn = false;
						tglLewdnessNormal.isOn = true;
						if (isStage3PChange)
						{
							tglExprience3P1.isOn = false;
							tglLewdness3P1.isOn = true;
						}
					}
				}
				isStageMasturbationChange = true;
			}
		});
		Toggle[] array2 = new Toggle[2] { tglDiscoverySafeMasturbation, tglDiscoveryOutMasturbation };
		array2[statusH].isOn = true;
		if (!objMasturbation.activeSelf)
		{
			array2[statusH ^ 1].isOn = false;
		}
		tglDiscoverySafeMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultDiscovery.Value = false;
			}
		});
		tglDiscoveryOutMasturbation.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultDiscovery.Value = true;
			}
		});
		Toggle[] source = new Toggle[7] { tglNoonMasturbation, tglEveningMasturbation, tglNightMasturbation, tglExprienceMasturbation, tglLewdnessMasturbation, tglDiscoverySafeMasturbation, tglDiscoveryOutMasturbation };
		source.ToList().ForEach(delegate(Toggle bt)
		{
			bt.OnPointerClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		});
		Button[] source2 = new Button[2] { femaleSelectButtonMasturbation, mapSelectButtonMasturbation };
		source2.ToList().ForEach(delegate(Button bt)
		{
			bt.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
			});
		});
	}

	private void LesbianSetup()
	{
		femaleSelectButtonLesbian.OnClickAsObservable().Subscribe(delegate
		{
			string levelName = "FreeHCharaSelectFemale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName, delegate
			{
				FreeHCharaSelect rootComponent = Scene.GetRootComponent<FreeHCharaSelect>(levelName);
				if (!(rootComponent == null))
				{
					(from heroine in rootComponent.ObserveEveryValueChanged((FreeHCharaSelect p) => p.heroine)
						where heroine != null
						select heroine).Subscribe(delegate(SaveData.Heroine heroine)
					{
						member.resultHeroine.Value = heroine;
					});
				}
			});
		});
		partnerSelectButtonLesbian.OnClickAsObservable().Subscribe(delegate
		{
			string levelName2 = "FreeHCharaSelectFemale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName2, delegate
			{
				FreeHCharaSelect rootComponent2 = Scene.GetRootComponent<FreeHCharaSelect>(levelName2);
				if (!(rootComponent2 == null))
				{
					(from heroine in rootComponent2.ObserveEveryValueChanged((FreeHCharaSelect p) => p.heroine)
						where heroine != null
						select heroine).Subscribe(delegate(SaveData.Heroine heroine)
					{
						member.resultPartner.Value = heroine;
					});
				}
			});
		});
		mapSelectButtonLesbian.OnClickAsObservable().Subscribe(delegate
		{
			string levelName3 = "MapSelectMenu";
			AddScene("action/menu/mapselect.unity3d", levelName3, delegate
			{
				MapSelectMenuScene rootComponent3 = Scene.GetRootComponent<MapSelectMenuScene>(levelName3);
				if (!(rootComponent3 == null))
				{
					rootComponent3.visibleType = MapSelectMenuScene.VisibleType.FreeH;
					rootComponent3.baseMap = member.map;
					rootComponent3.result = MapSelectMenuScene.ResultType.Wait;
					(from mapInfo in rootComponent3.ObserveEveryValueChanged((MapSelectMenuScene p) => p.mapInfo)
						where mapInfo != null
						select mapInfo).Subscribe(delegate(MapInfo.Param mapInfo)
					{
						SetMapTimeZone(mapInfo);
						member.resultMapInfo.Value = mapInfo;
					});
				}
			});
		});
		Toggle[] array = new Toggle[3] { tglNoonLesbian, tglEveningLesbian, tglNightLesbian };
		array[timeZone].isOn = true;
		tglNoonLesbian.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison && isInit)
			{
				member.resultTimeZone.Value = 0;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 0);
				}
			}
		});
		tglEveningLesbian.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison && isInit)
			{
				member.resultTimeZone.Value = 1;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 1);
				}
			}
		});
		tglNightLesbian.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison && isInit)
			{
				member.resultTimeZone.Value = 2;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 2);
				}
			}
		});
		array.ToList().ForEach(delegate(Toggle bt)
		{
			bt.OnPointerClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		});
		Button[] source = new Button[3] { femaleSelectButtonLesbian, partnerSelectButtonLesbian, mapSelectButtonLesbian };
		source.ToList().ForEach(delegate(Button bt)
		{
			bt.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
			});
		});
	}

	private void Same3PSetup()
	{
		tglFemal3P1.OnValueChangedAsObservable().Subscribe(delegate(bool isON)
		{
			if (isON)
			{
				objStatus.SetActive(true);
				objStage3P1.SetActive(true);
				objStage3P2.SetActive(false);
				if (member.resultHeroine.Value == null)
				{
					cardFemale3P.Clear();
				}
				else
				{
					cardFemale3P.SetCharaInfo(member.resultHeroine.Value.charFile);
				}
			}
		});
		tglFemal3P2.OnValueChangedAsObservable().Subscribe(delegate(bool isON)
		{
			if (isON)
			{
				objStatus.SetActive(false);
				objStage3P1.SetActive(false);
				objStage3P2.SetActive(true);
				if (member.resultPartner.Value == null)
				{
					cardFemale3P.Clear();
				}
				else
				{
					cardFemale3P.SetCharaInfo(member.resultPartner.Value.charFile);
				}
			}
		});
		femaleSelectButton3P.OnClickAsObservable().Subscribe(delegate
		{
			string levelName = "FreeHCharaSelectFemale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName, delegate
			{
				FreeHCharaSelect rootComponent = Scene.GetRootComponent<FreeHCharaSelect>(levelName);
				if (!(rootComponent == null))
				{
					(from heroine in rootComponent.ObserveEveryValueChanged((FreeHCharaSelect p) => p.heroine)
						where heroine != null
						select heroine).Subscribe(delegate(SaveData.Heroine heroine)
					{
						if (tglFemal3P1.isOn)
						{
							member.resultHeroine.Value = heroine;
						}
						else
						{
							member.resultPartner.Value = heroine;
						}
					});
				}
			});
		});
		maleSelectButton3P.OnClickAsObservable().Subscribe(delegate
		{
			string levelName2 = "FreeHCharaSelectMale";
			AddScene("h/scene/freehcharaselect.unity3d", levelName2, delegate
			{
				FreeHCharaSelect rootComponent2 = Scene.GetRootComponent<FreeHCharaSelect>(levelName2);
				if (!(rootComponent2 == null))
				{
					(from player in rootComponent2.ObserveEveryValueChanged((FreeHCharaSelect p) => p.player)
						where player != null
						select player).Subscribe(delegate(SaveData.Player player)
					{
						member.resultPlayer.Value = player;
					});
				}
			});
		});
		member.resultMapInfo.Value = member.map.infoDic[mapNo];
		mapSelectButton3P.OnClickAsObservable().Subscribe(delegate
		{
			string levelName3 = "MapSelectMenu";
			AddScene("action/menu/mapselect.unity3d", levelName3, delegate
			{
				MapSelectMenuScene rootComponent3 = Scene.GetRootComponent<MapSelectMenuScene>(levelName3);
				if (!(rootComponent3 == null))
				{
					rootComponent3.visibleType = MapSelectMenuScene.VisibleType.FreeH;
					rootComponent3.baseMap = member.map;
					rootComponent3.result = MapSelectMenuScene.ResultType.Wait;
					(from mapInfo in rootComponent3.ObserveEveryValueChanged((MapSelectMenuScene p) => p.mapInfo)
						where mapInfo != null
						select mapInfo).Subscribe(delegate(MapInfo.Param mapInfo)
					{
						SetMapTimeZone(mapInfo);
						member.resultMapInfo.Value = mapInfo;
					});
				}
			});
		});
		Toggle[] array = new Toggle[3] { tglNoon3P, tglEvening3P, tglNight3P };
		array[timeZone].isOn = true;
		tglNoon3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultTimeZone.Value = 0;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 0);
				}
			}
		});
		tglEvening3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultTimeZone.Value = 1;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 1);
				}
			}
		});
		tglNight3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultTimeZone.Value = 2;
				if (isTimeZoneChange)
				{
					SetTimeZone(0, 2);
				}
			}
		});
		Toggle[] array2 = new Toggle[4] { null, null, tglExprience3P1, tglLewdness3P1 };
		if (array2[stageH1] == null)
		{
			array2[2].isOn = true;
			array2[3].isOn = false;
		}
		else
		{
			array2[stageH1].isOn = true;
			array2[(stageH1 != 3) ? 3 : 2].isOn = false;
		}
		tglExprience3P1.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				if (isInit)
				{
					member.resultStage1.Value = 2;
					if (isStage3PChange)
					{
						tglFirstNormal.isOn = false;
						tglInexperienceNormal.isOn = false;
						tglExprienceNormal.isOn = true;
						tglLewdnessNormal.isOn = false;
						if (isStageMasturbationChange)
						{
							tglExprienceMasturbation.isOn = true;
							tglLewdnessMasturbation.isOn = false;
						}
					}
				}
				isStage3PChange = true;
			}
		});
		tglLewdness3P1.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				if (isInit)
				{
					member.resultStage1.Value = 3;
					if (isStage3PChange)
					{
						tglFirstNormal.isOn = false;
						tglInexperienceNormal.isOn = false;
						tglExprienceNormal.isOn = false;
						tglLewdnessNormal.isOn = true;
						if (isStageMasturbationChange)
						{
							tglExprienceMasturbation.isOn = false;
							tglLewdnessMasturbation.isOn = true;
						}
					}
				}
				isStage3PChange = true;
			}
		});
		Toggle[] array3 = new Toggle[4] { null, null, tglExprience3P2, tglLewdness3P2 };
		if (array3[stageH2] == null)
		{
			array3[2].isOn = true;
			array3[3].isOn = false;
		}
		else
		{
			array3[stageH2].isOn = true;
			array3[(stageH2 != 3) ? 3 : 2].isOn = false;
		}
		tglExprience3P2.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultStage2.Value = 2;
			}
		});
		tglLewdness3P2.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultStage2.Value = 3;
			}
		});
		Toggle[] array4 = new Toggle[2] { tglSafe3P, tglDanger3P };
		array4[statusH].isOn = true;
		tglSafe3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultStatus.Value = 0;
				tglSafeNormal.isOn = true;
				tglDangerNormal.isOn = false;
			}
		});
		tglDanger3P.OnValueChangedAsObservable().Subscribe(delegate(bool ison)
		{
			if (ison)
			{
				member.resultStatus.Value = 1;
				tglSafeNormal.isOn = false;
				tglDangerNormal.isOn = true;
			}
		});
		Toggle[] source = new Toggle[11]
		{
			tglFemal3P1, tglFemal3P2, tglNoon3P, tglEvening3P, tglNight3P, tglExprience3P1, tglLewdness3P1, tglExprience3P2, tglLewdness3P2, tglSafe3P,
			tglDanger3P
		};
		source.ToList().ForEach(delegate(Toggle bt)
		{
			bt.OnPointerClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.sel);
			});
		});
		Button[] source2 = new Button[3] { femaleSelectButton3P, maleSelectButton3P, mapSelectButton3P };
		source2.ToList().ForEach(delegate(Button bt)
		{
			bt.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
			});
		});
	}

	private string GetPersonality(ChaFileParameter _param)
	{
		VoiceInfo.Param[] source = Singleton<Voice>.Instance.voiceInfoDic.Values.Where((VoiceInfo.Param x) => 0 <= x.No).ToArray();
		int[] array = source.Select((VoiceInfo.Param x) => x.No).ToArray();
		string[] array2 = source.Select((VoiceInfo.Param x) => x.Personality).ToArray();
		int personality = _param.personality;
		VoiceInfo.Param value;
		if (!Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(personality, out value))
		{
			Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(0, out value);
		}
		int num = Array.IndexOf(array, value.No);
		return (num == -1) ? string.Empty : array2[num];
	}
}
