using UnityEngine;

namespace StrayTech
{
	[ExecuteInEditMode]
	public class Grayscale : MonoBehaviour
	{
		[SerializeField]
		private Shader _shader;

		private Material _material;

		private void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				base.enabled = false;
			}
			else if (!_shader || !_shader.isSupported)
			{
				base.enabled = false;
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (_material == null)
			{
				_material = new Material(_shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
			}
			Graphics.Blit(source, destination, _material);
		}

		private void OnDisable()
		{
			if ((bool)_material)
			{
				Object.DestroyImmediate(_material);
			}
		}
	}
}
