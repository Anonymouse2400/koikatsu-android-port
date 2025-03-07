using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Illusion.Component;
using Illusion.Extensions;
using Localize.Translate;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RandomNetChara : MonoBehaviour
{
	[SerializeField]
	private Canvas cvsChangeScene;

	[SerializeField]
	private GameObject objNoControl;

	[SerializeField]
	private GameObject objMessage;

	[SerializeField]
	private Text txtMessage;

	private float msgCount;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	private readonly string[] infoMessages = new string[2] { "キャラ情報を取得しています", "キャラ情報の取得に失敗しました" };

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.NET_CHECK));
		}
	}

	private string GetMessage(int index)
	{
		string title = infoMessages[index];
		uiTranslater.Get(1).Values.ToArray("CharaInfoCheck").SafeProc(index, delegate(string text)
		{
			title = text;
		});
		return title;
	}

	private IEnumerator Start()
	{
		ShortcutKey shortcutKey = base.gameObject.AddComponent<ShortcutKey>();
		ShortcutKey.Proc temp = new ShortcutKey.Proc
		{
			keyCode = KeyCode.F1,
			enabled = false
		};
		shortcutKey.procList.Add(temp);
		temp = new ShortcutKey.Proc
		{
			keyCode = KeyCode.F2,
			enabled = false
		};
		shortcutKey.procList.Add(temp);
		temp = new ShortcutKey.Proc
		{
			keyCode = KeyCode.F5,
			enabled = false
		};
		shortcutKey.procList.Add(temp);
		temp = new ShortcutKey.Proc
		{
			keyCode = KeyCode.F11,
			enabled = false
		};
		shortcutKey.procList.Add(temp);
		temp = new ShortcutKey.Proc
		{
			keyCode = KeyCode.Escape,
			enabled = false
		};
		shortcutKey.procList.Add(temp);
		Observable.EveryUpdate().Subscribe(delegate
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
		}).AddTo(this);
		int num = Singleton<Character>.Instance.netRandCharaNum;
		ObservableYieldInstruction<ChaFileControl[]> o = Observable.FromCoroutine((IObserver<ChaFileControl[]> res) => GetRandomFemaleNetworkCoroutine(res, num)).ToYieldInstruction(false);
		yield return o;
		if (!o.HasError && o.HasResult)
		{
			Singleton<Character>.Instance.netRandChara = o.Result;
		}
		Observable.FromCoroutine(ReturnCor).ToYieldInstruction();
		yield return null;
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

	public IEnumerator GetRandomFemaleNetworkCoroutine(IObserver<ChaFileControl[]> observer, int num)
	{
		string UploaderURL = CreateURL.Load_KK_Cha_URL();
		SetMessage(GetMessage(0), -1f);
		if (Singleton<Character>.Instance.lstProductId == null)
		{
			Singleton<Character>.Instance.lstProductId = new List<string>();
		}
		WWWForm wwwform = null;
		if (Singleton<Character>.Instance.lstProductId.Count == 0)
		{
			wwwform = new WWWForm();
			wwwform.AddField("mode", 7);
			ObservableYieldInstruction<string> getid = ObservableWWW.Post(UploaderURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
			yield return getid;
			if (getid.HasError)
			{
				SetMessage(GetMessage(1), 3f);
				observer.OnError(getid.Error);
				yield break;
			}
			if (getid.HasResult)
			{
				string[] array = getid.Result.Split("\n"[0]);
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (string.Empty == text)
					{
						break;
					}
					Singleton<Character>.Instance.lstProductId.Add(text);
				}
			}
		}
		Singleton<Character>.Instance.lstProductId = Singleton<Character>.Instance.lstProductId.Shuffle().ToList();
		int fig = Mathf.Min(Singleton<Character>.Instance.lstProductId.Count(), num);
		if (fig == 0)
		{
			int num2 = 1;
			SetMessage(GetMessage(num2), 3f);
			observer.OnError(new Exception(infoMessages[num2]));
			yield break;
		}
		string strPID = string.Empty;
		for (int j = 0; j < fig; j++)
		{
			if (!Singleton<Character>.Instance.lstProductId[j].IsNullOrEmpty())
			{
				strPID += Singleton<Character>.Instance.lstProductId[j];
				strPID += "\n";
			}
		}
		if (string.Empty != strPID)
		{
			wwwform = new WWWForm();
			wwwform.AddField("mode", 8);
			wwwform.AddField("pid", strPID);
			ObservableYieldInstruction<string> o = ObservableWWW.Post(UploaderURL, wwwform).Timeout(TimeSpan.FromSeconds(20.0)).ToYieldInstruction(false);
			yield return o;
			if (o.HasError)
			{
				SetMessage(GetMessage(1), 3f);
				observer.OnError(o.Error);
				yield break;
			}
			if (o.HasResult)
			{
				string[] array3 = o.Result.Split("\n"[0]);
				List<ChaFileControl> list = new List<ChaFileControl>();
				string[] array4 = array3;
				foreach (string text2 in array4)
				{
					if (string.Empty == text2)
					{
						break;
					}
					byte[] bytes = Convert.FromBase64String(text2);
					ChaFileControl chaFileControl = new ChaFileControl();
					if (chaFileControl.LoadFromBytes(bytes))
					{
						list.Add(chaFileControl);
					}
				}
				observer.OnNext(list.ToArray());
				observer.OnCompleted();
			}
			DisvisibleMessage();
			yield return null;
		}
		else
		{
			int num3 = 1;
			SetMessage(GetMessage(num3), 3f);
			observer.OnError(new Exception(infoMessages[num3]));
		}
	}

	private IEnumerator ReturnCor()
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
		Singleton<Scene>.Instance.UnLoad();
		Singleton<Scene>.Instance.UnLoad();
		yield return null;
	}
}
