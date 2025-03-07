using System;
using System.Linq;
using Illusion.Extensions;
using Localize.Translate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsCharaEx : MonoBehaviour
	{
		[SerializeField]
		private GameObject objName;

		[SerializeField]
		private GameObject objNickName;

		[SerializeField]
		private GameObject objPersonality;

		[SerializeField]
		private GameObject objBloodType;

		[SerializeField]
		private GameObject objBirthday;

		[SerializeField]
		private GameObject objClub;

		private GameObject[] _array;

		private GameObject[] array
		{
			get
			{
				return this.GetCache(ref _array, () => new GameObject[6] { objName, objNickName, objPersonality, objBloodType, objBirthday, objClub });
			}
		}

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return customBase.chaCtrl;
			}
		}

		private ChaFileParameter param
		{
			get
			{
				return chaCtrl.chaFile.parameter;
			}
		}

		private static TextMeshProUGUI GetTitle(GameObject obj)
		{
			return obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		}

		private static TextMeshProUGUI GetSep(GameObject obj)
		{
			return obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
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
				anchoredPosition.x = num + 12f;
				item.rectTransform.anchoredPosition = anchoredPosition;
			}
		}

		public void CalculateUI()
		{
			SetSep(this.array);
			float value = this.array.Select(GetSep).Max((TextMeshProUGUI p) => p.rectTransform.anchoredPosition.x + p.rectTransform.sizeDelta.x);
			SetLabel(objName, param.fullname, value);
			SetLabel(objNickName, param.nickname, value);
			string[] array = customBase.dictPersonality.Values.ToArray();
			int num = Array.IndexOf(customBase.dictPersonality.Keys.ToArray(), param.personality);
			SetLabel(objPersonality, array[(num != -1) ? num : 0], value);
			SetLabel(objBloodType, ChaFileDefine.GetBloodTypeStr(param.bloodType), value);
			SetLabel(objBirthday, param.strBirthDay, value);
			SetLabel(objClub, Localize.Translate.Manager.GetClubName(param.clubActivities, true), value);
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		protected virtual void Start()
		{
			customBase.actUpdateCvsChara += UpdateCustomUI;
		}
	}
}
