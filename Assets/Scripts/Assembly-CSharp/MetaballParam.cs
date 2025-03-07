using System;
using System.Collections.Generic;
using UnityEngine;

public class MetaballParam : ScriptableObject
{
	[Serializable]
	public class Param
	{
		public int ID;

		public bool Enable;

		public int Dir_parent_sex;

		public string ObjDirParent;

		public int Stop_parent_sex;

		public string ObjStopParent;

		public string BulletManifesto;

		public string BulletAssetPath;

		public string ObjBullet;

		public string CondomManifesto;

		public string CondomAssetPath;

		public string ObjCondom;

		public float[] Interval;

		public float S_Speed_Min;

		public float S_Speed_Max;

		public float S_RandomDirX;

		public float S_RandomDirY;

		public float M_Speed_Min;

		public float M_Speed_Max;

		public float M_RandomDirX;

		public float M_RandomDirY;

		public float L_Speed_Min;

		public float L_Speed_Max;

		public float L_RandomDirX;

		public float L_RandomDirY;

		public string[] ShootCondition;
	}

	public List<Param> param = new List<Param>();
}
