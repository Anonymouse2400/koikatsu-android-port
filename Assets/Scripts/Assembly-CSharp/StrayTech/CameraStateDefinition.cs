using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	[RenderHierarchyIcon("Assets/StrayTech/Camera System/Graphics/CameraRig.png")]
	public class CameraStateDefinition : MonoBehaviour
	{
		[Tooltip("The camera state to use.")]
		[SerializeField]
		private CameraSystem.CameraStateEnum _cameraState;

		[Tooltip("The type of transition to the new camera state.")]
		[SerializeField]
		private CameraSystem.StateTransitionType _transitionType;

		[Tooltip("The duration of the transition to the new camera state.")]
		[SerializeField]
		private float _transitionDuration = 1f;

		[SerializeField]
		[Tooltip("The Camera GameObject to use for this state.")]
		private Camera _camera;

		[SerializeField]
		private FirstPersonCameraStateSettings _firstPersonStateSettings;

		[SerializeField]
		private IsometricCameraStateSettings _isometricStateSettings;

		[SerializeField]
		private SplineCameraStateSettings _splineStateSettings;

		[SerializeField]
		private ThirdPersonCameraStateSettings _thirdPersonStateSettings;

		[SerializeField]
		private AnimatedCameraStateSettings _animatedCameraStateSettings;

		[SerializeField]
		private PivotCameraStateSettings _pivotCameraStateSettings;

		[SerializeField]
		private ParentedCameraStateSettings _parentedCameraStateSettings;

		[SerializeField]
		private FirstPersonActionCameraStateSettings _firstPersonActionStateSettings;

		[SerializeField]
		private ThirdPersonActionCameraStateSettings _thirdPersonActionStateSettings;

		private ICameraState _state;

		public ICameraState State
		{
			get
			{
				return _state;
			}
		}

		public CameraSystem.StateTransitionType TransitionType
		{
			get
			{
				return _transitionType;
			}
		}

		public float TransitionDuration
		{
			get
			{
				return _transitionDuration;
			}
		}

		public Camera Camera
		{
			get
			{
				return _camera;
			}
		}

		private void Start()
		{
			if (!(MonoBehaviourSingleton<CameraSystem>.Instance == null))
			{
			}
		}

		public void InitializeState()
		{
			if (_state != null)
			{
				_state.Cleanup();
				_state = null;
			}
			switch (_cameraState)
			{
			case CameraSystem.CameraStateEnum.Isometric:
				_state = new IsometricCamera(_isometricStateSettings);
				break;
			case CameraSystem.CameraStateEnum.Spline:
				_state = new SplineCamera(_splineStateSettings);
				break;
			case CameraSystem.CameraStateEnum.FirstPerson:
				_state = new FirstPersonCamera(_firstPersonStateSettings);
				break;
			case CameraSystem.CameraStateEnum.ThirdPerson:
				_state = new ThirdPersonCamera(_thirdPersonStateSettings);
				break;
			case CameraSystem.CameraStateEnum.Animated:
				_state = new AnimatedCamera(_animatedCameraStateSettings);
				break;
			case CameraSystem.CameraStateEnum.Pivot:
				_state = new PivotCamera(_pivotCameraStateSettings);
				break;
			case CameraSystem.CameraStateEnum.Parented:
				_state = new ParentedCamera(_parentedCameraStateSettings);
				break;
			case CameraSystem.CameraStateEnum.FirstPersonAction:
				_state = new FirstPersonActionCamera(_firstPersonActionStateSettings);
				break;
			case CameraSystem.CameraStateEnum.ThirdPersonAction:
				_state = new ThirdPersonActionCamera(_thirdPersonActionStateSettings);
				break;
			}
		}

		public void AddCameraStateTriggerChild()
		{
			GameObject gameObject = new GameObject("Camera State Trigger", typeof(BoxCollider), typeof(EditorVisibleVolume), typeof(EnforceUnitScale), typeof(CameraStateTransitionTrigger));
			gameObject.transform.parent = base.transform;
			CameraStateTransitionTrigger component = gameObject.GetComponent<CameraStateTransitionTrigger>();
			component.TargetCameraStateDefinition = this;
			BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
			component2.isTrigger = true;
			EditorVisibleVolume component3 = gameObject.GetComponent<EditorVisibleVolume>();
			component3.VolumeColor = new Color(0f, 1f, 0f, 0.25f);
		}

		public void AddCameraSplineChild()
		{
			GameObject gameObject = new GameObject("Camera Spline", typeof(BezierSpline));
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
		}
	}
}
