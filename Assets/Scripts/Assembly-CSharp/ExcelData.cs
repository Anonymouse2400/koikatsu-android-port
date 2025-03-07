using System;
using System.Collections.Generic;
using UnityEngine;

public class ExcelData : ScriptableObject
{
	public class Specify
	{
		public int cell;

		public int row;

		public Specify(int cell, int row)
		{
			this.cell = cell;
			this.row = row;
		}

		public Specify()
		{
		}
	}

	[Serializable]
	public class Param
	{
		public List<string> list = new List<string>();
	}

	public List<Param> list = new List<Param>();

	public string this[int cell, int row]
	{
		get
		{
			return Get(cell, row);
		}
	}

	public int MaxCell
	{
		get
		{
			return list.Count;
		}
	}

	public string Get(Specify specify)
	{
		return Get(specify.cell, specify.row);
	}

	public string Get(int cell, int row)
	{
		string result = string.Empty;
		if (cell < list.Count && row < list[cell].list.Count)
		{
			result = list[cell].list[row];
		}
		return result;
	}

	public List<string> GetCell(int row)
	{
		List<string> list = new List<string>();
		foreach (Param item in this.list)
		{
			if (row < item.list.Count)
			{
				list.Add(item.list[row]);
			}
		}
		return list;
	}

	public List<string> GetCell(int rowStart, int rowEnd)
	{
		List<string> list = new List<string>();
		if ((uint)rowStart > rowEnd)
		{
			return list;
		}
		foreach (Param item in this.list)
		{
			for (int i = rowStart; i < item.list.Count && i <= rowEnd; i++)
			{
				list.Add(item.list[i]);
			}
		}
		return list;
	}

	public List<string> GetRow(int cell)
	{
		List<string> list = new List<string>();
		if (cell < this.list.Count)
		{
			foreach (string item in this.list[cell].list)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<string> GetRow(int cellStart, int cellEnd)
	{
		List<string> list = new List<string>();
		if ((uint)cellStart > cellEnd)
		{
			return list;
		}
		for (int i = cellStart; i < this.list.Count && i <= cellEnd; i++)
		{
			foreach (string item in this.list[i].list)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<Param> Get(Specify start, Specify end)
	{
		List<Param> list = new List<Param>();
		if ((uint)start.cell > end.cell || (uint)start.row > end.row)
		{
			return list;
		}
		if (start.cell < this.list.Count)
		{
			for (int i = start.cell; i < this.list.Count && i <= end.cell; i++)
			{
				Param param = new Param();
				if (start.row < this.list[i].list.Count)
				{
					param.list = new List<string>();
					for (int j = start.row; j < this.list[i].list.Count && j <= end.row; j++)
					{
						param.list.Add(this.list[i].list[j]);
					}
				}
				list.Add(param);
			}
		}
		return list;
	}

	public List<Param> Get(Specify start, Specify end, string find)
	{
		List<Param> list = new List<Param>();
		list = Get(start, end);
		foreach (Param item in list)
		{
			foreach (string item2 in item.list)
			{
				if (!(find == item2))
				{
					continue;
				}
				Param param = new Param();
				param.list = new List<string>();
				foreach (string item3 in item.list)
				{
					param.list.Add(item3);
				}
				list.Add(param);
				break;
			}
		}
		return list;
	}

	public List<Param> Find(string find)
	{
		List<Param> list = new List<Param>();
		foreach (Param item in this.list)
		{
			foreach (string item2 in item.list)
			{
				if (!(find == item2))
				{
					continue;
				}
				Param param = new Param();
				param.list = new List<string>();
				foreach (string item3 in item.list)
				{
					param.list.Add(item3);
				}
				list.Add(param);
				break;
			}
		}
		return list;
	}

	public List<Param> FindArea(string find, Specify spe)
	{
		List<Param> list = new List<Param>();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.list.Count; i++)
		{
			for (int j = 0; j < this.list[i].list.Count; j++)
			{
				if (find == this.list[i].list[j])
				{
					num = i + 1;
					num2 = j;
					break;
				}
			}
		}
		for (int k = num; k < this.list.Count && k < num + spe.cell; k++)
		{
			Param param = new Param();
			for (int l = num2; l < this.list[k].list.Count && l < num2 + spe.row; l++)
			{
				param.list = new List<string>();
				param.list.Add(this.list[k].list[l]);
				list.Add(param);
			}
		}
		return list;
	}

	public List<Param> FindArea(string find)
	{
		List<Param> list = new List<Param>();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.list.Count; i++)
		{
			for (int j = 0; j < this.list[i].list.Count; j++)
			{
				if (find == this.list[i].list[j])
				{
					num = i + 1;
					num2 = j;
					break;
				}
			}
		}
		for (int k = num; k < this.list.Count; k++)
		{
			Param param = new Param();
			for (int l = num2; l < this.list[k].list.Count; l++)
			{
				param.list = new List<string>();
				param.list.Add(this.list[k].list[l]);
				list.Add(param);
			}
		}
		return list;
	}
}
