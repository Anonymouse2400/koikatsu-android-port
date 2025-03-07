using UnityEngine;

public class getPosition : MonoBehaviour
{
	public GameObject cf_j_foot_L;

	public GameObject cf_j_foot_R;

	public GameObject colL;

	public GameObject colR;

	private void Update()
	{
		colL.transform.position = cf_j_foot_L.transform.position;
		colR.transform.position = cf_j_foot_R.transform.position;
	}
}
