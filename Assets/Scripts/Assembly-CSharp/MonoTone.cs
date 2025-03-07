using UnityEngine;

internal class MonoTone : MonoBehaviour
{
	[SerializeField]
	private Material monoTone;

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, monoTone);
	}
}
