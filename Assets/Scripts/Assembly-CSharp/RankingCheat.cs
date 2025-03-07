using System;
using System.Collections;
using System.Linq;
using Illusion.Extensions;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RankingCheat : MonoBehaviour
{
	[SerializeField]
	private Transform trfContant;

	[SerializeField]
	private GameObject objTemp;

	[SerializeField]
	private Button btnAllReset;

	[SerializeField]
	private Button btnAllRandom;

	[SerializeField]
	private Button btnSave;

	[SerializeField]
	private Button btnCreateDummy001;

	[SerializeField]
	private Button btnCreateDummy010;

	[SerializeField]
	private Button btnCreateDummy100;

	[SerializeField]
	private GameObject objNoControl;

	[SerializeField]
	private GameObject objMessage;

	[SerializeField]
	private Text txtMessage;

	private string RankingURL = string.Empty;

	private float msgCount;

	private TMP_InputField[] inpScore;

	private Button[] btnRandom;

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

	private RankingData rankingDat
	{
		get
		{
			return Singleton<Game>.Instance.rankSaveData;
		}
	}

	public void EditRankingData(int no, int val)
	{
		rankingDat.ResetValue(no);
		switch (no)
		{
		case 0:
			rankingDat.peepToiletCount = (uint)val;
			break;
		case 1:
			rankingDat.peepShowerCount = (uint)val;
			break;
		case 2:
			rankingDat.peepOnanismCount = (uint)val;
			break;
		case 3:
			rankingDat.easyPlaceHCount = (uint)val;
			break;
		case 4:
			rankingDat.orgasmCount = (uint)val;
			break;
		case 5:
			rankingDat.inEjaculationCount = (uint)val;
			break;
		case 6:
			rankingDat.inEjaculationKokanNamaCount = (uint)val;
			break;
		case 7:
			rankingDat.inEjaculationAnalCount = (uint)val;
			break;
		case 8:
			rankingDat.outEjaculationCount = (uint)val;
			break;
		case 9:
			rankingDat.angerCount = (uint)val;
			break;
		case 10:
			rankingDat.dateCount = (uint)val;
			break;
		case 11:
			rankingDat.myRoomHCount = (uint)val;
			break;
		case 12:
			rankingDat.bustGauge = (uint)val;
			break;
		case 13:
			rankingDat.hipGauge = (uint)val;
			break;
		case 14:
			rankingDat.asokoGauge = (uint)val;
			break;
		case 15:
			rankingDat.adultToyGauge = (uint)val;
			break;
		case 16:
			rankingDat.staffCount = (uint)val;
			break;
		case 17:
			rankingDat.girlfriendCount = (uint)val;
			break;
		case 18:
			rankingDat.expNumCount = (uint)val;
			break;
		case 19:
			rankingDat.playTime = (uint)val;
			break;
		case 20:
			rankingDat.angerLockerCount = (uint)val;
			break;
		case 21:
			rankingDat.angerToiletCount = (uint)val;
			break;
		case 22:
			rankingDat.angerShowerCount = (uint)val;
			break;
		case 23:
			rankingDat.physicalCount = (uint)val;
			break;
		case 24:
			rankingDat.intellectCount = (uint)val;
			break;
		case 25:
			rankingDat.hentaiCount = (uint)val;
			break;
		}
	}

	public string GetRankingValue(int no)
	{
		switch (no)
		{
		case 0:
			return rankingDat.peepToiletCount.ToString();
		case 1:
			return rankingDat.peepShowerCount.ToString();
		case 2:
			return rankingDat.peepOnanismCount.ToString();
		case 3:
			return rankingDat.easyPlaceHCount.ToString();
		case 4:
			return rankingDat.orgasmCount.ToString();
		case 5:
			return rankingDat.inEjaculationCount.ToString();
		case 6:
			return rankingDat.inEjaculationKokanNamaCount.ToString();
		case 7:
			return rankingDat.inEjaculationAnalCount.ToString();
		case 8:
			return rankingDat.outEjaculationCount.ToString();
		case 9:
			return rankingDat.angerCount.ToString();
		case 10:
			return rankingDat.dateCount.ToString();
		case 11:
			return rankingDat.myRoomHCount.ToString();
		case 12:
			return rankingDat.bustGauge.ToString();
		case 13:
			return rankingDat.hipGauge.ToString();
		case 14:
			return rankingDat.asokoGauge.ToString();
		case 15:
			return rankingDat.adultToyGauge.ToString();
		case 16:
			return rankingDat.staffCount.ToString();
		case 17:
			return rankingDat.girlfriendCount.ToString();
		case 18:
			return rankingDat.expNumCount.ToString();
		case 19:
			return rankingDat.playTime.ToString();
		case 20:
			return rankingDat.angerLockerCount.ToString();
		case 21:
			return rankingDat.angerToiletCount.ToString();
		case 22:
			return rankingDat.angerShowerCount.ToString();
		case 23:
			return rankingDat.physicalCount.ToString();
		case 24:
			return rankingDat.intellectCount.ToString();
		case 25:
			return rankingDat.hentaiCount.ToString();
		default:
			return string.Empty;
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

	private IEnumerator DebugEntryData(int fig)
	{
		SetMessage("ダミーデータを作成しています", -1f);
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 255);
		wwwform.AddField("fig", fig);
		ObservableYieldInstruction<string> o = ObservableWWW.Post(RankingURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
		yield return o;
		if (o.HasError)
		{
			SetMessage("ダミーデータを作成に失敗しました", 3f);
			objNoControl.SetActiveIfDifferent(false);
			yield break;
		}
		if (o.HasResult)
		{
			DisvisibleMessage();
		}
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(false);
		}
		yield return null;
	}

	private void Start()
	{
		RankingURL = CreateURL.Load_KK_Ranking_URL();
		inpScore = new TMP_InputField[rankingItemName.Length];
		btnRandom = new Button[rankingItemName.Length];
		for (int i = 0; i < rankingItemName.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(objTemp, trfContant, false);
			TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
			componentInChildren.text = rankingItemName[i];
			inpScore[i] = gameObject.GetComponentInChildren<TMP_InputField>(true);
			inpScore[i].text = GetRankingValue(i);
			btnRandom[i] = gameObject.GetComponentInChildren<Button>(true);
			gameObject.SetActive(true);
		}
		Transform[] array = (from x in btnRandom.Select((Button val, int idx) => new
			{
				value = val.transform,
				idx = rankingSiblingNo[idx]
			})
			orderby x.idx
			select x into n
			select n.value).ToArray();
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetSiblingIndex(j);
		}
		inpScore.Select((TMP_InputField inp, int idx) => new { inp, idx }).ToList().ForEach(item =>
		{
			item.inp.onValueChanged.AddListener(delegate(string val)
			{
				int result = 0;
				int.TryParse(val, out result);
				EditRankingData(item.idx, result);
			});
		});
		btnRandom.Select((Button btn, int idx) => new { btn, idx }).ToList().ForEach(item =>
		{
			item.btn.OnClickAsObservable().Subscribe(delegate
			{
				int val2 = UnityEngine.Random.Range(0, 999);
				inpScore[item.idx].text = val2.ToString();
				EditRankingData(item.idx, val2);
			});
		});
		btnAllReset.OnClickAsObservable().Subscribe(delegate
		{
			for (int k = 0; k < 26; k++)
			{
				inpScore[k].text = "0";
				EditRankingData(k, 0);
			}
		});
		btnAllRandom.OnClickAsObservable().Subscribe(delegate
		{
			for (int l = 0; l < 26; l++)
			{
				int val3 = UnityEngine.Random.Range(0, 999);
				inpScore[l].text = val3.ToString();
				EditRankingData(l, val3);
			}
		});
		btnSave.OnClickAsObservable().Subscribe(delegate
		{
			rankingDat.Save();
		});
		btnCreateDummy001.OnClickAsObservable().Subscribe(delegate
		{
			StartCoroutine(DebugEntryData(1));
		});
		btnCreateDummy010.OnClickAsObservable().Subscribe(delegate
		{
			StartCoroutine(DebugEntryData(10));
		});
		btnCreateDummy100.OnClickAsObservable().Subscribe(delegate
		{
			StartCoroutine(DebugEntryData(100));
		});
	}
}
