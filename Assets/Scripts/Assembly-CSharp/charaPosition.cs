using UnityEngine;

public class charaPosition : MonoBehaviour
{
	public GameObject cf_j_root;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position = cf_j_root.transform.position;
	}
}
