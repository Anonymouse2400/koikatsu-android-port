using Illusion.CustomAttributes;
using UnityEngine;

public class ChaClothesComponent : MonoBehaviour
{
	[Header("通常パーツ")]
	public Renderer[] rendNormal01;

	public Renderer[] rendNormal02;

	public bool useColorN01;

	public bool useColorN02;

	public bool useColorN03;

	[Header("半透明パーツ")]
	public Renderer[] rendAlpha01;

	public Renderer[] rendAlpha02;

	public bool useColorA01;

	public bool useColorA02;

	public bool useColorA03;

	[Header("装飾パーツ ※制服")]
	public Renderer rendAccessory;

	[Header("エンブレムパーツ")]
	public Renderer rendEmblem01;

	public Renderer rendEmblem02;

	[Header("基本初期カラー")]
	public Color defMainColor01 = Color.white;

	public Color defMainColor02 = Color.white;

	public Color defMainColor03 = Color.white;

	[Header("装飾初期カラー")]
	public Color defAccessoryColor = Color.white;

	[Header("オプションパーツ")]
	public GameObject[] objOpt01;

	public GameObject[] objOpt02;

	public bool useOpt01;

	public bool useOpt02;

	[Space]
	[Button("Initialize", "設定の初期化", new object[] { })]
	public int initialize;

	[Button("SetColor", "初期色を設定", new object[] { })]
	public int setcolor;

	public void Initialize()
	{
		SetColor();
	}

	public void SetColor()
	{
		if (rendNormal01 != null && rendNormal01.Length != 0)
		{
			Material sharedMaterial = rendNormal01[0].sharedMaterial;
			if (null != sharedMaterial)
			{
				if (useColorN01)
				{
					defMainColor01 = sharedMaterial.GetColor("_Color");
				}
				if (useColorN02)
				{
					defMainColor02 = sharedMaterial.GetColor("_Color2");
				}
				if (useColorN03)
				{
					defMainColor03 = sharedMaterial.GetColor("_Color3");
				}
			}
		}
		if (rendAlpha01 != null && rendAlpha01.Length != 0)
		{
			Material sharedMaterial2 = rendAlpha01[0].sharedMaterial;
			if (null != sharedMaterial2)
			{
				if (useColorA01)
				{
					defMainColor01 = sharedMaterial2.GetColor("_Color");
				}
				if (useColorA02)
				{
					defMainColor02 = sharedMaterial2.GetColor("_Color2");
				}
				if (useColorA03)
				{
					defMainColor03 = sharedMaterial2.GetColor("_Color3");
				}
			}
		}
		if (null != rendAccessory)
		{
			Material sharedMaterial3 = rendAccessory.sharedMaterial;
			if (null != sharedMaterial3)
			{
				defAccessoryColor = sharedMaterial3.GetColor("_Color4");
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
