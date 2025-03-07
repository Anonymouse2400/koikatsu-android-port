using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame
{
	public class FixCharaInfo : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public int FixID;

			public int ClassIndex;
		}

		public List<Param> param = new List<Param>();
	}
}
