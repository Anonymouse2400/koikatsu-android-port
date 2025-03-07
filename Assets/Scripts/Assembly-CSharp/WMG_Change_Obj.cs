using System;
using UnityEngine;

public class WMG_Change_Obj
{
	public delegate void ObjChangedHandler();

	public bool changesPaused;

	public bool changePaused;

	public event ObjChangedHandler OnChange;

	public void Changed()
	{
		ObjChangedHandler objChangedHandler = this.OnChange;
		if (objChangedHandler != null && changeOk())
		{
			objChangedHandler();
		}
	}

	private bool changeOk()
	{
		if (!Application.isPlaying)
		{
			return false;
		}
		if (changesPaused)
		{
			changePaused = true;
			return false;
		}
		return true;
	}

	public void UnsubscribeAllHandlers()
	{
		if (this.OnChange != null)
		{
			Delegate[] invocationList = this.OnChange.GetInvocationList();
			foreach (Delegate @delegate in invocationList)
			{
				OnChange -= @delegate as ObjChangedHandler;
			}
		}
	}
}
