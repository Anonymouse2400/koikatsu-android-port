using System;
using System.Collections.Generic;
using UnityEngine;

public class VoiceInfo : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string Personality;

		public int No;

		public string FileName;

		public int Sort;
	}

	public List<Param> param = new List<Param>();
}
