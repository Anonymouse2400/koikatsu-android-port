using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UniRx;

public class ChaLoadingTask
{
	private class ReserveData
	{
		public ChaControl chaCtrl { get; private set; }

		public bool isAsync { get; private set; }

		public bool reflectStatus { get; private set; }

		public bool isCancel { get; private set; }

		public string name { get; private set; }

		public ReserveData(ChaControl chaCtrl, bool isAsync, bool reflectStatus)
		{
			this.chaCtrl = chaCtrl;
			this.isAsync = isAsync;
			this.reflectStatus = reflectStatus;
			name = chaCtrl.name;
		}

		public void Cancel()
		{
			isCancel = true;
		}
	}

	private List<ReserveData> reserveList = new List<ReserveData>();

	private List<ChaControl> loadEndList = new List<ChaControl>();

	private ReserveData loadingData;

	public bool isLoading
	{
		get
		{
			return loadingData != null;
		}
	}

	public ChaLoadingTask()
	{
		Observable.FromCoroutine(Loading).Subscribe().AddTo(Singleton<Character>.Instance);
	}

	public bool IsEnd(ChaControl ctrl)
	{
		return loadEndList.Remove(ctrl);
	}

	public void Load(ChaControl chaCtrl, bool reflectStatus = false)
	{
		Load(reserveList.Count, chaCtrl, reflectStatus);
	}

	public void Load(int priority, ChaControl chaCtrl, bool reflectStatus = false)
	{
		Reserve(priority, new ChaControl[1] { chaCtrl }, false, reflectStatus);
	}

	public void LoadAsync(ChaControl chaCtrl, bool reflectStatus = false)
	{
		LoadAsync(reserveList.Count, chaCtrl, reflectStatus);
	}

	public void LoadAsync(int priority, ChaControl chaCtrl, bool reflectStatus = false)
	{
		Reserve(priority, new ChaControl[1] { chaCtrl }, true, reflectStatus);
	}

	public void Load(ChaControl[] chaCtrls, bool reflectStatus = false)
	{
		Load(reserveList.Count, chaCtrls, reflectStatus);
	}

	public void Load(int priority, ChaControl[] chaCtrls, bool reflectStatus = false)
	{
		Reserve(priority, chaCtrls, false, reflectStatus);
	}

	public void LoadAsync(ChaControl[] chaCtrls, bool reflectStatus = false)
	{
		LoadAsync(reserveList.Count, chaCtrls, reflectStatus);
	}

	public void LoadAsync(int priority, ChaControl[] chaCtrls, bool reflectStatus = false)
	{
		Reserve(priority, chaCtrls, true, reflectStatus);
	}

	public bool Cancel(ChaControl chaCtrl)
	{
		int num = reserveList.FindIndex((ReserveData item) => item.chaCtrl == chaCtrl);
		if (num != -1)
		{
			reserveList.RemoveAt(num);
			SafeDeleteChara(chaCtrl);
			return true;
		}
		if (IsEnd(chaCtrl))
		{
			SafeDeleteChara(chaCtrl);
			return true;
		}
		if (loadingData != null && loadingData.chaCtrl == chaCtrl)
		{
			loadingData.Cancel();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		reserveList.ForEach(delegate(ReserveData item)
		{
			SafeDeleteChara(item.chaCtrl);
		});
		reserveList.Clear();
		loadEndList.ForEach(delegate(ChaControl item)
		{
			SafeDeleteChara(item);
		});
		loadEndList.Clear();
		if (loadingData != null)
		{
			loadingData.Cancel();
		}
	}

	public void SafeDeleteChara(ChaControl chaCtrl)
	{
		if (Singleton<Character>.IsInstance() && !(chaCtrl == null) && !(chaCtrl.name == "Delete_Reserve"))
		{
			Singleton<Character>.Instance.DeleteChara(chaCtrl);
		}
	}

	private void Reserve(int priority, ChaControl[] chaCtrls, bool isAsync, bool reflectStatus)
	{
		IEnumerable<ReserveData> collection = chaCtrls.Select((ChaControl item) => new ReserveData(item, isAsync, reflectStatus));
		if (priority == reserveList.Count)
		{
			reserveList.AddRange(collection);
		}
		else
		{
			reserveList.InsertRange(priority, collection);
		}
	}

	private IEnumerator LoadEndCheck()
	{
		while (true)
		{
			if (loadingData.chaCtrl == null || loadingData.isCancel)
			{
				throw new OperationCanceledException(loadingData.name);
			}
			if (loadingData.chaCtrl.loadEnd)
			{
				break;
			}
			yield return null;
		}
	}

	private IEnumerator Loading()
	{
		while (true)
		{
			if (Singleton<Scene>.Instance.IsOverlap)
			{
				yield return null;
				continue;
			}
			if (!reserveList.Any())
			{
				yield return null;
				continue;
			}
			loadingData = reserveList[0];
			reserveList.RemoveAt(0);
			ObservableYieldInstruction<Unit> o = Observable.WhenAll(Observable.FromCoroutine((CancellationToken _) => loadingData.chaCtrl.LoadAsync(loadingData.reflectStatus, loadingData.isAsync)), Observable.FromCoroutine((CancellationToken _) => LoadEndCheck())).ToYieldInstruction(false);
			yield return o;
			if (o.HasError || !o.HasResult)
			{
			}
			if (loadingData.isCancel)
			{
				if (loadingData.chaCtrl != null)
				{
					SafeDeleteChara(loadingData.chaCtrl);
				}
			}
			else
			{
				loadEndList.Add(loadingData.chaCtrl);
			}
			loadingData = null;
		}
	}
}
