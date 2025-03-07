using System;
using System.Collections.Generic;
using System.Linq;
using StrayTech.CustomAttributes;
using UnityEngine;

namespace StrayTech
{
	[RenderHierarchyIcon("Assets/StrayTech/Camera System/Graphics/CameraSystem.png")]
	public class CameraSystem : MonoBehaviourSingleton<CameraSystem>
	{
		public enum CameraStateEnum
		{
			Isometric = 0,
			Spline = 1,
			FirstPerson = 2,
			ThirdPerson = 3,
			Animated = 4,
			Pivot = 5,
			Parented = 6,
			FirstPersonAction = 7,
			ThirdPersonAction = 8
		}

		public enum StateTransitionType
		{
			Interpolation = 0,
			Crossfade = 1,
			Instant = 2
		}

		public enum StateTransitionTypeInternal
		{
			Interpolation = 0,
			Crossfade = 1,
			Instant = 2,
			InterpolatedCrossfade = 3
		}

		public enum CameraSystemStatus
		{
			Active = 0,
			Transitioning = 1,
			Inactive = 2
		}

		[Serializable]
		public class UserDefinedFlag
		{
			[SerializeField]
			private string _name;

			[SerializeField]
			private bool _value;

			public string Name
			{
				get
				{
					return _name;
				}
				set
				{
					_name = value;
				}
			}

			public bool Value
			{
				get
				{
					return _value;
				}
				set
				{
					_value = value;
				}
			}
		}

		[Tooltip("The target for most of the camera states that require a target.")]
		[SerializeField]
		private Transform _cameraTarget;

		[SerializeField]
		[Tooltip("The default Camera State.")]
		private CameraStateDefinition _defaultCameraState;

		[Tooltip("Use FixedUpdate for physics based camera tracking.")]
		[SerializeField]
		private bool _useFixedUpdate;

		[Tooltip("The current camera state.")]
		[Uneditable]
		public string _debugCurrentStateName = "None";

		[Tooltip("User defined flags.")]
		[SerializeField]
		private List<UserDefinedFlag> _userDefinedFlags = new List<UserDefinedFlag>();

		private CameraSystemStatus _systemStatus = CameraSystemStatus.Inactive;

		private Camera _defaultCamera;

		private Camera _currentCamera;

		private Camera _nextCamera;

		private float _stateTransitionRamp = 1f;

		private StateTransitionTypeInternal _currentTransitionType;

		private LinkedList<CameraStateDefinition> _stateDefinitionHistory = new LinkedList<CameraStateDefinition>();

		private Queue<CameraStateDefinition> _stateDefinitionsToAdd = new Queue<CameraStateDefinition>();

		private Queue<CameraStateDefinition> _stateDefinitionsToRemove = new Queue<CameraStateDefinition>();

		private CameraStateDefinition _cameraStateOverride;

		private CameraStateDefinition _currentCameraStateDefinition;

		private CameraStateDefinition _nextCameraStateDefinition;

		private CameraStateDefinition _currentTransitionHost;

		private bool _transitionInteruptTransition;

		private List<CameraStateModifierBase> _cameraModifiers = new List<CameraStateModifierBase>();

		private RenderTexture _cameraRenderTexture;

		private CrossfadePostProcess _crossfadePostProcess;

		private bool _shouldUpdate = true;

		private Vector3 _cachedCameraPosition;

		private Quaternion _cachedCameraRotation;

		private Dictionary<string, bool> _userDefinedFlagsLookup = new Dictionary<string, bool>();

		private AnimationCurve _cameraInterpolationCurve;

		public CameraSystemStatus SystemStatus
		{
			get
			{
				return _systemStatus;
			}
		}

		public CameraStateDefinition CurrentCameraStateDefinition
		{
			get
			{
				return _currentCameraStateDefinition;
			}
		}

		public CameraStateDefinition NextCameraStateDefinition
		{
			get
			{
				return _nextCameraStateDefinition;
			}
		}

		public StateTransitionTypeInternal CurrentTransitionType
		{
			get
			{
				return _currentTransitionType;
			}
		}

		public Camera CurrentCamera
		{
			get
			{
				return _currentCamera;
			}
		}

		public Camera NextCamera
		{
			get
			{
				return _nextCamera;
			}
		}

		public AnimationCurve CameraInterpolationCurve
		{
			get
			{
				return _cameraInterpolationCurve;
			}
		}

		public float CurrentInterpolationCurveSample
		{
			get
			{
				return _cameraInterpolationCurve.Evaluate(Mathf.Clamp01(_stateTransitionRamp));
			}
		}

		public RenderTexture CameraRenderTexture
		{
			get
			{
				return _cameraRenderTexture;
			}
		}

		public List<CameraStateModifierBase> CameraStateModifiers
		{
			get
			{
				return _cameraModifiers;
			}
		}

		public bool ShouldUpdate
		{
			get
			{
				return _shouldUpdate;
			}
			set
			{
				_shouldUpdate = value;
			}
		}

		public Transform CameraTarget
		{
			get
			{
				return _cameraTarget;
			}
			set
			{
				_cameraTarget = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_defaultCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			_currentCamera = _defaultCamera;
			_crossfadePostProcess = _currentCamera.gameObject.AddOrGetComponent<CrossfadePostProcess>();
			_crossfadePostProcess.enabled = false;
			_cameraRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBHalf);
			_cameraInterpolationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			CacheUserDefinedFlags();
			if (_defaultCameraState != null)
			{
				RegisterCameraState(_defaultCameraState);
			}
		}

		private void Update()
		{
			if (!_useFixedUpdate)
			{
				DoCameraUpdate(Time.deltaTime);
			}
		}

		private void FixedUpdate()
		{
			if (_useFixedUpdate)
			{
				DoCameraUpdate(Time.fixedDeltaTime);
			}
		}

		private void DoCameraUpdate(float deltaTime)
		{
			if (!_shouldUpdate || _currentCameraStateDefinition == null)
			{
				return;
			}
			_debugCurrentStateName = _currentCameraStateDefinition.State.ToString();
			ManageQueuedTransitions();
			_currentCameraStateDefinition.State.UpdateCamera(deltaTime);
			if (_currentCameraStateDefinition.State.AllowsModifiers)
			{
				for (int i = 0; i < _cameraModifiers.Count; i++)
				{
					_cameraModifiers[i].ModifiyCamera(_currentCameraStateDefinition.State, deltaTime);
				}
			}
			if (_systemStatus == CameraSystemStatus.Transitioning)
			{
				_nextCameraStateDefinition.State.UpdateCamera(deltaTime);
				if (_nextCameraStateDefinition.State.AllowsModifiers)
				{
					for (int j = 0; j < _cameraModifiers.Count; j++)
					{
						_cameraModifiers[j].ModifiyCamera(_nextCameraStateDefinition.State, deltaTime);
					}
				}
			}
			else
			{
				_currentCamera.transform.position = _currentCameraStateDefinition.State.Position;
				_currentCamera.transform.rotation = _currentCameraStateDefinition.State.Rotation;
				if (MonoBehaviourSingleton<CameraCollision>.Instance != null && _currentCameraStateDefinition.State.StateSettings.UseCameraCollision)
				{
					MonoBehaviourSingleton<CameraCollision>.Instance.PreventCameraCollision(_currentCamera);
				}
			}
		}

		private void LateUpdate()
		{
			if (_systemStatus == CameraSystemStatus.Transitioning)
			{
				switch (_currentTransitionType)
				{
				case StateTransitionTypeInternal.Interpolation:
					HandleInterpolationTransition();
					break;
				case StateTransitionTypeInternal.Crossfade:
					HandleCrossfadeTransition();
					break;
				case StateTransitionTypeInternal.Instant:
					HandleInstantTransition();
					break;
				case StateTransitionTypeInternal.InterpolatedCrossfade:
					HandleInterpolatedCrossfadeTransition();
					break;
				}
			}
		}

		public void SetCameraStateTempOverride(CameraStateDefinition newState)
		{
			_cameraStateOverride = newState;
			_currentTransitionHost = newState;
			ActivateTransition(newState, newState);
		}

		public void RegisterCameraState(CameraStateDefinition newState)
		{
			if (_shouldUpdate)
			{
				if (_cameraStateOverride != null)
				{
					_cameraStateOverride = null;
				}
				if (_currentCameraStateDefinition == null)
				{
					_stateDefinitionHistory.AddLast(newState);
					_currentTransitionHost = newState;
					ActivateTransition(newState, newState);
				}
				else
				{
					_stateDefinitionsToAdd.Enqueue(newState);
				}
			}
		}

		public void UnregisterCameraState(CameraStateDefinition oldState)
		{
			if (_shouldUpdate && _stateDefinitionHistory.Contains(oldState))
			{
				_stateDefinitionsToRemove.Enqueue(oldState);
			}
		}

		private void ManageQueuedTransitions()
		{
			if (_systemStatus == CameraSystemStatus.Transitioning && (_currentTransitionType == StateTransitionTypeInternal.Crossfade || _currentTransitionType == StateTransitionTypeInternal.InterpolatedCrossfade))
			{
				return;
			}
			if (_stateDefinitionsToAdd.Count > 0)
			{
				CameraStateDefinition cameraStateDefinition = _stateDefinitionsToAdd.Dequeue();
				_stateDefinitionHistory.AddLast(cameraStateDefinition);
				if (_stateDefinitionHistory.Count == 1 || (_stateDefinitionHistory.Count > 1 && _stateDefinitionHistory.Last.Previous.Value != cameraStateDefinition))
				{
					ActivateTransition(cameraStateDefinition, cameraStateDefinition);
				}
			}
			if (_stateDefinitionsToRemove.Count <= 0)
			{
				return;
			}
			if (_stateDefinitionHistory.Last() == _stateDefinitionsToRemove.Peek() && _cameraStateOverride == null)
			{
				CameraStateDefinition cameraStateDefinition2 = _stateDefinitionsToRemove.Peek();
				_stateDefinitionHistory.RemoveLast();
				if (_stateDefinitionHistory.Count > 0)
				{
					CameraStateDefinition cameraStateDefinition3 = _stateDefinitionHistory.Last();
					if (_stateDefinitionHistory.Count == 1)
					{
						ActivateTransition(cameraStateDefinition3, cameraStateDefinition3);
					}
					else if (cameraStateDefinition3 != cameraStateDefinition2)
					{
						ActivateTransition(cameraStateDefinition3, cameraStateDefinition2);
					}
				}
			}
			else
			{
				_stateDefinitionHistory.Remove(_stateDefinitionsToRemove.Peek());
			}
			_stateDefinitionsToRemove.Dequeue();
		}

		private void ActivateTransition(CameraStateDefinition toState, CameraStateDefinition transitionHost)
		{
			if (_systemStatus == CameraSystemStatus.Transitioning)
			{
				_transitionInteruptTransition = true;
			}
			else
			{
				_transitionInteruptTransition = false;
			}
			_systemStatus = CameraSystemStatus.Transitioning;
			_nextCameraStateDefinition = toState;
			_nextCamera = ((!(_nextCameraStateDefinition.Camera != null)) ? _defaultCamera : _nextCameraStateDefinition.Camera);
			_currentTransitionHost = transitionHost;
			if (_currentTransitionHost == _nextCameraStateDefinition)
			{
				_currentTransitionHost.InitializeState();
			}
			if (_currentCamera != null && _nextCamera != null && _currentCamera != _nextCamera)
			{
				AudioListener component = _currentCamera.gameObject.GetComponent<AudioListener>();
				if (component != null)
				{
					component.enabled = false;
				}
				AudioListener component2 = _nextCamera.gameObject.GetComponent<AudioListener>();
				if (component2 != null)
				{
					component2.enabled = true;
				}
			}
			if (_currentTransitionHost.TransitionType == StateTransitionType.Instant || Mathf.Approximately(_currentTransitionHost.TransitionDuration, 0f) || _currentCameraStateDefinition == null)
			{
				_currentTransitionType = StateTransitionTypeInternal.Instant;
				return;
			}
			if (_currentTransitionHost.TransitionType == StateTransitionType.Crossfade)
			{
				_currentTransitionType = StateTransitionTypeInternal.Crossfade;
			}
			else if (_currentCamera == _nextCamera)
			{
				_currentTransitionType = StateTransitionTypeInternal.Interpolation;
			}
			else
			{
				_currentTransitionType = StateTransitionTypeInternal.InterpolatedCrossfade;
			}
			_cachedCameraPosition = _currentCamera.transform.position;
			_cachedCameraRotation = _currentCamera.transform.rotation;
			_stateTransitionRamp = 0f;
		}

		private void HandleInstantTransition()
		{
			FinalizeTransition();
		}

		private void HandleInterpolationTransition()
		{
			_stateTransitionRamp += Time.unscaledDeltaTime / _currentTransitionHost.TransitionDuration;
			_stateTransitionRamp = Mathf.Clamp01(_stateTransitionRamp);
			if (Mathf.Approximately(_stateTransitionRamp, 1f))
			{
				FinalizeTransition();
				return;
			}
			float currentInterpolationCurveSample = CurrentInterpolationCurveSample;
			if (_transitionInteruptTransition)
			{
				_currentCamera.transform.position = Vector3.Lerp(_cachedCameraPosition, _nextCameraStateDefinition.State.Position, currentInterpolationCurveSample);
				_currentCamera.transform.rotation = Quaternion.Slerp(_cachedCameraRotation, _nextCameraStateDefinition.State.Rotation, currentInterpolationCurveSample);
			}
			else
			{
				_currentCamera.transform.position = Vector3.Lerp(_currentCameraStateDefinition.State.Position, _nextCameraStateDefinition.State.Position, currentInterpolationCurveSample);
				_currentCamera.transform.rotation = Quaternion.Slerp(_currentCameraStateDefinition.State.Rotation, _nextCameraStateDefinition.State.Rotation, currentInterpolationCurveSample);
			}
			if (MonoBehaviourSingleton<CameraCollision>.Instance != null && _nextCameraStateDefinition != null && _nextCameraStateDefinition.State.StateSettings.UseCameraCollision)
			{
				MonoBehaviourSingleton<CameraCollision>.Instance.PreventCameraCollision(_nextCamera);
			}
		}

		private void HandleCrossfadeTransition()
		{
			_stateTransitionRamp += Time.unscaledDeltaTime / _currentTransitionHost.TransitionDuration;
			_stateTransitionRamp = Mathf.Clamp01(_stateTransitionRamp);
			_nextCamera.transform.position = _nextCameraStateDefinition.State.Position;
			_nextCamera.transform.rotation = _nextCameraStateDefinition.State.Rotation;
			if (MonoBehaviourSingleton<CameraCollision>.Instance != null && _nextCameraStateDefinition != null && _nextCameraStateDefinition.State.StateSettings.UseCameraCollision)
			{
				MonoBehaviourSingleton<CameraCollision>.Instance.PreventCameraCollision(_nextCamera);
			}
			if (Mathf.Approximately(_stateTransitionRamp, 1f))
			{
				FinalizeTransition();
				return;
			}
			_crossfadePostProcess.enabled = false;
			_nextCamera.targetTexture = _cameraRenderTexture;
			_nextCamera.Render();
			_nextCamera.targetTexture = null;
			_crossfadePostProcess.enabled = true;
			if (_transitionInteruptTransition)
			{
				_currentCamera.transform.position = _cachedCameraPosition;
				_currentCamera.transform.rotation = _cachedCameraRotation;
			}
			else
			{
				_currentCamera.transform.position = _currentCameraStateDefinition.State.Position;
				_currentCamera.transform.rotation = _currentCameraStateDefinition.State.Rotation;
			}
			if (MonoBehaviourSingleton<CameraCollision>.Instance != null && _currentCameraStateDefinition != null && _currentCameraStateDefinition.State.StateSettings.UseCameraCollision)
			{
				MonoBehaviourSingleton<CameraCollision>.Instance.PreventCameraCollision(_currentCamera);
			}
		}

		private void HandleInterpolatedCrossfadeTransition()
		{
			_stateTransitionRamp += Time.unscaledDeltaTime / _currentTransitionHost.TransitionDuration;
			_stateTransitionRamp = Mathf.Clamp01(_stateTransitionRamp);
			if (Mathf.Approximately(_stateTransitionRamp, 1f))
			{
				FinalizeTransition();
				return;
			}
			float currentInterpolationCurveSample = CurrentInterpolationCurveSample;
			Vector3 position;
			Quaternion rotation;
			if (_transitionInteruptTransition)
			{
				position = Vector3.Lerp(_cachedCameraPosition, _nextCameraStateDefinition.State.Position, currentInterpolationCurveSample);
				rotation = Quaternion.Slerp(_cachedCameraRotation, _nextCameraStateDefinition.State.Rotation, currentInterpolationCurveSample);
			}
			else
			{
				position = Vector3.Lerp(_currentCameraStateDefinition.State.Position, _nextCameraStateDefinition.State.Position, currentInterpolationCurveSample);
				rotation = Quaternion.Slerp(_currentCameraStateDefinition.State.Rotation, _nextCameraStateDefinition.State.Rotation, currentInterpolationCurveSample);
			}
			_currentCamera.transform.position = position;
			_currentCamera.transform.rotation = rotation;
			if (MonoBehaviourSingleton<CameraCollision>.Instance != null && _currentCameraStateDefinition != null && _currentCameraStateDefinition.State.StateSettings.UseCameraCollision)
			{
				MonoBehaviourSingleton<CameraCollision>.Instance.PreventCameraCollision(_currentCamera);
			}
			_nextCamera.transform.position = _currentCamera.transform.position;
			_nextCamera.transform.rotation = _currentCamera.transform.rotation;
			_crossfadePostProcess.enabled = false;
			_nextCamera.targetTexture = _cameraRenderTexture;
			_nextCamera.Render();
			_nextCamera.targetTexture = null;
			_crossfadePostProcess.enabled = true;
		}

		private void FinalizeTransition()
		{
			if (_systemStatus != CameraSystemStatus.Transitioning)
			{
				return;
			}
			if (_currentCameraStateDefinition != null)
			{
				_currentCameraStateDefinition.State.Cleanup();
			}
			_currentCameraStateDefinition = _nextCameraStateDefinition;
			_nextCameraStateDefinition = null;
			if (_nextCamera != _currentCamera)
			{
				if (_currentCamera != null)
				{
					_currentCamera.enabled = false;
				}
				_currentCamera = _nextCamera;
				_currentCamera.enabled = true;
				_crossfadePostProcess.enabled = false;
				_crossfadePostProcess = _currentCamera.gameObject.AddOrGetComponent<CrossfadePostProcess>();
				_nextCamera = null;
			}
			if (!_transitionInteruptTransition)
			{
				_currentCamera.transform.position = _currentCameraStateDefinition.State.Position;
				_currentCamera.transform.rotation = _currentCameraStateDefinition.State.Rotation;
			}
			_stateTransitionRamp = 0f;
			_crossfadePostProcess.enabled = false;
			_currentTransitionHost = null;
			_transitionInteruptTransition = false;
			_systemStatus = CameraSystemStatus.Active;
		}

		public void AddModifier(CameraStateModifierBase modifier)
		{
			if (!_cameraModifiers.Contains(modifier))
			{
				_cameraModifiers.Add(modifier);
				modifier.Initialize();
				_cameraModifiers = _cameraModifiers.OrderBy((CameraStateModifierBase m) => m.Priority).ToList();
			}
		}

		public void RemoveModifier(CameraStateModifierBase modifier)
		{
			if (_cameraModifiers.Contains(modifier))
			{
				_cameraModifiers.Remove(modifier);
			}
		}

		public void ChangeCameraFOV(float fov)
		{
			_currentCamera.fieldOfView = fov;
		}

		private void CacheUserDefinedFlags()
		{
			foreach (UserDefinedFlag userDefinedFlag in _userDefinedFlags)
			{
				_userDefinedFlagsLookup.Add(userDefinedFlag.Name, userDefinedFlag.Value);
			}
		}

		public bool GetUserDefinedFlagValue(string flagName)
		{
			bool value = false;
			if (_userDefinedFlagsLookup.TryGetValue(flagName, out value))
			{
				return value;
			}
			return false;
		}

		public void SetUserDefinedFlagValue(string flagName, bool flagValue)
		{
			bool value = false;
			if (_userDefinedFlagsLookup.TryGetValue(flagName, out value))
			{
				_userDefinedFlagsLookup[flagName] = flagValue;
				return;
			}
			_userDefinedFlags.Add(new UserDefinedFlag
			{
				Name = flagName,
				Value = flagValue
			});
			_userDefinedFlagsLookup.Add(flagName, flagValue);
		}
	}
}
