using System;
using UnityEngine;

namespace StrayTech
{
	[ExecuteInEditMode]
	public class EnforceUnitScale : MonoBehaviour
	{
		private void Awake()
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void Start()
		{
			UpdateCollider();
		}

		private void UpdateCollider()
		{
			if (base.transform.localScale == Vector3.one)
			{
				return;
			}
			Collider[] components = base.gameObject.GetComponents<Collider>();
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				Type type = collider.GetType();
				if (type == typeof(BoxCollider))
				{
					UpdateBoxCollider(collider as BoxCollider);
				}
				else if (type == typeof(SphereCollider))
				{
					UpdateSphereCollider(collider as SphereCollider);
				}
			}
		}

		private void UpdateSphereCollider(SphereCollider sphereCollider)
		{
			float num = Mathf.Max(Mathf.Max(base.transform.localScale.x, base.transform.localScale.y), base.transform.localScale.z);
			sphereCollider.radius *= num;
		}

		private void UpdateBoxCollider(BoxCollider boxCollider)
		{
			boxCollider.size = Vector3.Scale(boxCollider.size, base.transform.localScale);
		}

		private void Update()
		{
			base.transform.localScale = Vector3.one;
		}
	}
}
