using UnityEngine;

namespace Studio
{
	public class PanelComponent : MonoBehaviour
	{
		public Renderer[] renderer;

		public void SetMainTex(Texture2D _texture)
		{
			Renderer[] array = this.renderer;
			foreach (Renderer renderer in array)
			{
				renderer.material.SetTexture(ItemShader._MainTex, _texture);
			}
		}

		public void UpdateColor(OIItemInfo _info)
		{
			Renderer[] array = this.renderer;
			foreach (Renderer renderer in array)
			{
				renderer.material.SetColor(ItemShader._Color, _info.color[0]);
				renderer.material.SetVector(ItemShader._Patternuv1, _info.pattern[0].uv);
				renderer.material.SetFloat(ItemShader._patternrotator1, _info.pattern[0].rot);
				renderer.material.SetFloat(ItemShader._patternclamp1, (!_info.pattern[0].clamp) ? 0f : 1f);
			}
		}

		private void Reset()
		{
			renderer = GetComponentsInChildren<Renderer>();
		}
	}
}
