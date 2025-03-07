using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class LightmapPrefab : MonoBehaviour
{
	[Serializable]
	private class LightmapParameter
	{
		public int lightmapIndex = -1;

		public Vector4 scaleOffset = Vector4.zero;

		public Renderer renderer;

		public void UpdateLightmapUVs()
		{
			if (!(renderer == null) && lightmapIndex >= 0)
			{
				renderer.lightmapScaleOffset = scaleOffset;
				renderer.lightmapIndex = lightmapIndex;
			}
		}
	}

	[SerializeField]
	private LightmapParameter[] lightmapParameters;

	private void Start()
	{
		Resetup();
	}

	[ContextMenu("Setup")]
	private void Setup()
	{
		Renderer[] array = (from v in GetComponentsInChildren<Renderer>()
			where v.enabled && v.lightmapIndex >= 0
			select v).ToArray();
		lightmapParameters = new LightmapParameter[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			lightmapParameters[i] = new LightmapParameter
			{
				lightmapIndex = renderer.lightmapIndex,
				scaleOffset = renderer.lightmapScaleOffset,
				renderer = renderer
			};
		}
	}

	[ContextMenu("Resetup")]
	public void Resetup()
	{
		if (lightmapParameters == null)
		{
			return;
		}
		int num = lightmapParameters.Length;
		for (int i = 0; i < num; i++)
		{
			if (lightmapParameters[i] != null)
			{
				lightmapParameters[i].UpdateLightmapUVs();
			}
		}
	}
}
