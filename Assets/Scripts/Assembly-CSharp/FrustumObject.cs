using System.Collections.Generic;
using System.Text;
using IllusionUtility.GetUtility;
using UnityEngine;

public class FrustumObject : CollisionCamera
{
	private new void Start()
	{
		base.Start();
	}

	private void Update()
	{
		SetCollision();
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
		GameObject[] array = objDels;
		foreach (GameObject gameObject in array)
		{
			List<GameObject> list = new List<GameObject>();
			gameObject.transform.parent.FindLoopAll(list);
			if (gameObject.GetComponent<Collider>() == null || gameObject.GetComponent<Renderer>() == null)
			{
				continue;
			}
			foreach (GameObject item in list)
			{
				if ((bool)item.GetComponent<Renderer>())
				{
					item.GetComponent<Renderer>().enabled = true;
				}
			}
			if (!GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<Collider>().bounds))
			{
				continue;
			}
			Vector3 targetPos = camCtrl.TargetPos;
			float num = Vector3.Distance(targetPos, base.transform.position);
			float num2 = Vector3.Distance(gameObject.GetComponent<Collider>().bounds.center, base.transform.position);
			if (!(num > num2))
			{
				continue;
			}
			foreach (GameObject item2 in list)
			{
				if ((bool)item2.GetComponent<Renderer>())
				{
					item2.GetComponent<Renderer>().enabled = false;
				}
			}
		}
	}

	private void OnGUI()
	{
		StringBuilder stringBuilder = new StringBuilder();
		float height = 1000f;
		int num = 0;
		GameObject[] array = objDels;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<Renderer>() != null && (!gameObject.GetComponent<Renderer>().enabled || !gameObject.activeSelf))
			{
				num++;
			}
		}
		stringBuilder.Append("Count:" + num);
		stringBuilder.Append("\n");
		GameObject[] array2 = objDels;
		foreach (GameObject gameObject2 in array2)
		{
			if (gameObject2.GetComponent<Renderer>() != null && (!gameObject2.GetComponent<Renderer>().enabled || !gameObject2.activeSelf))
			{
				stringBuilder.Append(gameObject2.name);
				stringBuilder.Append("\n");
			}
		}
		GUI.Box(new Rect(5f, 5f, 300f, height), string.Empty);
		GUI.Label(new Rect(10f, 5f, 1000f, height), stringBuilder.ToString());
	}

	public object[] GetObjects(Vector3 position, float distance, float fov, Vector3 direction)
	{
		List<GameObject> list = new List<GameObject>();
		Object[] array = Object.FindObjectsOfType(typeof(GameObject));
		base.transform.position = position;
		base.transform.forward = direction;
		GetComponent<Camera>().fieldOfView = fov;
		GetComponent<Camera>().farClipPlane = distance;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = (GameObject)array2[i];
			if (!(gameObject.GetComponent<Collider>() == null) && GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<Collider>().bounds))
			{
				list.Add(gameObject);
			}
		}
		return list.ToArray();
	}

	private bool IsLook(Vector3 pos)
	{
		Vector3 vector = GetComponent<Camera>().WorldToViewportPoint(pos);
		if (vector.x < -0.5f || vector.x > 1.5f || vector.y < -0.5f || vector.y > 1.5f)
		{
			return true;
		}
		return false;
	}
}
