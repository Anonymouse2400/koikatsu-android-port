using System;
using System.Collections.Generic;
using ActionGame.MapObject;
using UnityEngine;

namespace ActionGame.H
{
	public class OpenHData : OpenData
	{
		[Serializable]
		public new class Data : OpenData.Data
		{
			public List<int> peepCategory = new List<int>();

			public Kind kind;

			public int state;

			public bool isInvited;

			public bool isFound;

			public bool isEasyPlace;

			public bool isLoadPeepLoom;

			public int appoint = -1;

			public List<int> lstFemaleCoordinateType = new List<int> { 0, 0 };

			public int maleCoordinateType;

			public Vector3 offsetPos = Vector3.zero;

			public Vector3 offsetAngle = Vector3.zero;

			public int hScenePlayCount;

			public bool isFirstPlayMasturbation;

			public bool isKokanForceInsert;

			public List<List<int>> clothStates = new List<List<int>>();

			public List<List<bool>> accessoryStates = new List<List<bool>>();

			public bool isFreeH;

			public int mapNoFreeH = -1;

			public int timezoneFreeH = -1;

			public int stageFreeH1 = -1;

			public int stageFreeH2 = -1;

			public int statusFreeH = -1;

			public SaveData.Player player = new SaveData.Player();

			public List<SaveData.Heroine> lstFemale = new List<SaveData.Heroine>();

			public SaveData.Heroine newHeroione;

			private GameObject _map;

			public GameObject map
			{
				get
				{
					return _map;
				}
				set
				{
					_map = value;
				}
			}
		}

		private Data _data;

		public override OpenData.Data data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value as Data;
			}
		}

		protected override void Start()
		{
			assetBundleName = string.Empty;
			levelName = "H";
			base.Start();
		}
	}
}
