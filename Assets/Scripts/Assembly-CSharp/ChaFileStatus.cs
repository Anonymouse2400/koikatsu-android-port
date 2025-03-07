using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileStatus
{
	[IgnoreMember]
	public static readonly string BlockName = "Status";

	public Version version { get; set; }

	public int coordinateType { get; set; }

	public int backCoordinateType { get; set; }

	public byte[] clothesState { get; set; }

	public byte shoesType { get; set; }

	public bool[] showAccessory { get; set; }

	public int eyebrowPtn { get; set; }

	public float eyebrowOpenMax { get; set; }

	public int eyesPtn { get; set; }

	public float eyesOpenMax { get; set; }

	public bool eyesBlink { get; set; }

	public bool eyesYure { get; set; }

	public int mouthPtn { get; set; }

	public float mouthOpenMax { get; set; }

	public bool mouthFixed { get; set; }

	public bool mouthAdjustWidth { get; set; }

	public byte tongueState { get; set; }

	public int eyesLookPtn { get; set; }

	public int eyesTargetType { get; set; }

	public float eyesTargetAngle { get; set; }

	public float eyesTargetRange { get; set; }

	public float eyesTargetRate { get; set; }

	public int neckLookPtn { get; set; }

	public int neckTargetType { get; set; }

	public float neckTargetAngle { get; set; }

	public float neckTargetRange { get; set; }

	public float neckTargetRate { get; set; }

	public bool disableMouthShapeMask { get; set; }

	public bool[,] disableBustShapeMask { get; set; }

	public float nipStandRate { get; set; }

	public float skinTuyaRate { get; set; }

	public float hohoAkaRate { get; set; }

	public byte tearsLv { get; set; }

	public bool hideEyesHighlight { get; set; }

	public byte[] siruLv { get; set; }

	public bool visibleSon { get; set; }

	public bool visibleSonAlways { get; set; }

	public bool visibleHeadAlways { get; set; }

	public bool visibleBodyAlways { get; set; }

	public bool visibleSimple { get; set; }

	public bool visibleGomu { get; set; }

	public Color simpleColor { get; set; }

	public ChaFileStatus()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileStatusVersion;
		coordinateType = 0;
		backCoordinateType = 0;
		clothesState = new byte[Enum.GetValues(typeof(ChaFileDefine.ClothesKind)).Length];
		shoesType = 0;
		showAccessory = new bool[20];
		for (int i = 0; i < showAccessory.Length; i++)
		{
			showAccessory[i] = true;
		}
		eyebrowPtn = 0;
		eyebrowOpenMax = 1f;
		eyesPtn = 0;
		eyesOpenMax = 1f;
		eyesBlink = true;
		eyesYure = false;
		mouthPtn = 0;
		mouthOpenMax = 1f;
		mouthFixed = false;
		mouthAdjustWidth = true;
		tongueState = 0;
		eyesLookPtn = 0;
		eyesTargetType = 0;
		eyesTargetAngle = 0f;
		eyesTargetRange = 1f;
		eyesTargetRate = 1f;
		neckLookPtn = 0;
		neckTargetType = 0;
		neckTargetAngle = 0f;
		neckTargetRange = 1f;
		neckTargetRate = 1f;
		disableMouthShapeMask = false;
		disableBustShapeMask = new bool[2, ChaFileDefine.cf_BustShapeMaskID.Length];
		nipStandRate = 0f;
		skinTuyaRate = 0f;
		hohoAkaRate = 0f;
		tearsLv = 0;
		hideEyesHighlight = false;
		siruLv = new byte[Enum.GetValues(typeof(ChaFileDefine.SiruParts)).Length];
		visibleSon = false;
		visibleSonAlways = true;
		visibleHeadAlways = true;
		visibleBodyAlways = true;
		visibleSimple = false;
		visibleGomu = false;
		simpleColor = new Color(0.188f, 0.286f, 0.8f, 0.5f);
	}

	public void Copy(ChaFileStatus src)
	{
		coordinateType = src.coordinateType;
		backCoordinateType = src.backCoordinateType;
		clothesState = src.clothesState;
		shoesType = src.shoesType;
		showAccessory = src.showAccessory;
		eyebrowPtn = src.eyebrowPtn;
		eyebrowOpenMax = src.eyebrowOpenMax;
		eyesPtn = src.eyesPtn;
		eyesOpenMax = src.eyesOpenMax;
		eyesBlink = src.eyesBlink;
		eyesYure = src.eyesYure;
		mouthPtn = src.mouthPtn;
		mouthOpenMax = src.mouthOpenMax;
		mouthFixed = src.mouthFixed;
		mouthAdjustWidth = src.mouthAdjustWidth;
		tongueState = src.tongueState;
		eyesLookPtn = src.eyesLookPtn;
		eyesTargetType = src.eyesTargetType;
		eyesTargetAngle = src.eyesTargetAngle;
		eyesTargetRange = src.eyesTargetRange;
		eyesTargetRate = src.eyesTargetRate;
		neckLookPtn = src.neckLookPtn;
		neckTargetType = src.neckTargetType;
		neckTargetAngle = src.neckTargetAngle;
		neckTargetRange = src.neckTargetRange;
		neckTargetRate = src.neckTargetRate;
		disableMouthShapeMask = src.disableMouthShapeMask;
		disableBustShapeMask = src.disableBustShapeMask;
		nipStandRate = src.nipStandRate;
		skinTuyaRate = src.skinTuyaRate;
		hohoAkaRate = src.hohoAkaRate;
		tearsLv = src.tearsLv;
		hideEyesHighlight = src.hideEyesHighlight;
		siruLv = src.siruLv;
		visibleSon = src.visibleSon;
		visibleSonAlways = src.visibleSonAlways;
		visibleHeadAlways = src.visibleHeadAlways;
		visibleBodyAlways = src.visibleBodyAlways;
		visibleSimple = src.visibleSimple;
		visibleGomu = src.visibleGomu;
		simpleColor = src.simpleColor;
	}

	public void ComplementWithVersion()
	{
		version = ChaFileDefine.ChaFileStatusVersion;
	}
}
