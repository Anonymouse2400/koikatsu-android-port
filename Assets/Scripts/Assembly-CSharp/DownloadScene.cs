using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Illusion.Extensions;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class DownloadScene : BaseLoader
{
	[Serializable]
	public class DLDataHeader
	{
		public int PID;

		public int PersonalType;

		public int BustSize;

		public int HairType;

		public int Sex;

		public int Height;

		public int BloodType;

		public string BirthDay = string.Empty;

		public int ClubActivity;

		public string HandleName = string.Empty;

		public string Comment = string.Empty;

		public string CharaName = string.Empty;

		public string NickName = string.Empty;

		public string ClubActivityName = string.Empty;

		public string PersonalTypeName = string.Empty;

		public string UserID = string.Empty;

		public int DLCount;

		public int WeekCount;
	}

	public class RankData
	{
		public int type;

		public Image imgBack;

		public Image[] imgNum = new Image[6];

		public Image[] imgTop3 = new Image[3];

		public void Setup(GameObject _obj)
		{
			imgBack = _obj.GetComponent<Image>();
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject = _obj.transform.FindLoop("No" + i.ToString("00"));
				if (!(null == gameObject))
				{
					imgTop3[i] = gameObject.GetComponent<Image>();
				}
			}
			for (int j = 0; j < 6; j++)
			{
				GameObject gameObject2 = _obj.transform.FindLoop("num" + j.ToString("00"));
				if (!(null == gameObject2))
				{
					imgNum[j] = gameObject2.GetComponent<Image>();
				}
			}
		}

		public void Disable()
		{
			if ((bool)imgBack)
			{
				imgBack.enabled = false;
			}
			Image[] array = imgNum;
			foreach (Image image in array)
			{
				image.enabled = false;
			}
			if ((bool)imgTop3[0])
			{
				imgTop3[0].enabled = false;
			}
			if ((bool)imgTop3[1])
			{
				imgTop3[1].enabled = false;
			}
			if ((bool)imgTop3[2])
			{
				imgTop3[2].enabled = false;
			}
		}

		public void Update(int value, Image[] imgNR)
		{
			if ((bool)imgBack)
			{
				imgBack.enabled = false;
			}
			Image[] array = imgNum;
			foreach (Image image in array)
			{
				image.enabled = false;
			}
			if ((bool)imgTop3[0])
			{
				imgTop3[0].enabled = false;
			}
			if ((bool)imgTop3[1])
			{
				imgTop3[1].enabled = false;
			}
			if ((bool)imgTop3[2])
			{
				imgTop3[2].enabled = false;
			}
			switch (value)
			{
			case 1:
				if ((bool)imgTop3[0])
				{
					imgTop3[0].enabled = true;
				}
				return;
			case 2:
				if ((bool)imgTop3[1])
				{
					imgTop3[1].enabled = true;
				}
				return;
			case 3:
				if ((bool)imgTop3[2])
				{
					imgTop3[2].enabled = true;
				}
				return;
			}
			int[] array2 = new int[6]
			{
				value / 100000,
				value % 100000 / 10000,
				value % 10000 / 1000,
				value % 1000 / 100,
				value % 100 / 10,
				value % 10
			};
			if ((bool)imgBack)
			{
				imgBack.enabled = true;
			}
			for (int j = 0; j < 6; j++)
			{
				imgNum[j].enabled = true;
				imgNum[j].sprite = imgNR[array2[j]].sprite;
			}
		}
	}

	public class CacheHeader
	{
		public int id;

		public long pos;

		public int size;
	}

	public readonly string CacheFileMark = "CharaThumbnail";

	public readonly int CacheFileVersion = 100;

	public string UploaderURL = string.Empty;

	private const int TMBS_FIG = 21;

	private const int TMBL_FIG = 10;

	[SerializeField]
	private Canvas cvsChangeScene;

	[SerializeField]
	private PopupCheck popupCheck;

	[SerializeField]
	private GameObject objNoControl;

	[HideInInspector]
	public bool checkMode;

	[HideInInspector]
	public bool noUpdate;

	public Toggle[] tglSex;

	public GameObject objFrameM;

	public GameObject objFrameF;

	public GameObject objThumbnailS;

	public GameObject objThumbnailL;

	public GameObject objRankingS;

	public GameObject objRankingL;

	public GameObject objSearchFemaleOnly;

	public RectTransform rtfSearchRequestRect;

	public GameObject ojbSearchRequestSBar;

	public ScrollRect srSearchRequest;

	public Image[] imgNumP;

	public Image[] imgNumRS;

	public Image[] imgNumRL;

	public Image[] imgPNum;

	public Image[] imgPMax;

	public Button btnDownload;

	public TextMeshProUGUI textDownload;

	public Button btnDelete;

	public TextMeshProUGUI textDelete;

	public Button btnTitle;

	public Button btnUpload;

	public Toggle[] tglHeight;

	public Toggle[] tglBust;

	public Toggle[] tglHair;

	public GameObject objClubTmp;

	public Toggle[] tglClub;

	public GameObject objPersonalityTmp;

	public Toggle[] tglPersonality;

	private Dictionary<int, int> dictClub = new Dictionary<int, int>();

	private Dictionary<int, int> dictPersonality = new Dictionary<int, int>();

	private bool searchMyChara;

	private bool drawRanking;

	private int modeSex = 1;

	private int modeThumbSize;

	private int maxPage;

	private int nowPage;

	private int selChara = -1;

	private int sortType = 1;

	private int sortRankType;

	private Image[] imgTmbSmall = new Image[21];

	private Image[] imgTmbLarge = new Image[10];

	private Button[] btnTmbSmall = new Button[21];

	private Button[] btnTmbLarge = new Button[10];

	private Image[] imgSelSmall = new Image[21];

	private Image[] imgSelLarge = new Image[10];

	private RankData[] dataRankS = new RankData[21];

	private RankData[] dataRankL = new RankData[10];

	private List<DLDataHeader> lstAll = new List<DLDataHeader>();

	private List<DLDataHeader> lstSearch = new List<DLDataHeader>();

	public GameObject objMessage;

	public Text txtMessage;

	private float msgCount;

	private bool updateTmbFlag;

	private int modeGetInfoList;

	private int modeGetInfoData;

	private float corGetInfoStartTime;

	private IEnumerator corGetInfo;

	private int DLThumbNum;

	private int modeDLCount;

	private float corDLCntStartTime;

	private IEnumerator corDLCnt;

	private Dictionary<int, bool> lstDownloadPID = new Dictionary<int, bool>();

	public string fileCacheSetting = "setting.dat";

	private string pathCacheDir = string.Empty;

	private bool enableCache = true;

	public Toggle tglEnableCache;

	private Dictionary<string, List<CacheHeader>> dictCacheHeaderInfo = new Dictionary<string, List<CacheHeader>>();

	[SerializeField]
	private Button btnClearCache;

	[SerializeField]
	private DownloadCharaInfoViewer dlCharaInfoViewer;

	[SerializeField]
	private GameObject popularityType;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private Dictionary<int, Data.Param> translateCharaInfo
	{
		get
		{
			return uiTranslater.Get(1);
		}
	}

	private Dictionary<int, Data.Param> translateQuestionTitle
	{
		get
		{
			return uiTranslater.Get(3);
		}
	}

	private Dictionary<int, Data.Param> translateMessageTitle
	{
		get
		{
			return uiTranslater.Get(4);
		}
	}

	private Dictionary<int, Data.Param> translateOther
	{
		get
		{
			return uiTranslater.Get(5);
		}
	}

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.DOWNLOAD));
		}
	}

	private void Start()
	{
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		if (popularityType != null)
		{
			popularityType.UpdateAsObservable().Take(1).DelayFrame(1)
				.Subscribe(delegate
				{
					popularityType.SetActive(false);
					popularityType.SetActive(true);
				});
		}
		string path = UserData.Path;
		path += "thumb_sv/";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		pathCacheDir = path;
		UploaderURL = CreateURL.Load_KK_Cha_URL();
		string path2 = pathCacheDir + fileCacheSetting;
		if (!File.Exists(path2))
		{
			SaveCacheSetting();
		}
		else
		{
			LoadCacheSetting();
		}
		tglEnableCache.isOn = enableCache;
		UpdateCacheHeaderInfo();
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		gameObject = objThumbnailS.transform.FindLoop("thumbTop");
		if ((bool)gameObject)
		{
			for (int i = 0; i < 21; i++)
			{
				gameObject2 = gameObject.transform.FindLoop("thumb" + i.ToString("00"));
				imgTmbSmall[i] = gameObject2.GetComponent<Image>();
				btnTmbSmall[i] = gameObject2.GetComponent<Button>();
				SetButtonClickHandler(btnTmbSmall[i], i);
			}
		}
		gameObject = objThumbnailL.transform.FindLoop("thumbTop");
		if ((bool)gameObject)
		{
			for (int j = 0; j < 10; j++)
			{
				gameObject2 = gameObject.transform.FindLoop("thumb" + j.ToString("00"));
				imgTmbLarge[j] = gameObject2.GetComponent<Image>();
				btnTmbLarge[j] = gameObject2.GetComponent<Button>();
				SetButtonClickHandler(btnTmbLarge[j], j);
			}
		}
		GameObject gameObject3 = null;
		gameObject3 = objThumbnailS.transform.FindLoop("selectTop");
		if ((bool)gameObject3)
		{
			for (int k = 0; k < 21; k++)
			{
				gameObject2 = gameObject3.transform.FindLoop("select" + k.ToString("00"));
				imgSelSmall[k] = gameObject2.GetComponent<Image>();
			}
		}
		gameObject3 = objThumbnailL.transform.FindLoop("selectTop");
		if ((bool)gameObject3)
		{
			for (int l = 0; l < 10; l++)
			{
				gameObject2 = gameObject3.transform.FindLoop("select" + l.ToString("00"));
				imgSelLarge[l] = gameObject2.GetComponent<Image>();
			}
		}
		if ((bool)objRankingS)
		{
			for (int m = 0; m < 21; m++)
			{
				GameObject obj = objRankingS.transform.FindLoop("rank" + m.ToString("00"));
				dataRankS[m] = new RankData();
				dataRankS[m].Setup(obj);
			}
		}
		if ((bool)objRankingL)
		{
			for (int n = 0; n < 10; n++)
			{
				GameObject obj2 = objRankingL.transform.FindLoop("rank" + n.ToString("00"));
				dataRankL[n] = new RankData();
				dataRankL[n].Setup(obj2);
			}
		}
		ClubInfo.Param[] array = Game.ClubInfos.Values.ToArray();
		tglClub = new Toggle[array.Length];
		int num = 0;
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(objClubTmp);
			tglClub[num2] = gameObject4.GetComponent<Toggle>();
			Transform transform = gameObject4.transform.Find("Label");
			if ((bool)transform)
			{
				TextMeshProUGUI component = transform.GetComponent<TextMeshProUGUI>();
				if ((bool)component)
				{
					component.text = array[num2].Name;
				}
			}
			gameObject4.transform.SetParent(objClubTmp.transform.parent, false);
			gameObject4.SetActiveIfDifferent(true);
			dictClub[num] = array[num2].ID;
			num++;
		}
		VoiceInfo.Param[] array2 = Singleton<Voice>.Instance.voiceInfoDic.Values.Where((VoiceInfo.Param x) => 0 <= x.No).ToArray();
		tglPersonality = new Toggle[array2.Length];
		num = 0;
		for (int num3 = 0; num3 < array2.Length; num3++)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(objPersonalityTmp);
			tglPersonality[num3] = gameObject5.GetComponent<Toggle>();
			Transform transform2 = gameObject5.transform.Find("Label");
			if ((bool)transform2)
			{
				TextMeshProUGUI component2 = transform2.GetComponent<TextMeshProUGUI>();
				if ((bool)component2)
				{
					component2.text = array2[num3].Personality;
				}
			}
			gameObject5.transform.SetParent(objPersonalityTmp.transform.parent, false);
			gameObject5.SetActiveIfDifferent(true);
			dictPersonality[num] = array2[num3].No;
			num++;
		}
		tglSex.Select((Toggle p, int idx) => new
		{
			toggle = p,
			index = (byte)idx
		}).ToList().ForEach(p =>
		{
			(from isOn in p.toggle.OnValueChangedAsObservable()
				where isOn
				select isOn).Subscribe(delegate
			{
				objFrameM.SetActiveIfDifferent(0 == p.index);
				objFrameF.SetActiveIfDifferent(1 == p.index);
				OnChangeModeSex(1 == p.index);
			});
		});
		btnDelete.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			Observable.FromCoroutine(DeleteMyUpload).Subscribe(delegate
			{
			}, delegate
			{
			}, delegate
			{
			});
		});
		btnClearCache.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.window_o);
			Observable.FromCoroutine(ClearCache).Subscribe(delegate
			{
			}, delegate
			{
			}, delegate
			{
			});
		});
		btnUpload.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			cvsChangeScene.gameObject.SetActive(true);
			Scene.Data data = new Scene.Data
			{
				levelName = "NetworkCheckScene",
				isAdd = false,
				isFade = true,
				isAsync = true,
				onLoad = delegate
				{
					NetworkCheckScene rootComponent = Scene.GetRootComponent<NetworkCheckScene>("NetworkCheckScene");
					if (!(rootComponent == null))
					{
						rootComponent.nextSceneName = "Uploader";
					}
				}
			};
			Singleton<Scene>.Instance.LoadReserve(data, true);
		});
		btnTitle.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			cvsChangeScene.gameObject.SetActive(true);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "Title",
				isFade = true
			}, false);
		});
		btnDownload.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			corDLCntStartTime = Time.realtimeSinceStartup;
			corDLCnt = DLCountUpdate(lstSearch[selChara].PID);
			StartCoroutine(corDLCnt);
		});
		corGetInfoStartTime = Time.realtimeSinceStartup;
		corGetInfo = GetListAll();
		StartCoroutine(corGetInfo);
	}

	private void SetMessage(string msg, float time)
	{
		msgCount = time;
		txtMessage.text = msg;
		objMessage.SetActiveIfDifferent(true);
	}

	private void UpdateMessage()
	{
		if (0f < msgCount)
		{
			msgCount -= Time.deltaTime;
			msgCount = Mathf.Max(0f, msgCount);
			if (msgCount == 0f)
			{
				DisvisibleMessage();
			}
		}
	}

	private void DisvisibleMessage()
	{
		msgCount = 0f;
		txtMessage.text = string.Empty;
		objMessage.SetActiveIfDifferent(false);
	}

	public void OnChangeModeSex(bool value)
	{
		int num = (value ? 1 : 0);
		if (modeSex != num)
		{
			modeSex = num;
			if ((bool)objSearchFemaleOnly)
			{
				objSearchFemaleOnly.SetActiveIfDifferent(value);
			}
			if ((bool)ojbSearchRequestSBar)
			{
				ojbSearchRequestSBar.SetActiveIfDifferent(value);
			}
			if ((bool)srSearchRequest)
			{
				srSearchRequest.enabled = value;
			}
			if (rtfSearchRequestRect != null)
			{
				Vector2 anchoredPosition = rtfSearchRequestRect.anchoredPosition;
				anchoredPosition.y = 0f;
				rtfSearchRequestRect.anchoredPosition = anchoredPosition;
			}
			if (dlCharaInfoViewer != null)
			{
				dlCharaInfoViewer.isFemale = value;
			}
			DisvisibleRanking();
			UpdateSearch();
		}
	}

	public void OnChangeSearchMyChara(bool flags)
	{
		searchMyChara = flags;
		DisvisibleRanking();
		UpdateSearch();
	}

	public void OnChangeDrawRanking(bool flags)
	{
		drawRanking = flags;
		UpdateRanking();
	}

	public void OnChangeThumbSize(bool flags)
	{
		modeThumbSize = ((!flags) ? 1 : 0);
		selChara = -1;
		UpdateSelectCharaInfo();
		if ((bool)objThumbnailL)
		{
			objThumbnailL.SetActiveIfDifferent(!flags);
		}
		if ((bool)objThumbnailS)
		{
			objThumbnailS.SetActiveIfDifferent(flags);
		}
		UpdatePageMax();
		DisvisibleRanking();
		UpdatePage();
	}

	public void OnChangeSortType(UI_Parameter param)
	{
		if (!(null == param))
		{
			Toggle component = param.gameObject.GetComponent<Toggle>();
			if (component.isOn && sortType != param.index)
			{
				sortType = param.index;
				DisvisibleRanking();
				UpdateSearch();
			}
		}
	}

	public void OnChangeSortRankType(bool flags)
	{
		sortRankType = ((!flags) ? 1 : 0);
		if (sortType == 0)
		{
			DisvisibleRanking();
			UpdateSearch();
		}
	}

	public void OnChangePage(int flags)
	{
		int num = nowPage;
		switch (flags)
		{
		case 0:
			nowPage = 0;
			break;
		case 1:
			nowPage = Mathf.Max(0, nowPage - 1);
			break;
		case 2:
			if (maxPage == 0)
			{
				nowPage = 0;
			}
			else
			{
				nowPage = Mathf.Min(maxPage - 1, nowPage + 1);
			}
			break;
		case 3:
			nowPage = ((maxPage != 0) ? (maxPage - 1) : 0);
			break;
		}
		if (num != nowPage)
		{
			selChara = -1;
			UpdateSelectCharaInfo();
			DisvisibleRanking();
			UpdatePage();
		}
	}

	public void OnChangeSelect(int index)
	{
		if (modeThumbSize == 0)
		{
			selChara = nowPage * 21 + index;
		}
		else
		{
			selChara = nowPage * 10 + index;
		}
		UpdateSelectCharaInfo();
	}

	public void OnUnCheckedAllSortConditions()
	{
		noUpdate = true;
		Toggle[] array = tglHeight;
		foreach (Toggle toggle in array)
		{
			toggle.isOn = false;
		}
		Toggle[] array2 = tglBust;
		foreach (Toggle toggle2 in array2)
		{
			toggle2.isOn = false;
		}
		Toggle[] array3 = tglHair;
		foreach (Toggle toggle3 in array3)
		{
			toggle3.isOn = false;
		}
		Toggle[] array4 = tglClub;
		foreach (Toggle toggle4 in array4)
		{
			toggle4.isOn = false;
		}
		Toggle[] array5 = tglPersonality;
		foreach (Toggle toggle5 in array5)
		{
			toggle5.isOn = false;
		}
		noUpdate = false;
		if (modeSex != 0)
		{
			DisvisibleRanking();
			UpdateSearch();
		}
	}

	private void SetButtonClickHandler(Button clickObj, int index)
	{
		clickObj.onClick.AddListener(delegate
		{
			OnChangeSelect(index);
		});
	}

	private void DisvisibleRanking()
	{
		if ((bool)objRankingS)
		{
			objRankingS.SetActiveIfDifferent(false);
		}
		if ((bool)objRankingL)
		{
			objRankingL.SetActiveIfDifferent(false);
		}
	}

	private void UpdatePageMax()
	{
		int num = ((modeThumbSize != 0) ? 10 : 21);
		maxPage = lstSearch.Count / num;
		maxPage += ((lstSearch.Count % num != 0) ? 1 : 0);
		if (nowPage >= maxPage)
		{
			nowPage = 0;
		}
	}

	private void UpdatePage()
	{
		if (modeThumbSize == 0)
		{
			for (int i = 0; i < 21; i++)
			{
				if ((bool)imgTmbSmall[i])
				{
					imgTmbSmall[i].enabled = false;
				}
				if ((bool)btnTmbSmall[i])
				{
					btnTmbSmall[i].interactable = false;
				}
			}
		}
		else
		{
			for (int j = 0; j < 10; j++)
			{
				if ((bool)imgTmbLarge[j])
				{
					imgTmbLarge[j].enabled = false;
				}
				if ((bool)btnTmbLarge[j])
				{
					btnTmbLarge[j].interactable = false;
				}
			}
		}
		if (lstSearch.Count == 0)
		{
			return;
		}
		int num = ((modeThumbSize != 0) ? 10 : 21);
		int num2 = nowPage * num;
		for (int k = 0; k < num; k++)
		{
			int num3 = num2 + k;
			if (lstSearch.Count <= num3)
			{
				break;
			}
			if (modeThumbSize == 0)
			{
				if ((bool)btnTmbSmall[k])
				{
					btnTmbSmall[k].interactable = true;
				}
			}
			else if ((bool)btnTmbLarge[k])
			{
				btnTmbLarge[k].interactable = true;
			}
		}
		if (modeGetInfoData == 0)
		{
			corGetInfoStartTime = Time.realtimeSinceStartup;
			corGetInfo = UpdateThumbnail();
			StartCoroutine(corGetInfo);
		}
		else
		{
			updateTmbFlag = true;
		}
	}

	private void UpdatePageSprite()
	{
		int num = ((maxPage != 0) ? (nowPage + 1) : 0);
		int[] array = new int[5];
		int[] array2 = new int[5];
		array[0] = num % 10;
		array[1] = num % 100 / 10;
		array[2] = num % 1000 / 100;
		array[3] = num % 10000 / 1000;
		array[4] = num / 10000;
		array2[0] = maxPage % 10;
		array2[1] = maxPage % 100 / 10;
		array2[2] = maxPage % 1000 / 100;
		array2[3] = maxPage % 10000 / 1000;
		array2[4] = maxPage / 10000;
		for (int i = 0; i < 5; i++)
		{
			imgPNum[i].sprite = imgNumP[array[i]].sprite;
			imgPMax[i].sprite = imgNumP[array2[i]].sprite;
		}
		for (int j = 0; j < 5; j++)
		{
			imgPNum[j].enabled = true;
			imgPMax[j].enabled = true;
		}
		int num2 = 4;
		while (num2 > 0 && array[num2] == 0)
		{
			imgPNum[num2].enabled = false;
			num2--;
		}
		int num3 = 4;
		while (num3 > 0 && array2[num3] == 0)
		{
			imgPMax[num3].enabled = false;
			num3--;
		}
	}

	private void UpdateRanking()
	{
		if ((bool)objRankingS)
		{
			objRankingS.SetActiveIfDifferent(false);
		}
		if ((bool)objRankingL)
		{
			objRankingL.SetActiveIfDifferent(false);
		}
		if (!drawRanking || lstSearch.Count == 0)
		{
			return;
		}
		if ((bool)objRankingS)
		{
			objRankingS.SetActiveIfDifferent(true);
		}
		if ((bool)objRankingL)
		{
			objRankingL.SetActiveIfDifferent(true);
		}
		int num = ((modeThumbSize != 0) ? 10 : 21);
		int num2 = nowPage * num;
		List<DLDataHeader> list = lstAll.ToList();
		list.RemoveAll((DLDataHeader t) => modeSex != t.Sex);
		list = ((sortRankType != 0) ? (from n in list
			orderby n.WeekCount descending, n.PID
			select n).ToList() : (from n in list
			orderby n.DLCount descending, n.PID
			select n).ToList());
		if (modeThumbSize == 0)
		{
			for (int i = 0; i < num; i++)
			{
				dataRankS[i].Disable();
			}
		}
		else
		{
			for (int j = 0; j < num; j++)
			{
				dataRankL[j].Disable();
			}
		}
		for (int k = 0; k < num; k++)
		{
			int index = num2 + k;
			if (lstSearch.Count <= index)
			{
				break;
			}
			int value = list.FindIndex((DLDataHeader p) => p.PID == lstSearch[index].PID) + 1;
			if (modeThumbSize == 0)
			{
				dataRankS[k].Update(value, imgNumRS);
			}
			else
			{
				dataRankL[k].Update(value, imgNumRL);
			}
		}
	}

	private void UpdateSelectCharaSprite()
	{
		if (modeThumbSize == 0)
		{
			int num = -1;
			if (selChara != -1)
			{
				num = selChara % 21;
			}
			for (int i = 0; i < 21; i++)
			{
				imgSelSmall[i].enabled = ((i == num) ? true : false);
			}
		}
		else
		{
			int num2 = -1;
			if (selChara != -1)
			{
				num2 = selChara % 10;
			}
			for (int j = 0; j < 10; j++)
			{
				imgSelLarge[j].enabled = ((j == num2) ? true : false);
			}
		}
	}

	private void UpdateSelectCharaInfo()
	{
		if (selChara == -1)
		{
			if (dlCharaInfoViewer != null)
			{
				dlCharaInfoViewer.data = null;
			}
			if ((bool)btnDelete)
			{
				btnDelete.interactable = false;
			}
			if ((bool)textDelete)
			{
				textDelete.color = new Color(textDelete.color.r, textDelete.color.g, textDelete.color.b, 0.5f);
			}
			if ((bool)btnDownload)
			{
				btnDownload.interactable = false;
			}
			if ((bool)textDownload)
			{
				textDownload.color = new Color(textDownload.color.r, textDownload.color.g, textDownload.color.b, 0.5f);
			}
			return;
		}
		DLDataHeader dLDataHeader = lstSearch[selChara];
		string nickname = string.Empty;
		string personality = string.Empty;
		string empty = string.Empty;
		string text = dLDataHeader.BirthDay;
		if (Localize.Translate.Manager.isTranslate)
		{
			string text2 = translateCharaInfo.Values.FindTagText("Month");
			text = text.Replace("月", text2 ?? string.Empty);
			string text3 = translateCharaInfo.Values.FindTagText("Day");
			text = text.Replace("日", text3 ?? string.Empty);
		}
		if (modeSex == 0)
		{
			empty = translateOther.SafeGetText(0) ?? "コイカツ部";
		}
		else
		{
			nickname = dLDataHeader.NickName;
			personality = dLDataHeader.PersonalTypeName;
			empty = dLDataHeader.ClubActivityName;
		}
		if (dlCharaInfoViewer != null)
		{
			dlCharaInfoViewer.data = new DownloadCharaInfoViewer.Data
			{
				name = dLDataHeader.CharaName,
				nickname = nickname,
				personality = personality,
				birthday = text,
				blood = ChaFileDefine.GetBloodTypeStr(dLDataHeader.BloodType),
				club = empty,
				HN = dLDataHeader.HandleName,
				comment = dLDataHeader.Comment
			};
		}
		string uUID = CommonLib.GetUUID();
		if (string.Empty != dLDataHeader.UserID && uUID == dLDataHeader.UserID)
		{
			if ((bool)btnDelete)
			{
				btnDelete.interactable = true;
			}
			if ((bool)textDelete)
			{
				textDelete.color = new Color(textDelete.color.r, textDelete.color.g, textDelete.color.b, 1f);
			}
		}
		else
		{
			if ((bool)btnDelete)
			{
				btnDelete.interactable = false;
			}
			if ((bool)textDelete)
			{
				textDelete.color = new Color(textDelete.color.r, textDelete.color.g, textDelete.color.b, 0.5f);
			}
		}
		if ((bool)btnDownload)
		{
			btnDownload.interactable = true;
		}
		if ((bool)textDownload)
		{
			textDownload.color = new Color(textDownload.color.r, textDownload.color.g, textDownload.color.b, 1f);
		}
	}

	public void UpdateSearch()
	{
		if (noUpdate)
		{
			return;
		}
		int num = -1;
		if (selChara != -1)
		{
			num = lstSearch[selChara].PID;
		}
		lstSearch.Clear();
		bool[] array = new bool[3];
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			if (!(null == tglHeight[i]))
			{
				array[i] = tglHeight[i].isOn;
				if (array[i])
				{
					flag = true;
				}
			}
		}
		bool[] array2 = new bool[3];
		bool flag2 = false;
		for (int j = 0; j < 3; j++)
		{
			if (!(null == tglBust[j]))
			{
				array2[j] = tglBust[j].isOn;
				if (array2[j])
				{
					flag2 = true;
				}
			}
		}
		bool[] array3 = new bool[6];
		bool flag3 = false;
		for (int k = 0; k < 6; k++)
		{
			if (!(null == tglHair[k]))
			{
				array3[k] = tglHair[k].isOn;
				if (array3[k])
				{
					flag3 = true;
				}
			}
		}
		bool flag4 = false;
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		foreach (KeyValuePair<int, int> item in dictClub)
		{
			int key = item.Key;
			if (!(null == tglClub[key]))
			{
				dictionary[item.Value] = tglClub[key].isOn;
				if (dictionary[item.Value])
				{
					flag4 = true;
				}
			}
		}
		bool flag5 = false;
		Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>();
		foreach (KeyValuePair<int, int> item2 in dictPersonality)
		{
			int key2 = item2.Key;
			if (!(null == tglPersonality[key2]))
			{
				dictionary2[item2.Value] = tglPersonality[key2].isOn;
				if (dictionary2[item2.Value])
				{
					flag5 = true;
				}
			}
		}
		string uUID = CommonLib.GetUUID();
		foreach (DLDataHeader item3 in lstAll)
		{
			if ((searchMyChara && (string.Empty == item3.UserID || uUID != item3.UserID)) || modeSex != item3.Sex)
			{
				continue;
			}
			if (modeSex != 0)
			{
				if ((flag && !array[item3.Height]) || (flag2 && !array2[item3.BustSize]) || (flag3 && !array3[item3.HairType]))
				{
					continue;
				}
				if (flag4)
				{
					bool value = false;
					if (!dictionary.TryGetValue(item3.ClubActivity, out value) || !value)
					{
						continue;
					}
				}
				if (flag5)
				{
					bool value2 = false;
					if (!dictionary2.TryGetValue(item3.PersonalType, out value2) || !value2)
					{
						continue;
					}
				}
			}
			lstSearch.Add(item3);
		}
		if (sortType == 0)
		{
			if (sortRankType == 0)
			{
				lstSearch = (from n in lstSearch
					orderby n.DLCount descending, n.PID
					select n).ToList();
			}
			else
			{
				lstSearch = (from n in lstSearch
					orderby n.WeekCount descending, n.PID
					select n).ToList();
			}
		}
		else if (sortType == 1)
		{
			lstSearch.Sort((DLDataHeader a, DLDataHeader b) => b.PID - a.PID);
		}
		else
		{
			lstSearch.Sort((DLDataHeader a, DLDataHeader b) => a.PID - b.PID);
		}
		UpdatePageMax();
		selChara = -1;
		for (int l = 0; l < lstSearch.Count; l++)
		{
			if (lstSearch[l].PID == num)
			{
				selChara = l;
				break;
			}
		}
		if (!CheckCharaInPage(selChara))
		{
			selChara = -1;
		}
		UpdateSelectCharaInfo();
		UpdatePage();
	}

	private bool CheckCharaInPage(int _selChara)
	{
		int num = ((modeThumbSize != 0) ? 10 : 21);
		int num2 = nowPage * num;
		for (int i = 0; i < num; i++)
		{
			int num3 = num2 + i;
			if (lstSearch.Count <= num3)
			{
				break;
			}
			if (num3 == _selChara)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator GetListAll()
	{
		SetMessage(translateMessageTitle.Values.FindTagText("GetData") ?? "データを取得しています", -1f);
		lstAll.Clear();
		modeGetInfoList = 1;
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		int trial = 0;
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 1);
		wwwform.AddField("trial", trial);
		WWW www = new WWW(UploaderURL, wwwform);
		yield return www;
		if (www.error != null)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedNet") ?? "ネットワークの接続に失敗しました", 2f);
		}
		else
		{
			string[] array = www.text.Split("\n"[0]);
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (string.Empty == text)
				{
					break;
				}
				string[] array3 = text.Split("\t"[0]);
				DLDataHeader dLDataHeader = new DLDataHeader();
				if (string.Empty == array3[0])
				{
					continue;
				}
				dLDataHeader.PID = int.Parse(array3[0]);
				if (string.Empty == array3[1])
				{
					continue;
				}
				dLDataHeader.Sex = int.Parse(array3[1]);
				dLDataHeader.Sex = Mathf.Clamp(dLDataHeader.Sex, 0, 1);
				if (string.Empty == array3[2])
				{
					continue;
				}
				dLDataHeader.Height = int.Parse(array3[2]);
				dLDataHeader.Height = Mathf.Clamp(dLDataHeader.Height, 0, 2);
				if (string.Empty == array3[3])
				{
					continue;
				}
				dLDataHeader.BustSize = int.Parse(array3[3]);
				dLDataHeader.BustSize = Mathf.Clamp(dLDataHeader.BustSize, 0, 2);
				if (string.Empty == array3[4])
				{
					continue;
				}
				dLDataHeader.HairType = int.Parse(array3[4]);
				dLDataHeader.HairType = Mathf.Clamp(dLDataHeader.HairType, 0, 5);
				if (string.Empty == array3[5])
				{
					continue;
				}
				dLDataHeader.PersonalType = int.Parse(array3[5]);
				if (!Singleton<Voice>.Instance.voiceInfoDic.ContainsKey(dLDataHeader.PersonalType))
				{
					dLDataHeader.PersonalType = 0;
				}
				if (string.Empty == array3[6])
				{
					continue;
				}
				dLDataHeader.BloodType = int.Parse(array3[6]);
				dLDataHeader.BloodType = Mathf.Clamp(dLDataHeader.BloodType, 0, 3);
				dLDataHeader.BirthDay = Encoding.UTF8.GetString(Convert.FromBase64String(array3[7]));
				if (string.Empty == array3[8])
				{
					continue;
				}
				dLDataHeader.ClubActivity = int.Parse(array3[8]);
				if (!Game.ClubInfos.ContainsKey(dLDataHeader.ClubActivity))
				{
					dLDataHeader.ClubActivity = 0;
				}
				dLDataHeader.CharaName = Encoding.UTF8.GetString(Convert.FromBase64String(array3[9]));
				dLDataHeader.NickName = Encoding.UTF8.GetString(Convert.FromBase64String(array3[10]));
				dLDataHeader.HandleName = Encoding.UTF8.GetString(Convert.FromBase64String(array3[11]));
				dLDataHeader.Comment = Encoding.UTF8.GetString(Convert.FromBase64String(array3[12]));
				dLDataHeader.UserID = array3[13];
				if (!(string.Empty == array3[14]))
				{
					dLDataHeader.DLCount = int.Parse(array3[14]);
					if (!(string.Empty == array3[15]))
					{
						dLDataHeader.WeekCount = int.Parse(array3[15]);
						dLDataHeader.PersonalTypeName = ((dLDataHeader.Sex != 0) ? Localize.Translate.Manager.GetPersonalityName(dLDataHeader.PersonalType, false) : string.Empty);
						dLDataHeader.ClubActivityName = ((dLDataHeader.Sex != 0) ? Localize.Translate.Manager.GetClubName(dLDataHeader.ClubActivity, false) : string.Empty);
						lstAll.Add(dLDataHeader);
					}
				}
			}
			DisvisibleMessage();
			UpdateSearch();
		}
		modeGetInfoList = 0;
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(false);
		}
		yield return null;
	}

	private IEnumerator UpdateThumbnail()
	{
		SetMessage(translateMessageTitle.Values.FindTagText("GetData") ?? "データを取得しています", -1f);
		DLThumbNum = 0;
		modeGetInfoData = 1;
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		int drawfig = ((modeThumbSize != 0) ? 10 : 21);
		int topindex = nowPage * drawfig;
		if (enableCache)
		{
			Dictionary<int, bool> dictThumbID = new Dictionary<int, bool>();
			for (int i = 0; i < drawfig; i++)
			{
				int num = topindex + i;
				int pID = lstSearch[num].PID;
				CacheHeader ch = null;
				if (GetCacheHeader(pID, out ch) != string.Empty)
				{
					dictThumbID[pID] = true;
				}
				else
				{
					dictThumbID[pID] = false;
				}
				if (lstSearch.Count <= num + 1 || i + 1 >= drawfig)
				{
					break;
				}
			}
			string strPID = string.Empty;
			foreach (KeyValuePair<int, bool> item in dictThumbID)
			{
				if (!item.Value)
				{
					strPID += item.Key;
					strPID += "\n";
				}
			}
			if (dictThumbID.Count == 0)
			{
				modeGetInfoData = 0;
				if ((bool)objNoControl)
				{
					objNoControl.SetActiveIfDifferent(false);
				}
				SetMessage(translateMessageTitle.Values.FindTagText("NotFound") ?? "アップロードされたデータが見つかりませんでした", 2f);
				yield break;
			}
			if (strPID != string.Empty)
			{
				strPID = YS_Assist.GetRemoveStringRight(strPID, "\n", true);
				WWWForm wwwform = new WWWForm();
				wwwform.AddField("mode", 2);
				wwwform.AddField("pid", strPID);
				WWW www = new WWW(UploaderURL, wwwform);
				yield return www;
				if (www.error != null)
				{
					SetMessage(translateMessageTitle.Values.FindTagText("FailedNet") ?? "ネットワークの接続に失敗しました", 2f);
					modeGetInfoData = 0;
					if ((bool)objNoControl)
					{
						objNoControl.SetActiveIfDifferent(false);
					}
					yield break;
				}
				if (!updateTmbFlag)
				{
					CreateCacheThumbnail(strPID, www.text);
				}
			}
			if (!updateTmbFlag)
			{
				Dictionary<int, byte[]> dictionary = new Dictionary<int, byte[]>();
				foreach (KeyValuePair<int, bool> item2 in dictThumbID)
				{
					dictionary[item2.Key] = null;
				}
				GetCacheThumbnail(dictionary);
				foreach (var item3 in dictionary.Select((KeyValuePair<int, byte[]> val, int idx) => new { val, idx }))
				{
					byte[] value = item3.val.Value;
					int idx2 = item3.idx;
					DLThumbNum++;
					Vector2 pivot = new Vector2(0f, 1f);
					Texture2D texture2D = PngAssist.ChangeTextureFromByte(value);
					Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
					Sprite sprite = Sprite.Create(texture2D, rect, pivot);
					if (modeThumbSize == 0)
					{
						if ((bool)imgTmbSmall[idx2].sprite)
						{
							if ((bool)imgTmbSmall[idx2].sprite.texture)
							{
								UnityEngine.Object.Destroy(imgTmbSmall[idx2].sprite.texture);
							}
							UnityEngine.Object.Destroy(imgTmbSmall[idx2].sprite);
						}
						imgTmbSmall[idx2].sprite = sprite;
						imgTmbSmall[idx2].enabled = true;
						continue;
					}
					if ((bool)imgTmbLarge[idx2].sprite)
					{
						if ((bool)imgTmbLarge[idx2].sprite.texture)
						{
							UnityEngine.Object.Destroy(imgTmbLarge[idx2].sprite.texture);
						}
						UnityEngine.Object.Destroy(imgTmbLarge[idx2].sprite);
					}
					imgTmbLarge[idx2].sprite = sprite;
					imgTmbLarge[idx2].enabled = true;
				}
				UpdateRanking();
			}
			DisvisibleMessage();
		}
		else
		{
			string pid = string.Empty;
			for (int j = 0; j < drawfig; j++)
			{
				int num2 = topindex + j;
				pid += lstSearch[num2].PID;
				if (lstSearch.Count <= num2 + 1 || j + 1 >= drawfig)
				{
					break;
				}
				pid += "\n";
			}
			if (pid == string.Empty)
			{
				modeGetInfoData = 0;
				if ((bool)objNoControl)
				{
					objNoControl.SetActiveIfDifferent(false);
				}
				SetMessage(translateMessageTitle.Values.FindTagText("NotFound") ?? "アップロードされたデータが見つかりませんでした", 2f);
				yield break;
			}
			WWWForm wwwform2 = new WWWForm();
			wwwform2.AddField("mode", 2);
			wwwform2.AddField("pid", pid);
			WWW www2 = new WWW(UploaderURL, wwwform2);
			yield return www2;
			if (www2.error != null)
			{
				SetMessage(translateMessageTitle.Values.FindTagText("FailedNet") ?? "ネットワークの接続に失敗しました", 2f);
			}
			else
			{
				if (!updateTmbFlag)
				{
					string[] array = www2.text.Split("\n"[0]);
					for (int k = 0; k < array.Length; k++)
					{
						byte[] data = Convert.FromBase64String(array[k]);
						DLThumbNum++;
						Vector2 pivot2 = new Vector2(0f, 1f);
						Texture2D texture2D2 = PngAssist.ChangeTextureFromByte(data);
						Rect rect2 = new Rect(0f, 0f, texture2D2.width, texture2D2.height);
						Sprite sprite2 = Sprite.Create(texture2D2, rect2, pivot2);
						if (modeThumbSize == 0)
						{
							if ((bool)imgTmbSmall[k].sprite)
							{
								if ((bool)imgTmbSmall[k].sprite.texture)
								{
									UnityEngine.Object.Destroy(imgTmbSmall[k].sprite.texture);
								}
								UnityEngine.Object.Destroy(imgTmbSmall[k].sprite);
							}
							imgTmbSmall[k].sprite = sprite2;
							imgTmbSmall[k].enabled = true;
							continue;
						}
						if ((bool)imgTmbLarge[k].sprite)
						{
							if ((bool)imgTmbLarge[k].sprite.texture)
							{
								UnityEngine.Object.Destroy(imgTmbLarge[k].sprite.texture);
							}
							UnityEngine.Object.Destroy(imgTmbLarge[k].sprite);
						}
						imgTmbLarge[k].sprite = sprite2;
						imgTmbLarge[k].enabled = true;
					}
					UpdateRanking();
				}
				DisvisibleMessage();
			}
		}
		modeGetInfoData = 0;
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(false);
		}
		yield return null;
	}

	private IEnumerator DeleteMyUpload()
	{
		string msg = translateQuestionTitle.Values.FindTagText("Question") ?? "本当に削除しますか？";
		ObservableYieldInstruction<bool> chk = Observable.FromCoroutine((IObserver<bool> res) => popupCheck.CheckAnswerCor(res, msg)).ToYieldInstruction(false);
		yield return chk;
		if (!chk.Result)
		{
			yield break;
		}
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		DisvisibleRanking();
		int pid = lstSearch[selChara].PID;
		string uid = CommonLib.GetUUID();
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 5);
		wwwform.AddField("pid", pid);
		wwwform.AddField("uid", uid);
		ObservableYieldInstruction<string> o = ObservableWWW.Post(UploaderURL, wwwform).Timeout(TimeSpan.FromSeconds(10.0)).ToYieldInstruction(false);
		yield return o;
		if (o.HasError)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedNet") ?? "ネットワークの接続に失敗しました", 2f);
			if ((bool)objNoControl)
			{
				objNoControl.SetActiveIfDifferent(true);
			}
			yield break;
		}
		if (o.HasResult)
		{
		}
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		corGetInfoStartTime = Time.realtimeSinceStartup;
		corGetInfo = GetListAll();
		StartCoroutine(corGetInfo);
		yield return null;
	}

	private IEnumerator DLCountUpdate(int pid)
	{
		modeDLCount = 1;
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 4);
		wwwform.AddField("pid", pid);
		if (lstDownloadPID.ContainsKey(lstSearch[selChara].PID))
		{
			modeDLCount = 0;
			if ((bool)objNoControl)
			{
				objNoControl.SetActiveIfDifferent(false);
			}
			SetMessage(translateMessageTitle.Values.FindTagText("Overlaped") ?? "既にダウンロード済みのファイルです", 2f);
			yield break;
		}
		WWW www = new WWW(UploaderURL, wwwform);
		yield return www;
		if (www.error != null)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedNet") ?? "ネットワークの接続に失敗しました", 2f);
		}
		else
		{
			string[] array = new string[2]
			{
				UserData.Path + "chara/male/",
				UserData.Path + "chara/female/"
			};
			string[] array2 = new string[2] { "charaM_", "charaF_" };
			string text = array2[modeSex] + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
			string path = array[modeSex] + text;
			byte[] buffer = Convert.FromBase64String(www.text);
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					binaryWriter.Write(buffer);
				}
			}
			lstDownloadPID[lstSearch[selChara].PID] = true;
			SetMessage(translateMessageTitle.Values.FindTagText("Success") ?? "キャラデータを追加しました", 2f);
		}
		modeDLCount = 0;
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(false);
		}
		yield return null;
	}

	private void Update()
	{
		UpdateMessage();
		UpdatePageSprite();
		UpdateSelectCharaSprite();
		if (modeDLCount == 1)
		{
			float num = Time.realtimeSinceStartup - corDLCntStartTime;
			if (num > 10f)
			{
				StopCoroutine(corDLCnt);
				modeDLCount = 0;
				if ((bool)objNoControl)
				{
					objNoControl.SetActiveIfDifferent(false);
				}
			}
			return;
		}
		if (modeGetInfoList == 1)
		{
			float num2 = Time.realtimeSinceStartup - corGetInfoStartTime;
			if (num2 > 20f)
			{
				StopCoroutine(corGetInfo);
				modeGetInfoList = 0;
				if ((bool)objNoControl)
				{
					objNoControl.SetActiveIfDifferent(false);
				}
				SetMessage(translateMessageTitle.Values.FindTagText("FailedData") ?? "データの取得に失敗しました", 2f);
			}
			return;
		}
		if (modeGetInfoData == 1)
		{
			float num3 = Time.realtimeSinceStartup - corGetInfoStartTime;
			if (num3 > 20f && DLThumbNum == 0)
			{
				StopCoroutine(corGetInfo);
				modeGetInfoData = 0;
				if ((bool)objNoControl)
				{
					objNoControl.SetActiveIfDifferent(false);
				}
				SetMessage(translateMessageTitle.Values.FindTagText("FailedData") ?? "データの取得に失敗しました", 2f);
				return;
			}
		}
		else if (modeGetInfoData == 0 && updateTmbFlag)
		{
			updateTmbFlag = false;
			corGetInfoStartTime = Time.realtimeSinceStartup;
			corGetInfo = UpdateThumbnail();
			StartCoroutine(corGetInfo);
			return;
		}
		if (string.Empty == Singleton<Scene>.Instance.AddSceneName && !checkMode && Input.GetKeyDown(KeyCode.Escape))
		{
			Utils.Scene.GameEnd();
		}
	}

	public bool SaveCacheSetting()
	{
		string path = pathCacheDir + fileCacheSetting;
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(enableCache);
			}
		}
		return true;
	}

	public bool LoadCacheSetting()
	{
		string path = pathCacheDir + fileCacheSetting;
		using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				enableCache = binaryReader.ReadBoolean();
			}
		}
		return true;
	}

	private Dictionary<int, string> GetCacheFileList()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		string empty = string.Empty;
		for (int i = 0; i < 100; i++)
		{
			empty = pathCacheDir + "thumb_" + i.ToString("00") + ".dat";
			if (File.Exists(empty))
			{
				dictionary[i] = empty;
			}
		}
		return dictionary;
	}

	public IEnumerator ClearCache()
	{
		string msg = "本当に削除しますか？";
		string title = null;
		translateQuestionTitle.Values.ToArray("Question").SafeProc(1, delegate(string text)
		{
			title = text;
		});
		Utils.Sound.Play(SystemSE.window_o);
		ObservableYieldInstruction<bool> chk = Observable.FromCoroutine((IObserver<bool> res) => popupCheck.CheckAnswerCor(res, title ?? msg)).ToYieldInstruction(false);
		yield return chk;
		if (!chk.Result)
		{
			yield break;
		}
		Dictionary<int, string> dictCacheFileList = GetCacheFileList();
		foreach (KeyValuePair<int, string> item in dictCacheFileList)
		{
			if (File.Exists(item.Value))
			{
				File.Delete(item.Value);
			}
		}
		UpdateCacheHeaderInfo();
		yield return null;
	}

	public void OnEnableCache(bool value)
	{
		enableCache = value;
		SaveCacheSetting();
	}

	public bool CreateCacheThumbnail(string strPID, string strData)
	{
		Dictionary<int, byte[]> dictionary = null;
		string text = string.Empty;
		for (int i = 0; i < 100; i++)
		{
			text = pathCacheDir + "thumb_" + i.ToString("00") + ".dat";
			if (File.Exists(text))
			{
				List<CacheHeader> value = null;
				if (!dictCacheHeaderInfo.TryGetValue(text, out value) || value.Count < 1000)
				{
					dictionary = LoadCacheFile(text);
					break;
				}
				continue;
			}
			dictionary = new Dictionary<int, byte[]>();
			break;
		}
		string[] array = strPID.Split("\n"[0]);
		string[] array2 = strData.Split("\n"[0]);
		if (array.Length != array2.Length)
		{
			return false;
		}
		for (int j = 0; j < array.Length; j++)
		{
			int key = int.Parse(array[j]);
			dictionary[key] = Convert.FromBase64String(array2[j]);
		}
		SaveCacheFile(text, dictionary);
		UpdateCacheHeaderInfo();
		return true;
	}

	public void GetCacheThumbnail(Dictionary<int, byte[]> dictPNG)
	{
		int[] array = dictPNG.Keys.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			CacheHeader ch = null;
			string cacheHeader = GetCacheHeader(array[i], out ch);
			if (ch != null)
			{
				dictPNG[ch.id] = LoadCache(cacheHeader, ch.pos, ch.size);
			}
		}
	}

	public string GetCacheHeader(int id, out CacheHeader ch)
	{
		ch = null;
		string empty = string.Empty;
		foreach (KeyValuePair<string, List<CacheHeader>> item in dictCacheHeaderInfo)
		{
			foreach (CacheHeader item2 in item.Value)
			{
				if (item2.id == id)
				{
					ch = new CacheHeader();
					ch.id = item2.id;
					ch.pos = item2.pos;
					ch.size = item2.size;
					return item.Key;
				}
			}
		}
		return empty;
	}

	private void SaveCacheFile(string path, Dictionary<int, byte[]> dictPNG)
	{
		int[] array = dictPNG.Keys.ToArray();
		Dictionary<int, long> dictionary = new Dictionary<int, long>();
		byte[] buffer = null;
		int num = 16;
		long num2 = 4 + Encoding.UTF8.GetByteCount(CacheFileMark) + 4 + num * array.Length + 1;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				foreach (int key in array)
				{
					dictionary[key] = num2;
					num2 += dictPNG[key].Length;
					binaryWriter.Write(dictPNG[key]);
				}
				buffer = memoryStream.ToArray();
			}
		}
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter2 = new BinaryWriter(output))
			{
				binaryWriter2.Write(CacheFileVersion);
				binaryWriter2.Write(CacheFileMark);
				binaryWriter2.Write(array.Length);
				foreach (int num3 in array)
				{
					binaryWriter2.Write(num3);
					binaryWriter2.Write(dictionary[num3]);
					binaryWriter2.Write(dictPNG[num3].Length);
				}
				binaryWriter2.Write(buffer);
			}
		}
	}

	private Dictionary<int, byte[]> LoadCacheFile(string path)
	{
		Dictionary<int, byte[]> dictionary = new Dictionary<int, byte[]>();
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				int num = binaryReader.ReadInt32();
				string text = binaryReader.ReadString();
				List<CacheHeader> list = new List<CacheHeader>();
				int num2 = binaryReader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					CacheHeader cacheHeader = new CacheHeader();
					cacheHeader.id = binaryReader.ReadInt32();
					cacheHeader.pos = binaryReader.ReadInt64();
					cacheHeader.size = binaryReader.ReadInt32();
					list.Add(cacheHeader);
				}
				foreach (CacheHeader item in list)
				{
					fileStream.Seek(item.pos, SeekOrigin.Begin);
					byte[] value = binaryReader.ReadBytes(item.size);
					dictionary[item.id] = value;
				}
				return dictionary;
			}
		}
	}

	private byte[] LoadCache(string path, long pos, int size)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			fileStream.Seek(pos, SeekOrigin.Begin);
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				return binaryReader.ReadBytes(size);
			}
		}
	}

	public void UpdateCacheHeaderInfo()
	{
		dictCacheHeaderInfo.Clear();
		Dictionary<int, string> cacheFileList = GetCacheFileList();
		foreach (KeyValuePair<int, string> item in cacheFileList)
		{
			using (FileStream input = new FileStream(item.Value, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					string text = binaryReader.ReadString();
					List<CacheHeader> list = new List<CacheHeader>();
					int num2 = binaryReader.ReadInt32();
					for (int i = 0; i < num2; i++)
					{
						CacheHeader cacheHeader = new CacheHeader();
						cacheHeader.id = binaryReader.ReadInt32();
						cacheHeader.pos = binaryReader.ReadInt64();
						cacheHeader.size = binaryReader.ReadInt32();
						list.Add(cacheHeader);
					}
					dictCacheHeaderInfo[item.Value] = list;
				}
			}
		}
	}
}
