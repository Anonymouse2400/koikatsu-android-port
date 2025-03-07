using System.Collections.Generic;
using UnityEngine;

public class MetaballCellCluster : MetaballCellClusterInterface
{
	private List<MetaballCell> _cells = new List<MetaballCell>();

	private float _baseRadius;

	private Vector3 _baseColor = Vector3.one;

	public float BaseRadius
	{
		get
		{
			return _baseRadius;
		}
		set
		{
			_baseRadius = value;
		}
	}

	public int CellCount
	{
		get
		{
			return _cells.Count;
		}
	}

	public void DoForeachCell(ForeachCellDeleg deleg)
	{
		foreach (MetaballCell cell in _cells)
		{
			deleg(cell);
		}
	}

	public MetaballCell GetCell(int index)
	{
		return _cells[index];
	}

	public int FindCell(MetaballCell cell)
	{
		for (int i = 0; i < _cells.Count; i++)
		{
			if (_cells[i] == cell)
			{
				return i;
			}
		}
		return -1;
	}

	public MetaballCell AddCell(Vector3 position, float minDistanceCoef = 1f, float? radius = null, string tag = null)
	{
		MetaballCell cell = new MetaballCell();
		cell.baseColor = _baseColor;
		cell.radius = (radius.HasValue ? radius.Value : _baseRadius);
		cell.modelPosition = position;
		cell.tag = tag;
		bool bFail = false;
		DoForeachCell(delegate(MetaballCell c)
		{
			if ((cell.modelPosition - c.modelPosition).sqrMagnitude < cell.radius * cell.radius * minDistanceCoef * minDistanceCoef)
			{
				bFail = true;
			}
		});
		if (!bFail)
		{
			_cells.Add(cell);
		}
		return (!bFail) ? cell : null;
	}

	public void RemoveCell(MetaballCell cell)
	{
		_cells.Remove(cell);
	}

	public void ClearCells()
	{
		_cells.Clear();
	}

	public string GetPositionsString()
	{
		string text = string.Empty;
		foreach (MetaballCell cell in _cells)
		{
			text += cell.modelPosition.ToString("F3");
			text += ";";
		}
		if (text[text.Length - 1] == ';')
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text;
	}

	public void ReadPositionsString(string positions)
	{
		ClearCells();
		string[] array = positions.Split(';');
		if (array.Length == 0)
		{
			throw new UnityException("invalid input positions data :" + positions);
		}
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position = ParseVector3(array[i]);
			AddCell(position, 0f, null);
		}
	}

	private static Vector3 ParseVector3(string data)
	{
		int num = data.IndexOf('(');
		int num2 = data.IndexOf(')');
		string text = data.Substring(num + 1, num2 - num - 1);
		string[] array = text.Split(',');
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3 && i < array.Length; i++)
		{
			zero[i] = float.Parse(array[i]);
		}
		return zero;
	}
}
