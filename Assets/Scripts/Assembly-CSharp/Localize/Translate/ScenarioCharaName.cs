using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localize.Translate
{
	public class ScenarioCharaName : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string Target;

			public string Bundle;

			public string Asset;

			public string Replace;

			public int priority
			{
				get
				{
					bool flag = !string.IsNullOrEmpty(Bundle);
					bool flag2 = !string.IsNullOrEmpty(Asset);
					if (flag && flag2)
					{
						return 0;
					}
					if (flag)
					{
						return 1;
					}
					if (flag2)
					{
						return 2;
					}
					return 3;
				}
			}
		}

		public List<Param> param = new List<Param>();
	}
}
