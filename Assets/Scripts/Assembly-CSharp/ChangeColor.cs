using UnityEngine;

public class ChangeColor
{
	public static void SetColor(string propertyName, Color color, int idx, params Renderer[] arrRend)
	{
		int propertyID = Shader.PropertyToID(propertyName);
		SetColor(propertyID, color, idx, arrRend);
	}

	public static void SetColor(int propertyID, Color color, int idx, params Renderer[] arrRend)
	{
		if (arrRend == null || arrRend.Length == 0)
		{
			return;
		}
		foreach (Renderer renderer in arrRend)
		{
			if (idx < renderer.materials.Length)
			{
				Material material = renderer.materials[idx];
				if (null != material)
				{
					material.SetColor(propertyID, color);
				}
			}
		}
	}

	public static void SetColor(string propertyName, Color color, params Material[] mat)
	{
		int propertyID = Shader.PropertyToID(propertyName);
		SetColor(propertyID, color, mat);
	}

	public static void SetColor(int propertyID, Color color, params Material[] mat)
	{
		if (mat != null && mat.Length != 0)
		{
			foreach (Material material in mat)
			{
				material.SetColor(propertyID, color);
			}
		}
	}
}
