using System;
using System.Collections;
using UnityEngine;

public class CoroutineAssist
{
	public enum Status
	{
		Idle = 0,
		Run = 1,
		Pause = 2
	}

	private MonoBehaviour mono;

	private Func<IEnumerator> func;

	private IEnumerator nowFunc;

	private float timeoutTime;

	private bool enableTimeout;

	private float startTime;

	public Status status { get; set; }

	public CoroutineAssist(MonoBehaviour _mono, Func<IEnumerator> _func)
	{
		nowFunc = null;
		status = Status.Idle;
		timeoutTime = 0f;
		enableTimeout = false;
		startTime = 0f;
		mono = _mono;
		func = _func;
	}

	public bool IsIdle()
	{
		return status == Status.Idle;
	}

	public bool IsRun()
	{
		return status == Status.Run;
	}

	public bool IsPause()
	{
		return status == Status.Pause;
	}

	public bool Start(bool _enableTimeout = false, float _timeout = 20f)
	{
		if (func == null)
		{
			return false;
		}
		if (status != 0)
		{
			return false;
		}
		nowFunc = func();
		status = Status.Run;
		if (_enableTimeout)
		{
			StartTimeoutCheck(_timeout);
		}
		mono.StartCoroutine(nowFunc);
		return true;
	}

	public bool Restart()
	{
		if (nowFunc == null)
		{
			return false;
		}
		mono.StartCoroutine(nowFunc);
		status = Status.Run;
		startTime = Time.realtimeSinceStartup;
		return true;
	}

	public void Pause()
	{
		if (nowFunc != null)
		{
			mono.StopCoroutine(nowFunc);
			status = Status.Pause;
		}
	}

	public void End()
	{
		if (nowFunc != null)
		{
			mono.StopCoroutine(nowFunc);
			EndStatus();
		}
	}

	public void EndStatus()
	{
		nowFunc = null;
		status = Status.Idle;
	}

	public void StartTimeoutCheck(float _timeout = 20f)
	{
		enableTimeout = true;
		timeoutTime = _timeout;
		startTime = Time.realtimeSinceStartup;
	}

	public void EndTimeoutCheck()
	{
		enableTimeout = false;
		timeoutTime = 0f;
		startTime = 0f;
	}

	public bool TimeOutCheck()
	{
		if (status != Status.Run)
		{
			return false;
		}
		if (!enableTimeout)
		{
			return false;
		}
		float num = Time.realtimeSinceStartup - startTime;
		if (num > timeoutTime)
		{
			enableTimeout = false;
			End();
			return true;
		}
		return false;
	}
}
