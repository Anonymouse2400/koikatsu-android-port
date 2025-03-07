using System.Collections;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DownloadCharaInfoViewer : MonoBehaviour
{
	public class Data
	{
		public string name;

		public string nickname;

		public string personality;

		public string birthday;

		public string blood;

		public string club;

		public string HN;

		public string comment;

		public static Data Empty()
		{
			Data data = new Data();
			data.name = string.Empty;
			data.nickname = string.Empty;
			data.personality = string.Empty;
			data.birthday = string.Empty;
			data.blood = string.Empty;
			data.club = string.Empty;
			data.HN = string.Empty;
			data.comment = string.Empty;
			return data;
		}
	}

	[SerializeField]
	private GameObject objName;

	[SerializeField]
	private GameObject objNickName;

	[SerializeField]
	private GameObject objPersonality;

	[SerializeField]
	private GameObject objBirthday;

	[SerializeField]
	private GameObject objBloodType;

	[SerializeField]
	private GameObject objClub;

	[SerializeField]
	private GameObject objHN;

	[SerializeField]
	private GameObject objComment;

	private ReactiveProperty<bool> _isFemale = new ReactiveProperty<bool>(true);

	private ReactiveProperty<Data> property = new ReactiveProperty<Data>();

	public bool isFemale
	{
		get
		{
			return _isFemale.Value;
		}
		set
		{
			_isFemale.Value = value;
		}
	}

	public Data data
	{
		get
		{
			return property.Value;
		}
		set
		{
			property.Value = value;
		}
	}

	private static TextMeshProUGUI GetTitle(GameObject obj)
	{
		return obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	private static TextMeshProUGUI GetSep(GameObject obj)
	{
		return obj.Children().Find((GameObject p) => p.name == "Sep").GetComponent<TextMeshProUGUI>();
	}

	private static void SetLabel(GameObject obj, string title, float? space = null)
	{
		title = "  " + title;
		RectTransform rectTransform = obj.transform.Children().LastOrDefault() as RectTransform;
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		anchoredPosition.x = ((!space.HasValue) ? GetTitle(obj).preferredWidth : space.Value);
		rectTransform.anchoredPosition = anchoredPosition;
		TextMeshProUGUI component = rectTransform.GetComponent<TextMeshProUGUI>();
		if (component != null)
		{
			component.text = title;
			return;
		}
		Text component2 = rectTransform.GetComponent<Text>();
		if (component2 != null)
		{
			component2.text = title;
		}
	}

	private static void SetSep(params GameObject[] objects)
	{
		TextMeshProUGUI[] source = objects.Select(GetTitle).ToArray();
		float num = source.Max((TextMeshProUGUI item) => item.preferredWidth);
		foreach (TextMeshProUGUI item in objects.Select(GetSep))
		{
			Vector2 anchoredPosition = item.rectTransform.anchoredPosition;
			anchoredPosition.x = num + 8f;
			item.rectTransform.anchoredPosition = anchoredPosition;
		}
	}

	private IEnumerator Start()
	{
		_isFemale.Subscribe(delegate(bool female)
		{
			objNickName.SetActiveIfDifferent(female);
			objPersonality.SetActiveIfDifferent(female);
			property.Value = null;
		});
		while (property.Value == null)
		{
			yield return null;
		}
		GameObject[] array = new GameObject[5] { objNickName, objPersonality, objBloodType, objBirthday, objClub };
		SetSep(objName);
		SetSep(array);
		TextMeshProUGUI sep = GetSep(objName);
		float nameWidth = sep.rectTransform.anchoredPosition.x + sep.rectTransform.sizeDelta.x;
		float maxWidth = array.Select(GetSep).Max((TextMeshProUGUI p) => p.rectTransform.anchoredPosition.x + p.rectTransform.sizeDelta.x);
		Text hnText = objHN.GetComponentInChildren<Text>();
		Text commentText = objComment.GetComponentInChildren<Text>();
		property.Subscribe(delegate(Data value)
		{
			foreach (GameObject item in from o in array.Concat(new GameObject[1] { objName })
				select GetSep(o).gameObject)
			{
				item.SetActiveIfDifferent(value != null);
			}
			Data data = value ?? Data.Empty();
			SetLabel(objName, data.name, nameWidth);
			SetLabel(objNickName, data.nickname, maxWidth);
			SetLabel(objBloodType, data.blood, maxWidth);
			SetLabel(objBirthday, data.birthday, maxWidth);
			SetLabel(objPersonality, data.personality, maxWidth);
			SetLabel(objClub, data.club, maxWidth);
			hnText.text = data.HN;
			commentText.text = data.comment;
		});
	}
}
