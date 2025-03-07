using UnityEngine;

public class SpriteInfo
{
	public static Vector2 GetPivotInSprite(Sprite sprite)
	{
		Vector2 result = default(Vector2);
		if (null != sprite)
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
}
