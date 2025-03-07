using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Pixelation")]
[ExecuteInEditMode]
public class Pixelation : PostEffectsBase
{
	public Shader shader;

	public int scale;

	private Material material;

	public Pixelation()
	{
		scale = 16;
	}

	public override bool CheckResources()
	{
		material = CheckShaderAndCreateMaterial(shader, material);
		return CheckSupport();
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			ReportAutoDisable();
			Graphics.Blit(source, destination);
			return;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / scale, source.height / scale, 0);
		Graphics.Blit(source, temporary);
		temporary.filterMode = FilterMode.Point;
		material.SetTexture("_SmallTex", temporary);
		Graphics.Blit(source, destination, material, 0);
		RenderTexture.ReleaseTemporary(temporary);
	}

	public override void Main()
	{
	}
}
