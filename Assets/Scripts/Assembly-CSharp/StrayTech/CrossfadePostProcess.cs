using UnityEngine;

namespace StrayTech
{
	[RequireComponent(typeof(Camera))]
	public class CrossfadePostProcess : MonoBehaviour
	{
		private Material _material;

		private void Start()
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null)
			{
				base.enabled = false;
				return;
			}
			_material = new Material(Shader.Find("Hidden/CrossfadeCamera"));
			_material.hideFlags = HideFlags.HideAndDontSave;
			_material.SetTexture("_CrossfadeTexture", MonoBehaviourSingleton<CameraSystem>.Instance.CameraRenderTexture);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning && (MonoBehaviourSingleton<CameraSystem>.Instance.CurrentTransitionType == CameraSystem.StateTransitionTypeInternal.Crossfade || MonoBehaviourSingleton<CameraSystem>.Instance.CurrentTransitionType == CameraSystem.StateTransitionTypeInternal.InterpolatedCrossfade))
			{
				_material.SetFloat("_Alpha", MonoBehaviourSingleton<CameraSystem>.Instance.CurrentInterpolationCurveSample);
				Graphics.Blit(source, destination, _material);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		private void OnDestroy()
		{
			if ((bool)_material)
			{
				Object.DestroyImmediate(_material);
			}
		}
	}
}
