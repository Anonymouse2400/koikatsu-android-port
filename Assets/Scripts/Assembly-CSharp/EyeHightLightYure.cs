using UnityEngine;

public class EyeHightLightYure : MonoBehaviour
{
	public EyeLookMaterialControll eyeLookMaterialCtrl;

	public int Inside;

	public int Outside;

	public int Up;

	public int Down;

	public Rect rect;

	private Material _material;

	private int textureWidth;

	private int textureHeight;

	private Vector2 offset;

	private Vector2 scale;

	public string[] texNames;

	private void Start()
	{
		_material = GetComponent<Renderer>().material;
		Texture mainTexture = _material.mainTexture;
		textureWidth = mainTexture.width;
		textureHeight = mainTexture.height;
		 Rect reference =  rect;
		float num = 0f;
		rect.y = num;
		reference.x = num;
		rect.width = textureWidth;
		rect.height = textureHeight;
		offset = new Vector2(0f, 0f);
		scale = new Vector2(1f, 1f);
	}

	private void FixedUpdate()
	{
		if (eyeLookMaterialCtrl != null)
		{
			offset = eyeLookMaterialCtrl.GetEyeTexOffset();
		}
		rect.x = Random.Range(Inside, Outside);
		rect.y = Random.Range(Up, Down);
		offset += new Vector2(rect.x / (float)textureWidth, rect.y / (float)textureHeight);
		scale = new Vector2(rect.width / (float)textureWidth, rect.height / (float)textureHeight);
		string[] array = texNames;
		foreach (string text in array)
		{
			_material.SetTextureOffset(text, offset);
			_material.SetTextureScale(text, scale);
		}
	}
}
