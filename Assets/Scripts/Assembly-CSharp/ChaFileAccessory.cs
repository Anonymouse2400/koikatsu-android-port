using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileAccessory
{
	[MessagePackObject(true)]
	public class PartsInfo
	{
		public int type { get; set; }

		public int id { get; set; }

		public string parentKey { get; set; }

		public Vector3[,] addMove { get; set; }

		public Color[] color { get; set; }

		public int hideCategory { get; set; }

		[IgnoreMember]
		public bool partsOfHead { get; set; }

		public PartsInfo()
		{
			MemberInit();
		}

		public void MemberInit()
		{
			type = 120;
			id = 0;
			parentKey = string.Empty;
			addMove = new Vector3[2, 3];
			for (int i = 0; i < 2; i++)
			{
				addMove[i, 0] = Vector3.zero;
				addMove[i, 1] = Vector3.zero;
				addMove[i, 2] = Vector3.one;
			}
			color = new Color[4];
			for (int j = 0; j < color.Length; j++)
			{
				color[j] = Color.white;
			}
			hideCategory = 0;
			partsOfHead = false;
		}
	}

	public Version version { get; set; }

	public PartsInfo[] parts { get; set; }

	public ChaFileAccessory()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileAccessoryVersion;
		parts = new PartsInfo[20];
		for (int i = 0; i < parts.Length; i++)
		{
			parts[i] = new PartsInfo();
		}
	}

	public void ComplementWithVersion()
	{
		if (version.CompareTo(new Version("0.0.1")) == -1)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				Color[] array = new Color[4];
				Array.Copy(parts[i].color, array, parts[i].color.Length);
				parts[i].color = array;
			}
		}
		version = ChaFileDefine.ChaFileAccessoryVersion;
	}
}
