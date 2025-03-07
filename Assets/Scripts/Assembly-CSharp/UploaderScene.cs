using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CharaFiles;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UploaderScene : MonoBehaviour
{
	private class ServerHashInfo
	{
		public string hashpng = string.Empty;

		public string hashparam = string.Empty;

		public string userId = string.Empty;

		public ServerHashInfo(string _hashpng, string _hashparam, string _userId)
		{
			hashpng = _hashpng;
			hashparam = _hashparam;
			userId = _userId;
		}
	}

	[SerializeField]
	private Canvas cvsChangeScene;

	[SerializeField]
	private GameObject objNoControl;

	[SerializeField]
	private PopupCheck popupCheck;

	[SerializeField]
	private ChaFileListCtrl[] fileListCtrl;

	[SerializeField]
	private Toggle[] tglSex;

	[SerializeField]
	private GameObject[] objSexTop;

	[SerializeField]
	private Image[] imgPreview;

	[SerializeField]
	private GameObject objPolicy;

	[SerializeField]
	private Text textMaleName;

	[SerializeField]
	private TextMeshProUGUI[] textMaleInfo;

	[SerializeField]
	private Text textFemaleName;

	[SerializeField]
	private Text textFemaleNickName;

	[SerializeField]
	private TextMeshProUGUI[] textFemaleInfo;

	[SerializeField]
	private InputField inpHN;

	[SerializeField]
	private GameObject objHNDummy;

	[SerializeField]
	private Text textHNDummy;

	[SerializeField]
	private InputField inpComment;

	[SerializeField]
	private GameObject objCommentDummy;

	[SerializeField]
	private Text textCommentDummy;

	[SerializeField]
	private Button btnGotoDownload;

	[SerializeField]
	private Button btnGotoTitle;

	[SerializeField]
	private Button btnPolicy;

	[SerializeField]
	private Button btnUpload;

	[SerializeField]
	private Button btnExitPolicy;

	[SerializeField]
	private string UploaderURL = string.Empty;

	[SerializeField]
	private GameObject objMessage;

	[SerializeField]
	private Text txtMessage;

	private float msgCount;

	private int resultChangeUpdata = -1;

	private Dictionary<int, ServerHashInfo> dictServerHash = new Dictionary<int, ServerHashInfo>();

	private Dictionary<int, ServerHashInfo> dictLocalHash = new Dictionary<int, ServerHashInfo>();

	private bool uploadDebug;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private Dictionary<int, Data.Param> translateQuestionTitle
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

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.UPLOAD));
		}
	}

	private void CreateCharaList()
	{
		for (int i = 0; i < 2; i++)
		{
			foreach (var item in Localize.Translate.Manager.CreateChaFileInfo(i, false).Select((Localize.Translate.Manager.ChaFileInfo p, int index) => new { p, index }))
			{
				fileListCtrl[i].AddList(new ChaFileInfo(item.p.chaFile, item.p.info)
				{
					index = item.index
				});
			}
		}
		fileListCtrl[0].Create(OnChangeChaFileListSelectMale);
		fileListCtrl[1].Create(OnChangeChaFileListSelectFemale);
	}

	public void OnChangeChaFileListSelectMale(ChaFileInfo info)
	{
		Utils.Sound.Play(SystemSE.sel);
		bool flag = false;
		if (fileListCtrl[0].GetSelectIndex().IsNullOrEmpty())
		{
			flag = true;
		}
		UpdateCharaInfo(info, flag);
		UpdatePreview((!flag) ? info.FullPath : string.Empty, 0);
	}

	public void OnChangeChaFileListSelectFemale(ChaFileInfo info)
	{
		Utils.Sound.Play(SystemSE.sel);
		bool flag = false;
		if (fileListCtrl[1].GetSelectIndex().IsNullOrEmpty())
		{
			flag = true;
		}
		UpdateCharaInfo(info, flag);
		UpdatePreview((!flag) ? info.FullPath : string.Empty, 1);
	}

	private void UpdatePreview(string path, int sex)
	{
		if (null == imgPreview[sex])
		{
			return;
		}
		if ((bool)imgPreview[sex].sprite)
		{
			if ((bool)imgPreview[sex].sprite.texture)
			{
				UnityEngine.Object.Destroy(imgPreview[sex].sprite.texture);
			}
			UnityEngine.Object.Destroy(imgPreview[sex].sprite);
			imgPreview[sex].sprite = null;
		}
		if (!path.IsNullOrEmpty())
		{
			Sprite sprite = PngAssist.LoadSpriteFromFile(path);
			if ((bool)sprite)
			{
				imgPreview[sex].sprite = sprite;
			}
			imgPreview[sex].enabled = true;
		}
		else
		{
			imgPreview[sex].enabled = false;
		}
	}

	private void UpdateCharaInfo(ChaFileInfo info, bool clear)
	{
		if (info.sex == 0)
		{
			if (clear)
			{
				textMaleName.text = string.Empty;
				TextMeshProUGUI[] array = textMaleInfo;
				foreach (TextMeshProUGUI textMeshProUGUI in array)
				{
					textMeshProUGUI.text = string.Empty;
				}
			}
			else
			{
				textMaleName.text = info.name;
				textMaleInfo[0].text = info.birthDay;
				textMaleInfo[1].text = ChaFileDefine.GetBloodTypeStr(info.bloodType);
			}
		}
		else if (clear)
		{
			textFemaleName.text = string.Empty;
			textFemaleNickName.text = string.Empty;
			TextMeshProUGUI[] array2 = textFemaleInfo;
			foreach (TextMeshProUGUI textMeshProUGUI2 in array2)
			{
				textMeshProUGUI2.text = string.Empty;
			}
		}
		else
		{
			textFemaleName.text = info.name;
			textFemaleNickName.text = info.nickname;
			textFemaleInfo[0].text = Localize.Translate.Manager.GetPersonalityName(info.personality, false);
			textFemaleInfo[1].text = info.birthDay;
			textFemaleInfo[2].text = ChaFileDefine.GetBloodTypeStr(info.bloodType);
			textFemaleInfo[3].text = Localize.Translate.Manager.GetClubName(info.club, false);
		}
	}

	private void SaveHandleName()
	{
		string path = UserData.Path + "save/netInfo.dat";
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(NetworkDefine.NetInfoVersion.ToString());
				binaryWriter.Write(inpHN.text);
			}
		}
	}

	private void LoadHandleName()
	{
		string path = UserData.Path + "save/netInfo.dat";
		if (!File.Exists(path))
		{
			return;
		}
		using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				Version value = new Version(binaryReader.ReadString());
				if (0 <= NetworkDefine.NetInfoVersion.CompareTo(value))
				{
					inpHN.text = binaryReader.ReadString();
					textHNDummy.text = inpHN.text;
				}
			}
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

	public void GetCharaFileHashBytes(byte[] data, out byte[] hashPngBytes, out byte[] hashParamBytes)
	{
		ChaFileControl chaFileControl = null;
		chaFileControl = new ChaFileControl();
		chaFileControl.parameter.sex = ((!tglSex[0].isOn) ? ((byte)1) : ((byte)0));
		using (MemoryStream memoryStream = new MemoryStream())
		{
			memoryStream.Write(data, 0, data.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			chaFileControl.LoadCharaFile(memoryStream);
			byte[] pngData = chaFileControl.pngData;
			byte[] facePngData = chaFileControl.facePngData;
			byte[] array = new byte[pngData.Length + facePngData.Length];
			Array.Copy(pngData, array, pngData.Length);
			Array.Copy(facePngData, 0, array, pngData.Length, facePngData.Length);
			hashPngBytes = YS_Assist.CreateSha256(array, "【KoiKatuCharaS】");
			byte[] customBytes = chaFileControl.GetCustomBytes();
			byte[] coordinateBytes = chaFileControl.GetCoordinateBytes();
			byte[] parameterBytes = chaFileControl.GetParameterBytes();
			array = new byte[customBytes.Length + coordinateBytes.Length + parameterBytes.Length];
			Array.Copy(customBytes, array, customBytes.Length);
			Array.Copy(coordinateBytes, 0, array, customBytes.Length, coordinateBytes.Length);
			Array.Copy(parameterBytes, 0, array, customBytes.Length + coordinateBytes.Length, parameterBytes.Length);
			hashParamBytes = YS_Assist.CreateSha256(array, "【KoiKatuCharaS】");
		}
	}

	public void GetCharaFileHashStr(byte[] data, ref string hashPngStr, ref string hashParamStr)
	{
		byte[] hashPngBytes;
		byte[] hashParamBytes;
		GetCharaFileHashBytes(data, out hashPngBytes, out hashParamBytes);
		if (hashPngBytes == null || hashParamBytes == null)
		{
			hashPngStr = string.Empty;
			hashParamStr = string.Empty;
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = hashPngBytes;
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		hashPngStr = stringBuilder.ToString();
		stringBuilder.Length = 0;
		byte[] array2 = hashParamBytes;
		foreach (byte b2 in array2)
		{
			stringBuilder.Append(b2.ToString("x2"));
		}
		hashParamStr = stringBuilder.ToString();
	}

	private IEnumerator GetHashAll()
	{
		SetMessage(translateMessageTitle.Values.FindTagText("GetData") ?? "データを取得しています", -1f);
		dictServerHash.Clear();
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(true);
		}
		int trial = 0;
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 0);
		wwwform.AddField("trial", trial);
		ObservableYieldInstruction<string> o = ObservableWWW.Post(UploaderURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
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
			yield break;
		}
		if (o.HasResult)
		{
			string[] array = o.Result.Split("\n"[0]);
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (string.Empty == text)
				{
					break;
				}
				string[] array3 = text.Split("\t"[0]);
				if (!(string.Empty == array3[0]) && !(string.Empty == array3[1]) && !(string.Empty == array3[2]))
				{
					dictServerHash[int.Parse(array3[0])] = new ServerHashInfo(array3[1], array3[2], array3[3]);
				}
			}
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

	private IEnumerator UploadCor()
	{
		if ((bool)objNoControl)
		{
			objNoControl.SetActiveIfDifferent(false);
		}
		string msg = translateQuestionTitle.Values.FindTagText("Question") ?? "本当にアップロードしますか？";
		Utils.Sound.Play(SystemSE.window_o);
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
		string uid = CommonLib.GetUUID();
		if (string.Empty == uid)
		{
			CommonLib.CreateUUID();
			uid = CommonLib.GetUUID();
		}
		string handleName = inpHN.text;
		string comment = inpComment.text;
		int sex = ((!tglSex[0].isOn) ? 1 : 0);
		ChaFileInfoComponent cmp = fileListCtrl[sex].GetSelectTopItem();
		byte[] data = null;
		using (FileStream fileStream = new FileStream(cmp.info.FullPath, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				data = binaryReader.ReadBytes((int)fileStream.Length);
			}
		}
		if (data == null)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedUpload") ?? "キャラデータのアップロードに失敗しました", 2f);
			yield break;
		}
		string hashPng = string.Empty;
		string hashParam = string.Empty;
		GetCharaFileHashStr(data, ref hashPng, ref hashParam);
		int pid = -1;
		foreach (KeyValuePair<int, ServerHashInfo> item in dictLocalHash)
		{
			if (hashParam != item.Value.hashparam)
			{
				continue;
			}
			if (hashPng == item.Value.hashpng)
			{
				SetMessage(translateMessageTitle.Values.FindTagText("Overlaped") ?? "そのデータは既にアップされています。", 2f);
				yield break;
			}
			pid = item.Key;
			break;
		}
		if (pid == -1)
		{
			foreach (KeyValuePair<int, ServerHashInfo> item2 in dictServerHash)
			{
				if (hashParam != item2.Value.hashparam)
				{
					continue;
				}
				if (uid == item2.Value.userId)
				{
					if (!(hashPng == item2.Value.hashpng))
					{
						pid = item2.Key;
						break;
					}
					SetMessage(translateMessageTitle.Values.FindTagText("Overlaped") ?? "そのデータは既にアップされています。", 2f);
				}
				else
				{
					SetMessage(translateMessageTitle.Values.FindTagText("Overlaped") ?? "そのデータは既にアップされています。", 2f);
				}
				yield break;
			}
		}
		resultChangeUpdata = -1;
		if (pid != -1)
		{
			if ((bool)objNoControl)
			{
				objNoControl.SetActiveIfDifferent(false);
			}
			msg = translateMessageTitle.Values.FindTagText("Overlaped") ?? "既にアップ済みのキャラです。\r\n現在選択しているキャラに置き換えますか？";
			Utils.Sound.Play(SystemSE.window_o);
			chk = Observable.FromCoroutine((IObserver<bool> res) => popupCheck.CheckAnswerCor(res, msg)).ToYieldInstruction(false);
			yield return chk;
			if (!chk.Result)
			{
				yield break;
			}
			resultChangeUpdata = 1;
			if ((bool)objNoControl)
			{
				objNoControl.SetActiveIfDifferent(true);
			}
		}
		bool mod = false;
		ChaFileControl chaFileControl = new ChaFileControl();
		chaFileControl.skipRangeCheck = true;
		chaFileControl.LoadCharaFile(cmp.info.FullPath, (byte)sex);
		chaFileControl.skipRangeCheck = false;
		mod = ChaFileControl.CheckDataRange(chaFileControl, true, true, true);
		string encHN = Convert.ToBase64String(Encoding.UTF8.GetBytes(handleName));
		string encComment = Convert.ToBase64String(Encoding.UTF8.GetBytes(comment));
		string encCharaName = Convert.ToBase64String(Encoding.UTF8.GetBytes(cmp.info.name));
		string encNickName = Convert.ToBase64String(Encoding.UTF8.GetBytes(cmp.info.nickname));
		string birthDay = cmp.info.birthDay_Origin;
		string encBirthDay = Convert.ToBase64String(Encoding.UTF8.GetBytes(birthDay));
		if (uploadDebug)
		{
			encHN = handleName;
			encComment = comment;
			encCharaName = cmp.info.name;
			encNickName = cmp.info.nickname;
			encBirthDay = birthDay;
		}
		int trial = 0;
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 3);
		wwwform.AddField("pid", pid);
		wwwform.AddField("uid", uid);
		wwwform.AddBinaryData("png", data);
		wwwform.AddField("trial", trial);
		wwwform.AddField("sex", (!mod) ? cmp.info.sex : 99);
		wwwform.AddField("height", cmp.info.height);
		wwwform.AddField("bust", cmp.info.bustSize);
		wwwform.AddField("hair", cmp.info.hair);
		wwwform.AddField("personality", cmp.info.personality);
		wwwform.AddField("blood", cmp.info.bloodType);
		wwwform.AddField("birthday", encBirthDay);
		wwwform.AddField("club", cmp.info.club);
		wwwform.AddField("chara_name", encCharaName);
		wwwform.AddField("nickname", encNickName);
		wwwform.AddField("handlename", encHN);
		wwwform.AddField("products_description", encComment);
		wwwform.AddField("hash_png", hashPng);
		wwwform.AddField("hash_param", hashParam);
		ObservableYieldInstruction<string> o = ObservableWWW.Post(UploaderURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
		yield return o;
		if (o.HasError)
		{
			SetMessage(translateMessageTitle.Values.FindTagText("FailedNet") ?? "ネットワークの接続に失敗しました", 2f);
		}
		else if (o.HasResult)
		{
			int key = int.Parse(o.Result);
			dictLocalHash[key] = new ServerHashInfo(hashPng, hashParam, uid);
			if (resultChangeUpdata == 1)
			{
				SetMessage(translateMessageTitle.Values.FindTagText("Changed") ?? "キャラデータを変更しました", 2f);
				dictLocalHash.Remove(pid);
				dictServerHash.Remove(pid);
			}
			else
			{
				SetMessage(translateMessageTitle.Values.FindTagText("Success") ?? "キャラデータをアップロードしました", 2f);
			}
		}
	}

	private void Start()
	{
		Singleton<Scene>.Instance.sceneFade.SortingOrder();
		UploaderURL = CreateURL.Load_KK_Cha_URL();
		if (UploaderURL.Contains("192.168.1.89"))
		{
			uploadDebug = true;
		}
		string path = UserData.Path + "/regulation.rnf";
		if (File.Exists(path))
		{
			btnUpload.gameObject.SetActiveIfDifferent(false);
		}
		CreateCharaList();
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
				objSexTop[0].SetActiveIfDifferent(0 == p.index);
				objSexTop[1].SetActiveIfDifferent(1 == p.index);
			});
		});
		inpHN.OnEndEditAsObservable().Subscribe(delegate
		{
			textHNDummy.text = inpHN.text;
			SaveHandleName();
		});
		inpComment.OnEndEditAsObservable().Subscribe(delegate
		{
			textCommentDummy.text = inpComment.text;
		});
		btnGotoDownload.OnClickAsObservable().Subscribe(delegate
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
						rootComponent.nextSceneName = "Downloader";
					}
				}
			};
			Singleton<Scene>.Instance.LoadReserve(data, true);
		});
		btnGotoTitle.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			cvsChangeScene.gameObject.SetActive(true);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "Title",
				isFade = true
			}, false);
		});
		btnPolicy.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.window_o);
			if ((bool)objPolicy)
			{
				objPolicy.SetActive(true);
			}
		});
		btnExitPolicy.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			if ((bool)objPolicy)
			{
				objPolicy.SetActive(false);
			}
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			where btnExitPolicy.gameObject.activeInHierarchy
			select _).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			if ((bool)objPolicy)
			{
				objPolicy.SetActive(false);
			}
		});
		btnUpload.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.ok_s);
			objNoControl.SetActiveIfDifferent(true);
			Observable.FromCoroutine(UploadCor).Subscribe(delegate
			{
			}, delegate
			{
				objNoControl.SetActiveIfDifferent(false);
			}, delegate
			{
				objNoControl.SetActiveIfDifferent(false);
			});
		});
		Observable.EveryUpdate().Subscribe(delegate
		{
			int[] array = null;
			array = ((!tglSex[0].isOn) ? fileListCtrl[1].GetSelectIndex() : fileListCtrl[0].GetSelectIndex());
			btnUpload.interactable = !array.IsNullOrEmpty();
			bool isFocused = inpHN.isFocused;
			objHNDummy.SetActiveIfDifferent(!isFocused);
			if (isFocused && Input.GetKeyDown(KeyCode.Tab))
			{
				inpComment.ActivateInputField();
			}
			bool isFocused2 = inpComment.isFocused;
			objCommentDummy.SetActiveIfDifferent(!isFocused2);
			if (isFocused2 && Input.GetKeyDown(KeyCode.Tab))
			{
				inpHN.ActivateInputField();
			}
			if (0f < msgCount)
			{
				msgCount -= Time.deltaTime;
				msgCount = Mathf.Max(0f, msgCount);
				if (msgCount == 0f)
				{
					DisvisibleMessage();
				}
			}
			if (string.Empty == Singleton<Scene>.Instance.AddSceneName && !objNoControl.activeSelf && !cvsChangeScene.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
			{
				Utils.Scene.GameEnd();
			}
		}).AddTo(this);
		LoadHandleName();
		Observable.FromCoroutine(GetHashAll).Subscribe(delegate
		{
		}, delegate
		{
			objNoControl.SetActiveIfDifferent(false);
		}, delegate
		{
			objNoControl.SetActiveIfDifferent(false);
		});
	}
}
