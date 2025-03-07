using System;
using System.Collections.Generic;
using Illusion.Extensions;
using UnityEngine;
using WavInfoControl;

public class ChaInfo : ChaReference
{
	public enum DynamicBoneKind
	{
		BreastL = 0,
		BreastR = 1,
		HipL = 2,
		HipR = 3
	}

	private GameObject[] _objHair;

	private GameObject[] _objClothes;

	private GameObject[] _objParts;

	private GameObject[] _objAccessory;

	private GameObject[,] _objAcsMove;

	private ListInfoBase[] _infoHair;

	private ListInfoBase[] _infoClothes;

	private ListInfoBase[] _infoParts;

	private ListInfoBase[] _infoAccessory;

	public ChaFileControl chaFile { get; protected set; }

	public ChaFileCustom fileCustom
	{
		get
		{
			return chaFile.custom;
		}
	}

	public ChaFileBody fileBody
	{
		get
		{
			return chaFile.custom.body;
		}
	}

	public ChaFileFace fileFace
	{
		get
		{
			return chaFile.custom.face;
		}
	}

	public ChaFileHair fileHair
	{
		get
		{
			return chaFile.custom.hair;
		}
	}

	public ChaFileParameter fileParam
	{
		get
		{
			return chaFile.parameter;
		}
	}

	public ChaFileStatus fileStatus
	{
		get
		{
			return chaFile.status;
		}
	}

	public ChaListControl lstCtrl { get; protected set; }

	public WavInfoData wavInfoData { get; set; }

	public EyeLookController eyeLookCtrl { get; protected set; }

	public EyeLookMaterialControll[] eyeLookMatCtrl { get; protected set; }

	public NeckLookControllerVer2 neckLookCtrl { get; protected set; }

	public FaceBlendShape fbsCtrl { get; protected set; }

	public FBSCtrlEyebrow eyebrowCtrl { get; protected set; }

	public FBSCtrlEyes eyesCtrl { get; protected set; }

	public FBSCtrlMouth mouthCtrl { get; protected set; }

	public Dictionary<DynamicBoneKind, DynamicBone_Ver02> dictDynamicBoneBust { get; protected set; }

	public ChaForegroundComponent foregroundCtrl { get; protected set; }

	public ExpressionBone expression { get; protected set; }

	public ChaHitControlComponent hitBodyCtrlCmp { get; protected set; }

	public ChaCustomHairComponent[] cusHairCmp { get; protected set; }

	public ChaClothesComponent[] cusClothesCmp { get; protected set; }

	public ChaClothesComponent[] cusClothesSubCmp { get; protected set; }

	public ChaAccessoryComponent[] cusAcsCmp { get; protected set; }

	public int chaID { get; protected set; }

	public int loadNo { get; protected set; }

	public byte sex
	{
		get
		{
			return chaFile.parameter.sex;
		}
	}

	public bool hiPoly { get; protected set; }

	public bool hideMoz { get; set; }

	public bool loadEnd { get; protected set; }

	public bool visibleAll { get; set; }

	public bool updateShapeFace { get; set; }

	public bool updateShapeBody { get; set; }

	public bool updateShape
	{
		set
		{
			updateShapeFace = value;
			updateShapeBody = value;
		}
	}

	public bool resetDynamicBoneAll { get; set; }

	public bool reSetupDynamicBoneBust { get; set; }

	public bool updateBustSize { get; set; }

	public bool releaseCustomInputTexture { get; set; }

	public bool loadWithDefaultColorAndPtn { get; set; }

	public Renderer rendFace { get; protected set; }

	public Renderer[] rendEye { get; protected set; }

	public Renderer[] rendEyeW { get; protected set; }

	public Renderer rendEyelineUp { get; protected set; }

	public Renderer rendEyelineDown { get; protected set; }

	public Renderer rendEyebrow { get; protected set; }

	public Renderer rendNose { get; protected set; }

	public Renderer rendBody { get; protected set; }

	public Renderer[] rendBra { get; protected set; }

	public Renderer[] rendInner { get; protected set; }

	public Renderer rendSimpleBody { get; protected set; }

	public Renderer rendSimpleTongue { get; protected set; }

	public CustomTextureControl customTexCtrlFace { get; protected set; }

	public CustomTextureControl customTexCtrlBody { get; protected set; }

	public CustomTextureCreate ctCreateEyeL { get; protected set; }

	public CustomTextureCreate ctCreateEyeR { get; protected set; }

	public CustomTextureCreate ctCreateEyeW { get; protected set; }

	public Material customMatFace
	{
		get
		{
			return (customTexCtrlFace != null) ? customTexCtrlFace.matDraw : null;
		}
	}

	public Material customMatBody
	{
		get
		{
			return (customTexCtrlBody != null) ? customTexCtrlBody.matDraw : null;
		}
	}

	public CustomTextureCreate[,] ctCreateClothes { get; protected set; }

	public CustomTextureCreate[,] ctCreateClothesSub { get; protected set; }

	public Material[] matGag { get; set; }

	public GameObject objRoot { get; protected set; }

	public GameObject objTop { get; protected set; }

	public GameObject objAnim { get; protected set; }

	public GameObject objBodyBone { get; protected set; }

	public GameObject objBody { get; protected set; }

	public GameObject objHeadBone { get; protected set; }

	public GameObject objHead { get; protected set; }

	public GameObject[] objHair
	{
		get
		{
			return _objHair;
		}
		protected set
		{
			_objHair = value;
		}
	}

	public GameObject[] objClothes
	{
		get
		{
			return _objClothes;
		}
		protected set
		{
			_objClothes = value;
		}
	}

	public GameObject[] objParts
	{
		get
		{
			return _objParts;
		}
		protected set
		{
			_objParts = value;
		}
	}

	public GameObject[] objAccessory
	{
		get
		{
			return _objAccessory;
		}
		protected set
		{
			_objAccessory = value;
		}
	}

	public GameObject[,] objAcsMove
	{
		get
		{
			return _objAcsMove;
		}
		protected set
		{
			_objAcsMove = value;
		}
	}

	public List<GameObject>[] lstObjBraOpt { get; protected set; }

	public List<GameObject>[] lstObjShortsOpt { get; protected set; }

	public GameObject objTongueEx { get; protected set; }

	public GameObject objHitBody { get; protected set; }

	public GameObject objHitHead { get; protected set; }

	public Animator animBody { get; protected set; }

	public Animator animTongueEx { get; protected set; }

	public GameObject objEyesLookTargetP { get; protected set; }

	public GameObject objEyesLookTarget { get; protected set; }

	public GameObject objNeckLookTargetP { get; protected set; }

	public GameObject objNeckLookTarget { get; protected set; }

	public ListInfoBase infoHead { get; protected set; }

	public ListInfoBase[] infoHair
	{
		get
		{
			return _infoHair;
		}
		protected set
		{
			_infoHair = value;
		}
	}

	public ListInfoBase[] infoClothes
	{
		get
		{
			return _infoClothes;
		}
		protected set
		{
			_infoClothes = value;
		}
	}

	public ListInfoBase[] infoParts
	{
		get
		{
			return _infoParts;
		}
		protected set
		{
			_infoParts = value;
		}
	}

	public ListInfoBase[] infoAccessory
	{
		get
		{
			return _infoAccessory;
		}
		protected set
		{
			_infoAccessory = value;
		}
	}

	public int weakPoint
	{
		get
		{
			return fileParam.weakPoint;
		}
	}

	public ChaCustomHairComponent GetCustomHairComponent(int parts)
	{
		if (cusHairCmp == null)
		{
			return null;
		}
		if (parts >= cusHairCmp.Length)
		{
			return null;
		}
		ChaCustomHairComponent chaCustomHairComponent = cusHairCmp[parts];
		if (null == chaCustomHairComponent)
		{
			return null;
		}
		return chaCustomHairComponent;
	}

	public ChaClothesComponent GetCustomClothesComponent(int parts)
	{
		if (cusClothesCmp == null)
		{
			return null;
		}
		if (parts >= cusClothesCmp.Length)
		{
			return null;
		}
		ChaClothesComponent chaClothesComponent = cusClothesCmp[parts];
		if (null == chaClothesComponent)
		{
			return null;
		}
		return chaClothesComponent;
	}

	public ChaClothesComponent GetCustomClothesSubComponent(int parts)
	{
		if (cusClothesSubCmp == null)
		{
			return null;
		}
		if (parts >= cusClothesSubCmp.Length)
		{
			return null;
		}
		ChaClothesComponent chaClothesComponent = cusClothesSubCmp[parts];
		if (null == chaClothesComponent)
		{
			return null;
		}
		return chaClothesComponent;
	}

	public ChaAccessoryComponent GetAccessoryComponent(int parts)
	{
		if (cusAcsCmp == null)
		{
			return null;
		}
		if (parts >= cusAcsCmp.Length)
		{
			return null;
		}
		ChaAccessoryComponent chaAccessoryComponent = cusAcsCmp[parts];
		if (null == chaAccessoryComponent)
		{
			return null;
		}
		return chaAccessoryComponent;
	}

	protected void MemberInitializeAll()
	{
		chaFile = null;
		lstCtrl = null;
		wavInfoData = null;
		chaID = 0;
		loadNo = -1;
		hiPoly = false;
		hideMoz = false;
		releaseCustomInputTexture = true;
		loadWithDefaultColorAndPtn = false;
		objRoot = null;
		customTexCtrlBody = null;
		ctCreateEyeL = null;
		ctCreateEyeR = null;
		ctCreateEyeW = null;
		MemberInitializeObject();
	}

	protected void MemberInitializeObject()
	{
		eyeLookCtrl = null;
		eyeLookMatCtrl = new EyeLookMaterialControll[2];
		neckLookCtrl = null;
		fbsCtrl = null;
		eyebrowCtrl = null;
		eyesCtrl = null;
		mouthCtrl = null;
		dictDynamicBoneBust = null;
		foregroundCtrl = null;
		expression = null;
		hitBodyCtrlCmp = null;
		cusHairCmp = new ChaCustomHairComponent[Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length];
		cusClothesCmp = new ChaClothesComponent[Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length];
		cusClothesSubCmp = new ChaClothesComponent[Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length];
		cusAcsCmp = new ChaAccessoryComponent[20];
		customTexCtrlFace = null;
		ctCreateClothes = new CustomTextureCreate[Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length, 2];
		ctCreateClothesSub = new CustomTextureCreate[Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length, 2];
		matGag = new Material[3];
		for (int i = 0; i < 3; i++)
		{
			matGag[i] = null;
		}
		loadEnd = false;
		visibleAll = true;
		updateShapeFace = false;
		updateShapeBody = false;
		resetDynamicBoneAll = false;
		reSetupDynamicBoneBust = false;
		updateBustSize = false;
		rendFace = null;
		rendEye = new Renderer[2];
		rendEyeW = new Renderer[2];
		rendEyelineUp = null;
		rendEyelineDown = null;
		rendEyebrow = null;
		rendNose = null;
		rendBody = null;
		rendBra = new Renderer[2];
		rendInner = new Renderer[2];
		rendSimpleBody = null;
		rendSimpleTongue = null;
		objTop = null;
		objAnim = null;
		objBodyBone = null;
		objBody = null;
		objHeadBone = null;
		objHead = null;
		objHair = new GameObject[Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length];
		objClothes = new GameObject[Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length];
		objParts = new GameObject[Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length];
		objAccessory = new GameObject[20];
		objAcsMove = new GameObject[20, 2];
		lstObjBraOpt = new List<GameObject>[2];
		for (int j = 0; j < 2; j++)
		{
			lstObjBraOpt[j] = new List<GameObject>();
		}
		lstObjShortsOpt = new List<GameObject>[2];
		for (int k = 0; k < 2; k++)
		{
			lstObjShortsOpt[k] = new List<GameObject>();
		}
		objTongueEx = null;
		objHitHead = null;
		objHitBody = null;
		animBody = null;
		animTongueEx = null;
		objEyesLookTargetP = null;
		objEyesLookTarget = null;
		objNeckLookTargetP = null;
		objNeckLookTarget = null;
		infoHead = null;
		infoHair = new ListInfoBase[Enum.GetNames(typeof(ChaFileDefine.HairKind)).Length];
		infoClothes = new ListInfoBase[Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length];
		infoParts = new ListInfoBase[Enum.GetNames(typeof(ChaFileDefine.ClothesSubKind)).Length];
		infoAccessory = new ListInfoBase[20];
	}

	protected void ReleaseInfoAll()
	{
		ReleaseInfoObject(false);
		if (customTexCtrlBody != null)
		{
			customTexCtrlBody.Release();
		}
		if (ctCreateEyeW != null)
		{
			ctCreateEyeW.Release();
			ctCreateEyeW = null;
		}
		if (ctCreateEyeL != null)
		{
			ctCreateEyeL.Release();
			ctCreateEyeL = null;
		}
		if (ctCreateEyeR != null)
		{
			ctCreateEyeR.Release();
			ctCreateEyeR = null;
		}
		Resources.UnloadUnusedAssets();
	}

	protected void ReleaseInfoObject(bool init = true)
	{
		if (matGag != null)
		{
			for (int i = 0; i < 3; i++)
			{
				if ((bool)matGag[i])
				{
					UnityEngine.Object.Destroy(matGag[i]);
					matGag[i] = null;
				}
			}
		}
		if (customTexCtrlFace != null)
		{
			customTexCtrlFace.Release();
		}
		if (ctCreateClothes != null)
		{
			for (int j = 0; j < ctCreateClothes.GetLength(0); j++)
			{
				for (int k = 0; k < 2; k++)
				{
					if (ctCreateClothes[j, k] != null)
					{
						ctCreateClothes[j, k].Release();
						ctCreateClothes[j, k] = null;
					}
				}
			}
		}
		if (ctCreateClothesSub != null)
		{
			for (int l = 0; l < ctCreateClothesSub.GetLength(0); l++)
			{
				for (int m = 0; m < 2; m++)
				{
					if (ctCreateClothesSub[l, m] != null)
					{
						ctCreateClothesSub[l, m].Release();
						ctCreateClothesSub[l, m] = null;
					}
				}
			}
		}
		if (false)
		{
			if ((bool)objTop)
			{
				objTop.SetActiveIfDifferent(false);
				objTop.name = "Delete_Reserve";
				UnityEngine.Object.Destroy(objTop);
			}
		}
		else
		{
			SafeDestroy(objTop);
		}
		objTop = null;
		ReleaseRefAll();
		if (init)
		{
			MemberInitializeObject();
		}
	}

	public void SafeDestroy(GameObject obj)
	{
		if ((bool)obj)
		{
			obj.SetActiveIfDifferent(false);
			obj.transform.SetParent(null);
			obj.name = "Delete_Reserve";
			UnityEngine.Object.Destroy(obj);
		}
	}

	public ChaFileParameter.Awnser GetAwnser()
	{
		return fileParam.awnser;
	}

	public bool[] GetAwnserArry()
	{
		return GetAwnserArry(chaFile);
	}

	public static bool[] GetAwnserArry(ChaFileControl _chafile)
	{
		return new bool[9]
		{
			_chafile.parameter.awnser.animal,
			_chafile.parameter.awnser.eat,
			_chafile.parameter.awnser.cook,
			_chafile.parameter.awnser.exercise,
			_chafile.parameter.awnser.study,
			_chafile.parameter.awnser.fashionable,
			_chafile.parameter.awnser.blackCoffee,
			_chafile.parameter.awnser.spicy,
			_chafile.parameter.awnser.sweet
		};
	}

	public ChaFileParameter.Denial GetDenial()
	{
		return fileParam.denial;
	}

	public bool[] GetDenialArry()
	{
		return GetDenialArry(chaFile);
	}

	public static bool[] GetDenialArry(ChaFileControl _chafile)
	{
		return new bool[5]
		{
			_chafile.parameter.denial.kiss,
			_chafile.parameter.denial.aibu,
			_chafile.parameter.denial.anal,
			_chafile.parameter.denial.massage,
			_chafile.parameter.denial.notCondom
		};
	}

	public bool[] GetAttribute()
	{
		return GetAttribute(chaFile);
	}

	public static bool[] GetAttribute(ChaFileControl _chafile)
	{
		return new bool[18]
		{
			_chafile.parameter.attribute.hinnyo,
			_chafile.parameter.attribute.harapeko,
			_chafile.parameter.attribute.donkan,
			_chafile.parameter.attribute.choroi,
			_chafile.parameter.attribute.bitch,
			_chafile.parameter.attribute.mutturi,
			_chafile.parameter.attribute.dokusyo,
			_chafile.parameter.attribute.ongaku,
			_chafile.parameter.attribute.kappatu,
			_chafile.parameter.attribute.ukemi,
			_chafile.parameter.attribute.friendly,
			_chafile.parameter.attribute.kireizuki,
			_chafile.parameter.attribute.taida,
			_chafile.parameter.attribute.sinsyutu,
			_chafile.parameter.attribute.hitori,
			_chafile.parameter.attribute.undo,
			_chafile.parameter.attribute.majime,
			_chafile.parameter.attribute.likeGirls
		};
	}

	public bool GetAttribute(int no)
	{
		bool[] attribute = GetAttribute();
		if (attribute.Length <= no)
		{
			return false;
		}
		return attribute[no];
	}
}
