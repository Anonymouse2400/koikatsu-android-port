using System;
using System.Collections.Generic;
using Illusion.CustomAttributes;
using IllusionUtility.GetUtility;
using UnityEngine;

public class HMotionEyeNeckFemale : HMotionEyeNeck
{
	[Serializable]
	public class EyeNeck
	{
		[Label("アニメーション名")]
		public string anim = string.Empty;

		[Label("コンフィグ 目で見る無視")]
		public bool isConfigEyeDisregard;

		[Label("コンフィグ 首で見る無視")]
		public bool isConfigNeckDisregard;

		public int[] idEyeNecks = new int[4] { -1, -1, -1, -1 };

		[Label("セリフ無視時 目首挙動ID")]
		public int idEyeNeckNoVoice = -1;

		public EyeNeckRange rangeNeck = new EyeNeckRange();

		public EyeNeckRange rangeFace = new EyeNeckRange();
	}

	[SerializeField]
	private Dictionary<string, EyeNeck> dicEyeNeck = new Dictionary<string, EyeNeck>();

	private SaveData.Heroine heroine;

	public bool Init(ChaControl _female, ChaControl _male, SaveData.Heroine _heroine)
	{
		Release();
		chara = _female;
		heroine = _heroine;
		partner = _male;
		objPartnerBody = ((!partner) ? null : partner.GetReferenceInfo(ChaReference.RefObjKey.ObjBody));
		objPartnerSilhouetteBody = ((!partner) ? null : partner.GetReferenceInfo(ChaReference.RefObjKey.S_SimpleTop));
		LoadEyeNeckPtn();
		objKokan = null;
		if ((bool)_female && (bool)_female.objBodyBone && nameGenital != string.Empty)
		{
			objKokan = _female.objBodyBone.transform.FindLoop(nameGenital);
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
				eyeNeck.isConfigEyeDisregard = data[i, num++] == "1";
				eyeNeck.isConfigNeckDisregard = data[i, num++] == "1";
				eyeNeck.idEyeNecks[0] = int.Parse(data[i, num++]);
				eyeNeck.idEyeNecks[1] = int.Parse(data[i, num++]);
				eyeNeck.idEyeNecks[2] = int.Parse(data[i, num++]);
				eyeNeck.idEyeNecks[3] = int.Parse(data[i, num++]);
				eyeNeck.idEyeNeckNoVoice = int.Parse(data[i, num++]);
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

	public bool Proc(AnimatorStateInfo _ai, int _faceVoice, GameObject _objCamera, bool _isConfigEyes, bool _isConfigNeck)
	{
		foreach (KeyValuePair<string, EyeNeck> item in dicEyeNeck)
		{
			if (_ai.IsName(item.Key))
			{
				if (item.Value.idEyeNeckNoVoice != -1 && _faceVoice != -1)
				{
					SetEyeNeckPtn(item.Value.idEyeNeckNoVoice, _objCamera, item.Value.isConfigEyeDisregard, item.Value.isConfigNeckDisregard);
				}
				else
				{
					SetEyeNeckPtn((_faceVoice == -1) ? item.Value.idEyeNecks[(int)heroine.HExperience] : _faceVoice, _objCamera, item.Value.isConfigEyeDisregard, item.Value.isConfigNeckDisregard);
				}
				SetRange(item.Value);
				if (_isConfigEyes && !item.Value.isConfigEyeDisregard)
				{
					SetEyeNeckPtn(1000, _objCamera, false, false);
				}
				if (_isConfigNeck && !item.Value.isConfigNeckDisregard)
				{
					SetEyeNeckPtn(1001, _objCamera, false, false);
				}
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
