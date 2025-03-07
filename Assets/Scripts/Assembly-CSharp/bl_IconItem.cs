using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class bl_IconItem : MonoBehaviour
{
	[Separator("SETTINGS")]
	public float DestroyIn = 5f;

	[SerializeField]
	[Separator("REFERENCES")]
	private Image TargetGrapihc;

	[SerializeField]
	private RectTransform CircleAreaRect;

	public Sprite DeathIcon;

	[SerializeField]
	private Text InfoText;

	private CanvasGroup m_CanvasGroup;

	private Animator Anim;

	private float delay = 0.1f;

	private bl_MaskHelper MaskHelper;

	private bool open;

	private void Awake()
	{
		if (GetComponent<CanvasGroup>() != null)
		{
			m_CanvasGroup = GetComponent<CanvasGroup>();
		}
		else
		{
			m_CanvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
		if (GetComponent<Animator>() != null)
		{
			Anim = GetComponent<Animator>();
		}
		if (Anim != null)
		{
			Anim.enabled = false;
		}
		m_CanvasGroup.ignoreParentGroups = true;
		m_CanvasGroup.alpha = 0f;
		if (CircleAreaRect != null)
		{
			CircleAreaRect.gameObject.SetActive(false);
		}
	}

	public void DestroyIcon(bool inmediate)
	{
		if (inmediate)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TargetGrapihc.sprite = DeathIcon;
		Object.Destroy(base.gameObject, DestroyIn);
	}

	public void DestroyIcon(bool inmediate, Sprite death)
	{
		if (inmediate)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		TargetGrapihc.sprite = death;
		Object.Destroy(base.gameObject, DestroyIn);
	}

	public void GetInfoItem(string info)
	{
		if (!(InfoText == null))
		{
			InfoText.text = info;
		}
	}

	public void SetIcon(Sprite ico)
	{
		TargetGrapihc.sprite = ico;
	}

	public RectTransform SetCircleArea(float radius, Color AreaColor)
	{
		if (CircleAreaRect == null)
		{
			return null;
		}
		MaskHelper = base.transform.root.GetComponentInChildren<bl_MaskHelper>();
		MaskHelper.SetMaskedIcon(CircleAreaRect);
		radius *= 10f;
		radius *= bl_MiniMapUtils.GetMiniMap().IconMultiplier;
		Vector2 sizeDelta = new Vector2(radius, radius);
		CircleAreaRect.sizeDelta = sizeDelta;
		CircleAreaRect.GetComponent<Image>().CrossFadeColor(AreaColor, 1f, true, true);
		CircleAreaRect.gameObject.SetActive(true);
		return CircleAreaRect;
	}

	public void HideCircleArea()
	{
		CircleAreaRect.SetParent(base.transform);
		CircleAreaRect.gameObject.SetActive(false);
	}

	private IEnumerator FadeIcon()
	{
		yield return new WaitForSeconds(delay);
		while (m_CanvasGroup.alpha < 1f)
		{
			m_CanvasGroup.alpha += Time.deltaTime * 2f;
			yield return null;
		}
		if (Anim != null)
		{
			Anim.enabled = true;
		}
	}

	public void SetVisibleAlpha()
	{
		m_CanvasGroup.alpha = 1f;
	}

	public void InfoItem()
	{
		open = !open;
		Animator component = GetComponent<Animator>();
		if (open)
		{
			component.SetBool("Open", true);
		}
		else
		{
			component.SetBool("Open", false);
		}
	}

	public void DelayStart(float v)
	{
		delay = v;
		StartCoroutine(FadeIcon());
	}
}
