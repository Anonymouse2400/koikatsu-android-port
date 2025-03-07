using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Grid : WMG_Graph_Manager
{
	public enum gridTypes
	{
		quadrilateral = 0,
		hexagonal_flat_top = 1,
		hexagonal_flat_side = 2
	}

	public bool autoRefresh = true;

	public gridTypes gridType;

	public UnityEngine.Object nodePrefab;

	public UnityEngine.Object linkPrefab;

	public int gridNumNodesX;

	public int gridNumNodesY;

	public float gridLinkLengthX;

	public float gridLinkLengthY;

	public bool createLinks;

	public bool noVerticalLinks;

	public bool noHorizontalLinks;

	public Color linkColor = Color.white;

	public int linkWidth;

	private List<List<WMG_Node>> gridNodesXY = new List<List<WMG_Node>>();

	private List<GameObject> gridLinks = new List<GameObject>();

	private gridTypes cachedGridType;

	private UnityEngine.Object cachedNodePrefab;

	private UnityEngine.Object cachedLinkPrefab;

	private int cachedGridNumNodesX;

	private int cachedGridNumNodesY;

	private float cachedGridLinkLengthX;

	private float cachedGridLinkLengthY;

	private bool cachedCreateLinks;

	private bool cachedNoVerticalLinks;

	private bool cachedNoHorizontalLinks;

	private Color cachedLinkColor;

	private int cachedLinkWidth;

	private bool gridChanged = true;

	private void Update()
	{
		if (autoRefresh)
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		checkCache();
		if (gridChanged)
		{
			refresh();
		}
		setCacheFlags(false);
	}

	private void checkCache()
	{
		updateCacheAndFlag(ref cachedGridType, gridType, ref gridChanged);
		updateCacheAndFlag(ref cachedNodePrefab, nodePrefab, ref gridChanged);
		updateCacheAndFlag(ref cachedLinkPrefab, linkPrefab, ref gridChanged);
		updateCacheAndFlag(ref cachedGridNumNodesX, gridNumNodesX, ref gridChanged);
		updateCacheAndFlag(ref cachedGridNumNodesY, gridNumNodesY, ref gridChanged);
		updateCacheAndFlag(ref cachedGridLinkLengthX, gridLinkLengthX, ref gridChanged);
		updateCacheAndFlag(ref cachedGridLinkLengthY, gridLinkLengthY, ref gridChanged);
		updateCacheAndFlag(ref cachedCreateLinks, createLinks, ref gridChanged);
		updateCacheAndFlag(ref cachedNoVerticalLinks, noVerticalLinks, ref gridChanged);
		updateCacheAndFlag(ref cachedNoHorizontalLinks, noHorizontalLinks, ref gridChanged);
		updateCacheAndFlag(ref cachedLinkColor, linkColor, ref gridChanged);
		updateCacheAndFlag(ref cachedLinkWidth, linkWidth, ref gridChanged);
	}

	private void setCacheFlags(bool val)
	{
		gridChanged = val;
	}

	public List<WMG_Node> getColumn(int colNum)
	{
		if (gridNodesXY.Count <= colNum)
		{
			return new List<WMG_Node>();
		}
		return gridNodesXY[colNum];
	}

	public void setActiveColumn(bool active, int colNum)
	{
		if (gridNodesXY.Count > colNum)
		{
			for (int i = 0; i < gridNodesXY[colNum].Count; i++)
			{
				SetActive(gridNodesXY[colNum][i].gameObject, active);
			}
		}
	}

	public List<WMG_Node> getRow(int rowNum)
	{
		List<WMG_Node> list = new List<WMG_Node>();
		for (int i = 0; i < gridNodesXY.Count; i++)
		{
			list.Add(gridNodesXY[i][rowNum]);
		}
		return list;
	}

	public void setActiveRow(bool active, int rowNum)
	{
		for (int i = 0; i < gridNodesXY.Count; i++)
		{
			SetActive(gridNodesXY[i][rowNum].gameObject, active);
		}
	}

	public List<GameObject> GetNodesAndLinks()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < gridNodesXY.Count; i++)
		{
			for (int j = 0; j < gridNodesXY[i].Count; j++)
			{
				list.Add(gridNodesXY[i][j].gameObject);
			}
		}
		for (int k = 0; k < gridLinks.Count; k++)
		{
			list.Add(gridLinks[k]);
		}
		return list;
	}

	private void refresh()
	{
		for (int i = 0; i < gridNumNodesX; i++)
		{
			if (gridNodesXY.Count <= i)
			{
				List<WMG_Node> item = new List<WMG_Node>();
				gridNodesXY.Add(item);
				for (int j = 0; j < gridNumNodesY; j++)
				{
					WMG_Node component = CreateNode(nodePrefab, base.gameObject).GetComponent<WMG_Node>();
					gridNodesXY[i].Add(component);
				}
			}
		}
		for (int k = 0; k < gridNumNodesX; k++)
		{
			for (int l = 0; l < gridNumNodesY; l++)
			{
				if (gridNodesXY[k].Count <= l)
				{
					WMG_Node component2 = CreateNode(nodePrefab, base.gameObject).GetComponent<WMG_Node>();
					gridNodesXY[k].Add(component2);
				}
			}
		}
		for (int m = 0; m < gridNumNodesX; m++)
		{
			for (int num = gridNodesXY[m].Count - 1; num >= 0; num--)
			{
				if (num >= gridNumNodesY)
				{
					DeleteNode(gridNodesXY[m][num]);
					gridNodesXY[m].RemoveAt(num);
				}
			}
		}
		for (int num2 = gridNodesXY.Count - 1; num2 >= 0; num2--)
		{
			if (num2 >= gridNumNodesX)
			{
				for (int num3 = gridNumNodesY - 1; num3 >= 0; num3--)
				{
					DeleteNode(gridNodesXY[num2][num3]);
					gridNodesXY[num2].RemoveAt(num3);
				}
				gridNodesXY.RemoveAt(num2);
			}
		}
		for (int n = 0; n < gridNumNodesX; n++)
		{
			for (int num4 = 0; num4 < gridNumNodesY; num4++)
			{
				if (num4 + 1 < gridNumNodesY)
				{
					CreateOrDeleteLink(gridNodesXY[n][num4], gridNodesXY[n][num4 + 1], noVerticalLinks);
				}
				if (n + 1 >= gridNumNodesX)
				{
					continue;
				}
				CreateOrDeleteLink(gridNodesXY[n][num4], gridNodesXY[n + 1][num4], noHorizontalLinks);
				if (gridType == gridTypes.hexagonal_flat_top || gridType == gridTypes.hexagonal_flat_side)
				{
					if (n % 2 == 1)
					{
						if (num4 + 1 < gridNumNodesY)
						{
							CreateOrDeleteLink(gridNodesXY[n][num4], gridNodesXY[n + 1][num4 + 1], noHorizontalLinks);
						}
					}
					else if (num4 > 0)
					{
						CreateOrDeleteLink(gridNodesXY[n][num4], gridNodesXY[n + 1][num4 - 1], noHorizontalLinks);
					}
				}
				else
				{
					if (gridType != 0)
					{
						continue;
					}
					if (n % 2 == 1)
					{
						if (num4 + 1 < gridNumNodesY)
						{
							CreateOrDeleteLink(gridNodesXY[n][num4], gridNodesXY[n + 1][num4 + 1], true);
						}
					}
					else if (num4 > 0)
					{
						CreateOrDeleteLink(gridNodesXY[n][num4], gridNodesXY[n + 1][num4 - 1], true);
					}
				}
			}
		}
		for (int num5 = 0; num5 < gridNumNodesY; num5++)
		{
			for (int num6 = 0; num6 < gridNumNodesX; num6++)
			{
				float x = 0f;
				float y = 0f;
				if (gridType == gridTypes.quadrilateral)
				{
					x = (float)num6 * gridLinkLengthX + (float)(num6 * 2) * gridNodesXY[num6][num5].radius;
					y = (float)num5 * gridLinkLengthY + (float)(num5 * 2) * gridNodesXY[num6][num5].radius;
				}
				else if (gridType == gridTypes.hexagonal_flat_top)
				{
					int num7 = num6 % 2;
					x = (float)num6 * gridLinkLengthX * Mathf.Cos((float)Math.PI / 6f) + (float)num6 * Mathf.Sqrt(3f) * gridNodesXY[num6][num5].radius;
					y = (float)num5 * gridLinkLengthY + (float)(num5 * 2) * gridNodesXY[num6][num5].radius + (float)num7 * gridNodesXY[num6][num5].radius + (float)num7 * gridLinkLengthY * Mathf.Sin((float)Math.PI / 6f);
				}
				else if (gridType == gridTypes.hexagonal_flat_side)
				{
					int num8 = num5 % 2;
					x = (float)num6 * gridLinkLengthX + (float)(num6 * 2) * gridNodesXY[num6][num5].radius + (float)num8 * gridNodesXY[num6][num5].radius + (float)num8 * gridLinkLengthX * Mathf.Sin((float)Math.PI / 6f);
					y = (float)num5 * gridLinkLengthY * Mathf.Cos((float)Math.PI / 6f) + (float)num5 * Mathf.Sqrt(3f) * gridNodesXY[num6][num5].radius;
				}
				gridNodesXY[num6][num5].Reposition(x, y);
			}
		}
		for (int num9 = 0; num9 < gridLinks.Count; num9++)
		{
			if (gridLinks[num9] != null)
			{
				changeSpriteColor(gridLinks[num9], linkColor);
				changeSpriteWidth(gridLinks[num9], linkWidth);
			}
		}
	}

	private void CreateOrDeleteLink(WMG_Node fromNode, WMG_Node toNode, bool noVertHoriz)
	{
		WMG_Link link = GetLink(fromNode, toNode);
		if (link == null)
		{
			if (createLinks && !noVertHoriz)
			{
				gridLinks.Add(CreateLink(fromNode, toNode.gameObject, linkPrefab, base.gameObject));
			}
		}
		else if (!createLinks || noVertHoriz)
		{
			gridLinks.Remove(link.gameObject);
			DeleteLink(link);
		}
	}
}
