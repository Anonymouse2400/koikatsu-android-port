using System.Collections.Generic;
using UnityEngine;

public class SkinnedMetaballCell : MetaballCell, MetaballCellClusterInterface
{
	public delegate void ForeachSkinnedCellDeleg(SkinnedMetaballCell c);

	public SkinnedMetaballCell parent;

	public List<SkinnedMetaballCell> children = new List<SkinnedMetaballCell>();

	public List<SkinnedMetaballCell> links = new List<SkinnedMetaballCell>();

	public int distanceFromRoot;

	public float BaseRadius
	{
		get
		{
			return radius;
		}
	}

	public bool IsRoot
	{
		get
		{
			return parent == null;
		}
	}

	public bool IsTerminal
	{
		get
		{
			return children.Count == 0;
		}
	}

	public bool IsBranch
	{
		get
		{
			return IsRoot || children.Count > 1;
		}
	}

	public SkinnedMetaballCell Root
	{
		get
		{
			if (IsRoot)
			{
				return this;
			}
			return parent.Root;
		}
	}

	public int CellCount
	{
		get
		{
			int num = 1;
			foreach (SkinnedMetaballCell child in children)
			{
				num += child.CellCount;
			}
			return num;
		}
	}

	public int DistanceFromBranch
	{
		get
		{
			if (IsBranch)
			{
				return 0;
			}
			int distanceFromLastBranch = DistanceFromLastBranch;
			int distanceToNextBranch = DistanceToNextBranch;
			return Mathf.Min(distanceFromLastBranch, distanceToNextBranch);
		}
	}

	public int DistanceFromLastLink
	{
		get
		{
			if (IsRoot || children.Count > 1 || links.Count > 0)
			{
				return 0;
			}
			return parent.DistanceFromLastLink + 1;
		}
	}

	private int DistanceFromLastBranch
	{
		get
		{
			if (IsBranch)
			{
				return 0;
			}
			return 1 + parent.DistanceFromLastBranch;
		}
	}

	private int DistanceToNextBranch
	{
		get
		{
			if (IsBranch)
			{
				return 0;
			}
			int num = int.MaxValue;
			foreach (SkinnedMetaballCell child in children)
			{
				int distanceToNextBranch = child.DistanceToNextBranch;
				if (distanceToNextBranch < num)
				{
					num = distanceToNextBranch;
				}
			}
			return num;
		}
	}

	public void DoForeachSkinnedCell(ForeachSkinnedCellDeleg deleg)
	{
		deleg(this);
		foreach (SkinnedMetaballCell child in children)
		{
			child.DoForeachSkinnedCell(deleg);
		}
	}

	public void DoForeachCell(ForeachCellDeleg deleg)
	{
		deleg(this);
		foreach (SkinnedMetaballCell child in children)
		{
			child.DoForeachCell(deleg);
		}
	}

	public SkinnedMetaballCell AddChild(Vector3 position, float in_radius, float minDistanceCoef = 1f)
	{
		SkinnedMetaballCell child = new SkinnedMetaballCell();
		child.baseColor = baseColor;
		child.radius = in_radius;
		child.distanceFromRoot = distanceFromRoot + 1;
		child.modelPosition = position;
		child.parent = this;
		children.Add(child);
		bool bFail = false;
		Root.DoForeachSkinnedCell(delegate(SkinnedMetaballCell c)
		{
			if (c != child && (child.modelPosition - c.modelPosition).sqrMagnitude < child.radius * child.radius * minDistanceCoef * minDistanceCoef)
			{
				bFail = true;
			}
		});
		if (bFail)
		{
			children.Remove(child);
			return null;
		}
		child.CalcRotation();
		return child;
	}

	public void CalcRotation()
	{
		if (IsRoot)
		{
			modelRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
			return;
		}
		Vector3 fromDirection = parent.modelRotation * Vector3.forward;
		Vector3 toDirection = modelPosition - parent.modelPosition;
		modelRotation = Quaternion.FromToRotation(fromDirection, toDirection) * parent.modelRotation;
	}

	public string GetStringExpression()
	{
		string empty = string.Empty;
		empty += modelPosition.ToString("F3");
		empty += ";";
		foreach (SkinnedMetaballCell child in children)
		{
			empty += child.GetStringExpression();
			empty += ";";
		}
		if (empty[empty.Length - 1] == ';')
		{
			empty = empty.Substring(0, empty.Length - 1);
		}
		return empty;
	}

	public static SkinnedMetaballCell ConstructFromString(string data, float radius)
	{
		string[] array = data.Split(';');
		if (array.Length == 0)
		{
			throw new UnityException("invalid input data :" + data);
		}
		SkinnedMetaballCell skinnedMetaballCell = new SkinnedMetaballCell();
		skinnedMetaballCell.parent = null;
		skinnedMetaballCell.modelPosition = ParseVector3(array[0]);
		skinnedMetaballCell.radius = radius;
		skinnedMetaballCell.baseColor = Vector3.zero;
		skinnedMetaballCell.CalcRotation();
		for (int i = 1; i < array.Length; i++)
		{
			Vector3 position = ParseVector3(array[i]);
			skinnedMetaballCell.AddChild(position, radius, 0f);
		}
		return skinnedMetaballCell;
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
