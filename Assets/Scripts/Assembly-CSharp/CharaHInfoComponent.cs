using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using Localize.Translate;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CharaHInfoComponent : MonoBehaviour
{
	private const float drawH = 272f;

	private readonly string[] strWeakPoint = new string[7] { "なし", "唇", "胸", "股間", "アナル", "尻", "乳首" };

	[SerializeField]
	private RectTransform rtfResize;

	[SerializeField]
	private float drawHeight = 272f;

	[SerializeField]
	private bool updateSize = true;

	[SerializeField]
	private Image baseImage;

	[SerializeField]
	private RawImage riChara;

	[SerializeField]
	private Text textName;

	[SerializeField]
	private TextMeshProUGUI textPersonality;

	[SerializeField]
	private TextMeshProUGUI textWeakPoint;

	[SerializeField]
	private TextMeshProUGUI[] textDenial = new TextMeshProUGUI[5];

	[Button("Initialize", "設定の初期化(男)", new object[] { 0 })]
	[Space]
	public int initializeM;

	[Button("Initialize", "設定の初期化(女)", new object[] { 1 })]
	public int initializeF;

	private Dictionary<int, Data.Param> _weakLT;

	public Image BaseImage
	{
		get
		{
			return baseImage;
		}
	}

	private Dictionary<int, Data.Param> weakLT
	{
		get
		{
			return this.GetCache(ref _weakLT, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.CUSTOM_UI).Get(1002));
		}
	}

	public void SetDrawSize(float height = 272f)
	{
		drawHeight = height;
		updateSize = true;
	}

	public void Clear()
	{
		if ((bool)riChara)
		{
			if ((bool)riChara.texture)
			{
				UnityEngine.Object.Destroy(riChara.texture);
			}
			riChara.texture = null;
			riChara.enabled = false;
		}
		if ((bool)textName)
		{
			textName.text = string.Empty;
		}
		if ((bool)textPersonality)
		{
			textPersonality.text = string.Empty;
		}
		if ((bool)textWeakPoint)
		{
			textWeakPoint.text = string.Empty;
		}
		if (textDenial == null)
		{
			return;
		}
		for (int i = 0; i < textDenial.Length; i++)
		{
			if (null != textDenial[i])
			{
				textDenial[i].text = string.Empty;
			}
		}
	}

	public void SetCharaInfo(ChaFileControl chaFileCtrl)
	{
		if (chaFileCtrl == null)
		{
			return;
		}
		if ((bool)riChara)
		{
			if (!riChara.enabled)
			{
				riChara.enabled = true;
			}
			if ((bool)riChara.texture)
			{
				UnityEngine.Object.Destroy(riChara.texture);
			}
			riChara.texture = null;
			riChara.enabled = false;
			if (chaFileCtrl.pngData != null)
			{
				Texture2D texture2D = new Texture2D(240, 320);
				texture2D.LoadImage(chaFileCtrl.pngData);
				riChara.texture = texture2D;
				riChara.enabled = true;
			}
		}
		ChaFileParameter param = chaFileCtrl.parameter;
		if ((bool)textName)
		{
			textName.text = param.fullname;
		}
		if (param.sex == 0)
		{
			return;
		}
		Localize.Translate.Manager.BindFont(textPersonality);
		if ((bool)textPersonality)
		{
			VoiceInfo.Param[] source = Singleton<Voice>.Instance.voiceInfoDic.Values.Where((VoiceInfo.Param x) => 0 <= x.No).ToArray();
			int[] array = source.Select((VoiceInfo.Param x) => x.No).ToArray();
			string[] array2 = source.Select((VoiceInfo.Param x) => x.Personality).ToArray();
			int personality = param.personality;
			VoiceInfo.Param value;
			if (!Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(personality, out value))
			{
				Singleton<Voice>.Instance.voiceInfoDic.TryGetValue(0, out value);
			}
			int num = Array.IndexOf(array, value.No);
			if (num != -1)
			{
				textPersonality.text = array2[num];
			}
		}
		if ((bool)textWeakPoint)
		{
			int num2 = param.weakPoint + 1;
			if (!Localize.Translate.Manager.Bind(textWeakPoint, weakLT.Get(num2), true))
			{
				textWeakPoint.text = strWeakPoint[num2];
			}
		}
		Data.Param[] answerData = Localize.Translate.Manager.OtherData.Get(0).Values.FindTags("Answer").ToArray();
		string[] answers = new string[2] { "YES", "NO" };
		Action<TMP_Text, bool> bind = delegate(TMP_Text text, bool isOn)
		{
			int num3 = ((!isOn) ? 1 : 0);
			if (!answerData.SafeProc(num3, delegate(Data.Param answer)
			{
				Localize.Translate.Manager.Bind(text, answer, true);
			}))
			{
				text.text = answers[num3];
			}
		};
		int num4 = 0;
		textDenial.SafeProc(num4++, delegate(TextMeshProUGUI text)
		{
			bind(text, param.denial.kiss);
		});
		textDenial.SafeProc(num4++, delegate(TextMeshProUGUI text)
		{
			bind(text, param.denial.aibu);
		});
		textDenial.SafeProc(num4++, delegate(TextMeshProUGUI text)
		{
			bind(text, param.denial.anal);
		});
		textDenial.SafeProc(num4++, delegate(TextMeshProUGUI text)
		{
			bind(text, param.denial.massage);
		});
		textDenial.SafeProc(num4++, delegate(TextMeshProUGUI text)
		{
			bind(text, param.denial.notCondom);
		});
	}

	public void Initialize(int sex)
	{
		Transform transform = base.transform.Find("resize");
		if ((bool)transform)
		{
			rtfResize = transform as RectTransform;
		}
		transform = base.transform.Find("resize/image/base");
		if ((bool)transform)
		{
			baseImage = transform.GetComponent<Image>();
		}
		transform = base.transform.Find("resize/image/chara");
		if ((bool)transform)
		{
			riChara = transform.GetComponent<RawImage>();
		}
		textName = transform.GetComponent<Text>();
		transform = base.transform.Find("resize/text/name");
		if ((bool)transform)
		{
			textName = transform.GetComponent<Text>();
		}
		if (sex == 0)
		{
			textPersonality = null;
			textWeakPoint = null;
			textDenial = null;
			return;
		}
		transform = base.transform.Find("resize/text/personality");
		if ((bool)transform)
		{
			textPersonality = transform.GetComponent<TextMeshProUGUI>();
		}
		transform = base.transform.Find("resize/text/weakPoint");
		if ((bool)transform)
		{
			textWeakPoint = transform.GetComponent<TextMeshProUGUI>();
		}
		textDenial = new TextMeshProUGUI[5];
		transform = base.transform.Find("resize/text/denial01");
		if ((bool)transform)
		{
			textDenial[0] = transform.GetComponent<TextMeshProUGUI>();
		}
		transform = base.transform.Find("resize/text/denial02");
		if ((bool)transform)
		{
			textDenial[1] = transform.GetComponent<TextMeshProUGUI>();
		}
		transform = base.transform.Find("resize/text/denial03");
		if ((bool)transform)
		{
			textDenial[2] = transform.GetComponent<TextMeshProUGUI>();
		}
		transform = base.transform.Find("resize/text/denial04");
		if ((bool)transform)
		{
			textDenial[3] = transform.GetComponent<TextMeshProUGUI>();
		}
		transform = base.transform.Find("resize/text/denial05");
		if ((bool)transform)
		{
			textDenial[4] = transform.GetComponent<TextMeshProUGUI>();
		}
	}

	private void Reset()
	{
		Initialize(1);
	}

	private void Update()
	{
		if (updateSize && null != rtfResize)
		{
			float num = drawHeight / 272f;
			rtfResize.localScale = new Vector3(num, num, 1f);
			updateSize = false;
		}
	}
}
