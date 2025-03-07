using UnityEngine;
using UnityEngine.UI;

public class bl_MiniMapItem : MonoBehaviour
{
	[Separator("TARGET")]
	[Tooltip("UI Prefab")]
	public GameObject GraphicPrefab;

	[Tooltip("Transform to UI Icon will be follow")]
	public Transform Target;

	[Tooltip("Custom Position from target position")]
	public Vector3 OffSet = Vector3.zero;

	[Separator("ICON")]
	public Sprite Icon;

	public Sprite DeathIcon;

	public Color IconColor = new Color(1f, 1f, 1f, 0.9f);

	[Range(1f, 100f)]
	public float Size = 20f;

	[Separator("CIRCLE AREA")]
	public bool ShowCircleArea;

	[Range(1f, 100f)]
	public float CircleAreaRadius = 10f;

	public Color CircleAreaColor = new Color(1f, 1f, 1f, 0.9f);

	[CustomToggle("is Interactable")]
	[Separator("ICON BUTTON")]
	[Tooltip("UI can interact when press it?")]
	public bool isInteractable = true;

	[TextArea(2, 2)]
	public string InfoItem = "Info Icon here";

	[CustomToggle("Off Screen")]
	[Separator("SETTINGS")]
	[Tooltip("Can Icon show when is off screen?")]
	public bool OffScreen = true;

	[Range(0f, 5f)]
	public float BorderOffScreen = 0.01f;

	[Range(1f, 50f)]
	public float OffScreenSize = 10f;

	[Tooltip("Time before render/show item in minimap after instance")]
	[Range(0f, 3f)]
	public float RenderDelay = 0.3f;

	public ItemEffect m_Effect = ItemEffect.None;

	private Image Graphic;

	private RectTransform RectRoot;

	private GameObject cacheItem;

	private RectTransform CircleAreaRect;

	private Vector3 position;

	private bl_MiniMap MiniMap;

	private bl_MiniMap _minimap;

	public Vector3 TargetPosition
	{
		get
		{
			if (Target == null)
			{
				return Vector3.zero;
			}
			return new Vector3(Target.position.x, 0f, Target.position.z);
		}
	}

	private bl_MiniMap m_miniMap
	{
		get
		{
			if (_minimap == null)
			{
				_minimap = cacheItem.transform.root.GetComponentInChildren<bl_MiniMap>();
			}
			return _minimap;
		}
	}

	private void Start()
	{
		MiniMap = bl_MiniMapUtils.GetMiniMap();
		if (bl_MiniMap.MapUIRoot != null)
		{
			CreateIcon();
		}
	}

	private void CreateIcon()
	{
		cacheItem = Object.Instantiate(GraphicPrefab);
		RectRoot = bl_MiniMapUtils.GetMiniMap().MMUIRoot;
		Graphic = cacheItem.GetComponent<Image>();
		if (Icon != null)
		{
			Graphic.sprite = Icon;
			Graphic.color = IconColor;
		}
		cacheItem.transform.SetParent(RectRoot.transform, false);
		Graphic.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		cacheItem.GetComponent<CanvasGroup>().interactable = isInteractable;
		if (Target == null)
		{
			Target = GetComponent<Transform>();
		}
		StartEffect();
		bl_IconItem component = cacheItem.GetComponent<bl_IconItem>();
		component.DelayStart(RenderDelay);
		component.GetInfoItem(InfoItem);
		if (ShowCircleArea)
		{
			CircleAreaRect = component.SetCircleArea(CircleAreaRadius, CircleAreaColor);
		}
	}

	private void Update()
	{
		if (Target == null || Graphic == null)
		{
			return;
		}
		RectTransform component = Graphic.GetComponent<RectTransform>();
		Vector3 vector = TargetPosition + OffSet;
		Vector2 vector2 = bl_MiniMap.MiniMapCamera.WorldToViewportPoint(vector);
		position = new Vector2(vector2.x * RectRoot.sizeDelta.x - RectRoot.sizeDelta.x * 0.5f, vector2.y * RectRoot.sizeDelta.y - RectRoot.sizeDelta.y * 0.5f);
		Vector2 anchoredPosition = position;
		if (OffScreen)
		{
			position.x = Mathf.Clamp(position.x, 0f - (RectRoot.sizeDelta.x * 0.5f - BorderOffScreen), RectRoot.sizeDelta.x * 0.5f - BorderOffScreen);
			position.y = Mathf.Clamp(position.y, 0f - (RectRoot.sizeDelta.y * 0.5f - BorderOffScreen), RectRoot.sizeDelta.y * 0.5f - BorderOffScreen);
		}
		float size = Size;
		if (!m_miniMap.useCompassRotation)
		{
			size = ((position.x != RectRoot.sizeDelta.x * 0.5f - BorderOffScreen && position.y != RectRoot.sizeDelta.y * 0.5f - BorderOffScreen && position.x != 0f - RectRoot.sizeDelta.x * 0.5f - BorderOffScreen && 0f - position.y != RectRoot.sizeDelta.y * 0.5f - BorderOffScreen) ? Size : OffScreenSize);
		}
		else
		{
			Vector3 zero = Vector3.zero;
			Vector3 direction = Target.position - m_miniMap.TargetPosition;
			Vector3 vector3 = bl_MiniMap.MiniMapCamera.transform.InverseTransformDirection(direction);
			vector3.z = 0f;
			vector3 = vector3.normalized / 2f;
			float num = Mathf.Abs(position.x);
			float num2 = Mathf.Abs(0.5f + vector3.x * m_miniMap.CompassSize);
			if (num >= num2)
			{
				zero.x = 0.5f + vector3.x * m_miniMap.CompassSize;
				zero.y = 0.5f + vector3.y * m_miniMap.CompassSize;
				position = zero;
				size = OffScreenSize;
			}
			else
			{
				size = Size;
			}
		}
		component.anchoredPosition = position;
		if (CircleAreaRect != null)
		{
			CircleAreaRect.anchoredPosition = anchoredPosition;
		}
		float num3 = size * MiniMap.IconMultiplier;
		component.sizeDelta = Vector2.Lerp(component.sizeDelta, new Vector2(num3, num3), Time.deltaTime * 8f);
		if (MiniMap.RotationAlwaysFront)
		{
			Quaternion identity = Quaternion.identity;
			identity.x = Target.rotation.x;
			component.localRotation = identity;
		}
		else
		{
			Vector3 eulerAngles = MiniMap.transform.eulerAngles;
			Vector3 zero2 = Vector3.zero;
			zero2.z = 0f - Target.rotation.eulerAngles.y + eulerAngles.y;
			Quaternion rotation = Quaternion.Euler(zero2);
			component.rotation = rotation;
		}
	}

	private void StartEffect()
	{
		Animator component = Graphic.GetComponent<Animator>();
		if (m_Effect == ItemEffect.Pulsing)
		{
			component.SetInteger("Type", 2);
		}
		else if (m_Effect == ItemEffect.Fade)
		{
			component.SetInteger("Type", 1);
		}
	}

	public void DestroyItem(bool inmediate)
	{
		if (!(Graphic == null))
		{
			if (DeathIcon == null || inmediate)
			{
				Graphic.GetComponent<bl_IconItem>().DestroyIcon(inmediate);
			}
			else
			{
				Graphic.GetComponent<bl_IconItem>().DestroyIcon(inmediate, DeathIcon);
			}
		}
	}

	public void SetIcon(Sprite ico)
	{
		if (!(cacheItem == null))
		{
			cacheItem.GetComponent<bl_IconItem>().SetIcon(ico);
		}
	}

	public void SetCircleArea(float radius, Color AreaColor)
	{
		CircleAreaRect = cacheItem.GetComponent<bl_IconItem>().SetCircleArea(radius, AreaColor);
	}

	public void HideCircleArea()
	{
		cacheItem.GetComponent<bl_IconItem>().HideCircleArea();
		CircleAreaRect = null;
	}

	public void HideItem()
	{
		if (cacheItem != null)
		{
			cacheItem.SetActive(false);
		}
	}

	public void ShowItem()
	{
		if (cacheItem != null)
		{
			cacheItem.SetActive(true);
			cacheItem.GetComponent<bl_IconItem>().SetVisibleAlpha();
		}
	}
}
