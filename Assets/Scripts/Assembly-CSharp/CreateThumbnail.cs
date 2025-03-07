using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

public class CreateThumbnail : BaseLoader
{
	public class FacePaintLayout
	{
		public int index = -1;

		public float x;

		public float y;

		public float r;

		public float s;
	}

	public class MoleLayout
	{
		public int index = -1;

		public float x;

		public float y;

		public float s;
	}

	public Dictionary<int, FacePaintLayout> dictFacePaintLayout;

	public Dictionary<int, MoleLayout> dictMoleLayout;

	public CameraControl_Ver2 camCtrl;

	public Camera camMain;

	public Camera camBack;

	public Camera camFront;

	public ChaControl chaCtrl;

	private MotionIK motionIK;

	private void Start()
	{
		chaCtrl = Singleton<Character>.Instance.CreateFemale(base.gameObject, 0);
		chaCtrl.LoadPreset(1, string.Empty);
		int num = Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length;
		for (int i = 0; i < num; i++)
		{
			chaCtrl.nowCoordinate.clothes.parts[i].id = 0;
		}
		chaCtrl.releaseCustomInputTexture = false;
		chaCtrl.Load();
		chaCtrl.hideMoz = true;
		chaCtrl.loadWithDefaultColorAndPtn = true;
		chaCtrl.ChangeEyesOpenMax(0.85f);
		chaCtrl.ChangeEyesBlinkFlag(false);
		motionIK = new MotionIK(chaCtrl);
		motionIK.SetPartners(motionIK);
		string assetBundleName = "custom/custompose.unity3d";
		string assetName = "custom_pose";
		chaCtrl.LoadAnimation(assetBundleName, assetName, string.Empty);
		if (motionIK != null)
		{
			TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
			if ((bool)textAsset)
			{
				motionIK.LoadData(textAsset);
			}
		}
		chaCtrl.AnimPlay("Fmannequin_00");
		ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
		Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.facepaint_layout);
		dictFacePaintLayout = categoryInfo.Select((KeyValuePair<int, ListInfoBase> dict) => new FacePaintLayout
		{
			index = dict.Value.Id,
			x = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosX),
			y = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosY),
			r = dict.Value.GetInfoFloat(ChaListDefine.KeyType.Rot),
			s = dict.Value.GetInfoFloat(ChaListDefine.KeyType.Scale)
		}).ToDictionary((FacePaintLayout v) => v.index, (FacePaintLayout v) => v);
		categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mole_layout);
		dictMoleLayout = categoryInfo.Select((KeyValuePair<int, ListInfoBase> dict) => new MoleLayout
		{
			index = dict.Value.Id,
			x = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosX),
			y = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosY),
			s = dict.Value.GetInfoFloat(ChaListDefine.KeyType.Scale)
		}).ToDictionary((MoleLayout v) => v.index, (MoleLayout v) => v);
	}

	private void Update()
	{
	}
}
