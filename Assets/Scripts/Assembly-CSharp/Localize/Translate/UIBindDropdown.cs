using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	[DefaultExecutionOrder(-1)]
	public class UIBindDropdown : BaseLoader
	{
		[SerializeField]
		private int sceneID = -1;

		[SerializeField]
		private int category = -1;

		private Dictionary<int, Data.Param> dic;

		private bool BindTMP_Dropdown()
		{
			TMP_Dropdown component = GetComponent<TMP_Dropdown>();
			if (component == null)
			{
				return false;
			}
			foreach (var item in component.options.Select((TMP_Dropdown.OptionData p, int i) => new { p, i }))
			{
				dic.SafeGetText(item.i).SafeProc(delegate(string text)
				{
					item.p.text = text;
				});
			}
			component.captionText.text = component.options[component.value].text;
			return true;
		}

		private bool BindUnity_Dropdown()
		{
			Dropdown component = GetComponent<Dropdown>();
			if (component == null)
			{
				return false;
			}
			foreach (var item in component.options.Select((Dropdown.OptionData p, int i) => new { p, i }))
			{
				dic.SafeGetText(item.i).SafeProc(delegate(string text)
				{
					item.p.text = text;
				});
			}
			component.captionText.text = component.options[component.value].text;
			return true;
		}

		private void Load()
		{
			Manager.LoadScene(sceneID, base.gameObject);
			dic = Manager.GetCategory(sceneID, category);
			if (!BindTMP_Dropdown() && !BindUnity_Dropdown())
			{
			}
		}

		private void Unload()
		{
			if (dic != null)
			{
				Manager.Unload(dic.Values);
				dic = null;
			}
		}

		private void Start()
		{
			Load();
		}

		private void OnDestroy()
		{
			Unload();
		}
	}
}
