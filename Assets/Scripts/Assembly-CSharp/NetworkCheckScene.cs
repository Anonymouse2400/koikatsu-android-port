using System.Collections;
using System.Collections.Generic;
using Illusion.Component;
using Illusion.Extensions;
using Localize.Translate;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCheckScene : MonoBehaviour
{
	public string nextSceneName = string.Empty;

	public Text txtInfomation;

	public GameObject objClick;

	public GameObject objMsg;

	private CoroutineAssist caCheck;

	private int nextType = -1;

	private bool changeScene;

	private float startTime;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private Dictionary<int, Data.Param> translateMessageTitle
	{
		get
		{
			return uiTranslater.Get(0);
		}
	}

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.NET_CHECK));
		}
	}

	private void Start()
	{
		ShortcutKey shortcutKey = base.gameObject.AddComponent<ShortcutKey>();
		shortcutKey.procList.Add(new ShortcutKey.Proc
		{
			keyCode = KeyCode.F1
		});
		shortcutKey.procList.Add(new ShortcutKey.Proc
		{
			keyCode = KeyCode.F2
		});
		shortcutKey.procList.Add(new ShortcutKey.Proc
		{
			keyCode = KeyCode.Escape
		});
		caCheck = new CoroutineAssist(this, CheckNetworkStatus);
		caCheck.Start(true, 10f);
		startTime = Time.realtimeSinceStartup;
		Localize.Translate.Manager.BindFont(txtInfomation);
	}

	public IEnumerator CheckNetworkStatus()
	{
		string url = CreateURL.Load_KK_Check_URL();
		if (url.IsNullOrEmpty())
		{
			if ((bool)txtInfomation)
			{
				txtInfomation.text = translateMessageTitle.Values.FindTagText("ServerAccessInfoField") ?? "サーバーへアクセスするための情報の読み込みに失敗しました。";
			}
			caCheck.EndStatus();
			if ((bool)objClick)
			{
				objClick.SetActiveIfDifferent(true);
			}
			if ((bool)objMsg)
			{
				objMsg.SetActiveIfDifferent(false);
			}
			nextType = 2;
			yield break;
		}
		int trial = 0;
		int checkMode = 0;
		checkMode = ((Singleton<Character>.Instance.nextNetworkScene == "RandomNetChara") ? 1 : ((!("Uploader" == nextSceneName)) ? ((!("RankingScene" == nextSceneName)) ? 1 : 2) : 0));
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("mode", checkMode);
		wwwform.AddField("trial", trial);
		WWW www = new WWW(url, wwwform);
		yield return www;
		if (www.error != null)
		{
			if ((bool)txtInfomation)
			{
				txtInfomation.text = translateMessageTitle.Values.FindTagText("ServerAccessField") ?? "サーバーへのアクセスに失敗しました。";
			}
			caCheck.EndStatus();
			if ((bool)objClick)
			{
				objClick.SetActiveIfDifferent(true);
			}
			if ((bool)objMsg)
			{
				objMsg.SetActiveIfDifferent(false);
			}
			nextType = 2;
			yield break;
		}
		string[] array = www.text.Split("\t"[0]);
		if ("0" == array[0])
		{
			nextType = 1;
		}
		else if ("1" == array[0])
		{
			nextType = 0;
			txtInfomation.text = array[1];
			if ((bool)objClick)
			{
				objClick.SetActiveIfDifferent(true);
			}
			if ((bool)objMsg)
			{
				objMsg.SetActiveIfDifferent(false);
			}
		}
		else
		{
			nextType = 2;
			txtInfomation.text = array[1];
			if ((bool)objClick)
			{
				objClick.SetActiveIfDifferent(true);
			}
			if ((bool)objMsg)
			{
				objMsg.SetActiveIfDifferent(false);
			}
		}
		caCheck.EndStatus();
		yield return null;
	}

	private void Update()
	{
		if (nextType == -1)
		{
			int num = Mathf.FloorToInt(Time.realtimeSinceStartup - startTime);
			string text = translateMessageTitle.Values.FindTagText("ServerCheck") ?? "サーバーをチェックしています";
			for (int i = 0; i < num; i++)
			{
				text += "．";
			}
			if ((bool)txtInfomation)
			{
				txtInfomation.text = text;
			}
		}
		if (caCheck != null && caCheck.status == CoroutineAssist.Status.Run && caCheck.TimeOutCheck())
		{
			caCheck.End();
			if ((bool)txtInfomation)
			{
				txtInfomation.text = translateMessageTitle.Values.FindTagText("ServerAccessField") ?? "サーバーへのアクセスに失敗しました。";
			}
			caCheck.EndStatus();
			if ((bool)objClick)
			{
				objClick.SetActiveIfDifferent(true);
			}
			if ((bool)objMsg)
			{
				objMsg.SetActiveIfDifferent(false);
			}
			nextType = 2;
		}
		if (changeScene)
		{
			return;
		}
		if (nextType == 0)
		{
			if (Input.anyKeyDown)
			{
				nextType = 1;
			}
		}
		else if (nextType == 1)
		{
			bool isAdd = false;
			bool isFade = true;
			if ("RandomNetChara" == Singleton<Character>.Instance.nextNetworkScene)
			{
				Singleton<Character>.Instance.netRandChara = null;
				Singleton<Character>.Instance.nextNetworkScene = string.Empty;
				nextSceneName = "RandomNetChara";
				isAdd = true;
				isFade = false;
			}
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				isAdd = isAdd,
				levelName = nextSceneName,
				isFade = isFade
			}, false);
			changeScene = true;
		}
		else if (nextType == 2 && Input.anyKeyDown)
		{
			if ("RankingScene" == nextSceneName)
			{
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = "Title",
					isFade = true
				}, false);
			}
			else
			{
				Singleton<Scene>.Instance.UnLoad();
			}
			Singleton<Character>.Instance.nextNetworkScene = string.Empty;
			changeScene = true;
		}
	}
}
