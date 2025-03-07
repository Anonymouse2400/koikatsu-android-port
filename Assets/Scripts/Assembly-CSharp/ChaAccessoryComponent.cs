using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using UnityEngine;

public class ChaAccessoryComponent : MonoBehaviour
{
	[Header("通常パーツ")]
	public Renderer[] rendNormal;

	public bool useColor01;

	public Color defColor01 = Color.white;

	public bool useColor02;

	public Color defColor02 = Color.white;

	public bool useColor03;

	public Color defColor03 = Color.white;

	[Header("半透明パーツ")]
	public Renderer[] rendAlpha;

	public Color defColor04 = Color.white;

	[Header("特殊髪情報")]
	public Renderer[] rendHair;

	[Header("アウトラインなし")]
	public bool noOutline;

	[Button("Initialize", "設定の初期化", new object[] { })]
	[Space]
	public int initialize;

	[Button("SetColor", "初期色を設定", new object[] { })]
	public int setcolor;

	public void Initialize()
	{
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(base.transform);
		rendNormal = (from s in findAssist.dictObjName
			where s.Key.StartsWith("on_")
			select s into x
			select GetRenderer(x.Value) into r
			where null != r
			select r).ToArray();
		rendAlpha = (from s in findAssist.dictObjName
			where s.Key.StartsWith("oa_")
			select s into x
			select GetRenderer(x.Value) into r
			where null != r
			select r).ToArray();
		rendHair = (from s in findAssist.dictObjName
			where s.Key.StartsWith("cf_hair")
			select s into x
			select GetRenderer(x.Value) into r
			where null != r
			select r).ToArray();
		SetColor();
	}

	public void SetColor()
	{
		if (rendNormal.Length != 0)
		{
			Material sharedMaterial = rendNormal[0].sharedMaterial;
			if (null != sharedMaterial)
			{
				defColor01 = sharedMaterial.GetColor("_Color");
				defColor02 = sharedMaterial.GetColor("_Color2");
				defColor03 = sharedMaterial.GetColor("_Color3");
			}
		}
		if (rendAlpha.Length != 0)
		{
			Material sharedMaterial2 = rendAlpha[0].sharedMaterial;
			if (null != sharedMaterial2)
			{
				defColor04 = sharedMaterial2.GetColor("_Color4");
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
	}
}
