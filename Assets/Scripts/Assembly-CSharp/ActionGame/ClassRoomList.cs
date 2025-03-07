using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
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
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ActionGame
{
	public class ClassRoomList : BaseLoader
	{
		[Serializable]
		private class RandomCheck : ToggleCheck
		{
			[Label("ネットから")]
			[SerializeField]
			private Toggle _netToggle;

			[Label("フォルダから")]
			[SerializeField]
			private Toggle _directoryToggle;

			[Label("デフォルトフォルダから")]
			[SerializeField]
			private Toggle _directoryDefaultToggle;

			protected override Toggle[] toggles
			{
				get
				{
					return new Toggle[3] { _netToggle, _directoryToggle, _directoryDefaultToggle };
				}
			}
		}

		[Serializable]
		private class CardInformation : ToggleCheck
		{
			[Label("情報表示なし")]
			[SerializeField]
			private Toggle _noneToggle;

			[Label("あだ名表示")]
			[SerializeField]
			private Toggle _callNameToggle;

			[Label("性格名表示")]
			[SerializeField]
			private Toggle _personalNameToggle;

			protected override Toggle[] toggles
			{
				get
				{
					return new Toggle[3] { _noneToggle, _callNameToggle, _personalNameToggle };
				}
			}

			public PreviewClassData.VisibleMode Mode
			{
				get
				{
					if (_callNameToggle.isOn)
					{
						return PreviewClassData.VisibleMode.CallName;
					}
					if (_personalNameToggle.isOn)
					{
						return PreviewClassData.VisibleMode.Personality;
					}
					return PreviewClassData.VisibleMode.None;
				}
			}
		}

		[Serializable]
		private class EmptyTargetCheck : ToggleCheck
		{
			[Label("全体")]
			[SerializeField]
			private Toggle _allToggle;

			[Label("クラス")]
			[SerializeField]
			private Toggle _classToggle;

			protected override Toggle[] toggles
			{
				get
				{
					return new Toggle[2] { _allToggle, _classToggle };
				}
			}
		}

		private abstract class ToggleCheck
		{
			protected abstract Toggle[] toggles { get; }

			public ReadOnlyReactiveProperty<int> ToReadOnlyReactiveProperty()
			{
				return (from list in toggles.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
					select list.IndexOf(true) into i
					where i >= 0
					select i).ToReadOnlyReactiveProperty();
			}
		}

		private class SwapData
		{
			public readonly int schoolClass;

			public readonly int schoolClassIndex;

			public SwapData(int schoolClass, int schoolClassIndex)
			{
				this.schoolClass = schoolClass;
				this.schoolClassIndex = schoolClassIndex;
			}
		}

		[SerializeField]
		private ClassRoomSelectScene classRoomSelectScene;

		[SerializeField]
		private Transform PreviewRoot;

		[SerializeField]
		[Label("選択中の色")]
		private Color selColor = Color.yellow;

		[SerializeField]
		[Label("クラスタブ")]
		private RectTransform classTab;

		[Header("ランダムチェック")]
		[SerializeField]
		private RandomCheck randomCheck;

		[SerializeField]
		[Header("学生証のあだ名枠関連")]
		private CardInformation cardInformation;

		[SerializeField]
		[Label("登校人数テキスト")]
		private TextMeshProUGUI textAttendanceNum;

		[SerializeField]
		[Label("登校人数スライダー")]
		private Slider sldAttendanceNum;

		[SerializeField]
		[Label("登校人数MAX")]
		private Button btnAttendanceNum;

		[Label("クラスの登録人数を最大にする")]
		[SerializeField]
		private Toggle tglOtherClassRegisterMax;

		[Label("カスタム")]
		[SerializeField]
		private Button customSceneButton;

		[SerializeField]
		[Label("キャラボタン1")]
		private Button chara1Button;

		[SerializeField]
		private string[] chara1Strs = new string[2];

		[Label("キャラボタン2")]
		[SerializeField]
		private Button chara2Button;

		[SerializeField]
		private string[] chara2Strs = new string[2];

		[SerializeField]
		[Label("決定ボタン")]
		private Button enterButton;

		[SerializeField]
		[Label("戻るボタン")]
		private Button returnButton;

		[SerializeField]
		[Header("対象チェック")]
		private EmptyTargetCheck emptyTargetCheck;

		[SerializeField]
		[Label("ランダムボタン")]
		private Button randomButton;

		[SerializeField]
		[Label("除外ボタン")]
		private Button removeButton;

		private BoolReactiveProperty canvasEnabled = new BoolReactiveProperty(true);

		private int? _pageMax;

		[SerializeField]
		private IntReactiveProperty _page = new IntReactiveProperty(1);

		[SerializeField]
		private CvsAttribute cvsAttribute;

		private int randomTarget;

		private bool isEmptyTargetAll;

		private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

		private List<PreviewClassData> _charaPreviewList;

		private List<SaveData.CharaData> charaList = new List<SaveData.CharaData>();

		private ReactiveProperty<PreviewClassData> enterPreview = new ReactiveProperty<PreviewClassData>();

		private CompositeDisposable disposables = new CompositeDisposable();

		public bool isCustomScene { get; private set; }

		public bool isVisible
		{
			get
			{
				return canvasEnabled.Value;
			}
			set
			{
				canvasEnabled.Value = value;
			}
		}

		private int pageMax
		{
			get
			{
				if (!_pageMax.HasValue)
				{
					_pageMax = classTab.childCount - 1;
				}
				return _pageMax.Value;
			}
		}

		private int page
		{
			get
			{
				return _page.Value;
			}
		}

		private int emptyRegisterSum
		{
			get
			{
				return isEmptyTargetAll ? Enumerable.Range(0, pageMax).Select(GetEmptyRegisterSum).Sum() : GetEmptyRegisterSum(page);
			}
		}

		private Dictionary<int, Data.Param> translateButtonTitle
		{
			get
			{
				return uiTranslater.Get(2);
			}
		}

		private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
		{
			get
			{
				return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CLASS_REGISTER));
			}
		}

		private List<PreviewClassData> charaPreviewList
		{
			get
			{
				return this.GetCache(ref _charaPreviewList, () => new List<PreviewClassData>(from o in PreviewRoot.gameObject.Children()
					select new PreviewClassData(o)));
			}
		}

		private SaveData.Player player
		{
			get
			{
				return Singleton<Game>.Instance.Player;
			}
		}

		private List<SaveData.Heroine> heroineList
		{
			get
			{
				return Singleton<Game>.Instance.HeroineList;
			}
		}

		private int GetEmptyRegisterSum(int schoolClass)
		{
			if (schoolClass == pageMax)
			{
				return 0;
			}
			int num = 0;
			int classRegisterMax = GetClassRegisterMax(schoolClass);
			bool flag = isPlayerSchoolClass(schoolClass);
			for (int i = 0; i < classRegisterMax; i++)
			{
				if ((!flag || (i != player.schoolClassIndex && i != player.schoolClassIndex + 1)) && FindCharaIndex(schoolClass, charaPreviewList[i].classIndex) == -1)
				{
					num++;
				}
			}
			return num;
		}

		private string GetTranslateQuestionTitle(int index)
		{
			switch (index)
			{
			default:
				return null;
			case 0:
			case 1:
			case 2:
				return uiTranslater.Get(3).Values.FindTagText((new string[3] { "Remove", "RemoveAll", "RemoveClass" })[index]);
			}
		}

		private int GetClassRegisterMax(int schoolClass)
		{
			if (schoolClass == player.schoolClass || Manager.Config.AddData.OtherClassRegisterMax)
			{
				return charaPreviewList.Count;
			}
			return 5;
		}

		private bool isPlayerSchoolClass(int schoolClass)
		{
			return player.schoolClass == schoolClass;
		}

		private static bool isCharaFind(SaveData.CharaData chara, int schoolClass, int classIndex)
		{
			return chara.schoolClass == schoolClass && chara.schoolClassIndex == classIndex;
		}

		private SaveData.CharaData FindChara(int schoolClass, int classIndex)
		{
			return charaList.Find((SaveData.CharaData chara) => isCharaFind(chara, schoolClass, classIndex));
		}

		private int FindCharaIndex(int schoolClass, int classIndex)
		{
			return charaList.FindIndex((SaveData.CharaData chara) => isCharaFind(chara, schoolClass, classIndex));
		}

		private void PreviewUpdate()
		{
			PreviewClassData value = enterPreview.Value;
			enterPreview.Value = null;
			enterPreview.Value = value;
		}

		private void PreviewUpdateAll()
		{
			PreviewUpdate();
			charaPreviewList.ForEach(delegate(PreviewClassData p)
			{
				p.visibleMode = cardInformation.Mode;
			});
			if (enterPreview.Value != null && !enterPreview.Value.isVisible)
			{
				enterPreview.Value = null;
			}
		}

		private void RemoveChara()
		{
			int index = FindCharaIndex(page, enterPreview.Value.classIndex);
			SaveData.CharaData chara = charaList[index];
			SaveData saveData = Singleton<Game>.Instance.saveData;
			if (saveData.withHeroine.Check(chara))
			{
				saveData.withHeroine.Set(null);
			}
			if (saveData.dateHeroine.Check(chara))
			{
				saveData.dateHeroine.Set(null);
			}
			enterPreview.Value.Clear();
			charaList.RemoveAt(index);
			PreviewUpdate();
		}

		private void RemoveChara(bool isCheckScene)
		{
			if (!isCheckScene)
			{
				RemoveChara();
				return;
			}
			CheckScene.Parameter param = new CheckScene.Parameter();
			param.Yes = delegate
			{
				RemoveChara();
				Singleton<Scene>.Instance.UnLoad();
			};
			param.No = delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			};
			param.Title = GetTranslateQuestionTitle(0) ?? "転校させますか？";
			Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).StartAsCoroutine();
		}

		private SwapData CreateSwapData(PreviewClassData preview)
		{
			return new SwapData(page, preview.classIndex);
		}

		private bool IsSwapData(PreviewClassData preview)
		{
			return preview != null && IsSwapData(page, preview.classIndex);
		}

		private bool IsSwapData(SwapData swapData)
		{
			return swapData != null && IsSwapData(swapData.schoolClass, swapData.schoolClassIndex);
		}

		private bool IsSwapData(int schoolClass, int schoolClassIndex)
		{
			if (schoolClass == pageMax)
			{
				return false;
			}
			if (schoolClassIndex >= GetClassRegisterMax(schoolClass))
			{
				return false;
			}
			if (FindChara(schoolClass, schoolClassIndex) is SaveData.Player)
			{
				return false;
			}
			return true;
		}

		private void Swap(SwapData mData, SwapData tData)
		{
			SaveData.CharaData charaData = FindChara(mData.schoolClass, mData.schoolClassIndex);
			SaveData.CharaData charaData2 = FindChara(tData.schoolClass, tData.schoolClassIndex);
			if (charaData != null)
			{
				charaData.schoolClass = tData.schoolClass;
				charaData.schoolClassIndex = tData.schoolClassIndex;
			}
			if (charaData2 != null)
			{
				charaData2.schoolClass = mData.schoolClass;
				charaData2.schoolClassIndex = mData.schoolClassIndex;
			}
			foreach (var item in new[]
			{
				new
				{
					chara = charaData,
					index = tData.schoolClassIndex,
					isUpdate = true
				},
				new
				{
					chara = charaData2,
					index = mData.schoolClassIndex,
					isUpdate = (mData.schoolClass == tData.schoolClass)
				}
			}.Where(item => item.isUpdate))
			{
				PreviewClassData previewClassData = charaPreviewList[item.index];
				if (item.chara != null)
				{
					previewClassData.Set(item.chara);
				}
				else
				{
					previewClassData.Clear();
				}
			}
		}

		private void Start()
		{
			string[] args = translateButtonTitle.Values.ToArray("Chara1");
			for (int j = 0; j < chara1Strs.Length; j++)
			{
				args.SafeProc(j, delegate(string text)
				{
					chara1Strs[j] = text;
				});
			}
			args = translateButtonTitle.Values.ToArray("Chara2");
			for (int k = 0; k < chara2Strs.Length; k++)
			{
				args.SafeProc(k, delegate(string text)
				{
					chara2Strs[k] = text;
				});
			}
			heroineList.AddRange(Game.CreateFixCharaList((from p in heroineList
				select p.fixCharaID into id
				where id < 0
				select id).ToArray()));
			charaList.AddRange(heroineList.OfType<SaveData.CharaData>());
			charaList.Add(player);
			string prevBGM = string.Empty;
			Utils.Sound.GetBGM().SafeProc(delegate(LoadSound bgm)
			{
				prevBGM = bgm.assetBundleName;
			});
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
			this.ObserveEveryValueChanged((ClassRoomList _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
			Canvas canvas = GetComponent<Canvas>();
			canvasEnabled.Subscribe(delegate(bool isOn)
			{
				canvas.enabled = isOn;
			}).AddTo(disposables);
			bool isPlaySE = true;
			Action<SystemSE> PlaySE = delegate(SystemSE se)
			{
				if (isPlaySE)
				{
					Utils.Sound.Play(se);
				}
			};
			SwapData swap = null;
			charaPreviewList.ForEach(delegate(PreviewClassData item)
			{
				IObservable<Unit> source = item.button.OnClickAsObservable().Share();
				source.Subscribe(delegate
				{
					PlaySE(SystemSE.sel);
				}).AddTo(disposables);
				source.Where((Unit _) => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)).Subscribe(delegate
				{
					if (IsSwapData(swap) && IsSwapData(item))
					{
						Swap(swap, CreateSwapData(item));
					}
				}).AddTo(disposables);
				source.Subscribe(delegate
				{
					enterPreview.Value = item;
				}).AddTo(disposables);
				IObservable<IList<double>> source2 = source.DoubleInterval(250f);
				source2.Subscribe(delegate
				{
					if (enterPreview.Value.data == null)
					{
						classRoomSelectScene.OpenPreview();
					}
					else
					{
						SaveData.Heroine heroine2 = enterPreview.Value.data as SaveData.Heroine;
						if (heroine2 != null && heroine2.fixCharaID >= 0)
						{
							RemoveChara(true);
						}
					}
				}).AddTo(disposables);
			});
			ColorBlock backupButtonColor = charaPreviewList[0].button.colors;
			enterPreview.Scan(delegate(PreviewClassData acc, PreviewClassData cur)
			{
				if (acc != null)
				{
					acc.button.colors = backupButtonColor;
				}
				return cur;
			}).Subscribe(delegate(PreviewClassData preview)
			{
				if (preview != null)
				{
					ColorBlock colors = preview.button.colors;
					Color highlightedColor = (colors.normalColor = selColor);
					colors.highlightedColor = highlightedColor;
					preview.button.colors = colors;
				}
			}).AddTo(disposables);
			IObservable<PreviewClassData> source3 = enterPreview.Where((PreviewClassData preview) => preview != null).Share();
			source3.Subscribe(delegate(PreviewClassData preview)
			{
				preview.visibleMode = cardInformation.Mode;
			}).AddTo(disposables);
			source3.Subscribe(delegate(PreviewClassData preview)
			{
				swap = CreateSwapData(preview);
			}).AddTo(disposables);
			IObservable<bool> source4 = source3.Select((PreviewClassData preview) => preview.data != null).Share();
			TextMeshProUGUI componentInChildren = chara1Button.GetComponentInChildren<TextMeshProUGUI>();
			componentInChildren.text = chara1Strs[0];
			source4.SubscribeWithState(componentInChildren, delegate(bool isAlive, TextMeshProUGUI t)
			{
				t.text = chara1Strs[isAlive ? 1 : 0];
			}).AddTo(disposables);
			TextMeshProUGUI componentInChildren2 = chara2Button.GetComponentInChildren<TextMeshProUGUI>();
			componentInChildren2.text = chara2Strs[0];
			source4.SubscribeWithState(componentInChildren2, delegate(bool isAlive, TextMeshProUGUI t)
			{
				t.text = chara2Strs[isAlive ? 1 : 0];
			}).AddTo(disposables);
			SaveData.Heroine[] fixHeroines = heroineList.Where((SaveData.Heroine p) => p.fixCharaID < 0).ToArray();
			enterPreview.Select(delegate(PreviewClassData preview)
			{
				if (preview == null)
				{
					return false;
				}
				if (preview.data == null)
				{
					return true;
				}
				if (preview.data == player)
				{
					return false;
				}
				return !fixHeroines.Contains(preview.data);
			}).SubscribeToInteractable(chara1Button).AddTo(disposables);
			SaveData.Heroine mankenHeroine = heroineList.FirstOrDefault((SaveData.Heroine p) => p.fixCharaID == -8);
			enterPreview.Select(delegate(PreviewClassData preview)
			{
				if (preview == null)
				{
					return false;
				}
				if (preview.data == null)
				{
					return true;
				}
				if (mankenHeroine == preview.data)
				{
					return true;
				}
				return !fixHeroines.Contains(preview.data);
			}).SubscribeToInteractable(chara2Button).AddTo(disposables);
			enterPreview.Select((PreviewClassData p) => p != null && p.data != null).SubscribeToInteractable(customSceneButton).AddTo(disposables);
			enterPreview.Subscribe(delegate(PreviewClassData preview)
			{
				if (preview == null || preview.data == null || !(preview.data is SaveData.Heroine) || (preview.data as SaveData.Heroine).fixCharaID < 0)
				{
					cvsAttribute.chaFile = null;
				}
				else
				{
					cvsAttribute.chaFile = preview.data.charFile;
				}
			}).AddTo(disposables);
			Toggle[] source5 = (from t in classTab.Children()
				select t.GetComponent<Toggle>()).ToArray();
			(from list in source5.Select(UnityUIComponentExtensions.OnValueChangedAsObservable).CombineLatest()
				select list.IndexOf(true) into i
				where i >= 0
				select i).Subscribe(delegate(int i)
			{
				_page.Value = i;
			}).AddTo(disposables);
			ReadOnlyReactiveProperty<int> source6 = randomCheck.ToReadOnlyReactiveProperty();
			source6.Subscribe(delegate(int i)
			{
				randomTarget = i;
			}).AddTo(disposables);
			source6.Skip(1).Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
			}).AddTo(disposables);
			ReadOnlyReactiveProperty<int> source7 = emptyTargetCheck.ToReadOnlyReactiveProperty();
			source7.Subscribe(delegate(int i)
			{
				isEmptyTargetAll = i == 0;
			}).AddTo(disposables);
			source7.Skip(1).Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
			}).AddTo(disposables);
			PreviewClassData playerSideSheet = charaPreviewList[player.schoolClassIndex + 1];
			List<GameObject> source8 = playerSideSheet.rootObj.Children();
			GameObject imgSwim = source8.Last();
			UI_OnMouseOverMessage uiOnMouseOverMessage = source8.First().GetComponent<UI_OnMouseOverMessage>();
			_page.Select((int i) => !isPlayerSchoolClass(i)).Subscribe(delegate(bool isOn)
			{
				playerSideSheet.button.interactable = isOn;
				imgSwim.SetActiveIfDifferent(!isOn);
				uiOnMouseOverMessage.enabled = !isOn;
			}).AddTo(disposables);
			_page.Skip(1).Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
			}).AddTo(disposables);
			_page.Subscribe(delegate(int schoolClass)
			{
				charaPreviewList.ForEach(delegate(PreviewClassData preview)
				{
					preview.Clear();
				});
				Resources.UnloadUnusedAssets();
				if (schoolClass == pageMax)
				{
					List<SaveData.CharaData> list2 = charaList.Where((SaveData.CharaData p) => p.schoolClass == -1).ToList();
					int count = list2.Count;
					for (int l = 0; l < charaPreviewList.Count; l++)
					{
						charaPreviewList[l].isVisible = l < count;
					}
					for (int m = 0; m < charaPreviewList.Count && m < count; m++)
					{
						charaPreviewList[m].Set(list2[m]);
					}
				}
				else
				{
					List<SaveData.CharaData> list3 = charaList.Where((SaveData.CharaData p) => p.schoolClass == schoolClass).ToList();
					int classRegisterMax = GetClassRegisterMax(schoolClass);
					for (int n = 0; n < charaPreviewList.Count; n++)
					{
						charaPreviewList[n].isVisible = n < classRegisterMax;
					}
					for (int num = 0; num < charaPreviewList.Count; num++)
					{
						PreviewClassData previewClassData = charaPreviewList[num];
						int num2 = list3.FindIndex((SaveData.CharaData p) => p.schoolClassIndex == num);
						if (num2 != -1)
						{
							SaveData.CharaData charaData = list3[num2];
							list3.RemoveAt(num2);
							previewClassData.Set(charaData);
						}
					}
				}
				if (swap != null && swap.schoolClass == schoolClass)
				{
					enterPreview.Value = charaPreviewList[swap.schoolClassIndex];
				}
				else
				{
					enterPreview.Value = null;
				}
				PreviewUpdateAll();
			}).AddTo(disposables);
			chara1Button.OnClickAsObservable().Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
				if (enterPreview.Value.data != null)
				{
					RemoveChara(true);
				}
				else
				{
					classRoomSelectScene.OpenPreview();
				}
			}).AddTo(disposables);
			chara2Button.OnClickAsObservable().Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
				PreviewClassData preview2 = enterPreview.Value;
				if (preview2.data != null)
				{
					string levelName = "NickNameSetting";
					Scene.Data data = new Scene.Data
					{
						levelName = levelName,
						isAdd = true,
						isAsync = true,
						onLoad = delegate
						{
							NickNameSettingScene rootComponent = Scene.GetRootComponent<NickNameSettingScene>(levelName);
							if (!(rootComponent == null))
							{
								canvas.enabled = false;
								rootComponent.OnDestroyAsObservable().TakeUntilDestroy(this).Subscribe(delegate
								{
									canvas.enabled = true;
								});
								rootComponent.charaData = preview2.data;
								rootComponent.onEnter += delegate
								{
									Singleton<Scene>.Instance.UnLoad();
									preview2.Update();
									if (preview2.data is SaveData.Player)
									{
										charaPreviewList.ForEach(delegate(PreviewClassData item)
										{
											item.Update();
										});
									}
								};
								rootComponent.onCancel += delegate
								{
									Singleton<Scene>.Instance.UnLoad();
								};
							}
						}
					};
					Singleton<Scene>.Instance.LoadReserve(data, true);
				}
				else
				{
					SaveData.Heroine heroine3 = new SaveData.Heroine(true);
					heroine3.schoolClass = page;
					heroine3.schoolClassIndex = preview2.classIndex;
					Observable.FromCoroutine((CancellationToken __) => RandomNetChara(heroine3)).Subscribe(delegate
					{
						PreviewUpdate();
					});
				}
			}).AddTo(disposables);
			ReadOnlyReactiveProperty<int> source9 = cardInformation.ToReadOnlyReactiveProperty();
			source9.Select((int i) => (PreviewClassData.VisibleMode)i).Subscribe(delegate(PreviewClassData.VisibleMode mode)
			{
				charaPreviewList.ForEach(delegate(PreviewClassData p)
				{
					p.visibleMode = mode;
				});
			}).AddTo(disposables);
			source9.Skip(1).Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
			}).AddTo(disposables);
			customSceneButton.OnClickAsObservable().Subscribe(delegate
			{
				PlaySE(SystemSE.ok_s);
				isCustomScene = true;
				string levelName2 = "CustomScene";
				Scene.Data data2 = new Scene.Data
				{
					levelName = levelName2,
					isAdd = true,
					isFade = true,
					isAsync = true,
					onLoad = delegate
					{
						CustomScene rootComponent2 = Scene.GetRootComponent<CustomScene>(levelName2);
						if (!(rootComponent2 == null))
						{
							classRoomSelectScene.gameObject.SetActive(false);
							SaveData.CharaData charaData2 = enterPreview.Value.data;
							rootComponent2.modeNew = false;
							rootComponent2.chaFileCtrl = charaData2.charFile;
							Camera nowCamea = null;
							if (Singleton<Game>.IsInstance())
							{
								ActionScene actScene = Singleton<Game>.Instance.actScene;
								if (actScene != null)
								{
									actScene.PlayerDelete();
									actScene.npcList.ForEach(delegate(NPC npc)
									{
										UnityEngine.Object.Destroy(npc.gameObject);
									});
									actScene.npcList.Clear();
									actScene.FixCharaDelete();
									actScene.actCtrl.Refresh();
									actScene.Map.mapRoot.SetActive(false);
									nowCamea = Camera.main;
									if (nowCamea != null)
									{
										nowCamea.gameObject.SetActive(false);
									}
								}
								if (Singleton<Character>.IsInstance())
								{
									Singleton<Character>.Instance.EndLoadAssetBundle();
								}
							}
							rootComponent2.OnDestroyAsObservable().TakeUntilDestroy(this).Subscribe(delegate
							{
								Utils.Sound.Play(new Utils.Sound.SettingBGM(prevBGM));
								if (Singleton<Character>.IsInstance() && classRoomSelectScene != null)
								{
									classRoomSelectScene.gameObject.SetActive(true);
									classRoomSelectScene.previewCharaList.CharFile.Initialize();
									if (Singleton<Character>.Instance.editChara != null)
									{
										ChaFileControl editChara = Singleton<Character>.Instance.editChara;
										SaveData.CharaData charaData3 = charaList.Find((SaveData.CharaData p) => p == charaData2);
										charaData3.SetCharFile(editChara);
										if (!(charaData3 is SaveData.Heroine) || (charaData3 as SaveData.Heroine).fixCharaID < 0)
										{
											cvsAttribute.chaFile = null;
										}
										else
										{
											cvsAttribute.chaFile = editChara;
										}
										enterPreview.Value.Set(charaData3);
									}
									if (nowCamea != null)
									{
										nowCamea.gameObject.SetActive(true);
									}
									if (Singleton<Game>.IsInstance())
									{
										ActionScene actScene2 = Singleton<Game>.Instance.actScene;
										if (actScene2 != null)
										{
											actScene2.Map.mapRoot.SetActive(true);
										}
									}
									Singleton<Character>.Instance.editChara = null;
								}
							});
						}
					}
				};
				Singleton<Scene>.Instance.LoadReserve(data2, true);
			}).AddTo(disposables);
			classRoomSelectScene.previewCharaList.onEnter += delegate(ChaFileControl chaFile)
			{
				int schoolClass2 = page;
				int classIndex = enterPreview.Value.classIndex;
				SaveData.Heroine heroine4 = new SaveData.Heroine(chaFile, true)
				{
					charFileInitialized = true,
					schoolClass = schoolClass2,
					schoolClassIndex = classIndex
				};
				charaList.Add(heroine4);
				enterPreview.Value.Set(heroine4);
				PreviewUpdate();
				classRoomSelectScene.OpenClassRoom();
			};
			classRoomSelectScene.previewCharaList.onCancel += delegate
			{
				classRoomSelectScene.OpenClassRoom();
			};
			IObservable<bool> source10 = (from _ in this.UpdateAsObservable()
				select isEmptyTargetAll || page != pageMax).Share();
			source10.Select((bool isOn) => isOn && emptyRegisterSum > 0).SubscribeToInteractable(randomButton).AddTo(disposables);
			source10.Select((bool isOn) => isOn && ((!isEmptyTargetAll) ? Enumerable.Range(0, charaPreviewList.Count).Any((int index) => FindChara(page, index) is SaveData.Heroine) : charaList.OfType<SaveData.Heroine>().Any((SaveData.Heroine p) => p.fixCharaID == 0))).SubscribeToInteractable(removeButton).AddTo(disposables);
			Func<int, List<SaveData.Heroine>> getRamdomList = delegate(int schoolClass)
			{
				bool isPlayerSchoolClass = this.isPlayerSchoolClass(schoolClass);
				return (from heroine in Enumerable.Range(0, GetClassRegisterMax(schoolClass)).Select(delegate(int index)
					{
						if (isPlayerSchoolClass && (index == player.schoolClassIndex || index == player.schoolClassIndex + 1))
						{
							return (SaveData.Heroine)null;
						}
						return (FindChara(schoolClass, index) is SaveData.Heroine) ? null : new SaveData.Heroine(true)
						{
							schoolClass = schoolClass,
							schoolClassIndex = index
						};
					})
					where heroine != null
					select heroine).ToList();
			};
			randomButton.OnClickAsObservable().Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
				List<SaveData.Heroine> list4 = new List<SaveData.Heroine>();
				if (!isEmptyTargetAll)
				{
					list4.AddRange(getRamdomList(page));
				}
				else
				{
					for (int num3 = 0; num3 < pageMax; num3++)
					{
						list4.AddRange(getRamdomList(num3));
					}
				}
				Observable.FromCoroutine((CancellationToken __) => RandomNetChara(list4)).Subscribe(delegate
				{
					PreviewUpdateAll();
				});
			}).AddTo(disposables);
			removeButton.OnClickAsObservable().Subscribe(delegate
			{
				PlaySE(SystemSE.sel);
				int schoolClass3 = page;
				CheckScene.Parameter param = new CheckScene.Parameter();
				param.Yes = delegate
				{
					SaveData saveData = Singleton<Game>.Instance.saveData;
					if (!isEmptyTargetAll)
					{
						(from index in Enumerable.Range(0, charaPreviewList.Count)
							select FindChara(schoolClass3, index) as SaveData.Heroine into heroine
							where heroine != null
							select heroine).ToList().ForEach(delegate(SaveData.Heroine chara)
						{
							if (saveData.withHeroine.Check(chara))
							{
								saveData.withHeroine.Set(null);
							}
							if (saveData.dateHeroine.Check(chara))
							{
								saveData.dateHeroine.Set(null);
							}
							charaList.Remove(chara);
						});
					}
					else
					{
						saveData.withHeroine.Set(null);
						saveData.dateHeroine.Set(null);
						charaList.RemoveAll(delegate(SaveData.CharaData p)
						{
							SaveData.Heroine heroine5 = p as SaveData.Heroine;
							return heroine5 != null && heroine5.fixCharaID == 0;
						});
					}
					int count2 = ((schoolClass3 != pageMax) ? charaPreviewList.Count : 0);
					foreach (int item in from index in Enumerable.Range(0, count2)
						where FindCharaIndex(schoolClass3, index) == -1
						select index)
					{
						charaPreviewList[item].Clear();
					}
					PreviewUpdate();
					Singleton<Scene>.Instance.UnLoad();
				};
				param.No = delegate
				{
					Singleton<Scene>.Instance.UnLoad();
				};
				param.Title = GetTranslateQuestionTitle(isEmptyTargetAll ? 1 : 2) ?? (((!isEmptyTargetAll) ? "クラス内のキャラ" : "全キャラ") + "を転校させますか？");
				Observable.FromCoroutine((IObserver<CheckScene> observer) => Utils.Scene.Check.Load(param, observer)).StartAsCoroutine();
			}).AddTo(disposables);
			GameObject enterButtonObject = enterButton.gameObject;
			if (Singleton<Scene>.Instance.AddSceneName == "ClassRoomSelect")
			{
				enterButtonObject.SetActive(false);
			}
			Action enterProc = delegate
			{
				charaList.ForEach(SaveData.CallNormalize);
				charaList.Remove(player);
				heroineList.Clear();
				heroineList.AddRange(charaList.OfType<SaveData.Heroine>());
				if (Singleton<Game>.Instance.actScene != null)
				{
					Singleton<Game>.Instance.actScene.actCtrl.Refresh();
					Singleton<Game>.Instance.actScene.paramUI.UpdatePlayer();
				}
				if (Singleton<Scene>.Instance.AddSceneName == "ClassRoomSelect")
				{
					Singleton<Scene>.Instance.UnLoad();
				}
				else
				{
					Singleton<Scene>.Instance.LoadReserve(new Scene.Data
					{
						levelName = "Action",
						fadeType = Scene.Data.FadeType.In,
						isAsync = true
					}, false);
				}
				PlaySE(SystemSE.ok_s);
			};
			if (enterButtonObject.activeSelf)
			{
				enterButton.OnClickAsObservable().Take(1).Subscribe(delegate
				{
					enterProc();
				});
			}
			returnButton.OnClickAsObservable().Take(1).Subscribe(delegate
			{
				if (!enterButtonObject.activeSelf)
				{
					enterProc();
				}
				else
				{
					PlaySE(SystemSE.cancel);
					Singleton<Scene>.Instance.UnLoad();
				}
			});
			ReadOnlyReactiveProperty<int> source11 = (from _ in this.UpdateAsObservable()
				select Manager.Config.ActData.MaxCharaNum).ToReadOnlyReactiveProperty();
			if (textAttendanceNum != null)
			{
				string s = Localize.Translate.Manager.OtherData.Get(4).Values.FindTagText("Members") ?? "{0} 人";
				source11.Select((int i) => string.Format(s, i)).SubscribeToText(textAttendanceNum).AddTo(disposables);
			}
			if (sldAttendanceNum != null)
			{
				source11.Subscribe(delegate(int value)
				{
					sldAttendanceNum.value = value;
				}).AddTo(disposables);
				sldAttendanceNum.maxValue = 38f;
				sldAttendanceNum.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					Manager.Config.ActData.MaxCharaNum = (int)value;
				}).AddTo(disposables);
				(from evData in sldAttendanceNum.OnScrollAsObservable()
					select evData.scrollDelta.y).Subscribe(delegate(float value)
				{
					int num4 = 0;
					if (value < 0f)
					{
						num4++;
					}
					else if (value > 0f)
					{
						num4--;
					}
					sldAttendanceNum.value = Mathf.Clamp((int)sldAttendanceNum.value + num4, 0, 38);
				}).AddTo(disposables);
				(from evData in sldAttendanceNum.OnPointerDownAsObservable()
					where evData.button == PointerEventData.InputButton.Left
					select evData).Subscribe(delegate
				{
					PlaySE(SystemSE.sel);
				}).AddTo(disposables);
			}
			if (btnAttendanceNum != null)
			{
				btnAttendanceNum.OnClickAsObservable().Subscribe(delegate
				{
					PlaySE(SystemSE.sel);
					Manager.Config.ActData.MaxCharaNum = 38;
				}).AddTo(disposables);
			}
			ReadOnlyReactiveProperty<bool> source12 = (from _ in this.UpdateAsObservable()
				select Manager.Config.AddData.OtherClassRegisterMax).ToReadOnlyReactiveProperty();
			if (tglOtherClassRegisterMax != null)
			{
				source12.Subscribe(delegate(bool isOn)
				{
					tglOtherClassRegisterMax.isOn = isOn;
				}).AddTo(disposables);
				tglOtherClassRegisterMax.onValueChanged.AsObservable().Subscribe(delegate(bool isOn)
				{
					Manager.Config.AddData.OtherClassRegisterMax = isOn;
				}).AddTo(disposables);
				(from evData in tglOtherClassRegisterMax.OnPointerClickAsObservable()
					where evData.button == PointerEventData.InputButton.Left
					select evData).Subscribe(delegate
				{
					PlaySE(SystemSE.sel);
				}).AddTo(disposables);
			}
			source12.Where((bool _) => page != pageMax).Subscribe(delegate
			{
				int classRegisterMax2 = GetClassRegisterMax(page);
				for (int num5 = 0; num5 < charaPreviewList.Count; num5++)
				{
					charaPreviewList[num5].isVisible = num5 < classRegisterMax2;
				}
				PreviewUpdateAll();
			}).AddTo(disposables);
		}

		private void OnDestroy()
		{
			if (Singleton<Character>.IsInstance())
			{
				Singleton<Character>.Instance.lstProductId.Clear();
			}
			disposables.Clear();
		}

		private IEnumerator RandomNetChara(SaveData.Heroine heroine)
		{
			ChaFileControl[] result = null;
			switch (randomTarget)
			{
			case 0:
				yield return StartCoroutine(LoadRandomSelectScene(1));
				result = Singleton<Character>.Instance.netRandChara;
				break;
			case 1:
				result = Localize.Translate.Manager.GetRandomUserDataFemaleCard(1);
				break;
			case 2:
				result = Localize.Translate.Manager.GetRandomDefaultDataFemaleCard(1);
				break;
			}
			if (!result.IsNullOrEmpty())
			{
				heroine.SetCharFile(result.First());
				heroine.charFileInitialized = true;
				charaList.Add(heroine);
				enterPreview.Value.Set(heroine);
			}
		}

		private IEnumerator RandomNetChara(List<SaveData.Heroine> randomHeroineList)
		{
			ChaFileControl[] result = null;
			switch (randomTarget)
			{
			case 0:
				yield return StartCoroutine(LoadRandomSelectScene(emptyRegisterSum));
				result = Singleton<Character>.Instance.netRandChara;
				break;
			case 1:
				result = Localize.Translate.Manager.GetRandomUserDataFemaleCard(emptyRegisterSum);
				break;
			case 2:
				result = Localize.Translate.Manager.GetRandomDefaultDataFemaleCard(emptyRegisterSum);
				break;
			}
			if (result.IsNullOrEmpty())
			{
				yield break;
			}
			int cnt = 0;
			foreach (SaveData.Heroine item in randomHeroineList.Shuffle().Take(result.Length))
			{
				item.SetCharFile(result[cnt++]);
				item.charFileInitialized = true;
				charaList.Add(item);
				if (item.schoolClass == page)
				{
					charaPreviewList[item.schoolClassIndex].Set(item);
				}
			}
		}

		private IEnumerator LoadRandomSelectScene(int num)
		{
			bool isLoaded = false;
			string levelName = "NetworkCheckScene";
			Singleton<Character>.Instance.nextNetworkScene = "RandomNetChara";
			Singleton<Character>.Instance.netRandCharaNum = num;
			Singleton<Character>.Instance.netRandChara = null;
			Scene.Data data = new Scene.Data
			{
				levelName = levelName,
				isAdd = true,
				isFade = false,
				isAsync = true,
				onLoad = delegate
				{
					isLoaded = true;
					NetworkCheckScene rootComponent = Scene.GetRootComponent<NetworkCheckScene>(levelName);
					if (rootComponent == null)
					{
						isVisible = true;
					}
					else
					{
						isVisible = false;
						rootComponent.OnDestroyAsObservable().TakeUntilDestroy(this).Subscribe(delegate
						{
							isVisible = true;
						});
					}
				}
			};
			Singleton<Scene>.Instance.LoadReserve(data, true);
			yield return new WaitUntil(() => isLoaded);
            yield return new WaitUntil(() => this.isVisible);
        }
	}
}
