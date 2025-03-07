using UnityEngine;

public class LightPanel_Bridge : MonoBehaviour
{
	private Animator camAnim;

	private float timer;

	private float speed;

	private Vector2 offset;

	private int colorState;

	private bool lightState;

	private void Start()
	{
		timer = 0f;
		colorState = 0;
		lightState = true;
	}

	private void Update()
	{
		camAnim = GetComponentInParent<Animator>();
		AnimatorStateInfo currentAnimatorStateInfo = camAnim.GetCurrentAnimatorStateInfo(0);
		timer += Time.deltaTime;
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("two_fast") || currentAnimatorStateInfo.tagHash == Animator.StringToHash("color_fast"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 0.3f;
			colorChange();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("two_slow") || currentAnimatorStateInfo.tagHash == Animator.StringToHash("color_slow"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 1f;
			colorChange();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("stop"))
		{
			lightOn();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("off"))
		{
			lightOn();
		}
	}

	private void colorChange()
	{
		if (timer > speed)
		{
			if (colorState == 1)
			{
				offset = new Vector2(0.8f, 0f);
				colorState = 2;
			}
			else
			{
				offset = new Vector2(0f, 0.3f);
				colorState = 1;
			}
			GetComponent<Renderer>().material.SetTextureOffset("_DetailAlbedoMap", offset);
			timer = 0f;
		}
	}

	private void lightOn()
	{
		if (colorState != 0)
		{
			GetComponent<Renderer>().material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0f, 0f));
		}
		colorState = 0;
		GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
		lightState = true;
	}

	private void lightOff()
	{
		if (colorState != 0)
		{
			GetComponent<Renderer>().material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0f, 0f));
		}
		colorState = 0;
		GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
		lightState = false;
	}
}
