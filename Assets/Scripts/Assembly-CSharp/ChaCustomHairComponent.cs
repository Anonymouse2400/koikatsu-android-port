using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using UnityEngine;

public class ChaCustomHairComponent : MonoBehaviour
{
	public Renderer[] rendHair;

	public Renderer[] rendAccessory;

	public Transform[] trfLength;

	public float[] baseLength;

	public float addLength;

	[Range(0f, 1f)]
	public float lengthRate;

	public Color[] acsDefColor;

	[Button("Initialize", "設定の初期化", new object[] { })]
	[Space]
	public int initialize;

	[Button("SetColor", "アクセサリの初期色を設定", new object[] { })]
	public int setcolor;

	public void Initialize()
	{
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(base.transform);
		rendHair = (from s in findAssist.dictObjName
			where s.Key.StartsWith("cf_hair")
			select s into x
			select GetRenderer(x.Value) into r
			where null != r
			select r).ToArray();
		rendAccessory = (from s in findAssist.dictObjName
			where s.Key.StartsWith("cf_acs")
			select s into x
			select GetRenderer(x.Value) into r
			where null != r
			select r).ToArray();
		SetColor();
		trfLength = (from s in findAssist.dictObjName
			where s.Key.EndsWith("_s")
			select s into x
			select x.Value.transform).ToArray();
		baseLength = new float[trfLength.Length];
		for (int i = 0; i < trfLength.Length; i++)
		{
			baseLength[i] = trfLength[i].localPosition.y;
		}
	}

	public void SetColor()
	{
		if (rendAccessory.Length == 0)
		{
			return;
		}
		Material sharedMaterial = rendAccessory[0].sharedMaterial;
		if (null != sharedMaterial)
		{
			acsDefColor = new Color[3];
			if (sharedMaterial.HasProperty("_Color"))
			{
				acsDefColor[0] = sharedMaterial.GetColor("_Color");
			}
			if (sharedMaterial.HasProperty("_Color2"))
			{
				acsDefColor[1] = sharedMaterial.GetColor("_Color2");
			}
			if (sharedMaterial.HasProperty("_Color3"))
			{
				acsDefColor[2] = sharedMaterial.GetColor("_Color3");
			}
		}
	}

	public Renderer GetRenderer(GameObject obj)
	{
		Renderer result = null;
		if (null == obj)
		{
			return result;
		}
		return obj.GetComponent<Renderer>();
	}

	private void Reset()
	{
		Initialize();
	}

	private void Update()
	{
		if (trfLength != null)
		{
			int num = trfLength.Length;
			for (int i = 0; i < num; i++)
			{
				float y = baseLength[i] + Mathf.Lerp(0f, addLength, lengthRate);
				trfLength[i].localPosition = new Vector3(trfLength[i].localPosition.x, y, trfLength[i].localPosition.z);
			}
		}
	}
}
