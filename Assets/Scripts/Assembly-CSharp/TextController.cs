using Localize.Translate;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
	[SerializeField]
	private Text nameWindow;

	[SerializeField]
	private Text messageWindow;

	private const int MAX_FONT_SPEED = 100;

	[SerializeField]
	[RangeReactiveProperty(1f, 100f)]
	private IntReactiveProperty _fontSpeed = new IntReactiveProperty(100);

	private ColorReactiveProperty _fontColor = new ColorReactiveProperty(Color.white);

	private TypefaceAnimatorEx TA;

	private HyphenationJpn hypJpn;

	public bool isMovie;

	[SerializeField]
	private float movieFontSpeed = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float movieProgress;

	public bool initialized { get; private set; }

	public bool IsCompleteDisplayText
	{
		get
		{
			return isMovie ? (movieProgress >= 1f) : (!TA.isPlaying);
		}
	}

	public bool NameMessageVisible
	{
		get
		{
			return nameVisible && messageVisible;
		}
		set
		{
			nameVisible = value;
			messageVisible = value;
		}
	}

	public bool nameVisible
	{
		get
		{
			return !(nameWindow == null) && nameWindow.enabled;
		}
		set
		{
			nameWindow.SafeProc(delegate(Text p)
			{
				p.enabled = value;
			});
		}
	}

	public bool messageVisible
	{
		get
		{
			return !(messageWindow == null) && messageWindow.enabled;
		}
		set
		{
			messageWindow.SafeProc(delegate(Text p)
			{
				p.enabled = value;
			});
		}
	}

	public Text NameWindow
	{
		get
		{
			return nameWindow;
		}
	}

	public Text MessageWindow
	{
		get
		{
			return messageWindow;
		}
	}

	public int FontSpeed
	{
		get
		{
			return _fontSpeed.Value;
		}
		set
		{
			_fontSpeed.Value = Mathf.Clamp(value, 1, 100);
		}
	}

	public Color FontColor
	{
		get
		{
			return _fontColor.Value;
		}
		set
		{
			_fontColor.Value = value;
		}
	}

	public void Change(Text nameWindow, Text messageWindow)
	{
		Clear();
		this.nameWindow = nameWindow;
		this.messageWindow = messageWindow;
		Object.Destroy(hypJpn);
		Initialize();
	}

	public void Clear()
	{
		if (!initialized)
		{
			Initialize();
		}
		nameWindow.SafeProc(delegate(Text p)
		{
			p.text = string.Empty;
		});
		messageWindow.text = string.Empty;
		if (hypJpn != null)
		{
			hypJpn.SetText(string.Empty);
		}
		TA.Stop();
		TA.progress = 0f;
		movieProgress = 0f;
	}

	public void Set(string nameText, string messageText)
	{
		nameWindow.SafeProc(delegate(Text p)
		{
			p.text = nameText;
		});
		messageWindow.text = messageText;
		if (hypJpn != null)
		{
			hypJpn.SetText(messageWindow.text);
		}
		TA.Play();
		movieProgress = 0f;
	}

	public void ForceCompleteDisplayText()
	{
		TA.progress = 1f;
	}

	public void Initialize()
	{
		hypJpn = messageWindow.GetComponent<HyphenationJpn>();
		if (Localize.Translate.Manager.isTranslate)
		{
			Object.Destroy(hypJpn);
			hypJpn = null;
		}
		else if (hypJpn == null)
		{
			hypJpn = messageWindow.gameObject.AddComponent<HyphenationJpn>();
		}
		if (hypJpn != null)
		{
			hypJpn.SetText(messageWindow);
		}
		TA = messageWindow.GetComponent<TypefaceAnimatorEx>();
		initialized = true;
	}

	private void Awake()
	{
		if (!initialized)
		{
			Initialize();
		}
	}

	private void Start()
	{
		_fontSpeed.TakeUntilDestroy(this).Subscribe(delegate(int value)
		{
			TA.isNoWait = value == 100;
			if (!TA.isNoWait)
			{
				TA.timeMode = TypefaceAnimatorEx.TimeMode.Speed;
				TA.speed = value;
			}
		});
		_fontColor.TakeUntilDestroy(this).Subscribe(delegate(Color color)
		{
			nameWindow.SafeProc(delegate(Text p)
			{
				p.color = color;
			});
			messageWindow.SafeProc(delegate(Text p)
			{
				p.color = color;
			});
		});
		(from _ in this.UpdateAsObservable()
			where base.enabled
			where isMovie
			select _).Subscribe(delegate
		{
			movieProgress = Mathf.Min(movieProgress + Time.deltaTime / movieFontSpeed, 1f);
		});
	}
}
