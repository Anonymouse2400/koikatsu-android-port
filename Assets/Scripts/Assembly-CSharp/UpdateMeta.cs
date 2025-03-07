using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class UpdateMeta : MonoBehaviour
{
	[ListDrawerSettings(ShowIndexLabels = true)]
	public List<MetaballShoot> lstShoot = new List<MetaballShoot>();

	[Tooltip("StaticMetaballSeedScript")]
	public StaticMetaballSeed metaseed;

	[Tooltip("StaticMesh")]
	public GameObject objStaticMesh;

	[SerializeField]
	private bool isCreate;

	private void LateUpdate()
	{
		isCreate = false;
		for (int i = 0; i < lstShoot.Count; i++)
		{
			if (lstShoot[i].IsCreate())
			{
				isCreate = true;
				break;
			}
		}
		if ((bool)metaseed && isCreate)
		{
			metaseed.CreateMesh();
		}
	}

	public void ConstMetaMesh()
	{
		bool flag = false;
		for (int i = 0; i < lstShoot.Count; i++)
		{
			if (lstShoot[i].isConstMetaMesh)
			{
				flag = true;
				break;
			}
		}
		if (flag && !isCreate)
		{
			for (int j = 0; j < lstShoot.Count; j++)
			{
				if (lstShoot[j].isEnable && lstShoot[j].isConstMetaMesh)
				{
					if (objStaticMesh.transform.parent != lstShoot[j].parentTransform)
					{
						objStaticMesh.transform.parent = lstShoot[j].parentTransform;
					}
					break;
				}
			}
		}
		else
		{
			StaticMeshParentInit();
		}
	}

	public void MetaBallClear()
	{
		for (int i = 0; i < lstShoot.Count; i++)
		{
			lstShoot[i].MetaBallClear();
		}
		StaticMeshParentInit();
		StartCoroutine("CreateMesh");
	}

	private bool StaticMeshParentInit()
	{
		if (!objStaticMesh)
		{
			return false;
		}
		if (objStaticMesh.transform.parent == metaseed.transform)
		{
			return true;
		}
		objStaticMesh.transform.parent = metaseed.transform;
		objStaticMesh.transform.localPosition = Vector3.zero;
		objStaticMesh.transform.localRotation = Quaternion.identity;
		objStaticMesh.transform.localScale = Vector3.one;
		return true;
	}

	private IEnumerator CreateMesh()
	{
		yield return new WaitForSeconds(0.001f);
		if ((bool)metaseed)
		{
			metaseed.CreateMesh();
		}
		yield return null;
	}
}
