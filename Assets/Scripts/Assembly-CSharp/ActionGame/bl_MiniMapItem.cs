using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class bl_MiniMapItem : MonoBehaviour
	{
		[Tooltip("UI Prefab")]
		[Separator("TARGET")]
		public GameObject GraphicPrefab;

		[Tooltip("Transform to UI Icon will be follow")]
		public Transform Target;

		[Tooltip("Custom Position from target position")]
		public Vector3 OffSet = Vector3.zero;

		[Separator("ICON")]
		public Sprite Icon;

		public Color IconColor = new Color(1f, 1f, 1f, 0.9f);

		[Range(1f, 100f)]
		public float Size = 20f;

		[Tooltip("UI can interact when press it?")]
		[Separator("ICON BUTTON")]
		[CustomToggle("is Interactable")]
		public bool isInteractable = true;

		[CustomToggle("Off Screen")]
		[Tooltip("Can Icon show when is off screen?")]
		[Separator("SETTINGS")]
		public bool OffScreen = true;

		[Range(0f, 5f)]
		public float BorderOffScreen = 0.01f;

		[Range(1f, 50f)]
		public float OffScreenSize = 10f;

		[Tooltip("Time before render/show item in minimap after instance")]
		[Range(0f, 3f)]
		public float RenderDelay = 0.3f;

		public ItemEffect m_Effect = ItemEffect.None;

		private bl_MiniMap MiniMap;

		private RectTransform RectRoot;

		private GameObject cacheItem;

		private RectTransform graphicRectTransform;

		private bl_IconItem _iconItem;

		private static int TypeHash = Animator.StringToHash("Type");

		private bl_MiniMap _minimap;

		public bl_IconItem iconItem
		{
			get
			{
				return _iconItem;
			}
		}

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

		public void SetVisible(bool visible)
		{
			_iconItem.visible = visible;
		}

		private IEnumerator Start()
		{
			base.enabled = false;
			yield return new WaitWhile(() => !Singleton<Game>.IsInstance());
			yield return new WaitWhile(() => Singleton<Game>.Instance.actScene == null);
			MiniMap = Singleton<Game>.Instance.actScene.miniMap;
			if (bl_MiniMap.MapUIRoot != null)
			{
				CreateIcon();
			}
			base.enabled = true;
		}

		private void CreateIcon()
		{
			RectRoot = MiniMap.MMUIRoot;
			cacheItem = Object.Instantiate(GraphicPrefab, RectRoot, false);
			cacheItem.name = GraphicPrefab.name;
			_iconItem = cacheItem.GetComponent<bl_IconItem>();
			SetVisible(false);
			Image graphic = _iconItem.graphic;
			graphicRectTransform = graphic.rectTransform;
			graphicRectTransform.anchoredPosition = Vector2.zero;
			if (Icon != null)
			{
				graphic.sprite = Icon;
				graphic.color = IconColor;
			}
			_iconItem.canvasGroup.interactable = isInteractable;
			if (Target == null)
			{
				Target = base.transform;
			}
			StartEffect();
			_iconItem.DelayStart(RenderDelay);
		}

		private void Update()
		{
			Vector3 position = TargetPosition + OffSet;
			Vector2 vector = bl_MiniMap.MiniMapCamera.WorldToViewportPoint(position);
			Vector3 vector2 = new Vector2(vector.x * RectRoot.sizeDelta.x - RectRoot.sizeDelta.x * 0.5f, vector.y * RectRoot.sizeDelta.y - RectRoot.sizeDelta.y * 0.5f);
			if (OffScreen)
			{
				vector2.x = Mathf.Clamp(vector2.x, 0f - (RectRoot.sizeDelta.x * 0.5f - BorderOffScreen), RectRoot.sizeDelta.x * 0.5f - BorderOffScreen);
				vector2.y = Mathf.Clamp(vector2.y, 0f - (RectRoot.sizeDelta.y * 0.5f - BorderOffScreen), RectRoot.sizeDelta.y * 0.5f - BorderOffScreen);
			}
			float size = Size;
			if (!m_miniMap.useCompassRotation)
			{
				size = ((vector2.x != RectRoot.sizeDelta.x * 0.5f - BorderOffScreen && vector2.y != RectRoot.sizeDelta.y * 0.5f - BorderOffScreen && vector2.x != 0f - RectRoot.sizeDelta.x * 0.5f - BorderOffScreen && 0f - vector2.y != RectRoot.sizeDelta.y * 0.5f - BorderOffScreen) ? Size : OffScreenSize);
			}
			else
			{
				Vector3 zero = Vector3.zero;
				Vector3 direction = Target.position - m_miniMap.TargetPosition;
				Vector3 vector3 = bl_MiniMap.MiniMapCamera.transform.InverseTransformDirection(direction);
				vector3.z = 0f;
				vector3 = vector3.normalized / 2f;
				float num = Mathf.Abs(vector2.x);
				float num2 = Mathf.Abs(0.5f + vector3.x * m_miniMap.CompassSize);
				if (num >= num2)
				{
					zero.x = 0.5f + vector3.x * m_miniMap.CompassSize;
					zero.y = 0.5f + vector3.y * m_miniMap.CompassSize;
					vector2 = zero;
					size = OffScreenSize;
				}
				else
				{
					size = Size;
				}
			}
			RectTransform rectTransform = graphicRectTransform;
			rectTransform.anchoredPosition = vector2;
			float num3 = size * MiniMap.IconMultiplier;
			rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(num3, num3), Time.deltaTime * 8f);
			if (MiniMap.RotationAlwaysFront)
			{
				Quaternion identity = Quaternion.identity;
				identity.x = Target.rotation.x;
				rectTransform.localRotation = identity;
			}
			else
			{
				Vector3 eulerAngles = MiniMap.transform.eulerAngles;
				Vector3 zero2 = Vector3.zero;
				zero2.z = 0f - Target.rotation.eulerAngles.y + eulerAngles.y;
				Quaternion rotation = Quaternion.Euler(zero2);
				rectTransform.rotation = rotation;
			}
		}

		private void StartEffect()
		{
			Animator animator = _iconItem.animator;
			if (!(animator == null))
			{
				switch (m_Effect)
				{
				case ItemEffect.Pulsing:
					animator.SetInteger(TypeHash, 2);
					break;
				case ItemEffect.Fade:
					animator.SetInteger(TypeHash, 1);
					break;
				}
			}
		}

		public void DestroyItem()
		{
			if (!(_iconItem == null))
			{
				_iconItem.DestroyIcon();
			}
		}

		public void SetIcon(Sprite ico)
		{
			_iconItem.SetIcon(ico);
		}

		private void OnDestroy()
		{
			DestroyItem();
		}
	}
}
