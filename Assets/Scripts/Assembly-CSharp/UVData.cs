using System;
using System.Collections.Generic;
using UnityEngine;

public class UVData : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string ObjectName = string.Empty;

		public List<Vector2> UV = new List<Vector2>();
	}

	public List<Param> data = new List<Param>();

	public int[] rangeIndex;
}
