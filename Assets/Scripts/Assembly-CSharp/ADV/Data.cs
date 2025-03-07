using System;
using System.Collections.Generic;

namespace ADV
{
	[Serializable]
	public class Data : OpenData.Data
	{
		public string bundleName = string.Empty;

		public string assetName = string.Empty;

		public float fadeInTime;

		public List<SaveData.Heroine> heroineList;

		public List<Program.Transfer> transferList;

		public bool isParentChara;

		public List<CharaData.MotionReserver> motionReserverList { get; set; }
	}
}
