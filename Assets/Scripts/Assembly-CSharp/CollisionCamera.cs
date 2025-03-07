using UnityEngine;

public class CollisionCamera : MonoBehaviour
{
	public string tagName = "CollDels";

	protected GameObject[] objDels;

	protected BaseCameraControl camCtrl;

	public void SetCollision()
	{
		objDels = GameObject.FindGameObjectsWithTag(tagName);
	}

	protected void Start()
	{
		camCtrl = base.gameObject.GetComponent<BaseCameraControl>();
	}

	private void Update()
	{
		SetCollision();
		if (objDels == null)
		{
			return;
		}
		GameObject[] array = objDels;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject.GetComponent<Renderer>())
			{
				gameObject.GetComponent<Renderer>().enabled = true;
			}
		}
		Vector3 position = base.transform.position;
		Vector3 vector = ((!camCtrl.targetObj) ? camCtrl.TargetPos : camCtrl.targetObj.transform.position);
		Vector3 vector2 = vector - position;
		RaycastHit[] array2 = Physics.RaycastAll(position, vector2.normalized, vector2.magnitude);
		RaycastHit[] array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			RaycastHit raycastHit = array3[j];
			if (raycastHit.collider.gameObject.tag == tagName)
			{
				float magnitude = vector2.magnitude;
				float num = Vector3.Distance(raycastHit.collider.bounds.center, position);
				if (magnitude > num)
				{
					raycastHit.collider.gameObject.GetComponent<Renderer>().enabled = false;
				}
			}
		}
	}
}
