using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	[Serializable]
	public class CharaFileSort
	{
		public Transform root;

		public Image[] imageSort;

		public Sprite[] spriteSort;

		public List<CharaFileInfo> cfiList = new List<CharaFileInfo>();

		private int m_Select = -1;

		private bool[] sortType = new bool[2] { true, true };

		public int select
		{
			get
			{
				return m_Select;
			}
			set
			{
				int num = m_Select;
				if (Utility.SetStruct(ref m_Select, value))
				{
					if (MathfEx.RangeEqualOn(0, num, cfiList.Count))
					{
						cfiList[num].select = false;
					}
					if (MathfEx.RangeEqualOn(0, m_Select, cfiList.Count))
					{
						cfiList[m_Select].select = true;
					}
				}
			}
		}

		public int sortKind { get; private set; }

		public string selectPath
		{
			get
			{
				if (cfiList.Count == 0)
				{
					return string.Empty;
				}
				if (!MathfEx.RangeEqualOn(0, select, cfiList.Count - 1))
				{
					return string.Empty;
				}
				return cfiList[select].file;
			}
		}

		public CharaFileSort()
		{
			m_Select = -1;
			sortKind = -1;
		}

		public void DeleteAllNode()
		{
			int childCount = root.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.Destroy(root.GetChild(i).gameObject);
			}
			root.DetachChildren();
			m_Select = -1;
		}

		public void Sort(int _type, bool _ascend)
		{
			sortKind = _type;
			switch (sortKind)
			{
			case 0:
				SortName(_ascend);
				break;
			case 1:
				SortTime(_ascend);
				break;
			}
			for (int i = 0; i < imageSort.Length; i++)
			{
				imageSort[i].enabled = i == sortKind;
			}
		}

		public void Sort(int _type)
		{
			Sort(_type, (sortKind != _type) ? sortType[_type] : (!sortType[_type]));
		}

		private void SortName(bool _ascend)
		{
			if (cfiList.IsNullOrEmpty())
			{
				return;
			}
			sortType[0] = _ascend;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
			if (_ascend)
			{
				cfiList.Sort((CharaFileInfo a, CharaFileInfo b) => a.name.CompareTo(b.name));
			}
			else
			{
				cfiList.Sort((CharaFileInfo a, CharaFileInfo b) => b.name.CompareTo(a.name));
			}
			Thread.CurrentThread.CurrentCulture = currentCulture;
			for (int i = 0; i < cfiList.Count; i++)
			{
				cfiList[i].index = i;
				cfiList[i].siblingIndex = i;
			}
			select = cfiList.FindIndex((CharaFileInfo v) => v.select);
			imageSort[0].sprite = spriteSort[(!sortType[0]) ? 1u : 0u];
		}

		private void SortTime(bool _ascend)
		{
			if (cfiList.IsNullOrEmpty())
			{
				return;
			}
			sortType[1] = _ascend;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
			if (_ascend)
			{
				cfiList.Sort((CharaFileInfo a, CharaFileInfo b) => a.time.CompareTo(b.time));
			}
			else
			{
				cfiList.Sort((CharaFileInfo a, CharaFileInfo b) => b.time.CompareTo(a.time));
			}
			Thread.CurrentThread.CurrentCulture = currentCulture;
			for (int i = 0; i < cfiList.Count; i++)
			{
				cfiList[i].index = i;
				cfiList[i].siblingIndex = i;
			}
			select = cfiList.FindIndex((CharaFileInfo v) => v.select);
			imageSort[1].sprite = spriteSort[(!sortType[1]) ? 1u : 0u];
		}
	}
}
