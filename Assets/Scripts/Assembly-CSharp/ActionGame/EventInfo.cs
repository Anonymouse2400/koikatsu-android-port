using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame
{
	public class EventInfo : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string Name;

			public int ID;

			public string Bundle;

			public string Asset;
		}

		public List<Param> param = new List<Param>();
	}
}
