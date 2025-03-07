using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Illusion.Component
{
	public class GizmosObjectDrawer : MonoBehaviour
	{
		public enum DrawType
		{
			None = 0,
			Sphere = 1,
			Cube = 2
		}

		[Serializable]
		public class Drawer
		{
			public DrawType type;

			public Color color = Color.clear;

			public float radius;

			public Vector3 size = Vector2.zero;
		}

		public Drawer baseDrawer = new Drawer
		{
			type = DrawType.Sphere,
			color = Color.red,
			radius = 1f,
			size = Vector3.one
		};

		public Drawer childDrawer = new Drawer
		{
			type = DrawType.Sphere,
			color = Color.blue,
			radius = 0.5f,
			size = Vector3.one * 0.5f
		};

		public bool isChildActiveForce = true;

		private void OnDrawGizmos()
		{
			if (baseDrawer.type != 0)
			{
				Gizmos.color = baseDrawer.color;
				switch (baseDrawer.type)
				{
				case DrawType.Sphere:
					Gizmos.DrawSphere(base.transform.position, baseDrawer.radius);
					break;
				case DrawType.Cube:
					Gizmos.DrawCube(base.transform.position, baseDrawer.size);
					break;
				}
			}
			if (childDrawer.type == DrawType.None)
			{
				return;
			}
			Gizmos.color = childDrawer.color;
			List<Transform> list = base.transform.GetComponentsInChildren<Transform>(isChildActiveForce).Skip(1).ToList();
			switch (childDrawer.type)
			{
			case DrawType.Sphere:
				list.ForEach(delegate(Transform child)
				{
					Gizmos.DrawSphere(child.position, childDrawer.radius);
				});
				break;
			case DrawType.Cube:
				list.ForEach(delegate(Transform child)
				{
					Gizmos.DrawCube(child.position, childDrawer.size);
				});
				break;
			}
		}
	}
}
