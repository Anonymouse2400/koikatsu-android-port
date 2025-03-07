using System.Collections.Generic;
using UnityEngine;

public class FindAssist
{
	public Dictionary<string, GameObject> dictObjName { get; private set; }

	public void Initialize(Transform trf)
	{
		dictObjName = new Dictionary<string, GameObject>();
		FindAll(trf);
	}

	private void FindAll(Transform trf)
	{
		if (!dictObjName.ContainsKey(trf.name))
		{
			dictObjName[trf.name] = trf.gameObject;
		}
		for (int i = 0; i < trf.childCount; i++)
		{
			FindAll(trf.GetChild(i));
		}
	}

	public GameObject GetObjectFromName(string objName)
	{
		if (dictObjName == null)
		{
			return null;
		}
		GameObject value = null;
		dictObjName.TryGetValue(objName, out value);
		return value;
	}
}
