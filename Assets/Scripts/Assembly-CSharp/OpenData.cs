using System;
using UnityEngine;

public abstract class OpenData : SceneLoader
{
	[Serializable]
	public class CameraData
	{
		public Vector3 position;

		public Quaternion rotation;

		public static Quaternion ToTargetRotation(Vector3 myPos, Vector3 targetPos)
		{
			return Quaternion.LookRotation((targetPos - myPos).normalized);
		}
	}

	[Serializable]
	public class Data
	{
		public MonoBehaviour scene;

		public Vector3 position;

		public Quaternion rotation;

		public CameraData camera;
	}

	public abstract Data data { get; set; }

	protected override void Start()
	{
		isStartAfterErase = false;
		base.Start();
	}
}
