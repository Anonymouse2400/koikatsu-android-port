using IllusionUtility.SetUtility;
using UnityEngine;

public class SpCalc : MonoBehaviour
{
	public Vector2 Pos = new Vector2(0f, 0f);

	public Vector2 Scale = new Vector2(1f, 1f);

	public byte CorrectX = 1;

	public byte CorrectY = 1;

	private Vector2 GetPivotInSprite(Sprite sprite)
	{
		Vector2 result = default(Vector2);
		if ((bool)sprite)
		{
			if (sprite.bounds.size.x != 0f)
			{
				result.x = 0.5f - sprite.bounds.center.x / sprite.bounds.size.x;
			}
			if (sprite.bounds.size.y != 0f)
			{
				result.y = 0.5f - sprite.bounds.center.y / sprite.bounds.size.y;
			}
		}
		return result;
	}

	private void Update()
	{
		Calc();
	}

	public void Calc()
	{
		Transform parent = base.transform.parent;
		if (null == parent)
		{
			return;
		}
		SpRoot component = parent.GetComponent<SpRoot>();
		if (null == component)
		{
			return;
		}
		SpriteRenderer component2 = base.gameObject.transform.GetComponent<SpriteRenderer>();
		if (!(null == component2) && !(null == component2.sprite))
		{
			float baseScreenWidth = component.baseScreenWidth;
			float baseScreenHeight = component.baseScreenHeight;
			float spriteRate = component.GetSpriteRate();
			float spriteCorrectY = component.GetSpriteCorrectY();
			Vector2 pivotInSprite = GetPivotInSprite(component2.sprite);
			float x = pivotInSprite.x;
			float num = 1f - pivotInSprite.y;
			float x2 = (Pos.x - (baseScreenWidth * 0.5f - component2.sprite.rect.width * x)) * spriteRate * 0.01f;
			float num2 = (baseScreenHeight * 0.5f - component2.sprite.rect.height * num - Pos.y) * spriteRate * 0.01f;
			if (CorrectY == 0)
			{
				num2 += spriteCorrectY;
			}
			else if (CorrectY == 2)
			{
				num2 -= spriteCorrectY;
			}
			component2.transform.SetLocalPosition(x2, num2, 0f);
			float x3 = spriteRate * Scale.x;
			float y = spriteRate * Scale.y;
			component2.transform.SetLocalScale(x3, y, 1f);
		}
	}
}
