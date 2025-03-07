using UnityEngine;

public class bl_MMCompass : MonoBehaviour
{
	[Tooltip("This target can be null, this will be take the same target of minimap")]
	public Transform Target;

	[Space(7f)]
	public RectTransform CompassRoot;

	public RectTransform North;

	public RectTransform South;

	public RectTransform East;

	public RectTransform West;

	[HideInInspector]
	public int Grade;

	private int Opposite;

	private Transform t;

	private Transform m_Transform
	{
		get
		{
			if (t == null)
			{
				t = GetComponent<Transform>();
			}
			return t;
		}
	}

	private void Start()
	{
		if (Target == null && GetComponent<bl_MiniMap>() != null)
		{
			Target = GetComponent<bl_MiniMap>().Target;
		}
	}

	private void Update()
	{
		if (Target != null)
		{
			Opposite = (int)Mathf.Abs(Target.eulerAngles.y);
		}
		else
		{
			Opposite = (int)Mathf.Abs(m_Transform.eulerAngles.y);
		}
		if (Opposite > 360)
		{
			Opposite %= 360;
		}
		Grade = Opposite;
		if (Grade > 180)
		{
			Grade -= 360;
		}
		North.anchoredPosition = new Vector2(CompassRoot.sizeDelta.x * 0.5f - (float)(Grade * 2) - CompassRoot.sizeDelta.x * 0.5f, 0f);
		South.anchoredPosition = new Vector2(CompassRoot.sizeDelta.x * 0.5f - (float)(Opposite * 2) + 360f - CompassRoot.sizeDelta.x * 0.5f, 0f);
		East.anchoredPosition = new Vector2(CompassRoot.sizeDelta.x * 0.5f - (float)(Grade * 2) + 180f - CompassRoot.sizeDelta.x * 0.5f, 0f);
		West.anchoredPosition = new Vector2(CompassRoot.sizeDelta.x * 0.5f - (float)(Opposite * 2) + 540f - CompassRoot.sizeDelta.x * 0.5f, 0f);
	}
}
