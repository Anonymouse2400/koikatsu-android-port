using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FBSAssist;
using Illusion.Extensions;
using IllusionUtility.GetUtility;
using IllusionUtility.SetUtility;
using Localize.Translate;
using Manager;
using UnityEngine;

public class ChaControl : ChaInfo
{
	public class MannequinBackInfo
	{
		public int coordinateType;

		public byte[] custom;

		public int eyesPtn;

		public int mouthPtn;

		public float mouthOpen;

		public bool mouthFixed;

		public int neckLook;

		public byte[] eyesInfo;

		public bool mannequin;

		public void Backup(ChaControl chaCtrl)
		{
			coordinateType = chaCtrl.fileStatus.coordinateType;
			custom = chaCtrl.chaFile.GetCustomBytes();
			eyesPtn = chaCtrl.fileStatus.eyesPtn;
			mouthPtn = chaCtrl.fileStatus.mouthPtn;
			mouthOpen = chaCtrl.fileStatus.mouthOpenMax;
			mouthFixed = chaCtrl.fileStatus.mouthFixed;
			neckLook = chaCtrl.fileStatus.neckLookPtn;
		}

		public void Restore(ChaControl chaCtrl)
		{
			chaCtrl.chaFile.SetCustomBytes(custom, ChaFileDefine.ChaFileCustomVersion);
			chaCtrl.Reload(true);
			chaCtrl.ChangeEyesPtn(eyesPtn, false);
			chaCtrl.ChangeMouthPtn(mouthPtn, false);
			chaCtrl.fileStatus.mouthFixed = mouthFixed;
			chaCtrl.ChangeMouthOpenMax(mouthOpen);
			chaCtrl.ChangeLookNeckPtn(neckLook);
			chaCtrl.neckLookCtrl.ForceLateUpdate();
			chaCtrl.eyeLookCtrl.ForceLateUpdate();
			chaCtrl.neckLookCtrl.neckLookScript.skipCalc = false;
			chaCtrl.resetDynamicBoneAll = true;
			chaCtrl.LateUpdateForce();
		}
	}

	public enum BustNormalKind
	{
		NmlBody = 0,
		NmlTop = 1,
		NmlBra = 2,
		NmlTopPA = 3
	}

	public enum FaceTexKind
	{
		inpBase = 0,
		inpSub = 1,
		inpPaint01 = 2,
		inpPaint02 = 3,
		inpCheek = 4,
		inpLipLine = 5,
		inpMole = 6
	}

	public enum BodyTexKind
	{
		inpBase = 0,
		inpSub = 1,
		inpPaint01 = 2,
		inpPaint02 = 3,
		inpSunburn = 4,
		inpNail = 5
	}

	private MannequinBackInfo mannequinBackInfo = new MannequinBackInfo();

	private const string deleteHeadBoneName = "cf_J_N_FaceRoot";

	private const string deleteBodyBoneName = "cf_j_root";

	private readonly Bounds bounds = new Bounds(new Vector3(0f, -0.2f, 0f), new Vector3(2f, 2f, 2f));

	private AssignedAnotherWeights aaWeightsHead;

	private AssignedAnotherWeights aaWeightsBody;

	private Texture texBodyAlphaMask { get; set; }

	private Texture texBraAlphaMask { get; set; }

	public Texture texInnerAlphaMask { get; set; }

	public Dictionary<BustNormalKind, BustNormal> dictBustNormal { get; private set; }

	private byte[] siruNewLv { get; set; }

	public bool enableExpression
	{
		get
		{
			return !(null == base.expression) && base.expression.enable;
		}
		set
		{
			if (null != base.expression)
			{
				base.expression.enable = value;
			}
		}
	}

	public ChaFileCoordinate nowCoordinate { get; private set; }

	public bool notBra { get; private set; }

	public bool notBot { get; private set; }

	public bool notShorts { get; private set; }

	public bool[] hideHairAcs { get; private set; }

	public bool isChangeOfClothesRandom { get; private set; }

	public Dictionary<int, Dictionary<byte, string>> dictStateType { get; private set; }

	private int ShapeFaceNum { get; set; }

	private int ShapeBodyNum { get; set; }

	private ShapeInfoBase sibFace { get; set; }

	private ShapeInfoBase sibBody { get; set; }

	private bool changeShapeBodyMask { get; set; }

	public BustSoft bustSoft { get; private set; }

	public BustGravity bustGravity { get; private set; }

	public bool[] updateCMFaceTex { get; private set; }

	public bool[] updateCMFaceColor { get; private set; }

	public bool[] updateCMFaceLayout { get; private set; }

	public bool[] updateCMBodyTex { get; private set; }

	public bool[] updateCMBodyColor { get; private set; }

	public bool[] updateCMBodyLayout { get; private set; }

	public AudioSource asVoice { get; private set; }

	private AudioAssist fbsaaVoice { get; set; }

	private Texture texHairGloss { get; set; }

	public byte tearsLv
	{
		get
		{
			return base.fileStatus.tearsLv;
		}
		set
		{
			base.fileStatus.tearsLv = value;
		}
	}

	private bool updateAlphaMask { get; set; }

	public void Initialize(byte _sex, bool _hiPoly, GameObject _objRoot, int _id, int _no, ChaFileControl _chaFile = null)
	{
		if (_chaFile == null || _sex != _chaFile.parameter.sex)
		{
		}
		MemberInitializeAll();
		InitializeControlLoadAll();
		InitializeControlFaceAll();
		InitializeControlBodyAll();
		InitializeControlCoordinateAll();
		InitializeControlCustomAll();
		base.objRoot = _objRoot;
		base.chaID = _id;
		base.loadNo = _no;
		base.hiPoly = _hiPoly;
		base.hideMoz = true;
		base.lstCtrl = Singleton<Character>.Instance.chaListCtrl;
		InitBaseCustomTextureBody(_sex);
		InitBaseCustomTextureEtc();
		if (_chaFile == null)
		{
			base.chaFile = new ChaFileControl();
			LoadPreset(_sex, string.Empty);
		}
		else
		{
			base.chaFile = _chaFile;
		}
		base.chaFile.parameter.sex = _sex;
		if (_chaFile == null)
		{
			ChangeCoordinateType(ChaFileDefine.CoordinateType.School01);
		}
		else
		{
			ChangeCoordinateType(false);
		}
		if (_sex == 0)
		{
			base.chaFile.custom.body.shapeValueBody[0] = 0.6f;
		}
		if (_sex == 1)
		{
			base.chaFile.status.visibleSonAlways = false;
		}
		else
		{
			base.chaFile.status.visibleSon = false;
		}
	}

	public void ReleaseAll()
	{
		ReleaseControlLoadAll();
		ReleaseControlFaceAll();
		ReleaseControlBodyAll();
		ReleaseControlCoordinateAll();
		ReleaseControlCustomAll();
		ReleaseInfoAll();
	}

	public void ReleaseObject()
	{
		ReleaseControlLoadObject();
		ReleaseControlFaceObject();
		ReleaseControlBodyObject();
		ReleaseControlCoordinateObject();
		ReleaseControlCustomObject();
		ReleaseInfoObject();
		if (Singleton<Character>.Instance.enableCharaLoadGCClear)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	public void LoadPreset(int _sex, string presetName = "")
	{
		string empty = string.Empty;
		empty = ((!presetName.IsNullOrEmpty()) ? presetName : ((_sex != 0) ? "ill_Default_Female" : "ill_Default_Male"));
		Dictionary<int, ListInfoBase> dictionary = null;
		dictionary = ((_sex != 0) ? base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_sample_f) : base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_sample_m));
		foreach (KeyValuePair<int, ListInfoBase> item in dictionary)
		{
			if (item.Value.GetInfo(ChaListDefine.KeyType.MainData) == empty)
			{
				string path = item.Value.GetInfo(ChaListDefine.KeyType.MainAB);
				Localize.Translate.Manager.DefaultData.GetBundlePath(ref path);
				base.chaFile.LoadFromAssetBundle(path, empty);
				break;
			}
		}
	}

	public void LoadPresetDemo(int type)
	{
		string assetBundleName = "custom/presets_demo.unity3d";
		string assetName = string.Empty;
		switch (type)
		{
		case 0:
			assetName = "ill_Demo_Female";
			break;
		case 1:
			assetName = "ill_Demo_Shop01";
			break;
		case 2:
			assetName = "ill_Demo_Shop02";
			break;
		}
		base.chaFile.LoadFromAssetBundle(assetBundleName, assetName);
	}

	public bool AssignDefaultCoordinate()
	{
		return AssignDefaultCoordinate(base.chaFile);
	}

	public static bool AssignDefaultCoordinate(ChaFileControl chaFile)
	{
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		Dictionary<int, ListInfoBase> dictionary = null;
		dictionary = ((chaFile.parameter.sex != 0) ? chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_default_cos_f) : chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_default_cos_m));
		if (dictionary == null)
		{
			return false;
		}
		int count = dictionary.Keys.Count;
		if (count == 0)
		{
			return false;
		}
		int index = UnityEngine.Random.Range(0, count - 1);
		int key = dictionary.Keys.ToList()[index];
		ListInfoBase value = null;
		if (!dictionary.TryGetValue(key, out value))
		{
			return false;
		}
		string info = value.GetInfo(ChaListDefine.KeyType.MainManifest);
		string info2 = value.GetInfo(ChaListDefine.KeyType.MainAB);
		ChaListDefine.KeyType[] array = new ChaListDefine.KeyType[7]
		{
			ChaListDefine.KeyType.DefCoorde01,
			ChaListDefine.KeyType.DefCoorde02,
			ChaListDefine.KeyType.DefCoorde03,
			ChaListDefine.KeyType.DefCoorde04,
			ChaListDefine.KeyType.DefCoorde05,
			ChaListDefine.KeyType.DefCoorde06,
			ChaListDefine.KeyType.DefCoorde07
		};
		for (int i = 0; i < array.Length; i++)
		{
			string info3 = value.GetInfo(array[i]);
			TextAsset ta = CommonLib.LoadAsset<TextAsset>(info2, info3, false, info);
			chaFile.coordinate[i].LoadFile(ta);
		}
		return true;
	}

	public static ChaFileControl[] GetRandomFemaleCard(int num)
	{
		FolderAssist folderAssist = new FolderAssist();
		string[] searchPattern = new string[1] { "*.png" };
		string folder = UserData.Path + "chara/female/";
		folderAssist.CreateFolderInfoEx(folder, searchPattern);
		List<string> list = (from n in folderAssist.lstFile.Shuffle()
			select n.FullPath).ToList();
		int num2 = Mathf.Min(list.Count, num);
		if (num2 == 0)
		{
			return null;
		}
		List<ChaFileControl> list2 = new List<ChaFileControl>();
		for (int i = 0; i < num2; i++)
		{
			ChaFileControl chaFileControl = new ChaFileControl();
			if (chaFileControl.LoadCharaFile(list[i], 1, true) && chaFileControl.parameter.sex != 0)
			{
				list2.Add(chaFileControl);
			}
		}
		return list2.ToArray();
	}

	public void SetActiveTop(bool active)
	{
		if ((bool)base.objTop)
		{
			base.objTop.SetActiveIfDifferent(active);
		}
	}

	public bool GetActiveTop()
	{
		if ((bool)base.objTop)
		{
			return base.objTop.activeSelf;
		}
		return false;
	}

	public void SetPosition(float x, float y, float z)
	{
		if (null != base.objTop)
		{
			base.objTop.transform.localPosition = new Vector3(x, y, z);
		}
	}

	public void SetPosition(Vector3 pos)
	{
		if (null != base.objTop)
		{
			base.objTop.transform.localPosition = pos;
		}
	}

	public Vector3 GetPosition()
	{
		return (!(null == base.objTop)) ? base.objTop.transform.localPosition : Vector3.zero;
	}

	public void SetRotation(float x, float y, float z)
	{
		if (null != base.objTop)
		{
			base.objTop.transform.localRotation = Quaternion.Euler(x, y, z);
		}
	}

	public void SetRotation(Vector3 rot)
	{
		if (null != base.objTop)
		{
			base.objTop.transform.localRotation = Quaternion.Euler(rot);
		}
	}

	public Vector3 GetRotation()
	{
		return (!(null == base.objTop)) ? base.objTop.transform.localRotation.eulerAngles : Vector3.zero;
	}

	public void SetTransform(Transform trf)
	{
		if (null != base.objTop)
		{
			base.objTop.transform.localPosition = trf.localPosition;
			base.objTop.transform.localRotation = trf.localRotation;
			base.objTop.transform.localScale = trf.localScale;
		}
	}

	public void ChangeSettingMannequin(bool mannequin)
	{
		if (mannequin)
		{
			if (!mannequinBackInfo.mannequin)
			{
				mannequinBackInfo.mannequin = true;
				mannequinBackInfo.Backup(this);
				string assetBundleName = "chara/oo_base.unity3d";
				string assetName = "mannequin";
				base.chaFile.LoadFileLimited(assetBundleName, assetName, true, true, true, false, false);
				Reload(true);
				ChangeEyesPtn(21, false);
				ChangeMouthPtn(0, false);
				base.fileStatus.mouthFixed = true;
				ChangeMouthOpenMax(0f);
				ChangeLookNeckPtn(3);
				base.neckLookCtrl.neckLookScript.skipCalc = true;
				base.neckLookCtrl.ForceLateUpdate();
				base.eyeLookCtrl.ForceLateUpdate();
				base.resetDynamicBoneAll = true;
				LateUpdateForce();
				GameObject referenceInfo = GetReferenceInfo(RefObjKey.ObjEyebrow);
				if ((bool)referenceInfo)
				{
					referenceInfo.SetActiveIfDifferent(false);
				}
				GameObject referenceInfo2 = GetReferenceInfo(RefObjKey.ObjNoseline);
				if ((bool)referenceInfo2)
				{
					referenceInfo2.SetActiveIfDifferent(false);
				}
				GameObject referenceInfo3 = GetReferenceInfo(RefObjKey.N_FaceSpecial);
				if ((bool)referenceInfo3)
				{
					referenceInfo3.SetActiveIfDifferent(false);
				}
			}
		}
		else if (mannequinBackInfo.mannequin)
		{
			mannequinBackInfo.Restore(this);
			GameObject referenceInfo4 = GetReferenceInfo(RefObjKey.ObjEyebrow);
			if ((bool)referenceInfo4)
			{
				referenceInfo4.SetActiveIfDifferent(true);
			}
			GameObject referenceInfo5 = GetReferenceInfo(RefObjKey.ObjNoseline);
			if ((bool)referenceInfo5)
			{
				referenceInfo5.SetActiveIfDifferent(true);
			}
			GameObject referenceInfo6 = GetReferenceInfo(RefObjKey.N_FaceSpecial);
			if ((bool)referenceInfo6)
			{
				referenceInfo6.SetActiveIfDifferent(true);
			}
			mannequinBackInfo.mannequin = false;
		}
	}

	public void RestoreMannequinHair()
	{
		ChaFileControl chaFileControl = new ChaFileControl();
		chaFileControl.SetCustomBytes(mannequinBackInfo.custom, ChaFileDefine.ChaFileCustomVersion);
		base.fileCustom.hair = chaFileControl.custom.hair;
		Reload(true, true, false, true);
	}

	public void OnDestroy()
	{
		if (Singleton<Character>.IsInstance())
		{
			Singleton<Character>.Instance.DeleteChara(this, true);
		}
		ReleaseAll();
	}

	public void UpdateForce()
	{
		if (base.loadEnd)
		{
			UpdateBlendShapeVoice();
			if (base.hiPoly)
			{
				UpdateSiru();
			}
		}
	}

	public void LateUpdateForce()
	{
		if (!base.loadEnd)
		{
			return;
		}
		UpdateVisible();
		if (base.resetDynamicBoneAll)
		{
			ResetDynamicBoneAll();
			base.resetDynamicBoneAll = false;
		}
		if (base.updateShapeFace)
		{
			UpdateShapeFace();
			base.updateShapeFace = false;
		}
		if (base.updateShapeBody)
		{
			UpdateShapeBody();
			base.updateShapeBody = false;
		}
		UpdateAlwaysShapeBody();
		GameObject referenceInfo = GetReferenceInfo(RefObjKey.S_ANA);
		GameObject referenceInfo2 = GetReferenceInfo(RefObjKey.a_n_ana);
		if (null != referenceInfo && null != referenceInfo2)
		{
			float x = 1f / referenceInfo.transform.localScale.x;
			float y = 1f / referenceInfo.transform.localScale.y;
			float z = 1f / referenceInfo.transform.localScale.z;
			referenceInfo2.transform.localScale = new Vector3(x, y, z);
		}
		if (base.hiPoly && base.sex == 1)
		{
			float x2 = ((!Singleton<Character>.Instance.enableCorrectArmSize) ? 1f : 0.92f);
			float y2 = ((!Singleton<Character>.Instance.enableCorrectHandSize) ? 1f : 0.9f);
			float z2 = ((!Singleton<Character>.Instance.enableCorrectHandSize) ? 1f : 0.9f);
			GameObject referenceInfo3 = GetReferenceInfo(RefObjKey.CORRECT_ARM_L);
			if ((bool)referenceInfo3)
			{
				referenceInfo3.transform.localScale = new Vector3(x2, 1f, 1f);
			}
			GameObject referenceInfo4 = GetReferenceInfo(RefObjKey.CORRECT_ARM_R);
			if ((bool)referenceInfo4)
			{
				referenceInfo4.transform.localScale = new Vector3(x2, 1f, 1f);
			}
			GameObject referenceInfo5 = GetReferenceInfo(RefObjKey.CORRECT_HAND_L);
			if ((bool)referenceInfo5)
			{
				referenceInfo5.transform.localScale = new Vector3(1f, y2, z2);
			}
			GameObject referenceInfo6 = GetReferenceInfo(RefObjKey.CORRECT_HAND_R);
			if ((bool)referenceInfo6)
			{
				referenceInfo6.transform.localScale = new Vector3(1f, y2, z2);
			}
		}
		if (base.reSetupDynamicBoneBust)
		{
			ReSetupDynamicBoneBust();
			UpdateBustSoftnessAndGravity();
			base.reSetupDynamicBoneBust = false;
		}
		if (base.updateBustSize)
		{
			int num = 4;
			float rate = 1f;
			if (0.5f > base.chaFile.custom.body.shapeValueBody[num])
			{
				rate = Mathf.InverseLerp(0f, 0.5f, base.chaFile.custom.body.shapeValueBody[num]);
			}
			foreach (KeyValuePair<BustNormalKind, BustNormal> item in dictBustNormal)
			{
				item.Value.Blend(rate);
			}
			base.updateBustSize = false;
		}
		if (!base.hiPoly)
		{
			RefObjKey[] array = new RefObjKey[2]
			{
				RefObjKey.a_n_nip_L,
				RefObjKey.a_n_nip_R
			};
			RefObjKey[] array2 = array;
			foreach (RefObjKey key in array2)
			{
				GameObject referenceInfo7 = GetReferenceInfo(key);
				if (!(null == referenceInfo7))
				{
					Vector3 lossyScale = referenceInfo7.transform.parent.lossyScale;
					Vector3 one = Vector3.one;
					if (lossyScale.x != 0f)
					{
						one.x = 1f / lossyScale.x;
					}
					if (lossyScale.y != 0f)
					{
						one.y = 1f / lossyScale.y;
					}
					if (lossyScale.z != 0f)
					{
						one.z = 1f / lossyScale.z;
					}
					referenceInfo7.transform.localScale = one;
				}
			}
		}
		SetForegroundEyesAndEyebrow();
	}

	public RuntimeAnimatorController LoadAnimation(string assetBundleName, string assetName, string manifestName = "")
	{
		if (null == base.animBody)
		{
			return null;
		}
		RuntimeAnimatorController runtimeAnimatorController = CommonLib.LoadAsset<RuntimeAnimatorController>(assetBundleName, assetName, false, manifestName);
		if (null == runtimeAnimatorController)
		{
			return null;
		}
		base.animBody.runtimeAnimatorController = runtimeAnimatorController;
		AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		return runtimeAnimatorController;
	}

	public void AnimPlay(string stateName)
	{
		if (!(null == base.animBody))
		{
			base.animBody.Play(stateName);
		}
	}

	public AnimatorStateInfo getAnimatorStateInfo(int _nLayer)
	{
		if (null == base.animBody || null == base.animBody.runtimeAnimatorController)
		{
			return default(AnimatorStateInfo);
		}
		return base.animBody.GetCurrentAnimatorStateInfo(_nLayer);
	}

	public bool syncPlay(AnimatorStateInfo _syncState, int _nLayer)
	{
		if (null == base.animBody)
		{
			return false;
		}
		base.animBody.Play(_syncState.shortNameHash, _nLayer, _syncState.normalizedTime);
		return true;
	}

	public bool syncPlay(int _nameHash, int _nLayer, float _fnormalizedTime)
	{
		if (null == base.animBody)
		{
			return false;
		}
		base.animBody.Play(_nameHash, _nLayer, _fnormalizedTime);
		return true;
	}

	public bool syncPlay(string _strameHash, int _nLayer, float _fnormalizedTime)
	{
		if (null == base.animBody)
		{
			return false;
		}
		base.animBody.Play(_strameHash, _nLayer, _fnormalizedTime);
		return true;
	}

	public bool setLayerWeight(float _fWeight, int _nLayer)
	{
		if (null == base.animBody)
		{
			return false;
		}
		base.animBody.SetLayerWeight(_nLayer, _fWeight);
		return true;
	}

	public bool setAllLayerWeight(float _fWeight)
	{
		if (null == base.animBody)
		{
			return false;
		}
		for (int i = 1; i < base.animBody.layerCount; i++)
		{
			base.animBody.SetLayerWeight(i, _fWeight);
		}
		return true;
	}

	public float getLayerWeight(int _nLayer)
	{
		if (null == base.animBody)
		{
			return 0f;
		}
		return base.animBody.GetLayerWeight(_nLayer);
	}

	public bool setPlay(string _strAnmName, int _nLayer)
	{
		if (null == base.animBody)
		{
			return false;
		}
		base.animBody.Play(_strAnmName, _nLayer);
		return true;
	}

	public void setAnimatorParamTrigger(string _strAnmName)
	{
		if (!(null == base.animBody))
		{
			base.animBody.SetTrigger(_strAnmName);
		}
	}

	public void setAnimatorParamResetTrigger(string _strAnmName)
	{
		if (!(null == base.animBody))
		{
			base.animBody.ResetTrigger(_strAnmName);
		}
	}

	public void setAnimatorParamBool(string _strAnmName, bool _bFlag)
	{
		if (!(null == base.animBody))
		{
			base.animBody.SetBool(_strAnmName, _bFlag);
		}
	}

	public bool getAnimatorParamBool(string _strAnmName)
	{
		if (null == base.animBody)
		{
			return false;
		}
		return base.animBody.GetBool(_strAnmName);
	}

	public void setAnimatorParamFloat(string _strAnmName, float _fValue)
	{
		if (base.animBody != null)
		{
			base.animBody.SetFloat(_strAnmName, _fValue);
		}
	}

	public void setAnimPtnCrossFade(string _strAnmName, float _fBlendTime, int _nLayer, float _fCrossStateTime)
	{
		if (!(null == base.animBody))
		{
			base.animBody.CrossFade(_strAnmName, _fBlendTime, _nLayer, _fCrossStateTime);
		}
	}

	public bool isBlend(int _nLayer)
	{
		if (null == base.animBody)
		{
			return false;
		}
		return base.animBody.IsInTransition(_nLayer);
	}

	public bool IsNextHash(int _nLayer, string _nameHash)
	{
		if (null == base.animBody)
		{
			return false;
		}
		return base.animBody.GetNextAnimatorStateInfo(_nLayer).IsName(_nameHash);
	}

	protected void InitializeControlBodyAll()
	{
		siruNewLv = new byte[Enum.GetNames(typeof(ChaFileDefine.SiruParts)).Length];
		for (int i = 0; i < siruNewLv.Length; i++)
		{
			siruNewLv[i] = 0;
		}
		InitializeControlBodyObject();
	}

	protected void InitializeControlBodyObject()
	{
		dictBustNormal = new Dictionary<BustNormalKind, BustNormal>();
		texBodyAlphaMask = null;
		texBraAlphaMask = null;
		texInnerAlphaMask = null;
	}

	protected void ReleaseControlBodyAll()
	{
		ReleaseControlBodyObject(false);
	}

	protected void ReleaseControlBodyObject(bool init = true)
	{
		if (dictBustNormal != null)
		{
			foreach (KeyValuePair<BustNormalKind, BustNormal> item in dictBustNormal)
			{
				item.Value.Release();
			}
			dictBustNormal.Clear();
		}
		for (int i = 0; i < siruNewLv.Length; i++)
		{
			siruNewLv[i] = 0;
		}
		Resources.UnloadUnusedAssets();
		if (init)
		{
			InitializeControlBodyObject();
		}
	}

	public int GetHeightCategory()
	{
		float shapeBodyValue = GetShapeBodyValue(0);
		if (0.25f > shapeBodyValue)
		{
			return 0;
		}
		if (0.6f < shapeBodyValue)
		{
			return 2;
		}
		return 1;
	}

	public int GetWaistCategory()
	{
		float num = (GetShapeBodyValue(ChaFileDefine.cf_CategoryWaist[0]) + GetShapeBodyValue(ChaFileDefine.cf_CategoryWaist[1]) + GetShapeBodyValue(ChaFileDefine.cf_CategoryWaist[2])) / 3f;
		if (0.35f > num)
		{
			return 0;
		}
		if (0.65f < num)
		{
			return 2;
		}
		return 1;
	}

	public int GetBustCategory()
	{
		float shapeBodyValue = GetShapeBodyValue(4);
		if (0.4f > shapeBodyValue)
		{
			return 0;
		}
		if (0.7f < shapeBodyValue)
		{
			return 2;
		}
		return 1;
	}

	public void ResetDynamicBoneAll()
	{
		DynamicBone[] componentsInChildren = base.objTop.GetComponentsInChildren<DynamicBone>(true);
		DynamicBone[] array = componentsInChildren;
		foreach (DynamicBone dynamicBone in array)
		{
			dynamicBone.ResetParticlesPosition();
		}
	}

	public DynamicBone_Ver02 getDynamicBoneBust(DynamicBoneKind _eArea)
	{
		DynamicBone_Ver02 value = null;
		base.dictDynamicBoneBust.TryGetValue(_eArea, out value);
		return value;
	}

	public bool InitDynamicBoneBust()
	{
		foreach (KeyValuePair<DynamicBoneKind, DynamicBone_Ver02> item in base.dictDynamicBoneBust)
		{
			if (null != item.Value)
			{
				item.Value.ResetParticlesPosition();
			}
		}
		return true;
	}

	public bool ReSetupDynamicBoneBust(int _nArea = 0)
	{
		DynamicBone_Ver02 value = null;
		if (_nArea == 0)
		{
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.BreastL, out value))
			{
				value.ResetPosition();
			}
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.BreastR, out value))
			{
				value.ResetPosition();
			}
		}
		else
		{
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.HipL, out value))
			{
				value.ResetPosition();
			}
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.HipR, out value))
			{
				value.ResetPosition();
			}
		}
		return true;
	}

	public bool playDynamicBoneBust(int _nArea, bool _bPlay)
	{
		DynamicBone_Ver02 value = null;
		if (_nArea == 0)
		{
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.BreastL, out value))
			{
				value.enabled = _bPlay;
			}
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.BreastR, out value))
			{
				value.enabled = _bPlay;
			}
		}
		else
		{
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.HipL, out value))
			{
				value.enabled = _bPlay;
			}
			if (base.dictDynamicBoneBust.TryGetValue(DynamicBoneKind.HipR, out value))
			{
				value.enabled = _bPlay;
			}
		}
		return true;
	}

	public bool playDynamicBoneBust(DynamicBoneKind _eArea, bool _bPlay)
	{
		DynamicBone_Ver02 value = null;
		if (base.dictDynamicBoneBust.TryGetValue(_eArea, out value))
		{
			value.enabled = _bPlay;
		}
		return true;
	}

	public bool ChangeNipRate(float rate)
	{
		base.chaFile.status.nipStandRate = rate;
		changeShapeBodyMask = true;
		base.updateShapeBody = true;
		return true;
	}

	public void ChangeHitBustBlendShapeValue(byte typeSex)
	{
		if (null != base.hitBodyCtrlCmp)
		{
			base.hitBodyCtrlCmp.ChangeBustBlendShapeValue((typeSex != 0) ? 0f : 90f);
		}
	}

	public void EnableExpressionIndex(int indexNo, bool enable)
	{
		if (null != base.expression)
		{
			base.expression.EnableIndex((ExpressionBone.ExpressionIndex)indexNo, enable);
		}
	}

	public void EnableExpressionCategory(int categoryNo, bool enable)
	{
		if (null != base.expression)
		{
			base.expression.EnableCategory((ExpressionBone.ExpressionCategory)categoryNo, enable);
		}
	}

	public void SetSiruFlags(ChaFileDefine.SiruParts parts, byte lv)
	{
		siruNewLv[(int)parts] = lv;
	}

	public byte GetSiruFlags(ChaFileDefine.SiruParts parts)
	{
		return base.chaFile.status.siruLv[(int)parts];
	}

	private bool UpdateSiru(bool forceChange = false)
	{
		if (!base.hiPoly)
		{
			return true;
		}
		if (!(null == base.customMatFace))
		{
			int num = 0;
			if (forceChange || base.fileStatus.siruLv[num] != siruNewLv[num])
			{
				base.fileStatus.siruLv[num] = siruNewLv[num];
				base.customMatFace.SetFloat(ChaShader._liquidface, (int)base.fileStatus.siruLv[num]);
			}
		}
		if (!(null == base.customMatBody))
		{
			ChaFileDefine.SiruParts[] array = new ChaFileDefine.SiruParts[4]
			{
				ChaFileDefine.SiruParts.SiruFrontUp,
				ChaFileDefine.SiruParts.SiruFrontDown,
				ChaFileDefine.SiruParts.SiruBackUp,
				ChaFileDefine.SiruParts.SiruBackDown
			};
			List<string> list = new List<string>();
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (forceChange || base.fileStatus.siruLv[(int)array[i]] != siruNewLv[(int)array[i]])
				{
					flag = true;
				}
				if (siruNewLv[(int)array[i]] != 0)
				{
					string item = array[i].ToString() + siruNewLv[(int)array[i]].ToString("00");
					list.Add(item);
				}
				base.fileStatus.siruLv[(int)array[i]] = siruNewLv[(int)array[i]];
			}
			if (flag)
			{
				float[] array2 = new float[4]
				{
					(int)base.fileStatus.siruLv[(int)array[0]],
					(int)base.fileStatus.siruLv[(int)array[1]],
					(int)base.fileStatus.siruLv[(int)array[2]],
					(int)base.fileStatus.siruLv[(int)array[3]]
				};
				base.customMatBody.SetFloat(ChaShader._liquidftop, array2[0]);
				base.customMatBody.SetFloat(ChaShader._liquidfbot, array2[1]);
				base.customMatBody.SetFloat(ChaShader._liquidbtop, array2[2]);
				base.customMatBody.SetFloat(ChaShader._liquidbbot, array2[3]);
				int[] array3 = new int[5] { 0, 1, 2, 3, 5 };
				for (int j = 0; j < array3.Length; j++)
				{
					UpdateClothesSiru(j, array2[0], array2[1], array2[2], array2[3]);
				}
			}
		}
		return true;
	}

	public void ChangeAlphaMask(params byte[] state)
	{
		if ((bool)base.customMatBody)
		{
			base.customMatBody.SetFloat(ChaShader._alpha_a, (int)state[0]);
			base.customMatBody.SetFloat(ChaShader._alpha_b, (int)state[1]);
		}
		if (base.rendBra != null)
		{
			for (int i = 0; i < 2; i++)
			{
				if ((bool)base.rendBra[i])
				{
					Material material = base.rendBra[i].material;
					if ((bool)material)
					{
						material.SetFloat(ChaShader._alpha_a, (int)state[0]);
						material.SetFloat(ChaShader._alpha_b, (int)state[1]);
					}
				}
			}
		}
		if (base.rendInner == null)
		{
			return;
		}
		for (int j = 0; j < 2; j++)
		{
			if ((bool)base.rendInner[j])
			{
				Material material2 = base.rendInner[j].material;
				if ((bool)material2)
				{
					material2.SetFloat(ChaShader._alpha_a, (int)state[0]);
					material2.SetFloat(ChaShader._alpha_b, (int)state[1]);
				}
			}
		}
	}

	public void ChangeSimpleBodyDraw(bool drawSimple)
	{
		base.fileStatus.visibleSimple = drawSimple;
	}

	public void ChangeSimpleBodyColor(Color color)
	{
		base.fileStatus.simpleColor = color;
		if ((bool)base.rendSimpleBody)
		{
			Material material = base.rendSimpleBody.material;
			if ((bool)material)
			{
				material.SetColor(ChaShader._Color, color);
			}
		}
		if ((bool)base.rendSimpleTongue)
		{
			Material material2 = base.rendSimpleTongue.material;
			if ((bool)material2)
			{
				material2.SetColor(ChaShader._Color, color);
			}
		}
	}

	private void UpdateVisible()
	{
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		if (Singleton<Scene>.Instance.NowSceneNames.Any((string s) => s == "H"))
		{
			flag = base.sex != 0 || Manager.Config.EtcData.VisibleSon;
			flag2 = base.sex != 0 || Manager.Config.EtcData.VisibleBody;
			flag3 = base.sex == 0 && base.fileStatus.visibleSimple;
		}
		GameObject referenceInfo = GetReferenceInfo(RefObjKey.S_TongueB);
		if (YS_Assist.SetActiveControl(referenceInfo, base.visibleAll, base.fileStatus.tongueState == 1, flag2, !flag3, base.fileStatus.visibleHeadAlways, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		GameObject referenceInfo2 = GetReferenceInfo(RefObjKey.S_MOZ_ALL);
		if (YS_Assist.SetActiveControl(referenceInfo2, !base.hideMoz))
		{
			base.updateShape = true;
		}
		GameObject referenceInfo3 = GetReferenceInfo(RefObjKey.ObjBody);
		if (YS_Assist.SetActiveControl(referenceInfo3, base.visibleAll, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		bool flag4 = IsClothesStateKind(1) && 0 == base.fileStatus.clothesState[1];
		bool flag5 = IsClothesStateKind(3) && 0 == base.fileStatus.clothesState[3];
		bool flag6 = flag3 || (!flag4 && !flag5) || base.fileStatus.visibleSon;
		GameObject referenceInfo4 = GetReferenceInfo(RefObjKey.S_Son);
		if (YS_Assist.SetActiveControl(referenceInfo4, base.visibleAll, flag6, flag, base.fileStatus.visibleSonAlways))
		{
			base.updateShape = true;
		}
		GameObject referenceInfo5 = GetReferenceInfo(RefObjKey.S_GOMU);
		if (YS_Assist.SetActiveControl(referenceInfo5, base.fileStatus.visibleGomu))
		{
			base.updateShape = true;
		}
		GameObject referenceInfo6 = GetReferenceInfo(RefObjKey.S_SimpleTop);
		if (YS_Assist.SetActiveControl(referenceInfo6, base.visibleAll, flag2, base.fileStatus.visibleBodyAlways, flag3))
		{
			base.updateShape = true;
		}
		if (YS_Assist.SetActiveControl(base.objHead, base.visibleAll, flag2, !flag3, base.fileStatus.visibleHeadAlways, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		GameObject referenceInfo7 = GetReferenceInfo(RefObjKey.S_TongueF);
		if (YS_Assist.SetActiveControl(referenceInfo7, base.fileStatus.tongueState == 0))
		{
			base.updateShape = true;
		}
		bool[,] array = new bool[4, 3]
		{
			{ false, false, false },
			{ true, false, false },
			{ false, true, false },
			{ false, false, true }
		};
		GameObject[] array2 = new GameObject[3]
		{
			GetReferenceInfo(RefObjKey.S_TEARS_01),
			GetReferenceInfo(RefObjKey.S_TEARS_02),
			GetReferenceInfo(RefObjKey.S_TEARS_03)
		};
		for (int i = 0; i < 3; i++)
		{
			if (!(null == array2[i]) && array2[i].SetActiveIfDifferent(array[base.fileStatus.tearsLv, i]))
			{
				base.updateShape = true;
			}
		}
		if (YS_Assist.SetActiveControl(base.objTongueEx, base.visibleAll, base.fileStatus.tongueState == 2, flag2, !flag3, base.fileStatus.visibleHeadAlways, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		bool flag7 = CheckHideHair();
		GameObject[] array3 = base.objHair;
		foreach (GameObject obj in array3)
		{
			if (YS_Assist.SetActiveControl(obj, base.visibleAll, flag2, !flag7, !flag3, base.fileStatus.visibleHeadAlways, base.fileStatus.visibleBodyAlways))
			{
				base.updateShape = true;
			}
		}
		bool[] array4 = new bool[4] { true, true, true, true };
		bool[] array5 = new bool[4] { true, true, true, true };
		bool[] array6 = new bool[4] { true, true, true, true };
		bool[] array7 = new bool[5] { true, true, true, true, true };
		if (YS_Assist.SetActiveControl(base.objClothes[0], base.visibleAll, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		bool flag8 = false;
		GameObject gameObject = null;
		bool[] flags = new bool[1] { base.fileStatus.clothesState[0] == 0 };
		array4[0] = YS_Assist.CheckFlagsArray(flags);
		gameObject = GetReferenceInfo(RefObjKey.S_CTOP_T_DEF);
		if (YS_Assist.SetActiveControl(gameObject, array4[0]))
		{
			base.updateShape = true;
			flag8 = true;
		}
		bool[] flags2 = new bool[1] { base.fileStatus.clothesState[0] == 1 };
		array4[1] = YS_Assist.CheckFlagsArray(flags2);
		gameObject = GetReferenceInfo(RefObjKey.S_CTOP_T_NUGE);
		if (YS_Assist.SetActiveControl(gameObject, array4[1]))
		{
			base.updateShape = true;
			flag8 = true;
		}
		bool[] flags3 = new bool[2]
		{
			base.fileStatus.clothesState[1] == 0,
			(base.fileStatus.clothesState[0] != 3) ? true : false
		};
		array4[2] = YS_Assist.CheckFlagsArray(flags3);
		gameObject = GetReferenceInfo(RefObjKey.S_CTOP_B_DEF);
		if (YS_Assist.SetActiveControl(gameObject, array4[2]))
		{
			base.updateShape = true;
		}
		bool[] flags4 = new bool[2]
		{
			(base.fileStatus.clothesState[1] != 0) ? true : false,
			(base.fileStatus.clothesState[0] != 3) ? true : false
		};
		array4[3] = YS_Assist.CheckFlagsArray(flags4);
		gameObject = GetReferenceInfo(RefObjKey.S_CTOP_B_NUGE);
		if (YS_Assist.SetActiveControl(gameObject, array4[3]))
		{
			base.updateShape = true;
		}
		int[] array8 = new int[3] { 0, 1, 2 };
		RefObjKey[,] array9 = new RefObjKey[3, 2]
		{
			{
				RefObjKey.S_TPARTS_00_DEF,
				RefObjKey.S_TPARTS_00_NUGE
			},
			{
				RefObjKey.S_TPARTS_01_DEF,
				RefObjKey.S_TPARTS_01_NUGE
			},
			{
				RefObjKey.S_TPARTS_02_DEF,
				RefObjKey.S_TPARTS_02_NUGE
			}
		};
		int num = 0;
		ListInfoBase listInfoBase = base.infoClothes[0];
		if (listInfoBase != null)
		{
			num = listInfoBase.Kind;
		}
		for (int k = 0; k < array8.Length; k++)
		{
			if (YS_Assist.SetActiveControl(base.objParts[array8[k]], base.visibleAll, (base.fileStatus.clothesState[0] != 3) ? true : false, flag2, !flag3, base.fileStatus.visibleBodyAlways))
			{
				base.updateShape = true;
			}
			GameObject gameObject2 = null;
			bool[] flags5 = new bool[1] { base.fileStatus.clothesState[0] == 0 };
			gameObject2 = GetReferenceInfo(array9[k, 0]);
			if (YS_Assist.SetActiveControl(gameObject2, flags5))
			{
				base.updateShape = true;
				switch (k)
				{
				case 0:
					flag8 = true;
					break;
				case 1:
					if (num == 2)
					{
						flag8 = true;
					}
					break;
				}
			}
			bool[] flags6 = new bool[1] { base.fileStatus.clothesState[0] == 1 };
			gameObject2 = GetReferenceInfo(array9[k, 1]);
			if (!YS_Assist.SetActiveControl(gameObject2, flags6))
			{
				continue;
			}
			base.updateShape = true;
			switch (k)
			{
			case 0:
				flag8 = true;
				break;
			case 1:
				if (num == 2)
				{
					flag8 = true;
				}
				break;
			}
		}
		if (flag8 || updateAlphaMask)
		{
			byte b = base.fileStatus.clothesState[0];
			byte[,] array10 = new byte[4, 2]
			{
				{ 1, 1 },
				{ 0, 1 },
				{ 0, 1 },
				{ 0, 0 }
			};
			ChangeAlphaMask(array10[b, 0], array10[b, 1]);
			updateAlphaMask = false;
		}
		if (YS_Assist.SetActiveControl(base.objClothes[1], base.visibleAll, !notBot, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		GameObject gameObject3 = null;
		bool[] flags7 = new bool[1] { base.fileStatus.clothesState[1] == 0 };
		array5[0] = YS_Assist.CheckFlagsArray(flags7);
		gameObject3 = GetReferenceInfo(RefObjKey.S_CBOT_B_DEF);
		if (YS_Assist.SetActiveControl(gameObject3, array5[0]))
		{
			base.updateShape = true;
		}
		bool[] flags8 = new bool[1] { base.fileStatus.clothesState[1] == 1 };
		array5[1] = YS_Assist.CheckFlagsArray(flags8);
		gameObject3 = GetReferenceInfo(RefObjKey.S_CBOT_B_NUGE);
		if (YS_Assist.SetActiveControl(gameObject3, array5[1]))
		{
			base.updateShape = true;
		}
		bool[] flags9 = new bool[2]
		{
			base.fileStatus.clothesState[0] == 0,
			(base.fileStatus.clothesState[1] != 2) ? true : false
		};
		array5[2] = YS_Assist.CheckFlagsArray(flags9);
		gameObject3 = GetReferenceInfo(RefObjKey.S_CBOT_T_DEF);
		if (YS_Assist.SetActiveControl(gameObject3, array5[2]))
		{
			base.updateShape = true;
		}
		bool[] flags10 = new bool[2]
		{
			(base.fileStatus.clothesState[0] != 0) ? true : false,
			(base.fileStatus.clothesState[1] != 2) ? true : false
		};
		array5[3] = YS_Assist.CheckFlagsArray(flags10);
		gameObject3 = GetReferenceInfo(RefObjKey.S_CBOT_T_NUGE);
		if (YS_Assist.SetActiveControl(gameObject3, array5[3]))
		{
			base.updateShape = true;
		}
		if (YS_Assist.SetActiveControl(base.objClothes[2], base.visibleAll, flag2, !flag3, !notBra, 3 != base.fileStatus.clothesState[2], base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		GameObject gameObject4 = null;
		bool[] flags11 = new bool[1] { base.fileStatus.clothesState[2] == 0 };
		array6[0] = YS_Assist.CheckFlagsArray(flags11);
		gameObject4 = GetReferenceInfo(RefObjKey.S_UWT_T_DEF);
		if (YS_Assist.SetActiveControl(gameObject4, array6[0]))
		{
			base.updateShape = true;
		}
		bool[] flags12 = new bool[1] { base.fileStatus.clothesState[2] == 1 };
		array6[1] = YS_Assist.CheckFlagsArray(flags12);
		gameObject4 = GetReferenceInfo(RefObjKey.S_UWT_T_NUGE);
		if (YS_Assist.SetActiveControl(gameObject4, array6[1]))
		{
			base.updateShape = true;
		}
		bool[] flags13 = new bool[2]
		{
			base.fileStatus.clothesState[3] == 0,
			(base.fileStatus.clothesState[2] != 3) ? true : false
		};
		array6[2] = YS_Assist.CheckFlagsArray(flags13);
		gameObject4 = GetReferenceInfo(RefObjKey.S_UWT_B_DEF);
		if (YS_Assist.SetActiveControl(gameObject4, array6[2]))
		{
			base.updateShape = true;
		}
		bool[] flags14 = new bool[2]
		{
			(base.fileStatus.clothesState[3] != 0) ? true : false,
			(base.fileStatus.clothesState[2] != 3) ? true : false
		};
		array6[3] = YS_Assist.CheckFlagsArray(flags14);
		gameObject4 = GetReferenceInfo(RefObjKey.S_UWT_B_NUGE);
		if (YS_Assist.SetActiveControl(gameObject4, array6[3]))
		{
			base.updateShape = true;
		}
		for (int l = 0; l < 2; l++)
		{
			foreach (GameObject item in base.lstObjBraOpt[l])
			{
				YS_Assist.SetActiveControl(item, !nowCoordinate.clothes.hideBraOpt[l]);
			}
		}
		if (YS_Assist.SetActiveControl(base.objClothes[3], base.visibleAll, !notShorts, flag2, !flag3, 3 != base.fileStatus.clothesState[3], base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		GameObject gameObject5 = null;
		bool[] flags15 = new bool[1] { base.fileStatus.clothesState[3] == 0 };
		array7[0] = YS_Assist.CheckFlagsArray(flags15);
		gameObject5 = GetReferenceInfo(RefObjKey.S_UWB_B_DEF);
		if (YS_Assist.SetActiveControl(gameObject5, array7[0]))
		{
			base.updateShape = true;
		}
		bool[] flags16 = new bool[1] { base.fileStatus.clothesState[3] == 1 };
		array7[1] = YS_Assist.CheckFlagsArray(flags16);
		gameObject5 = GetReferenceInfo(RefObjKey.S_UWB_B_NUGE);
		if (YS_Assist.SetActiveControl(gameObject5, array7[1]))
		{
			base.updateShape = true;
		}
		bool[] flags17 = new bool[1] { base.fileStatus.clothesState[3] == 2 };
		array7[2] = YS_Assist.CheckFlagsArray(flags17);
		gameObject5 = GetReferenceInfo(RefObjKey.S_UWB_B_NUGE2);
		if (YS_Assist.SetActiveControl(gameObject5, array7[2]))
		{
			base.updateShape = true;
		}
		for (int m = 0; m < 2; m++)
		{
			foreach (GameObject item2 in base.lstObjShortsOpt[m])
			{
				YS_Assist.SetActiveControl(item2, !nowCoordinate.clothes.hideShortsOpt[m]);
			}
		}
		if (YS_Assist.SetActiveControl(base.objClothes[4], base.visibleAll, base.fileStatus.clothesState[4] == 0, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		byte b2 = base.fileStatus.clothesState[5];
		if (YS_Assist.SetActiveControl(base.objClothes[5], base.visibleAll, (b2 != 3 && b2 != 2) ? true : false, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		GameObject gameObject6 = null;
		gameObject6 = GetReferenceInfo(RefObjKey.S_PANST_DEF);
		if (YS_Assist.SetActiveControl(gameObject6, base.fileStatus.clothesState[5] == 0))
		{
			base.updateShape = true;
		}
		gameObject6 = GetReferenceInfo(RefObjKey.S_PANST_NUGE);
		if (YS_Assist.SetActiveControl(gameObject6, base.fileStatus.clothesState[5] == 1))
		{
			base.updateShape = true;
		}
		if (YS_Assist.SetActiveControl(base.objClothes[6], base.visibleAll, base.fileStatus.clothesState[6] == 0, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		if (YS_Assist.SetActiveControl(base.objClothes[7], base.visibleAll, 0 == base.fileStatus.shoesType, base.fileStatus.clothesState[7] == 0, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		if (YS_Assist.SetActiveControl(base.objClothes[8], base.visibleAll, 1 == base.fileStatus.shoesType, base.fileStatus.clothesState[8] == 0, flag2, !flag3, base.fileStatus.visibleBodyAlways))
		{
			base.updateShape = true;
		}
		for (int n = 0; n < base.objAccessory.Length; n++)
		{
			bool flag9 = false;
			if (!base.fileStatus.visibleHeadAlways && nowCoordinate.accessory.parts[n].partsOfHead)
			{
				flag9 = true;
			}
			if (!base.fileStatus.visibleBodyAlways || !flag2)
			{
				flag9 = true;
			}
			if (YS_Assist.SetActiveControl(base.objAccessory[n], base.visibleAll, base.fileStatus.showAccessory[n], !flag3, !flag9))
			{
				base.updateShape = true;
			}
		}
		GameObject referenceInfo8 = GetReferenceInfo(RefObjKey.DB_SKIRT_BOT);
		GameObject referenceInfo9 = GetReferenceInfo(RefObjKey.DB_SKIRT_TOP);
		GameObject referenceInfo10 = GetReferenceInfo(RefObjKey.DB_SKIRT_TOPA);
		GameObject referenceInfo11 = GetReferenceInfo(RefObjKey.DB_SKIRT_TOPB);
		bool active = !referenceInfo8;
		if ((bool)referenceInfo8 && base.fileStatus.clothesState[1] == 3)
		{
			active = true;
		}
		if ((bool)referenceInfo9)
		{
			referenceInfo9.SetActiveIfDifferent(active);
		}
		if ((bool)referenceInfo10)
		{
			referenceInfo10.SetActiveIfDifferent(active);
		}
		if ((bool)referenceInfo11)
		{
			referenceInfo11.SetActiveIfDifferent(active);
		}
	}

	protected void InitializeControlCoordinateAll()
	{
		nowCoordinate = new ChaFileCoordinate();
		InitializeControlCoordinateObject();
	}

	protected void InitializeControlCoordinateObject()
	{
		notBra = false;
		notBot = false;
		notShorts = false;
		hideHairAcs = new bool[20];
		isChangeOfClothesRandom = false;
		dictStateType = new Dictionary<int, Dictionary<byte, string>>();
	}

	protected void ReleaseControlCoordinateAll()
	{
		ReleaseControlCoordinateObject(false);
	}

	protected void ReleaseControlCoordinateObject(bool init = true)
	{
		if (init)
		{
			InitializeControlCoordinateObject();
		}
	}

	public int GetNowClothesType()
	{
		int num = 0;
		int num2 = 1;
		int num3 = 2;
		int num4 = 3;
		int num5 = ((!IsClothesStateKind(num)) ? 3 : base.fileStatus.clothesState[num]);
		int num6 = ((!IsClothesStateKind(num2)) ? 3 : base.fileStatus.clothesState[num2]);
		int num7 = ((!IsClothesStateKind(num3)) ? 3 : base.fileStatus.clothesState[num3]);
		int num8 = ((!IsClothesStateKind(num4)) ? 3 : base.fileStatus.clothesState[num4]);
		bool flag = true;
		bool flag2 = true;
		if (base.infoClothes[num3] != null)
		{
			flag = base.infoClothes[num3].Kind == 1;
		}
		if (base.infoClothes[num4] != null)
		{
			flag2 = base.infoClothes[num4].Kind == 1;
		}
		if (notShorts)
		{
			num8 = num7;
			flag2 = flag;
		}
		if (num5 == 0)
		{
			if (num6 == 0)
			{
				return 0;
			}
			if (num8 == 0)
			{
				return flag2 ? 1 : 2;
			}
			return 3;
		}
		if (num6 == 0)
		{
			if (num7 == 0)
			{
				return flag ? 1 : 2;
			}
			return 3;
		}
		if (num7 == 0)
		{
			if (num8 == 0)
			{
				if (flag)
				{
					return 1;
				}
				return flag2 ? 1 : 2;
			}
			return 3;
		}
		return 3;
	}

	public bool IsKokanHide()
	{
		bool result = false;
		int[] array = new int[4] { 0, 1, 2, 3 };
		int[] array2 = new int[4] { 1, 1, 3, 3 };
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i];
			if (IsClothes(num) && ((i != 0 && i != 2) || !("2" != base.infoClothes[num].GetInfo(ChaListDefine.KeyType.Coordinate))) && "1" == base.infoClothes[num].GetInfo(ChaListDefine.KeyType.KokanHide) && base.fileStatus.clothesState[array2[i]] == 0)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool AssignCoordinate(ChaFileDefine.CoordinateType type, string path)
	{
		ChaFileCoordinate chaFileCoordinate = new ChaFileCoordinate();
		string path2 = ChaFileControl.ConvertCoordinateFilePath(path);
		if (!chaFileCoordinate.LoadFile(path2))
		{
			return false;
		}
		return base.chaFile.AssignCoordinate(type, chaFileCoordinate);
	}

	public bool AssignCoordinate(ChaFileDefine.CoordinateType type, ChaFileCoordinate srcCoorde)
	{
		return base.chaFile.AssignCoordinate(type, srcCoorde);
	}

	public bool AssignCoordinate(ChaFileDefine.CoordinateType type)
	{
		return base.chaFile.AssignCoordinate(type, nowCoordinate);
	}

	public bool SetNowCoordinate(string path)
	{
		ChaFileCoordinate chaFileCoordinate = new ChaFileCoordinate();
		string path2 = ChaFileControl.ConvertCoordinateFilePath(path);
		if (!chaFileCoordinate.LoadFile(path2))
		{
			return false;
		}
		return SetNowCoordinate(chaFileCoordinate);
	}

	public bool SetNowCoordinate(ChaFileCoordinate srcCoorde)
	{
		byte[] data = srcCoorde.SaveBytes();
		return nowCoordinate.LoadBytes(data, srcCoorde.loadVersion);
	}

	public bool ChangeCoordinateType(bool changeBackCoordinateType = true)
	{
		return ChangeCoordinateType((ChaFileDefine.CoordinateType)base.fileStatus.coordinateType, changeBackCoordinateType);
	}

	public bool ChangeCoordinateType(ChaFileDefine.CoordinateType type, bool changeBackCoordinateType = true)
	{
		if (base.chaFile.coordinate.Length <= (int)type)
		{
			return false;
		}
		byte[] data = base.chaFile.coordinate[(int)type].SaveBytes();
		if (nowCoordinate.LoadBytes(data, ChaFileDefine.ChaFileCoordinateVersion))
		{
			if (changeBackCoordinateType)
			{
				base.fileStatus.backCoordinateType = base.fileStatus.coordinateType;
			}
			base.fileStatus.coordinateType = (int)type;
			return true;
		}
		return false;
	}

	public bool ChangeCoordinateTypeAndReload(bool changeBackCoordinateType = true)
	{
		if (ChangeCoordinateType(changeBackCoordinateType))
		{
			Reload(false, true, true, true);
			return true;
		}
		return false;
	}

	public bool ChangeCoordinateTypeAndReload(ChaFileDefine.CoordinateType type, bool changeBackCoordinateType = true)
	{
		if (ChangeCoordinateType(type, changeBackCoordinateType))
		{
			Reload(false, true, true, true);
			return true;
		}
		return false;
	}

	public bool AddClothesStateKind(int clothesKind, string stateType)
	{
		ChaFileDefine.ClothesKind clothesKind2 = (ChaFileDefine.ClothesKind)Enum.ToObject(typeof(ChaFileDefine.ClothesKind), clothesKind);
		switch (clothesKind2)
		{
		case ChaFileDefine.ClothesKind.gloves:
			dictStateType.Remove(4);
			AddClothesStateKindSub(4, byte.Parse(stateType));
			break;
		case ChaFileDefine.ClothesKind.panst:
			dictStateType.Remove(5);
			AddClothesStateKindSub(5, byte.Parse(stateType));
			break;
		case ChaFileDefine.ClothesKind.socks:
			dictStateType.Remove(6);
			AddClothesStateKindSub(6, byte.Parse(stateType));
			break;
		case ChaFileDefine.ClothesKind.shoes_inner:
			dictStateType.Remove(7);
			AddClothesStateKindSub(7, byte.Parse(stateType));
			break;
		case ChaFileDefine.ClothesKind.shoes_outer:
			dictStateType.Remove(8);
			AddClothesStateKindSub(8, byte.Parse(stateType));
			break;
		case ChaFileDefine.ClothesKind.top:
		case ChaFileDefine.ClothesKind.bot:
		{
			dictStateType.Remove(0);
			dictStateType.Remove(1);
			ListInfoBase listInfoBase3 = base.infoClothes[0];
			byte b3 = 3;
			if (listInfoBase3 != null)
			{
				b3 = byte.Parse(listInfoBase3.GetInfo(ChaListDefine.KeyType.StateType));
			}
			byte b4 = 3;
			ListInfoBase listInfoBase4 = base.infoClothes[1];
			if (listInfoBase4 != null)
			{
				b4 = byte.Parse(listInfoBase4.GetInfo(ChaListDefine.KeyType.StateType));
			}
			if (b4 != 0)
			{
				GameObject referenceInfo3 = GetReferenceInfo(RefObjKey.S_CTOP_B_DEF);
				if ((bool)referenceInfo3)
				{
					switch (b3)
					{
					case 0:
						b4 = 0;
						break;
					case 1:
						if (b4 == 2)
						{
							b4 = 1;
						}
						break;
					}
				}
			}
			if (b3 != 0)
			{
				GameObject referenceInfo4 = GetReferenceInfo(RefObjKey.S_CBOT_T_DEF);
				if ((bool)referenceInfo4)
				{
					switch (b4)
					{
					case 0:
						b3 = 0;
						break;
					case 1:
						if (b3 == 2)
						{
							b3 = 1;
						}
						break;
					}
				}
			}
			AddClothesStateKindSub(0, b3);
			AddClothesStateKindSub(1, b4);
			if (clothesKind2 == ChaFileDefine.ClothesKind.top)
			{
				AddClothesStateKind(2, string.Empty);
			}
			break;
		}
		case ChaFileDefine.ClothesKind.bra:
		case ChaFileDefine.ClothesKind.shorts:
		{
			dictStateType.Remove(2);
			dictStateType.Remove(3);
			byte b = 3;
			if (!notBra)
			{
				ListInfoBase listInfoBase = base.infoClothes[2];
				if (listInfoBase != null)
				{
					b = byte.Parse(listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
				}
			}
			byte b2 = 3;
			if (!notBra || !notShorts)
			{
				ListInfoBase listInfoBase2 = base.infoClothes[3];
				if (listInfoBase2 != null)
				{
					b2 = byte.Parse(listInfoBase2.GetInfo(ChaListDefine.KeyType.StateType));
				}
			}
			if (b2 != 0 && b2 != 2)
			{
				GameObject referenceInfo = GetReferenceInfo(RefObjKey.S_UWT_B_DEF);
				if ((bool)referenceInfo)
				{
					switch (b)
					{
					case 0:
						b2 = 0;
						break;
					case 1:
						if (b2 == 3)
						{
							b2 = 1;
						}
						break;
					}
				}
			}
			if (b != 0 && b != 2)
			{
				GameObject referenceInfo2 = GetReferenceInfo(RefObjKey.S_UWB_T_DEF);
				if ((bool)referenceInfo2)
				{
					switch (b2)
					{
					case 0:
						b = 0;
						break;
					case 1:
						if (b == 3)
						{
							b = 1;
						}
						break;
					}
				}
			}
			AddClothesStateKindSub(2, b);
			AddClothesStateKindSub(3, b2);
			break;
		}
		}
		return true;
	}

	private bool AddClothesStateKindSub(int clothesKind, byte type)
	{
		if (!MathfEx.RangeEqualOn(0, type, 2))
		{
			return false;
		}
		Dictionary<byte, string> dictionary = new Dictionary<byte, string>();
		switch (type)
		{
		case 0:
			dictionary[0] = "着衣";
			dictionary[1] = "半脱";
			dictionary[3] = "脱衣";
			break;
		case 1:
			dictionary[0] = "着衣";
			dictionary[3] = "脱衣";
			break;
		default:
			dictionary[0] = "着衣";
			dictionary[1] = "半脱１";
			dictionary[2] = "半脱２";
			dictionary[3] = "脱衣";
			break;
		}
		dictStateType[clothesKind] = dictionary;
		return true;
	}

	public void RemoveClothesStateKind(int clothesKind)
	{
		switch ((ChaFileDefine.ClothesKind)Enum.ToObject(typeof(ChaFileDefine.ClothesKind), clothesKind))
		{
		case ChaFileDefine.ClothesKind.gloves:
			dictStateType.Remove(4);
			break;
		case ChaFileDefine.ClothesKind.panst:
			dictStateType.Remove(5);
			break;
		case ChaFileDefine.ClothesKind.socks:
			dictStateType.Remove(6);
			break;
		case ChaFileDefine.ClothesKind.shoes_inner:
			dictStateType.Remove(7);
			break;
		case ChaFileDefine.ClothesKind.shoes_outer:
			dictStateType.Remove(8);
			break;
		case ChaFileDefine.ClothesKind.top:
		case ChaFileDefine.ClothesKind.bot:
			AddClothesStateKind(0, string.Empty);
			break;
		case ChaFileDefine.ClothesKind.bra:
		case ChaFileDefine.ClothesKind.shorts:
			AddClothesStateKind(2, string.Empty);
			break;
		}
	}

	public bool IsClothes(int clothesKind)
	{
		if (!IsClothesStateKind(clothesKind))
		{
			return false;
		}
		if (null == base.objClothes[clothesKind])
		{
			return false;
		}
		if (base.infoClothes[clothesKind] == null)
		{
			return false;
		}
		return true;
	}

	public bool IsClothesStateKind(int clothesKind)
	{
		return dictStateType.ContainsKey(clothesKind);
	}

	public bool IsShoesStateKind()
	{
		int clothesKind = ((base.fileStatus.shoesType != 0) ? 8 : 7);
		return IsClothesStateKind(clothesKind);
	}

	public Dictionary<byte, string> GetClothesStateKind(int clothesKind)
	{
		Dictionary<byte, string> value = null;
		dictStateType.TryGetValue(clothesKind, out value);
		return value;
	}

	public bool IsClothesStateType(int clothesKind, byte stateType)
	{
		Dictionary<byte, string> value = null;
		dictStateType.TryGetValue(clothesKind, out value);
		if (value == null)
		{
			return false;
		}
		return value.ContainsKey(stateType);
	}

	public bool IsAccessory(int slotNo)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		return !(null == base.objAccessory[slotNo]);
	}

	public void SetClothesState(int clothesKind, byte state, bool next = true)
	{
		if (next)
		{
			byte b = base.fileStatus.clothesState[clothesKind];
			do
			{
				if (!IsClothesStateKind(clothesKind))
				{
					base.fileStatus.clothesState[clothesKind] = state;
					break;
				}
				if (IsClothesStateType(clothesKind, state))
				{
					base.fileStatus.clothesState[clothesKind] = state;
					break;
				}
				state = (byte)((state + 1) % 4);
			}
			while (b != state);
		}
		else
		{
			byte b2 = base.fileStatus.clothesState[clothesKind];
			do
			{
				if (!IsClothesStateKind(clothesKind))
				{
					base.fileStatus.clothesState[clothesKind] = state;
					break;
				}
				if (IsClothesStateType(clothesKind, state))
				{
					base.fileStatus.clothesState[clothesKind] = state;
					break;
				}
				state = (byte)((state + 3) % 4);
			}
			while (b2 != state);
		}
		switch (clothesKind)
		{
		case 0:
			if (notBot)
			{
				if (base.fileStatus.clothesState[clothesKind] == 3)
				{
					base.fileStatus.clothesState[1] = 3;
				}
				else if (base.fileStatus.clothesState[1] == 3)
				{
					base.fileStatus.clothesState[1] = state;
				}
			}
			break;
		case 1:
			if (notBot)
			{
				if (base.fileStatus.clothesState[clothesKind] == 3)
				{
					base.fileStatus.clothesState[0] = 3;
				}
				else if (base.fileStatus.clothesState[0] == 3)
				{
					base.fileStatus.clothesState[0] = state;
				}
			}
			break;
		case 2:
			if (notShorts)
			{
				if (base.fileStatus.clothesState[clothesKind] == 3)
				{
					base.fileStatus.clothesState[3] = 3;
				}
				else if (base.fileStatus.clothesState[3] == 3)
				{
					base.fileStatus.clothesState[3] = state;
				}
			}
			break;
		case 3:
			if (notShorts)
			{
				if (base.fileStatus.clothesState[clothesKind] == 3)
				{
					base.fileStatus.clothesState[2] = 3;
				}
				else if (base.fileStatus.clothesState[2] == 3)
				{
					base.fileStatus.clothesState[2] = state;
				}
			}
			break;
		case 7:
			if (state == 0)
			{
				if (IsClothesStateKind(8))
				{
					base.fileStatus.clothesState[8] = state;
				}
			}
			else
			{
				base.fileStatus.clothesState[8] = state;
			}
			break;
		case 8:
			if (state == 0)
			{
				if (IsClothesStateKind(7))
				{
					base.fileStatus.clothesState[7] = state;
				}
			}
			else
			{
				base.fileStatus.clothesState[7] = state;
			}
			break;
		case 4:
		case 5:
		case 6:
			break;
		}
	}

	public void SetClothesStatePrev(int clothesKind)
	{
		byte b = base.fileStatus.clothesState[clothesKind];
		b = (byte)((b + 3) % 4);
		SetClothesState(clothesKind, b, false);
	}

	public void SetClothesStateNext(int clothesKind)
	{
		byte b = base.fileStatus.clothesState[clothesKind];
		b = (byte)((b + 1) % 4);
		SetClothesState(clothesKind, b);
	}

	public void SetClothesStateAll(byte state)
	{
		int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
		for (int i = 0; i < num; i++)
		{
			SetClothesState(i, state);
		}
	}

	public void UpdateClothesStateAll()
	{
		int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
		for (int i = 0; i < num; i++)
		{
			SetClothesState(i, base.fileStatus.clothesState[i]);
		}
	}

	public void RandomChangeOfClothesLowPoly(int h)
	{
		if (!isChangeOfClothesRandom)
		{
			isChangeOfClothesRandom = true;
			byte[,] array = new byte[6, 9]
			{
				{ 0, 0, 0, 0, 0, 0, 0, 0, 0 },
				{ 3, 3, 3, 3, 3, 3, 3, 3, 3 },
				{ 3, 3, 0, 0, 3, 3, 3, 3, 3 },
				{ 3, 0, 0, 0, 0, 0, 0, 3, 3 },
				{ 3, 0, 3, 0, 0, 0, 0, 3, 3 },
				{ 3, 3, 3, 0, 3, 0, 0, 3, 3 }
			};
			int num = (int)((float)h * 0.5f);
			int[] obj = new int[6] { 10, 0, 50, 50, 50, 50 };
			obj[1] = 5 + num;
			int[] weightTable = obj;
			int randomIndex = ChaRandom.GetRandomIndex(weightTable);
			int num2 = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
			for (int i = 0; i < num2; i++)
			{
				SetClothesState(i, array[randomIndex, i]);
			}
		}
	}

	public void RandomChangeOfClothesLowPolyEnd()
	{
		SetClothesStateAll(0);
		isChangeOfClothesRandom = false;
	}

	public void ChangeToiletStateLowPoly()
	{
		SetClothesState(1, 3);
		SetClothesState(3, 3);
		SetClothesState(5, 3);
	}

	public void SetAccessoryState(int slotNo, bool show)
	{
		if (base.fileStatus.showAccessory.Length > slotNo)
		{
			base.fileStatus.showAccessory[slotNo] = show;
		}
	}

	public void SetAccessoryStateAll(bool show)
	{
		for (int i = 0; i < base.fileStatus.showAccessory.Length; i++)
		{
			base.fileStatus.showAccessory[i] = show;
		}
	}

	public void SetAccessoryStateCategory(int cateNo, bool show)
	{
		if (cateNo != 0 && cateNo != 1)
		{
			return;
		}
		for (int i = 0; i < base.fileStatus.showAccessory.Length; i++)
		{
			if (nowCoordinate.accessory.parts[i].hideCategory == cateNo)
			{
				base.fileStatus.showAccessory[i] = show;
			}
		}
	}

	public int GetAccessoryCategoryCount(int cateNo)
	{
		if (cateNo != 0 && cateNo != 1)
		{
			return -1;
		}
		int num = 0;
		for (int i = 0; i < base.fileStatus.showAccessory.Length; i++)
		{
			if (nowCoordinate.accessory.parts[i].hideCategory == cateNo)
			{
				num++;
			}
		}
		return num;
	}

	public string GetAccessoryDefaultParentStr(int type, int id)
	{
		int num = Enum.GetNames(typeof(ChaListDefine.CategoryNo)).Length;
		if (!MathfEx.RangeEqualOn(0, type, num - 1))
		{
			return string.Empty;
		}
		ListInfoBase value = null;
		Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)type);
		if (!categoryInfo.TryGetValue(id, out value))
		{
			return string.Empty;
		}
		return value.GetInfo(ChaListDefine.KeyType.Parent);
	}

	public string GetAccessoryDefaultParentStr(int slotNo)
	{
		GameObject gameObject = base.objAccessory[slotNo];
		if (null == gameObject)
		{
			return string.Empty;
		}
		ListInfoComponent component = gameObject.GetComponent<ListInfoComponent>();
		return component.data.GetInfo(ChaListDefine.KeyType.Parent);
	}

	public bool ChangeAccessoryParent(int slotNo, string parentStr)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		GameObject gameObject = base.objAccessory[slotNo];
		if (null == gameObject)
		{
			return false;
		}
		if ("none" == parentStr)
		{
			gameObject.transform.SetParent(null, false);
			return true;
		}
		ListInfoComponent component = gameObject.GetComponent<ListInfoComponent>();
		ListInfoBase data = component.data;
		if ("0" == data.GetInfo(ChaListDefine.KeyType.Parent))
		{
			return false;
		}
		try
		{
			RefObjKey key = (RefObjKey)Enum.Parse(typeof(RefObjKey), parentStr);
			GameObject referenceInfo = GetReferenceInfo(key);
			if (null == referenceInfo)
			{
				return false;
			}
			gameObject.transform.SetParent(referenceInfo.transform, false);
			nowCoordinate.accessory.parts[slotNo].parentKey = parentStr;
			nowCoordinate.accessory.parts[slotNo].partsOfHead = ChaAccessoryDefine.CheckPartsOfHead(parentStr);
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	public bool SetAccessoryPos(int slotNo, int correctNo, float value, bool add, int flags = 7)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		GameObject gameObject = base.objAcsMove[slotNo, correctNo];
		if (null == gameObject)
		{
			return false;
		}
		ChaFileAccessory accessory = nowCoordinate.accessory;
		if ((flags & 1) != 0)
		{
			float value2 = float.Parse((((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 0].x) + value).ToString("f1"));
			accessory.parts[slotNo].addMove[correctNo, 0].x = Mathf.Clamp(value2, -100f, 100f);
		}
		if ((flags & 2) != 0)
		{
			float value3 = float.Parse((((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 0].y) + value).ToString("f1"));
			accessory.parts[slotNo].addMove[correctNo, 0].y = Mathf.Clamp(value3, -100f, 100f);
		}
		if ((flags & 4) != 0)
		{
			float value4 = float.Parse((((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 0].z) + value).ToString("f1"));
			accessory.parts[slotNo].addMove[correctNo, 0].z = Mathf.Clamp(value4, -100f, 100f);
		}
		Dictionary<int, ListInfoBase> dictionary = null;
		dictionary = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)accessory.parts[slotNo].type);
		ListInfoBase value5 = null;
		dictionary.TryGetValue(accessory.parts[slotNo].id, out value5);
		if (value5.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
		{
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(accessory.parts[slotNo].addMove[correctNo, 0].x * 0.01f, accessory.parts[slotNo].addMove[correctNo, 0].y * 0.01f, accessory.parts[slotNo].addMove[correctNo, 0].z * 0.01f);
		}
		return true;
	}

	public bool SetAccessoryRot(int slotNo, int correctNo, float value, bool add, int flags = 7)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		GameObject gameObject = base.objAcsMove[slotNo, correctNo];
		if (null == gameObject)
		{
			return false;
		}
		ChaFileAccessory accessory = nowCoordinate.accessory;
		if ((flags & 1) != 0)
		{
			float t = (int)(((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 1].x) + value);
			accessory.parts[slotNo].addMove[correctNo, 1].x = Mathf.Repeat(t, 360f);
		}
		if ((flags & 2) != 0)
		{
			float t2 = (int)(((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 1].y) + value);
			accessory.parts[slotNo].addMove[correctNo, 1].y = Mathf.Repeat(t2, 360f);
		}
		if ((flags & 4) != 0)
		{
			float t3 = (int)(((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 1].z) + value);
			accessory.parts[slotNo].addMove[correctNo, 1].z = Mathf.Repeat(t3, 360f);
		}
		Dictionary<int, ListInfoBase> dictionary = null;
		dictionary = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)accessory.parts[slotNo].type);
		ListInfoBase value2 = null;
		dictionary.TryGetValue(accessory.parts[slotNo].id, out value2);
		if (value2.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
		{
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		else
		{
			gameObject.transform.localEulerAngles = new Vector3(accessory.parts[slotNo].addMove[correctNo, 1].x, accessory.parts[slotNo].addMove[correctNo, 1].y, accessory.parts[slotNo].addMove[correctNo, 1].z);
		}
		return true;
	}

	public bool SetAccessoryScl(int slotNo, int correctNo, float value, bool add, int flags = 7)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		GameObject gameObject = base.objAcsMove[slotNo, correctNo];
		if (null == gameObject)
		{
			return false;
		}
		ChaFileAccessory accessory = nowCoordinate.accessory;
		if ((flags & 1) != 0)
		{
			float value2 = float.Parse((((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 2].x) + value).ToString("f2"));
			accessory.parts[slotNo].addMove[correctNo, 2].x = Mathf.Clamp(value2, 0.01f, 100f);
		}
		if ((flags & 2) != 0)
		{
			float value3 = float.Parse((((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 2].y) + value).ToString("f2"));
			accessory.parts[slotNo].addMove[correctNo, 2].y = Mathf.Clamp(value3, 0.01f, 100f);
		}
		if ((flags & 4) != 0)
		{
			float value4 = float.Parse((((!add) ? 0f : accessory.parts[slotNo].addMove[correctNo, 2].z) + value).ToString("f2"));
			accessory.parts[slotNo].addMove[correctNo, 2].z = Mathf.Clamp(value4, 0.01f, 100f);
		}
		Dictionary<int, ListInfoBase> dictionary = null;
		dictionary = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)accessory.parts[slotNo].type);
		ListInfoBase value5 = null;
		dictionary.TryGetValue(accessory.parts[slotNo].id, out value5);
		if (value5.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
		{
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			gameObject.transform.localScale = new Vector3(accessory.parts[slotNo].addMove[correctNo, 2].x, accessory.parts[slotNo].addMove[correctNo, 2].y, accessory.parts[slotNo].addMove[correctNo, 2].z);
		}
		return true;
	}

	public bool ResetAccessoryMove(int slotNo, int correctNo, int type = 7)
	{
		bool flag = true;
		if ((type & 1) != 0)
		{
			flag &= SetAccessoryPos(slotNo, correctNo, 0f, false);
		}
		if ((type & 2) != 0)
		{
			flag &= SetAccessoryRot(slotNo, correctNo, 0f, false);
		}
		if ((type & 4) != 0)
		{
			flag &= SetAccessoryScl(slotNo, correctNo, 1f, false);
		}
		return flag;
	}

	public bool UpdateAccessoryMoveFromInfo(int slotNo)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		ChaFileAccessory accessory = nowCoordinate.accessory;
		Dictionary<int, ListInfoBase> dictionary = null;
		dictionary = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)accessory.parts[slotNo].type);
		ListInfoBase value = null;
		dictionary.TryGetValue(accessory.parts[slotNo].id, out value);
		if (value.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
		{
			for (int i = 0; i < 2; i++)
			{
				GameObject gameObject = base.objAcsMove[slotNo, i];
				if (!(null == gameObject))
				{
					gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
					gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
		else
		{
			for (int j = 0; j < 2; j++)
			{
				GameObject gameObject2 = base.objAcsMove[slotNo, j];
				if (!(null == gameObject2))
				{
					gameObject2.transform.localPosition = new Vector3(accessory.parts[slotNo].addMove[j, 0].x * 0.01f, accessory.parts[slotNo].addMove[j, 0].y * 0.01f, accessory.parts[slotNo].addMove[j, 0].z * 0.01f);
					gameObject2.transform.localEulerAngles = new Vector3(accessory.parts[slotNo].addMove[j, 1].x, accessory.parts[slotNo].addMove[j, 1].y, accessory.parts[slotNo].addMove[j, 1].z);
					gameObject2.transform.localScale = new Vector3(accessory.parts[slotNo].addMove[j, 2].x, accessory.parts[slotNo].addMove[j, 2].y, accessory.parts[slotNo].addMove[j, 2].z);
				}
			}
		}
		return true;
	}

	public bool UpdateAccessoryMoveAllFromInfo()
	{
		for (int i = 0; i < 20; i++)
		{
			UpdateAccessoryMoveFromInfo(i);
		}
		return true;
	}

	public bool ChangeAccessoryColor(int slotNo)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		ChaAccessoryComponent chaAccessoryComponent = base.cusAcsCmp[slotNo];
		ChaFileAccessory.PartsInfo partsInfo = nowCoordinate.accessory.parts[slotNo];
		if (null == chaAccessoryComponent)
		{
			return false;
		}
		if (chaAccessoryComponent.rendNormal != null)
		{
			Renderer[] rendNormal = chaAccessoryComponent.rendNormal;
			foreach (Renderer renderer in rendNormal)
			{
				if (chaAccessoryComponent.useColor01)
				{
					renderer.material.SetColor(ChaShader._Color, partsInfo.color[0]);
				}
				if (chaAccessoryComponent.useColor02)
				{
					renderer.material.SetColor(ChaShader._Color2, partsInfo.color[1]);
				}
				if (chaAccessoryComponent.useColor03)
				{
					renderer.material.SetColor(ChaShader._Color3, partsInfo.color[2]);
				}
			}
		}
		if (chaAccessoryComponent.rendAlpha != null)
		{
			Renderer[] rendAlpha = chaAccessoryComponent.rendAlpha;
			foreach (Renderer renderer2 in rendAlpha)
			{
				renderer2.material.SetColor(ChaShader._Color4, partsInfo.color[3]);
				renderer2.gameObject.SetActiveIfDifferent((partsInfo.color[3].a != 0f) ? true : false);
			}
		}
		if (chaAccessoryComponent.rendHair != null)
		{
			Color startColor = base.fileHair.parts[0].startColor;
			Renderer[] rendHair = chaAccessoryComponent.rendHair;
			foreach (Renderer renderer3 in rendHair)
			{
				renderer3.material.SetColor(ChaShader._Color, startColor);
				renderer3.material.SetColor(ChaShader._Color2, startColor);
				renderer3.material.SetColor(ChaShader._Color3, startColor);
			}
		}
		return true;
	}

	public bool GetAccessoryDefaultColor(ref Color color, int slotNo, int no)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return false;
		}
		ChaAccessoryComponent chaAccessoryComponent = base.cusAcsCmp[slotNo];
		if (null == chaAccessoryComponent)
		{
			return false;
		}
		if (no == 0 && chaAccessoryComponent.useColor01)
		{
			color = chaAccessoryComponent.defColor01;
			return true;
		}
		if (no == 1 && chaAccessoryComponent.useColor02)
		{
			color = chaAccessoryComponent.defColor02;
			return true;
		}
		if (no == 2 && chaAccessoryComponent.useColor03)
		{
			color = chaAccessoryComponent.defColor03;
			return true;
		}
		if (no == 3 && chaAccessoryComponent.rendAlpha != null && chaAccessoryComponent.rendAlpha.Length != 0)
		{
			color = chaAccessoryComponent.defColor04;
			return true;
		}
		return false;
	}

	public void SetAccessoryDefaultColor(int slotNo)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			return;
		}
		ChaAccessoryComponent chaAccessoryComponent = base.cusAcsCmp[slotNo];
		if (!(null == chaAccessoryComponent))
		{
			if (chaAccessoryComponent.useColor01)
			{
				nowCoordinate.accessory.parts[slotNo].color[0] = chaAccessoryComponent.defColor01;
			}
			if (chaAccessoryComponent.useColor02)
			{
				nowCoordinate.accessory.parts[slotNo].color[1] = chaAccessoryComponent.defColor02;
			}
			if (chaAccessoryComponent.useColor03)
			{
				nowCoordinate.accessory.parts[slotNo].color[2] = chaAccessoryComponent.defColor03;
			}
			if (chaAccessoryComponent.rendAlpha != null && chaAccessoryComponent.rendAlpha.Length != 0)
			{
				nowCoordinate.accessory.parts[slotNo].color[3] = chaAccessoryComponent.defColor04;
			}
		}
	}

	public void SetHideHairAccessory()
	{
		for (int i = 0; i < hideHairAcs.Length; i++)
		{
			hideHairAcs[i] = false;
		}
		for (int j = 0; j < base.infoAccessory.Length; j++)
		{
			if (base.infoAccessory[j] != null && base.infoAccessory[j].GetInfoInt(ChaListDefine.KeyType.HideHair) != 0)
			{
				hideHairAcs[j] = true;
			}
		}
	}

	public bool CheckHideHair()
	{
		for (int i = 0; i < hideHairAcs.Length; i++)
		{
			if (hideHairAcs[i] && base.fileStatus.showAccessory[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool ChangeCustomClothes(bool main, int kind, bool updateColor, bool updateTex01, bool updateTex02, bool updateTex03, bool updateTex04)
	{
		CustomTextureCreate[] array = new CustomTextureCreate[2]
		{
			(!main) ? base.ctCreateClothesSub[kind, 0] : base.ctCreateClothes[kind, 0],
			(!main) ? base.ctCreateClothesSub[kind, 1] : base.ctCreateClothes[kind, 1]
		};
		if (array[0] == null)
		{
			return false;
		}
		ChaClothesComponent chaClothesComponent = ((!main) ? GetCustomClothesSubComponent(kind) : GetCustomClothesComponent(kind));
		if (null == chaClothesComponent)
		{
			return false;
		}
		ChaFileClothes.PartsInfo partsInfo = ((!main) ? nowCoordinate.clothes.parts[0] : nowCoordinate.clothes.parts[kind]);
		if (main)
		{
			if (!updateColor && !updateTex01 && !updateTex02 && !updateTex03)
			{
				return false;
			}
		}
		else if (!updateColor && !updateTex01 && !updateTex02 && !updateTex03 && !updateTex04)
		{
			return false;
		}
		bool result = true;
		int[] array2 = new int[4]
		{
			ChaShader._PatternMask1,
			ChaShader._PatternMask2,
			ChaShader._PatternMask3,
			ChaShader._PatternMask1
		};
		bool[] array3 = new bool[4] { updateTex01, updateTex02, updateTex03, updateTex04 };
		int num = ((main || kind != 2) ? 3 : 4);
		for (int i = 0; i < num; i++)
		{
			if (!array3[i])
			{
				continue;
			}
			Texture2D texture2D = null;
			ListInfoBase listInfo = base.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.mt_pattern, partsInfo.colorInfo[i].pattern);
			if (listInfo != null)
			{
				string info = listInfo.GetInfo(ChaListDefine.KeyType.MainTexAB);
				string text = listInfo.GetInfo(ChaListDefine.KeyType.MainTex);
				if ("0" != info && "0" != text)
				{
					if (!base.hiPoly)
					{
						text += "_low";
					}
					texture2D = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
					Singleton<Character>.Instance.AddLoadAssetBundle(info, string.Empty);
				}
			}
			if (null != texture2D)
			{
				CustomTextureCreate[] array4 = array;
				foreach (CustomTextureCreate customTextureCreate in array4)
				{
					if (customTextureCreate != null)
					{
						customTextureCreate.SetTexture(array2[i], texture2D);
					}
				}
				continue;
			}
			CustomTextureCreate[] array5 = array;
			foreach (CustomTextureCreate customTextureCreate2 in array5)
			{
				if (customTextureCreate2 != null)
				{
					customTextureCreate2.SetTexture(array2[i], null);
				}
			}
		}
		if (updateColor)
		{
			CustomTextureCreate[] array6 = array;
			foreach (CustomTextureCreate customTextureCreate3 in array6)
			{
				if (customTextureCreate3 == null)
				{
					break;
				}
				if (!main && kind == 2)
				{
					customTextureCreate3.SetColor(ChaShader._Color, partsInfo.colorInfo[3].baseColor);
					customTextureCreate3.SetColor(ChaShader._Color1_2, partsInfo.colorInfo[3].patternColor);
					customTextureCreate3.SetFloat(ChaShader._PatternScale1u, partsInfo.colorInfo[3].tiling.x);
					customTextureCreate3.SetFloat(ChaShader._PatternScale1v, partsInfo.colorInfo[3].tiling.y);
				}
				else
				{
					customTextureCreate3.SetColor(ChaShader._Color, partsInfo.colorInfo[0].baseColor);
					customTextureCreate3.SetColor(ChaShader._Color1_2, partsInfo.colorInfo[0].patternColor);
					customTextureCreate3.SetFloat(ChaShader._PatternScale1u, partsInfo.colorInfo[0].tiling.x);
					customTextureCreate3.SetFloat(ChaShader._PatternScale1v, partsInfo.colorInfo[0].tiling.y);
				}
				customTextureCreate3.SetColor(ChaShader._Color2, partsInfo.colorInfo[1].baseColor);
				customTextureCreate3.SetColor(ChaShader._Color2_2, partsInfo.colorInfo[1].patternColor);
				customTextureCreate3.SetFloat(ChaShader._PatternScale2u, partsInfo.colorInfo[1].tiling.x);
				customTextureCreate3.SetFloat(ChaShader._PatternScale2v, partsInfo.colorInfo[1].tiling.y);
				customTextureCreate3.SetColor(ChaShader._Color3, partsInfo.colorInfo[2].baseColor);
				customTextureCreate3.SetColor(ChaShader._Color3_2, partsInfo.colorInfo[2].patternColor);
				customTextureCreate3.SetFloat(ChaShader._PatternScale3u, partsInfo.colorInfo[2].tiling.x);
				customTextureCreate3.SetFloat(ChaShader._PatternScale3v, partsInfo.colorInfo[2].tiling.y);
			}
		}
		bool flag = chaClothesComponent.rendNormal01 != null && 0 != chaClothesComponent.rendNormal01.Length;
		bool flag2 = chaClothesComponent.rendAlpha01 != null && 0 != chaClothesComponent.rendAlpha01.Length;
		if (flag || flag2)
		{
			Texture texture = array[0].RebuildTextureAndSetMaterial();
			if (null != texture)
			{
				if (flag)
				{
					for (int m = 0; m < chaClothesComponent.rendNormal01.Length; m++)
					{
						if ((bool)chaClothesComponent.rendNormal01[m])
						{
							chaClothesComponent.rendNormal01[m].material.SetTexture(ChaShader._MainTex, texture);
						}
						else
						{
							result = false;
						}
					}
				}
				if (flag2)
				{
					for (int n = 0; n < chaClothesComponent.rendAlpha01.Length; n++)
					{
						if ((bool)chaClothesComponent.rendAlpha01[n])
						{
							chaClothesComponent.rendAlpha01[n].material.SetTexture(ChaShader._MainTex, texture);
						}
						else
						{
							result = false;
						}
					}
				}
			}
			else
			{
				result = false;
			}
		}
		else
		{
			result = false;
		}
		if (chaClothesComponent.rendNormal02 != null && chaClothesComponent.rendNormal02.Length != 0 && array[1] != null)
		{
			Texture texture2 = array[1].RebuildTextureAndSetMaterial();
			if (null != texture2)
			{
				for (int num2 = 0; num2 < chaClothesComponent.rendNormal02.Length; num2++)
				{
					if ((bool)chaClothesComponent.rendNormal02[num2])
					{
						chaClothesComponent.rendNormal02[num2].material.SetTexture(ChaShader._MainTex, texture2);
					}
					else
					{
						result = false;
					}
				}
			}
			else
			{
				result = false;
			}
		}
		if (null != chaClothesComponent.rendAccessory && array[0] != null)
		{
			Texture texture3 = array[0].RebuildTextureAndSetMaterial();
			if (null != texture3)
			{
				if ((bool)chaClothesComponent.rendAccessory)
				{
					chaClothesComponent.rendAccessory.material.SetTexture(ChaShader._MainTex, texture3);
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public bool UpdateClothesSiru(int kind, float frontTop, float frontBot, float downTop, float downBot)
	{
		int num = 0;
		if (kind == 0)
		{
			Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
			ListInfoBase value;
			if (categoryInfo.TryGetValue(nowCoordinate.clothes.parts[0].id, out value))
			{
				num = value.Kind;
			}
		}
		ChaClothesComponent[] array = null;
		switch (num)
		{
		case 1:
			array = new ChaClothesComponent[1] { GetCustomClothesSubComponent(0) };
			if (null == array[0])
			{
				return false;
			}
			break;
		case 2:
			array = new ChaClothesComponent[2]
			{
				GetCustomClothesSubComponent(0),
				GetCustomClothesSubComponent(1)
			};
			if (null == array[0] && null == array[1])
			{
				return false;
			}
			break;
		default:
			array = new ChaClothesComponent[1] { GetCustomClothesComponent(kind) };
			if (null == array[0])
			{
				return false;
			}
			break;
		}
		ChaClothesComponent[] array2 = array;
		foreach (ChaClothesComponent chaClothesComponent in array2)
		{
			if (null == chaClothesComponent)
			{
				continue;
			}
			if (chaClothesComponent.rendNormal01 != null && chaClothesComponent.rendNormal01.Length != 0)
			{
				Renderer[] rendNormal = chaClothesComponent.rendNormal01;
				foreach (Renderer renderer in rendNormal)
				{
					if (!(null == renderer))
					{
						renderer.material.SetFloat(ChaShader._liquidftop, frontTop);
						renderer.material.SetFloat(ChaShader._liquidfbot, frontBot);
						renderer.material.SetFloat(ChaShader._liquidbtop, downTop);
						renderer.material.SetFloat(ChaShader._liquidbbot, downBot);
					}
				}
			}
			if (chaClothesComponent.rendNormal02 == null || chaClothesComponent.rendNormal02.Length == 0)
			{
				continue;
			}
			Renderer[] rendNormal2 = chaClothesComponent.rendNormal02;
			foreach (Renderer renderer2 in rendNormal2)
			{
				if (!(null == renderer2))
				{
					renderer2.material.SetFloat(ChaShader._liquidftop, frontTop);
					renderer2.material.SetFloat(ChaShader._liquidfbot, frontBot);
					renderer2.material.SetFloat(ChaShader._liquidbtop, downTop);
					renderer2.material.SetFloat(ChaShader._liquidbbot, downBot);
				}
			}
		}
		return true;
	}

	public Color GetClothesDefaultColor(int kind, int no)
	{
		int num = 0;
		if (kind == 0)
		{
			Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
			ListInfoBase value;
			if (categoryInfo.TryGetValue(nowCoordinate.clothes.parts[0].id, out value))
			{
				num = value.Kind;
			}
		}
		if (num == 1 || num == 2)
		{
			return ChaFileDefine.defClothesSubColor[no];
		}
		ChaClothesComponent customClothesComponent = GetCustomClothesComponent(kind);
		if (null != customClothesComponent)
		{
			switch (no)
			{
			case 0:
				return customClothesComponent.defMainColor01;
			case 1:
				return customClothesComponent.defMainColor02;
			case 2:
				return customClothesComponent.defMainColor03;
			}
		}
		return Color.white;
	}

	public void SetClothesDefaultColor(int kind)
	{
		int num = 0;
		if (kind == 0)
		{
			Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
			ListInfoBase value;
			if (categoryInfo.TryGetValue(nowCoordinate.clothes.parts[0].id, out value))
			{
				num = value.Kind;
			}
		}
		if (num == 1 || num == 2)
		{
			for (int i = 0; i < 4; i++)
			{
				nowCoordinate.clothes.parts[kind].colorInfo[i].baseColor = ChaFileDefine.defClothesSubColor[i];
			}
			return;
		}
		ChaClothesComponent customClothesComponent = GetCustomClothesComponent(kind);
		if (null != customClothesComponent)
		{
			nowCoordinate.clothes.parts[kind].colorInfo[0].baseColor = customClothesComponent.defMainColor01;
			nowCoordinate.clothes.parts[kind].colorInfo[1].baseColor = customClothesComponent.defMainColor02;
			nowCoordinate.clothes.parts[kind].colorInfo[2].baseColor = customClothesComponent.defMainColor03;
		}
	}

	public bool IsEmblem(int kind)
	{
		ChaClothesComponent[] array = null;
		int num = 0;
		if (kind == 0)
		{
			Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
			ListInfoBase value;
			if (categoryInfo.TryGetValue(nowCoordinate.clothes.parts[0].id, out value))
			{
				num = value.Kind;
			}
		}
		if (num != 1 && num != 2)
		{
			array = new ChaClothesComponent[1] { GetCustomClothesComponent(kind) };
		}
		else
		{
			array = new ChaClothesComponent[Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetCustomClothesSubComponent(i);
			}
		}
		return IsEmblem(array);
	}

	public bool IsEmblem(ChaClothesComponent[] ccc)
	{
		if (ccc == null || ccc.Length == 0)
		{
			return false;
		}
		foreach (ChaClothesComponent chaClothesComponent in ccc)
		{
			if (!(null == chaClothesComponent) && !(null == chaClothesComponent.rendEmblem01))
			{
				return true;
			}
		}
		return false;
	}

	public bool ChangeCustomEmblem(int kind)
	{
		ChaClothesComponent[] array = null;
		int num = 0;
		ListInfoBase value;
		if (kind == 0)
		{
			Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
			if (categoryInfo.TryGetValue(nowCoordinate.clothes.parts[0].id, out value))
			{
				num = value.Kind;
			}
		}
		if (num != 1 && num != 2)
		{
			array = new ChaClothesComponent[1] { GetCustomClothesComponent(kind) };
		}
		else
		{
			array = new ChaClothesComponent[Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = GetCustomClothesSubComponent(i);
			}
		}
		if (!IsEmblem(array))
		{
			return false;
		}
		ChaFileClothes.PartsInfo partsInfo = nowCoordinate.clothes.parts[kind];
		Texture2D value2 = null;
		value = base.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.mt_emblem, partsInfo.emblemeId);
		if (value != null)
		{
			string info = value.GetInfo(ChaListDefine.KeyType.MainTexAB);
			string text = value.GetInfo(ChaListDefine.KeyType.MainTex);
			if ("0" != info && "0" != text)
			{
				if (!base.hiPoly)
				{
					text += "_low";
				}
				value2 = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
				Singleton<Character>.Instance.AddLoadAssetBundle(info, string.Empty);
			}
		}
		ChaClothesComponent[] array2 = array;
		foreach (ChaClothesComponent chaClothesComponent in array2)
		{
			if (!(null == chaClothesComponent))
			{
				if (null != chaClothesComponent.rendEmblem01)
				{
					chaClothesComponent.rendEmblem01.material.SetTexture(ChaShader._MainTex, value2);
				}
				if (null != chaClothesComponent.rendEmblem02)
				{
					chaClothesComponent.rendEmblem02.material.SetTexture(ChaShader._MainTex, value2);
				}
			}
		}
		return true;
	}

	protected void InitializeControlCustomAll()
	{
		ShapeFaceNum = ChaFileDefine.cf_headshapename.Length;
		ShapeBodyNum = ChaFileDefine.cf_bodyshapename.Length;
		InitializeControlCustomObject();
	}

	protected void InitializeControlCustomObject()
	{
		sibFace = new ShapeHeadInfoFemale();
		sibBody = new ShapeBodyInfoFemale();
		changeShapeBodyMask = false;
		bustSoft = new BustSoft(this);
		bustGravity = new BustGravity(this);
		updateCMFaceTex = new bool[Enum.GetNames(typeof(FaceTexKind)).Length];
		updateCMFaceColor = new bool[Enum.GetNames(typeof(FaceTexKind)).Length];
		updateCMFaceLayout = new bool[Enum.GetNames(typeof(FaceTexKind)).Length];
		updateCMBodyTex = new bool[Enum.GetNames(typeof(BodyTexKind)).Length];
		updateCMBodyColor = new bool[Enum.GetNames(typeof(BodyTexKind)).Length];
		updateCMBodyLayout = new bool[Enum.GetNames(typeof(BodyTexKind)).Length];
	}

	protected void ReleaseControlCustomAll()
	{
		ReleaseControlCustomObject();
	}

	protected void ReleaseControlCustomObject(bool init = true)
	{
		if (sibFace != null)
		{
			sibFace.ReleaseShapeInfo();
		}
		if (sibBody != null)
		{
			sibBody.ReleaseShapeInfo();
		}
		if (init)
		{
			InitializeControlCustomObject();
		}
	}

	private bool SetBaseMaterial(Renderer rend, Material mat)
	{
		if (null == mat || null == rend)
		{
			return false;
		}
		int num = rend.materials.Length;
		Material[] array = null;
		if (num == 0)
		{
			array = new Material[1] { mat };
		}
		else
		{
			array = new Material[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = rend.materials[i];
			}
			Material material = array[0];
			array[0] = mat;
			if (material != mat)
			{
				UnityEngine.Object.Destroy(material);
			}
		}
		rend.materials = array;
		return true;
	}

	private bool SetCreateTexture(CustomTextureCreate ctc, bool main, ChaListDefine.CategoryNo type, int id, ChaListDefine.KeyType assetBundleKey, ChaListDefine.KeyType assetKey, int propertyID)
	{
		ListInfoBase listInfo = base.lstCtrl.GetListInfo(type, id);
		if (listInfo != null)
		{
			string info = listInfo.GetInfo(assetBundleKey);
			string text = listInfo.GetInfo(assetKey);
			Texture2D texture2D = null;
			if ("0" != info && "0" != text)
			{
				if (!base.hiPoly)
				{
					text += "_low";
				}
				texture2D = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
				Singleton<Character>.Instance.AddLoadAssetBundle(info, string.Empty);
			}
			if (main)
			{
				ctc.SetMainTexture(texture2D);
			}
			else
			{
				ctc.SetTexture(propertyID, texture2D);
			}
			return true;
		}
		return false;
	}

	private void ChangeTexture(Renderer rend, ChaListDefine.CategoryNo type, int id, ChaListDefine.KeyType assetBundleKey, ChaListDefine.KeyType assetKey, int propertyID)
	{
		if (!(null == rend))
		{
			ChangeTexture(rend.material, type, id, assetBundleKey, assetKey, propertyID);
		}
	}

	private void ChangeTexture(Material mat, ChaListDefine.CategoryNo type, int id, ChaListDefine.KeyType assetBundleKey, ChaListDefine.KeyType assetKey, int propertyID)
	{
		if (!(null == mat))
		{
			Texture2D texture = GetTexture(type, id, assetBundleKey, assetKey);
			mat.SetTexture(propertyID, texture);
		}
	}

	private Texture2D GetTexture(ChaListDefine.CategoryNo type, int id, ChaListDefine.KeyType assetBundleKey, ChaListDefine.KeyType assetKey)
	{
		ListInfoBase listInfo = base.lstCtrl.GetListInfo(type, id);
		if (listInfo != null)
		{
			string info = listInfo.GetInfo(assetBundleKey);
			string text = listInfo.GetInfo(assetKey);
			Texture2D result = null;
			if ("0" != info && "0" != text)
			{
				if (!base.hiPoly)
				{
					text += "_low";
				}
				result = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
				Singleton<Character>.Instance.AddLoadAssetBundle(info, string.Empty);
			}
			return result;
		}
		return null;
	}

	public void AddUpdateCMFaceTexFlags(bool inpBase, bool inpSub, bool inpPaint01, bool inpPaint02, bool inpCheek, bool inpLipLine, bool inpMole)
	{
		if (inpBase)
		{
			updateCMFaceTex[0] = inpBase;
		}
		if (inpSub)
		{
			updateCMFaceTex[1] = inpSub;
		}
		if (inpPaint01)
		{
			updateCMFaceTex[2] = inpPaint01;
		}
		if (inpPaint02)
		{
			updateCMFaceTex[3] = inpPaint02;
		}
		if (inpCheek)
		{
			updateCMFaceTex[4] = inpCheek;
		}
		if (inpLipLine)
		{
			updateCMFaceTex[5] = inpLipLine;
		}
		if (inpMole)
		{
			updateCMFaceTex[6] = inpMole;
		}
	}

	public void AddUpdateCMFaceColorFlags(bool inpBase, bool inpSub, bool inpPaint01, bool inpPaint02, bool inpCheek, bool inpLipLine, bool inpMole)
	{
		if (inpBase)
		{
			updateCMFaceColor[0] = inpBase;
		}
		if (inpSub)
		{
			updateCMFaceColor[1] = inpSub;
		}
		if (inpPaint01)
		{
			updateCMFaceColor[2] = inpPaint01;
		}
		if (inpPaint02)
		{
			updateCMFaceColor[3] = inpPaint02;
		}
		if (inpCheek)
		{
			updateCMFaceColor[4] = inpCheek;
		}
		if (inpLipLine)
		{
			updateCMFaceColor[5] = inpLipLine;
		}
		if (inpMole)
		{
			updateCMFaceColor[6] = inpMole;
		}
	}

	public void AddUpdateCMFaceLayoutFlags(bool inpPaint01, bool inpPaint02, bool inpMole)
	{
		if (inpPaint01)
		{
			updateCMFaceLayout[2] = inpPaint01;
		}
		if (inpPaint02)
		{
			updateCMFaceLayout[3] = inpPaint02;
		}
		if (inpMole)
		{
			updateCMFaceLayout[6] = inpMole;
		}
	}

	public bool CreateFaceTexture()
	{
		ChaFileMakeup chaFileMakeup = ((!nowCoordinate.enableMakeup) ? base.fileFace.baseMakeup : nowCoordinate.makeup);
		ListInfoBase listInfoBase = null;
		bool flag = false;
		if (updateCMFaceTex[0])
		{
			if ((bool)base.objHead)
			{
				ListInfoComponent component = base.objHead.GetComponent<ListInfoComponent>();
				if (null != component)
				{
					listInfoBase = component.data;
					string info = listInfoBase.GetInfo(ChaListDefine.KeyType.MainTexAB);
					string text = listInfoBase.GetInfo(ChaListDefine.KeyType.MainTex);
					if ("0" != text && !base.hiPoly)
					{
						text += "_low";
					}
					Texture2D mainTexture = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
					Singleton<Character>.Instance.AddLoadAssetBundle(info, string.Empty);
					base.customTexCtrlFace.SetMainTexture(mainTexture);
					flag = true;
				}
			}
			updateCMFaceTex[0] = false;
		}
		if (updateCMFaceColor[0])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color, base.fileBody.skinMainColor);
			flag = true;
			updateCMFaceColor[0] = false;
		}
		if (updateCMFaceTex[1])
		{
			if ((bool)base.objHead)
			{
				ListInfoComponent component2 = base.objHead.GetComponent<ListInfoComponent>();
				if (null != component2)
				{
					listInfoBase = component2.data;
					string info2 = listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMaskAB);
					string info3 = listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMaskTex);
					if ("0" != info3 && !base.hiPoly)
					{
						info3 += "_low";
					}
					Texture2D tex = CommonLib.LoadAsset<Texture2D>(info2, listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMaskTex), false, string.Empty);
					Singleton<Character>.Instance.AddLoadAssetBundle(info2, string.Empty);
					base.customTexCtrlFace.SetTexture(ChaShader._ColorMask, tex);
					flag = true;
				}
			}
			updateCMFaceTex[1] = false;
		}
		if (updateCMFaceColor[1])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color2, base.fileBody.skinSubColor);
			flag = true;
			updateCMFaceColor[1] = false;
		}
		if (updateCMFaceTex[2])
		{
			if (SetCreateTexture(base.customTexCtrlFace, false, ChaListDefine.CategoryNo.mt_face_paint, chaFileMakeup.paintId[0], ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.PaintTex, ChaShader._Texture3))
			{
				flag = true;
			}
			updateCMFaceTex[2] = false;
		}
		if (updateCMFaceColor[2])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color3, chaFileMakeup.paintColor[0]);
			updateCMFaceColor[2] = false;
			flag = true;
		}
		if (updateCMFaceLayout[2])
		{
			Vector4 zero = Vector4.zero;
			zero.x = Mathf.Lerp(0.25f, -0.25f, chaFileMakeup.paintLayout[0].x);
			zero.y = Mathf.Lerp(0.3f, -0.3f, chaFileMakeup.paintLayout[0].y);
			zero.z = Mathf.Lerp(1f, -1f, chaFileMakeup.paintLayout[0].z);
			zero.w = Mathf.Lerp(-8f, 0.7f, chaFileMakeup.paintLayout[0].w);
			base.customTexCtrlFace.SetVector4(ChaShader._paint1, zero);
			updateCMFaceLayout[2] = false;
			flag = true;
		}
		if (updateCMFaceTex[3])
		{
			if (SetCreateTexture(base.customTexCtrlFace, false, ChaListDefine.CategoryNo.mt_face_paint, chaFileMakeup.paintId[1], ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.PaintTex, ChaShader._Texture7))
			{
				flag = true;
			}
			updateCMFaceTex[3] = false;
		}
		if (updateCMFaceColor[3])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color7, chaFileMakeup.paintColor[1]);
			updateCMFaceColor[3] = false;
			flag = true;
		}
		if (updateCMFaceLayout[3])
		{
			Vector4 zero2 = Vector4.zero;
			zero2.x = Mathf.Lerp(0.25f, -0.25f, chaFileMakeup.paintLayout[1].x);
			zero2.y = Mathf.Lerp(0.3f, -0.3f, chaFileMakeup.paintLayout[1].y);
			zero2.z = Mathf.Lerp(1f, -1f, chaFileMakeup.paintLayout[1].z);
			zero2.w = Mathf.Lerp(-8f, 0.7f, chaFileMakeup.paintLayout[1].w);
			base.customTexCtrlFace.SetVector4(ChaShader._paint2, zero2);
			updateCMFaceLayout[3] = false;
			flag = true;
		}
		if (updateCMFaceTex[4])
		{
			if (SetCreateTexture(base.customTexCtrlFace, false, ChaListDefine.CategoryNo.mt_cheek, chaFileMakeup.cheekId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.CheekTex, ChaShader._Texture4))
			{
				flag = true;
			}
			updateCMFaceTex[4] = false;
		}
		if (updateCMFaceColor[4])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color4, chaFileMakeup.cheekColor);
			updateCMFaceColor[4] = false;
			flag = true;
		}
		if (updateCMFaceTex[5])
		{
			if (SetCreateTexture(base.customTexCtrlFace, false, ChaListDefine.CategoryNo.mt_lipline, base.fileFace.lipLineId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.LiplineTex, ChaShader._Texture5))
			{
				flag = true;
			}
			updateCMFaceTex[5] = false;
		}
		if (updateCMFaceColor[5])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color5, base.fileFace.lipLineColor);
			updateCMFaceColor[5] = false;
			flag = true;
		}
		if (updateCMFaceTex[6])
		{
			if (SetCreateTexture(base.customTexCtrlFace, false, ChaListDefine.CategoryNo.mt_mole, base.fileFace.moleId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.MoleTex, ChaShader._Texture6))
			{
				flag = true;
			}
			updateCMFaceTex[6] = false;
		}
		if (updateCMFaceColor[6])
		{
			base.customTexCtrlFace.SetColor(ChaShader._Color6, base.fileFace.moleColor);
			updateCMFaceColor[6] = false;
			flag = true;
		}
		if (updateCMFaceLayout[6])
		{
			Vector4 zero3 = Vector4.zero;
			zero3.x = Mathf.Lerp(0.25f, -0.25f, base.fileFace.moleLayout.x);
			zero3.y = Mathf.Lerp(0.3f, -0.3f, base.fileFace.moleLayout.y);
			zero3.w = Mathf.Lerp(0f, 0.7f, base.fileFace.moleLayout.w);
			base.customTexCtrlFace.SetVector4(ChaShader._hokuro, zero3);
			updateCMFaceLayout[6] = false;
			flag = true;
		}
		if (flag)
		{
			base.customTexCtrlFace.SetNewCreateTexture();
		}
		if (base.releaseCustomInputTexture)
		{
			ReleaseFaceCustomTexture();
		}
		return true;
	}

	public bool ChangeSettingWhiteOfEye(bool updateTex, bool updateColor)
	{
		if (base.ctCreateEyeW == null)
		{
			return false;
		}
		if (!updateTex && !updateColor)
		{
			return false;
		}
		bool result = true;
		if (updateTex && !SetCreateTexture(base.ctCreateEyeW, true, ChaListDefine.CategoryNo.mt_eye_white, base.fileFace.whiteId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyeWhiteTex, ChaShader._MainTex))
		{
			result = false;
		}
		if (updateColor)
		{
			base.ctCreateEyeW.SetColor(ChaShader._Color, base.fileFace.whiteBaseColor);
			base.ctCreateEyeW.SetColor(ChaShader._Color2, base.fileFace.whiteSubColor);
		}
		Texture texture = base.ctCreateEyeW.RebuildTextureAndSetMaterial();
		if (null != texture && base.rendEyeW != null && base.rendEyeW.Length == 2)
		{
			if (null != base.rendEyeW[0] && null != base.rendEyeW[1])
			{
				base.rendEyeW[0].material.SetTexture(ChaShader._MainTex, texture);
				base.rendEyeW[1].material = base.rendEyeW[0].material;
			}
			else
			{
				result = false;
			}
		}
		else
		{
			result = false;
		}
		if (base.releaseCustomInputTexture)
		{
			base.ctCreateEyeW.SetMainTexture(null);
			base.ctCreateEyeW.SetTexture(ChaShader._MainTex, null);
		}
		return result;
	}

	public bool ChangeSettingEyeL(bool updateBaseTex, bool updateMaskTex, bool updateColorAndOffset)
	{
		return ChangeSettingEye(0, updateBaseTex, updateMaskTex, updateColorAndOffset);
	}

	public bool ChangeSettingEyeR(bool updateBaseTex, bool updateMaskTex, bool updateColorAndOffset)
	{
		return ChangeSettingEye(1, updateBaseTex, updateMaskTex, updateColorAndOffset);
	}

	public bool ChangeSettingEye(bool updateBaseTex, bool updateMaskTex, bool updateColorAndOffset)
	{
		return ChangeSettingEye(2, updateBaseTex, updateMaskTex, updateColorAndOffset);
	}

	public bool ChangeSettingEye(byte lr, bool updateBaseTex, bool updateMaskTex, bool updateColorAndOffset)
	{
		bool result = true;
		bool[] array = new bool[2]
		{
			(lr != 1) ? true : false,
			(lr != 0) ? true : false
		};
		CustomTextureCreate[] array2 = new CustomTextureCreate[2] { base.ctCreateEyeL, base.ctCreateEyeR };
		for (int i = 0; i < 2; i++)
		{
			if (array[i])
			{
				if (array2[i] == null)
				{
					return false;
				}
				if (!updateBaseTex && !updateMaskTex && !updateColorAndOffset)
				{
					return false;
				}
				if (updateBaseTex && !SetCreateTexture(array2[i], true, ChaListDefine.CategoryNo.mt_eye, base.fileFace.pupil[i].id, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyeTex, ChaShader._MainTex))
				{
					result = false;
				}
				if (updateMaskTex && !SetCreateTexture(array2[i], false, ChaListDefine.CategoryNo.mt_eye_gradation, base.fileFace.pupil[i].gradMaskId, ChaListDefine.KeyType.ColorMaskAB, ChaListDefine.KeyType.ColorMaskTex, ChaShader._ColorMask))
				{
					result = false;
				}
				if (updateColorAndOffset)
				{
					array2[i].SetColor(ChaShader._Color, base.fileFace.pupil[i].baseColor);
					array2[i].SetColor(ChaShader._Color2, base.fileFace.pupil[i].subColor);
					array2[i].SetFloat(ChaShader._Blend, base.fileFace.pupil[i].gradBlend);
					float y = Mathf.Lerp(-0.5f, 0.5f, base.fileFace.pupil[i].gradOffsetY);
					float w = Mathf.Lerp(-1f, 1f, base.fileFace.pupil[i].gradScale);
					array2[i].SetVector4(ChaShader._grad, new Vector4(0f, y, 0f, w));
				}
				Texture texture = array2[i].RebuildTextureAndSetMaterial();
				if (null != texture)
				{
					base.rendEye[i].material.SetTexture(ChaShader._MainTex, texture);
				}
				if (base.releaseCustomInputTexture)
				{
					array2[i].SetMainTexture(null);
					array2[i].SetTexture(ChaShader._MainTex, null);
					array2[i].SetTexture(ChaShader._ColorMask, null);
				}
			}
		}
		return result;
	}

	public bool ChangeSettingEyeHLUpPosY()
	{
		float eyeTexHLUpOffsetY = Mathf.Lerp(0.1f, -0.1f, base.fileFace.hlUpY);
		for (int i = 0; i < base.fileFace.pupil.Length; i++)
		{
			if (!(null == base.eyeLookMatCtrl[i]))
			{
				base.eyeLookMatCtrl[i].SetEyeTexHLUpOffsetY(eyeTexHLUpOffsetY);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeHLDownPosY()
	{
		float eyeTexHLDownOffsetY = Mathf.Lerp(0.1f, -0.1f, base.fileFace.hlDownY);
		for (int i = 0; i < base.fileFace.pupil.Length; i++)
		{
			if (!(null == base.eyeLookMatCtrl[i]))
			{
				base.eyeLookMatCtrl[i].SetEyeTexHLDownOffsetY(eyeTexHLDownOffsetY);
			}
		}
		return true;
	}

	public bool ChangeSettingEyePosX()
	{
		float eyeTexOffsetX = Mathf.Lerp(0.2f, -0.6f, base.fileFace.pupilX);
		for (int i = 0; i < base.fileFace.pupil.Length; i++)
		{
			if (!(null == base.eyeLookMatCtrl[i]))
			{
				base.eyeLookMatCtrl[i].SetEyeTexOffsetX(eyeTexOffsetX);
			}
		}
		return true;
	}

	public bool ChangeSettingEyePosY()
	{
		float eyeTexOffsetY = Mathf.Lerp(-0.5f, 0.5f, base.fileFace.pupilY);
		for (int i = 0; i < base.fileFace.pupil.Length; i++)
		{
			if (!(null == base.eyeLookMatCtrl[i]))
			{
				base.eyeLookMatCtrl[i].SetEyeTexOffsetY(eyeTexOffsetY);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeScaleWidth()
	{
		float eyeTexScaleX = Mathf.Lerp(1.8f, -0.2f, base.fileFace.pupilWidth);
		for (int i = 0; i < base.fileFace.pupil.Length; i++)
		{
			if (!(null == base.eyeLookMatCtrl[i]))
			{
				base.eyeLookMatCtrl[i].SetEyeTexScaleX(eyeTexScaleX);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeScaleHeight()
	{
		float eyeTexScaleY = Mathf.Lerp(1.8f, -0.2f, base.fileFace.pupilHeight);
		for (int i = 0; i < base.fileFace.pupil.Length; i++)
		{
			if (!(null == base.eyeLookMatCtrl[i]))
			{
				base.eyeLookMatCtrl[i].SetEyeTexScaleY(eyeTexScaleY);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeTilt()
	{
		if (base.rendEye == null)
		{
			return false;
		}
		int num = 33;
		float[] array = new float[2]
		{
			Mathf.Lerp(0.02f, -0.02f, base.fileFace.shapeValueFace[num]),
			Mathf.Lerp(-0.02f, 0.02f, base.fileFace.shapeValueFace[num])
		};
		for (int i = 0; i < base.rendEye.Length; i++)
		{
			if (!(null == base.rendEye[i]))
			{
				base.rendEye[i].material.SetFloat(ChaShader._rotation, array[i]);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeHiUp()
	{
		if (base.rendEye == null)
		{
			return false;
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(null == base.rendEye[i]))
			{
				ChangeTexture(base.rendEye[i], ChaListDefine.CategoryNo.mt_eye_hi_up, base.fileFace.hlUpId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyeHiUpTex, ChaShader._overtex1);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeHiUpColor()
	{
		if (base.rendEye == null)
		{
			return false;
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(null == base.rendEye[i]))
			{
				base.rendEye[i].material.SetColor(ChaShader._overcolor1, base.fileFace.hlUpColor);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeHiDown()
	{
		if (base.rendEye == null)
		{
			return false;
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(null == base.rendEye[i]))
			{
				ChangeTexture(base.rendEye[i], ChaListDefine.CategoryNo.mt_eye_hi_down, base.fileFace.hlDownId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyeHiDownTex, ChaShader._overtex2);
			}
		}
		return true;
	}

	public bool ChangeSettingEyeHiDownColor()
	{
		if (base.rendEye == null)
		{
			return false;
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(null == base.rendEye[i]))
			{
				base.rendEye[i].material.SetColor(ChaShader._overcolor2, base.fileFace.hlDownColor);
			}
		}
		return true;
	}

	public bool ChangeSettingEyelineUp()
	{
		if (null == base.rendEyelineUp)
		{
			return false;
		}
		if (2 > base.rendEyelineUp.materials.Length)
		{
			return false;
		}
		ChangeTexture(base.rendEyelineUp.materials[0], ChaListDefine.CategoryNo.mt_eyeline_up, base.fileFace.eyelineUpId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyelineUpTex, ChaShader._MainTex);
		ChangeTexture(base.rendEyelineUp.materials[1], ChaListDefine.CategoryNo.mt_eyeline_up, base.fileFace.eyelineUpId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyelineShadowTex, ChaShader._MainTex);
		return true;
	}

	public bool ChangeSettingEyelineColor()
	{
		if (null != base.rendEyelineUp)
		{
			base.rendEyelineUp.materials[0].SetColor(ChaShader._Color, base.fileFace.eyelineColor);
		}
		if (null != base.rendEyelineDown)
		{
			base.rendEyelineDown.material.SetColor(ChaShader._Color, base.fileFace.eyelineColor);
		}
		return true;
	}

	public bool UpdateEyelineShadowColor()
	{
		if (null == base.rendEyelineUp)
		{
			return false;
		}
		if (2 > base.rendEyelineUp.materials.Length)
		{
			return false;
		}
		base.rendEyelineUp.materials[1].SetColor(ChaShader._Color, base.fileBody.skinMainColor);
		return true;
	}

	public bool ChangeSettingEyelineDown()
	{
		if (null == base.rendEyelineDown)
		{
			return false;
		}
		ChangeTexture(base.rendEyelineDown, ChaListDefine.CategoryNo.mt_eyeline_down, base.fileFace.eyelineDownId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyelineDownTex, ChaShader._MainTex);
		return true;
	}

	public bool ChangeSettingEyebrow()
	{
		ChangeTexture(base.rendEyebrow, ChaListDefine.CategoryNo.mt_eyebrow, base.fileFace.eyebrowId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyebrowTex, ChaShader._MainTex);
		return true;
	}

	public bool ChangeSettingEyebrowColor()
	{
		if (null == base.rendEyebrow)
		{
			return false;
		}
		base.rendEyebrow.material.SetColor(ChaShader._Color, base.fileFace.eyebrowColor);
		return true;
	}

	public bool ChangeSettingNose()
	{
		ChangeTexture(base.rendNose, ChaListDefine.CategoryNo.mt_nose, base.fileFace.noseId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.NoseTex, ChaShader._MainTex);
		return true;
	}

	public bool VisibleDoubleTooth()
	{
		GameObject referenceInfo = GetReferenceInfo(RefObjKey.ObjDoubleTooth);
		if ((bool)referenceInfo)
		{
			referenceInfo.SetActiveIfDifferent(base.fileFace.doubleTooth);
		}
		return true;
	}

	public bool ChangeSettingFaceDetail()
	{
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		ChangeTexture(base.customTexCtrlFace.matDraw, ChaListDefine.CategoryNo.mt_face_detail, base.fileFace.detailId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.NormallMapDetail, ChaShader._NormalMapDetail);
		ChangeTexture(base.customTexCtrlFace.matDraw, ChaListDefine.CategoryNo.mt_face_detail, base.fileFace.detailId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.LineMask, ChaShader._LineMask);
		return true;
	}

	public bool ChangeSettingFaceDetailPower()
	{
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		base.customTexCtrlFace.matDraw.SetFloat(ChaShader._DetailNormalMapScale, base.fileFace.detailPower);
		return true;
	}

	public bool ChangeSettingEyeShadow()
	{
		ChaFileMakeup chaFileMakeup = ((!nowCoordinate.enableMakeup) ? base.fileFace.baseMakeup : nowCoordinate.makeup);
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		ChangeTexture(base.customTexCtrlFace.matDraw, ChaListDefine.CategoryNo.mt_eyeshadow, chaFileMakeup.eyeshadowId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.EyeshadowTex, ChaShader._overtex3);
		return true;
	}

	public bool ChangeSettingEyeShadowColor()
	{
		ChaFileMakeup chaFileMakeup = ((!nowCoordinate.enableMakeup) ? base.fileFace.baseMakeup : nowCoordinate.makeup);
		if (base.customTexCtrlFace == null || null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		if (IsGagEyes())
		{
			Color eyeshadowColor = chaFileMakeup.eyeshadowColor;
			eyeshadowColor.a = 0f;
			base.customTexCtrlFace.matDraw.SetColor(ChaShader._overcolor3, eyeshadowColor);
		}
		else
		{
			base.customTexCtrlFace.matDraw.SetColor(ChaShader._overcolor3, chaFileMakeup.eyeshadowColor);
		}
		return true;
	}

	public bool ChangeSettingLip()
	{
		ChaFileMakeup chaFileMakeup = ((!nowCoordinate.enableMakeup) ? base.fileFace.baseMakeup : nowCoordinate.makeup);
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		ChangeTexture(base.customTexCtrlFace.matDraw, ChaListDefine.CategoryNo.mt_lip, chaFileMakeup.lipId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.LipTex, ChaShader._overtex1);
		return true;
	}

	public bool ChangeSettingLipColor()
	{
		ChaFileMakeup chaFileMakeup = ((!nowCoordinate.enableMakeup) ? base.fileFace.baseMakeup : nowCoordinate.makeup);
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		base.customTexCtrlFace.matDraw.SetColor(ChaShader._overcolor1, chaFileMakeup.lipColor);
		return true;
	}

	public bool ChangeSettingCheekGlossPower()
	{
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		base.customTexCtrlFace.matDraw.SetFloat(ChaShader._SpecularPower, base.fileFace.cheekGlossPower);
		return true;
	}

	public bool ChangeSettingLipGlossPower()
	{
		if (null == base.customTexCtrlFace.matDraw)
		{
			return false;
		}
		base.customTexCtrlFace.matDraw.SetFloat(ChaShader._SpecularPowerNail, base.fileFace.lipGlossPower);
		return true;
	}

	public bool SetFaceBaseMaterial()
	{
		if (null == base.customMatFace)
		{
			return false;
		}
		return SetBaseMaterial(base.rendFace, base.customMatFace);
	}

	public bool ReleaseFaceCustomTexture()
	{
		if (base.customTexCtrlFace == null)
		{
			return false;
		}
		base.customTexCtrlFace.SetTexture(ChaShader._MainTex, null);
		base.customTexCtrlFace.SetTexture(ChaShader._ColorMask, null);
		base.customTexCtrlFace.SetTexture(ChaShader._Texture3, null);
		base.customTexCtrlFace.SetTexture(ChaShader._Texture4, null);
		base.customTexCtrlFace.SetTexture(ChaShader._Texture5, null);
		base.customTexCtrlFace.SetTexture(ChaShader._Texture6, null);
		base.customTexCtrlFace.SetTexture(ChaShader._Texture7, null);
		Resources.UnloadUnusedAssets();
		return true;
	}

	public void ChangeCustomFaceWithoutCustomTexture()
	{
		ChangeSettingEyebrow();
		ChangeSettingEyebrowColor();
		ChangeSettingEyeHiUp();
		ChangeSettingEyeHiUpColor();
		ChangeSettingEyeHiDown();
		ChangeSettingEyeHiDownColor();
		ChangeSettingEyeHLUpPosY();
		ChangeSettingEyeHLDownPosY();
		ChangeSettingEyePosX();
		ChangeSettingEyePosY();
		ChangeSettingEyeScaleWidth();
		ChangeSettingEyeScaleHeight();
		ChangeSettingEyeTilt();
		ChangeSettingNose();
		ChangeSettingEyelineUp();
		ChangeSettingEyelineDown();
		ChangeSettingEyelineColor();
		UpdateEyelineShadowColor();
		VisibleDoubleTooth();
		ChangeSettingFaceDetail();
		ChangeSettingFaceDetailPower();
		ChangeSettingLip();
		ChangeSettingLipColor();
		ChangeSettingEyeShadow();
		ChangeSettingEyeShadowColor();
		ChangeSettingCheekGlossPower();
		ChangeSettingLipGlossPower();
	}

	public void ChangeSettingHairColor(int parts, bool c00, bool c01, bool c02)
	{
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (null == customHairComponent || customHairComponent.rendHair == null || customHairComponent.rendHair.Length == 0)
		{
			return;
		}
		ChaFileHair hair = base.chaFile.custom.hair;
		for (int i = 0; i < customHairComponent.rendHair.Length; i++)
		{
			if (c00)
			{
				if (1f > hair.parts[parts].baseColor.a)
				{
					hair.parts[parts].baseColor = new Color(hair.parts[parts].baseColor.r, hair.parts[parts].baseColor.g, hair.parts[parts].baseColor.b, 1f);
				}
				customHairComponent.rendHair[i].material.SetColor(ChaShader._Color, hair.parts[parts].baseColor);
			}
			if (c01)
			{
				if (1f > hair.parts[parts].startColor.a)
				{
					hair.parts[parts].startColor = new Color(hair.parts[parts].startColor.r, hair.parts[parts].startColor.g, hair.parts[parts].startColor.b, 1f);
				}
				customHairComponent.rendHair[i].material.SetColor(ChaShader._Color2, hair.parts[parts].startColor);
			}
			if (c02)
			{
				if (1f > hair.parts[parts].endColor.a)
				{
					hair.parts[parts].endColor = new Color(hair.parts[parts].endColor.r, hair.parts[parts].endColor.g, hair.parts[parts].endColor.b, 1f);
				}
				customHairComponent.rendHair[i].material.SetColor(ChaShader._Color3, hair.parts[parts].endColor);
			}
		}
	}

	public void ChangeSettingHairOutlineColor(int parts)
	{
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (!(null == customHairComponent) && customHairComponent.rendHair != null && customHairComponent.rendHair.Length != 0)
		{
			ChaFileHair hair = base.chaFile.custom.hair;
			for (int i = 0; i < customHairComponent.rendHair.Length; i++)
			{
				customHairComponent.rendHair[i].material.SetColor(ChaShader._LineColor, hair.parts[parts].outlineColor);
			}
		}
	}

	public int GetHairAcsColorNum(int parts)
	{
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (null == customHairComponent || customHairComponent.acsDefColor == null || customHairComponent.acsDefColor.Length == 0)
		{
			return 0;
		}
		return customHairComponent.acsDefColor.Length;
	}

	public void SetAcsDefaultColorParameterOnly(int parts)
	{
		ChaFileHair hair = base.chaFile.custom.hair;
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (!(null == customHairComponent))
		{
			int num = customHairComponent.acsDefColor.Length;
			for (int i = 0; i < num; i++)
			{
				hair.parts[parts].acsColor[i] = customHairComponent.acsDefColor[i];
			}
		}
	}

	public void ChangeSettingHairAcsColor(int parts)
	{
		int hairAcsColorNum = GetHairAcsColorNum(parts);
		if (hairAcsColorNum == 0)
		{
			return;
		}
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (null == customHairComponent)
		{
			return;
		}
		int[] array = new int[3]
		{
			ChaShader._Color,
			ChaShader._Color2,
			ChaShader._Color3
		};
		ChaFileHair hair = base.chaFile.custom.hair;
		for (int i = 0; i < customHairComponent.rendAccessory.Length; i++)
		{
			for (int j = 0; j < hairAcsColorNum; j++)
			{
				if (1f > hair.parts[parts].acsColor[j].a)
				{
					hair.parts[parts].acsColor[j] = new Color(hair.parts[parts].acsColor[j].r, hair.parts[parts].acsColor[j].g, hair.parts[parts].acsColor[j].b, 1f);
				}
				customHairComponent.rendAccessory[i].material.SetColor(array[j], hair.parts[parts].acsColor[j]);
			}
		}
	}

	public void ChangeSettingHairLength(int parts)
	{
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (!(null == customHairComponent))
		{
			ChaFileHair hair = base.chaFile.custom.hair;
			customHairComponent.lengthRate = hair.parts[parts].length;
		}
	}

	public bool ChangeSettingHairFrontLength()
	{
		int num = 1;
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(num);
		if (null == customHairComponent)
		{
			return false;
		}
		ChaFileHair hair = base.chaFile.custom.hair;
		customHairComponent.lengthRate = hair.parts[num].length;
		return true;
	}

	public void LoadHairGlossMask()
	{
		ChaFileHair hair = base.chaFile.custom.hair;
		texHairGloss = GetTexture(ChaListDefine.CategoryNo.mt_hairgloss, hair.glossId, ChaListDefine.KeyType.MainTexAB, ChaListDefine.KeyType.MainTex);
	}

	public bool ChangeSettingHairGlossMaskAll()
	{
		int[] array = (int[])Enum.GetValues(typeof(ChaFileDefine.HairKind));
		int[] array2 = array;
		foreach (int parts in array2)
		{
			ChangeSettingHairGlossMask(parts);
		}
		return true;
	}

	public void ChangeSettingHairGlossMask(int parts)
	{
		ChaCustomHairComponent customHairComponent = GetCustomHairComponent(parts);
		if (null == customHairComponent || null == customHairComponent || customHairComponent.rendHair == null || customHairComponent.rendHair.Length == 0)
		{
			return;
		}
		for (int i = 0; i < customHairComponent.rendHair.Length; i++)
		{
			if (!(null == customHairComponent.rendHair[i]))
			{
				customHairComponent.rendHair[i].material.SetTexture(ChaShader._HairGloss, texHairGloss);
			}
		}
	}

	public void AddUpdateCMBodyTexFlags(bool inpBase, bool inpSub, bool inpPaint01, bool inpPaint02, bool inpSunburn)
	{
		if (inpBase)
		{
			updateCMBodyTex[0] = inpBase;
		}
		if (inpSub)
		{
			updateCMBodyTex[1] = inpSub;
		}
		if (inpPaint01)
		{
			updateCMBodyTex[2] = inpPaint01;
		}
		if (inpPaint02)
		{
			updateCMBodyTex[3] = inpPaint02;
		}
		if (inpSunburn)
		{
			updateCMBodyTex[4] = inpSunburn;
		}
	}

	public void AddUpdateCMBodyColorFlags(bool inpBase, bool inpSub, bool inpPaint01, bool inpPaint02, bool inpSunburn, bool inpNail)
	{
		if (inpBase)
		{
			updateCMBodyColor[0] = inpBase;
		}
		if (inpSub)
		{
			updateCMBodyColor[1] = inpSub;
		}
		if (inpPaint01)
		{
			updateCMBodyColor[2] = inpPaint01;
		}
		if (inpPaint02)
		{
			updateCMBodyColor[3] = inpPaint02;
		}
		if (inpSunburn)
		{
			updateCMBodyColor[4] = inpSunburn;
		}
		if (inpNail)
		{
			updateCMBodyColor[5] = inpNail;
		}
	}

	public void AddUpdateCMBodyLayoutFlags(bool inpPaint01, bool inpPaint02)
	{
		if (inpPaint01)
		{
			updateCMBodyLayout[2] = inpPaint01;
		}
		if (inpPaint02)
		{
			updateCMBodyLayout[3] = inpPaint02;
		}
	}

	public bool CreateBodyTexture()
	{
		ChaFileBody body = base.chaFile.custom.body;
		bool flag = false;
		if (updateCMBodyTex[0])
		{
			string assetBundleName = "chara/oo_base.unity3d";
			string text = "cf_body_00_t";
			if (!base.hiPoly)
			{
				text += "_low";
			}
			Texture2D mainTexture = CommonLib.LoadAsset<Texture2D>(assetBundleName, text, false, string.Empty);
			Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, string.Empty);
			base.customTexCtrlBody.SetMainTexture(mainTexture);
			flag = true;
			updateCMBodyTex[0] = false;
		}
		if (updateCMBodyColor[0])
		{
			base.customTexCtrlBody.SetColor(ChaShader._Color, body.skinMainColor);
			flag = true;
			updateCMBodyColor[0] = false;
		}
		if (updateCMBodyTex[1])
		{
			string assetBundleName2 = "chara/oo_base.unity3d";
			string text2 = ((base.sex != 0) ? "cf_body_00_mc" : "cm_body_00_mc");
			if (!base.hiPoly)
			{
				text2 += "_low";
			}
			Texture2D tex = CommonLib.LoadAsset<Texture2D>(assetBundleName2, text2, false, string.Empty);
			Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName2, string.Empty);
			base.customTexCtrlBody.SetTexture(ChaShader._ColorMask, tex);
			flag = true;
			updateCMBodyTex[1] = false;
		}
		if (updateCMBodyColor[1])
		{
			base.customTexCtrlBody.SetColor(ChaShader._Color2, body.skinSubColor);
			flag = true;
			updateCMBodyColor[1] = false;
		}
		if (updateCMBodyTex[2])
		{
			if (SetCreateTexture(base.customTexCtrlBody, false, ChaListDefine.CategoryNo.mt_body_paint, body.paintId[0], ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.PaintTex, ChaShader._Texture3))
			{
				flag = true;
			}
			updateCMBodyTex[2] = false;
		}
		if (updateCMBodyColor[2])
		{
			base.customTexCtrlBody.SetColor(ChaShader._Color3, body.paintColor[0]);
			updateCMBodyColor[2] = false;
			flag = true;
		}
		if (updateCMBodyLayout[2])
		{
			ListInfoBase listInfo = base.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.bodypaint_layout, body.paintLayoutId[0]);
			if (listInfo != null)
			{
				float b = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterX) - listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveX);
				float a = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterX) + listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveX);
				float b2 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterY) - listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveY);
				float a2 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterY) + listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveY);
				float a3 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterScale) - listInfo.GetInfoFloat(ChaListDefine.KeyType.AddScale);
				float b3 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterScale) + listInfo.GetInfoFloat(ChaListDefine.KeyType.AddScale);
				Vector4 value = default(Vector4);
				value.x = Mathf.Lerp(a, b, body.paintLayout[0].x);
				value.y = Mathf.Lerp(a2, b2, body.paintLayout[0].y);
				value.z = Mathf.Lerp(-1f, 1f, body.paintLayout[0].z);
				value.w = Mathf.Lerp(a3, b3, body.paintLayout[0].w);
				base.customTexCtrlBody.SetVector4(ChaShader._paint1, value);
				updateCMBodyLayout[2] = false;
				flag = true;
			}
		}
		if (updateCMBodyTex[3])
		{
			if (SetCreateTexture(base.customTexCtrlBody, false, ChaListDefine.CategoryNo.mt_body_paint, body.paintId[1], ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.PaintTex, ChaShader._Texture6))
			{
				flag = true;
			}
			updateCMBodyTex[3] = false;
		}
		if (updateCMBodyColor[3])
		{
			base.customTexCtrlBody.SetColor(ChaShader._Color6, body.paintColor[1]);
			updateCMBodyColor[3] = false;
			flag = true;
		}
		if (updateCMBodyLayout[3])
		{
			ListInfoBase listInfo2 = base.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.bodypaint_layout, body.paintLayoutId[1]);
			if (listInfo2 != null)
			{
				float b4 = listInfo2.GetInfoFloat(ChaListDefine.KeyType.CenterX) - listInfo2.GetInfoFloat(ChaListDefine.KeyType.MoveX);
				float a4 = listInfo2.GetInfoFloat(ChaListDefine.KeyType.CenterX) + listInfo2.GetInfoFloat(ChaListDefine.KeyType.MoveX);
				float b5 = listInfo2.GetInfoFloat(ChaListDefine.KeyType.CenterY) - listInfo2.GetInfoFloat(ChaListDefine.KeyType.MoveY);
				float a5 = listInfo2.GetInfoFloat(ChaListDefine.KeyType.CenterY) + listInfo2.GetInfoFloat(ChaListDefine.KeyType.MoveY);
				float a6 = listInfo2.GetInfoFloat(ChaListDefine.KeyType.CenterScale) - listInfo2.GetInfoFloat(ChaListDefine.KeyType.AddScale);
				float b6 = listInfo2.GetInfoFloat(ChaListDefine.KeyType.CenterScale) + listInfo2.GetInfoFloat(ChaListDefine.KeyType.AddScale);
				Vector4 value2 = default(Vector4);
				value2.x = Mathf.Lerp(a4, b4, body.paintLayout[1].x);
				value2.y = Mathf.Lerp(a5, b5, body.paintLayout[1].y);
				value2.z = Mathf.Lerp(-1f, 1f, body.paintLayout[1].z);
				value2.w = Mathf.Lerp(a6, b6, body.paintLayout[1].w);
				base.customTexCtrlBody.SetVector4(ChaShader._paint2, value2);
				updateCMBodyLayout[3] = false;
				flag = true;
			}
		}
		if (updateCMBodyTex[4])
		{
			if (SetCreateTexture(base.customTexCtrlBody, false, ChaListDefine.CategoryNo.mt_sunburn, body.sunburnId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.SunburnTex, ChaShader._Texture4))
			{
				flag = true;
			}
			updateCMBodyTex[4] = false;
		}
		if (updateCMBodyColor[4])
		{
			base.customTexCtrlBody.SetColor(ChaShader._Color4, body.sunburnColor);
			updateCMBodyColor[4] = false;
			flag = true;
		}
		if (updateCMBodyColor[5])
		{
			base.customTexCtrlBody.SetColor(ChaShader._Color5, body.nailColor);
			updateCMBodyColor[5] = false;
			flag = true;
		}
		if (flag)
		{
			base.customTexCtrlBody.SetNewCreateTexture();
		}
		if (base.releaseCustomInputTexture)
		{
			ReleaseBodyCustomTexture();
		}
		return true;
	}

	public bool ChangeSettingNip()
	{
		ChaFileBody body = base.chaFile.custom.body;
		ChangeTexture(base.customTexCtrlBody.matDraw, ChaListDefine.CategoryNo.mt_nip, body.nipId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.NipTex, ChaShader._overtex1);
		return true;
	}

	public bool ChangeSettingAreolaSize()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetFloat(ChaShader._nipsize, body.areolaSize);
		return true;
	}

	public bool ChangeSettingNipColor()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetColor(ChaShader._overcolor1, body.nipColor);
		return true;
	}

	public bool ChangeSettingNipGlossPower()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetFloat(ChaShader._nip_specular, body.nipGlossPower);
		return true;
	}

	public bool ChangeSettingUnderhair()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		ChangeTexture(base.customTexCtrlBody.matDraw, ChaListDefine.CategoryNo.mt_underhair, body.underhairId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.UnderhairTex, ChaShader._overtex2);
		return true;
	}

	public bool ChangeSettingUnderhairColor()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetColor(ChaShader._overcolor2, body.underhairColor);
		return true;
	}

	public bool VisibleAddBodyLine()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetFloat(ChaShader._linetexon, (!body.drawAddLine) ? 0f : 1f);
		return true;
	}

	public bool ChangeSettingBodyDetail()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		ChangeTexture(base.customTexCtrlBody.matDraw, ChaListDefine.CategoryNo.mt_body_detail, body.detailId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.NormallMapDetail, ChaShader._NormalMapDetail);
		ChangeTexture(base.customTexCtrlBody.matDraw, ChaListDefine.CategoryNo.mt_body_detail, body.detailId, ChaListDefine.KeyType.MainAB, ChaListDefine.KeyType.LineMask, ChaShader._LineMask);
		return true;
	}

	public bool ChangeSettingBodyDetailPower()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetFloat(ChaShader._DetailNormalMapScale, body.detailPower);
		return true;
	}

	public bool ChangeSettingSkinGlossPower()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		float value = Mathf.Lerp(body.skinGlossPower, 1f, base.chaFile.status.skinTuyaRate);
		base.customTexCtrlBody.matDraw.SetFloat(ChaShader._SpecularPower, value);
		return true;
	}

	public bool ChangeSettingNailGlossPower()
	{
		ChaFileBody body = base.chaFile.custom.body;
		if (null == base.customTexCtrlBody.matDraw)
		{
			return false;
		}
		base.customTexCtrlBody.matDraw.SetFloat(ChaShader._SpecularPowerNail, body.nailGlossPower);
		return true;
	}

	public Vector4 GetLayoutInfo(int id)
	{
		Vector4 zero = Vector4.zero;
		ListInfoBase listInfo = base.lstCtrl.GetListInfo(ChaListDefine.CategoryNo.bodypaint_layout, id);
		if (listInfo != null)
		{
			float a = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterX) - listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveX);
			float b = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterX) + listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveX);
			float a2 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterY) - listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveY);
			float b2 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterY) + listInfo.GetInfoFloat(ChaListDefine.KeyType.MoveY);
			float a3 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterScale) - listInfo.GetInfoFloat(ChaListDefine.KeyType.AddScale);
			float b3 = listInfo.GetInfoFloat(ChaListDefine.KeyType.CenterScale) + listInfo.GetInfoFloat(ChaListDefine.KeyType.AddScale);
			float infoFloat = listInfo.GetInfoFloat(ChaListDefine.KeyType.PosX);
			float infoFloat2 = listInfo.GetInfoFloat(ChaListDefine.KeyType.PosY);
			float infoFloat3 = listInfo.GetInfoFloat(ChaListDefine.KeyType.Rot);
			float infoFloat4 = listInfo.GetInfoFloat(ChaListDefine.KeyType.Scale);
			zero.x = Mathf.InverseLerp(a, b, infoFloat);
			zero.y = Mathf.InverseLerp(a2, b2, infoFloat2);
			zero.z = Mathf.InverseLerp(-1f, 1f, infoFloat3);
			zero.w = Mathf.InverseLerp(a3, b3, infoFloat4);
		}
		return zero;
	}

	public bool SetBodyBaseMaterial()
	{
		if (null == base.customMatBody)
		{
			return false;
		}
		return SetBaseMaterial(base.rendBody, base.customMatBody);
	}

	public bool ReleaseBodyCustomTexture()
	{
		if (base.customTexCtrlBody == null)
		{
			return false;
		}
		base.customTexCtrlBody.SetTexture(ChaShader._MainTex, null);
		base.customTexCtrlBody.SetTexture(ChaShader._ColorMask, null);
		base.customTexCtrlBody.SetTexture(ChaShader._Texture3, null);
		base.customTexCtrlBody.SetTexture(ChaShader._Texture4, null);
		base.customTexCtrlBody.SetTexture(ChaShader._Texture6, null);
		Resources.UnloadUnusedAssets();
		return true;
	}

	public void ChangeCustomBodyWithoutCustomTexture()
	{
		ChangeSettingNip();
		ChangeSettingAreolaSize();
		ChangeSettingNipColor();
		ChangeSettingUnderhair();
		ChangeSettingUnderhairColor();
		VisibleAddBodyLine();
		ChangeSettingBodyDetail();
		ChangeSettingBodyDetailPower();
		ChangeSettingSkinGlossPower();
		ChangeSettingNailGlossPower();
		ChangeSettingNipGlossPower();
	}

	public bool InitShapeFace(Transform trfBone, string assetBundleAnmShapeFace, string assetAnmShapeFace)
	{
		if (sibFace == null)
		{
			return false;
		}
		if (null == trfBone)
		{
			return false;
		}
		string cateInfoName = "cf_customhead";
		sibFace.InitShapeInfo(assetBundleAnmShapeFace, "list/customshape.unity3d", assetAnmShapeFace, cateInfoName, trfBone);
		for (int i = 0; i < ShapeFaceNum; i++)
		{
			sibFace.ChangeValue(i, base.chaFile.custom.face.shapeValueFace[i]);
		}
		base.updateShapeFace = true;
		return true;
	}

	public void ReleaseShapeFace()
	{
		if (sibFace != null)
		{
			sibFace.ReleaseShapeInfo();
		}
	}

	public bool SetShapeFaceValue(int index, float value)
	{
		if (index >= ShapeFaceNum)
		{
			return false;
		}
		base.chaFile.custom.face.shapeValueFace[index] = value;
		if (sibFace != null && sibFace.InitEnd)
		{
			sibFace.ChangeValue(index, value);
		}
		if (index == 33)
		{
			ChangeSettingEyeTilt();
		}
		base.updateShapeFace = true;
		return true;
	}

	public bool UpdateShapeFaceValueFromCustomInfo()
	{
		if (sibFace == null || !sibFace.InitEnd)
		{
			return false;
		}
		for (int i = 0; i < base.chaFile.custom.face.shapeValueFace.Length; i++)
		{
			sibFace.ChangeValue(i, base.chaFile.custom.face.shapeValueFace[i]);
		}
		base.updateShapeFace = true;
		return true;
	}

	public float GetShapeFaceValue(int index)
	{
		if (index >= ShapeFaceNum)
		{
			return 0f;
		}
		return base.chaFile.custom.face.shapeValueFace[index];
	}

	public void UpdateShapeFace()
	{
		if (sibFace == null || !sibFace.InitEnd)
		{
			return;
		}
		if (base.chaFile.status.disableMouthShapeMask)
		{
			for (int i = 0; i < ChaFileDefine.cf_MouthShapeMaskID.Length; i++)
			{
				sibFace.ChangeValue(ChaFileDefine.cf_MouthShapeMaskID[i], ChaFileDefine.cf_MouthShapeDefault[i]);
			}
		}
		else
		{
			int[] cf_MouthShapeMaskID = ChaFileDefine.cf_MouthShapeMaskID;
			foreach (int num in cf_MouthShapeMaskID)
			{
				sibFace.ChangeValue(num, base.chaFile.custom.face.shapeValueFace[num]);
			}
		}
		sibFace.Update();
	}

	public void DisableShapeMouth(bool disable)
	{
		base.updateShapeFace = true;
		base.chaFile.status.disableMouthShapeMask = disable;
	}

	public bool InitShapeBody(Transform trfBone)
	{
		if (sibBody == null)
		{
			return false;
		}
		if (null == trfBone)
		{
			return false;
		}
		string anmKeyInfoName = "cf_anmShapeBody";
		string cateInfoName = "cf_custombody";
		sibBody.InitShapeInfo("chara/oo_base.unity3d", "list/customshape.unity3d", anmKeyInfoName, cateInfoName, trfBone);
		float[] array = new float[base.chaFile.custom.body.shapeValueBody.Length];
		base.chaFile.custom.body.shapeValueBody.CopyTo(array, 0);
		if (!base.hiPoly)
		{
			array = array.Select(delegate(float val)
			{
				float num = 0f;
				return (val < 0.5f) ? Mathf.Lerp(0.2f, 0.5f, Mathf.InverseLerp(0f, 0.5f, val)) : Mathf.Lerp(0.5f, 0.8f, Mathf.InverseLerp(0.5f, 1f, val));
			}).ToArray();
			array[0] = base.chaFile.custom.body.shapeValueBody[0];
		}
		array[13] = Mathf.Lerp(array[13], 1f, base.chaFile.status.nipStandRate);
		for (int i = 0; i < ShapeBodyNum; i++)
		{
			sibBody.ChangeValue(i, array[i]);
		}
		base.updateShapeBody = true;
		base.updateBustSize = true;
		base.reSetupDynamicBoneBust = true;
		return true;
	}

	public void ReleaseShapeBody()
	{
		if (sibBody != null)
		{
			sibBody.ReleaseShapeInfo();
		}
	}

	public bool SetShapeBodyValue(int index, float value)
	{
		if (index >= ShapeBodyNum)
		{
			return false;
		}
		if (base.sex == 0 && index == 0)
		{
			value = 0.6f;
		}
		float num = (base.chaFile.custom.body.shapeValueBody[index] = value);
		if (!base.hiPoly && base.sex != 0)
		{
			float num2 = 0f;
			num2 = ((!(num < 0.5f)) ? Mathf.Lerp(0.5f, 0.8f, Mathf.InverseLerp(0.5f, 1f, num)) : Mathf.Lerp(0.2f, 0.5f, Mathf.InverseLerp(0f, 0.5f, num)));
			num = num2;
			if (index == 0)
			{
				num = value;
			}
		}
		if (index == 13)
		{
			num = Mathf.Lerp(value, 1f, base.chaFile.status.nipStandRate);
		}
		if (sibBody != null && sibBody.InitEnd)
		{
			sibBody.ChangeValue(index, num);
		}
		base.updateShapeBody = true;
		base.updateBustSize = true;
		base.reSetupDynamicBoneBust = true;
		return true;
	}

	public bool UpdateShapeBodyValueFromCustomInfo()
	{
		if (sibBody == null || !sibBody.InitEnd)
		{
			return false;
		}
		float[] array = new float[base.chaFile.custom.body.shapeValueBody.Length];
		base.chaFile.custom.body.shapeValueBody.CopyTo(array, 0);
		if (!base.hiPoly)
		{
			array = array.Select(delegate(float val)
			{
				float num = 0f;
				return (val < 0.5f) ? Mathf.Lerp(0.2f, 0.5f, Mathf.InverseLerp(0f, 0.5f, val)) : Mathf.Lerp(0.5f, 0.8f, Mathf.InverseLerp(0.5f, 1f, val));
			}).ToArray();
			array[0] = base.chaFile.custom.body.shapeValueBody[0];
		}
		array[13] = Mathf.Lerp(array[13], 1f, base.chaFile.status.nipStandRate);
		for (int i = 0; i < base.chaFile.custom.body.shapeValueBody.Length; i++)
		{
			sibBody.ChangeValue(i, array[i]);
		}
		if (base.sex == 0)
		{
			sibBody.ChangeValue(0, 0.6f);
		}
		base.updateShapeBody = true;
		base.updateBustSize = true;
		base.reSetupDynamicBoneBust = true;
		return true;
	}

	public float GetShapeBodyValue(int index)
	{
		if (index >= ShapeBodyNum)
		{
			return 0f;
		}
		return base.chaFile.custom.body.shapeValueBody[index];
	}

	public void UpdateShapeBody()
	{
		if (sibBody == null || !sibBody.InitEnd)
		{
			return;
		}
		ShapeBodyInfoFemale shapeBodyInfoFemale = sibBody as ShapeBodyInfoFemale;
		if (shapeBodyInfoFemale == null)
		{
			return;
		}
		shapeBodyInfoFemale.updateMask = 7;
		sibBody.Update();
		if (!changeShapeBodyMask)
		{
			return;
		}
		float[] array = new float[ChaFileDefine.cf_BustShapeMaskID.Length];
		int num = 0;
		int[] array2 = new int[2] { 1, 2 };
		float[] array3 = new float[9] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0f, 0.5f, 0.5f };
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < ChaFileDefine.cf_BustShapeMaskID.Length; j++)
			{
				num = ChaFileDefine.cf_BustShapeMaskID[j];
				array[j] = ((!base.chaFile.status.disableBustShapeMask[i, j]) ? base.chaFile.custom.body.shapeValueBody[num] : array3[j]);
			}
			int cf_ShapeMaskNipStand = ChaFileDefine.cf_ShapeMaskNipStand;
			num = ChaFileDefine.cf_BustShapeMaskID[cf_ShapeMaskNipStand];
			array[cf_ShapeMaskNipStand] = ((!base.chaFile.status.disableBustShapeMask[i, cf_ShapeMaskNipStand]) ? Mathf.Lerp(base.chaFile.custom.body.shapeValueBody[num], 1f, base.chaFile.status.nipStandRate) : 0.5f);
			for (int k = 0; k < ChaFileDefine.cf_BustShapeMaskID.Length; k++)
			{
				sibBody.ChangeValue(ChaFileDefine.cf_BustShapeMaskID[k], array[k]);
			}
			shapeBodyInfoFemale.updateMask = array2[i];
			sibBody.Update();
		}
		changeShapeBodyMask = false;
	}

	public void UpdateAlwaysShapeBody()
	{
		if (sibBody != null && sibBody.InitEnd)
		{
			sibBody.UpdateAlways();
		}
	}

	public void DisableShapeBodyID(int LR, int id, bool disable)
	{
		if (sibBody != null && sibBody.InitEnd && id < ChaFileDefine.cf_BustShapeMaskID.Length)
		{
			changeShapeBodyMask = true;
			base.updateShapeBody = true;
			switch (LR)
			{
			case 0:
				base.chaFile.status.disableBustShapeMask[0, id] = disable;
				break;
			case 1:
				base.chaFile.status.disableBustShapeMask[1, id] = disable;
				break;
			default:
				base.chaFile.status.disableBustShapeMask[0, id] = disable;
				base.chaFile.status.disableBustShapeMask[1, id] = disable;
				break;
			}
			base.reSetupDynamicBoneBust = true;
		}
	}

	public void DisableShapeBust(int LR, bool disable)
	{
		if (sibBody == null || !sibBody.InitEnd)
		{
			return;
		}
		changeShapeBodyMask = true;
		base.updateShapeBody = true;
		switch (LR)
		{
		case 0:
		{
			for (int k = 0; k < ChaFileDefine.cf_ShapeMaskBust.Length; k++)
			{
				base.chaFile.status.disableBustShapeMask[0, ChaFileDefine.cf_ShapeMaskBust[k]] = disable;
			}
			break;
		}
		case 1:
		{
			for (int l = 0; l < ChaFileDefine.cf_ShapeMaskBust.Length; l++)
			{
				base.chaFile.status.disableBustShapeMask[1, ChaFileDefine.cf_ShapeMaskBust[l]] = disable;
			}
			break;
		}
		default:
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < ChaFileDefine.cf_ShapeMaskBust.Length; j++)
				{
					base.chaFile.status.disableBustShapeMask[i, ChaFileDefine.cf_ShapeMaskBust[j]] = disable;
				}
			}
			break;
		}
		}
		base.reSetupDynamicBoneBust = true;
	}

	public void DisableShapeNip(int LR, bool disable)
	{
		if (sibBody == null || !sibBody.InitEnd)
		{
			return;
		}
		changeShapeBodyMask = true;
		base.updateShapeBody = true;
		switch (LR)
		{
		case 0:
		{
			for (int k = 0; k < ChaFileDefine.cf_ShapeMaskNip.Length; k++)
			{
				base.chaFile.status.disableBustShapeMask[0, ChaFileDefine.cf_ShapeMaskNip[k]] = disable;
			}
			break;
		}
		case 1:
		{
			for (int l = 0; l < ChaFileDefine.cf_ShapeMaskNip.Length; l++)
			{
				base.chaFile.status.disableBustShapeMask[1, ChaFileDefine.cf_ShapeMaskNip[l]] = disable;
			}
			break;
		}
		default:
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < ChaFileDefine.cf_ShapeMaskNip.Length; j++)
				{
					base.chaFile.status.disableBustShapeMask[i, ChaFileDefine.cf_ShapeMaskNip[j]] = disable;
				}
			}
			break;
		}
		}
		base.reSetupDynamicBoneBust = true;
	}

	public void UpdateBustSoftnessAndGravity()
	{
		UpdateBustSoftness();
		UpdateBustGravity();
	}

	public void ChangeBustSoftness(float soft)
	{
		if (bustSoft != null)
		{
			bustSoft.Change(soft, default(int));
			base.reSetupDynamicBoneBust = true;
		}
	}

	public bool UpdateBustSoftness()
	{
		if (bustSoft != null)
		{
			bustSoft.ReCalc(default(int));
			base.reSetupDynamicBoneBust = true;
			return true;
		}
		return false;
	}

	public void ChangeBustGravity(float gravity)
	{
		if (bustGravity != null)
		{
			bustGravity.Change(gravity, default(int));
			base.reSetupDynamicBoneBust = true;
		}
	}

	public bool UpdateBustGravity()
	{
		if (bustGravity != null)
		{
			bustGravity.ReCalc(default(int));
			base.reSetupDynamicBoneBust = true;
			return true;
		}
		return false;
	}

	protected void InitializeControlFaceAll()
	{
		fbsaaVoice = new AudioAssist();
		InitializeControlFaceObject();
	}

	protected void InitializeControlFaceObject()
	{
		asVoice = null;
		texHairGloss = null;
	}

	protected void ReleaseControlFaceAll()
	{
		ReleaseControlFaceObject(false);
	}

	protected void ReleaseControlFaceObject(bool init = true)
	{
		if (init)
		{
			InitializeControlFaceObject();
		}
	}

	public void HideEyeHighlight(bool hide)
	{
		base.fileStatus.hideEyesHighlight = hide;
		float value = ((!hide) ? 1f : 0f);
		if (base.rendEye == null)
		{
			return;
		}
		Renderer[] array = base.rendEye;
		foreach (Renderer renderer in array)
		{
			if (!(null == renderer))
			{
				Material material = renderer.material;
				if (null != material)
				{
					material.SetFloat(ChaShader._isHighLight, value);
				}
			}
		}
	}

	public void ChangeEyesShaking(bool enable)
	{
		base.fileStatus.eyesYure = enable;
		if (base.eyeLookMatCtrl != null)
		{
			EyeLookMaterialControll[] array = base.eyeLookMatCtrl;
			foreach (EyeLookMaterialControll eyeLookMaterialControll in array)
			{
				eyeLookMaterialControll.ChangeShaking(enable);
			}
		}
	}

	public bool GetEyesShaking()
	{
		return base.fileStatus.eyesYure;
	}

	public int GetEyesPtnNum()
	{
		Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_eyeset);
		return (categoryInfo != null) ? categoryInfo.Keys.Count : 0;
	}

	public void ChangeEyesPtn(int ptn, bool blend = true)
	{
		if (base.eyesCtrl == null)
		{
			return;
		}
		base.fileStatus.eyesPtn = ptn;
		Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_eyeset);
		ListInfoBase value;
		if (!categoryInfo.TryGetValue(ptn, out value))
		{
			base.fileStatus.eyesPtn = (ptn = 0);
			categoryInfo.TryGetValue(ptn, out value);
		}
		if (value == null)
		{
			return;
		}
		GameObject referenceInfo = GetReferenceInfo(RefObjKey.N_EyeBase);
		if ((bool)referenceInfo)
		{
			referenceInfo.SetActiveIfDifferent(1 == value.GetInfoInt(ChaListDefine.KeyType.EyeBase));
		}
		GameObject referenceInfo2 = GetReferenceInfo(RefObjKey.N_Hitomi);
		if ((bool)referenceInfo2)
		{
			referenceInfo2.SetActiveIfDifferent(0 != value.GetInfoInt(ChaListDefine.KeyType.EyeHitomi));
		}
		GameObject[] array = new GameObject[3]
		{
			GetReferenceInfo(RefObjKey.N_Gag00),
			GetReferenceInfo(RefObjKey.N_Gag01),
			GetReferenceInfo(RefObjKey.N_Gag02)
		};
		int infoInt = value.GetInfoInt(ChaListDefine.KeyType.EyeObjNo);
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i])
			{
				array[i].SetActiveIfDifferent(i + 1 == infoInt);
			}
		}
		if (base.rendEye != null && base.rendEye.Length == 2)
		{
			Texture value2 = null;
			string info = value.GetInfo(ChaListDefine.KeyType.EpsTexAB);
			string text = value.GetInfo(ChaListDefine.KeyType.EpsTex);
			float value3 = 0f;
			if ("0" != info && "0" != text)
			{
				if (!base.hiPoly)
				{
					text += "_low";
				}
				value2 = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
				value3 = 1f;
			}
			Renderer[] array2 = base.rendEye;
			foreach (Renderer renderer in array2)
			{
				if (!(null == renderer))
				{
					renderer.material.SetTexture(ChaShader._expression, value2);
					renderer.material.SetFloat(ChaShader._exppower, value3);
				}
			}
		}
		if (array != null && array.Length != 0 && infoInt != 0)
		{
			Texture tex = null;
			string info2 = value.GetInfo(ChaListDefine.KeyType.MainTexAB);
			string text2 = value.GetInfo(ChaListDefine.KeyType.MainTex);
			if ("0" != info2 && "0" != text2)
			{
				if (!base.hiPoly)
				{
					text2 += "_low";
				}
				tex = CommonLib.LoadAsset<Texture2D>(info2, text2, false, string.Empty);
			}
			string[] array3 = value.GetInfo(ChaListDefine.KeyType.TileAnim).Split('/');
			Vector4 zero = Vector4.zero;
			if (3 <= array3.Length)
			{
				float.TryParse(array3[0], out zero.x);
				float.TryParse(array3[1], out zero.y);
				float.TryParse(array3[2], out zero.z);
			}
			float infoFloat = value.GetInfoFloat(ChaListDefine.KeyType.SizeSpeed);
			float infoFloat2 = value.GetInfoFloat(ChaListDefine.KeyType.SizeWidth);
			float infoFloat3 = value.GetInfoFloat(ChaListDefine.KeyType.AngleSpeed);
			float infoFloat4 = value.GetInfoFloat(ChaListDefine.KeyType.Yurayura);
			ChangeGagEyesMaterial(infoInt - 1, tex, zero, infoFloat, infoFloat2, infoFloat3, infoFloat4);
		}
		SetForegroundEyesAndEyebrow();
		ChangeSettingEyeShadowColor();
		base.eyesCtrl.ChangePtn(value.GetInfoInt(ChaListDefine.KeyType.EyesPtn), blend);
	}

	public int GetEyesPtn()
	{
		return base.fileStatus.eyesPtn;
	}

	public void ChangeGagEyesMaterial(int no, Texture tex, Vector4 v4TileAnim, float sizeSpeed, float sizeWidth, float angleSpeed, float yurayura)
	{
		GameObject[] array = new GameObject[3]
		{
			GetReferenceInfo(RefObjKey.N_Gag00),
			GetReferenceInfo(RefObjKey.N_Gag01),
			GetReferenceInfo(RefObjKey.N_Gag02)
		};
		if (!(null == array[no]))
		{
			SkinnedMeshRenderer component = array[no].GetComponent<SkinnedMeshRenderer>();
			if (!(null == component))
			{
				base.matGag[no].SetTexture(ChaShader._MainTex, null);
				base.matGag[no].SetTexture(ChaShader._MainTex, tex);
				base.matGag[no].SetVector(ChaShader._TileAnimation, v4TileAnim);
				base.matGag[no].SetFloat(ChaShader._SizeSpeed, sizeSpeed);
				base.matGag[no].SetFloat(ChaShader._SizeWidth, sizeWidth);
				base.matGag[no].SetFloat(ChaShader._angleSpeed, angleSpeed);
				base.matGag[no].SetFloat(ChaShader._yurayura, yurayura);
				component.material = base.matGag[no];
				Resources.UnloadUnusedAssets();
			}
		}
	}

	public void ChangeEyesOpenMax(float maxValue)
	{
		if (base.eyesCtrl != null)
		{
			float num = Mathf.Clamp(maxValue, 0f, 1f);
			base.fileStatus.eyesOpenMax = num;
			base.eyesCtrl.OpenMax = Mathf.Clamp(maxValue, 0f, 0.92f);
			if (!base.fileStatus.eyesBlink)
			{
				base.eyesCtrl.SetOpenRateForce(num);
			}
		}
	}

	public float GetEyesOpenMax()
	{
		return base.fileStatus.eyesOpenMax;
	}

	public void ChangeEyebrowPtn(int ptn, bool blend = true)
	{
		if (base.eyebrowCtrl != null)
		{
			base.fileStatus.eyebrowPtn = ptn;
			base.eyebrowCtrl.ChangePtn(ptn, blend);
		}
	}

	public int GetEyebrowPtn()
	{
		return base.fileStatus.eyebrowPtn;
	}

	public void ChangeEyebrowOpenMax(float maxValue)
	{
		if (base.eyebrowCtrl != null)
		{
			float num = Mathf.Clamp(maxValue, 0f, 1f);
			base.fileStatus.eyebrowOpenMax = num;
			base.eyebrowCtrl.OpenMax = num;
			if (!base.fileStatus.eyesBlink)
			{
				base.eyebrowCtrl.SetOpenRateForce(1f);
			}
		}
	}

	public float GetEyebrowOpenMax()
	{
		return base.fileStatus.eyebrowOpenMax;
	}

	public void ChangeEyesBlinkFlag(bool blink)
	{
		if (!(null == base.fbsCtrl) && base.fbsCtrl.BlinkCtrl != null)
		{
			base.fileStatus.eyesBlink = blink;
			base.fbsCtrl.BlinkCtrl.SetFixedFlags((!blink) ? ((byte)1) : ((byte)0));
			if (!blink)
			{
				base.eyesCtrl.SetOpenRateForce(1f);
				base.eyebrowCtrl.SetOpenRateForce(1f);
			}
		}
	}

	public bool GetEyesBlinkFlag()
	{
		return base.fileStatus.eyesBlink;
	}

	public void ChangeMouthPtn(int ptn, bool blend = true)
	{
		if (base.mouthCtrl != null)
		{
			base.fileStatus.mouthPtn = ptn;
			base.mouthCtrl.ChangePtn(ptn, blend);
			ChangeTongueState((ptn == 21) ? ((byte)1) : ((byte)0));
			base.mouthCtrl.UseAdjustWidthScale(ptn != 21 && ptn != 22);
		}
	}

	public int GetMouthPtn()
	{
		return base.fileStatus.mouthPtn;
	}

	public void ChangeMouthOpenMax(float maxValue)
	{
		if (base.mouthCtrl != null)
		{
			float num = Mathf.Clamp(maxValue, 0f, 1f);
			base.fileStatus.mouthOpenMax = num;
			base.mouthCtrl.OpenMax = num;
			if (base.fileStatus.mouthFixed)
			{
				base.mouthCtrl.FixedRate = num;
			}
		}
	}

	public float GetMouthOpenMax()
	{
		return base.fileStatus.mouthOpenMax;
	}

	public void ChangeMouthFixed(bool fix)
	{
		if (base.mouthCtrl != null)
		{
			base.fileStatus.mouthFixed = fix;
			if (fix)
			{
				base.mouthCtrl.FixedRate = base.fileStatus.mouthOpenMax;
			}
			else
			{
				base.mouthCtrl.FixedRate = -1f;
			}
		}
	}

	public bool GetMouthFixed()
	{
		return base.fileStatus.mouthFixed;
	}

	public void ChangeTongueState(byte state)
	{
		base.fileStatus.tongueState = state;
	}

	public byte GetTongueState()
	{
		return base.fileStatus.tongueState;
	}

	public bool SetVoiceTransform(Transform trfVoice)
	{
		if (null == trfVoice)
		{
			asVoice = null;
			return false;
		}
		asVoice = trfVoice.GetComponent<AudioSource>();
		if (null == asVoice)
		{
			return false;
		}
		return true;
	}

	private void UpdateBlendShapeVoice()
	{
		float voiceVaule = 0f;
		float correct = 3f;
		if ((bool)asVoice && asVoice.isPlaying)
		{
			voiceVaule = ((base.wavInfoData == null) ? fbsaaVoice.GetAudioWaveValue(asVoice, correct) : base.wavInfoData.GetValue(asVoice.time));
		}
		if ((bool)base.fbsCtrl)
		{
			base.fbsCtrl.SetVoiceVaule(voiceVaule);
		}
		if (!base.fileStatus.mouthAdjustWidth || !base.objHeadBone)
		{
			return;
		}
		GameObject referenceInfo = GetReferenceInfo(RefObjKey.F_ADJUSTWIDTHSCALE);
		if (null != referenceInfo)
		{
			float x = 1f;
			if (base.mouthCtrl != null)
			{
				x = base.mouthCtrl.GetAdjustWidthScale();
			}
			referenceInfo.transform.SetLocalScaleX(x);
		}
	}

	public void ChangeLookEyesTarget(int targetType, Transform trfTarg = null, float rate = 0.5f, float rotDeg = 0f, float range = 1f, float dis = 2f)
	{
		if (null == base.eyeLookCtrl)
		{
			return;
		}
		if (targetType == -1)
		{
			targetType = base.fileStatus.eyesTargetType;
		}
		else
		{
			base.fileStatus.eyesTargetType = targetType;
		}
		base.eyeLookCtrl.target = null;
		if ((bool)trfTarg)
		{
			base.eyeLookCtrl.target = trfTarg;
			return;
		}
		if (targetType == 0)
		{
			if ((bool)Camera.main)
			{
				base.eyeLookCtrl.target = Camera.main.transform;
			}
		}
		else if ((bool)base.objEyesLookTarget && (bool)base.objEyesLookTargetP)
		{
			switch (targetType)
			{
			case 1:
				rotDeg = 0f;
				range = 1f;
				break;
			case 2:
				rotDeg = 45f;
				range = 1f;
				break;
			case 3:
				rotDeg = 90f;
				range = 1f;
				break;
			case 4:
				rotDeg = 135f;
				range = 1f;
				break;
			case 5:
				rotDeg = 180f;
				range = 1f;
				break;
			case 6:
				rotDeg = 225f;
				range = 1f;
				break;
			case 7:
				rotDeg = 270f;
				range = 1f;
				break;
			case 8:
				rotDeg = 315f;
				range = 1f;
				break;
			}
			base.objEyesLookTargetP.transform.SetLocalPosition(0f, 0.07f, 0f);
			base.eyeLookCtrl.target = base.objEyesLookTarget.transform;
			float y = Mathf.Lerp(0f, range, rate);
			base.eyeLookCtrl.target.SetLocalPosition(0f, y, dis);
			base.objEyesLookTargetP.transform.localEulerAngles = new Vector3(0f, 0f, 360f - rotDeg);
		}
		base.fileStatus.eyesTargetAngle = rotDeg;
		base.fileStatus.eyesTargetRange = range;
		base.fileStatus.eyesTargetRate = rate;
	}

	public void ChangeLookEyesPtn(int ptn)
	{
		if (!(null == base.eyeLookCtrl))
		{
			EyeLookController eyeLookController = base.eyeLookCtrl;
			base.fileStatus.eyesLookPtn = ptn;
			eyeLookController.ptnNo = ptn;
		}
	}

	public int GetLookEyesPtn()
	{
		return base.fileStatus.eyesLookPtn;
	}

	public void ChangeLookNeckTarget(int targetType, Transform trfTarg = null, float rate = 0.5f, float rotDeg = 0f, float range = 1f, float dis = 0.8f)
	{
		if (null == base.neckLookCtrl)
		{
			return;
		}
		if (targetType == -1)
		{
			targetType = base.fileStatus.neckTargetType;
		}
		else
		{
			base.fileStatus.neckTargetType = targetType;
		}
		base.neckLookCtrl.target = null;
		if ((bool)trfTarg)
		{
			base.neckLookCtrl.target = trfTarg;
			return;
		}
		if (targetType == 0)
		{
			if ((bool)Camera.main)
			{
				base.neckLookCtrl.target = Camera.main.transform;
			}
		}
		else if ((bool)base.objNeckLookTarget && (bool)base.objNeckLookTargetP)
		{
			switch (targetType)
			{
			case 1:
				rotDeg = 0f;
				range = 1f;
				break;
			case 2:
				rotDeg = 45f;
				range = 1f;
				break;
			case 3:
				rotDeg = 90f;
				range = 1f;
				break;
			case 4:
				rotDeg = 135f;
				range = 1f;
				break;
			case 5:
				rotDeg = 180f;
				range = 1f;
				break;
			case 6:
				rotDeg = 225f;
				range = 1f;
				break;
			case 7:
				rotDeg = 270f;
				range = 1f;
				break;
			case 8:
				rotDeg = 315f;
				range = 1f;
				break;
			}
			base.objNeckLookTargetP.transform.SetLocalPosition(0f, 0.27f, 0f);
			base.neckLookCtrl.target = base.objNeckLookTarget.transform;
			float y = Mathf.Lerp(0f, range, rate);
			base.neckLookCtrl.target.SetLocalPosition(0f, y, dis);
			base.objNeckLookTargetP.transform.localEulerAngles = new Vector3(0f, 0f, 360f - rotDeg);
		}
		base.fileStatus.neckTargetAngle = rotDeg;
		base.fileStatus.neckTargetRange = range;
		base.fileStatus.neckTargetRate = rate;
	}

	public void ChangeLookNeckPtn(int ptn, float rate = 1f)
	{
		if (!(null == base.neckLookCtrl))
		{
			NeckLookControllerVer2 neckLookControllerVer = base.neckLookCtrl;
			base.fileStatus.neckLookPtn = ptn;
			neckLookControllerVer.ptnNo = ptn;
			base.neckLookCtrl.rate = rate;
		}
	}

	public int GetLookNeckPtn()
	{
		return base.fileStatus.neckLookPtn;
	}

	public void SetForegroundEyesAndEyebrow()
	{
		bool eyes = true;
		if (Manager.Config.EtcData != null)
		{
			eyes = Manager.Config.EtcData.ForegroundEyes;
		}
		if (base.fileFace.foregroundEyes == 1)
		{
			eyes = false;
		}
		else if (base.fileFace.foregroundEyes == 2)
		{
			eyes = true;
		}
		bool eyebrow = true;
		if (Manager.Config.EtcData != null)
		{
			eyebrow = Manager.Config.EtcData.ForegroundEyebrow;
		}
		if (base.fileFace.foregroundEyebrow == 1)
		{
			eyebrow = false;
		}
		else if (base.fileFace.foregroundEyebrow == 2)
		{
			eyebrow = true;
		}
		if (null != base.foregroundCtrl)
		{
			base.foregroundCtrl.SetForeground(eyes, eyebrow);
		}
	}

	public void ChangeHohoAkaRate(float value)
	{
		base.fileStatus.hohoAkaRate = value;
		if ((bool)base.customMatFace)
		{
			Color color = base.customMatFace.GetColor(ChaShader._overcolor2);
			color.a = Mathf.Lerp(0f, 0.2f, base.fileStatus.hohoAkaRate);
			if (Manager.Config.EtcData != null && !Manager.Config.EtcData.hohoAka)
			{
				color.a = 0f;
			}
			base.customMatFace.SetColor(ChaShader._overcolor2, color);
		}
	}

	public bool IsGagEyes()
	{
		Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.cha_eyeset);
		int eyesPtn = GetEyesPtn();
		ListInfoBase value;
		if (!categoryInfo.TryGetValue(eyesPtn, out value))
		{
			return false;
		}
		if (value.GetInfoInt(ChaListDefine.KeyType.EyeObjNo) == 0)
		{
			return false;
		}
		return true;
	}

	protected void InitializeControlLoadAll()
	{
		InitializeControlLoadObject();
	}

	protected void InitializeControlLoadObject()
	{
		aaWeightsHead = new AssignedAnotherWeights();
		aaWeightsBody = new AssignedAnotherWeights();
		updateAlphaMask = true;
	}

	protected void ReleaseControlLoadAll()
	{
		ReleaseControlLoadObject(false);
	}

	protected void ReleaseControlLoadObject(bool init = true)
	{
		if (aaWeightsHead != null)
		{
			aaWeightsHead.Release();
		}
		if (aaWeightsBody != null)
		{
			aaWeightsBody.Release();
		}
		if (init)
		{
			InitializeControlLoadObject();
		}
	}

	public bool Load(bool reflectStatus = false)
	{
		StartCoroutine(LoadAsync(false, false));
		return true;
	}

	public IEnumerator LoadAsync(bool reflectStatus = false, bool asyncFlags = true)
	{
		bool sexFemale = ((base.sex != 0) ? true : false);
		string mainManifestName = Singleton<Character>.Instance.mainManifestName;
		byte[] status = null;
		if (reflectStatus)
		{
			status = base.chaFile.GetStatusBytes();
		}
		ReleaseObject();
		if (asyncFlags)
		{
			yield return null;
		}
		base.objTop = new GameObject("BodyTop");
		if ((bool)base.objRoot)
		{
			base.objTop.transform.SetParent(base.objRoot.transform, false);
		}
		if (asyncFlags)
		{
			SetActiveTop(false);
		}
		AddUpdateCMBodyTexFlags(true, true, true, true, true);
		AddUpdateCMBodyColorFlags(true, true, true, true, true, true);
		AddUpdateCMBodyLayoutFlags(true, true);
		CreateBodyTexture();
		LoadGagMaterial();
		string assetBundleName = "chara/oo_base.unity3d";
		string assetName = ((!base.hiPoly) ? "p_cf_body_bone_low" : "p_cf_body_bone");
		if (asyncFlags)
		{
			yield return StartCoroutine(Load_Coroutine(assetBundleName, assetName, delegate(GameObject x)
			{
				base.objAnim = x;
			}, true, mainManifestName));
		}
		else
		{
			base.objAnim = CommonLib.LoadAsset<GameObject>(assetBundleName, assetName, true, mainManifestName);
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, mainManifestName);
		if (!base.objAnim)
		{
			yield break;
		}
		base.animBody = base.objAnim.GetComponent<Animator>();
		base.objAnim.transform.SetParent(base.objTop.transform, false);
		base.dictDynamicBoneBust = new Dictionary<DynamicBoneKind, DynamicBone_Ver02>();
		DynamicBone_Ver02[] componentsInChildren = base.objAnim.GetComponentsInChildren<DynamicBone_Ver02>(true);
		DynamicBone_Ver02[] array = componentsInChildren;
		foreach (DynamicBone_Ver02 dynamicBone_Ver in array)
		{
			if (dynamicBone_Ver.Comment == "左胸")
			{
				base.dictDynamicBoneBust[DynamicBoneKind.BreastL] = dynamicBone_Ver;
			}
			else if (dynamicBone_Ver.Comment == "右胸")
			{
				base.dictDynamicBoneBust[DynamicBoneKind.BreastR] = dynamicBone_Ver;
			}
			else if (dynamicBone_Ver.Comment == "左尻")
			{
				base.dictDynamicBoneBust[DynamicBoneKind.HipL] = dynamicBone_Ver;
				if (base.sex == 0)
				{
					dynamicBone_Ver.enabled = false;
				}
			}
			else if (dynamicBone_Ver.Comment == "右尻")
			{
				base.dictDynamicBoneBust[DynamicBoneKind.HipR] = dynamicBone_Ver;
				if (base.sex == 0)
				{
					dynamicBone_Ver.enabled = false;
				}
			}
		}
		base.objBodyBone = base.objAnim.transform.FindLoop("cf_j_root");
		if ((bool)base.objBodyBone)
		{
			aaWeightsBody.CreateBoneList(base.objBodyBone, string.Empty);
			CreateReferenceInfo(1uL, base.objBodyBone);
			if (!base.hiPoly)
			{
				GameObject referenceInfo = GetReferenceInfo(RefObjKey.a_n_back_L);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalRotationY(180f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_back_R);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalRotationY(180f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_bust);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPositionY(0.14f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_bust_f);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPositionZ(0.04f);
					referenceInfo.transform.SetLocalRotationX(-35f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_ind_L);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(-0.0162f, -0.0024f, 0.0006f);
					referenceInfo.transform.SetLocalRotation(0f, 0f, 0f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_ind_R);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(0.0162f, 0.0024f, -0.0006f);
					referenceInfo.transform.SetLocalRotation(180f, 0f, 0f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_mid_L);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(-0.016f, -0.0028f, 0f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_mid_R);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(0.016f, 0.0028f, 0f);
					referenceInfo.transform.SetLocalRotation(180f, 0f, 0f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_ring_L);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(-0.018f, -0.0018f, -0.0005f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_ring_R);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(0.018f, 0.0018f, 0.0005f);
					referenceInfo.transform.SetLocalRotation(180f, 0f, 0f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_nip_L);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(0f, 0f, 0.05f);
					referenceInfo.transform.SetLocalRotation(0f, 0f, 0f);
				}
				referenceInfo = GetReferenceInfo(RefObjKey.a_n_nip_R);
				if ((bool)referenceInfo)
				{
					referenceInfo.transform.SetLocalPosition(0f, 0f, 0.05f);
					referenceInfo.transform.SetLocalRotation(0f, 0f, 0f);
				}
			}
			NeckLookControllerVer2[] componentsInChildren2 = base.objBodyBone.GetComponentsInChildren<NeckLookControllerVer2>(true);
			if (componentsInChildren2.Length != 0)
			{
				base.neckLookCtrl = componentsInChildren2[0];
			}
			if ((bool)base.neckLookCtrl)
			{
				ChangeLookNeckTarget(base.fileStatus.neckTargetType);
				ChangeLookNeckPtn(0);
			}
			InitShapeBody(base.objBodyBone.transform);
			base.objNeckLookTargetP = new GameObject("N_NeckLookTargetP");
			GameObject referenceInfo2 = GetReferenceInfo(RefObjKey.NECK_LOOK_TARGET);
			if ((bool)base.objNeckLookTargetP && (bool)referenceInfo2)
			{
				base.objNeckLookTargetP.transform.SetParent(referenceInfo2.transform, false);
				base.objNeckLookTarget = new GameObject("N_NeckLookTarget");
				if ((bool)base.objNeckLookTarget)
				{
					base.objNeckLookTarget.transform.SetParent(base.objNeckLookTargetP.transform, false);
				}
			}
			base.objEyesLookTargetP = new GameObject("N_EyesLookTargetP");
			referenceInfo2 = GetReferenceInfo(RefObjKey.HeadParent);
			if ((bool)base.objEyesLookTargetP && (bool)referenceInfo2)
			{
				base.objEyesLookTargetP.transform.SetParent(referenceInfo2.transform, false);
				base.objEyesLookTarget = new GameObject("N_EyesLookTarget");
				if ((bool)base.objEyesLookTarget)
				{
					base.objEyesLookTarget.transform.SetParent(base.objEyesLookTargetP.transform, false);
				}
			}
		}
		string assetBundleName2 = "chara/oo_base.unity3d";
		string assetName2 = "p_cf_head_bone";
		if (asyncFlags)
		{
			yield return StartCoroutine(Load_Coroutine(assetBundleName2, assetName2, delegate(GameObject x)
			{
				base.objHeadBone = x;
			}, true, mainManifestName));
		}
		else
		{
			base.objHeadBone = CommonLib.LoadAsset<GameObject>(assetBundleName2, assetName2, true, mainManifestName);
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName2, mainManifestName);
		if (!base.objHeadBone)
		{
			yield break;
		}
		GameObject referenceInfo3 = GetReferenceInfo(RefObjKey.HeadParent);
		base.objHeadBone.transform.SetParent(referenceInfo3.transform, false);
		EyeLookController[] componentsInChildren3 = base.objHeadBone.GetComponentsInChildren<EyeLookController>(true);
		if (componentsInChildren3.Length != 0)
		{
			base.eyeLookCtrl = componentsInChildren3[0];
		}
		if ((bool)base.eyeLookCtrl)
		{
			EyeLookCalc component = base.eyeLookCtrl.GetComponent<EyeLookCalc>();
			if ((bool)component)
			{
				ChangeLookEyesTarget(base.fileStatus.eyesTargetType);
				ChangeLookEyesPtn(0);
			}
		}
		aaWeightsHead.CreateBoneList(base.objHeadBone, string.Empty);
		CreateReferenceInfo(2uL, base.objHeadBone);
		string assetBundleName3 = "chara/oo_base.unity3d";
		string assetName3 = (base.hiPoly ? ((!sexFemale) ? "p_cm_body_00" : "p_cf_body_00") : ((!sexFemale) ? "p_cm_body_00_low" : "p_cf_body_00_low"));
		if (asyncFlags)
		{
			yield return StartCoroutine(Load_Coroutine(assetBundleName3, assetName3, delegate(GameObject x)
			{
				base.objBody = x;
			}, true, mainManifestName));
		}
		else
		{
			base.objBody = CommonLib.LoadAsset<GameObject>(assetBundleName3, assetName3, true, mainManifestName);
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName3, mainManifestName);
		if ((bool)base.objBody)
		{
			CreateReferenceInfo(3uL, base.objBody);
			GameObject referenceInfo4 = GetReferenceInfo(RefObjKey.ObjBody);
			if ((bool)referenceInfo4)
			{
				base.rendBody = referenceInfo4.GetComponent<Renderer>();
			}
			GameObject referenceInfo5 = GetReferenceInfo(RefObjKey.S_SimpleBody);
			if ((bool)referenceInfo5)
			{
				base.rendSimpleBody = referenceInfo5.GetComponent<Renderer>();
			}
			GameObject referenceInfo6 = GetReferenceInfo(RefObjKey.S_SimpleTongue);
			if ((bool)referenceInfo6)
			{
				base.rendSimpleTongue = referenceInfo6.GetComponent<Renderer>();
			}
			SetBodyBaseMaterial();
			base.objBody.transform.SetParent(base.objTop.transform, false);
			GameObject referenceInfo7 = GetReferenceInfo(RefObjKey.A_ROOTBONE);
			Transform rootBone = ((!referenceInfo7) ? null : referenceInfo7.transform);
			aaWeightsBody.AssignedWeightsAndSetBounds(base.objBody, "cf_j_root", bounds, rootBone);
			if (base.sex == 1 && base.hiPoly)
			{
				assetBundleName3 = "chara/oo_base.unity3d";
				assetName3 = "p_cf_body_00_Nml";
				BustNormal value = null;
				if (dictBustNormal.TryGetValue(BustNormalKind.NmlBody, out value))
				{
					value.Release();
				}
				value = new BustNormal();
				value.Init(base.objBody, assetBundleName3, assetName3, string.Empty);
				dictBustNormal[BustNormalKind.NmlBody] = value;
			}
			ChangeCustomBodyWithoutCustomTexture();
		}
		if (base.hiPoly)
		{
			string assetBundleName4 = "chara/oo_base.unity3d";
			string assetName4 = "p_tang2";
			if (asyncFlags)
			{
				yield return StartCoroutine(Load_Coroutine(assetBundleName4, assetName4, delegate(GameObject x)
				{
					base.objTongueEx = x;
				}, true, mainManifestName));
			}
			else
			{
				base.objTongueEx = CommonLib.LoadAsset<GameObject>(assetBundleName4, assetName4, true, mainManifestName);
			}
			Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName4, mainManifestName);
			if ((bool)base.objTongueEx)
			{
				GameObject referenceInfo8 = GetReferenceInfo(RefObjKey.HeadParent);
				base.objTongueEx.transform.SetParent(referenceInfo8.transform, false);
				base.animTongueEx = base.objTongueEx.GetComponent<Animator>();
			}
		}
		if (asyncFlags)
		{
			yield return null;
		}
		if (asyncFlags)
		{
			yield return StartCoroutine(ChangeHeadAsync(true));
		}
		else
		{
			ChangeHead(true);
		}
		LoadHairGlossMask();
		if (asyncFlags)
		{
			yield return StartCoroutine(ChangeHairAsync(true));
		}
		else
		{
			ChangeHair(true);
		}
		if (asyncFlags)
		{
			yield return StartCoroutine(ChangeClothesAsync(true));
		}
		else
		{
			ChangeClothes(true);
		}
		if (asyncFlags)
		{
			yield return StartCoroutine(ChangeAccessoryAsync(true));
		}
		else
		{
			ChangeAccessory(true);
		}
		base.updateBustSize = true;
		base.reSetupDynamicBoneBust = true;
		if (base.hiPoly)
		{
			UpdateSiru(true);
		}
		ChangeHohoAkaRate(base.fileStatus.hohoAkaRate);
		if (reflectStatus)
		{
			base.chaFile.SetStatusBytes(status);
		}
		if (Singleton<Character>.Instance.enableCharaLoadGCClear)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
		base.loadEnd = true;
	}

	public bool Reload(bool noChangeClothes = false, bool noChangeHead = false, bool noChangeHair = false, bool noChangeBody = false)
	{
		StartCoroutine(ReloadAsync(noChangeClothes, noChangeHead, noChangeHair, noChangeBody, false));
		return true;
	}

	public IEnumerator ReloadAsync(bool noChangeClothes = false, bool noChangeHead = false, bool noChangeHair = false, bool noChangeBody = false, bool asyncFlags = true)
	{
		if (asyncFlags)
		{
			SetActiveTop(false);
		}
		if (!noChangeBody)
		{
			AddUpdateCMBodyTexFlags(true, true, true, true, true);
			AddUpdateCMBodyColorFlags(true, true, true, true, true, true);
			AddUpdateCMBodyLayoutFlags(true, true);
			CreateBodyTexture();
			if (noChangeHead)
			{
				AddUpdateCMFaceTexFlags(true, true, true, true, true, true, true);
				AddUpdateCMFaceColorFlags(true, true, true, true, true, true, true);
				AddUpdateCMFaceLayoutFlags(true, true, true);
				CreateFaceTexture();
				ChangeCustomFaceWithoutCustomTexture();
			}
		}
		if (!noChangeHead)
		{
			if (asyncFlags)
			{
				yield return StartCoroutine(ChangeHeadAsync(true));
			}
			else
			{
				ChangeHead(true);
			}
		}
		if (!noChangeHair)
		{
			LoadHairGlossMask();
			if (asyncFlags)
			{
				yield return StartCoroutine(ChangeHairAsync(true));
			}
			else
			{
				ChangeHair(true);
			}
		}
		if (!noChangeClothes)
		{
			if (asyncFlags)
			{
				yield return StartCoroutine(ChangeClothesAsync(true));
			}
			else
			{
				ChangeClothes(true);
			}
			if (asyncFlags)
			{
				yield return StartCoroutine(ChangeAccessoryAsync(true));
			}
			else
			{
				ChangeAccessory(true);
			}
		}
		if (asyncFlags)
		{
			yield return null;
		}
		UpdateShapeBodyValueFromCustomInfo();
		UpdateShapeFaceValueFromCustomInfo();
		base.updateBustSize = true;
		base.reSetupDynamicBoneBust = true;
		if (!noChangeBody)
		{
			ChangeCustomBodyWithoutCustomTexture();
		}
		UpdateClothesStateAll();
		if (Singleton<Character>.Instance.enableCharaLoadGCClear)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
		if (asyncFlags)
		{
			yield return null;
		}
	}

	public void ChangeHead(bool forceChange = false)
	{
		StartCoroutine(ChangeHeadAsync(base.fileFace.headId, forceChange, false));
	}

	public void ChangeHead(int _headId, bool forceChange = false)
	{
		StartCoroutine(ChangeHeadAsync(_headId, forceChange, false));
	}

	public IEnumerator ChangeHeadAsync(bool forceChange = false)
	{
		yield return StartCoroutine(ChangeHeadAsync(base.fileFace.headId, forceChange));
	}

	public IEnumerator ChangeHeadAsync(int _headId, bool forceChange = false, bool asyncFlags = true)
	{
		if (_headId == -1 || (!forceChange && null != base.objHead && _headId == base.fileFace.headId))
		{
			yield break;
		}
		string objName = "ct_head";
		if ((bool)base.objHead)
		{
			SafeDestroy(base.objHead);
			base.objHead = null;
			base.infoHead = null;
			ReleaseRefObject(4uL);
			ReleaseShapeFace();
			base.foregroundCtrl = null;
			base.rendFace = null;
			base.rendEye = new Renderer[2];
			base.rendEyeW = new Renderer[2];
			base.rendEyelineUp = null;
			base.rendEyelineDown = null;
			base.rendEyebrow = null;
			base.rendNose = null;
		}
		if (asyncFlags)
		{
			IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
			{
				base.objHead = o;
			}, true, 100, _headId, objName, false, 0, null, 0);
			yield return StartCoroutine(cor);
		}
		else
		{
			base.objHead = LoadCharaFbxData(true, 100, _headId, objName, false, 0, null, 0);
		}
		if ((bool)base.objHead)
		{
			CommonLib.CopySameNameTransform(base.objHeadBone.transform, base.objHead.transform);
			base.objHead.transform.SetParent(base.objHeadBone.transform, false);
			GameObject objRootBone = GetReferenceInfo(RefObjKey.A_ROOTBONE);
			Transform trfRootBone = ((!objRootBone) ? null : objRootBone.transform);
			aaWeightsHead.AssignedWeightsAndSetBounds(base.objHead, "cf_J_N_FaceRoot", bounds, trfRootBone);
			if (asyncFlags)
			{
				yield return null;
			}
			ListInfoComponent libComponent = base.objHead.GetComponent<ListInfoComponent>();
			ListInfoBase listInfoBase = (base.infoHead = libComponent.data);
			ListInfoBase lib = listInfoBase;
			base.foregroundCtrl = base.objHead.GetComponent<ChaForegroundComponent>();
			base.fileFace.headId = lib.Id;
			CreateReferenceInfo(4uL, base.objHead);
			GameObject objEyeL = GetReferenceInfo(RefObjKey.ObjEyeL);
			if ((bool)objEyeL)
			{
				base.eyeLookMatCtrl[0] = objEyeL.GetComponent<EyeLookMaterialControll>();
				if ((bool)base.eyeLookMatCtrl[0])
				{
					base.eyeLookMatCtrl[0].script = base.eyeLookCtrl.eyeLookScript;
					base.eyeLookMatCtrl[0].ReSetupMaterial();
				}
			}
			GameObject objEyeR = GetReferenceInfo(RefObjKey.ObjEyeR);
			if ((bool)objEyeR)
			{
				base.eyeLookMatCtrl[1] = objEyeR.GetComponent<EyeLookMaterialControll>();
				if ((bool)base.eyeLookMatCtrl[1])
				{
					base.eyeLookMatCtrl[1].script = base.eyeLookCtrl.eyeLookScript;
					base.eyeLookMatCtrl[1].ReSetupMaterial();
				}
			}
			GameObject refObj = GetReferenceInfo(RefObjKey.ObjFace);
			if (null != refObj)
			{
				base.rendFace = refObj.GetComponent<Renderer>();
			}
			base.rendEye = new Renderer[2];
			refObj = GetReferenceInfo(RefObjKey.ObjEyeL);
			if (null != refObj)
			{
				base.rendEye[0] = refObj.GetComponent<Renderer>();
			}
			refObj = GetReferenceInfo(RefObjKey.ObjEyeR);
			if (null != refObj)
			{
				base.rendEye[1] = refObj.GetComponent<Renderer>();
			}
			base.rendEyeW = new Renderer[2];
			refObj = GetReferenceInfo(RefObjKey.ObjEyeWL);
			if (null != refObj)
			{
				base.rendEyeW[0] = refObj.GetComponent<Renderer>();
			}
			refObj = GetReferenceInfo(RefObjKey.ObjEyeWR);
			if (null != refObj)
			{
				base.rendEyeW[1] = refObj.GetComponent<Renderer>();
			}
			refObj = GetReferenceInfo(RefObjKey.ObjEyeline);
			if (null != refObj)
			{
				base.rendEyelineUp = refObj.GetComponent<Renderer>();
			}
			refObj = GetReferenceInfo(RefObjKey.ObjEyelineLow);
			if (null != refObj)
			{
				base.rendEyelineDown = refObj.GetComponent<Renderer>();
			}
			refObj = GetReferenceInfo(RefObjKey.ObjEyebrow);
			if (null != refObj)
			{
				base.rendEyebrow = refObj.GetComponent<Renderer>();
			}
			refObj = GetReferenceInfo(RefObjKey.ObjNoseline);
			if (null != refObj)
			{
				base.rendNose = refObj.GetComponent<Renderer>();
			}
			InitBaseCustomTextureFace(drawManifest: lib.GetInfo(ChaListDefine.KeyType.MatManifest), drawAssetBundleName: lib.GetInfo(ChaListDefine.KeyType.MatAB), drawAssetName: lib.GetInfo(ChaListDefine.KeyType.MatData), _sex: base.sex);
			AddUpdateCMFaceTexFlags(true, true, true, true, true, true, true);
			AddUpdateCMFaceColorFlags(true, true, true, true, true, true, true);
			AddUpdateCMFaceLayoutFlags(true, true, true);
			CreateFaceTexture();
			SetFaceBaseMaterial();
			ChangeSettingWhiteOfEye(true, true);
			ChangeSettingEye(2, true, true, true);
			if (base.hiPoly)
			{
				UpdateSiru(true);
			}
			ChangeHohoAkaRate(base.fileStatus.hohoAkaRate);
			InitShapeFace(base.objHeadBone.transform, lib.GetInfo(ChaListDefine.KeyType.MainAB), lib.GetInfo(ChaListDefine.KeyType.ShapeAnime));
			ChangeCustomFaceWithoutCustomTexture();
			HideEyeHighlight(false);
			base.fbsCtrl = base.objHead.GetComponent<FaceBlendShape>();
			if (null != base.fbsCtrl)
			{
				base.eyebrowCtrl = base.fbsCtrl.EyebrowCtrl;
				base.eyesCtrl = base.fbsCtrl.EyesCtrl;
				base.mouthCtrl = base.fbsCtrl.MouthCtrl;
				ChangeEyebrowPtn(base.fileStatus.eyebrowPtn);
				ChangeEyebrowOpenMax(base.fileStatus.eyebrowOpenMax);
				ChangeEyesPtn(base.fileStatus.eyesPtn);
				ChangeEyesOpenMax(base.fileStatus.eyesOpenMax);
				ChangeMouthPtn(base.fileStatus.mouthPtn);
				ChangeMouthOpenMax(base.fileStatus.mouthOpenMax);
				base.fbsCtrl.EyeLookController = base.eyeLookCtrl;
				ChangeEyesBlinkFlag(base.fileStatus.eyesBlink);
			}
		}
		if (asyncFlags)
		{
			yield return null;
		}
	}

	public bool ChangeHairFront(bool forceChange = false)
	{
		int num = 1;
		StartCoroutine(ChangeHairAsync(num, base.fileHair.parts[num].id, forceChange, false));
		return true;
	}

	public bool ChangeHairBack(bool forceChange = false)
	{
		int num = 0;
		StartCoroutine(ChangeHairAsync(num, base.fileHair.parts[num].id, forceChange, false));
		return true;
	}

	public bool ChangeHairSide(bool forceChange = false)
	{
		int num = 2;
		StartCoroutine(ChangeHairAsync(num, base.fileHair.parts[num].id, forceChange, false));
		return true;
	}

	public bool ChangeHairOption(bool forceChange = false)
	{
		int num = 3;
		StartCoroutine(ChangeHairAsync(num, base.fileHair.parts[num].id, forceChange, false));
		return true;
	}

	public void ChangeHair(bool forceChange = false)
	{
		int[] array = (int[])Enum.GetValues(typeof(ChaFileDefine.HairKind));
		int[] array2 = array;
		foreach (int num in array2)
		{
			StartCoroutine(ChangeHairAsync(num, base.fileHair.parts[num].id, forceChange, false));
		}
	}

	public void ChangeHair(int kind, int id, bool forceChange = false)
	{
		StartCoroutine(ChangeHairAsync(kind, id, forceChange, false));
	}

	public IEnumerator ChangeHairAsync(bool forceChange = false)
	{
		int[] hairKind = (int[])Enum.GetValues(typeof(ChaFileDefine.HairKind));
		int[] array = hairKind;
		foreach (int n in array)
		{
			yield return StartCoroutine(ChangeHairAsync(n, base.fileHair.parts[n].id, forceChange));
		}
	}

	public IEnumerator ChangeHairAsync(int kind, int id, bool forceChange = false, bool asyncFlags = true)
	{
		ChaListDefine.CategoryNo[] fbxType = new ChaListDefine.CategoryNo[5]
		{
			ChaListDefine.CategoryNo.bo_hair_b,
			ChaListDefine.CategoryNo.bo_hair_f,
			ChaListDefine.CategoryNo.bo_hair_s,
			ChaListDefine.CategoryNo.bo_hair_o,
			ChaListDefine.CategoryNo.bo_hair_o
		};
		string[] objName = new string[5] { "ct_hairB", "ct_hairF", "ct_hairS", "ct_hairO_01", "ct_hairO_02" };
		int[] defId = new int[5] { 0, 1, 0, 0, 0 };
		if (!forceChange && null != base.objHair[kind] && id == base.fileHair.parts[kind].id)
		{
			yield break;
		}
		if ((bool)base.objHair[kind])
		{
			SafeDestroy(base.objHair[kind]);
			base.objHair[kind] = null;
			base.infoHair[kind] = null;
			base.cusHairCmp[kind] = null;
		}
		GameObject objHairParent = GetReferenceInfo(RefObjKey.HairParent);
		if (asyncFlags)
		{
			IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
			{
				base.objHair[kind] = o;
			}, base.hiPoly, (int)fbxType[kind], id, objName[kind], false, 0, objHairParent.transform, defId[kind]);
			yield return StartCoroutine(cor);
		}
		else
		{
			base.objHair[kind] = LoadCharaFbxData(base.hiPoly, (int)fbxType[kind], id, objName[kind], false, 0, objHairParent.transform, defId[kind]);
		}
		if ((bool)base.objHair[kind])
		{
			ListInfoComponent component = base.objHair[kind].GetComponent<ListInfoComponent>();
			ListInfoBase listInfoBase = (base.infoHair[kind] = component.data);
			base.fileHair.parts[kind].id = listInfoBase.Id;
			if (kind == 0)
			{
				base.fileHair.kind = listInfoBase.Kind;
			}
			base.cusHairCmp[kind] = base.objHair[kind].GetComponent<ChaCustomHairComponent>();
			if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusHairCmp[kind])
			{
			}
			ChangeSettingHairGlossMask(kind);
			ChangeSettingHairColor(kind, true, true, true);
			ChangeSettingHairOutlineColor(kind);
			ChangeSettingHairLength(kind);
			ChangeSettingHairAcsColor(kind);
			if (!base.hiPoly)
			{
				DynamicBone[] componentsInChildren = base.objHair[kind].GetComponentsInChildren<DynamicBone>(true);
				DynamicBone[] array = componentsInChildren;
				foreach (DynamicBone dynamicBone in array)
				{
					dynamicBone.enabled = false;
				}
			}
		}
		if (asyncFlags)
		{
			yield return null;
		}
	}

	public void ChangeClothes(bool forceChange = false)
	{
		int[] array = (int[])Enum.GetValues(typeof(ChaFileDefine.ClothesKind));
		int[] array2 = array;
		foreach (int num in array2)
		{
			StartCoroutine(ChangeClothesAsync(num, nowCoordinate.clothes.parts[num].id, nowCoordinate.clothes.subPartsId[0], nowCoordinate.clothes.subPartsId[1], nowCoordinate.clothes.subPartsId[2], forceChange, false));
		}
	}

	public void ChangeClothes(int kind, int id, int subId01, int subId02, int subId03, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesAsync(kind, id, subId01, subId02, subId03, forceChange, false));
	}

	public IEnumerator ChangeClothesAsync(bool forceChange = false)
	{
		int[] clothesKind = (int[])Enum.GetValues(typeof(ChaFileDefine.ClothesKind));
		int[] array = clothesKind;
		foreach (int n in array)
		{
			yield return StartCoroutine(ChangeClothesAsync(n, nowCoordinate.clothes.parts[n].id, nowCoordinate.clothes.subPartsId[0], nowCoordinate.clothes.subPartsId[1], nowCoordinate.clothes.subPartsId[2], forceChange));
		}
	}

	public IEnumerator ChangeClothesAsync(int kind, int id, int subId01, int subId02, int subId03, bool forceChange = false, bool asyncFlags = true)
	{
		if (asyncFlags)
		{
			switch (kind)
			{
			case 0:
				yield return StartCoroutine(ChangeClothesTopAsync(id, subId01, subId02, subId03, forceChange));
				updateAlphaMask = true;
				break;
			case 1:
				yield return StartCoroutine(ChangeClothesBotAsync(id, forceChange));
				break;
			case 2:
				yield return StartCoroutine(ChangeClothesBraAsync(id, forceChange));
				break;
			case 3:
				yield return StartCoroutine(ChangeClothesShortsAsync(id, forceChange));
				break;
			case 4:
				yield return StartCoroutine(ChangeClothesGlovesAsync(id, forceChange));
				break;
			case 5:
				yield return StartCoroutine(ChangeClothesPanstAsync(id, forceChange));
				break;
			case 6:
				yield return StartCoroutine(ChangeClothesSocksAsync(id, forceChange));
				break;
			case 7:
				yield return StartCoroutine(ChangeClothesShoesAsync(0, id, forceChange));
				break;
			case 8:
				yield return StartCoroutine(ChangeClothesShoesAsync(1, id, forceChange));
				break;
			}
		}
		else
		{
			switch (kind)
			{
			case 0:
				ChangeClothesTop(id, subId01, subId02, subId03, forceChange);
				updateAlphaMask = true;
				break;
			case 1:
				ChangeClothesBot(id, forceChange);
				break;
			case 2:
				ChangeClothesBra(id, forceChange);
				break;
			case 3:
				ChangeClothesShorts(id, forceChange);
				break;
			case 4:
				ChangeClothesGloves(id, forceChange);
				break;
			case 5:
				ChangeClothesPanst(id, forceChange);
				break;
			case 6:
				ChangeClothesSocks(id, forceChange);
				break;
			case 7:
				ChangeClothesShoes(0, id, forceChange);
				break;
			case 8:
				ChangeClothesShoes(1, id, forceChange);
				break;
			}
		}
	}

	public void ChangeClothesTop(int id, int subId01, int subId02, int subId03, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesTopAsync(id, subId01, subId02, subId03, forceChange, false));
	}

	public IEnumerator ChangeClothesTopAsync(int id, int subId01, int subId02, int subId03, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 0;
		string objName = "ct_clothesTop";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			BustNormal value = null;
			if (dictBustNormal.TryGetValue(BustNormalKind.NmlTop, out value))
			{
				value.Release();
			}
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			ReleaseRefObject(5uL);
			notBra = false;
			notBot = false;
			RemoveClothesStateKind(kindNo);
			if (null != texBodyAlphaMask && (bool)base.customMatBody)
			{
				base.customMatBody.SetTexture(ChaShader._AlphaMask, null);
			}
			texBodyAlphaMask = null;
			texBraAlphaMask = null;
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 105, id, objName, true, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 105, id, objName, true, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				CreateReferenceInfo(5uL, base.objClothes[kindNo]);
				notBra = listInfoBase.GetInfoInt(ChaListDefine.KeyType.NotBra) == 1;
				notBot = listInfoBase.GetInfoInt(ChaListDefine.KeyType.Coordinate) == 2;
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
				LoadAlphaMaskTexture(listInfoBase.GetInfo(ChaListDefine.KeyType.OverBodyMaskAB), listInfoBase.GetInfo(ChaListDefine.KeyType.OverBodyMask), 0);
				if ((bool)base.customMatBody)
				{
					base.customMatBody.SetTexture(ChaShader._AlphaMask, texBodyAlphaMask);
				}
				LoadAlphaMaskTexture(listInfoBase.GetInfo(ChaListDefine.KeyType.OverBraMaskAB), listInfoBase.GetInfo(ChaListDefine.KeyType.OverBraMask), 1);
				if (base.rendBra != null)
				{
					Vector2[] array = new Vector2[2]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f)
					};
					Vector2[] array2 = new Vector2[2]
					{
						new Vector2(1f, 1f),
						new Vector2(1f, 2f)
					};
					ListInfoBase listInfoBase2 = base.infoClothes[2];
					if (listInfoBase2 != null)
					{
						int num = ((listInfoBase2.GetInfoInt(ChaListDefine.KeyType.Coordinate) == 2) ? 1 : 0);
						if ((bool)base.rendBra[0])
						{
							base.rendBra[0].material.SetTexture(ChaShader._AlphaMask, texBraAlphaMask);
							base.rendBra[0].material.SetTextureOffset(ChaShader._AlphaMask, array[num]);
							base.rendBra[0].material.SetTextureScale(ChaShader._AlphaMask, array2[num]);
						}
						if ((bool)base.rendBra[1])
						{
							base.rendBra[1].material.SetTexture(ChaShader._AlphaMask, texBraAlphaMask);
							base.rendBra[1].material.SetTextureOffset(ChaShader._AlphaMask, array[num]);
							base.rendBra[1].material.SetTextureScale(ChaShader._AlphaMask, array2[num]);
						}
					}
				}
				string info = listInfoBase.GetInfo(ChaListDefine.KeyType.NormalData);
				if ("0" != info)
				{
					BustNormal value2 = null;
					if (!dictBustNormal.TryGetValue(BustNormalKind.NmlTop, out value2))
					{
						value2 = new BustNormal();
					}
					value2.Init(base.objClothes[kindNo], listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB), info, listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest));
					dictBustNormal[BustNormalKind.NmlTop] = value2;
				}
			}
		}
		if ((bool)base.objClothes[kindNo])
		{
			InitBaseCustomTextureClothes(true, kindNo);
			if (base.loadWithDefaultColorAndPtn)
			{
				SetClothesDefaultColor(kindNo);
				for (int i = 0; i < 4; i++)
				{
					nowCoordinate.clothes.parts[kindNo].colorInfo[i].pattern = 0;
				}
			}
			ChangeCustomClothes(true, kindNo, true, true, true, true, false);
			ChangeCustomEmblem(kindNo);
			if (base.releaseCustomInputTexture)
			{
				ReleaseBaseCustomTextureClothes(true, kindNo, false);
			}
			int[] partsKindNo = new int[3] { 0, 1, 2 };
			int[] partsID = new int[3] { subId01, subId02, subId03 };
			int partsFig = Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length;
			bool forcePartsChange = load || forceChange;
			if (asyncFlags)
			{
				for (int j = 0; j < partsFig; j++)
				{
					yield return StartCoroutine(ChangeClothesTopPartsAsync(partsKindNo[j], partsID[j], forceChange));
				}
			}
			else
			{
				for (int k = 0; k < partsFig; k++)
				{
					ChangeClothesTopParts(partsKindNo[k], partsID[k], forcePartsChange);
				}
			}
		}
		if (base.hiPoly)
		{
			UpdateSiru(true);
		}
		if (!("0" == base.infoClothes[kindNo].GetInfo(ChaListDefine.KeyType.Coordinate)))
		{
			yield break;
		}
		int botKind = 1;
		if (null == base.objClothes[botKind])
		{
			if (asyncFlags)
			{
				yield return StartCoroutine(ChangeClothesBotAsync(nowCoordinate.clothes.parts[botKind].id));
			}
			else
			{
				ChangeClothesBot(nowCoordinate.clothes.parts[botKind].id);
			}
		}
	}

	public void ChangeClothesTopParts(int kind, int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesTopPartsAsync(kind, id, forceChange, false));
	}

	public IEnumerator ChangeClothesTopPartsAsync(int kind, int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int nowId = ((base.infoParts[kind] != null) ? base.infoParts[kind].Id : (-1));
		if (!forceChange && null != base.objParts[kind] && id == nowId)
		{
			load = false;
			release = false;
		}
		int topType = 0;
		ListInfoBase libTop = base.infoClothes[0];
		if (libTop != null)
		{
			topType = libTop.Kind;
		}
		bool noPartsItem = topType != 1 && 2 != topType;
		if (noPartsItem || null == base.objClothes[0])
		{
			load = false;
			release = true;
			nowCoordinate.clothes.subPartsId[kind] = 0;
		}
		ReleaseBaseCustomTextureClothes(false, kind);
		if (release && (bool)base.objParts[kind])
		{
			SafeDestroy(base.objParts[kind]);
			base.objParts[kind] = null;
			BustNormal value = null;
			if (dictBustNormal.TryGetValue(BustNormalKind.NmlTopPA, out value))
			{
				value.Release();
			}
			base.infoParts[kind] = null;
			base.cusClothesSubCmp[kind] = null;
			if ((topType == 1 && kind == 0) || (topType == 2 && kind != 2))
			{
				notBra = false;
			}
			ReleaseRefObject(13uL + (ulong)kind);
			if (!noPartsItem)
			{
				if (null != texBodyAlphaMask && kind == 0)
				{
					if ((bool)base.customMatBody)
					{
						base.customMatBody.SetTexture(ChaShader._AlphaMask, null);
					}
					texBodyAlphaMask = null;
				}
				if (null != texBraAlphaMask && kind == 0)
				{
					if (base.rendBra != null)
					{
						Renderer[] array = base.rendBra;
						foreach (Renderer renderer in array)
						{
							if (null != renderer)
							{
								renderer.material.SetTexture(ChaShader._AlphaMask, null);
							}
						}
					}
					texBraAlphaMask = null;
				}
			}
			if (kind == 0)
			{
				base.rendInner = new Renderer[2];
			}
			if (kind == 1 && null != texInnerAlphaMask)
			{
				if (base.rendInner != null)
				{
					Renderer[] array2 = base.rendInner;
					foreach (Renderer renderer2 in array2)
					{
						if (null != renderer2)
						{
							renderer2.material.SetTexture(ChaShader._AlphaMask, null);
						}
					}
				}
				texInnerAlphaMask = null;
			}
		}
		if (load)
		{
			string[] goName = new string[3] { "ct_top_parts_A", "ct_top_parts_B", "ct_top_parts_C" };
			int cateNo = ((topType != 1) ? 210 : 200) + kind;
			if (asyncFlags)
			{
				yield return null;
			}
			int[,] defaultId = new int[2, 3]
			{
				{ 0, 0, 1 },
				{ 0, 1, 1 }
			};
			int defFirstArrNo = ((topType != 1) ? 1 : 0);
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objParts[kind] = o;
				}, base.hiPoly, cateNo, id, goName[kind], true, 1, base.objClothes[0].transform, defaultId[defFirstArrNo, kind]);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objParts[kind] = LoadCharaFbxData(base.hiPoly, cateNo, id, goName[kind], true, 1, base.objClothes[0].transform, defaultId[defFirstArrNo, kind]);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objParts[kind])
			{
				ListInfoComponent component = base.objParts[kind].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoParts[kind] = component.data);
				base.cusClothesSubCmp[kind] = base.objParts[kind].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesSubCmp[kind])
				{
				}
				nowCoordinate.clothes.subPartsId[kind] = listInfoBase.Id;
				if (topType == 2)
				{
					if (kind == 0)
					{
						if (listInfoBase.Id == 7)
						{
							base.cusClothesSubCmp[kind].useColorN02 = true;
							base.cusClothesSubCmp[kind].useColorN03 = false;
						}
						else if (listInfoBase.Id == 9)
						{
							base.cusClothesSubCmp[kind].useColorN03 = true;
						}
					}
					else if (kind == 1)
					{
						if (listInfoBase.Id == 11)
						{
							base.cusClothesSubCmp[kind].useColorN01 = false;
							base.cusClothesSubCmp[kind].useColorN02 = true;
							base.cusClothesSubCmp[kind].useColorN03 = false;
						}
						else if (listInfoBase.Id == 12)
						{
							base.cusClothesSubCmp[kind].useColorN01 = false;
							base.cusClothesSubCmp[kind].useColorN02 = true;
							base.cusClothesSubCmp[kind].useColorN03 = false;
						}
						else if (listInfoBase.Id == 9)
						{
							base.cusClothesSubCmp[kind].useColorN01 = false;
							base.cusClothesSubCmp[kind].useColorN02 = true;
							base.cusClothesSubCmp[kind].useColorN03 = false;
						}
					}
				}
				CreateReferenceInfo(13uL + (ulong)kind, base.objParts[kind]);
				string info = listInfoBase.GetInfo(ChaListDefine.KeyType.NormalData);
				if (topType == 1 && kind == 0)
				{
					notBra = listInfoBase.GetInfoInt(ChaListDefine.KeyType.NotBra) == 1;
				}
				if (topType == 2)
				{
					ListInfoBase listInfoBase2 = base.infoParts[0];
					ListInfoBase listInfoBase3 = base.infoParts[1];
					notBra = false;
					if (listInfoBase2 != null && listInfoBase2.GetInfoInt(ChaListDefine.KeyType.NotBra) == 1)
					{
						notBra = true;
					}
					if (listInfoBase3 != null && listInfoBase3.GetInfoInt(ChaListDefine.KeyType.NotBra) == 1)
					{
						notBra = true;
					}
				}
				if ("0" != info)
				{
					BustNormal value2 = null;
					if (!dictBustNormal.TryGetValue(BustNormalKind.NmlTopPA, out value2))
					{
						value2 = new BustNormal();
					}
					value2.Init(base.objParts[kind], listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB), info, listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest));
					dictBustNormal[BustNormalKind.NmlTopPA] = value2;
				}
				if (kind == 0)
				{
					GameObject referenceInfo = GetReferenceInfo(RefObjKey.ObjInnerDef);
					if (null != referenceInfo)
					{
						base.rendInner[0] = referenceInfo.GetComponent<Renderer>();
					}
					referenceInfo = GetReferenceInfo(RefObjKey.ObjInnerNuge);
					if (null != referenceInfo)
					{
						base.rendInner[1] = referenceInfo.GetComponent<Renderer>();
					}
					if (base.rendInner != null)
					{
						if ((bool)base.rendInner[0])
						{
							base.rendInner[0].material.SetTexture(ChaShader._AlphaMask, texInnerAlphaMask);
						}
						if ((bool)base.rendInner[1])
						{
							base.rendInner[1].material.SetTexture(ChaShader._AlphaMask, texInnerAlphaMask);
						}
					}
					LoadAlphaMaskTexture(listInfoBase.GetInfo(ChaListDefine.KeyType.OverBodyMaskAB), listInfoBase.GetInfo(ChaListDefine.KeyType.OverBodyMask), 0);
					if ((bool)base.customMatBody)
					{
						base.customMatBody.SetTexture(ChaShader._AlphaMask, texBodyAlphaMask);
					}
					LoadAlphaMaskTexture(listInfoBase.GetInfo(ChaListDefine.KeyType.OverBraMaskAB), listInfoBase.GetInfo(ChaListDefine.KeyType.OverBraMask), 1);
					if (base.rendBra != null)
					{
						Vector2[] array3 = new Vector2[2]
						{
							new Vector2(0f, 0f),
							new Vector2(0f, -1f)
						};
						Vector2[] array4 = new Vector2[2]
						{
							new Vector2(1f, 1f),
							new Vector2(1f, 2f)
						};
						ListInfoBase listInfoBase4 = base.infoClothes[2];
						if (listInfoBase4 != null)
						{
							int num = ((listInfoBase4.GetInfoInt(ChaListDefine.KeyType.Coordinate) == 2) ? 1 : 0);
							if ((bool)base.rendBra[0])
							{
								base.rendBra[0].material.SetTexture(ChaShader._AlphaMask, texBraAlphaMask);
								base.rendBra[0].material.SetTextureOffset(ChaShader._AlphaMask, array3[num]);
								base.rendBra[0].material.SetTextureScale(ChaShader._AlphaMask, array4[num]);
							}
							if ((bool)base.rendBra[1])
							{
								base.rendBra[1].material.SetTexture(ChaShader._AlphaMask, texBraAlphaMask);
								base.rendBra[1].material.SetTextureOffset(ChaShader._AlphaMask, array3[num]);
								base.rendBra[1].material.SetTextureScale(ChaShader._AlphaMask, array4[num]);
							}
						}
					}
				}
				else if (kind == 1)
				{
					LoadAlphaMaskTexture(listInfoBase.GetInfo(ChaListDefine.KeyType.OverInnerMaskAB), listInfoBase.GetInfo(ChaListDefine.KeyType.OverInnerMask), 2);
					if (base.rendInner != null)
					{
						if ((bool)base.rendInner[0])
						{
							base.rendInner[0].material.SetTexture(ChaShader._AlphaMask, texInnerAlphaMask);
						}
						if ((bool)base.rendInner[1])
						{
							base.rendInner[1].material.SetTexture(ChaShader._AlphaMask, texInnerAlphaMask);
						}
					}
					byte b = base.fileStatus.clothesState[0];
					byte[,] array5 = new byte[4, 2]
					{
						{ 1, 1 },
						{ 0, 1 },
						{ 0, 1 },
						{ 0, 0 }
					};
					ChangeAlphaMask(array5[b, 0], array5[b, 1]);
				}
			}
		}
		if (!base.objParts[kind])
		{
			yield break;
		}
		InitBaseCustomTextureClothes(false, kind);
		if (base.loadWithDefaultColorAndPtn)
		{
			SetClothesDefaultColor(0);
			for (int k = 0; k < 4; k++)
			{
				nowCoordinate.clothes.parts[0].colorInfo[k].pattern = 0;
			}
		}
		ChangeCustomClothes(false, kind, true, true, true, true, true);
		ChangeCustomEmblem(0);
		if (base.releaseCustomInputTexture)
		{
			ReleaseBaseCustomTextureClothes(true, kind, false);
		}
	}

	public void ChangeClothesBot(int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesBotAsync(id, forceChange, false));
	}

	public IEnumerator ChangeClothesBotAsync(int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 1;
		string objName = "ct_clothesBot";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		Dictionary<int, ListInfoBase> dictFbx = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_top);
		if (dictFbx.Count != 0)
		{
			ListInfoBase value = null;
			if (dictFbx.TryGetValue(nowCoordinate.clothes.parts[0].id, out value) && "2" == value.GetInfo(ChaListDefine.KeyType.Coordinate))
			{
				load = false;
				release = true;
			}
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			ReleaseRefObject(6uL);
			RemoveClothesStateKind(kindNo);
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 106, id, objName, true, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 106, id, objName, true, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				CreateReferenceInfo(6uL, base.objClothes[kindNo]);
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
			}
		}
		if ((bool)base.objClothes[kindNo])
		{
			InitBaseCustomTextureClothes(true, kindNo);
			if (base.loadWithDefaultColorAndPtn)
			{
				SetClothesDefaultColor(kindNo);
				for (int i = 0; i < 4; i++)
				{
					nowCoordinate.clothes.parts[kindNo].colorInfo[i].pattern = 0;
				}
			}
			ChangeCustomClothes(true, kindNo, true, true, true, true, false);
			ChangeCustomEmblem(kindNo);
			if (base.releaseCustomInputTexture)
			{
				ReleaseBaseCustomTextureClothes(true, kindNo, false);
			}
		}
		if (base.hiPoly)
		{
			UpdateSiru(true);
		}
	}

	public void ChangeClothesBra(int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesBraAsync(id, forceChange, false));
	}

	public IEnumerator ChangeClothesBraAsync(int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 2;
		string objName = "ct_bra";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			List<GameObject>[] array = base.lstObjBraOpt;
			foreach (List<GameObject> list in array)
			{
				list.Clear();
			}
			BustNormal value = null;
			if (dictBustNormal.TryGetValue(BustNormalKind.NmlBra, out value))
			{
				value.Release();
			}
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			ReleaseRefObject(7uL);
			notShorts = false;
			RemoveClothesStateKind(kindNo);
			base.rendBra = new Renderer[2];
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 107, id, objName, false, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 107, id, objName, false, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				notShorts = (("2" == listInfoBase.GetInfo(ChaListDefine.KeyType.Coordinate)) ? true : false);
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				CreateReferenceInfo(7uL, base.objClothes[kindNo]);
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
				GameObject referenceInfo = GetReferenceInfo(RefObjKey.ObjBraDef);
				if (null != referenceInfo)
				{
					base.rendBra[0] = referenceInfo.GetComponent<Renderer>();
				}
				referenceInfo = GetReferenceInfo(RefObjKey.ObjBraNuge);
				if (null != referenceInfo)
				{
					base.rendBra[1] = referenceInfo.GetComponent<Renderer>();
				}
				if (null != base.cusClothesCmp[kindNo])
				{
					if (base.cusClothesCmp[kindNo].useOpt01 && base.cusClothesCmp[kindNo].objOpt01 != null)
					{
						GameObject[] objOpt = base.cusClothesCmp[kindNo].objOpt01;
						foreach (GameObject item in objOpt)
						{
							base.lstObjBraOpt[0].Add(item);
						}
					}
					if (base.cusClothesCmp[kindNo].useOpt02 && base.cusClothesCmp[kindNo].objOpt02 != null)
					{
						GameObject[] objOpt2 = base.cusClothesCmp[kindNo].objOpt02;
						foreach (GameObject item2 in objOpt2)
						{
							base.lstObjBraOpt[1].Add(item2);
						}
					}
				}
				if (base.rendBra != null)
				{
					Vector2[] array2 = new Vector2[2]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f)
					};
					Vector2[] array3 = new Vector2[2]
					{
						new Vector2(1f, 1f),
						new Vector2(1f, 2f)
					};
					int num = ((listInfoBase.GetInfoInt(ChaListDefine.KeyType.Coordinate) == 2) ? 1 : 0);
					if ((bool)base.rendBra[0])
					{
						base.rendBra[0].material.SetTexture(ChaShader._AlphaMask, texBraAlphaMask);
						base.rendBra[0].material.SetTextureOffset(ChaShader._AlphaMask, array2[num]);
						base.rendBra[0].material.SetTextureScale(ChaShader._AlphaMask, array3[num]);
					}
					if ((bool)base.rendBra[1])
					{
						base.rendBra[1].material.SetTexture(ChaShader._AlphaMask, texBraAlphaMask);
						base.rendBra[1].material.SetTextureOffset(ChaShader._AlphaMask, array2[num]);
						base.rendBra[1].material.SetTextureScale(ChaShader._AlphaMask, array3[num]);
					}
				}
				byte b = base.fileStatus.clothesState[0];
				byte[,] array4 = new byte[4, 2]
				{
					{ 1, 1 },
					{ 0, 1 },
					{ 0, 1 },
					{ 0, 0 }
				};
				ChangeAlphaMask(array4[b, 0], array4[b, 1]);
				string info = listInfoBase.GetInfo(ChaListDefine.KeyType.NormalData);
				if ("0" != info)
				{
					BustNormal value2 = null;
					if (!dictBustNormal.TryGetValue(BustNormalKind.NmlBra, out value2))
					{
						value2 = new BustNormal();
					}
					value2.Init(base.objClothes[kindNo], listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB), info, listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest));
					dictBustNormal[BustNormalKind.NmlBra] = value2;
				}
			}
		}
		if ((bool)base.objClothes[kindNo])
		{
			InitBaseCustomTextureClothes(true, kindNo);
			if (base.loadWithDefaultColorAndPtn)
			{
				SetClothesDefaultColor(kindNo);
				for (int l = 0; l < 4; l++)
				{
					nowCoordinate.clothes.parts[kindNo].colorInfo[l].pattern = 0;
				}
			}
			ChangeCustomClothes(true, kindNo, true, true, true, true, false);
			ChangeCustomEmblem(kindNo);
			if (base.releaseCustomInputTexture)
			{
				ReleaseBaseCustomTextureClothes(true, kindNo, false);
			}
		}
		if (base.hiPoly)
		{
			UpdateSiru(true);
		}
		if (!("0" == base.infoClothes[kindNo].GetInfo(ChaListDefine.KeyType.Coordinate)))
		{
			yield break;
		}
		int shortsKind = 3;
		if (null == base.objClothes[shortsKind])
		{
			if (asyncFlags)
			{
				yield return StartCoroutine(ChangeClothesShortsAsync(nowCoordinate.clothes.parts[shortsKind].id));
			}
			else
			{
				ChangeClothesShorts(nowCoordinate.clothes.parts[shortsKind].id);
			}
		}
	}

	public void ChangeClothesShorts(int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesShortsAsync(id, forceChange, false));
	}

	public IEnumerator ChangeClothesShortsAsync(int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 3;
		string objName = "ct_shorts";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		Dictionary<int, ListInfoBase> dictFbx = base.lstCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.co_bra);
		if (dictFbx.Count != 0)
		{
			ListInfoBase value = null;
			if (dictFbx.TryGetValue(nowCoordinate.clothes.parts[2].id, out value) && "2" == value.GetInfo(ChaListDefine.KeyType.Coordinate))
			{
				load = false;
				release = true;
			}
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			List<GameObject>[] array = base.lstObjShortsOpt;
			foreach (List<GameObject> list in array)
			{
				list.Clear();
			}
			ReleaseRefObject(8uL);
			RemoveClothesStateKind(kindNo);
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 108, id, objName, false, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 108, id, objName, false, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				if (listInfoBase.Id == 17)
				{
					for (int j = 0; j < base.cusClothesCmp[kindNo].rendNormal01.Length; j++)
					{
						base.cusClothesCmp[kindNo].rendNormal01[j].material.renderQueue = 3030;
					}
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				CreateReferenceInfo(8uL, base.objClothes[kindNo]);
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
				if (null != base.cusClothesCmp[kindNo])
				{
					if (base.cusClothesCmp[kindNo].useOpt01 && base.cusClothesCmp[kindNo].objOpt01 != null)
					{
						GameObject[] objOpt = base.cusClothesCmp[kindNo].objOpt01;
						foreach (GameObject item in objOpt)
						{
							base.lstObjShortsOpt[0].Add(item);
						}
					}
					if (base.cusClothesCmp[kindNo].useOpt02 && base.cusClothesCmp[kindNo].objOpt02 != null)
					{
						GameObject[] objOpt2 = base.cusClothesCmp[kindNo].objOpt02;
						foreach (GameObject item2 in objOpt2)
						{
							base.lstObjShortsOpt[1].Add(item2);
						}
					}
				}
			}
		}
		if ((bool)base.objClothes[kindNo])
		{
			InitBaseCustomTextureClothes(true, kindNo);
			if (base.loadWithDefaultColorAndPtn)
			{
				SetClothesDefaultColor(kindNo);
				for (int m = 0; m < 4; m++)
				{
					nowCoordinate.clothes.parts[kindNo].colorInfo[m].pattern = 0;
				}
			}
			ChangeCustomClothes(true, kindNo, true, true, true, true, false);
			ChangeCustomEmblem(kindNo);
			if (base.releaseCustomInputTexture)
			{
				ReleaseBaseCustomTextureClothes(true, kindNo, false);
			}
		}
		if (base.hiPoly)
		{
			UpdateSiru(true);
		}
	}

	public void ChangeClothesGloves(int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesGlovesAsync(id, forceChange, false));
	}

	public IEnumerator ChangeClothesGlovesAsync(int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 4;
		string objName = "ct_gloves";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			RemoveClothesStateKind(kindNo);
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 109, id, objName, false, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 109, id, objName, false, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
			}
		}
		if (!base.objClothes[kindNo])
		{
			yield break;
		}
		InitBaseCustomTextureClothes(true, kindNo);
		if (base.loadWithDefaultColorAndPtn)
		{
			SetClothesDefaultColor(kindNo);
			for (int i = 0; i < 4; i++)
			{
				nowCoordinate.clothes.parts[kindNo].colorInfo[i].pattern = 0;
			}
		}
		ChangeCustomClothes(true, kindNo, true, true, true, true, false);
		ChangeCustomEmblem(kindNo);
		if (base.releaseCustomInputTexture)
		{
			ReleaseBaseCustomTextureClothes(true, kindNo, false);
		}
	}

	public void ChangeClothesPanst(int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesPanstAsync(id, forceChange, false));
	}

	public IEnumerator ChangeClothesPanstAsync(int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 5;
		string objName = "ct_panst";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			ReleaseRefObject(10uL);
			RemoveClothesStateKind(kindNo);
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 110, id, objName, false, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 110, id, objName, false, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				if (listInfoBase.Id == 11)
				{
					for (int i = 0; i < base.cusClothesCmp[kindNo].rendNormal01.Length; i++)
					{
						base.cusClothesCmp[kindNo].rendNormal01[i].material.renderQueue = 3040;
					}
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				CreateReferenceInfo(10uL, base.objClothes[kindNo]);
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
			}
		}
		if ((bool)base.objClothes[kindNo])
		{
			InitBaseCustomTextureClothes(true, kindNo);
			if (base.loadWithDefaultColorAndPtn)
			{
				SetClothesDefaultColor(kindNo);
				for (int j = 0; j < 4; j++)
				{
					nowCoordinate.clothes.parts[kindNo].colorInfo[j].pattern = 0;
				}
			}
			ChangeCustomClothes(true, kindNo, true, true, true, true, false);
			ChangeCustomEmblem(kindNo);
			if (base.releaseCustomInputTexture)
			{
				ReleaseBaseCustomTextureClothes(true, kindNo, false);
			}
		}
		if (base.hiPoly)
		{
			UpdateSiru(true);
		}
	}

	public void ChangeClothesSocks(int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesSocksAsync(id, forceChange, false));
	}

	public IEnumerator ChangeClothesSocksAsync(int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 6;
		string objName = "ct_socks";
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			RemoveClothesStateKind(kindNo);
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 111, id, objName, false, 1, base.objTop.transform, 0);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 111, id, objName, false, 1, base.objTop.transform, 0);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				if (listInfoBase.Id == 21)
				{
					for (int i = 0; i < base.cusClothesCmp[kindNo].rendNormal01.Length; i++)
					{
						base.cusClothesCmp[kindNo].rendNormal01[i].material.renderQueue = 3050;
					}
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
			}
		}
		if (!base.objClothes[kindNo])
		{
			yield break;
		}
		InitBaseCustomTextureClothes(true, kindNo);
		if (base.loadWithDefaultColorAndPtn)
		{
			SetClothesDefaultColor(kindNo);
			for (int j = 0; j < 4; j++)
			{
				nowCoordinate.clothes.parts[kindNo].colorInfo[j].pattern = 0;
			}
		}
		ChangeCustomClothes(true, kindNo, true, true, true, true, false);
		ChangeCustomEmblem(kindNo);
		if (base.releaseCustomInputTexture)
		{
			ReleaseBaseCustomTextureClothes(true, kindNo, false);
		}
	}

	public void ChangeClothesShoes(int type, int id, bool forceChange = false)
	{
		StartCoroutine(ChangeClothesShoesAsync(type, id, forceChange, false));
	}

	public IEnumerator ChangeClothesShoesAsync(int type, int id, bool forceChange = false, bool asyncFlags = true)
	{
		bool load = true;
		bool release = true;
		int kindNo = 0;
		string objName = string.Empty;
		int defID = 0;
		if (type == 0)
		{
			kindNo = 7;
			objName = "ct_shoes_inner";
			defID = 0;
		}
		else
		{
			kindNo = 8;
			objName = "ct_shoes_outer";
			defID = 0;
		}
		int nowId = ((base.infoClothes[kindNo] != null) ? base.infoClothes[kindNo].Id : (-1));
		if (!forceChange && null != base.objClothes[kindNo] && id == nowId)
		{
			load = false;
			release = false;
		}
		ReleaseBaseCustomTextureClothes(true, kindNo);
		if (release && (bool)base.objClothes[kindNo])
		{
			SafeDestroy(base.objClothes[kindNo]);
			base.objClothes[kindNo] = null;
			base.infoClothes[kindNo] = null;
			base.cusClothesCmp[kindNo] = null;
			RemoveClothesStateKind(kindNo);
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objClothes[kindNo] = o;
				}, base.hiPoly, 112, id, objName, false, 1, base.objTop.transform, defID);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objClothes[kindNo] = LoadCharaFbxData(base.hiPoly, 112, id, objName, false, 1, base.objTop.transform, defID);
			}
			if (asyncFlags)
			{
				yield return null;
			}
			if ((bool)base.objClothes[kindNo])
			{
				ListInfoComponent component = base.objClothes[kindNo].GetComponent<ListInfoComponent>();
				ListInfoBase listInfoBase = (base.infoClothes[kindNo] = component.data);
				base.cusClothesCmp[kindNo] = base.objClothes[kindNo].GetComponent<ChaClothesComponent>();
				if (!(listInfoBase.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusClothesCmp[kindNo])
				{
				}
				nowCoordinate.clothes.parts[kindNo].id = listInfoBase.Id;
				AddClothesStateKind(kindNo, listInfoBase.GetInfo(ChaListDefine.KeyType.StateType));
			}
		}
		if (!base.objClothes[kindNo])
		{
			yield break;
		}
		InitBaseCustomTextureClothes(true, kindNo);
		if (base.loadWithDefaultColorAndPtn)
		{
			SetClothesDefaultColor(kindNo);
			for (int i = 0; i < 4; i++)
			{
				nowCoordinate.clothes.parts[kindNo].colorInfo[i].pattern = 0;
			}
		}
		ChangeCustomClothes(true, kindNo, true, true, true, true, false);
		ChangeCustomEmblem(kindNo);
		if (base.releaseCustomInputTexture)
		{
			ReleaseBaseCustomTextureClothes(true, kindNo, false);
		}
	}

	public void ChangeAccessory(bool forceChange = false)
	{
		for (int i = 0; i < 20; i++)
		{
			StartCoroutine(ChangeAccessoryAsync(i, nowCoordinate.accessory.parts[i].type, nowCoordinate.accessory.parts[i].id, nowCoordinate.accessory.parts[i].parentKey, forceChange, false));
		}
	}

	public void ChangeAccessory(int slotNo, int type, int id, string parentKey, bool forceChange = false)
	{
		StartCoroutine(ChangeAccessoryAsync(slotNo, type, id, parentKey, forceChange, false));
	}

	public IEnumerator ChangeAccessoryAsync(bool forceChange = false)
	{
		for (int i = 0; i < 20; i++)
		{
			yield return StartCoroutine(ChangeAccessoryAsync(i, nowCoordinate.accessory.parts[i].type, nowCoordinate.accessory.parts[i].id, nowCoordinate.accessory.parts[i].parentKey, forceChange));
		}
	}

	public IEnumerator ChangeAccessoryAsync(int slotNo, int type, int id, string parentKey, bool forceChange = false, bool asyncFlags = true)
	{
		if (!MathfEx.RangeEqualOn(0, slotNo, 19))
		{
			yield break;
		}
		ListInfoBase lib = null;
		bool load = true;
		bool release = true;
		if (type == 120 || !MathfEx.RangeEqualOn(121, type, 130))
		{
			release = true;
			load = false;
		}
		else
		{
			if (id == -1)
			{
				release = false;
				load = false;
			}
			int num = ((base.infoAccessory[slotNo] != null) ? base.infoAccessory[slotNo].Category : (-1));
			int num2 = ((base.infoAccessory[slotNo] != null) ? base.infoAccessory[slotNo].Id : (-1));
			if (!forceChange && null != base.objAccessory[slotNo] && type == num && id == num2)
			{
				load = false;
				release = false;
			}
			if (id != -1)
			{
				Dictionary<int, ListInfoBase> categoryInfo = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)type);
				if (categoryInfo == null)
				{
					release = true;
					load = false;
				}
				else if (!categoryInfo.TryGetValue(id, out lib))
				{
					release = true;
					load = false;
				}
				else if (!base.hiPoly)
				{
					bool flag = true;
					if (type == 123 && lib.Kind == 1)
					{
						flag = false;
					}
					if (type == 122 && lib.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
					{
						flag = false;
					}
					if (Manager.Config.EtcData.loadHeadAccessory && type == 122 && lib.Kind == 1)
					{
						flag = false;
					}
					if (Manager.Config.EtcData.loadAllAccessory)
					{
						flag = false;
					}
					if (flag)
					{
						release = true;
						load = false;
					}
				}
			}
		}
		if (release)
		{
			if (!load)
			{
				nowCoordinate.accessory.parts[slotNo].MemberInit();
				nowCoordinate.accessory.parts[slotNo].type = 120;
			}
			if ((bool)base.objAccessory[slotNo])
			{
				SafeDestroy(base.objAccessory[slotNo]);
				base.objAccessory[slotNo] = null;
				base.infoAccessory[slotNo] = null;
				base.cusAcsCmp[slotNo] = null;
				for (int i = 0; i < 2; i++)
				{
					base.objAcsMove[slotNo, i] = null;
				}
			}
		}
		if (load)
		{
			if (asyncFlags)
			{
				yield return null;
			}
			byte weight = 0;
			Transform trfParent = null;
			if ("null" == lib.GetInfo(ChaListDefine.KeyType.Parent))
			{
				weight = 2;
				trfParent = base.objTop.transform;
			}
			if (asyncFlags)
			{
				IEnumerator cor = LoadCharaFbxDataAsync(delegate(GameObject o)
				{
					base.objAccessory[slotNo] = o;
				}, true, type, id, "ca_slot" + slotNo.ToString("00"), false, weight, trfParent, -1);
				yield return StartCoroutine(cor);
			}
			else
			{
				base.objAccessory[slotNo] = LoadCharaFbxData(true, type, id, "ca_slot" + slotNo.ToString("00"), false, weight, trfParent, -1);
			}
			if ((bool)base.objAccessory[slotNo])
			{
				ListInfoComponent component = base.objAccessory[slotNo].GetComponent<ListInfoComponent>();
				lib = (base.infoAccessory[slotNo] = component.data);
				base.cusAcsCmp[slotNo] = base.objAccessory[slotNo].GetComponent<ChaAccessoryComponent>();
				if (!(lib.GetInfo(ChaListDefine.KeyType.MainData) != "p_dummy") || null == base.cusAcsCmp[slotNo])
				{
				}
				nowCoordinate.accessory.parts[slotNo].type = type;
				nowCoordinate.accessory.parts[slotNo].id = lib.Id;
				base.objAcsMove[slotNo, 0] = base.objAccessory[slotNo].transform.FindLoop("N_move");
				base.objAcsMove[slotNo, 1] = base.objAccessory[slotNo].transform.FindLoop("N_move2");
			}
		}
		if ((bool)base.objAccessory[slotNo])
		{
			if (base.loadWithDefaultColorAndPtn)
			{
				SetAccessoryDefaultColor(slotNo);
			}
			ChangeAccessoryColor(slotNo);
			if (string.Empty == parentKey)
			{
				parentKey = lib.GetInfo(ChaListDefine.KeyType.Parent);
			}
			ChangeAccessoryParent(slotNo, parentKey);
			UpdateAccessoryMoveFromInfo(slotNo);
			nowCoordinate.accessory.parts[slotNo].partsOfHead = ChaAccessoryDefine.CheckPartsOfHead(parentKey);
			if (!base.hiPoly && !Manager.Config.EtcData.loadAllAccessory)
			{
				DynamicBone[] componentsInChildren = base.objAccessory[slotNo].GetComponentsInChildren<DynamicBone>(true);
				DynamicBone[] array = componentsInChildren;
				foreach (DynamicBone dynamicBone in array)
				{
					dynamicBone.enabled = false;
				}
			}
		}
		SetHideHairAccessory();
	}

	private GameObject LoadCharaFbxData(bool _hiPoly, int category, int id, string createName, bool copyDynamicBone, byte copyWeights, Transform trfParent, int defaultId, bool worldPositionStays = false)
	{
		GameObject actObj = null;
		StartCoroutine(LoadCharaFbxDataAsync(delegate(GameObject o)
		{
			actObj = o;
		}, _hiPoly, category, id, createName, copyDynamicBone, copyWeights, trfParent, defaultId, false, worldPositionStays));
		return actObj;
	}

	private IEnumerator LoadCharaFbxDataAsync(Action<GameObject> actObj, bool _hiPoly, int category, int id, string createName, bool copyDynamicBone, byte copyWeights, Transform trfParent, int defaultId, bool AsyncFlags = true, bool worldPositionStays = false)
	{
		Dictionary<int, ListInfoBase> work = null;
		work = base.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)category);
		if (work.Count == 0)
		{
			actObj(null);
			yield break;
		}
		ListInfoBase lib = null;
		if (!work.TryGetValue(id, out lib))
		{
			if (defaultId == -1)
			{
				actObj(null);
				yield break;
			}
			if (id != defaultId)
			{
				work.TryGetValue(defaultId, out lib);
			}
			if (lib == null && !work.TryGetValue(0, out lib))
			{
				actObj(null);
				yield break;
			}
		}
		else if (category == 105 || category == 107)
		{
			int infoInt = lib.GetInfoInt(ChaListDefine.KeyType.Sex);
			bool flag = false;
			if (base.sex == 0 && infoInt == 3)
			{
				flag = true;
			}
			else if (base.sex == 1 && infoInt == 2)
			{
				flag = true;
			}
			if (flag)
			{
				if (id != defaultId)
				{
					work.TryGetValue(defaultId, out lib);
				}
				if (lib == null && !work.TryGetValue(0, out lib))
				{
					actObj(null);
					yield break;
				}
			}
		}
		string assetName = lib.GetInfo(ChaListDefine.KeyType.MainData);
		if (string.Empty == assetName)
		{
			yield break;
		}
		if (!_hiPoly)
		{
			assetName += "_low";
		}
		string manifestName = lib.GetInfo(ChaListDefine.KeyType.MainManifest);
		string assetBundleName = lib.GetInfo(ChaListDefine.KeyType.MainAB);
		GameObject newObj = null;
		if (AsyncFlags)
		{
			yield return StartCoroutine(Load_Coroutine(assetBundleName, assetName, delegate(GameObject x)
			{
				newObj = x;
			}, true, manifestName));
		}
		else
		{
			newObj = CommonLib.LoadAsset<GameObject>(assetBundleName, assetName, true, manifestName);
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, manifestName);
		if (null == newObj)
		{
			actObj(null);
			yield break;
		}
		newObj.name = createName;
		if ((bool)trfParent)
		{
			newObj.transform.SetParent(trfParent, worldPositionStays);
		}
		DynamicBoneCollider[] dbc = base.objBodyBone.GetComponentsInChildren<DynamicBoneCollider>(true);
		Dictionary<string, GameObject> dictBone = aaWeightsBody.dictBone;
		DynamicBone[] db = newObj.GetComponentsInChildren<DynamicBone>(true);
		DynamicBone[] array = db;
		foreach (DynamicBone dynamicBone in array)
		{
			if (copyDynamicBone)
			{
				if ((bool)dynamicBone.m_Root)
				{
					foreach (KeyValuePair<string, GameObject> item in dictBone)
					{
						if (item.Key != dynamicBone.m_Root.name)
						{
							continue;
						}
						dynamicBone.m_Root = item.Value.transform;
						break;
					}
				}
				if (dynamicBone.m_Exclusions != null && dynamicBone.m_Exclusions.Count != 0)
				{
					for (int j = 0; j < dynamicBone.m_Exclusions.Count; j++)
					{
						if (null == dynamicBone.m_Exclusions[j])
						{
							continue;
						}
						foreach (KeyValuePair<string, GameObject> item2 in dictBone)
						{
							if (item2.Key != dynamicBone.m_Exclusions[j].name)
							{
								continue;
							}
							dynamicBone.m_Exclusions[j] = item2.Value.transform;
							break;
						}
					}
				}
				if (dynamicBone.m_notRolls != null && dynamicBone.m_notRolls.Count != 0)
				{
					for (int k = 0; k < dynamicBone.m_notRolls.Count; k++)
					{
						if (null == dynamicBone.m_notRolls[k])
						{
							continue;
						}
						foreach (KeyValuePair<string, GameObject> item3 in dictBone)
						{
							if (item3.Key != dynamicBone.m_notRolls[k].name)
							{
								continue;
							}
							dynamicBone.m_notRolls[k] = item3.Value.transform;
							break;
						}
					}
				}
			}
			if (dynamicBone.m_Colliders != null)
			{
				dynamicBone.m_Colliders.Clear();
				for (int l = 0; l < dbc.Length; l++)
				{
					dynamicBone.m_Colliders.Add(dbc[l]);
				}
			}
		}
		GameObject objRootBone = GetReferenceInfo(RefObjKey.A_ROOTBONE);
		Transform trfRootBone = ((!objRootBone) ? null : objRootBone.transform);
		switch (copyWeights)
		{
		case 1:
			aaWeightsBody.AssignedWeightsAndSetBounds(newObj, "cf_j_root", bounds, trfRootBone);
			break;
		case 2:
			aaWeightsHead.AssignedWeightsAndSetBounds(newObj, "cf_J_N_FaceRoot", bounds, trfRootBone);
			break;
		}
		ListInfoComponent libComponent = newObj.AddComponent<ListInfoComponent>();
		libComponent.data = lib.Clone();
		actObj(newObj);
	}

	protected bool InitBaseCustomTextureBody(byte _sex)
	{
		if (base.customTexCtrlBody != null)
		{
			base.customTexCtrlBody.Release();
			base.customTexCtrlBody = null;
		}
		string text = "chara/mm_base.unity3d";
		string drawMatName = ((_sex != 0) ? "cf_m_body" : "cm_m_body");
		string createMatName = "cf_m_body_create";
		int num = ((!base.hiPoly) ? 512 : 2048);
		base.customTexCtrlBody = new CustomTextureControl(base.objRoot.transform);
		base.customTexCtrlBody.Initialize(text, drawMatName, string.Empty, text, createMatName, string.Empty, num, num);
		return true;
	}

	protected bool InitBaseCustomTextureFace(byte _sex, string drawAssetBundleName, string drawAssetName, string drawManifest)
	{
		if (base.customTexCtrlFace != null)
		{
			base.customTexCtrlFace.Release();
			base.customTexCtrlFace = null;
		}
		string createMatABName = "chara/mm_base.unity3d";
		string createMatName = "cf_m_face_create";
		int num = ((!base.hiPoly) ? 512 : 1024);
		base.customTexCtrlFace = new CustomTextureControl(base.objRoot.transform);
		base.customTexCtrlFace.Initialize(drawAssetBundleName, drawAssetName, drawManifest, createMatABName, createMatName, string.Empty, num, num);
		return true;
	}

	protected bool InitBaseCustomTextureEtc()
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		int num = ((!base.hiPoly) ? 256 : 512);
		if (base.ctCreateEyeW != null)
		{
			base.ctCreateEyeW.Release();
			base.ctCreateEyeW = null;
		}
		empty = "chara/mm_base.unity3d";
		empty2 = "cf_m_eyewhite_create";
		base.ctCreateEyeW = new CustomTextureCreate(base.objRoot.transform);
		base.ctCreateEyeW.Initialize(empty, empty2, string.Empty, num, num);
		if (base.ctCreateEyeL != null)
		{
			base.ctCreateEyeL.Release();
			base.ctCreateEyeL = null;
		}
		empty = "chara/mm_base.unity3d";
		empty2 = "cf_m_eye_create";
		base.ctCreateEyeL = new CustomTextureCreate(base.objRoot.transform);
		base.ctCreateEyeL.Initialize(empty, empty2, string.Empty, num, num);
		if (base.ctCreateEyeR != null)
		{
			base.ctCreateEyeR.Release();
			base.ctCreateEyeR = null;
		}
		empty = "chara/mm_base.unity3d";
		empty2 = "cf_m_eye_create";
		base.ctCreateEyeR = new CustomTextureCreate(base.objRoot.transform);
		base.ctCreateEyeR.Initialize(empty, empty2, string.Empty, num, num);
		return true;
	}

	protected void ReleaseBaseCustomTextureClothes(bool main, int parts, bool createTex = true)
	{
		if (main)
		{
			for (int i = 0; i < 2; i++)
			{
				if (base.ctCreateClothes[parts, i] != null)
				{
					if (createTex)
					{
						base.ctCreateClothes[parts, i].Release();
					}
					else
					{
						base.ctCreateClothes[parts, i].ReleaseCreateMaterial();
					}
					base.ctCreateClothes[parts, i] = null;
				}
			}
			return;
		}
		for (int j = 0; j < 2; j++)
		{
			if (base.ctCreateClothesSub[parts, j] != null)
			{
				if (createTex)
				{
					base.ctCreateClothesSub[parts, j].Release();
				}
				else
				{
					base.ctCreateClothesSub[parts, j].ReleaseCreateMaterial();
				}
				base.ctCreateClothesSub[parts, j] = null;
			}
		}
	}

	protected bool InitBaseCustomTextureClothes(bool main, int parts)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		string empty5 = string.Empty;
		string empty6 = string.Empty;
		string empty7 = string.Empty;
		string empty8 = string.Empty;
		string empty9 = string.Empty;
		string empty10 = string.Empty;
		string empty11 = string.Empty;
		string empty12 = string.Empty;
		if (main && base.infoClothes == null)
		{
			return false;
		}
		if (!main && base.infoParts == null)
		{
			return false;
		}
		ListInfoBase listInfoBase = ((!main) ? base.infoParts[parts] : base.infoClothes[parts]);
		empty = listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest);
		empty2 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainTexAB);
		if ("0" == empty2)
		{
			empty2 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB);
		}
		empty3 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainTex);
		empty4 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest);
		empty5 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainTex02AB);
		if ("0" == empty5)
		{
			empty5 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB);
		}
		empty6 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainTex02);
		empty7 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest);
		empty8 = listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMaskAB);
		if ("0" == empty8)
		{
			empty8 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB);
		}
		empty9 = listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMaskTex);
		empty10 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainManifest);
		empty11 = listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMask02AB);
		if ("0" == empty11)
		{
			empty11 = listInfoBase.GetInfo(ChaListDefine.KeyType.MainAB);
		}
		empty12 = listInfoBase.GetInfo(ChaListDefine.KeyType.ColorMask02Tex);
		Texture2D texture2D = null;
		if ("0" == empty2 || "0" == empty3)
		{
			return false;
		}
		if (!base.hiPoly)
		{
			empty3 += "_low";
		}
		texture2D = CommonLib.LoadAsset<Texture2D>(empty2, empty3, false, empty);
		if (null == texture2D)
		{
			return false;
		}
		Texture2D texture2D2 = null;
		if ("0" == empty8 || "0" == empty9)
		{
			Resources.UnloadAsset(texture2D);
			return false;
		}
		if (!base.hiPoly)
		{
			empty9 += "_low";
		}
		texture2D2 = CommonLib.LoadAsset<Texture2D>(empty8, empty9, false, empty7);
		if (null == texture2D2)
		{
			Resources.UnloadAsset(texture2D);
			return false;
		}
		Texture2D texture2D3 = null;
		if ("0" != empty5 && "0" != empty6)
		{
			if (!base.hiPoly)
			{
				empty6 += "_low";
			}
			texture2D3 = CommonLib.LoadAsset<Texture2D>(empty5, empty6, false, empty4);
		}
		Texture2D texture2D4 = null;
		if ("0" != empty11 && "0" != empty12)
		{
			if (!base.hiPoly)
			{
				empty12 += "_low";
			}
			texture2D4 = CommonLib.LoadAsset<Texture2D>(empty11, empty12, false, empty10);
		}
		string createMatABName = "chara/mm_base.unity3d";
		string createMatName = "cf_m_clothesN_create";
		for (int i = 0; i < 2; i++)
		{
			CustomTextureCreate customTextureCreate = null;
			if (null != ((i != 0) ? texture2D3 : texture2D))
			{
				customTextureCreate = new CustomTextureCreate(base.objRoot.transform);
				int width = ((i != 0) ? texture2D3.width : texture2D.width);
				int height = ((i != 0) ? texture2D3.height : texture2D.height);
				customTextureCreate.Initialize(createMatABName, createMatName, string.Empty, width, height);
				customTextureCreate.SetMainTexture((i != 0) ? texture2D3 : texture2D);
				customTextureCreate.SetTexture(ChaShader._ColorMask, (i != 0) ? texture2D4 : texture2D2);
			}
			if (main)
			{
				base.ctCreateClothes[parts, i] = customTextureCreate;
			}
			else
			{
				base.ctCreateClothesSub[parts, i] = customTextureCreate;
			}
		}
		return true;
	}

	public bool LoadGagMaterial()
	{
		string assetBundleName = "chara/mm_base.unity3d";
		string[] array = new string[3] { "cf_m_gageye_00", "cf_m_gageye_01", "cf_m_gageye_02" };
		for (int i = 0; i < 3; i++)
		{
			base.matGag[i] = CommonLib.LoadAsset<Material>(assetBundleName, array[i], true, string.Empty);
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, string.Empty);
		return true;
	}

	public bool LoadAlphaMaskTexture(string assetBundleName, string assetName, byte type)
	{
		if ("0" == assetBundleName || "0" == assetName)
		{
			return false;
		}
		if (!base.hiPoly)
		{
			assetName += "_low";
		}
		Texture texture = CommonLib.LoadAsset<Texture>(assetBundleName, assetName, false, string.Empty);
		if (null == texture)
		{
			return false;
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, string.Empty);
		switch (type)
		{
		case 0:
			texBodyAlphaMask = texture;
			break;
		case 1:
			texBraAlphaMask = texture;
			break;
		case 2:
			texInnerAlphaMask = texture;
			break;
		}
		return true;
	}

	private void MozInit(params GameObject[] objMoz)
	{
		Texture[] array = new Texture[4];
		string assetBundleName = "chara/etc.unity3d";
		string mainManifestName = Singleton<Character>.Instance.mainManifestName;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = CommonLib.LoadAsset<Texture>(assetBundleName, "cf_t_mnpb_00_" + i.ToString("00"), false, mainManifestName);
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, mainManifestName);
		foreach (GameObject gameObject in objMoz)
		{
			if (!(null == gameObject))
			{
				ChangePtnAnime changePtnAnime = gameObject.AddComponent<ChangePtnAnime>();
				changePtnAnime.Init(array);
			}
		}
	}

	public bool InitializeExpression(bool _enable = true)
	{
		base.expression = base.objRoot.AddComponent<ExpressionBone>();
		base.expression.trfBoneRoot = base.objRoot.transform;
		base.expression.Initialize();
		base.expression.enable = _enable;
		return true;
	}

	public void LoadHitObject()
	{
		ReleaseHitObject();
		string assetBundleName = "chara/oo_base.unity3d";
		string assetName = "p_cf_body_00_hit";
		base.objHitBody = CommonLib.LoadAsset<GameObject>(assetBundleName, assetName, true, string.Empty);
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, string.Empty);
		if ((bool)base.objHitBody)
		{
			base.objHitBody.transform.SetParent(base.objTop.transform, false);
			aaWeightsBody.AssignedWeights(base.objHitBody, "cf_j_root");
			SkinnedCollisionHelper[] componentsInChildren = base.objHitBody.GetComponentsInChildren<SkinnedCollisionHelper>(true);
			SkinnedCollisionHelper[] array = componentsInChildren;
			foreach (SkinnedCollisionHelper skinnedCollisionHelper in array)
			{
				skinnedCollisionHelper.Init();
			}
			base.hitBodyCtrlCmp = base.objHitBody.GetComponent<ChaHitControlComponent>();
			if (base.sex == 0 && null != base.hitBodyCtrlCmp)
			{
				base.hitBodyCtrlCmp.ChangeBustBlendShapeValue(90f);
			}
		}
		if (base.sex == 0 || !(null != base.objHead))
		{
			return;
		}
		ListInfoComponent component = base.objHead.GetComponent<ListInfoComponent>();
		assetBundleName = component.data.dictInfo[30];
		assetName = component.data.dictInfo[31] + "_hit";
		base.objHitHead = CommonLib.LoadAsset<GameObject>(assetBundleName, assetName, true, string.Empty);
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, string.Empty);
		if ((bool)base.objHitHead)
		{
			base.objHitHead.transform.SetParent(base.objTop.transform, false);
			aaWeightsHead.AssignedWeights(base.objHitHead, "cf_J_N_FaceRoot");
			SkinnedCollisionHelper[] componentsInChildren2 = base.objHitHead.GetComponentsInChildren<SkinnedCollisionHelper>(true);
			SkinnedCollisionHelper[] array2 = componentsInChildren2;
			foreach (SkinnedCollisionHelper skinnedCollisionHelper2 in array2)
			{
				skinnedCollisionHelper2.Init();
			}
		}
	}

	public void ReleaseHitObject()
	{
		if ((bool)base.objHitBody)
		{
			SkinnedCollisionHelper[] componentsInChildren = base.objHitBody.GetComponentsInChildren<SkinnedCollisionHelper>(true);
			SkinnedCollisionHelper[] array = componentsInChildren;
			foreach (SkinnedCollisionHelper skinnedCollisionHelper in array)
			{
				skinnedCollisionHelper.Release();
			}
			SafeDestroy(base.objHitBody);
			base.objHitBody = null;
		}
		if ((bool)base.objHitHead)
		{
			SkinnedCollisionHelper[] componentsInChildren2 = base.objHitHead.GetComponentsInChildren<SkinnedCollisionHelper>(true);
			SkinnedCollisionHelper[] array2 = componentsInChildren2;
			foreach (SkinnedCollisionHelper skinnedCollisionHelper2 in array2)
			{
				skinnedCollisionHelper2.Release();
			}
			SafeDestroy(base.objHitHead);
			base.objHitHead = null;
		}
		base.hitBodyCtrlCmp = null;
	}
}
