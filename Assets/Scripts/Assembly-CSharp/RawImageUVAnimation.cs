using UnityEngine;
using UnityEngine.UI;

public class RawImageUVAnimation : MonoBehaviour
{
	[SerializeField]
	private RawImage raw;

	public float speed = 0.01f;

	public float startU;

	private float time;

	private void Start()
	{
		if ((bool)raw)
		{
			raw = base.transform.GetComponent<RawImage>();
		}
	}

	private void Update()
	{
		time = Mathf.Repeat(time + Time.deltaTime * speed, 1f);
		Rect uvRect = raw.uvRect;
		uvRect.x = time + startU;
		raw.uvRect = uvRect;
	}
}
