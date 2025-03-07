using StrayTech.CustomAttributes;
using UnityEngine;

namespace StrayTech
{
	public class CameraFOVModifier : CameraStateModifierBase
	{
		[Tooltip("The Field of View that this modifier adjusts the camera to when it's active.")]
		[SerializeField]
		[NonNegative]
		private float _fieldOfView = 60f;

		private float _cachedFoV;

		public override string Name
		{
			get
			{
				return "Camera FOV Modifier";
			}
		}

		protected override void CalculateModification(ICameraState cameraState, float deltaTime)
		{
			_cameraTargetPosition = cameraState.Position;
			_cameraTargetRotation = cameraState.Rotation;
			MonoBehaviourSingleton<CameraSystem>.Instance.ChangeCameraFOV(Mathf.Lerp(_cachedFoV, _fieldOfView, base.TransitionLerpT));
		}

		public override bool Enable()
		{
			_cachedFoV = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.fieldOfView;
			return base.Enable();
		}

		public override void Cleanup()
		{
			MonoBehaviourSingleton<CameraSystem>.Instance.ChangeCameraFOV(_cachedFoV);
		}
	}
}
