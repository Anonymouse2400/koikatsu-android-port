public class ExcelDataSeek
{
	public ExcelData data { get; private set; }

	public int cell { get; private set; }

	public int row { get; private set; }

	public bool isEndCell
	{
		get
		{
			return cell >= data.list.Count;
		}
	}

	public bool isEndRow
	{
		get
		{
			bool result = true;
			if (cell < data.list.Count && row < data.list[cell].list.Count)
			{
				result = false;
			}
			return result;
		}
	}

	public ExcelDataSeek(ExcelData data)
	{
		this.data = data;
		cell = 0;
		row = 0;
	}

	public int SetCell(int set)
	{
		return cell = set;
	}

	public int SetRow(int set)
	{
		return row = set;
	}

	public int NextCell(int next)
	{
		return cell += next;
	}

	public int NextRow(int next)
	{
		return row += next;
	}

	public bool SearchCell(int next = 0)
	{
		bool result = false;
		cell += next;
		while (cell < data.list.Count)
		{
			if (row < data.list[cell].list.Count && data.list[cell].list[row] != string.Empty)
			{
				result = true;
				break;
			}
			cell++;
		}
		return result;
	}

	public bool SearchRow(int next = 0)
	{
		bool result = false;
		row += next;
		if (cell < data.list.Count)
		{
			while (row < data.list[cell].list.Count)
			{
				if (data.list[cell].list[row] != string.Empty)
				{
					result = true;
					break;
				}
				row++;
			}
		}
		return result;
	}
}
