using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using UnityEngine;

public class ChaHitControlComponent : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer smrHitBust;

	public GameObject[] objList;

	[Button("Initialize", "設定の初期化", new object[] { })]
	[Space]
	public int initialize;

	public void Initialize()
	{
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(base.transform);
		GameObject objectFromName = findAssist.GetObjectFromName("o_hit_mune");
		if (null != objectFromName)
		{
			smrHitBust = objectFromName.GetComponent<SkinnedMeshRenderer>();
		}
		objList = (from s in findAssist.dictObjName
			where s.Key.StartsWith("o_")
			select s into x
			select x.Value).ToArray();
	}

	private void Reset()
	{
		Initialize();
	}

	public void ChangeBustBlendShapeValue(float value)
	{
		if (!(null == smrHitBust) && smrHitBust.sharedMesh.blendShapeCount != 0)
		{
			smrHitBust.SetBlendShapeWeight(0, value);
		}
	}
}
