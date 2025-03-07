using System;
using System.Collections.Generic;
using UnityEngine;

public class NickName : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public string Name;

		public int ID;

		public int Category;

		public string Bundle;

		public bool isSpecial
		{
			get
			{
				return Category == 0;
			}
		}
	}

	public List<Param> param = new List<Param>();
}
