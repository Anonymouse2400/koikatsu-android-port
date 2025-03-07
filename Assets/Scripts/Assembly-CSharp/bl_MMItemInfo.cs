using UnityEngine;

public class bl_MMItemInfo
{
	public Vector3 Position;

	public Transform Target;

	public float Size = 12f;

	public Color Color = new Color(1f, 1f, 1f, 0.95f);

	public bool Interactable;

	public Sprite Sprite;

	public ItemEffect Effect = ItemEffect.Fade;

	public bl_MMItemInfo(Vector3 position)
	{
		Position = position;
	}

	public bl_MMItemInfo(Transform target)
	{
		Target = target;
	}
}
