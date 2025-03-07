using System;
using System.Collections.Generic;
using ActionGame.Chara;
using Illusion.Extensions;
using UnityEngine;

namespace ActionGame
{
	public class NonActiveWaitPointInfo : ScriptableObject
	{
		[Serializable]
		public class Param
		{
			public string Name;

			public int MapNo;

			public Vector3 pos;

			public float angY;

			public bool isReserved
			{
				get
				{
					return npc != null;
				}
			}

			public NPC npc { get; private set; }

			public List<string> RowData
			{
				get
				{
					List<string> list = new List<string>();
					list.Add(Name);
					list.Add(MapNo.ToString());
					list.Add(pos.Convert());
					list.Add(angY.ToString());
					return list;
				}
			}

			public void Set(NPC npc)
			{
				this.npc = npc;
			}

			public static Param Convert(string[] row)
			{
				if (row.IsNullOrEmpty())
				{
					return null;
				}
				Param param = new Param();
				int num = 0;
				row.SafeProc(num++, delegate(string s)
				{
					param.Name = s;
				});
				row.SafeProc(num++, delegate(string s)
				{
					int result;
					if (int.TryParse(s, out result))
					{
						param.MapNo = result;
					}
				});
				row.SafeProc(num++, delegate(string s)
				{
					param.pos = s.GetVector3();
				});
				row.SafeProc(num++, delegate(string s)
				{
					float result2;
					if (float.TryParse(s, out result2))
					{
						param.angY = result2;
					}
				});
				return param;
			}

			public static Param Convert(Transform t, int mapNo)
			{
				Param param = new Param();
				param.Name = t.name;
				param.MapNo = mapNo;
				param.pos = t.localPosition;
				param.angY = t.localEulerAngles.y;
				return param;
			}
		}

		public List<Param> param = new List<Param>();
	}
}
