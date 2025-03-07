using UnityEngine;
using UnityEngine.UI;

public class RawImageUVScrool : MonoBehaviour
{
	public RawImage image;

	public float scrollSpeed = 0.5f;

	private Rect uvRect;

	private void Start()
	{
		uvRect = image.uvRect;
	}

	private void Update()
	{
		uvRect = image.uvRect;
		uvRect.x += Time.deltaTime * scrollSpeed;
		image.uvRect = uvRect;
	}
}
