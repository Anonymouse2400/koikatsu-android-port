using System.Collections.Generic;
using UnityEngine;

public class DynamicBoneReferenceCtrl : MonoBehaviour
{
	private class RateInfo
	{
		public Vector3 sizeS;

		public Vector3 sizeM;

		public Vector3 sizeL;

		public Vector3 Calc(float _rate)
		{
			return (!(_rate >= 0.5f)) ? Vector3.Lerp(sizeS, sizeM, Mathf.InverseLerp(0f, 0.5f, _rate)) : Vector3.Lerp(sizeM, sizeL, Mathf.InverseLerp(0.5f, 1f, _rate));
		}
	}

	private class Reference
	{
		public RateInfo position = new RateInfo();

		public RateInfo rotation = new RateInfo();

		public RateInfo scale = new RateInfo();
	}

	private bool isInit;

	private ChaControl chaFemale;

	private DynamicBone_Ver02.BonePtn[] bonePtns = new DynamicBone_Ver02.BonePtn[2];

	private List<Transform>[] lstsTrans = new List<Transform>[2];

	private List<Reference>[] lstsRef = new List<Reference>[2];

	private List<RateInfo>[] lstsRateInfo = new List<RateInfo>[2];

	private DynamicBone_Ver02[] dynamics = new DynamicBone_Ver02[2];

	public bool IsInit
	{
		get
		{
			return isInit;
		}
		set
		{
			isInit = value;
		}
	}

	public bool Init(ChaControl _female)
	{
		chaFemale = _female;
		if (chaFemale == null)
		{
			return false;
		}
		dynamics[0] = chaFemale.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
		dynamics[1] = chaFemale.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
		for (int i = 0; i < 2; i++)
		{
			lstsRef[i] = new List<Reference>();
			bonePtns[i] = new DynamicBone_Ver02.BonePtn();
			lstsTrans[i] = new List<Transform>();
			lstsRateInfo[i] = new List<RateInfo>();
			if (dynamics[i] != null && dynamics[i].Patterns.Count > 0)
			{
				bonePtns[i] = dynamics[i].Patterns[0];
				for (int j = 0; j < bonePtns[i].Params.Count; j++)
				{
					lstsTrans[i].Add(bonePtns[i].Params[j].RefTransform);
				}
			}
		}
		return true;
	}

	public bool Load(string _assetpath, string _file)
	{
		List<bool>[] array = new List<bool>[2];
		List<bool>[] array2 = new List<bool>[2];
		isInit = false;
		for (int i = 0; i < 2; i++)
		{
			InitDynamicBoneReferenceBone(dynamics[i], lstsTrans[i]);
			if (lstsRef[i] != null)
			{
				lstsRef[i].Clear();
			}
			array[i] = new List<bool>();
			array2[i] = new List<bool>();
			lstsRateInfo[i] = new List<RateInfo>();
		}
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
		for (int j = 0; j < length; j += 3)
		{
			for (int k = 0; k < 3; k++)
			{
				Reference reference = new Reference();
				int num = j + k;
				int num2 = 0;
				reference.position.sizeS = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.position.sizeM = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.position.sizeL = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.rotation.sizeS = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.rotation.sizeM = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.rotation.sizeL = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.scale.sizeS = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.scale.sizeM = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				reference.scale.sizeL = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]));
				int num3 = j / 3;
				array[num3].Add(data[num, num2++] == "1");
				array2[num3].Add(data[num, num2++] == "1");
				lstsRef[num3].Add(reference);
				if (num2 < length2)
				{
					lstsRateInfo[num3].Add(new RateInfo
					{
						sizeS = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++])),
						sizeL = new Vector3(float.Parse(data[num, num2++]), float.Parse(data[num, num2++]), float.Parse(data[num, num2++]))
					});
				}
			}
		}
		for (int l = 0; l < 2; l++)
		{
			array[l].Add(false);
			array2[l].Add(false);
		}
		for (int m = 0; m < 2; m++)
		{
			SetDynamicBoneRotationCalc(dynamics[m], array[m]);
			SetDynamicBoneMoveLimitFlag(dynamics[m], array2[m]);
			SetDynamicBoneMoveLimit(dynamics[m], lstsRateInfo[m]);
		}
		isInit = true;
		return true;
	}

	public bool Proc()
	{
		if (!isInit)
		{
			return false;
		}
		float shapeBodyValue = chaFemale.GetShapeBodyValue(4);
		for (int i = 0; i < 2; i++)
		{
			if (lstsTrans[i].Count != lstsRef[i].Count)
			{
				continue;
			}
			for (int j = 0; j < lstsRef[i].Count; j++)
			{
				if (!(lstsTrans[i][j] == null))
				{
					lstsTrans[i][j].localPosition = lstsRef[i][j].position.Calc(shapeBodyValue);
					lstsTrans[i][j].localRotation = Quaternion.Euler(lstsRef[i][j].rotation.Calc(shapeBodyValue));
					lstsTrans[i][j].localScale = lstsRef[i][j].scale.Calc(shapeBodyValue);
				}
			}
		}
		return true;
	}

	public bool InitDynamicBoneReferenceBone()
	{
		if (!isInit)
		{
			return false;
		}
		for (int i = 0; i < 2; i++)
		{
			InitDynamicBoneReferenceBone(dynamics[i], lstsTrans[i]);
		}
		return true;
	}

	private bool InitDynamicBoneReferenceBone(DynamicBone_Ver02 _dynamic, List<Transform> _lstTrans)
	{
		if (_dynamic == null || _lstTrans == null)
		{
			return false;
		}
		List<bool> list = new List<bool>();
		list.Add(false);
		list.Add(true);
		list.Add(false);
		list.Add(false);
		List<bool> list2 = list;
		for (int i = 0; i < list2.Count; i++)
		{
			_dynamic.SetRotationCalcParams(0, i, list2[i]);
		}
		list = new List<bool>();
		list.Add(false);
		list.Add(false);
		list.Add(false);
		list.Add(false);
		List<bool> list3 = list;
		for (int j = 0; j < list3.Count; j++)
		{
			_dynamic.SetMoveLimitParams(0, j, list3[j]);
		}
		foreach (Transform _lstTran in _lstTrans)
		{
			if (!(_lstTran == null))
			{
				_lstTran.localPosition = Vector3.zero;
				_lstTran.localRotation = Quaternion.identity;
				_lstTran.localScale = Vector3.one;
			}
		}
		return true;
	}

	private bool InitDynamicBoneReferenceBone(DynamicBone_Ver02.BonePtn _ptn, List<Transform> _lstTrans)
	{
		if (_ptn == null || _lstTrans == null)
		{
			return false;
		}
		List<bool> list = new List<bool>();
		list.Add(false);
		list.Add(true);
		list.Add(false);
		list.Add(false);
		List<bool> lstBool = list;
		SetDynamicBoneRotationCalc(_ptn, lstBool);
		list = new List<bool>();
		list.Add(false);
		list.Add(false);
		list.Add(false);
		list.Add(false);
		List<bool> lstBool2 = list;
		SetDynamicBoneMoveLimitFlag(_ptn, lstBool2);
		foreach (Transform _lstTran in _lstTrans)
		{
			if (!(_lstTran == null))
			{
				_lstTran.localPosition = Vector3.zero;
				_lstTran.localRotation = Quaternion.identity;
				_lstTran.localScale = Vector3.one;
			}
		}
		return true;
	}

	private bool SetDynamicBoneRotationCalc(DynamicBone_Ver02 _dynamic, List<bool> _lstBool)
	{
		if (_dynamic == null || _lstBool == null)
		{
			return false;
		}
		for (int i = 0; i < _lstBool.Count; i++)
		{
			_dynamic.SetRotationCalcParams(0, i, _lstBool[i]);
		}
		return true;
	}

	private bool SetDynamicBoneMoveLimitFlag(DynamicBone_Ver02 _dynamic, List<bool> _lstBool)
	{
		if (_dynamic == null || _lstBool == null)
		{
			return false;
		}
		for (int i = 0; i < _lstBool.Count; i++)
		{
			_dynamic.SetMoveLimitParams(0, i, _lstBool[i]);
		}
		return true;
	}

	private bool SetDynamicBoneMoveLimit(DynamicBone_Ver02 _dynamic, List<RateInfo> _lstMove)
	{
		if (_dynamic == null || _lstMove == null)
		{
			return false;
		}
		for (int i = 0; i < _lstMove.Count; i++)
		{
			_dynamic.SetMoveLimitData(0, i, _lstMove[i].sizeS, _lstMove[i].sizeL);
		}
		return true;
	}

	private bool SetDynamicBoneRotationCalc(DynamicBone_Ver02.BonePtn _ptn, List<bool> _lstBool)
	{
		if (_ptn == null || _lstBool == null)
		{
			return false;
		}
		if (_lstBool.Count != _ptn.Params.Count)
		{
			return false;
		}
		for (int i = 0; i < _ptn.Params.Count; i++)
		{
			_ptn.Params[i].IsRotationCalc = _lstBool[i];
		}
		for (int j = 0; j < _ptn.ParticlePtns.Count && j < _lstBool.Count; j++)
		{
			_ptn.ParticlePtns[j].IsRotationCalc = _lstBool[j];
		}
		return true;
	}

	private bool SetDynamicBoneMoveLimitFlag(DynamicBone_Ver02.BonePtn _ptn, List<bool> _lstBool)
	{
		if (_ptn == null || _lstBool == null)
		{
			return false;
		}
		if (_lstBool.Count != _ptn.Params.Count)
		{
			return false;
		}
		for (int i = 0; i < _ptn.Params.Count; i++)
		{
			_ptn.Params[i].IsMoveLimit = _lstBool[i];
		}
		for (int j = 0; j < _ptn.ParticlePtns.Count && j < _lstBool.Count; j++)
		{
			_ptn.ParticlePtns[j].IsMoveLimit = _lstBool[j];
		}
		return true;
	}
}
