using UnityEngine;

namespace StrayTech
{
	public interface ICameraState
	{
		ICameraStateSettings StateSettings { get; }

		Vector3 Position { get; set; }

		Quaternion Rotation { get; set; }

		CameraSystem.CameraStateEnum StateType { get; }

		bool AllowsModifiers { get; }

		void UpdateCamera(float deltaTime);

		void Cleanup();
	}
}
