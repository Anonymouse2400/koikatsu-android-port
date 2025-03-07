using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrayTech
{
	[ExecuteInEditMode]
	[RenderHierarchyIcon("Assets/StrayTech/Camera System/Graphics/Trigger.png")]
	public abstract class CameraSystemTriggerBase : MonoBehaviour
	{
		public enum TriggerColliderType
		{
			Box = 0,
			Sphere = 1,
			ConvexMesh = 2
		}

		[Tooltip("The type of collider to use.")]
		[SerializeField]
		private TriggerColliderType _triggerColliderType;

		[SerializeField]
		[Tooltip("Mesh for Convex Mesh collider trigger.")]
		private Mesh _mesh;

		[Tooltip("The color of the volume in the editor.")]
		[SerializeField]
		private Color _volumeColor = new Color(0f, 1f, 0f, 0.25f);

		[SerializeField]
		[Tooltip("Should this volume render as a solid in editor?")]
		private bool _renderSolidVolume = true;

		[Tooltip("Render volume only when selected?")]
		[SerializeField]
		private bool _renderOnlyWhenSelected;

		[Tooltip("Filter collision by tag? (Blank means no tag filter)")]
		[SerializeField]
		protected string _tagFilter;

		[Tooltip("The layers that will trigger the volume.")]
		[SerializeField]
		protected LayerMask _layerMask = -1;

		[SerializeField]
		[Tooltip("Use once then disable.")]
		protected bool _singleUseTrigger;

		private bool _triggerEnabled = true;

		private Collider _collider;

		private List<ITriggerGate> _triggerGates;

		private bool _didTrigger;

		private void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null)
			{
				base.enabled = false;
				return;
			}
			switch (_triggerColliderType)
			{
			case TriggerColliderType.Box:
				_collider = base.gameObject.AddOrGetComponent<BoxCollider>();
				break;
			case TriggerColliderType.Sphere:
				_collider = base.gameObject.AddOrGetComponent<SphereCollider>();
				break;
			case TriggerColliderType.ConvexMesh:
				_collider = base.gameObject.AddOrGetComponent<MeshCollider>();
				(_collider as MeshCollider).sharedMesh = _mesh;
				(_collider as MeshCollider).convex = true;
				break;
			}
			_collider.isTrigger = true;
			_collider.hideFlags = HideFlags.HideInInspector;
			_triggerGates = new List<ITriggerGate>(base.gameObject.GetInterfacesInChildren<ITriggerGate>(true, true));
		}

		protected abstract void TriggerEntered();

		protected abstract void TriggerExited();

		private void OnTriggerEnter(Collider other)
		{
			if (!Application.isPlaying || !base.enabled || !_triggerEnabled || other == null || other.gameObject == null || (_layerMask.value & (1 << other.gameObject.layer)) == 0 || (!string.IsNullOrEmpty(_tagFilter) && other.gameObject.tag != _tagFilter))
			{
				return;
			}
			if (IsTriggerGated())
			{
				for (int i = 0; i < _triggerGates.Count; i++)
				{
					_triggerGates[i].TriggerWasEntered(other);
				}
				StartCoroutine("SpinOnGatedEnter");
			}
			else
			{
				_didTrigger = true;
				TriggerEntered();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (Application.isPlaying && base.enabled && _triggerEnabled && !(other == null) && !(other.gameObject == null) && (_layerMask.value & (1 << other.gameObject.layer)) != 0 && (string.IsNullOrEmpty(_tagFilter) || !(other.gameObject.tag != _tagFilter)))
			{
				StopCoroutine("SpinOnGatedEnter");
				if (_didTrigger)
				{
					TriggerExited();
					_didTrigger = false;
				}
				if (_singleUseTrigger)
				{
					_triggerEnabled = false;
				}
			}
		}

		private IEnumerator SpinOnGatedEnter()
		{
			while (true)
			{
				if (!IsTriggerBlocked() && MonoBehaviourSingleton<CameraSystem>.Instance.SystemStatus != CameraSystem.CameraSystemStatus.Transitioning)
				{
					_didTrigger = true;
					TriggerEntered();
					while (!IsTriggerBlocked() || MonoBehaviourSingleton<CameraSystem>.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
					{
						yield return null;
					}
					TriggerExited();
					_didTrigger = false;
					if (_singleUseTrigger)
					{
						break;
					}
				}
				else
				{
					yield return null;
				}
			}
			_triggerEnabled = false;
		}

		private bool IsTriggerGated()
		{
			return _triggerGates.Count > 0;
		}

		private bool IsTriggerBlocked()
		{
			if (_triggerGates.Count == 0)
			{
				return true;
			}
			bool result = false;
			for (int i = 0; i < _triggerGates.Count; i++)
			{
				if (_triggerGates[i].IsActive && _triggerGates[i].IsTriggerBlocked())
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void OnDestroy()
		{
			if (Application.isPlaying)
			{
				Object.Destroy(_collider);
			}
		}
	}
}
