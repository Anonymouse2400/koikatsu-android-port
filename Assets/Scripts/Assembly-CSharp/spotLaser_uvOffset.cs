using UnityEngine;

public class spotLaser_uvOffset : MonoBehaviour
{
	private GameObject parent;

	private Material mat;

	private float uvOffset;

	private void Start()
	{
		parent = base.transform.parent.parent.parent.gameObject;
		uvOffset = parent.GetComponent<SpotLaser>().offset;
		mat = GetComponent<Renderer>().material;
		mat.SetFloat("_uvoffset", uvOffset);
	}

	private void Update()
	{
	}
}
