using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame
{
	public class CharaInfo : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string MotionAsset;

			public string MotionBundle;

			public int Personality;
		}

		public List<Param> param = new List<Param>();
	}
}
