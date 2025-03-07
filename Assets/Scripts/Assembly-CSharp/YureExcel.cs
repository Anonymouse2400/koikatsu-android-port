using System;
using System.Collections.Generic;
using UnityEngine;

public class YureExcel : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string name;

		public bool Mune_L;

		public bool Mune_R;

		public bool Siri_L;

		public bool Siri_R;

		public bool UDPos_L;

		public bool UDPos_R;

		public bool LRRot_L;

		public bool LRRot_R;

		public bool LRPos_L;

		public bool LRPos_R;

		public bool UDRot_L;

		public bool UDRot_R;

		public bool Sharpness_L;

		public bool Sharpness_R;

		public bool Shape_L;

		public bool Shape_R;

		public bool Bulge_NipL;

		public bool Bulge_NipR;

		public bool Thickness_NipL;

		public bool Thickness_NipR;

		public bool Sharpness_NipL;

		public bool Sharpness_NipR;
	}

	public List<Param> param = new List<Param>();
}
