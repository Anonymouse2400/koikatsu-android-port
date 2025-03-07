using System;
using UnityEngine;

[Serializable]
public class FBSTargetInfo
{
	[Serializable]
	public class CloseOpen
	{
		public int Close;

		public int Open;
	}

	public GameObject ObjTarget;

	public CloseOpen[] PtnSet;

	private SkinnedMeshRenderer smrTarget;

	public void SetSkinnedMeshRenderer()
	{
		if ((bool)ObjTarget)
		{
			smrTarget = ObjTarget.GetComponent<SkinnedMeshRenderer>();
		}
	}

	public SkinnedMeshRenderer GetSkinnedMeshRenderer()
	{
		return smrTarget;
	}

	public void Clear()
	{
		ObjTarget = null;
		PtnSet = null;
		smrTarget = null;
	}
}
