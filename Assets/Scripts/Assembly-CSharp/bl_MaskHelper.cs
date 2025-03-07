using UnityEngine;
using UnityEngine.UI;

public class bl_MaskHelper : MonoBehaviour
{
	[Header("Mask")]
	public Sprite MiniMapMask;

	public Sprite WorldMapMask;

	[Header("References")]
	[SerializeField]
	private Image Background;

	[SerializeField]
	private Sprite MiniMapBackGround;

	[SerializeField]
	private Sprite WorldMapBackGround;

	[SerializeField]
	private RectTransform MaskIconRoot;

	private Image _image;

	private Image m_image
	{
		get
		{
			if (_image == null)
			{
				_image = GetComponent<Image>();
			}
			return _image;
		}
	}

	private void Start()
	{
		m_image.sprite = MiniMapMask;
	}

	public void OnChange(bool full = false)
	{
		if (full)
		{
			m_image.sprite = WorldMapMask;
			if (Background != null)
			{
				Background.sprite = WorldMapBackGround;
			}
		}
		else
		{
			m_image.sprite = MiniMapMask;
			if (Background != null)
			{
				Background.sprite = MiniMapBackGround;
			}
		}
	}

	public void SetMaskedIcon(RectTransform trans)
	{
		trans.SetParent(MaskIconRoot);
	}
}
