using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using H;
using Illusion.CustomAttributes;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class HSceneResult : BaseLoader
{
	public class ResultStatus
	{
		public int favor;

		public int h;

		public int text = -1;
	}

	public bool isEnd;

	public int finishADV = -1;

	public SaveData.Player player;

	public List<SaveData.Heroine> lstHeroine = new List<SaveData.Heroine>();

	public HScene.AddParameter addParameter = new HScene.AddParameter();

	public SpriteChangeCtrl imageResult;

	public HResultGauge[] statusResult = new HResultGauge[2];

	public HResultGauge[] aibuResult = new HResultGauge[5];

	public HResultGauge[] sonyuResult = new HResultGauge[2];

	public HResultGauge houshiResult;

	public RawImage rawFace;

	public Image imageAddH;

	[SerializeField]
	private List<int> lstAibu = new List<int>();

	[SerializeField]
	private Dictionary<int, List<int>> dicHoushi = new Dictionary<int, List<int>>();

	[SerializeField]
	private Dictionary<int, List<int>> dicKokan = new Dictionary<int, List<int>>();

	[SerializeField]
	private Dictionary<int, List<int>> dicAnal = new Dictionary<int, List<int>>();

	[Label("恋人+部員のときのH値の減衰率")]
	public float HPointAttenuationRate = 0.5f;

	private int addFavor;

	private int addH;

	private int resultText;

	private int resultCount;

	private AudioSource audioGauge;

	private IEnumerator Start()
	{
		LoadResult();
		LoadResultAddTaii();
		SaveData.Heroine heroine = lstHeroine[0];
		if (heroine.relation == 0)
		{
			statusResult[0].SetStart(heroine.favor, addFavor);
			statusResult[1].SetImageVisible(false);
			statusResult[2].SetImageVisible(false);
		}
		else if (heroine.relation == 1)
		{
			statusResult[0].SetImageFillAmout(1f);
			statusResult[1].SetStart(heroine.favor, addFavor);
			statusResult[2].SetImageVisible(false);
		}
		else if (heroine.relation == 2)
		{
			statusResult[0].SetImageVisible(false);
			statusResult[1].SetImageFillAmout(1f);
			statusResult[2].SetStart(heroine.favor, addFavor);
		}
		statusResult[3].SetStart(heroine.lewdness, (float)addH * ((!heroine.isGirlfriend || !heroine.isStaff) ? 1f : HPointAttenuationRate));
		AdditionalFunctionsSystem addData = Manager.Config.AddData;
		int num = ((!addData.expH.isON) ? 1 : addData.expH.property);
		for (int i = 0; i < aibuResult.Length; i++)
		{
			aibuResult[i].SetStart(heroine.hAreaExps[i + 1], addParameter.aibus[i] * (float)num);
		}
		houshiResult.SetStart(heroine.houshiExp, addParameter.houshi * (float)num);
		sonyuResult[0].SetStart(heroine.countKokanH, addParameter.sonyus[0] * (float)num);
		sonyuResult[1].SetStart(heroine.countAnalH, addParameter.sonyus[1] * (float)num);
		imageResult.gameObject.SetActive(false);
		imageResult.OnChangeValue(resultText);
		base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.H_RESULT_UI).SafeGet(1).SafeProc(delegate(Dictionary<int, Data.Param> p)
		{
			Data.Param value;
			if (p.TryGetValue(resultText, out value))
			{
				Localize.Translate.Manager.Bind(imageResult.image, value, true);
			}
		});
		Texture2D texture2D = new Texture2D(240, 320);
		texture2D.LoadImage(heroine.charFile.facePngData);
		rawFace.texture = texture2D;
		rawFace.enabled = true;
		yield break;
	}

	private void Update()
	{
		if (Singleton<Scene>.Instance.IsNowLoadingFade || Singleton<Scene>.Instance.NowSceneNames[0] != "HSceneResult")
		{
			return;
		}
		bool flag = !statusResult.Any((HResultGauge r) => !r.IsEnd()) && houshiResult.IsEnd() && !aibuResult.Any((HResultGauge r) => !r.IsEnd()) && !sonyuResult.Any((HResultGauge r) => !r.IsEnd());
		bool flag2 = statusResult.Any((HResultGauge r) => r.IsStart()) || houshiResult.IsStart() || aibuResult.Any((HResultGauge r) => r.IsStart()) || sonyuResult.Any((HResultGauge r) => r.IsStart());
		if (audioGauge == null && flag2)
		{
			Utils.Sound.Play(SystemSE.result_gauge);
			audioGauge = Utils.Sound.Get(SystemSE.result_gauge);
			if ((bool)audioGauge)
			{
				audioGauge.loop = true;
			}
		}
		if (flag)
		{
			if (!imageResult.gameObject.activeSelf)
			{
				if ((bool)audioGauge)
				{
					audioGauge.Stop();
				}
				Utils.Sound.Play(SystemSE.result_end);
				imageResult.gameObject.SetActive(true);
				imageAddH.enabled = IsGetH(lstHeroine[0]);
			}
			if ((!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) || resultCount != 0)
			{
				return;
			}
			resultCount = 1;
			isEnd = true;
			SaveData.Heroine heroine = lstHeroine[0];
			AdditionalFunctionsSystem addData = Manager.Config.AddData;
			int num = ((!addData.expH.isON) ? 1 : addData.expH.property);
			for (int i = 1; i < heroine.hAreaExps.Length; i++)
			{
				heroine.hAreaExps[i] = Mathf.Min(heroine.hAreaExps[i] + addParameter.aibus[i - 1] * (float)num, 100f);
			}
			heroine.houshiExp = Mathf.Min(heroine.houshiExp + addParameter.houshi * (float)num, 100f);
			heroine.countKokanH = Mathf.Min(heroine.countKokanH + addParameter.sonyus[0] * (float)num, 100f);
			heroine.countAnalH = Mathf.Min(heroine.countAnalH + addParameter.sonyus[1] * (float)num, 100f);
			heroine.favor = Mathf.Clamp(heroine.favor + addFavor, 0, 100);
			heroine.lewdness = Mathf.Clamp(heroine.lewdness + addH, 0, 100);
			if (finishADV != 6 && finishADV != 7 && finishADV != 13)
			{
				List<int> list = new List<int>();
				list.Add(1);
				list.Add(1);
				list.Add(1);
				list.Add(1);
				list.Add(1);
				list.Add(1);
				list.Add(2);
				list.Add(2);
				list.Add(2);
				list.Add(3);
				List<int> source = list;
				player.hentai = Mathf.Clamp(player.hentai + source.OrderBy((int _) => Guid.NewGuid()).First(), 0, 100);
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if ((bool)actScene)
			{
				actScene.actCtrl.AddDesire(5, heroine, -30);
			}
			GetH(heroine);
			if ((bool)audioGauge)
			{
				audioGauge.Stop();
			}
		}
		else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !Singleton<Scene>.Instance.IsNowLoadingFade)
		{
			SaveData.Heroine heroine2 = lstHeroine[0];
			statusResult[(heroine2.relation == -1) ? 2 : heroine2.relation].SetFinish();
			statusResult[3].SetFinish();
			HResultGauge[] array = aibuResult;
			foreach (HResultGauge hResultGauge in array)
			{
				hResultGauge.SetFinish();
			}
			houshiResult.SetFinish();
			HResultGauge[] array2 = sonyuResult;
			foreach (HResultGauge hResultGauge2 in array2)
			{
				hResultGauge2.SetFinish();
			}
			if ((bool)audioGauge)
			{
				audioGauge.Stop();
			}
			Utils.Sound.Play(SystemSE.result_end);
		}
	}

	private void LoadResult()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "ResultText");
		if (text.IsNullOrEmpty())
		{
			return;
		}
		Dictionary<int, ResultStatus> dictionary = new Dictionary<int, ResultStatus>();
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		ResultStatus resultStatus = null;
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int key = int.Parse(data[i, num++]);
			resultStatus = null;
			if (!dictionary.TryGetValue(key, out resultStatus))
			{
				resultStatus = (dictionary[key] = new ResultStatus());
			}
			resultStatus.favor = UnityEngine.Random.Range(int.Parse(data[i, num++]), int.Parse(data[i, num++]));
			resultStatus.h = UnityEngine.Random.Range(int.Parse(data[i, num++]), int.Parse(data[i, num++]));
			resultStatus.text = int.Parse(data[i, num++]);
		}
		resultStatus = null;
		if (dictionary.TryGetValue(finishADV, out resultStatus))
		{
			addFavor = resultStatus.favor;
			addH = resultStatus.h;
			resultText = resultStatus.text;
		}
	}

	private void LoadResultAddTaii()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "HAddTaii");
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
			int num2 = int.Parse(data[i, num++]);
			int key = int.Parse(data[i, num++]);
			List<int> value = new List<int>();
			switch (num2)
			{
			case 0:
				value = lstAibu;
				break;
			case 1:
				if (!dicHoushi.TryGetValue(key, out value))
				{
					value = (dicHoushi[key] = new List<int>());
				}
				break;
			case 2:
				if (!dicKokan.TryGetValue(key, out value))
				{
					value = (dicKokan[key] = new List<int>());
				}
				break;
			default:
				if (!dicAnal.TryGetValue(key, out value))
				{
					value = (dicAnal[key] = new List<int>());
				}
				break;
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

	private bool IsGetH(SaveData.Heroine _heroine)
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> value;
		if (!clubContents.TryGetValue(1, out value))
		{
			value = (clubContents[1] = new HashSet<int>());
		}
		AdditionalFunctionsSystem addData = Manager.Config.AddData;
		int additionalRate = ((!addData.expH.isON) ? 1 : addData.expH.property);
		int num = _heroine.hAreaExps.Where((float v, int i) => i != 0).Select((float v, int i) => addParameter.aibus[i] * (float)additionalRate + v).Count((float v) => v >= 100f);
		for (int j = 0; j < num; j++)
		{
			if (!value.Contains(lstAibu[j]))
			{
				return true;
			}
		}
		foreach (KeyValuePair<int, List<int>> item in dicHoushi)
		{
			if (_heroine.houshiExp + addParameter.houshi * (float)additionalRate < (float)item.Key)
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
		foreach (KeyValuePair<int, List<int>> item3 in dicKokan)
		{
			if (_heroine.countKokanH + addParameter.sonyus[0] * (float)additionalRate < (float)item3.Key)
			{
				continue;
			}
			foreach (int item4 in item3.Value)
			{
				if (!value.Contains(item4))
				{
					return true;
				}
			}
		}
		foreach (KeyValuePair<int, List<int>> item5 in dicAnal)
		{
			if (_heroine.countAnalH + addParameter.sonyus[1] * (float)additionalRate < (float)item5.Key)
			{
				continue;
			}
			foreach (int item6 in item5.Value)
			{
				if (!value.Contains(item6))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void GetH(SaveData.Heroine _heroine)
	{
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> saveHas;
		if (!clubContents.TryGetValue(1, out saveHas))
		{
			clubContents[1] = (saveHas = new HashSet<int>());
		}
		int num = _heroine.hAreaExps.Where((float v, int i) => i != 0).Count((float v) => v == 100f);
		for (int j = 0; j < num; j++)
		{
			if (!saveHas.Contains(lstAibu[j]))
			{
				saveHas.Add(lstAibu[j]);
			}
		}
		foreach (KeyValuePair<int, List<int>> item in dicHoushi)
		{
			if (_heroine.houshiExp < (float)item.Key)
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
		foreach (KeyValuePair<int, List<int>> item2 in dicKokan)
		{
			if (_heroine.countKokanH < (float)item2.Key)
			{
				continue;
			}
			item2.Value.ForEach(delegate(int id)
			{
				if (!saveHas.Contains(id))
				{
					saveHas.Add(id);
				}
			});
		}
		foreach (KeyValuePair<int, List<int>> item3 in dicAnal)
		{
			if (_heroine.countAnalH < (float)item3.Key)
			{
				continue;
			}
			item3.Value.ForEach(delegate(int id)
			{
				if (!saveHas.Contains(id))
				{
					saveHas.Add(id);
				}
			});
		}
	}
}
