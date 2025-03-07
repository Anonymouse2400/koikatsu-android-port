using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H;
using Illusion.CustomAttributes;
using Illusion.Game;
using Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClubReport : BaseLoader
{
	private class HInfo
	{
		public int id;

		public bool isRelease;
	}

	[SerializeField]
	[Label("勧誘した人数ポイントText")]
	private TextMeshProUGUI txtAddMemberPoint;

	[Label("ふれあいポイント小計Text")]
	[SerializeField]
	private TextMeshProUGUI txtContactPoint;

	[SerializeField]
	[Label("エッチポイント小計Text")]
	private TextMeshProUGUI txtHPoint;

	[Label("ランクマーク")]
	[SerializeField]
	private SpriteChangeCtrl mark;

	[Label("経験値バー")]
	[SerializeField]
	private Image imgExpBar;

	[SerializeField]
	[Label("経験値Text")]
	private TextMeshProUGUI txtExp;

	[Label("体位の追加Image")]
	[SerializeField]
	private Image imgAddH;

	[SerializeField]
	[Label("アイテムの追加Image")]
	private Image imgAddItem;

	[Label("体位親オブジェクト")]
	[SerializeField]
	private GameObject objTaii;

	[SerializeField]
	[Label("H進行度テキスト")]
	private TextMeshProUGUI txtH;

	[SerializeField]
	[Label("マップ進行度テキスト")]
	private TextMeshProUGUI txtMap;

	[Label("シナリオ進行度テキスト")]
	[SerializeField]
	private TextMeshProUGUI txtScenario;

	[SerializeField]
	[Label("H進行度テキスト")]
	private Image imgH;

	[Label("マップ進行度テキスト")]
	[SerializeField]
	private Image imgMap;

	[SerializeField]
	[Label("シナリオ進行度テキスト")]
	private Image imgScenario;

	[SerializeField]
	[Label("ゲージUPの時間")]
	private float timeGauge = 5f;

	[Label("ランクB")]
	[SerializeField]
	private int rankBPoint = 2000;

	[SerializeField]
	[Label("ランクA")]
	private int rankAPoint = 5000;

	[Label("ランクS")]
	[SerializeField]
	private int rankSPoint = 10000;

	[SerializeField]
	[Header("")]
	private List<int> lstH = new List<int>();

	[SerializeField]
	private List<int> lstMap = new List<int> { 1000, 1012, 1013, 1016, 1030, 1032, 1035, 1044 };

	[SerializeField]
	private List<int> lstScenario = new List<int> { 1002, 1028, 1036, 2004 };

	private Dictionary<int, List<int>> dicClubAdd = new Dictionary<int, List<int>>();

	[SerializeField]
	[DisabledGroup("")]
	private bool isEnd;

	private List<HInfo>[] hInfo = new List<HInfo>[3];

	private int[] hBaseCounts = new int[3];

	private IntReactiveProperty nowPoint = new IntReactiveProperty();

	private int oldPoint;

	private int addPoint;

	private Coroutine cor;

	private AudioSource audioGauge;

	private IEnumerator Start()
	{
		if (!Singleton<Game>.Instance.glSaveData.tutorialHash.Contains(6))
		{
			Utils.Scene.OpenTutorial(6);
			yield return new WaitWhile(Utils.Scene.IsTutorial);
		}
		CreateAllAnimationList();
		LoadResultAddTaii();
		LoadHResultAddTaii();
		SaveData.ClubReport report = Singleton<Game>.Instance.saveData.clubReport;
		addPoint = report.staffAdd * 400;
		txtAddMemberPoint.text = ((report.staffAdd == 0) ? "0" : addPoint.ToString());
		txtAddMemberPoint.enabled = false;
		txtContactPoint.text = ((report.comAdd == 0) ? "0" : report.comAdd.ToString());
		txtContactPoint.enabled = false;
		int hAdd = (int)report.hAdd;
		txtHPoint.text = ((hAdd == 0) ? "0" : hAdd.ToString());
		txtHPoint.enabled = false;
		addPoint += report.comAdd + (int)report.hAdd;
		nowPoint.Value = report.point;
		nowPoint.Subscribe(delegate(int p)
		{
			PointProc(p);
		});
		SetRemaining();
		oldPoint = nowPoint.Value;
		cor = StartCoroutine(ReprotProc());
		yield return null;
	}

	private void Update()
	{
		if (!Utils.Scene.IsTutorial() && !(Singleton<Scene>.Instance.NowSceneNames[0] != "ClubReport") && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
		{
			if (isEnd)
			{
				Singleton<Scene>.Instance.UnLoad();
			}
			else
			{
				Stop();
			}
		}
	}

	private int GetRank(int _point)
	{
		if (_point >= rankSPoint)
		{
			return 3;
		}
		if (_point >= rankAPoint)
		{
			return 2;
		}
		if (_point >= rankBPoint)
		{
			return 1;
		}
		return 0;
	}

	private void PointProc(int _point)
	{
		int rank = GetRank(_point);
		mark.OnChangeValue(rank);
		float num = 1f;
		switch (rank)
		{
		case 0:
			num = Mathf.InverseLerp(0f, rankBPoint, _point);
			break;
		case 1:
			num = Mathf.InverseLerp(rankBPoint, rankAPoint, _point);
			break;
		case 2:
			num = Mathf.InverseLerp(rankAPoint, rankSPoint, _point);
			break;
		case 3:
			num = 1f;
			break;
		}
		imgExpBar.fillAmount = num;
		txtExp.text = (int)(num * 100f)/*cast due to .constrained prefix*/ + "%";
	}

	private bool IsAddH(int _point)
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> value;
		if (!clubContents.TryGetValue(1, out value))
		{
			value = (clubContents[1] = new HashSet<int>());
		}
		int rank = GetRank(_point);
		foreach (KeyValuePair<int, List<int>> item in dicClubAdd)
		{
			if (rank < item.Key)
			{
				continue;
			}
			foreach (int item2 in item.Value)
			{
				if (!value.Contains(item2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AddH(int _point)
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> saveHas;
		if (!clubContents.TryGetValue(1, out saveHas))
		{
			clubContents[1] = (saveHas = new HashSet<int>());
		}
		int rank = GetRank(_point);
		foreach (KeyValuePair<int, List<int>> item in dicClubAdd)
		{
			if (rank < item.Key)
			{
				continue;
			}
			item.Value.ForEach(delegate(int id)
			{
				if (!saveHas.Contains(id))
				{
					saveHas.Add(id);
				}
			});
		}
	}

	private bool IsAddItem(int _point)
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> value;
		if (!clubContents.TryGetValue(0, out value))
		{
			value = (clubContents[0] = new HashSet<int>());
		}
		int rank = GetRank(_point);
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		dictionary.Add(0, new List<int>());
		dictionary.Add(1, new List<int> { 0 });
		dictionary.Add(2, new List<int> { 1 });
		dictionary.Add(3, new List<int>());
		Dictionary<int, List<int>> dictionary2 = dictionary;
		foreach (KeyValuePair<int, List<int>> item in dictionary2)
		{
			if (rank < item.Key)
			{
				continue;
			}
			foreach (int item2 in item.Value)
			{
				if (!value.Contains(item2))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AddItem(int _point)
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> saveHas;
		if (!clubContents.TryGetValue(0, out saveHas))
		{
			clubContents[0] = (saveHas = new HashSet<int>());
		}
		int rank = GetRank(_point);
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		dictionary.Add(0, new List<int>());
		dictionary.Add(1, new List<int> { 0 });
		dictionary.Add(2, new List<int> { 1 });
		dictionary.Add(3, new List<int>());
		Dictionary<int, List<int>> dictionary2 = dictionary;
		foreach (KeyValuePair<int, List<int>> item in dictionary2)
		{
			if (rank < item.Key)
			{
				continue;
			}
			item.Value.ForEach(delegate(int id)
			{
				if (!saveHas.Contains(id))
				{
					saveHas.Add(id);
				}
			});
		}
	}

	private IEnumerator ReprotProc()
	{
		yield return new WaitUntil(() => !Singleton<Scene>.Instance.IsNowLoadingFade);
		yield return new WaitForSeconds(2f);
		txtAddMemberPoint.enabled = true;
		Utils.Sound.Play(SystemSE.result_single);
		yield return new WaitForSeconds(2f);
		txtContactPoint.enabled = true;
		Utils.Sound.Play(SystemSE.result_single);
		yield return new WaitForSeconds(2f);
		txtHPoint.enabled = true;
		Utils.Sound.Play(SystemSE.result_single);
		yield return new WaitForSeconds(2f);
		Utils.Sound.Play(SystemSE.result_gauge);
		audioGauge = Utils.Sound.Get(SystemSE.result_gauge);
		if ((bool)audioGauge)
		{
			audioGauge.loop = true;
		}
		if (addPoint != 0 && nowPoint.Value < rankSPoint)
		{
			int now = nowPoint.Value;
			int end = Mathf.Min(rankSPoint, now + addPoint);
			float time = 0f;
			while (true)
			{
				time = Mathf.Min(time + Time.deltaTime, timeGauge);
				float t = time / timeGauge;
				nowPoint.Value = (int)Mathf.Lerp(now, end, t);
				if (t >= 1f)
				{
					break;
				}
				yield return null;
			}
		}
		if ((bool)audioGauge)
		{
			audioGauge.Stop();
		}
		Utils.Sound.Play(SystemSE.result_end);
		imgAddH.enabled = IsAddH(nowPoint.Value);
		AddH(nowPoint.Value);
		imgAddItem.enabled = IsAddItem(nowPoint.Value);
		AddItem(nowPoint.Value);
		SaveData.ClubReport report = Singleton<Game>.Instance.saveData.clubReport;
		report.ResetAddPoint();
		report.point = addPoint;
		isEnd = true;
		yield return null;
	}

	private void Stop()
	{
		StopCoroutine(cor);
		txtAddMemberPoint.enabled = true;
		txtContactPoint.enabled = true;
		txtHPoint.enabled = true;
		nowPoint.Value = oldPoint + addPoint;
		imgAddH.enabled = IsAddH(nowPoint.Value);
		AddH(nowPoint.Value);
		imgAddItem.enabled = IsAddItem(nowPoint.Value);
		AddItem(nowPoint.Value);
		SaveData.ClubReport clubReport = Singleton<Game>.Instance.saveData.clubReport;
		clubReport.ResetAddPoint();
		clubReport.point = addPoint;
		if ((bool)audioGauge)
		{
			audioGauge.Stop();
		}
		Utils.Sound.Play(SystemSE.result_end);
		isEnd = true;
	}

	private void SetRemaining()
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> saveHas;
		if (!clubContents.TryGetValue(1, out saveHas))
		{
			clubContents[1] = (saveHas = new HashSet<int>());
		}
		int[] addTaiis = new int[3];
		lstH.ForEach(delegate(int i)
		{
			if (saveHas.Contains(i))
			{
				addTaiis[0]++;
			}
		});
		lstMap.ForEach(delegate(int i)
		{
			if (saveHas.Contains(i))
			{
				addTaiis[1]++;
			}
		});
		lstScenario.ForEach(delegate(int i)
		{
			if (saveHas.Contains(i))
			{
				addTaiis[2]++;
			}
		});
		float num = (float)addTaiis[0] / (float)lstH.Count;
		float num2 = (float)addTaiis[1] / (float)lstMap.Count;
		float num3 = (float)addTaiis[2] / (float)lstScenario.Count;
		imgH.fillAmount = num;
		imgMap.fillAmount = num2;
		imgScenario.fillAmount = num3;
		txtH.text = (num * 100f).ToString("000");
		txtMap.text = (num2 * 100f).ToString("000");
		txtScenario.text = (num3 * 100f).ToString("000");
	}

	private void CreateAllAnimationList()
	{
		for (int i = 0; i < this.hInfo.Length; i++)
		{
			string text = GlobalMethod.LoadAllListText("h/list/", "AnimationInfo_" + i.ToString("00"));
			this.hInfo[i] = new List<HInfo>();
			if (text.IsNullOrEmpty())
			{
				continue;
			}
			string[,] data;
			GlobalMethod.GetListString(text, out data);
			int length = data.GetLength(0);
			for (int j = 0; j < length; j++)
			{
				int num = 0;
				int id = 0;
				int.TryParse(data[j, 1], out id);
				HInfo hInfo = this.hInfo[i].Find((HInfo l) => l.id == id);
				if (hInfo == null)
				{
					this.hInfo[i].Add(new HInfo());
					hInfo = this.hInfo[i][this.hInfo[i].Count - 1];
				}
				num++;
				num++;
				hInfo.id = id;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				num++;
				hInfo.isRelease = data[j, num++] == "1";
				num++;
			}
		}
		for (int k = 0; k < this.hInfo.Length; k++)
		{
			hBaseCounts[k] = this.hInfo[k].Count((HInfo info) => !info.isRelease);
		}
	}

	private void LoadResultAddTaii()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "ClubReportAddTaii");
		if (text.IsNullOrEmpty())
		{
			return;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int key = int.Parse(data[i, num++]);
			List<int> value = new List<int>();
			if (!dicClubAdd.TryGetValue(key, out value))
			{
				value = (dicClubAdd[key] = new List<int>());
			}
			value.Clear();
			while (length2 > num)
			{
				string text2 = data[i, num++];
				if (text2.IsNullOrEmpty())
				{
					break;
				}
				value.Add(int.Parse(text2));
			}
		}
	}

	private void LoadHResultAddTaii()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "HAddTaii");
		if (text.IsNullOrEmpty())
		{
			return;
		}
		Dictionary<int, List<int>>[] array = new Dictionary<int, List<int>>[5];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Dictionary<int, List<int>>();
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int j = 0; j < length; j++)
		{
			int num = 0;
			int num2 = int.Parse(data[j, num++]);
			int key = int.Parse(data[j, num++]);
			if (!array[num2].ContainsKey(key))
			{
				array[num2].Add(key, new List<int>());
			}
			List<int> list = array[num2][key];
			list.Clear();
			while (length2 > num)
			{
				string text2 = data[j, num++];
				if (text2.IsNullOrEmpty())
				{
					break;
				}
				list.Add(int.Parse(text2));
			}
		}
		Dictionary<int, List<int>>[] array2 = array;
		foreach (Dictionary<int, List<int>> dictionary in array2)
		{
			foreach (List<int> value in dictionary.Values)
			{
				lstH.AddRange(value);
			}
		}
	}
}
