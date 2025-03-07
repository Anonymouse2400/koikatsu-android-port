using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localize.Translate
{
	public class Info : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public int ID;

			public string Culture;

			public string BaseManifest;
		}

		public List<Param> param = new List<Param>();
	}
}
