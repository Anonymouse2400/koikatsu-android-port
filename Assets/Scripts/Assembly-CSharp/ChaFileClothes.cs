using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileClothes
{
	[MessagePackObject(true)]
	public class PartsInfo
	{
		[MessagePackObject(true)]
		public class ColorInfo
		{
			public Color baseColor { get; set; }

			public int pattern { get; set; }

			public Vector2 tiling { get; set; }

			public Color patternColor { get; set; }

			public ColorInfo()
			{
				MemberInit();
			}

			public void MemberInit()
			{
				baseColor = Color.white;
				pattern = 0;
				tiling = Vector2.zero;
				patternColor = Color.white;
			}
		}

		public int id { get; set; }

		public ColorInfo[] colorInfo { get; set; }

		public int emblemeId { get; set; }

		public PartsInfo()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			id = 0;
			colorInfo = new ColorInfo[4];
			for (int i = 0; i < colorInfo.Length; i++)
			{
				colorInfo[i] = new ColorInfo();
			}
			emblemeId = 0;
		}
	}

	public Version version { get; set; }

	public PartsInfo[] parts { get; set; }

	public int[] subPartsId { get; set; }

	public bool[] hideBraOpt { get; set; }

	public bool[] hideShortsOpt { get; set; }

	public ChaFileClothes()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileClothesVersion;
		parts = new PartsInfo[Enum.GetValues(typeof(ChaFileDefine.ClothesKind)).Length];
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i] = new PartsInfo();
		}
		subPartsId = new int[Enum.GetValues(typeof(ChaFileDefine.ClothesSubKind)).Length];
		hideBraOpt = new bool[2];
		hideShortsOpt = new bool[2];
	}

	public void ComplementWithVersion()
	{
		if (version.CompareTo(new Version("0.0.1")) == -1)
		{
			hideBraOpt = new bool[2];
			hideShortsOpt = new bool[2];
		}
		version = ChaFileDefine.ChaFileClothesVersion;
	}
}
