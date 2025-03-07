using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class YureCtrl
{
	[Serializable]
	public class BreastShapeInfo
	{
		public bool left = true;

		public bool right = true;
	}

	[Serializable]
	public class Info
	{
		public bool[] isActives = new bool[4];

		public BreastShapeInfo[] breastShapes = new BreastShapeInfo[9];
	}

	public Dictionary<string, Info> dicInfo = new Dictionary<string, Info>();

	public bool isInit;

	[Tooltip("動いているかの確認用")]
	public bool[] isActives = new bool[4] { true, true, true, true };

	[Tooltip("動いているかの確認用")]
	public BreastShapeInfo[] breastShapes = new BreastShapeInfo[9];

	private ChaControl chaFemale;

	private bool[] yureEnableActives = new bool[4] { true, true, true, true };

	private BreastShapeInfo[] breastShapeEnables = new BreastShapeInfo[9];

	public bool Init(ChaControl _female)
	{
		chaFemale = _female;
		for (int i = 0; i < 9; i++)
		{
			breastShapes[i] = new BreastShapeInfo();
			breastShapeEnables[i] = new BreastShapeInfo();
		}
		isInit = false;
		return true;
	}

	public bool Release()
	{
		isInit = false;
		dicInfo.Clear();
		return true;
	}

	public bool LoadAllExcel(string _assetpath, string _file)
	{
		isInit = false;
		dicInfo.Clear();
		if ((bool)chaFemale)
		{
			ResetShape();
		}
		if (_file == string.Empty)
		{
			return false;
		}
		List<YureExcel> list = GlobalMethod.LoadAllFolder<YureExcel>(_assetpath, _file);
		foreach (YureExcel item in list)
		{
			LoadParam(item.param);
		}
		if (list.Count != 0)
		{
			isInit = true;
		}
		return true;
	}

	public bool LoadExcel(string _assetpath, string _file)
	{
		isInit = false;
		dicInfo.Clear();
		if ((bool)chaFemale)
		{
			ResetShape();
		}
		if (_file == string.Empty)
		{
			return false;
		}
		YureExcel yureExcel = CommonLib.LoadAsset<YureExcel>(_assetpath, _file, false, string.Empty);
		AssetBundleManager.UnloadAssetBundle(_assetpath, true);
		if (yureExcel == null)
		{
			return false;
		}
		LoadParam(yureExcel.param);
		isInit = true;
		return true;
	}

	public bool LoadParam(List<YureExcel.Param> _param)
	{
		foreach (YureExcel.Param item in _param)
		{
			Info value;
			if (!dicInfo.TryGetValue(item.name, out value))
			{
				dicInfo.Add(item.name, new Info());
				value = dicInfo[item.name];
			}
			bool[] array = new bool[4] { item.Mune_L, item.Mune_R, item.Siri_L, item.Siri_R };
			for (int i = 0; i < 4; i++)
			{
				value.isActives[i] = array[i];
			}
			bool[] array2 = new bool[9] { item.UDPos_L, item.LRRot_L, item.LRPos_L, item.UDRot_L, item.Sharpness_L, item.Shape_L, item.Bulge_NipL, item.Thickness_NipL, item.Sharpness_NipL };
			bool[] array3 = new bool[9] { item.UDPos_R, item.LRRot_R, item.LRPos_R, item.UDRot_R, item.Sharpness_R, item.Shape_R, item.Bulge_NipR, item.Thickness_NipR, item.Sharpness_NipR };
			for (int j = 0; j < 9; j++)
			{
				value.breastShapes[j] = new BreastShapeInfo
				{
					left = array2[j],
					right = array3[j]
				};
			}
		}
		return true;
	}

	public bool Proc(string _animation, bool _isNoAniamtionBack = true, int _LR = 2)
	{
		if (!isInit)
		{
			return false;
		}
		Info value;
		if (dicInfo.TryGetValue(_animation, out value))
		{
			Active(value.isActives, _LR);
			Shape(value.breastShapes, _LR);
			return true;
		}
		if (!_isNoAniamtionBack)
		{
			return false;
		}
		Active(yureEnableActives, _LR);
		Shape(breastShapeEnables, _LR);
		return false;
	}

	private void Active(bool[] _aIsActive, int _LR)
	{
		switch (_LR)
		{
		case 0:
			chaFemale.playDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL, _aIsActive[0]);
			isActives[0] = _aIsActive[0];
			return;
		case 1:
			chaFemale.playDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR, _aIsActive[1]);
			isActives[1] = _aIsActive[1];
			return;
		}
		for (int i = 0; i < isActives.Length; i++)
		{
			chaFemale.playDynamicBoneBust((ChaInfo.DynamicBoneKind)i, _aIsActive[i]);
			isActives[i] = _aIsActive[i];
		}
	}

	private void Shape(BreastShapeInfo[] _shapeInfo, int _LR)
	{
		for (int i = 0; i < 9; i++)
		{
			BreastShapeInfo breastShapeInfo = _shapeInfo[i];
			BreastShapeInfo breastShapeInfo2 = breastShapes[i];
			if (_LR == 0 || _LR == 2)
			{
				chaFemale.DisableShapeBodyID(0, i, !breastShapeInfo.left);
				breastShapeInfo2.left = breastShapeInfo.left;
			}
			if (_LR == 1 || _LR == 2)
			{
				chaFemale.DisableShapeBodyID(1, i, !breastShapeInfo.right);
				breastShapeInfo2.right = breastShapeInfo.right;
			}
		}
	}

	public void ResetShape(int _LR = 2)
	{
		Shape(breastShapeEnables, _LR);
		if (chaFemale.dictDynamicBoneBust != null)
		{
			Active(yureEnableActives, _LR);
		}
	}
}
