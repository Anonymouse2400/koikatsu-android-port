using System;
using System.Collections.Generic;
using UnityEngine;

public class NormalData : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string ObjectName = string.Empty;

		public List<Vector3> NormalMin = new List<Vector3>();

		public List<Vector3> NormalMax = new List<Vector3>();
	}

	public List<Param> data = new List<Param>();
}
