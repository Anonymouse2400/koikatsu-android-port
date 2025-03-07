using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileFace
{
	[MessagePackObject(true)]
	public class PupilInfo
	{
		public int id { get; set; }

		public Color baseColor { get; set; }

		public Color subColor { get; set; }

		public int gradMaskId { get; set; }

		public float gradBlend { get; set; }

		public float gradOffsetY { get; set; }

		public float gradScale { get; set; }

		public PupilInfo()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			id = 0;
			baseColor = Color.black;
			subColor = Color.white;
			gradMaskId = 0;
			gradBlend = 0f;
			gradOffsetY = 0f;
			gradScale = 0f;
		}

		public void Copy(PupilInfo src)
		{
			id = src.id;
			baseColor = src.baseColor;
			subColor = src.subColor;
			gradMaskId = src.gradMaskId;
			gradBlend = src.gradBlend;
			gradOffsetY = src.gradOffsetY;
			gradScale = src.gradScale;
		}
	}

	public Version version { get; set; }

	public float[] shapeValueFace { get; set; }

	public int headId { get; set; }

	public int skinId { get; set; }

	public int detailId { get; set; }

	public float detailPower { get; set; }

	public float cheekGlossPower { get; set; }

	public int eyebrowId { get; set; }

	public Color eyebrowColor { get; set; }

	public int noseId { get; set; }

	public PupilInfo[] pupil { get; set; }

	public int hlUpId { get; set; }

	public Color hlUpColor { get; set; }

	public int hlDownId { get; set; }

	public Color hlDownColor { get; set; }

	public int whiteId { get; set; }

	public Color whiteBaseColor { get; set; }

	public Color whiteSubColor { get; set; }

	public float pupilWidth { get; set; }

	public float pupilHeight { get; set; }

	public float pupilX { get; set; }

	public float pupilY { get; set; }

	public float hlUpY { get; set; }

	public float hlDownY { get; set; }

	public int eyelineUpId { get; set; }

	public float eyelineUpWeight { get; set; }

	public int eyelineDownId { get; set; }

	public Color eyelineColor { get; set; }

	public int moleId { get; set; }

	public Color moleColor { get; set; }

	public Vector4 moleLayout { get; set; }

	public int lipLineId { get; set; }

	public Color lipLineColor { get; set; }

	public float lipGlossPower { get; set; }

	public bool doubleTooth { get; set; }

	public ChaFileMakeup baseMakeup { get; set; }

	public byte foregroundEyes { get; set; }

	public byte foregroundEyebrow { get; set; }

	[IgnoreMember]
	public bool isPupilSameSetting
	{
		get
		{
			if (pupil[0].id != pupil[1].id)
			{
				return false;
			}
			if (pupil[0].baseColor != pupil[1].baseColor)
			{
				return false;
			}
			if (pupil[0].subColor != pupil[1].subColor)
			{
				return false;
			}
			if (pupil[0].gradMaskId != pupil[1].gradMaskId)
			{
				return false;
			}
			if (pupil[0].gradBlend != pupil[1].gradBlend)
			{
				return false;
			}
			if (pupil[0].gradOffsetY != pupil[1].gradOffsetY)
			{
				return false;
			}
			if (pupil[0].gradScale != pupil[1].gradScale)
			{
				return false;
			}
			return true;
		}
	}

	public ChaFileFace()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileFaceVersion;
		shapeValueFace = new float[ChaFileDefine.cf_headshapename.Length];
		for (int i = 0; i < shapeValueFace.Length; i++)
		{
			shapeValueFace[i] = ChaFileDefine.cf_faceInitValue[i];
		}
		headId = 0;
		skinId = 0;
		detailId = 0;
		detailPower = 0f;
		cheekGlossPower = 0f;
		eyebrowId = 0;
		eyebrowColor = Color.white;
		noseId = 0;
		pupil = new PupilInfo[2];
		for (int j = 0; j < pupil.Length; j++)
		{
			pupil[j] = new PupilInfo();
		}
		hlUpId = 0;
		hlUpColor = Color.white;
		hlDownId = 0;
		hlDownColor = Color.white;
		whiteId = 0;
		whiteBaseColor = Color.white;
		whiteSubColor = Color.white;
		pupilWidth = 0.9f;
		pupilHeight = 0.9f;
		pupilX = 0.5f;
		pupilY = 0.5f;
		hlUpY = 0.5f;
		hlDownY = 0.5f;
		eyelineUpId = 0;
		eyelineUpWeight = 1f;
		eyelineDownId = 0;
		eyelineColor = Color.white;
		moleId = 0;
		moleColor = Color.white;
		moleLayout = new Vector4(0.5f, 0.5f, 0f, 0.5f);
		lipLineId = 0;
		lipLineColor = Color.white;
		lipGlossPower = 0f;
		doubleTooth = false;
		baseMakeup = new ChaFileMakeup();
		foregroundEyes = 0;
		foregroundEyebrow = 0;
	}

	public void ComplementWithVersion()
	{
		if (version.CompareTo(new Version("0.0.1")) == -1)
		{
			for (int i = 0; i < 2; i++)
			{
				pupil[i].gradOffsetY = Mathf.InverseLerp(-0.5f, 0.5f, pupil[i].gradOffsetY);
				pupil[i].gradScale = Mathf.InverseLerp(-1f, 1f, pupil[i].gradScale);
			}
			Vector4 zero = Vector4.zero;
			zero.x = Mathf.InverseLerp(-0.25f, 0.25f, moleLayout.x);
			zero.y = Mathf.InverseLerp(-0.3f, 0.3f, moleLayout.y);
			zero.w = Mathf.InverseLerp(0f, 0.7f, moleLayout.w);
			moleLayout = zero;
		}
		if (version.CompareTo(new Version("0.0.2")) == -1)
		{
			hlUpY = 0.5f;
			hlDownY = 0.5f;
		}
		version = ChaFileDefine.ChaFileFaceVersion;
	}
}
