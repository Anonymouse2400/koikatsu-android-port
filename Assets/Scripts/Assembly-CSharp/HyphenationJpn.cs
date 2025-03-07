using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HyphenationJpn : UIBehaviour
{
	[TextArea(3, 10)]
	[SerializeField]
	private string str;

	private RectTransform _rectTransform;

	private Text _text;

	private static readonly string RITCH_TEXT_REPLACE = "(\\<color=.*\\>|</color>|\\<size=.n\\>|</size>|<b>|</b>|<i>|</i>)";

	private static readonly char[] HYP_FRONT = ",)]｝、。）〕〉》」』】〙〗〟’”｠»ァィゥェォッャュョヮヵヶっぁぃぅぇぉっゃゅょゎ‐゠–〜ー?!！？‼⁇⁈⁉・:;。.".ToCharArray();

	private static readonly char[] HYP_BACK = "(（[｛〔〈《「『【〘〖〝‘“｟«".ToCharArray();

	private static readonly char[] HYP_LATIN = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789<>=/().,".ToCharArray();

	public float textWidth
	{
		get
		{
			return rectTransform.rect.width;
		}
		set
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
		}
	}

	public int fontSize
	{
		get
		{
			return text.fontSize;
		}
		set
		{
			text.fontSize = value;
		}
	}

	private RectTransform rectTransform
	{
		get
		{
			return this.GetComponentCache(ref _rectTransform);
		}
	}

	private Text text
	{
		get
		{
			return this.GetComponentCache(ref _text);
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void CheckHeight()
	{
		if (text.IsActive() && !IsHeightOver(text))
		{
		}
	}

	public void SetText(Text text)
	{
		_text = text;
	}

	public void SetText(string str)
	{
		this.str = str;
		UpdateText(this.str);
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		UpdateText(str);
	}

	private void UpdateText(string str)
	{
		text.text = GetFormatedText(text, str);
	}

	private bool IsHeightOver(Text textComp)
	{
		return textComp.preferredHeight > rectTransform.rect.height;
	}

	private bool IsLineCountOver(Text textComp, int lineCount)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < lineCount; i++)
		{
			stringBuilder.Append("\n");
		}
		textComp.text = stringBuilder.ToString();
		return textComp.preferredHeight > rectTransform.rect.height;
	}

	private float GetSpaceWidth(Text textComp)
	{
		float num = GetTextWidth(textComp, "m m");
		float num2 = GetTextWidth(textComp, "mm");
		return num - num2;
	}

	private float GetTextWidth(Text textComp, string message)
	{
		if (_text.supportRichText)
		{
			message = Regex.Replace(message, RITCH_TEXT_REPLACE, string.Empty);
		}
		textComp.text = message;
		return textComp.preferredWidth;
	}

	private string GetFormatedText(Text textComp, string msg)
	{
		if (string.IsNullOrEmpty(msg))
		{
			return string.Empty;
		}
		float width = rectTransform.rect.width;
		float spaceWidth = GetSpaceWidth(textComp);
		textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		float num2 = 0f;
		string text = "\n";
		bool flag = CHECK_HYP_BACK(msg[0]);
		bool flag2 = false;
		foreach (string word in GetWordList(msg))
		{
			float num3 = GetTextWidth(textComp, word);
			num2 += num3;
			if (word == text)
			{
				num2 = 0f;
				num2 += spaceWidth * 2f;
				num++;
			}
			else
			{
				if (word == " ")
				{
					num2 += spaceWidth;
				}
				if (flag)
				{
					if (!flag2)
					{
						flag2 = IsLineCountOver(textComp, num + 1);
					}
					if (flag2)
					{
						num3 = 0f;
					}
					if (num2 > width - num3)
					{
						stringBuilder.Append(text);
						stringBuilder.Append("\u3000");
						num2 = GetTextWidth(textComp, word);
						num2 += spaceWidth * 2f;
						num++;
					}
				}
				else if (num2 > width)
				{
					stringBuilder.Append(text);
					num2 = GetTextWidth(textComp, word);
					num++;
				}
			}
			stringBuilder.Append(word);
		}
		return stringBuilder.ToString();
	}

	private List<string> GetWordList(string tmpText)
	{
		List<string> list = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		for (int i = 0; i < tmpText.Length; i++)
		{
			char c2 = tmpText[i];
			char s = ((i >= tmpText.Length - 1) ? c : tmpText[i + 1]);
			char c3 = ((i <= 0) ? c : (c3 = tmpText[i - 1]));
			stringBuilder.Append(c2);
			if ((IsLatin(c2) && IsLatin(c3) && IsLatin(c2) && !IsLatin(c3)) || (!IsLatin(c2) && CHECK_HYP_BACK(c3)) || (!IsLatin(s) && !CHECK_HYP_FRONT(s) && !CHECK_HYP_BACK(c2)) || i == tmpText.Length - 1)
			{
				list.Add(stringBuilder.ToString());
				stringBuilder = new StringBuilder();
			}
		}
		return list;
	}

	private static bool CHECK_HYP_FRONT(char str)
	{
		return Array.Exists(HYP_FRONT, (char item) => item == str);
	}

	private static bool CHECK_HYP_BACK(char str)
	{
		return Array.Exists(HYP_BACK, (char item) => item == str);
	}

	private static bool IsLatin(char s)
	{
		return Array.Exists(HYP_LATIN, (char item) => item == s);
	}
}
