using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class ShapeBodyInfoMale : ShapeInfoBase
{
	public enum DstBoneName
	{

	}

	public enum SrcBoneName
	{

	}

	public override void InitShapeInfo(string assetBundleAnmKey, string assetBundleCategory, string anmKeyInfoName, string cateInfoName, Transform trfObj)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		DstBoneName[] array = (DstBoneName[])Enum.GetValues(typeof(DstBoneName));
		DstBoneName[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			DstBoneName value = array2[i];
			dictionary[value.ToString()] = (int)value;
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		SrcBoneName[] array3 = (SrcBoneName[])Enum.GetValues(typeof(SrcBoneName));
		SrcBoneName[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			SrcBoneName value2 = array4[j];
			dictionary2[value2.ToString()] = (int)value2;
		}
		InitShapeInfoBase(assetBundleAnmKey, assetBundleCategory, anmKeyInfoName, cateInfoName, trfObj, dictionary, dictionary2, Singleton<Character>.Instance.AddLoadAssetBundle);
		base.InitEnd = true;
	}

	public override void Update()
	{
		if (base.InitEnd && dictSrc.Count != 0)
		{
		}
	}

	public override void UpdateAlways()
	{
	}
}
