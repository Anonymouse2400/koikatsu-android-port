using System;
using UnityEngine;

public class RayChara : CollisionCamera
{
	[Serializable]
	public class Parts
	{
		public Transform target;

		public void Update(Vector3 pos, string tag)
		{
			Vector3 vector = target.position - pos;
			RaycastHit[] array = Physics.RaycastAll(pos, vector.normalized, vector.magnitude);
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (raycastHit.collider.gameObject.tag == tag)
				{
					raycastHit.collider.gameObject.GetComponent<Renderer>().enabled = false;
				}
			}
		}
	}

	public Parts[] parts;

	private new void Start()
	{
		base.Start();
	}

	private void Update()
	{
		if (this.parts != null && objDels != null)
		{
			GameObject[] array = objDels;
			foreach (GameObject gameObject in array)
			{
				gameObject.GetComponent<Renderer>().enabled = true;
			}
			Parts[] array2 = this.parts;
			foreach (Parts parts in array2)
			{
				parts.Update(base.transform.position, tagName);
			}
		}
	}
}
