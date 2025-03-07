using Manager;
using UnityEngine;

public class CustomTextureControl : CustomTextureCreate
{
	public bool DrawInitEnd;

	public Material matDraw { get; private set; }

	public CustomTextureControl(Transform trfParent = null)
		: base(trfParent)
	{
		matDraw = null;
		DrawInitEnd = false;
	}

	public bool Initialize(string drawMatABName, string drawMatName, string drawMatManifest, string createMatABName, string createMatName, string createMatManifest, int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGB32)
	{
		DrawInitEnd = false;
		if (!Initialize(createMatABName, createMatName, createMatManifest, width, height, format))
		{
			return DrawInitEnd;
		}
		matDraw = CommonLib.LoadAsset<Material>(drawMatABName, drawMatName, true, string.Empty);
		if (null == matDraw)
		{
			Object.Destroy(matCreate);
			matCreate = null;
			base.CreateInitEnd = false;
			return DrawInitEnd;
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(drawMatABName, drawMatManifest);
		DrawInitEnd = true;
		return DrawInitEnd;
	}

	public new void Release()
	{
		base.Release();
		Object.Destroy(matDraw);
		matDraw = null;
		DrawInitEnd = false;
	}

	public bool SetNewCreateTexture()
	{
		if (!DrawInitEnd)
		{
			return false;
		}
		RebuildTextureAndSetMaterial();
		matDraw.SetTexture(ChaShader._MainTex, createTex);
		return true;
	}
}
