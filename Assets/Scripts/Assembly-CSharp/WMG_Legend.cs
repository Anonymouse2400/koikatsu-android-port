using System.Collections.Generic;
using UnityEngine;

public class WMG_Legend : WMG_GUI_Functions
{
	public enum legendTypes
	{
		Bottom = 0,
		Right = 1
	}

	public WMG_Graph_Manager theGraph;

	public GameObject background;

	public GameObject entriesParent;

	public Object emptyPrefab;

	public List<WMG_Legend_Entry> legendEntries;

	private WMG_Pie_Graph pieGraph;

	private WMG_Axis_Graph axisGraph;

	[SerializeField]
	private bool _hideLegend;

	[SerializeField]
	private legendTypes _legendType;

	[SerializeField]
	private WMG_Enums.labelTypes _labelType;

	[SerializeField]
	private bool _showBackground;

	[SerializeField]
	private bool _oppositeSideLegend;

	[SerializeField]
	private float _offset;

	[SerializeField]
	private float _legendEntryWidth;

	[SerializeField]
	private bool _setWidthFromLabels;

	[SerializeField]
	private float _legendEntryHeight;

	[SerializeField]
	private int _numRowsOrColumns;

	[SerializeField]
	private int _numDecimals;

	[SerializeField]
	private float _legendEntryLinkSpacing;

	[SerializeField]
	private int _legendEntryFontSize;

	[SerializeField]
	private float _legendEntrySpacing;

	[SerializeField]
	private float _pieSwatchSize;

	[SerializeField]
	private float _backgroundPadding;

	[SerializeField]
	private bool _autofitEnabled;

	[SerializeField]
	private Color _labelColor;

	[SerializeField]
	private FontStyle _legendEntryFontStyle;

	[SerializeField]
	private Font _legendEntryFont;

	private bool hasInit;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	public WMG_Change_Obj legendC = new WMG_Change_Obj();

	public bool hideLegend
	{
		get
		{
			return _hideLegend;
		}
		set
		{
			if (_hideLegend != value)
			{
				_hideLegend = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public legendTypes legendType
	{
		get
		{
			return _legendType;
		}
		set
		{
			if (_legendType != value)
			{
				_legendType = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public WMG_Enums.labelTypes labelType
	{
		get
		{
			return _labelType;
		}
		set
		{
			if (_labelType != value)
			{
				_labelType = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public bool showBackground
	{
		get
		{
			return _showBackground;
		}
		set
		{
			if (_showBackground != value)
			{
				_showBackground = value;
				legendC.Changed();
			}
		}
	}

	public bool oppositeSideLegend
	{
		get
		{
			return _oppositeSideLegend;
		}
		set
		{
			if (_oppositeSideLegend != value)
			{
				_oppositeSideLegend = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public float offset
	{
		get
		{
			return _offset;
		}
		set
		{
			if (_offset != value)
			{
				_offset = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public float legendEntryWidth
	{
		get
		{
			return _legendEntryWidth;
		}
		set
		{
			if (_legendEntryWidth != value)
			{
				_legendEntryWidth = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public bool setWidthFromLabels
	{
		get
		{
			return _setWidthFromLabels;
		}
		set
		{
			if (_setWidthFromLabels != value)
			{
				_setWidthFromLabels = value;
				legendC.Changed();
			}
		}
	}

	public float legendEntryHeight
	{
		get
		{
			return _legendEntryHeight;
		}
		set
		{
			if (_legendEntryHeight != value)
			{
				_legendEntryHeight = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public int numRowsOrColumns
	{
		get
		{
			return _numRowsOrColumns;
		}
		set
		{
			if (_numRowsOrColumns != value)
			{
				_numRowsOrColumns = value;
				legendC.Changed();
			}
		}
	}

	public int numDecimals
	{
		get
		{
			return _numDecimals;
		}
		set
		{
			if (_numDecimals != value)
			{
				_numDecimals = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public float legendEntryLinkSpacing
	{
		get
		{
			return _legendEntryLinkSpacing;
		}
		set
		{
			if (_legendEntryLinkSpacing != value)
			{
				_legendEntryLinkSpacing = value;
				legendC.Changed();
			}
		}
	}

	public int legendEntryFontSize
	{
		get
		{
			return _legendEntryFontSize;
		}
		set
		{
			if (_legendEntryFontSize != value)
			{
				_legendEntryFontSize = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public float legendEntrySpacing
	{
		get
		{
			return _legendEntrySpacing;
		}
		set
		{
			if (_legendEntrySpacing != value)
			{
				_legendEntrySpacing = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public float pieSwatchSize
	{
		get
		{
			return _pieSwatchSize;
		}
		set
		{
			if (_pieSwatchSize != value)
			{
				_pieSwatchSize = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public float backgroundPadding
	{
		get
		{
			return _backgroundPadding;
		}
		set
		{
			if (_backgroundPadding != value)
			{
				_backgroundPadding = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public bool autofitEnabled
	{
		get
		{
			return _autofitEnabled;
		}
		set
		{
			if (_autofitEnabled != value)
			{
				_autofitEnabled = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}

	public Color labelColor
	{
		get
		{
			return _labelColor;
		}
		set
		{
			if (_labelColor != value)
			{
				_labelColor = value;
				legendC.Changed();
			}
		}
	}

	public FontStyle legendEntryFontStyle
	{
		get
		{
			return _legendEntryFontStyle;
		}
		set
		{
			if (_legendEntryFontStyle != value)
			{
				_legendEntryFontStyle = value;
				legendC.Changed();
			}
		}
	}

	public Font legendEntryFont
	{
		get
		{
			return _legendEntryFont;
		}
		set
		{
			if (_legendEntryFont != value)
			{
				_legendEntryFont = value;
				legendC.Changed();
			}
		}
	}

	public int LegendWidth
	{
		get
		{
			return Mathf.RoundToInt(2f * backgroundPadding + legendEntryLinkSpacing + legendEntryWidth * (float)((legendType != legendTypes.Right) ? MaxInRowOrColumn : numRowsOrColumns));
		}
	}

	public int LegendHeight
	{
		get
		{
			return Mathf.RoundToInt(2f * backgroundPadding + legendEntryHeight * (float)((legendType != 0) ? MaxInRowOrColumn : numRowsOrColumns));
		}
	}

	public int NumEntries
	{
		get
		{
			int num = legendEntries.Count;
			for (int i = 0; i < legendEntries.Count; i++)
			{
				if (!activeInHierarchy(legendEntries[i].gameObject))
				{
					num--;
				}
			}
			return num;
		}
	}

	public int MaxInRowOrColumn
	{
		get
		{
			return Mathf.CeilToInt(1f * (float)NumEntries / (float)numRowsOrColumns);
		}
	}

	public float origLegendEntryWidth { get; private set; }

	public float origLegendEntryHeight { get; private set; }

	public float origLegendEntryLinkSpacing { get; private set; }

	public int origLegendEntryFontSize { get; private set; }

	public float origLegendEntrySpacing { get; private set; }

	public float origPieSwatchSize { get; private set; }

	public float origOffset { get; private set; }

	public float origBackgroundPadding { get; private set; }

	public void Init()
	{
		if (!hasInit)
		{
			hasInit = true;
			pieGraph = theGraph.GetComponent<WMG_Pie_Graph>();
			axisGraph = theGraph.GetComponent<WMG_Axis_Graph>();
			changeObjs.Add(legendC);
			setOriginalPropertyValues();
			legendC.OnChange += LegendChanged;
			PauseCallbacks();
		}
	}

	public void PauseCallbacks()
	{
		for (int i = 0; i < changeObjs.Count; i++)
		{
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
	}

	public void ResumeCallbacks()
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

	public void LegendChanged()
	{
		updateLegend();
	}

	private void setGraphCallback()
	{
		if (pieGraph != null)
		{
			pieGraph.graphC.Changed();
		}
	}

	public void setOriginalPropertyValues()
	{
		origLegendEntryWidth = legendEntryWidth;
		origLegendEntryHeight = legendEntryHeight;
		origLegendEntryLinkSpacing = legendEntryLinkSpacing;
		origLegendEntryFontSize = legendEntryFontSize;
		origLegendEntrySpacing = legendEntrySpacing;
		origPieSwatchSize = pieSwatchSize;
		origOffset = offset;
		origBackgroundPadding = backgroundPadding;
	}

	public WMG_Legend_Entry createLegendEntry(Object prefab)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		theGraph.changeSpriteParent(gameObject, entriesParent);
		WMG_Legend_Entry component = gameObject.GetComponent<WMG_Legend_Entry>();
		component.legend = this;
		legendEntries.Add(component);
		return component;
	}

	public WMG_Legend_Entry createLegendEntry(Object prefab, WMG_Series series, int index)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		theGraph.changeSpriteParent(gameObject, entriesParent);
		WMG_Legend_Entry component = gameObject.GetComponent<WMG_Legend_Entry>();
		component.seriesRef = series;
		component.legend = this;
		component.nodeLeft = theGraph.CreateNode(emptyPrefab, gameObject);
		component.nodeRight = theGraph.CreateNode(emptyPrefab, gameObject);
		legendEntries.Insert(index, component);
		return component;
	}

	public void deleteLegendEntry(int index)
	{
		Object.DestroyImmediate(legendEntries[index].gameObject);
		legendEntries.RemoveAt(index);
	}

	private bool backgroundEnabled()
	{
		int num = 1;
		if (axisGraph != null)
		{
			num = axisGraph.lineSeries.Count;
		}
		if (pieGraph != null)
		{
			num = pieGraph.sliceValues.Count;
		}
		if (!hideLegend && showBackground && num != 0)
		{
			return true;
		}
		return false;
	}

	private float getMaxLabelWidth()
	{
		float num = 0f;
		foreach (WMG_Legend_Entry legendEntry in legendEntries)
		{
			float num2 = getTextSize(legendEntry.label).x * legendEntry.label.transform.localScale.x;
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public void updateLegend()
	{
		if (backgroundEnabled() && !theGraph.activeInHierarchy(background))
		{
			theGraph.SetActive(background, true);
		}
		if (!backgroundEnabled() && theGraph.activeInHierarchy(background))
		{
			theGraph.SetActive(background, false);
		}
		if (!hideLegend && !theGraph.activeInHierarchy(entriesParent))
		{
			theGraph.SetActive(entriesParent, true);
		}
		if (hideLegend && theGraph.activeInHierarchy(entriesParent))
		{
			theGraph.SetActive(entriesParent, false);
		}
		if (hideLegend)
		{
			return;
		}
		float num = 0f;
		Vector2 vector = Vector2.zero;
		Vector2 pivot = Vector2.zero;
		Vector2 anchoredPosition = Vector2.zero;
		if (axisGraph != null)
		{
			num = axisGraph.getMaxPointSize();
		}
		if (pieGraph != null)
		{
			num = pieSwatchSize;
		}
		if (legendType == legendTypes.Bottom)
		{
			if (oppositeSideLegend)
			{
				vector = new Vector2(0.5f, 1f);
				pivot = vector;
				anchoredPosition = new Vector2(0f, 0f - offset);
			}
			else
			{
				vector = new Vector2(0.5f, 0f);
				pivot = vector;
				anchoredPosition = new Vector2(0f, offset);
			}
		}
		else if (legendType == legendTypes.Right)
		{
			if (oppositeSideLegend)
			{
				vector = new Vector2(0f, 0.5f);
				pivot = vector;
				anchoredPosition = new Vector2(offset, 0f);
			}
			else
			{
				vector = new Vector2(1f, 0.5f);
				pivot = vector;
				anchoredPosition = new Vector2(0f - offset, 0f);
			}
		}
		if (pieGraph != null)
		{
			anchoredPosition = new Vector2(-1f * anchoredPosition.x, -1f * anchoredPosition.y);
			pivot = ((legendType != 0) ? new Vector2(1f - pivot.x, pivot.y) : new Vector2(pivot.x, 1f - pivot.y));
		}
		changeSpriteWidth(base.gameObject, LegendWidth);
		changeSpriteHeight(base.gameObject, LegendHeight);
		setAnchor(base.gameObject, vector, pivot, anchoredPosition);
		setAnchor(anchoredPosition: new Vector2(legendEntryLinkSpacing + backgroundPadding + num / 2f, (0f - legendEntryHeight) / 2f + (float)LegendHeight / 2f - backgroundPadding), go: entriesParent, anchor: new Vector2(0f, 0.5f), pivot: new Vector2(0f, 0.5f));
		int numEntries = NumEntries;
		int maxInRowOrColumn = MaxInRowOrColumn;
		if (numRowsOrColumns < 1)
		{
			_numRowsOrColumns = 1;
		}
		if (numRowsOrColumns > numEntries)
		{
			_numRowsOrColumns = numEntries;
		}
		int num2 = 0;
		if (numEntries > 0)
		{
			num2 = numEntries % numRowsOrColumns;
		}
		int num3 = num2;
		int num4 = 0;
		int num5 = 0;
		bool flag = false;
		if (maxInRowOrColumn == 0)
		{
			return;
		}
		for (int i = 0; i < legendEntries.Count; i++)
		{
			WMG_Legend_Entry wMG_Legend_Entry = legendEntries[i];
			if (axisGraph != null)
			{
				if (wMG_Legend_Entry.swatchNode == null)
				{
					foreach (GameObject item in axisGraph.lineSeries)
					{
						item.GetComponent<WMG_Series>().CreateOrDeleteSpritesBasedOnPointValues();
					}
				}
				theGraph.changeSpritePositionRelativeToObjBy(wMG_Legend_Entry.nodeLeft, wMG_Legend_Entry.swatchNode, new Vector3(0f - legendEntryLinkSpacing, 0f, 0f));
				theGraph.changeSpritePositionRelativeToObjBy(wMG_Legend_Entry.nodeRight, wMG_Legend_Entry.swatchNode, new Vector3(legendEntryLinkSpacing, 0f, 0f));
				WMG_Link component = wMG_Legend_Entry.line.GetComponent<WMG_Link>();
				component.Reposition();
			}
			else
			{
				changeSpriteWidth(wMG_Legend_Entry.swatchNode, Mathf.RoundToInt(pieSwatchSize));
				changeSpriteHeight(wMG_Legend_Entry.swatchNode, Mathf.RoundToInt(pieSwatchSize));
			}
			if (axisGraph != null)
			{
				theGraph.changeSpritePositionToX(wMG_Legend_Entry.label, legendEntrySpacing);
			}
			else
			{
				theGraph.changeSpritePositionToX(wMG_Legend_Entry.label, legendEntrySpacing + pieSwatchSize / 2f);
			}
			if (axisGraph != null)
			{
				string aText = wMG_Legend_Entry.seriesRef.seriesName;
				if (labelType == WMG_Enums.labelTypes.None)
				{
					aText = string.Empty;
				}
				changeLabelText(wMG_Legend_Entry.label, aText);
			}
			changeLabelFontSize(wMG_Legend_Entry.label, legendEntryFontSize);
			changeSpriteColor(wMG_Legend_Entry.label, labelColor);
			changeLabelFontStyle(wMG_Legend_Entry.label, legendEntryFontStyle);
			if (legendEntryFont != null)
			{
				changeLabelFont(wMG_Legend_Entry.label, legendEntryFont);
			}
			int num6 = Mathf.FloorToInt(i / maxInRowOrColumn);
			if (num3 > 0)
			{
				num6 = Mathf.FloorToInt((i + 1) / maxInRowOrColumn);
			}
			if (num2 == 0 && num3 > 0)
			{
				num6 = num3 + Mathf.FloorToInt((i - num3 * maxInRowOrColumn) / (maxInRowOrColumn - 1));
				if (i - num3 * maxInRowOrColumn > 0)
				{
					flag = true;
				}
			}
			if (num2 > 0 && (i + 1) % maxInRowOrColumn == 0)
			{
				num2--;
				num6--;
			}
			if (num5 != num6)
			{
				num5 = num6;
				num4 = ((!flag) ? (num4 + maxInRowOrColumn) : (num4 + (maxInRowOrColumn - 1)));
			}
			if (legendType == legendTypes.Bottom)
			{
				theGraph.changeSpritePositionTo(wMG_Legend_Entry.gameObject, new Vector3((float)i * legendEntryWidth - legendEntryWidth * (float)num4, (float)(-num6) * legendEntryHeight, 0f));
			}
			else if (legendType == legendTypes.Right)
			{
				theGraph.changeSpritePositionTo(wMG_Legend_Entry.gameObject, new Vector3((float)num6 * legendEntryWidth, (float)(-i) * legendEntryHeight + legendEntryHeight * (float)num4, 0f));
			}
		}
		if (setWidthFromLabels)
		{
			if (axisGraph != null && (axisGraph.graphType == WMG_Axis_Graph.graphTypes.line || axisGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked))
			{
				legendEntryWidth = Mathf.Max(legendEntryLinkSpacing, num / 2f) + legendEntrySpacing + getMaxLabelWidth() + 5f;
			}
			else
			{
				legendEntryWidth = num + legendEntrySpacing + getMaxLabelWidth() + 5f;
			}
		}
		if (!autofitEnabled)
		{
			return;
		}
		if (legendType == legendTypes.Bottom)
		{
			if ((float)LegendWidth > getSpriteWidth(theGraph.gameObject))
			{
				if (numRowsOrColumns < NumEntries)
				{
					numRowsOrColumns++;
				}
			}
			else if (numRowsOrColumns > 1)
			{
				_numRowsOrColumns--;
				if ((float)LegendWidth > getSpriteWidth(theGraph.gameObject))
				{
					_numRowsOrColumns++;
					return;
				}
				_numRowsOrColumns++;
				numRowsOrColumns--;
			}
		}
		else if ((float)LegendHeight > getSpriteHeight(theGraph.gameObject))
		{
			if (numRowsOrColumns < NumEntries)
			{
				numRowsOrColumns++;
			}
		}
		else if (numRowsOrColumns > 1)
		{
			_numRowsOrColumns--;
			if ((float)LegendHeight > getSpriteHeight(theGraph.gameObject))
			{
				_numRowsOrColumns++;
				return;
			}
			_numRowsOrColumns++;
			numRowsOrColumns--;
		}
	}

	public void setLabelScales(float newScale)
	{
		foreach (WMG_Legend_Entry legendEntry in legendEntries)
		{
			legendEntry.label.transform.localScale = new Vector3(newScale, newScale, 1f);
		}
	}
}
