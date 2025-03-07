using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Localize.Translate
{
	public class TextChangable : MonoBehaviour, IHTextChangable
	{
		[SerializeField]
		private int sceneID = -1;

		[SerializeField]
		private int category = -1;

		[SerializeField]
		private string tagName = "group";

		[SerializeField]
		private int startIndex = -1;

		[SerializeField]
		private TMP_Text text;

		private Dictionary<int, Dictionary<int, Data.Param>> _categorys;

		private Dictionary<int, Data.Param> _data;

		private string[] _textTable;

		private Dictionary<int, Dictionary<int, Data.Param>> categorys
		{
			get
			{
				return this.GetCache(ref _categorys, () => _categorys = Manager.LoadScene(sceneID, base.gameObject));
			}
		}

		private Dictionary<int, Data.Param> data
		{
			get
			{
				return this.GetCache(ref _data, () => categorys.Get(category));
			}
		}

		private string[] textTable
		{
			get
			{
				return this.GetCache(ref _textTable, () => Load(tagName));
			}
		}

		public void OnChange(int num)
		{
			textTable.SafeProc(num, delegate(string s)
			{
				text.text = s;
			});
		}

		private void Start()
		{
			if (text == null)
			{
				text = GetComponentInChildren<TMP_Text>(true);
			}
			if (text == null)
			{
				Object.Destroy(this);
				return;
			}
			Manager.BindFont(text);
			OnChange(startIndex);
		}

		private string[] Load(string tagName)
		{
			return data.Values.ToArray(tagName);
		}
	}
}
