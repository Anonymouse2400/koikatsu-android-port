using System.Collections.Generic;
using UnityEngine;

namespace FreeH
{
	public class FreeHBackData : MonoBehaviour
	{
		public SaveData.Heroine heroine;

		public SaveData.Heroine partner;

		public SaveData.Player player;

		public int map;

		public int timeZone;

		public int stageH1;

		public int stageH2;

		public int statusH;

		public bool discovery;

		public List<int> categorys = new List<int>();
	}
}
