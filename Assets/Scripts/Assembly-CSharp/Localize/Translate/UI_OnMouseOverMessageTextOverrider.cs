using System.Collections.Generic;
using UnityEngine;

namespace Localize.Translate
{
	public class UI_OnMouseOverMessageTextOverrider : MonoBehaviour
	{
		[SerializeField]
		private int sceneID = -1;

		[SerializeField]
		private int category = -1;

		[SerializeField]
		private string tagName;

		private Dictionary<int, Dictionary<int, Data.Param>> _categorys;

		private Dictionary<int, Data.Param> _translater;

		private List<Data.Param> useList = new List<Data.Param>();

		private Dictionary<int, Dictionary<int, Data.Param>> categorys
		{
			get
			{
				return this.GetCache(ref _categorys, () => _categorys = Manager.LoadScene(sceneID, base.gameObject));
			}
		}

		private Dictionary<int, Data.Param> translater
		{
			get
			{
				return this.GetCache(ref _translater, () => categorys.Get(category));
			}
		}

		private void Start()
		{
			UI_OnMouseOverMessage component = GetComponent<UI_OnMouseOverMessage>();
			if (component == null)
			{
				return;
			}
			Data.Param param = translater.Values.FindTag(tagName);
			if (param != null)
			{
				if (!param.text.IsNullOrEmpty())
				{
					component.comment = param.text;
				}
				Manager.Bind(component.txtComment, param, false);
			}
			useList.Add(param);
		}

		private void OnDestroy()
		{
			Manager.Unload(useList);
		}
	}
}
