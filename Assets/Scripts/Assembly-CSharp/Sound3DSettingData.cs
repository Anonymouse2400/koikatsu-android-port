using System;
using System.Collections.Generic;
using UnityEngine;

public class Sound3DSettingData : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public int No;

		public float DopplerLevel;

		public float Spread;

		public float MinDistance;

		public float MaxDistance;

		public int AudioRolloffMode;
	}

	public List<Param> param = new List<Param>();
}
