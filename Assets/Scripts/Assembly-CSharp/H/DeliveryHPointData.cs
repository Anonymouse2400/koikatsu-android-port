using System;
using System.Collections.Generic;
using ActionGame;
using UnityEngine;

namespace H
{
	public class DeliveryHPointData : MonoBehaviour
	{
		public Action<HPointData, int> actionSelect;

		public Action actionBack;

		public ActionMap map;

		public int IDMap;

		public CameraControl_Ver2 cam;

		public List<int> lstCategory = new List<int>();

		public Vector3 initPos;

		public Quaternion initRot;

		public bool isFreeH;

		public bool isDebug;

		public SaveData.Heroine.HExperienceKind status;

		public List<HSceneProc.AnimationListInfo>[] lstAnimInfo = new List<HSceneProc.AnimationListInfo>[8];

		public HFlag flags;
	}
}
