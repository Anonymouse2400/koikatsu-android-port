using System;
using System.Collections.Generic;
using UnityEngine;

public class ADVFaceIconData : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string key;

		public string path;

		public string name;

		public float[] pos;

		public float[] rot;

		public float[] scal;

		public string anim;
	}

	public List<Param> param = new List<Param>();
}
