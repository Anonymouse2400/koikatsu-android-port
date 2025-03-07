using System;
using System.Collections.Generic;
using Illusion.CustomAttributes;
using IllusionUtility.GetUtility;
using UnityEngine;

public class HMotionEyeNeckMale : HMotionEyeNeck
{
	[Serializable]
	public class EyeNeck
	{
		[Label("アニメーション名")]
		public string anim = string.Empty;

		[Label("目の開き")]
		public int openEye;

		[Label("口の開き")]
		public int openMouth;

		[Label("眉")]
		public int eyebrow;

		[Label("目")]
		public int eye;

		[Label("口")]
		public int mouth;

		[Label("涙")]
		public int tears;

		[Label("目首挙動ID")]
		public int idEyeNeck;

		public EyeNeckRange rangeNeck = new EyeNeckRange();

		public EyeNeckRange rangeFace = new EyeNeckRange();
	}

	[SerializeField]
	private Dictionary<string, EyeNeck> dicEyeNeck = new Dictionary<string, EyeNeck>();

	public bool Init(ChaControl _male, ChaControl _female)
	{
		Release();
		chara = _male;
		partner = _female;
		objPartnerBody = ((!partner) ? null : partner.GetReferenceInfo(ChaReference.RefObjKey.ObjBody));
		objPartnerSilhouetteBody = ((!partner) ? null : partner.GetReferenceInfo(ChaReference.RefObjKey.S_SimpleTop));
		LoadEyeNeckPtn();
		objKokan = null;
		if ((bool)_male && (bool)_male.objBodyBone && nameGenital != string.Empty)
		{
			objKokan = _male.objBodyBone.transform.FindLoop(nameGenital);
		}
		return true;
	}

	public void Release()
	{
		dicEyeNeck.Clear();
	}

	public bool Load(string _assetpath, string _file)
	{
		dicEyeNeck.Clear();
		if (_file == string.Empty)
		{
			return false;
		}
		string text = GlobalMethod.LoadAllListText(_assetpath, _file);
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			string text2 = data[i, num++];
			if (!(text2 == string.Empty))
			{
				if (!dicEyeNeck.ContainsKey(text2))
				{
					dicEyeNeck.Add(text2, new EyeNeck());
				}
				EyeNeck eyeNeck = dicEyeNeck[text2];
				eyeNeck.anim = text2;
				eyeNeck.eyebrow = int.Parse(data[i, num++]);
				eyeNeck.eye = int.Parse(data[i, num++]);
				eyeNeck.mouth = int.Parse(data[i, num++]);
				eyeNeck.openEye = int.Parse(data[i, num++]);
				eyeNeck.openMouth = int.Parse(data[i, num++]);
				eyeNeck.tears = int.Parse(data[i, num++]);
				eyeNeck.idEyeNeck = int.Parse(data[i, num++]);
				eyeNeck.rangeNeck.up = float.Parse(data[i, num++]);
				eyeNeck.rangeNeck.down = float.Parse(data[i, num++]);
				eyeNeck.rangeNeck.left = float.Parse(data[i, num++]);
				eyeNeck.rangeNeck.right = float.Parse(data[i, num++]);
				eyeNeck.rangeFace.up = float.Parse(data[i, num++]);
				eyeNeck.rangeFace.down = float.Parse(data[i, num++]);
				eyeNeck.rangeFace.left = float.Parse(data[i, num++]);
				eyeNeck.rangeFace.right = float.Parse(data[i, num++]);
			}
		}
		return true;
	}

	public bool Proc(AnimatorStateInfo _ai, GameObject _objCamera)
	{
		foreach (KeyValuePair<string, EyeNeck> item in dicEyeNeck)
		{
			if (_ai.IsName(item.Key))
			{
				chara.ChangeEyesOpenMax((float)item.Value.openEye * 0.1f);
				FBSCtrlMouth mouthCtrl = chara.mouthCtrl;
				if (mouthCtrl != null)
				{
					mouthCtrl.OpenMin = (float)item.Value.openMouth * 0.1f;
				}
				chara.ChangeEyebrowPtn(item.Value.eyebrow);
				chara.ChangeEyesPtn(item.Value.eye);
				chara.ChangeMouthPtn(item.Value.mouth);
				chara.tearsLv = (byte)item.Value.tears;
				SetEyeNeckPtn(item.Value.idEyeNeck, _objCamera, true, true);
				SetRange(item.Value);
			}
		}
		return true;
	}

	public bool SetRange(EyeNeck _en)
	{
		if (chara.neckLookCtrl == null)
		{
			return false;
		}
		if (chara.neckLookCtrl.neckLookScript == null)
		{
			return false;
		}
		SetRangeInfo(_en, 5);
		SetRangeInfo(_en, 6);
		return true;
	}

	public bool SetRangeInfo(EyeNeck _en, int _ptn)
	{
		if (chara.neckLookCtrl.neckLookScript.neckTypeStates.Length <= _ptn || _ptn < 0)
		{
			return false;
		}
		NeckTypeStateVer2 neckTypeStateVer = chara.neckLookCtrl.neckLookScript.neckTypeStates[_ptn];
		neckTypeStateVer.aParam[0].upBendingAngle = _en.rangeNeck.up;
		neckTypeStateVer.aParam[0].downBendingAngle = _en.rangeNeck.down;
		neckTypeStateVer.aParam[0].minBendingAngle = _en.rangeNeck.left;
		neckTypeStateVer.aParam[0].maxBendingAngle = _en.rangeNeck.right;
		neckTypeStateVer.aParam[1].upBendingAngle = _en.rangeFace.up;
		neckTypeStateVer.aParam[1].downBendingAngle = _en.rangeFace.down;
		neckTypeStateVer.aParam[1].minBendingAngle = _en.rangeFace.left;
		neckTypeStateVer.aParam[1].maxBendingAngle = _en.rangeFace.right;
		return true;
	}
}
