using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettingData : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public int No;

		public float Volume;

		public float Pitch;

		public float Pan;

		public float Level3D;

		public int Priority;

		public bool PlayAwake;

		public bool Loop;

		public float DelayTime;

		public int Setting3DNo;
	}

	public List<Param> param = new List<Param>();
}
