using System;
using System.Collections.Generic;
using Illusion.CustomAttributes;
using UnityEngine;

public class SiruPasteCtrl : MonoBehaviour
{
	[Serializable]
	public class Timing
	{
		[Label("タイミング")]
		public float normalizeTime;

		public List<int> lstWhere = new List<int>();
	}

	[Serializable]
	public class PasteInfo
	{
		[Label("アニメーション名")]
		public string anim = string.Empty;

		public List<Timing> lstTiming = new List<Timing>();
	}

	[SerializeField]
	private List<PasteInfo> lstPaste = new List<PasteInfo>();

	[SerializeField]
	private ChaControl female;

	private float oldFrame;

	private int oldHash;

	public bool Init(ChaControl _female)
	{
		Release();
		female = _female;
		return true;
	}

	public void Release()
	{
		lstPaste.Clear();
	}

	public bool Load(string _assetpath, string _file)
	{
		lstPaste.Clear();
		if (_file == string.Empty)
		{
			return false;
		}
		TextAsset textAsset = GlobalMethod.LoadAllFolderInOneFile<TextAsset>(_assetpath, _file);
		if (textAsset == null || textAsset.text == string.Empty)
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(textAsset.text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			PasteInfo pasteInfo = new PasteInfo();
			pasteInfo.anim = data[i, 0];
			for (int j = 1; j < length2; j += 2)
			{
				Timing timing = new Timing();
				string text = data[i, j];
				if (text == string.Empty)
				{
					break;
				}
				timing.normalizeTime = float.Parse(text);
				string[] array = data[i, j + 1].Split(',');
				for (int k = 0; k < array.Length; k++)
				{
					timing.lstWhere.Add(GlobalMethod.GetIntTryParse(array[k]));
				}
				pasteInfo.lstTiming.Add(timing);
			}
			lstPaste.Add(pasteInfo);
		}
		oldFrame = 0f;
		oldHash = 0;
		return true;
	}

	public bool Proc(AnimatorStateInfo _ai)
	{
		if (!female)
		{
			return false;
		}
		if (oldHash != _ai.shortNameHash)
		{
			oldFrame = 0f;
		}
		oldHash = _ai.shortNameHash;
		for (int i = 0; i < lstPaste.Count; i++)
		{
			PasteInfo pasteInfo = lstPaste[i];
			if (!_ai.IsName(pasteInfo.anim))
			{
				continue;
			}
			for (int j = 0; j < pasteInfo.lstTiming.Count; j++)
			{
				Timing timing = pasteInfo.lstTiming[j];
				if (!(oldFrame > timing.normalizeTime) && !(timing.normalizeTime >= _ai.normalizedTime))
				{
					for (int k = 0; k < timing.lstWhere.Count; k++)
					{
						ChaFileDefine.SiruParts parts = (ChaFileDefine.SiruParts)timing.lstWhere[k];
						female.SetSiruFlags(parts, (byte)Mathf.Min(female.GetSiruFlags(parts) + 1, 2));
					}
				}
			}
			oldFrame = _ai.normalizedTime;
			return true;
		}
		return true;
	}
}
