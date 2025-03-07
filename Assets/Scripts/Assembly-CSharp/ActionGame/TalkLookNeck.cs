using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame
{
	public class TalkLookNeck : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string StateName;

			public bool isLook;
		}

		public List<Param> param = new List<Param>();
	}
}
