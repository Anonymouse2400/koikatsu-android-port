using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class RankingScene : MonoBehaviour
{
	public class RankingInfo
	{
		public int index = -1;

		public string userName = string.Empty;

		public uint[] count = new uint[26];

		public int[] ranking = new int[26];

		public string playTime
		{
			get
			{
				uint num = count[19];
				int b = (int)(num / 3600);
				b = Mathf.Min(9999, b);
				int num2 = (int)(num % 3600) / 60;
				string empty = string.Empty;
				if (b != 0)
				{
					return string.Format("{0}時間{1}分", b, num2);
				}
				return string.Format("{0}分", num2);
			}
		}

		public RankingInfo(RankingInfo src = null)
		{
			if (src != null)
			{
				Copy(src);
			}
		}

		public void Copy(RankingInfo src)
		{
			index = src.index;
			userName = src.userName;
			for (int i = 0; i < count.Length; i++)
			{
				count[i] = src.count[i];
				ranking[i] = src.ranking[i];
			}
		}

		public string PlayTime(Dictionary<int, Data.Param> translateOther)
		{
			uint num = count[19];
			int b = (int)(num / 3600);
			b = Mathf.Min(9999, b);
			int num2 = (int)(num % 3600) / 60;
			string empty = string.Empty;
			if (b != 0)
			{
				return string.Format(translateOther.SafeGetText(0) ?? "{0}時間{1}分", b, num2);
			}
			return string.Format(translateOther.SafeGetText(1) ?? "{0}分", num2);
		}
	}

	[SerializeField]
	private Canvas cvsChangeScene;

	[SerializeField]
	private GameObject objNoControl;

	[SerializeField]
	private PopupCheck popupCheck;

	[SerializeField]
	private Transform trfContants;

	[SerializeField]
	private GameObject objItemTemp;

	[SerializeField]
	private TextMeshProUGUI txtIndividualRankTitle;

	[SerializeField]
	private TextMeshProUGUI txtIndividualMyRank;

	[SerializeField]
	private TextMeshProUGUI txtIndividualMyScore;

	[SerializeField]
	private Text txtHandleName;

	[SerializeField]
	private Button btnMyRanking;

	[SerializeField]
	private Transform trfIndividualContants;

	[SerializeField]
	private GameObject objIndividualItemTemp;

	[SerializeField]
	private Toggle[] tglPage;

	[SerializeField]
	private TextMeshProUGUI[] txtPage;

	[SerializeField]
	private Button btnBack;

	[SerializeField]
	private Button btnChangeHN;

	[SerializeField]
	private GameObject objMessage;

	[SerializeField]
	private Text txtMessage;

	private string RankingURL = string.Empty;

	private float msgCount;

	private StringReactiveProperty srpHandleName;

	private int selItemNo;

	private int selPageNo;

	private int myIndex = -1;

	private int myPageNo = -1;

	private float myPosition;

	private bool rankingDebug;

	private List<RankingInfo> lstRanking = new List<RankingInfo>();

	private RankingInfo myNewRankingInfo = new RankingInfo();

	private int[] prevRanks = new int[26];

	private string[] rankingItemName = new string[26]
	{
		"覗きマスター：トイレ", "覗きマスター：シャワー", "覗きマスター：オナニー", "豪胆Ｈマスター", "絶頂名人", "中出しマスター", "中出しマスター：生", "中出しマスター：アナル", "外出し仙人", "怒らせ名人",
		"デートマスター", "自宅Ｈ好き", "おっぱい星人", "お尻フェチ", "ゴッドフィンガー", "おもちゃマイスター", "部員勧誘に成功した回数", "これまで付き合った人数", "経験人数", "プレイ時間",
		"潜入失敗：ロッカー編", "潜入失敗：トイレ編", "潜入失敗：シャワー編", "筋肉大好き", "勉強大好き", "エロ大好き"
	};

	private int[] rankingSiblingNo = new int[26]
	{
		20, 21, 22, 14, 15, 16, 17, 18, 19, 7,
		8, 9, 10, 11, 12, 13, 0, 1, 2, 3,
		23, 24, 25, 4, 5, 6
	};

	private RankingItemComponent[] cmpItem;

	private RankingItemComponent[] cmpIndividualItem;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private RankingData rankingDat
	{
		get
		{
			return Singleton<Game>.Instance.rankSaveData;
		}
	}

	private Dictionary<int, Data.Param> translateItemTitle
	{
		get
		{
			return uiTranslater.Get(2);
		}
	}

	private Dictionary<int, Data.Param> translateMessageTitle
	{
		get
		{
			return uiTranslater.Get(3);
		}
	}

	private Dictionary<int, Data.Param> translateOther
	{
		get
		{
			return uiTranslater.Get(4);
		}
	}

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.RANKING));
		}
	}

	private void SetMessage(string msg, float time)
	{
		msgCount = time;
		txtMessage.text = msg;
		objMessage.SetActiveIfDifferent(true);
	}

	private void DisvisibleMessage()
	{
		msgCount = 0f;
		txtMessage.text = string.Empty;
		objMessage.SetActiveIfDifferent(false);
	}

	private IEnumerator GetInfoAll()
	{
		SetMessage(translateMessageTitle.Values.FindTagText("GetData") ?? "データを取得しています", -1f);
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		string uid = CommonLib.GetUUID();
		if (string.Empty == uid)
		{
			CommonLib.CreateUUID();
			uid = CommonLib.GetUUID();
		}
		string userName = rankingDat.userName;
		string encHN = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
		if (rankingDebug)
		{
			encHN = userName;
		}
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 0);
		wwwform.AddField("uid", uid);
		wwwform.AddField("userName", encHN);
		string[] countNames = new string[26]
		{
			"peepToiletCount", "peepShowerCount", "peepOnanismCount", "easyPlaceHCount", "orgasmCount", "inEjaculationCount", "inEjaculationKokanNamaCount", "inEjaculationAnalCount", "outEjaculationCount", "angerCount",
			"dateCount", "myRoomHCount", "bustGauge", "hipGauge", "asokoGauge", "adultToyGauge", "staffCount", "girlfriendCount", "expNumCount", "playTime",
			"angerLockerCount", "angerToiletCount", "angerShowerCount", "physicalCount", "intellectCount", "hentaiCount"
		};
		for (int i = 0; i < countNames.Length; i++)
		{
			wwwform.AddField(countNames[i], myNewRankingInfo.count[i].ToString());
		}
		ObservableYieldInstruction<string> o = ObservableWWW.Post(RankingURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
		yield return o;
		if (o.HasError)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedData") ?? "データの取得に失敗しました", 3f);
			Observable.FromCoroutine(ReturnTitleCor).Subscribe(delegate
			{
			}, delegate
			{
				objNoControl.SetActiveIfDifferent(false);
			}, delegate
			{
				objNoControl.SetActiveIfDifferent(false);
			});
		}
		else
		{
			if (!o.HasResult)
			{
				yield break;
			}
			lstRanking.Clear();
			string[] array = o.Result.Split("\n"[0]);
			myIndex = int.Parse(array[0]);
			for (int j = 1; j < array.Length && !(string.Empty == array[j]); j++)
			{
				string[] array2 = array[j].Split("\t"[0]);
				RankingInfo rankingInfo = new RankingInfo();
				if (string.Empty == array2[0])
				{
					continue;
				}
				rankingInfo.index = int.Parse(array2[0]);
				if (string.Empty == array2[1])
				{
					continue;
				}
				rankingInfo.userName = Encoding.UTF8.GetString(Convert.FromBase64String(array2[1]));
				for (int k = 0; k < rankingInfo.count.Length; k++)
				{
					if (!(string.Empty == array2[2 + k]))
					{
						rankingInfo.count[k] = uint.Parse(array2[2 + k]);
					}
				}
				lstRanking.Add(rankingInfo);
			}
			DisvisibleMessage();
		}
	}

	private IEnumerator UpdateHandleName()
	{
		SetMessage(translateMessageTitle.Values.FindTagText("UpdateHandleName") ?? "ハンドルネームを更新しています", -1f);
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		string uid = CommonLib.GetUUID();
		if (string.Empty == uid)
		{
			CommonLib.CreateUUID();
			uid = CommonLib.GetUUID();
		}
		string userName = rankingDat.userName;
		string encHN = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
		if (rankingDebug)
		{
			encHN = userName;
		}
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 1);
		wwwform.AddField("uid", uid);
		wwwform.AddField("userName", encHN);
		ObservableYieldInstruction<string> o = ObservableWWW.Post(RankingURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
		yield return o;
		if (o.HasError)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedHandleName") ?? "ハンドルネームの更新に失敗しました", 3f);
			yield break;
		}
		if (o.HasResult)
		{
			DisvisibleMessage();
		}
		yield return null;
	}

	private IEnumerator ReturnTitleCor()
	{
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		while (!Input.anyKey && objMessage.activeSelf)
		{
			yield return null;
		}
		cvsChangeScene.gameObject.SetActive(true);
		Singleton<Scene>.Instance.LoadReserve(new Scene.Data
		{
			levelName = "Title",
			isFade = true
		}, false);
		yield return null;
	}

	public void UpdateRankingList()
	{
		int count = lstRanking.Count;
		if (count == 0)
		{
			return;
		}
		int num = Mathf.Min(9, count / 100 - ((count % 100 == 0) ? 1 : 0));
		for (int i = 0; i < 10; i++)
		{
			tglPage[i].interactable = i <= num;
			txtPage[i].color = new Color(txtPage[i].color.r, txtPage[i].color.g, txtPage[i].color.b, (i > num) ? 0.7f : 1f);
		}
		int j;
		for (j = 0; j < cmpItem.Length; j++)
		{
			var enumerable = from x in lstRanking.OrderByDescending((RankingInfo x) => x.count[j]).Select((RankingInfo value, int index) => new { value, index })
				group x by x.value.count[j] into x
				select new
				{
					ranking = x.First().index,
					items = x.Select(y => y.value)
				};
			List<RankingInfo> list = new List<RankingInfo>();
			foreach (var item in enumerable)
			{
				foreach (RankingInfo item2 in item.items)
				{
					item2.ranking[j] = item.ranking;
					list.Add(item2);
				}
			}
			lstRanking = list;
			int num2 = lstRanking.First((RankingInfo n) => n.index == myIndex).ranking[j];
			if (num2 >= 999)
			{
				cmpItem[j].textRanking.text = translateOther.SafeGetText(3) ?? "圏外";
				if (prevRanks[j] < 999)
				{
					cmpItem[j].RankChangeImage(RankingItemComponent.ChangeType.DOWN);
				}
				else
				{
					cmpItem[j].RankChangeImage(RankingItemComponent.ChangeType.KEEP);
				}
			}
			else
			{
				if (prevRanks[j] > num2)
				{
					cmpItem[j].RankChangeImage(RankingItemComponent.ChangeType.UP);
				}
				else if (prevRanks[j] == num2)
				{
					cmpItem[j].RankChangeImage(RankingItemComponent.ChangeType.KEEP);
				}
				else
				{
					cmpItem[j].RankChangeImage(RankingItemComponent.ChangeType.DOWN);
				}
				cmpItem[j].textRanking.text = string.Format(translateOther.SafeGetText(2) ?? "{0} 位", num2 + 1);
			}
			rankingDat.prevRanks[j] = num2;
			if (j == 19)
			{
				cmpItem[j].textScore.text = myNewRankingInfo.PlayTime(translateOther);
			}
			else
			{
				cmpItem[j].textScore.text = myNewRankingInfo.count[j].ToString();
			}
		}
		rankingDat.Save();
		ChangeItemInfo();
	}

	private void ChangeItemInfo()
	{
		if (lstRanking.Count == 0)
		{
			return;
		}
		string text = ((!Localize.Translate.Manager.Check(Localize.Translate.Manager.LanguageID.US)) ? "\u3000" : " ");
		txtIndividualRankTitle.text = (rankingItemName[selItemNo] + text + translateOther.SafeGetText(4)) ?? "ランキング";
		int num = rankingDat.prevRanks[selItemNo];
		txtIndividualMyRank.text = ((999 > num) ? (num + 1).ToString() : (translateOther.SafeGetText(3) ?? "圏外"));
		if (selItemNo == 19)
		{
			txtIndividualMyScore.text = myNewRankingInfo.PlayTime(translateOther);
		}
		else
		{
			txtIndividualMyScore.text = myNewRankingInfo.count[selItemNo].ToString();
		}
		RankingInfo rankingInfo = lstRanking.Find((RankingInfo x) => x.index == myIndex);
		rankingInfo.userName = rankingDat.userName;
		int num2 = selPageNo * 100;
		int num3 = Mathf.Min(lstRanking.Count() - selPageNo * 100, (selPageNo != 9) ? 100 : 99);
		lstRanking = lstRanking.OrderBy((RankingInfo x) => x.ranking[selItemNo]).ToList();
		for (int i = 0; i < num3; i++)
		{
			cmpIndividualItem[i].textHandleName.text = lstRanking[num2 + i].userName;
			if (selItemNo == 19)
			{
				cmpIndividualItem[i].textScore.text = lstRanking[num2 + i].PlayTime(translateOther);
			}
			else
			{
				cmpIndividualItem[i].textScore.text = lstRanking[num2 + i].count[selItemNo].ToString();
			}
			int num4 = lstRanking[num2 + i].ranking[selItemNo];
			switch (num4)
			{
			case 0:
				cmpIndividualItem[i].textRanking.text = string.Empty;
				cmpIndividualItem[i].RankBackImage(RankingItemComponent.RankType.FIRST);
				break;
			case 1:
				cmpIndividualItem[i].textRanking.text = string.Empty;
				cmpIndividualItem[i].RankBackImage(RankingItemComponent.RankType.SECOND);
				break;
			case 2:
				cmpIndividualItem[i].textRanking.text = string.Empty;
				cmpIndividualItem[i].RankBackImage(RankingItemComponent.RankType.THIRD);
				break;
			default:
				cmpIndividualItem[i].textRanking.text = (num4 + 1).ToString();
				cmpIndividualItem[i].RankBackImage(RankingItemComponent.RankType.ETC);
				break;
			}
		}
		for (int j = 0; j < 100; j++)
		{
			cmpIndividualItem[j].gameObject.SetActiveIfDifferent(j < num3);
		}
		int num5 = lstRanking.FindIndex((RankingInfo n) => n.index == myIndex);
		myPageNo = -1;
		myPosition = 0f;
		if (999 <= num5)
		{
			btnMyRanking.interactable = false;
			return;
		}
		btnMyRanking.interactable = true;
		myPageNo = num5 / 100;
		int num6 = num5 % 100;
		float b = 10512f;
		myPosition = Mathf.Min((float)num6 * 112f, b);
	}

	private void Start()
	{
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		if (RankingURL == string.Empty)
		{
			RankingURL = CreateURL.Load_KK_Ranking_URL();
		}
		if (RankingURL.Contains("192.168.1.89"))
		{
			rankingDebug = true;
		}
		Localize.Translate.Manager.BindFont(txtIndividualRankTitle);
		Localize.Translate.Manager.BindFont(txtIndividualMyRank);
		Localize.Translate.Manager.BindFont(txtIndividualMyScore);
		Localize.Translate.Manager.BindFont(txtHandleName);
		Localize.Translate.Manager.BindFont(txtMessage);
		Action<RankingItemComponent> action = delegate(RankingItemComponent data)
		{
			Localize.Translate.Manager.BindFont(data.textTitle);
			Localize.Translate.Manager.BindFont(data.textScore);
			Localize.Translate.Manager.BindFont(data.textRanking);
			Localize.Translate.Manager.BindFont(data.textHandleName);
		};
		action(objItemTemp.GetComponent<RankingItemComponent>());
		action(objIndividualItemTemp.GetComponent<RankingItemComponent>());
		int i;
		for (i = 0; i < rankingItemName.Length; i++)
		{
			translateItemTitle.SafeGetText(i).SafeProc(delegate(string text)
			{
				rankingItemName[i] = text;
			});
		}
		txtHandleName.text = rankingDat.userName;
		srpHandleName = new StringReactiveProperty(rankingDat.userName);
		myNewRankingInfo.count[0] = rankingDat.peepToiletCount;
		myNewRankingInfo.count[1] = rankingDat.peepShowerCount;
		myNewRankingInfo.count[2] = rankingDat.peepOnanismCount;
		myNewRankingInfo.count[3] = rankingDat.easyPlaceHCount;
		myNewRankingInfo.count[4] = rankingDat.orgasmCount;
		myNewRankingInfo.count[5] = rankingDat.inEjaculationCount;
		myNewRankingInfo.count[6] = rankingDat.inEjaculationKokanNamaCount;
		myNewRankingInfo.count[7] = rankingDat.inEjaculationAnalCount;
		myNewRankingInfo.count[8] = rankingDat.outEjaculationCount;
		myNewRankingInfo.count[9] = rankingDat.angerCount;
		myNewRankingInfo.count[10] = rankingDat.dateCount;
		myNewRankingInfo.count[11] = rankingDat.myRoomHCount;
		myNewRankingInfo.count[12] = rankingDat.bustGauge;
		myNewRankingInfo.count[13] = rankingDat.hipGauge;
		myNewRankingInfo.count[14] = rankingDat.asokoGauge;
		myNewRankingInfo.count[15] = rankingDat.adultToyGauge;
		myNewRankingInfo.count[16] = rankingDat.staffCount;
		myNewRankingInfo.count[17] = rankingDat.girlfriendCount;
		myNewRankingInfo.count[18] = rankingDat.expNumCount;
		myNewRankingInfo.count[19] = rankingDat.playTime;
		myNewRankingInfo.count[20] = rankingDat.angerLockerCount;
		myNewRankingInfo.count[21] = rankingDat.angerToiletCount;
		myNewRankingInfo.count[22] = rankingDat.angerShowerCount;
		myNewRankingInfo.count[23] = rankingDat.physicalCount;
		myNewRankingInfo.count[24] = rankingDat.intellectCount;
		myNewRankingInfo.count[25] = rankingDat.hentaiCount;
		for (int j = 0; j < prevRanks.Length; j++)
		{
			prevRanks[j] = rankingDat.prevRanks[j];
		}
		cmpItem = new RankingItemComponent[rankingItemName.Length];
		for (int k = 0; k < rankingItemName.Length; k++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(objItemTemp, trfContants, false);
			cmpItem[k] = gameObject.GetComponent<RankingItemComponent>();
			cmpItem[k].textTitle.text = rankingItemName[k];
			cmpItem[k].textScore.text = string.Empty;
			cmpItem[k].textRanking.text = string.Empty;
			if (rankingSiblingNo[k] == 0)
			{
				selItemNo = k;
			}
			gameObject.SetActiveIfDifferent(true);
		}
		Transform[] array = (from x in cmpItem.Select((RankingItemComponent val, int idx) => new
			{
				value = val.transform,
				idx = rankingSiblingNo[idx]
			})
			orderby x.idx
			select x into n
			select n.value).ToArray();
		for (int l = 0; l < array.Length; l++)
		{
			array[l].SetSiblingIndex(l);
		}
		cmpItem[selItemNo].toggle.isOn = true;
		cmpIndividualItem = new RankingItemComponent[100];
		for (int m = 0; m < 100; m++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(objIndividualItemTemp, trfIndividualContants, false);
			cmpIndividualItem[m] = gameObject2.GetComponent<RankingItemComponent>();
			cmpIndividualItem[m].textScore.text = string.Empty;
			cmpIndividualItem[m].textRanking.text = string.Empty;
			cmpIndividualItem[m].textHandleName.text = string.Empty;
		}
		tglPage[0].isOn = true;
		cmpItem.Select((RankingItemComponent val, int idx) => new { val, idx }).ToList().ForEach(item =>
		{
			item.val.toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					selItemNo = item.idx;
					ChangeItemInfo();
				}
			});
		});
		tglPage.Select((Toggle tgl, int idx) => new { tgl, idx }).ToList().ForEach(item =>
		{
			item.tgl.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					selPageNo = item.idx;
					ChangeItemInfo();
				}
			});
		});
		Observable.FromCoroutine(GetInfoAll).Subscribe(delegate
		{
			UpdateRankingList();
		}, delegate
		{
			objNoControl.SetActiveIfDifferent(false);
		}, delegate
		{
			objNoControl.SetActiveIfDifferent(false);
		});
		Observable.EveryUpdate().Subscribe(delegate
		{
			srpHandleName.Value = rankingDat.userName;
			if (0f < msgCount)
			{
				msgCount -= Time.deltaTime;
				msgCount = Mathf.Max(0f, msgCount);
				if (msgCount == 0f)
				{
					DisvisibleMessage();
				}
			}
			if ("RankingScene" == Singleton<Scene>.Instance.NowSceneNames[0] && !objNoControl.activeSelf && !cvsChangeScene.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
			{
				cvsChangeScene.gameObject.SetActive(true);
				Utils.Scene.GameEnd();
			}
		}).AddTo(this);
		btnBack.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			if ((bool)objNoControl)
			{
				objNoControl.SetActiveIfDifferent(true);
			}
			cvsChangeScene.gameObject.SetActive(true);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "Title",
				isFade = true
			}, false);
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where (!objNoControl || !objNoControl.activeSelf) ? true : false
			where !cvsChangeScene.gameObject.activeSelf
			where !Singleton<Scene>.Instance.IsOverlap
			select _).Take(1).Subscribe(delegate
		{
			if ((bool)objNoControl)
			{
				objNoControl.SetActiveIfDifferent(true);
			}
			cvsChangeScene.gameObject.SetActive(true);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "Title",
				isFade = true
			}, false);
		});
		btnChangeHN.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			Scene.Data data2 = new Scene.Data
			{
				levelName = "RankingEntryScene",
				isAdd = true,
				isFade = false,
				isAsync = true,
				onLoad = delegate
				{
					RankingEntryScene rootComponent = Scene.GetRootComponent<RankingEntryScene>("RankingEntryScene");
					if (!(rootComponent == null))
					{
						rootComponent.backSceneName = "RankingScene";
					}
				}
			};
			Singleton<Scene>.Instance.LoadReserve(data2, true);
		});
		btnMyRanking.OnClickAsObservable().Subscribe(delegate
		{
			Toggle[] array2 = tglPage;
			foreach (Toggle toggle in array2)
			{
				toggle.isOn = false;
			}
			tglPage[myPageNo].isOn = true;
			RectTransform rectTransform = trfIndividualContants as RectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.y = myPosition;
			rectTransform.anchoredPosition = anchoredPosition;
		});
		if (srpHandleName == null)
		{
			return;
		}
		srpHandleName.SubscribeToText(txtHandleName);
		srpHandleName.Skip(1).Subscribe(delegate
		{
			ChangeItemInfo();
			Observable.FromCoroutine(UpdateHandleName).Subscribe(delegate
			{
			}, delegate
			{
				objNoControl.SetActiveIfDifferent(false);
			}, delegate
			{
				objNoControl.SetActiveIfDifferent(false);
			});
		});
	}
}
