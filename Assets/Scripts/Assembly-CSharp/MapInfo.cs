using System;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string MapName;

		public int No;

		public string AssetBundleName;

		public string AssetName;

		public bool isGate;

		public bool is2D;

		public bool isWarning;

		public int State;

		public int LookFor;

		public bool isOutdoors;

		public bool isFreeH;

		public bool isSpH;

		public string ThumbnailBundle;

		public string ThumbnailAsset;
	}

	public List<Param> param = new List<Param>();
}
