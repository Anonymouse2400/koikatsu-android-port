using UnityEngine;

public class LookHit : MonoBehaviour
{
	private bool isNowDragging;

	public bool IsNowDragging
	{
		get
		{
			return isNowDragging;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnMouseDown()
	{
		isNowDragging = true;
	}

	private void OnMouseUp()
	{
		isNowDragging = false;
	}

	private void OnCollisionEnter(Collision col)
	{
	}

	private void OnCollisionExit(Collision col)
	{
	}

	private void OnCollisionStay(Collision col)
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "CollDels")
		{
			col.gameObject.GetComponent<Renderer>().enabled = false;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "CollDels")
		{
			col.gameObject.GetComponent<Renderer>().enabled = true;
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (!(col.gameObject.name == "Floor"))
		{
		}
	}
}
