using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame
{
	public class FixEventSchedule : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string Asset;

			public string Bundle;

			public bool isVisible;

			public string[] Cycle;

			public string Map;

			public string[] Week;

			public string LayerName;

			public string Coordinate;

			public int AfterDay;
		}

		public List<Param> param = new List<Param>();
	}
}
