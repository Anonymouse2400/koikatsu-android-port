using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : SimpleFade
{
	private Canvas _canvas;

	private RectTransform canvasRt;

	private Image image;

	private int sortingOrder = -32768;

	private bool _isFadeProc;

	public Canvas canvas
	{
		get
		{
			return this.GetCache(ref _canvas, () => image.canvas);
		}
	}

	private bool isFadeProc
	{
		get
		{
			return _isFadeProc;
		}
		set
		{
			_isFadeProc = value;
			base.gameObject.SetActive(_isFadeProc);
			canvas.gameObject.SetActive(_isFadeProc);
		}
	}

	public void SortingOrder()
	{
		Canvas[] array = (from p in Object.FindObjectsOfType<Canvas>()
			where p != canvas
			select p).ToArray();
		if (array.Length > 0)
		{
			canvas.sortingOrder = array.Max((Canvas p) => p.sortingOrder) + 1;
		}
		else
		{
			canvas.sortingOrder = sortingOrder;
		}
	}

	public override void Init()
	{
		isFadeProc = true;
		base.Init();
	}

	public override void ForceEnd()
	{
		base.ForceEnd();
		image.color = _Color;
		if (_Fade == Fade.Out)
		{
			isFadeProc = false;
		}
	}

	protected void OnEnable()
	{
		SortingOrder();
	}

	protected override void Awake()
	{
		image = GetComponent<Image>();
		base.Awake();
		canvasRt = canvas.transform as RectTransform;
		image.rectTransform.sizeDelta = new Vector2(canvasRt.rect.width, canvasRt.rect.height);
		sortingOrder = canvas.sortingOrder;
		canvas.gameObject.SetActive(false);
		image.color = _Color;
	}

	protected override void Update()
	{
		if (!isFadeProc)
		{
			return;
		}
		if (_Texture != null)
		{
			if (image.sprite == null)
			{
				image.sprite = Sprite.Create(_Texture, new Rect(0f, 0f, _Texture.width, _Texture.height), image.rectTransform.pivot);
			}
		}
		else if (image.sprite != null)
		{
			Object.Destroy(image.sprite);
			image.sprite = null;
		}
		base.Update();
		image.color = _Color;
		image.rectTransform.sizeDelta = new Vector2(canvasRt.rect.width, canvasRt.rect.height);
		if (!base.IsFadeNow)
		{
			isFadeProc = false;
		}
	}
}
