namespace StrayTech
{
	public interface ICameraStateSettings
	{
		CameraSystem.CameraStateEnum StateType { get; }

		bool UseCameraCollision { get; }
	}
}
