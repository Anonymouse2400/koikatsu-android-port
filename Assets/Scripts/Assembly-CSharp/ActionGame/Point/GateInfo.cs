using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using UnityEngine;

namespace ActionGame.Point
{
	public class GateInfo
	{
		public int ID;

		public int mapNo;

		public int linkID;

		public Vector3 pos;

		public Vector3 ang;

		public string Name;

		public Vector3 playerPos;

		public Vector3 playerAng;

		public Vector3 playerHitPos;

		public Vector3 playerHitSize = Vector3.one;

		public Vector3 heroineHitPos;

		public Vector3 heroineHitSize = Vector3.one;

		public Vector3 iconPos;

		public Vector3 iconHitPos;

		public Vector3 iconHitSize = Vector3.one;

		public int moveType;

		public int seType = -1;

		public Dictionary<int, Vector3[]> calc { get; set; }

		public GateInfo(Gate gate)
		{
			ID = gate.ID;
			mapNo = gate.mapNo;
			linkID = gate.linkID;
			Transform transform = gate.transform;
			pos = transform.position;
			ang = transform.eulerAngles;
			Name = gate.name;
			playerPos = gate.playerTrans.localPosition;
			playerAng = gate.playerTrans.localEulerAngles;
			BoxCollider playerHitBox = gate.playerHitBox;
			playerHitPos = playerHitBox.center;
			playerHitSize = playerHitBox.size;
			playerHitBox = gate.heroineHitBox;
			heroineHitPos = playerHitBox.center;
			heroineHitSize = playerHitBox.size;
			iconPos = gate.canvas.GetComponent<RectTransform>().anchoredPosition3D;
			playerHitBox = gate.iconHitBox;
			iconHitPos = playerHitBox.center;
			iconHitSize = playerHitBox.size;
			moveType = gate.moveType;
			seType = gate.seType;
			calc = new Dictionary<int, Vector3[]>();
		}

		public GateInfo(List<string> list)
		{
			int num = 0;
			ID = int.Parse(list[num++]);
			mapNo = int.Parse(list[num++]);
			linkID = int.Parse(list[num++]);
			pos = list[num++].GetVector3();
			ang = list[num++].GetVector3();
			Name = list[num++];
			playerPos = list[num++].GetVector3();
			playerAng = list[num++].GetVector3();
			playerHitPos = list[num++].GetVector3();
			playerHitSize = list[num++].GetVector3();
			heroineHitPos = list[num++].GetVector3();
			heroineHitSize = list[num++].GetVector3();
			iconPos = list[num++].GetVector3();
			iconHitPos = list[num++].GetVector3();
			iconHitSize = list[num++].GetVector3();
			moveType = int.Parse(list[num++]);
			seType = int.Parse(list[num++]);
			calc = new Dictionary<int, Vector3[]>();
		}

		public static List<string> Convert(Gate gate)
		{
			return new GateInfo(gate).Convert();
		}

		public static void Convert(List<string> list, ref Gate gate)
		{
			GateInfo gateInfo = new GateInfo(list);
			gate.ID = gateInfo.ID;
			gate.mapNo = gateInfo.mapNo;
			gate.linkID = gateInfo.linkID;
			Transform transform = gate.transform;
			transform.position = gateInfo.pos;
			transform.eulerAngles = gateInfo.ang;
			gate.name = gateInfo.Name;
			gate.playerTrans.localPosition = gateInfo.playerPos;
			gate.playerTrans.localEulerAngles = gateInfo.playerAng;
			BoxCollider playerHitBox = gate.playerHitBox;
			playerHitBox.center = gateInfo.playerHitPos;
			playerHitBox.size = gateInfo.playerHitSize;
			playerHitBox = gate.heroineHitBox;
			playerHitBox.center = gateInfo.heroineHitPos;
			playerHitBox.size = gateInfo.heroineHitSize;
			gate.canvas.GetComponent<RectTransform>().anchoredPosition3D = gateInfo.iconPos;
			playerHitBox = gate.iconHitBox;
			playerHitBox.center = gateInfo.iconHitPos;
			playerHitBox.size = gateInfo.iconHitSize;
			gate.moveType = gateInfo.moveType;
			gate.seType = gateInfo.seType;
			gateInfo.calc = new Dictionary<int, Vector3[]>();
		}

		public static List<GateInfo> Create(List<ExcelData.Param> list)
		{
			List<GateInfo> list2 = new List<GateInfo>();
			GateInfo gateInfo = null;
			foreach (ExcelData.Param item in list)
			{
				if (item.list[0] == "@")
				{
					gateInfo = null;
				}
				else if (gateInfo == null)
				{
					GateInfo gateInfo2 = new GateInfo(item.list);
					list2.Add(gateInfo2);
					gateInfo = gateInfo2;
				}
				else
				{
					gateInfo.calc[int.Parse(item.list[0])] = item.list.Skip(1).Select(Illusion.Extensions.StringExtensions.GetVector3).ToArray();
				}
			}
			return list2;
		}

		public static List<List<string>> Create(List<GateInfo> list)
		{
			List<List<string>> ret = new List<List<string>>();
			list.Sort((GateInfo a, GateInfo b) => a.ID.CompareTo(b.ID));
			list.ForEach(delegate(GateInfo p)
			{
				ret.Add(new List<string> { "@" });
				ret.Add(p.Convert());
				foreach (KeyValuePair<int, Vector3[]> item in p.calc.OrderBy((KeyValuePair<int, Vector3[]> v) => v.Key))
				{
					List<string> list2 = new List<string> { item.Key.ToString() };
					Vector3[] value = item.Value;
					foreach (Vector3 self in value)
					{
						list2.Add(self.Convert());
					}
					ret.Add(list2);
				}
			});
			return ret;
		}

		public List<string> Convert()
		{
			List<string> list = new List<string>();
			list.Add(ID.ToString());
			list.Add(mapNo.ToString());
			list.Add(linkID.ToString());
			list.Add(pos.Convert());
			list.Add(ang.Convert());
			list.Add(Name);
			list.Add(playerPos.Convert());
			list.Add(playerAng.Convert());
			list.Add(playerHitPos.Convert());
			list.Add(playerHitSize.Convert());
			list.Add(heroineHitPos.Convert());
			list.Add(heroineHitSize.Convert());
			list.Add(iconPos.Convert());
			list.Add(iconHitPos.Convert());
			list.Add(iconHitSize.Convert());
			list.Add(moveType.ToString());
			list.Add(seType.ToString());
			return list;
		}
	}
}
