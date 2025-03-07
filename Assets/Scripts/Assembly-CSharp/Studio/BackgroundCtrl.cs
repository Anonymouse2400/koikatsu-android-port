using System;
using System.IO;
using UnityEngine;

namespace Studio
{
	public class BackgroundCtrl : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter meshFilter;

		[SerializeField]
		private MeshRenderer meshRenderer;

		private bool m_IsVisible;

		private Camera m_Camera;

		private float oldFOV;

		public bool isVisible
		{
			get
			{
				return m_IsVisible;
			}
			set
			{
				m_IsVisible = value;
				meshRenderer.enabled = value;
			}
		}

		private Camera mainCamera
		{
			get
			{
				if (m_Camera == null)
				{
					m_Camera = Camera.main;
				}
				return m_Camera;
			}
		}

		public bool Load(string _file)
		{
			string path = UserData.Path + BackgroundList.dirName + "/" + _file;
			if (!File.Exists(path))
			{
				isVisible = false;
				Singleton<Studio>.Instance.sceneInfo.background = string.Empty;
				return false;
			}
			Texture texture = PngAssist.LoadTexture(path);
			if (texture == null)
			{
				isVisible = false;
				return false;
			}
			Material material = meshRenderer.material;
			material.SetTexture("_MainTex", texture);
			meshRenderer.material = material;
			isVisible = true;
			Singleton<Studio>.Instance.sceneInfo.background = _file;
			Resources.UnloadUnusedAssets();
			GC.Collect();
			return true;
		}

		private void Reflect()
		{
			Vector3[] vertices = meshFilter.mesh.vertices;
			float num = mainCamera.fieldOfView / 2f;
			float angle = Mathf.Atan(Mathf.Tan((float)Math.PI / 180f * num) * mainCamera.aspect) * 57.29578f;
			Plane plane = new Plane(Vector3.back, mainCamera.farClipPlane - 2f);
			Vector3 vector = Raycast(plane, Vector3.forward);
			Vector3 vector2 = Raycast(plane, Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward);
			Vector3 vector3 = Raycast(plane, Quaternion.AngleAxis(num, Vector3.right) * Vector3.forward);
			vertices[0] = new Vector3(vector2.x, 0f - vector3.y, vector.z);
			vertices[1] = new Vector3(0f - vector2.x, vector3.y, vector.z);
			vertices[2] = new Vector3(0f - vector2.x, 0f - vector3.y, vector.z);
			vertices[3] = new Vector3(vector2.x, vector3.y, vector.z);
			meshFilter.mesh.vertices = vertices;
			meshFilter.mesh.RecalculateBounds();
			oldFOV = mainCamera.fieldOfView;
		}

		private Vector3 Raycast(Plane _plane, Vector3 _dir)
		{
			float enter = 0f;
			_plane.Raycast(new Ray(Vector3.zero, _dir), out enter);
			return _dir * enter;
		}

		private void Start()
		{
			isVisible = false;
		}

		private void LateUpdate()
		{
			if (isVisible && oldFOV != mainCamera.fieldOfView)
			{
				Reflect();
			}
		}
	}
}
