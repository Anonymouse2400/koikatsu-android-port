using System;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class CharaFileInfo
	{
		public string file = string.Empty;

		public string name = string.Empty;

		public DateTime time;

		public int index = -1;

		public Button button;

		public ListNode node { get; set; }

		public bool select
		{
			get
			{
				return node.select;
			}
			set
			{
				node.select = value;
				if ((bool)button)
				{
					button.image.color = ((!value) ? Color.white : Color.green);
				}
			}
		}

		public int siblingIndex
		{
			set
			{
				node.transform.SetSiblingIndex(value);
			}
		}

		public CharaFileInfo(string _file = "", string _name = "")
		{
			file = _file;
			name = _name;
		}
	}
}
