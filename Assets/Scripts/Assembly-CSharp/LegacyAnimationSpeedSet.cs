using UnityEngine;

public class LegacyAnimationSpeedSet : MonoBehaviour
{
	public float[] AnmSpeed;

	private Animation anim;

	private string[] name_a;

	private int AnmCount;

	private void Awake()
	{
		anim = base.transform.GetComponent<Animation>();
		AnmCount = anim.GetClipCount();
		name_a = new string[AnmCount];
		int num = 0;
		foreach (AnimationState item in anim)
		{
			name_a[num++] = item.name;
		}
	}

	private void Update()
	{
		for (int i = 0; i < AnmCount && AnmSpeed.Length > i; i++)
		{
			anim[name_a[i]].speed = AnmSpeed[i];
		}
	}
}
