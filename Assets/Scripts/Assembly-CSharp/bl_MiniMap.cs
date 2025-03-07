using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bl_MiniMap : MonoBehaviour
{
	[Serializable]
	public enum RenderType
	{
		RealTime = 0,
		Picture = 1
	}

	[Serializable]
	public enum RenderMode
	{
		Mode2D = 0,
		Mode3D = 1
	}

	[Serializable]
	public enum MapType
	{
		Target = 0,
		World = 1
	}

	[Separator("General Settings")]
	public GameObject m_Target;

	public string LevelName;

	[LayerMask]
	public int MiniMapLayer = 10;

	[Tooltip("Keycode to toggle map size mode (world and mini map)")]
	public KeyCode ToogleKey = KeyCode.E;

	public Camera MMCamera;

	public RenderType m_Type = RenderType.Picture;

	public RenderMode m_Mode;

	public MapType m_MapType;

	public bool Ortographic2D;

	[Separator("Height")]
	[Range(0.05f, 2f)]
	public float IconMultiplier = 1f;

	[Tooltip("How much should we move for each small movement on the mouse wheel?")]
	[Range(1f, 10f)]
	public int scrollSensitivity = 3;

	[Range(5f, 500f)]
	public float DefaultHeight = 30f;

	[Tooltip("Maximum heights that the camera can reach.")]
	public float maxHeight = 80f;

	[Tooltip("Minimum heights that the camera can reach.")]
	public float minHeight = 5f;

	public KeyCode IncreaseHeightKey = KeyCode.KeypadPlus;

	public KeyCode DecreaseHeightKey = KeyCode.KeypadMinus;

	[Tooltip("Smooth speed to height change.")]
	[Range(1f, 15f)]
	public float LerpHeight = 8f;

	[CustomToggle("Use Compass Rotation")]
	[Tooltip("Compass rotation for circle maps, rotate icons around pivot.")]
	[Separator("Rotation")]
	public bool useCompassRotation;

	[Tooltip("Size of Compass rotation diametre.")]
	[Range(25f, 500f)]
	public float CompassSize = 175f;

	[CustomToggle("Rotation Always in front")]
	public bool RotationAlwaysFront = true;

	[CustomToggle("Dynamic Rotation")]
	[Tooltip("Should the minimap rotate with the player?")]
	public bool DynamicRotation = true;

	[CustomToggle("Smooth Rotation")]
	[Tooltip("this work only is dynamic rotation.")]
	public bool SmoothRotation = true;

	[Range(1f, 15f)]
	public float LerpRotation = 8f;

	[Separator("Animations")]
	[CustomToggle("Show Level Name")]
	public bool ShowLevelName = true;

	[CustomToggle("Show Panel Info")]
	public bool ShowPanelInfo = true;

	[Range(0.1f, 5f)]
	public float HitEffectSpeed = 1.5f;

	[SerializeField]
	private Animator BottonAnimator;

	[SerializeField]
	private Animator PanelInfoAnimator;

	[SerializeField]
	private Animator HitEffectAnimator;

	[Tooltip("Position for World Map.")]
	[Separator("Map Rect")]
	public Vector3 FullMapPosition = Vector2.zero;

	[Tooltip("Rotation for World Map.")]
	public Vector3 FullMapRotation = Vector3.zero;

	[Tooltip("Size of World Map.")]
	public Vector2 FullMapSize = Vector2.zero;

	private Vector3 MiniMapPosition = Vector2.zero;

	private Vector3 MiniMapRotation = Vector3.zero;

	private Vector2 MiniMapSize = Vector2.zero;

	[Tooltip("Smooth Speed for MiniMap World Map transition.")]
	[Range(1f, 15f)]
	[Space(5f)]
	public float LerpTransition = 7f;

	[InspectorButton("GetFullMapSize")]
	[Space(5f)]
	public string GetWorldRect = string.Empty;

	[Separator("Drag Settings")]
	[CustomToggle("Can Drag MiniMap")]
	public bool CanDragMiniMap = true;

	[CustomToggle("Drag Only On Fullscreen")]
	public bool DragOnlyOnFullScreen = true;

	[CustomToggle("Reset Position On Change")]
	public bool ResetOffSetOnChange = true;

	public Vector2 DragMovementSpeed = new Vector2(0.5f, 0.35f);

	public Vector2 MaxOffSetPosition = new Vector2(1000f, 1000f);

	public Texture2D DragCursorIcon;

	public Vector2 HotSpot = Vector2.zero;

	[Separator("Picture Mode Settings")]
	[Tooltip("Texture for MiniMap renderer, you can take a snaphot from map.")]
	public Texture MapTexture;

	public Color TintColor = new Color(1f, 1f, 1f, 0.9f);

	public Color SpecularColor = new Color(1f, 1f, 1f, 0.9f);

	public Color EmessiveColor = new Color(0f, 0f, 0f, 0.9f);

	[Range(0.1f, 4f)]
	public float EmissionAmount = 1f;

	[SerializeField]
	private Material ReferenceMat;

	[Space(3f)]
	public GameObject MapPlane;

	public RectTransform WorldSpace;

	[Separator("UI")]
	public Canvas m_Canvas;

	public RectTransform MMUIRoot;

	public Image PlayerIcon;

	[SerializeField]
	private GameObject ItemPrefabSimple;

	[SerializeField]
	private GameObject ItemPrefab;

	public Dictionary<string, Transform> ItemsList = new Dictionary<string, Transform>();

	public static bool isFullScreen;

	public static Camera MiniMapCamera;

	public static RectTransform MapUIRoot;

	private Vector3 DragOffset = Vector3.zero;

	private bool DefaultRotationMode;

	private Vector3 DeafultMapRot = Vector3.zero;

	private bool DefaultRotationCircle;

	private GameObject plane;

	private float defaultYCameraPosition;

	private const string MMHeightKey = "MinimapCameraHeight";

	private bool getDelayPositionCamera;

	private Transform t;

	private bl_MaskHelper _maskHelper;

	public Transform Target
	{
		get
		{
			if (m_Target != null)
			{
				return m_Target.GetComponent<Transform>();
			}
			return GetComponent<Transform>();
		}
	}

	public Vector3 TargetPosition
	{
		get
		{
			Vector3 result = Vector3.zero;
			if (m_Target != null)
			{
				result = m_Target.transform.position;
			}
			return result;
		}
	}

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

	private bl_MaskHelper m_maskHelper
	{
		get
		{
			if (_maskHelper == null)
			{
				_maskHelper = base.transform.root.GetComponentInChildren<bl_MaskHelper>();
			}
			return _maskHelper;
		}
	}

	private void Awake()
	{
		GetMiniMapSize();
		MiniMapCamera = MMCamera;
		MapUIRoot = MMUIRoot;
		DefaultRotationMode = DynamicRotation;
		DeafultMapRot = m_Transform.eulerAngles;
		DefaultRotationCircle = useCompassRotation;
		if (m_Type == RenderType.Picture)
		{
			CreateMapPlane();
		}
		if (m_Mode == RenderMode.Mode3D)
		{
			ConfigureCamera3D();
		}
		if (m_MapType == MapType.Target)
		{
			DefaultHeight = PlayerPrefs.GetFloat("MinimapCameraHeight", DefaultHeight);
			return;
		}
		ConfigureWorlTarget();
		PlayerIcon.gameObject.SetActive(false);
	}

	private void CreateMapPlane()
	{
		string value = LayerMask.LayerToName(MiniMapLayer);
		if (string.IsNullOrEmpty(value))
		{
			MMUIRoot.gameObject.SetActive(false);
			base.enabled = false;
		}
		if (!(MapTexture == null))
		{
			Vector3 localPosition = WorldSpace.localPosition;
			Vector3 vector = WorldSpace.sizeDelta;
			MMCamera.cullingMask = 1 << MiniMapLayer;
			plane = UnityEngine.Object.Instantiate(MapPlane);
			plane.transform.localPosition = localPosition;
			plane.transform.localScale = new Vector3(vector.x, 10f, vector.y) / 10f;
			plane.GetComponent<Renderer>().material = CreateMaterial();
			plane.layer = MiniMapLayer;
			plane.SetActive(false);
			plane.SetActive(true);
			Invoke("DelayPositionInvoke", 2f);
		}
	}

	private void DelayPositionInvoke()
	{
		defaultYCameraPosition = MMCamera.transform.position.y;
		getDelayPositionCamera = true;
	}

	public void ConfigureCamera3D()
	{
		Camera camera = ((!(Camera.main != null)) ? Camera.current : Camera.main);
		if (!(camera == null))
		{
			m_Canvas.worldCamera = camera;
			camera.nearClipPlane = 0.015f;
			m_Canvas.planeDistance = 0.1f;
		}
	}

	public void ConfigureWorlTarget()
	{
		if (!(m_Target == null))
		{
			bl_MiniMapItem bl_MiniMapItem2 = m_Target.AddComponent<bl_MiniMapItem>();
			bl_MiniMapItem2.GraphicPrefab = ItemPrefab;
			bl_MiniMapItem2.Icon = PlayerIcon.sprite;
			bl_MiniMapItem2.IconColor = PlayerIcon.color;
			bl_MiniMapItem2.Target = m_Target.transform;
		}
	}

	private void Update()
	{
		if (!(m_Target == null) && !(MMCamera == null))
		{
			Inputs();
			PositionControll();
			RotationControll();
			MapSize();
		}
	}

	private void PositionControll()
	{
		if (m_MapType == MapType.Target)
		{
			Vector3 position = m_Transform.position;
			position.x = Target.position.x;
			if (!Ortographic2D)
			{
				position.z = Target.position.z;
			}
			else
			{
				position.y = Target.position.y;
			}
			position += DragOffset;
			if (Target != null)
			{
				Vector3 viewPoint = MMCamera.WorldToViewportPoint(TargetPosition);
				PlayerIcon.rectTransform.anchoredPosition = bl_MiniMapUtils.CalculateMiniMapPosition(viewPoint, MapUIRoot);
			}
			if (!Ortographic2D)
			{
				position.y = maxHeight + minHeight / 2f + Target.position.y * 2f;
			}
			else
			{
				position.z = Target.position.z * 2f - (maxHeight + minHeight / 2f);
			}
			m_Transform.position = Vector3.Lerp(m_Transform.position, position, Time.deltaTime * 10f);
		}
		if (plane != null && getDelayPositionCamera)
		{
			Vector3 position2 = plane.transform.position;
			Vector3 position3 = WorldSpace.position;
			float num = defaultYCameraPosition - MMCamera.transform.position.y;
			position2.y = position3.y - num;
			plane.transform.position = position2;
		}
	}

	private void RotationControll()
	{
		RectTransform component = PlayerIcon.GetComponent<RectTransform>();
		if (DynamicRotation && m_MapType != MapType.World)
		{
			Vector3 eulerAngles = m_Transform.eulerAngles;
			eulerAngles.y = Target.eulerAngles.y;
			if (SmoothRotation)
			{
				if (m_Mode == RenderMode.Mode2D)
				{
					component.rotation = Quaternion.identity;
				}
				else
				{
					component.localRotation = Quaternion.identity;
				}
				if (m_Transform.eulerAngles.y != eulerAngles.y)
				{
					float num = eulerAngles.y - m_Transform.eulerAngles.y;
					if (num > 180f || num < -180f)
					{
						m_Transform.eulerAngles = eulerAngles;
					}
				}
				m_Transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, eulerAngles, Time.deltaTime * LerpRotation);
			}
			else
			{
				m_Transform.eulerAngles = eulerAngles;
			}
		}
		else
		{
			m_Transform.eulerAngles = DeafultMapRot;
			if (m_Mode == RenderMode.Mode2D)
			{
				Vector3 zero = Vector3.zero;
				zero.z = 0f - Target.eulerAngles.y;
				component.eulerAngles = zero;
			}
			else
			{
				Vector3 localEulerAngles = Target.localEulerAngles;
				Vector3 zero2 = Vector3.zero;
				zero2.z = 0f - localEulerAngles.y;
				component.localEulerAngles = zero2;
			}
		}
	}

	private void Inputs()
	{
		if (Input.GetKeyDown(ToogleKey))
		{
			ToggleSize();
		}
		if (Input.GetKeyDown(DecreaseHeightKey) && DefaultHeight < maxHeight)
		{
			ChangeHeight(true);
		}
		if (Input.GetKeyDown(IncreaseHeightKey) && DefaultHeight > minHeight)
		{
			ChangeHeight(false);
		}
	}

	private void MapSize()
	{
		RectTransform mMUIRoot = MMUIRoot;
		if (isFullScreen)
		{
			if (DynamicRotation)
			{
				DynamicRotation = false;
				ResetMapRotation();
			}
			mMUIRoot.sizeDelta = Vector2.Lerp(mMUIRoot.sizeDelta, FullMapSize, Time.deltaTime * LerpTransition);
			mMUIRoot.anchoredPosition = Vector3.Lerp(mMUIRoot.anchoredPosition, FullMapPosition, Time.deltaTime * LerpTransition);
			mMUIRoot.localEulerAngles = Vector3.Lerp(mMUIRoot.localEulerAngles, FullMapRotation, Time.deltaTime * LerpTransition);
		}
		else
		{
			if (DynamicRotation != DefaultRotationMode)
			{
				DynamicRotation = DefaultRotationMode;
			}
			mMUIRoot.sizeDelta = Vector2.Lerp(mMUIRoot.sizeDelta, MiniMapSize, Time.deltaTime * LerpTransition);
			mMUIRoot.anchoredPosition = Vector3.Lerp(mMUIRoot.anchoredPosition, MiniMapPosition, Time.deltaTime * LerpTransition);
			mMUIRoot.localEulerAngles = Vector3.Lerp(mMUIRoot.localEulerAngles, MiniMapRotation, Time.deltaTime * LerpTransition);
		}
		MMCamera.orthographicSize = Mathf.Lerp(MMCamera.orthographicSize, DefaultHeight, Time.deltaTime * LerpHeight);
	}

	private void ToggleSize()
	{
		isFullScreen = !isFullScreen;
		if (isFullScreen)
		{
			if (m_MapType != MapType.World)
			{
				DefaultHeight = maxHeight;
			}
			useCompassRotation = false;
			if ((bool)m_maskHelper)
			{
				m_maskHelper.OnChange(true);
			}
		}
		else
		{
			if (m_MapType != MapType.World)
			{
				DefaultHeight = PlayerPrefs.GetFloat("MinimapCameraHeight", DefaultHeight);
			}
			if (useCompassRotation != DefaultRotationCircle)
			{
				useCompassRotation = DefaultRotationCircle;
			}
			if ((bool)m_maskHelper)
			{
				m_maskHelper.OnChange();
			}
		}
		if (ResetOffSetOnChange)
		{
			GoToTarget();
		}
		int value = (isFullScreen ? 1 : 2);
		if (BottonAnimator != null && ShowLevelName)
		{
			if (!BottonAnimator.gameObject.activeSelf)
			{
				BottonAnimator.gameObject.SetActive(true);
			}
			if (BottonAnimator.transform.GetComponentInChildren<Text>() != null)
			{
				BottonAnimator.transform.GetComponentInChildren<Text>().text = LevelName;
			}
			BottonAnimator.SetInteger("state", value);
		}
		else if (BottonAnimator != null)
		{
			BottonAnimator.gameObject.SetActive(false);
		}
		if (PanelInfoAnimator != null && ShowPanelInfo)
		{
			if (!PanelInfoAnimator.gameObject.activeSelf)
			{
				PanelInfoAnimator.gameObject.SetActive(true);
			}
			PanelInfoAnimator.SetInteger("show", value);
		}
		else if (PanelInfoAnimator != null)
		{
			PanelInfoAnimator.gameObject.SetActive(false);
		}
	}

	public void SetDragPosition(Vector3 pos)
	{
		if (!DragOnlyOnFullScreen || isFullScreen)
		{
			DragOffset.x += (0f - pos.x) * DragMovementSpeed.x;
			DragOffset.z += (0f - pos.y) * DragMovementSpeed.y;
			DragOffset.x = Mathf.Clamp(DragOffset.x, 0f - MaxOffSetPosition.x, MaxOffSetPosition.x);
			DragOffset.z = Mathf.Clamp(DragOffset.z, 0f - MaxOffSetPosition.y, MaxOffSetPosition.y);
		}
	}

	public void GoToTarget()
	{
		StopAllCoroutines();
		StartCoroutine(ResetOffset());
	}

	private IEnumerator ResetOffset()
	{
		while (Vector3.Distance(DragOffset, Vector3.zero) > 0.2f)
		{
			DragOffset = Vector3.Lerp(DragOffset, Vector3.zero, Time.deltaTime * LerpTransition);
			yield return null;
		}
		DragOffset = Vector3.zero;
	}

	public void ChangeHeight(bool b)
	{
		if (m_MapType == MapType.World)
		{
			return;
		}
		if (b)
		{
			if (DefaultHeight + (float)scrollSensitivity <= maxHeight)
			{
				DefaultHeight += scrollSensitivity;
			}
			else
			{
				DefaultHeight = maxHeight;
			}
		}
		else if (DefaultHeight - (float)scrollSensitivity >= minHeight)
		{
			DefaultHeight -= scrollSensitivity;
		}
		else
		{
			DefaultHeight = minHeight;
		}
		PlayerPrefs.SetFloat("MinimapCameraHeight", DefaultHeight);
	}

	public void DoHitEffect()
	{
		if (!(HitEffectAnimator == null))
		{
			HitEffectAnimator.speed = HitEffectSpeed;
			HitEffectAnimator.Play("HitEffect", 0, 0f);
		}
	}

	public Material CreateMaterial()
	{
		Material material = new Material(ReferenceMat);
		material.mainTexture = MapTexture;
		material.SetTexture("_EmissionMap", MapTexture);
		material.SetFloat("_EmissionScaleUI", EmissionAmount);
		material.SetColor("_EmissionColor", EmessiveColor);
		material.SetColor("_SpecColor", SpecularColor);
		material.EnableKeyword("_EMISSION");
		return material;
	}

	public bl_MiniMapItem CreateNewItem(bl_MMItemInfo item)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(ItemPrefabSimple, item.Position, Quaternion.identity);
		bl_MiniMapItem component = gameObject.GetComponent<bl_MiniMapItem>();
		if (item.Target != null)
		{
			component.Target = item.Target;
		}
		component.Size = item.Size;
		component.IconColor = item.Color;
		component.isInteractable = item.Interactable;
		component.m_Effect = item.Effect;
		if (item.Sprite != null)
		{
			component.Icon = item.Sprite;
		}
		return component;
	}

	private void ResetMapRotation()
	{
		m_Transform.eulerAngles = new Vector3(90f, 0f, 0f);
	}

	public void RotationMap(bool mode)
	{
		if (!isFullScreen)
		{
			DynamicRotation = mode;
			DefaultRotationMode = DynamicRotation;
		}
	}

	public void ChangeMapSize(bool fullscreen)
	{
		isFullScreen = fullscreen;
	}

	public void SetTarget(GameObject t)
	{
		m_Target = t;
	}

	public void SetMapTexture(Texture t)
	{
		if (m_Type == RenderType.Picture)
		{
			plane.GetComponent<MeshRenderer>().material.mainTexture = t;
		}
	}

	private void GetMiniMapSize()
	{
		MiniMapSize = MMUIRoot.sizeDelta;
		MiniMapPosition = MMUIRoot.anchoredPosition;
		MiniMapRotation = MMUIRoot.eulerAngles;
	}

	[ContextMenu("GetFullMapRect")]
	private void GetFullMapSize()
	{
		FullMapSize = MMUIRoot.sizeDelta;
		FullMapPosition = MMUIRoot.anchoredPosition;
		FullMapRotation = MMUIRoot.eulerAngles;
	}
}
