using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame
{
	public class TalkLookBody : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string StateName;

			public int State;
		}

		public List<Param> param = new List<Param>();
	}
}
