using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CheckCharaFile : BaseLoader
{
	public class UploadDataInfo
	{
		public int PID;

		public string HandleName = string.Empty;

		public string UserID = string.Empty;
	}

	public Text txtInfo;

	public Text txtMsg;

	private StringBuilder sb = new StringBuilder();

	private FolderAssist faCharaFile = new FolderAssist();

	private string charadir = string.Empty;

	private string moddir = string.Empty;

	private CoroutineAssist ccModCheck;

	private Dictionary<string, List<string>> dictModInfo = new Dictionary<string, List<string>>();

	private CoroutineAssist ccOutputInfo;

	private string infoURL = string.Empty;

	private int vSyncCountBack;

	private Dictionary<string, UploadDataInfo> dictAll = new Dictionary<string, UploadDataInfo>();

	private void Start()
	{
		if (infoURL == string.Empty)
		{
			infoURL = CreateURL.Load_KK_Cha_URL();
		}
		ccModCheck = new CoroutineAssist(this, ModCheck);
		ccOutputInfo = new CoroutineAssist(this, OutputInfo);
		charadir = UserData.Path + "checkchara/";
		if (!Directory.Exists(charadir))
		{
			Directory.CreateDirectory(charadir);
		}
		moddir = UserData.Path + "checkchara/mod/";
		if (!Directory.Exists(moddir))
		{
			Directory.CreateDirectory(moddir);
		}
		string[] searchPattern = new string[1] { "*.png" };
		faCharaFile.CreateFolderInfoEx(charadir, searchPattern);
		faCharaFile.SortName();
		txtInfo.text = string.Empty;
		txtMsg.text = "準備完了";
		vSyncCountBack = QualitySettings.vSyncCount;
		QualitySettings.vSyncCount = 0;
	}

	private void Update()
	{
		if (ccOutputInfo.TimeOutCheck())
		{
			UpdateMessage("タイムアウト：全リストの取得に失敗", 2);
		}
	}

	private void OnDestroy()
	{
		QualitySettings.vSyncCount = vSyncCountBack;
	}

	public void OnStopCoroutine()
	{
		if (!ccModCheck.IsIdle())
		{
			ccModCheck.End();
			UpdateMessage("作業を中止しました", 0);
		}
		if (!ccOutputInfo.IsIdle())
		{
			ccOutputInfo.End();
			UpdateMessage("作業を中止しました", 0);
		}
	}

	public string CreateInfoText(int type, string msg01 = "", string msg02 = "", string msg03 = "", string msg04 = "", string msg05 = "", string msg06 = "")
	{
		sb.Length = 0;
		switch (type)
		{
		case 0:
			sb.Append("\u3000\u3000総ファイル数：").Append(msg01).Append("\n");
			sb.Append("\u3000\u3000女ファイル数：").Append(msg02).Append("\n");
			sb.Append("\u3000\u3000男ファイル数：").Append(msg03).Append("\n");
			sb.Append("\u3000不明ファイル数：").Append(msg04).Append("\n");
			sb.Append("ＭＯＤファイル数：").Append(msg05).Append("\n");
			break;
		case 1:
			sb.Append("\u3000総ファイル数：").Append(msg01).Append("\n");
			sb.Append("処理ファイル数：").Append(msg02).Append("\n");
			sb.Append("\u3000重複データ数：").Append(msg03).Append("\n");
			break;
		}
		return sb.ToString();
	}

	public void OnModCheck()
	{
		if (ccModCheck.IsIdle() && ccOutputInfo.IsIdle())
		{
			string[] searchPattern = new string[1] { "*.png" };
			faCharaFile.CreateFolderInfoEx(charadir, searchPattern);
			faCharaFile.SortName();
			int fileCount = faCharaFile.GetFileCount();
			txtInfo.text = CreateInfoText(0, fileCount.ToString(), "0", "0", "0", "0", string.Empty);
			UpdateMessage("Modチェック中", 0);
			dictModInfo.Clear();
			ccModCheck.Start();
		}
	}

	private IEnumerator ModCheck()
	{
		int filefig = faCharaFile.GetFileCount();
		ChaFileControl chafile = null;
		int femaleFig = 0;
		int maleFig = 0;
		int unknownFig = 0;
		int modFig = 0;
		Stopwatch sw = new Stopwatch();
		sw.Start();
		for (int i = 0; i < filefig; i++)
		{
			chafile = new ChaFileControl
			{
				skipRangeCheck = true
			};
			bool ret = chafile.LoadCharaFile(faCharaFile.lstFile[i].FullPath);
			chafile.skipRangeCheck = false;
			if (ret)
			{
				if (chafile.parameter.sex == 0)
				{
					maleFig++;
				}
				else
				{
					femaleFig++;
				}
				List<string> list = new List<string>();
				if (ChaFileControl.CheckDataRange(chafile, true, true, true, list))
				{
					string fileName = Path.GetFileName(faCharaFile.lstFile[i].FullPath);
					string destFileName = moddir + fileName;
					File.Move(faCharaFile.lstFile[i].FullPath, destFileName);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(faCharaFile.lstFile[i].FileName);
					dictModInfo[fileNameWithoutExtension] = list;
					modFig++;
				}
			}
			else
			{
				unknownFig++;
			}
			txtInfo.text = CreateInfoText(0, filefig.ToString(), femaleFig.ToString(), maleFig.ToString(), unknownFig.ToString(), modFig.ToString(), string.Empty);
			yield return null;
		}
		sw.Stop();
		UnityEngine.Debug.Log("処理時間" + sw.Elapsed);
		ccModCheck.EndStatus();
		UpdateMessage("Modチェック終了", 0);
	}

	public void OnOutputInfo()
	{
		if (ccModCheck.IsIdle() && ccOutputInfo.IsIdle())
		{
			txtInfo.text = string.Empty;
			UpdateMessage("情報吐き出し開始", 0);
			ccOutputInfo.Start();
		}
	}

	private IEnumerator OutputInfo()
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		UpdateMessage("データを取得しています", 0);
		dictAll.Clear();
		yield return null;
		ccOutputInfo.StartTimeoutCheck(10f);
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", 1);
		WWW www = new WWW(infoURL, wwwform);
		yield return www;
		ccOutputInfo.EndTimeoutCheck();
		yield return null;
		UploadDataInfo dldh = null;
		if (www.error != null)
		{
			UpdateMessage("WWW Error : " + www.error, 2);
		}
		else
		{
			UpdateMessage("WWW OK", 0);
			string[] array = www.text.Split("\n"[0]);
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (string.Empty == text)
				{
					break;
				}
				string[] array3 = text.Split("\t"[0]);
				dldh = new UploadDataInfo();
				if (array3.Length != 0 && string.Empty != array3[0])
				{
					dldh.PID = int.Parse(array3[0]);
				}
				if (12 <= array3.Length && string.Empty != array3[11])
				{
					dldh.HandleName = Encoding.UTF8.GetString(Convert.FromBase64String(array3[11]));
				}
				if (14 <= array3.Length && string.Empty != array3[13])
				{
					dldh.UserID = array3[13];
				}
				string key = "koikatu" + dldh.PID.ToString("0000000");
				dictAll[key] = dldh;
			}
		}
		yield return null;
		string[] searchPattern = new string[1] { "*.png" };
		faCharaFile.CreateFolderInfoEx(moddir, searchPattern);
		faCharaFile.SortName();
		int fileCount = faCharaFile.GetFileCount();
		if (fileCount != 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			for (int j = 0; j < fileCount; j++)
			{
				stringBuilder.Length = 0;
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(faCharaFile.lstFile[j].FileName);
				stringBuilder.Append(fileNameWithoutExtension);
				if (dictAll.TryGetValue(fileNameWithoutExtension, out dldh))
				{
					stringBuilder.Append("\t").Append(dldh.UserID);
				}
				List<string> value = null;
				if (dictModInfo.TryGetValue(fileNameWithoutExtension, out value))
				{
					foreach (string item in value)
					{
						stringBuilder.Append("\t").Append(item);
					}
				}
				stringBuilder.Append("\n");
				list.Add(stringBuilder.ToString());
			}
			string path = moddir + "modinfo.txt";
			using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
				{
					for (int k = 0; k < list.Count; k++)
					{
						streamWriter.Write(list[k]);
					}
				}
			}
		}
		sw.Stop();
		UpdateMessage("処理時間" + sw.Elapsed, 0);
		ccOutputInfo.EndStatus();
	}

	private void UpdateMessage(string msg, byte logtype)
	{
		switch (logtype)
		{
		case 0:
			UnityEngine.Debug.Log(msg);
			break;
		case 1:
			UnityEngine.Debug.LogWarning(msg);
			break;
		default:
			UnityEngine.Debug.LogError(msg);
			break;
		}
		if ((bool)txtMsg)
		{
			txtMsg.text = msg;
		}
	}
}
