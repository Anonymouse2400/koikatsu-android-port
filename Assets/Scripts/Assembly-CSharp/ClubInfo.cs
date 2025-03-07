using System;
using System.Collections.Generic;
using UnityEngine;

public class ClubInfo : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string Name;

		public int ID;

		public string Place;

		public bool isSports;
	}

	public List<Param> param = new List<Param>();
}
