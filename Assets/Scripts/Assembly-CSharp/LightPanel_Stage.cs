using UnityEngine;

public class LightPanel_Stage : MonoBehaviour
{
	private Color[] color;

	private int matID;

	private int lineID;

	private Animator camAnim;

	private int count;

	private float timer;

	private float speed;

	private bool colorState;

	private bool lightState;

	private void Start()
	{
		color = GetComponentInParent<colorList>().color;
		matID = GetComponent<setPanelID>().matID;
		lineID = GetComponent<setPanelID>().lineID;
		GetComponent<Renderer>().material.color = color[matID];
		timer = 0f;
		count = 0;
		colorState = true;
		lightState = true;
	}

	private void Update()
	{
		camAnim = GetComponentInParent<Animator>();
		AnimatorStateInfo currentAnimatorStateInfo = camAnim.GetCurrentAnimatorStateInfo(0);
		timer += Time.deltaTime;
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("color_fast"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 0.03f;
			colorful();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("color_normal"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 0.1f;
			colorful();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("color_slow"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 0.2f;
			colorful();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("two_fast"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 0.3f;
			twotone();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("two_slow"))
		{
			if (!lightState)
			{
				lightOn();
			}
			speed = 1f;
			twotone();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("stop") && colorState)
		{
			twotone();
		}
		if (currentAnimatorStateInfo.tagHash == Animator.StringToHash("off") && colorState)
		{
			twotone();
		}
	}

	private void colorful()
	{
		if (timer > speed)
		{
			timer = 0f;
			if (count == lineID)
			{
				GetComponent<Renderer>().material.color = color[matID];
				matID++;
				if (matID > color.Length - 1)
				{
					matID = 0;
				}
				GetComponent<Renderer>().material.color = color[matID];
			}
			count++;
			if (count > 6)
			{
				count = 0;
			}
		}
		colorState = true;
	}

	private void twotone()
	{
		if (colorState)
		{
			matID = GetComponent<setPanelID>().matID;
		}
		if (timer > speed || colorState)
		{
			timer = 0f;
			GetComponent<Renderer>().material.color = color[matID % 2];
			if (lineID == 0)
			{
				GetComponent<Renderer>().material.color = color[0];
			}
			matID++;
			if (matID > color.Length - 1)
			{
				matID = 0;
			}
			colorState = false;
		}
	}

	private void lightOn()
	{
		GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
		lightState = true;
	}

	private void lightOff()
	{
		GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
		lightState = false;
	}
}
