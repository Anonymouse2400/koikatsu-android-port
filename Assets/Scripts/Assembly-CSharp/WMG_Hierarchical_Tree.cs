using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Hierarchical_Tree : WMG_Graph_Manager
{
	[Flags]
	public enum ResizeProperties
	{
		NodeWidthHeight = 1,
		NodeRadius = 2
	}

	public GameObject nodeParent;

	public GameObject linkParent;

	public UnityEngine.Object defaultNodePrefab;

	public UnityEngine.Object linkPrefab;

	public int numNodes;

	public int numLinks;

	public List<UnityEngine.Object> nodePrefabs;

	public List<int> nodeColumns;

	public List<int> nodeRows;

	public List<int> linkNodeFromIDs;

	public List<int> linkNodeToIDs;

	public UnityEngine.Object invisibleNodePrefab;

	public int numInvisibleNodes;

	public List<int> invisibleNodeColumns;

	public List<int> invisibleNodeRows;

	private float _gridLengthX;

	private float _gridLengthY;

	[SerializeField]
	private int _nodeWidthHeight;

	[SerializeField]
	private float _nodeRadius;

	[SerializeField]
	private bool _squareNodes;

	[SerializeField]
	private bool _resizeEnabled;

	[SerializeField]
	[WMG_EnumFlag]
	private ResizeProperties _resizeProperties;

	private float cachedContainerWidth;

	private float cachedContainerHeight;

	private float origWidth;

	private float origHeight;

	private int origNodeWidthHeight;

	private float origNodeRadius;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	private WMG_Change_Obj treeC = new WMG_Change_Obj();

	public WMG_Change_Obj resizeC = new WMG_Change_Obj();

	private List<GameObject> treeNodes = new List<GameObject>();

	private List<GameObject> treeLinks = new List<GameObject>();

	private List<GameObject> treeInvisibleNodes = new List<GameObject>();

	private bool hasInit;

	public ResizeProperties resizeProperties
	{
		get
		{
			return _resizeProperties;
		}
		set
		{
			if (_resizeProperties != value)
			{
				_resizeProperties = value;
				resizeC.Changed();
			}
		}
	}

	public bool resizeEnabled
	{
		get
		{
			return _resizeEnabled;
		}
		set
		{
			if (_resizeEnabled != value)
			{
				_resizeEnabled = value;
				resizeC.Changed();
			}
		}
	}

	public int nodeWidthHeight
	{
		get
		{
			return _nodeWidthHeight;
		}
		set
		{
			if (_nodeWidthHeight != value)
			{
				_nodeWidthHeight = value;
				treeC.Changed();
			}
		}
	}

	public float nodeRadius
	{
		get
		{
			return _nodeRadius;
		}
		set
		{
			if (_nodeRadius != value)
			{
				_nodeRadius = value;
				treeC.Changed();
			}
		}
	}

	public bool squareNodes
	{
		get
		{
			return _squareNodes;
		}
		set
		{
			if (_squareNodes != value)
			{
				_squareNodes = value;
				treeC.Changed();
			}
		}
	}

	private void Start()
	{
		Init();
		PauseCallbacks();
		CreateNodes();
		CreatedLinks();
		treeC.Changed();
	}

	public void Init()
	{
		if (!hasInit)
		{
			hasInit = true;
			changeObjs.Add(treeC);
			changeObjs.Add(resizeC);
			treeC.OnChange += refresh;
			resizeC.OnChange += ResizeChanged;
			setOriginalPropertyValues();
			PauseCallbacks();
		}
	}

	private void Update()
	{
		updateFromResize();
		Refresh();
	}

	public void setOriginalPropertyValues()
	{
		cachedContainerWidth = getSpriteWidth(base.gameObject);
		cachedContainerHeight = getSpriteHeight(base.gameObject);
		origWidth = getSpriteWidth(base.gameObject);
		origHeight = getSpriteHeight(base.gameObject);
		origNodeWidthHeight = nodeWidthHeight;
		origNodeRadius = nodeRadius;
	}

	private void ResizeChanged()
	{
		UpdateFromContainer();
	}

	private void UpdateFromContainer()
	{
		if (resizeEnabled)
		{
			Vector2 vector = new Vector2(cachedContainerWidth / origWidth, cachedContainerHeight / origHeight);
			float num = vector.x;
			if (vector.y < num)
			{
				num = vector.y;
			}
			if ((resizeProperties & ResizeProperties.NodeWidthHeight) == ResizeProperties.NodeWidthHeight)
			{
				nodeWidthHeight = Mathf.RoundToInt(getNewResizeVariable(num, origNodeWidthHeight));
			}
			if ((resizeProperties & ResizeProperties.NodeRadius) == ResizeProperties.NodeRadius)
			{
				nodeRadius = getNewResizeVariable(num, origNodeRadius);
			}
		}
	}

	private float getNewResizeVariable(float sizeFactor, float variable)
	{
		return variable + (sizeFactor - 1f) * variable;
	}

	private void updateFromResize()
	{
		bool flag = false;
		updateCacheAndFlag(ref cachedContainerWidth, getSpriteWidth(base.gameObject), ref flag);
		updateCacheAndFlag(ref cachedContainerHeight, getSpriteHeight(base.gameObject), ref flag);
		if (flag)
		{
			treeC.Changed();
			resizeC.Changed();
		}
	}

	public void Refresh()
	{
		ResumeCallbacks();
		PauseCallbacks();
	}

	private void PauseCallbacks()
	{
		for (int i = 0; i < changeObjs.Count; i++)
		{
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
	}

	private void ResumeCallbacks()
	{
		for (int i = 0; i < changeObjs.Count; i++)
		{
			changeObjs[i].changesPaused = false;
			if (changeObjs[i].changePaused)
			{
				changeObjs[i].Changed();
			}
		}
	}

	public List<GameObject> getNodesInRow(int rowNum)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < treeNodes.Count; i++)
		{
			if (Mathf.Approximately(getSpritePositionY(treeNodes[i]), (float)(-rowNum) * _gridLengthY))
			{
				list.Add(treeNodes[i]);
			}
		}
		return list;
	}

	public List<GameObject> getNodesInColumn(int colNum)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < treeNodes.Count; i++)
		{
			if (Mathf.Approximately(getSpritePositionX(treeNodes[i]), (float)colNum * _gridLengthX))
			{
				list.Add(treeNodes[i]);
			}
		}
		return list;
	}

	private void refresh()
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < treeNodes.Count; i++)
		{
			if (nodeRows[i] > num)
			{
				num = nodeRows[i];
			}
			if (nodeColumns[i] > num2)
			{
				num2 = nodeColumns[i];
			}
		}
		Vector2 vector = new Vector2(getSpriteWidth(base.gameObject), getSpriteHeight(base.gameObject));
		_gridLengthX = vector.x / (float)num2;
		_gridLengthY = vector.y / (float)num;
		for (int j = 0; j < treeNodes.Count; j++)
		{
			changeSpriteSize(treeNodes[j], nodeWidthHeight, nodeWidthHeight);
			treeNodes[j].GetComponent<WMG_Node>().radius = nodeRadius;
			treeNodes[j].GetComponent<WMG_Node>().isSquare = squareNodes;
			float x = ((float)nodeColumns[j] - 0.5f) * _gridLengthX - vector.x / 2f;
			float num3 = ((float)nodeRows[j] - 0.5f) * _gridLengthY - vector.y / 2f;
			treeNodes[j].GetComponent<WMG_Node>().Reposition(x, 0f - num3);
		}
		for (int k = 0; k < treeInvisibleNodes.Count; k++)
		{
			changeSpritePivot(treeInvisibleNodes[k], WMGpivotTypes.Center);
			changeSpriteSize(treeInvisibleNodes[k], nodeWidthHeight, nodeWidthHeight);
			float x2 = ((float)invisibleNodeColumns[k] - 0.5f) * _gridLengthX - vector.x / 2f;
			float num4 = ((float)invisibleNodeRows[k] - 0.5f) * _gridLengthY - vector.y / 2f;
			treeInvisibleNodes[k].GetComponent<WMG_Node>().Reposition(x2, 0f - num4);
		}
	}

	public void CreateNodes()
	{
		for (int i = 0; i < numNodes; i++)
		{
			if (treeNodes.Count <= i)
			{
				UnityEngine.Object prefabNode = defaultNodePrefab;
				if (nodePrefabs.Count > i)
				{
					prefabNode = nodePrefabs[i];
				}
				WMG_Node component = CreateNode(prefabNode, nodeParent).GetComponent<WMG_Node>();
				treeNodes.Add(component.gameObject);
			}
		}
		for (int j = 0; j < numInvisibleNodes; j++)
		{
			if (treeInvisibleNodes.Count <= j)
			{
				WMG_Node component2 = CreateNode(invisibleNodePrefab, nodeParent).GetComponent<WMG_Node>();
				treeInvisibleNodes.Add(component2.gameObject);
			}
		}
	}

	public void CreatedLinks()
	{
		for (int i = 0; i < numLinks; i++)
		{
			if (treeLinks.Count <= i)
			{
				GameObject gameObject = null;
				gameObject = ((linkNodeFromIDs[i] <= 0) ? treeInvisibleNodes[-linkNodeFromIDs[i] - 1] : treeNodes[linkNodeFromIDs[i] - 1]);
				GameObject gameObject2 = null;
				gameObject2 = ((linkNodeToIDs[i] <= 0) ? treeInvisibleNodes[-linkNodeToIDs[i] - 1] : treeNodes[linkNodeToIDs[i] - 1]);
				treeLinks.Add(CreateLinkNoRepos(gameObject.GetComponent<WMG_Node>(), gameObject2, linkPrefab, linkParent));
			}
		}
	}
}
