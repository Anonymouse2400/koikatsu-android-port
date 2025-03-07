using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileHair
{
	[MessagePackObject(true)]
	public class PartsInfo
	{
		public int id { get; set; }

		public Color baseColor { get; set; }

		public Color startColor { get; set; }

		public Color endColor { get; set; }

		public float length { get; set; }

		public Vector3 pos { get; set; }

		public Vector3 rot { get; set; }

		public Vector3 scl { get; set; }

		public Color[] acsColor { get; set; }

		public Color outlineColor { get; set; }

		public PartsInfo()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			id = 0;
			baseColor = Color.white;
			startColor = Color.white;
			endColor = Color.white;
			length = 0f;
			pos = Vector3.zero;
			rot = Vector3.zero;
			scl = Vector3.one;
			acsColor = new Color[4];
			for (int i = 0; i < acsColor.Length; i++)
			{
				acsColor[i] = Color.white;
			}
			outlineColor = Color.black;
		}
	}

	public Version version { get; set; }

	public PartsInfo[] parts { get; set; }

	public int kind { get; set; }

	public int glossId { get; set; }

	public ChaFileHair()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileHairVersion;
		parts = new PartsInfo[Enum.GetValues(typeof(ChaFileDefine.HairKind)).Length];
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i] = new PartsInfo();
		}
		kind = 0;
		glossId = 0;
	}

	public void ComplementWithVersion()
	{
		if (version.CompareTo(new Version("0.0.3")) == -1)
		{
			PartsInfo[] array = new PartsInfo[Enum.GetValues(typeof(ChaFileDefine.HairKind)).Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parts[i];
			}
			parts = array;
		}
		if (version.CompareTo(new Version("0.0.2")) == -1)
		{
			for (int j = 0; j < parts.Length; j++)
			{
				Color[] array2 = new Color[4];
				Array.Copy(parts[j].acsColor, array2, parts[j].acsColor.Length);
				parts[j].acsColor = array2;
			}
		}
		if (version.CompareTo(new Version("0.0.4")) == -1)
		{
			PartsInfo[] array3 = parts;
			foreach (PartsInfo partsInfo in array3)
			{
				partsInfo.acsColor[0].a = 1f;
				partsInfo.acsColor[1].a = 1f;
				partsInfo.acsColor[2].a = 1f;
				partsInfo.outlineColor = Color.black;
			}
		}
		version = ChaFileDefine.ChaFileHairVersion;
	}
}
