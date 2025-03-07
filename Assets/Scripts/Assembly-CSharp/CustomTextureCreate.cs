using UnityEngine;

public class CustomTextureCreate
{
	protected RenderTextureFormat rtFormat;

	protected RenderTexture createTex;

	protected Material matCreate;

	protected Texture texMain;

	public Transform trfParent;

	public int baseW { get; private set; }

	public int baseH { get; private set; }

	public bool CreateInitEnd { get; protected set; }

	public CustomTextureCreate(Transform _trfParent = null)
	{
		trfParent = _trfParent;
		CreateInitEnd = false;
	}

	public bool Initialize(string createMatABName, string createMatName, string createMatManifest, int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGB32)
	{
		CreateInitEnd = false;
		baseW = width;
		baseH = height;
		rtFormat = format;
		matCreate = CommonLib.LoadAsset<Material>(createMatABName, createMatName, true, string.Empty);
		if (null == matCreate)
		{
			return CreateInitEnd;
		}
		texMain = matCreate.GetTexture(ChaShader._MainTex);
		createTex = new RenderTexture(baseW, baseH, 0, rtFormat);
		createTex.useMipMap = true;
		CreateInitEnd = true;
		return CreateInitEnd;
	}

	public void Release()
	{
		Object.Destroy(createTex);
		createTex = null;
		Object.Destroy(matCreate);
		matCreate = null;
		CreateInitEnd = false;
	}

	public void ReleaseCreateMaterial()
	{
		Object.Destroy(matCreate);
		matCreate = null;
		CreateInitEnd = false;
	}

	public void SetMainTexture(Texture tex)
	{
		if (CreateInitEnd)
		{
			texMain = tex;
		}
	}

	public void SetTexture(string propertyName, Texture tex)
	{
		if (CreateInitEnd)
		{
			matCreate.SetTexture(propertyName, tex);
		}
	}

	public void SetTexture(int propertyID, Texture tex)
	{
		if (CreateInitEnd)
		{
			matCreate.SetTexture(propertyID, tex);
		}
	}

	public void SetColor(string propertyName, Color color)
	{
		if (CreateInitEnd)
		{
			matCreate.SetColor(propertyName, color);
		}
	}

	public void SetColor(int propertyID, Color color)
	{
		if (CreateInitEnd)
		{
			matCreate.SetColor(propertyID, color);
		}
	}

	public void SetOffsetAndTilingDirect(string propertyName, float tx, float ty, float ox, float oy)
	{
		if (CreateInitEnd)
		{
			matCreate.SetTextureOffset(propertyName, new Vector2(ox, oy));
			matCreate.SetTextureScale(propertyName, new Vector2(tx, ty));
		}
	}

	public void SetOffsetAndTilingDirect(int propertyID, float tx, float ty, float ox, float oy)
	{
		if (CreateInitEnd)
		{
			matCreate.SetTextureOffset(propertyID, new Vector2(ox, oy));
			matCreate.SetTextureScale(propertyID, new Vector2(tx, ty));
		}
	}

	public void SetOffsetAndTiling(string propertyName, int addW, int addH, float addPx, float addPy)
	{
		if (CreateInitEnd)
		{
			float num = (float)baseW / (float)addW;
			float num2 = (float)baseH / (float)addH;
			float ox = (0f - addPx / (float)baseW) * num;
			float oy = (0f - ((float)baseH - addPy - (float)addH) / (float)baseH) * num2;
			SetOffsetAndTilingDirect(propertyName, num, num2, ox, oy);
		}
	}

	public void SetOffsetAndTiling(int propertyID, int addW, int addH, float addPx, float addPy)
	{
		if (CreateInitEnd)
		{
			float num = (float)baseW / (float)addW;
			float num2 = (float)baseH / (float)addH;
			float ox = (0f - addPx / (float)baseW) * num;
			float oy = (0f - ((float)baseH - addPy - (float)addH) / (float)baseH) * num2;
			SetOffsetAndTilingDirect(propertyID, num, num2, ox, oy);
		}
	}

	public void SetFloat(string propertyName, float value)
	{
		if (CreateInitEnd)
		{
			matCreate.SetFloat(propertyName, value);
		}
	}

	public void SetFloat(int propertyID, float value)
	{
		if (CreateInitEnd)
		{
			matCreate.SetFloat(propertyID, value);
		}
	}

	public void SetVector4(string propertyName, Vector4 value)
	{
		if (CreateInitEnd)
		{
			matCreate.SetVector(propertyName, value);
		}
	}

	public void SetVector4(int propertyID, Vector4 value)
	{
		if (CreateInitEnd)
		{
			matCreate.SetVector(propertyID, value);
		}
	}

	public Texture RebuildTextureAndSetMaterial()
	{
		if (!CreateInitEnd)
		{
			return null;
		}
		bool sRGBWrite = GL.sRGBWrite;
		GL.sRGBWrite = true;
		Graphics.SetRenderTarget(createTex);
		GL.Clear(false, true, Color.clear);
		Graphics.SetRenderTarget(null);
		Graphics.Blit(texMain, createTex, matCreate, 0);
		GL.sRGBWrite = sRGBWrite;
		return createTex;
	}

	public Texture GetCreateTexture()
	{
		return createTex;
	}
}
