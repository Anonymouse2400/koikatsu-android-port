using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LightMapData", menuName = "LightMapData", order = 1)]
public class LightMapDataObject : ScriptableObject
{
	public LightProbes lightProbes;

	public LightmapsMode lightmapsMode;

	public Cubemap cubemap;

	public Texture2D[] light;

	public Texture2D[] dir;

	public Scene.FogData fog = new Scene.FogData();

	public static LightMapDataObject operator +(LightMapDataObject a, LightMapDataObject b)
	{
		List<Texture2D> list = a.light.ToList();
		List<Texture2D> list2 = a.dir.ToList();
		list.AddRange(b.light);
		list2.AddRange(b.dir);
		LightMapDataObject lightMapDataObject = new LightMapDataObject();
		lightMapDataObject.lightProbes = a.lightProbes;
		lightMapDataObject.cubemap = a.cubemap;
		lightMapDataObject.lightmapsMode = a.lightmapsMode;
		lightMapDataObject.light = list.ToArray();
		lightMapDataObject.dir = list2.ToArray();
		return lightMapDataObject;
	}

	public void Change()
	{
		LightmapData[] array = new LightmapData[light.Length];
		for (int i = 0; i < array.Length; i++)
		{
			LightmapData lightmapData = new LightmapData();
			lightmapData.lightmapDir = dir[i];
			lightmapData.lightmapColor = light[i];
			array[i] = lightmapData;
		}
		LightmapSettings.lightmaps = array;
		LightmapSettings.lightProbes = lightProbes;
		LightmapSettings.lightmapsMode = lightmapsMode;
		if (cubemap != null)
		{
			RenderSettings.customReflection = cubemap;
			RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
		}
		else
		{
			RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
		}
		if (fog != null)
		{
			fog.Change();
		}
	}
}
