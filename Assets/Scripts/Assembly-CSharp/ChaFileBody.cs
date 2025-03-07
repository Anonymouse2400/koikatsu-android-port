using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileBody
{
	public Version version { get; set; }

	public float[] shapeValueBody { get; set; }

	public float bustSoftness { get; set; }

	public float bustWeight { get; set; }

	public int skinId { get; set; }

	public int detailId { get; set; }

	public float detailPower { get; set; }

	public Color skinMainColor { get; set; }

	public Color skinSubColor { get; set; }

	public float skinGlossPower { get; set; }

	public int[] paintId { get; set; }

	public Color[] paintColor { get; set; }

	public int[] paintLayoutId { get; set; }

	public Vector4[] paintLayout { get; set; }

	public int sunburnId { get; set; }

	public Color sunburnColor { get; set; }

	public int nipId { get; set; }

	public Color nipColor { get; set; }

	public float nipGlossPower { get; set; }

	public float areolaSize { get; set; }

	public int underhairId { get; set; }

	public Color underhairColor { get; set; }

	public Color nailColor { get; set; }

	public float nailGlossPower { get; set; }

	public bool drawAddLine { get; set; }

	public ChaFileBody()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileBodyVersion;
		shapeValueBody = new float[ChaFileDefine.cf_bodyshapename.Length];
		for (int i = 0; i < shapeValueBody.Length; i++)
		{
			shapeValueBody[i] = 0.5f;
		}
		bustSoftness = 0.5f;
		bustWeight = 0.5f;
		skinId = 0;
		detailId = 0;
		detailPower = 0f;
		skinMainColor = Color.white;
		skinSubColor = Color.white;
		skinGlossPower = 0f;
		paintId = new int[2];
		paintColor = new Color[2];
		paintLayoutId = new int[2];
		paintLayout = new Vector4[2];
		for (int j = 0; j < 2; j++)
		{
			paintLayout[j] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
		}
		sunburnId = 0;
		sunburnColor = Color.white;
		nipId = 0;
		nipColor = Color.white;
		nipGlossPower = 0.5f;
		areolaSize = 0.5f;
		underhairId = 0;
		underhairColor = Color.white;
		nailColor = Color.white;
		nailGlossPower = 0.5f;
		drawAddLine = true;
	}

	public void ComplementWithVersion()
	{
		if (version.CompareTo(new Version("0.0.1")) == -1)
		{
			for (int i = 0; i < paintLayout.Length; i++)
			{
				paintLayout[i] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
			}
			paintLayoutId = new int[2];
		}
		if (version.CompareTo(new Version("0.0.2")) == -1)
		{
			nipGlossPower = 0.5f;
		}
		version = ChaFileDefine.ChaFileBodyVersion;
	}
}
