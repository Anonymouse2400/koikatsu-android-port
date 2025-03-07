using UnityEngine;

public class FinalIKAfter : MonoBehaviour
{
	public GameObject objUpdateMeta;

	private UpdateMeta updateMeta;

	private void Start()
	{
		if ((bool)objUpdateMeta)
		{
			updateMeta = objUpdateMeta.GetComponent<UpdateMeta>();
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < updateMeta.lstShoot.Count; i++)
		{
			updateMeta.lstShoot[i].ShootMetaBall();
		}
		if (updateMeta != null)
		{
			updateMeta.ConstMetaMesh();
		}
	}
}
