using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Series : MonoBehaviour
{
	public enum comboTypes
	{
		line = 0,
		bar = 1
	}

	public enum areaShadingTypes
	{
		None = 0,
		Solid = 1,
		Gradient = 2
	}

	public delegate string SeriesDataLabeler(WMG_Series series, float val);

	public delegate void SeriesDataChangedHandler(WMG_Series aSeries);

	[SerializeField]
	private List<Vector2> _pointValues;

	public WMG_List<Vector2> pointValues = new WMG_List<Vector2>();

	[SerializeField]
	private List<Color> _pointColors;

	public WMG_List<Color> pointColors = new WMG_List<Color>();

	[Obsolete("This parameter is no longer used. Use ManuallySetXDistBetween if needed.")]
	public bool AutoUpdateXDistBetween;

	public UnityEngine.Object dataLabelPrefab;

	public GameObject dataLabelsParent;

	public Material areaShadingMatSolid;

	public Material areaShadingMatGradient;

	public GameObject areaShadingParent;

	public UnityEngine.Object areaShadingPrefab;

	public UnityEngine.Object areaShadingCSPrefab;

	public WMG_Axis_Graph theGraph;

	public WMG_Data_Source realTimeDataSource;

	public WMG_Data_Source pointValuesDataSource;

	public UnityEngine.Object legendEntryPrefab;

	public GameObject linkParent;

	public GameObject nodeParent;

	public WMG_Legend_Entry legendEntry;

	[SerializeField]
	private comboTypes _comboType;

	[SerializeField]
	private bool _useSecondYaxis;

	[SerializeField]
	private string _seriesName;

	[SerializeField]
	private float _pointWidthHeight;

	[SerializeField]
	private float _lineScale;

	[SerializeField]
	private Color _pointColor;

	[SerializeField]
	private bool _usePointColors;

	[SerializeField]
	private Color _lineColor;

	[SerializeField]
	private bool _UseXDistBetweenToSpace;

	[SerializeField]
	private bool _ManuallySetXDistBetween;

	[SerializeField]
	private float _xDistBetweenPoints;

	[SerializeField]
	private bool _ManuallySetExtraXSpace;

	[SerializeField]
	private float _extraXSpace;

	[SerializeField]
	private bool _hidePoints;

	[SerializeField]
	private bool _hideLines;

	[SerializeField]
	private bool _connectFirstToLast;

	[SerializeField]
	private float _linePadding;

	[SerializeField]
	private bool _dataLabelsEnabled;

	[SerializeField]
	private int _dataLabelsNumDecimals;

	[SerializeField]
	private int _dataLabelsFontSize;

	[SerializeField]
	private Color _dataLabelsColor = Color.white;

	[SerializeField]
	private FontStyle _dataLabelsFontStyle;

	[SerializeField]
	private Font _dataLabelsFont;

	[SerializeField]
	private Vector2 _dataLabelsOffset;

	[SerializeField]
	private areaShadingTypes _areaShadingType;

	[SerializeField]
	private bool _areaShadingUsesComputeShader;

	[SerializeField]
	private Color _areaShadingColor;

	[SerializeField]
	private float _areaShadingAxisValue;

	[SerializeField]
	private int _pointPrefab;

	[SerializeField]
	private int _linkPrefab;

	private UnityEngine.Object nodePrefab;

	private List<GameObject> points = new List<GameObject>();

	private List<GameObject> lines = new List<GameObject>();

	private List<GameObject> areaShadingRects = new List<GameObject>();

	private List<GameObject> dataLabels = new List<GameObject>();

	private List<bool> barIsNegative = new List<bool>();

	private List<int> changedValIndices = new List<int>();

	private WMG_Axis_Graph.graphTypes cachedSeriesType;

	private bool realTimeRunning;

	private float realTimeLoopVar;

	private float realTimeOrigMax;

	private bool animatingFromPreviousData;

	private List<Vector2> afterPositions = new List<Vector2>();

	private List<int> afterWidths = new List<int>();

	private List<int> afterHeights = new List<int>();

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	public WMG_Change_Obj pointValuesC = new WMG_Change_Obj();

	public WMG_Change_Obj pointValuesCountC = new WMG_Change_Obj();

	private WMG_Change_Obj pointValuesValC = new WMG_Change_Obj();

	private WMG_Change_Obj lineScaleC = new WMG_Change_Obj();

	private WMG_Change_Obj pointWidthHeightC = new WMG_Change_Obj();

	private WMG_Change_Obj dataLabelsC = new WMG_Change_Obj();

	private WMG_Change_Obj lineColorC = new WMG_Change_Obj();

	private WMG_Change_Obj pointColorC = new WMG_Change_Obj();

	private WMG_Change_Obj hideLineC = new WMG_Change_Obj();

	private WMG_Change_Obj hidePointC = new WMG_Change_Obj();

	private WMG_Change_Obj seriesNameC = new WMG_Change_Obj();

	private WMG_Change_Obj linePaddingC = new WMG_Change_Obj();

	private WMG_Change_Obj areaShadingTypeC = new WMG_Change_Obj();

	private WMG_Change_Obj areaShadingC = new WMG_Change_Obj();

	public WMG_Change_Obj prefabC = new WMG_Change_Obj();

	private WMG_Change_Obj connectFirstToLastC = new WMG_Change_Obj();

	private bool hasInit;

	public SeriesDataLabeler seriesDataLabeler;

	public comboTypes comboType
	{
		get
		{
			return _comboType;
		}
		set
		{
			if (_comboType != value)
			{
				_comboType = value;
				prefabC.Changed();
			}
		}
	}

	public bool useSecondYaxis
	{
		get
		{
			return _useSecondYaxis;
		}
		set
		{
			if (_useSecondYaxis != value && (theGraph.axesType == WMG_Axis_Graph.axesTypes.DUAL_Y || !value))
			{
				_useSecondYaxis = value;
				pointValuesC.Changed();
			}
		}
	}

	public string seriesName
	{
		get
		{
			return _seriesName;
		}
		set
		{
			if (_seriesName != value)
			{
				_seriesName = value;
				seriesNameC.Changed();
			}
		}
	}

	public float pointWidthHeight
	{
		get
		{
			return _pointWidthHeight;
		}
		set
		{
			if (_pointWidthHeight != value)
			{
				_pointWidthHeight = value;
				pointWidthHeightC.Changed();
			}
		}
	}

	public float lineScale
	{
		get
		{
			return _lineScale;
		}
		set
		{
			if (_lineScale != value)
			{
				_lineScale = value;
				lineScaleC.Changed();
			}
		}
	}

	public Color pointColor
	{
		get
		{
			return _pointColor;
		}
		set
		{
			if (_pointColor != value)
			{
				_pointColor = value;
				pointColorC.Changed();
			}
		}
	}

	public bool usePointColors
	{
		get
		{
			return _usePointColors;
		}
		set
		{
			if (_usePointColors != value)
			{
				_usePointColors = value;
				pointColorC.Changed();
			}
		}
	}

	public Color lineColor
	{
		get
		{
			return _lineColor;
		}
		set
		{
			if (_lineColor != value)
			{
				_lineColor = value;
				lineColorC.Changed();
			}
		}
	}

	public bool UseXDistBetweenToSpace
	{
		get
		{
			return _UseXDistBetweenToSpace;
		}
		set
		{
			if (_UseXDistBetweenToSpace != value)
			{
				_UseXDistBetweenToSpace = value;
				pointValuesC.Changed();
			}
		}
	}

	public bool ManuallySetXDistBetween
	{
		get
		{
			return _ManuallySetXDistBetween;
		}
		set
		{
			if (_ManuallySetXDistBetween != value)
			{
				_ManuallySetXDistBetween = value;
				pointValuesC.Changed();
			}
		}
	}

	public float xDistBetweenPoints
	{
		get
		{
			return _xDistBetweenPoints;
		}
		set
		{
			if (_xDistBetweenPoints != value)
			{
				_xDistBetweenPoints = value;
				pointValuesC.Changed();
			}
		}
	}

	public bool ManuallySetExtraXSpace
	{
		get
		{
			return _ManuallySetExtraXSpace;
		}
		set
		{
			if (_ManuallySetExtraXSpace != value)
			{
				_ManuallySetExtraXSpace = value;
				pointValuesC.Changed();
			}
		}
	}

	public float extraXSpace
	{
		get
		{
			return _extraXSpace;
		}
		set
		{
			if (_extraXSpace != value)
			{
				_extraXSpace = value;
				pointValuesC.Changed();
			}
		}
	}

	public bool hidePoints
	{
		get
		{
			return _hidePoints;
		}
		set
		{
			if (_hidePoints != value)
			{
				_hidePoints = value;
				hidePointC.Changed();
			}
		}
	}

	public bool hideLines
	{
		get
		{
			return _hideLines;
		}
		set
		{
			if (_hideLines != value)
			{
				_hideLines = value;
				hideLineC.Changed();
			}
		}
	}

	public bool connectFirstToLast
	{
		get
		{
			return _connectFirstToLast;
		}
		set
		{
			if (_connectFirstToLast != value)
			{
				_connectFirstToLast = value;
				connectFirstToLastC.Changed();
				lineScaleC.Changed();
				linePaddingC.Changed();
				hideLineC.Changed();
				lineColorC.Changed();
			}
		}
	}

	public float linePadding
	{
		get
		{
			return _linePadding;
		}
		set
		{
			if (_linePadding != value)
			{
				_linePadding = value;
				linePaddingC.Changed();
			}
		}
	}

	public bool dataLabelsEnabled
	{
		get
		{
			return _dataLabelsEnabled;
		}
		set
		{
			if (_dataLabelsEnabled != value)
			{
				_dataLabelsEnabled = value;
				dataLabelsC.Changed();
			}
		}
	}

	public int dataLabelsNumDecimals
	{
		get
		{
			return _dataLabelsNumDecimals;
		}
		set
		{
			if (_dataLabelsNumDecimals != value)
			{
				_dataLabelsNumDecimals = value;
				dataLabelsC.Changed();
			}
		}
	}

	public int dataLabelsFontSize
	{
		get
		{
			return _dataLabelsFontSize;
		}
		set
		{
			if (_dataLabelsFontSize != value)
			{
				_dataLabelsFontSize = value;
				dataLabelsC.Changed();
			}
		}
	}

	public Color dataLabelsColor
	{
		get
		{
			return _dataLabelsColor;
		}
		set
		{
			if (_dataLabelsColor != value)
			{
				_dataLabelsColor = value;
				dataLabelsC.Changed();
			}
		}
	}

	public FontStyle dataLabelsFontStyle
	{
		get
		{
			return _dataLabelsFontStyle;
		}
		set
		{
			if (_dataLabelsFontStyle != value)
			{
				_dataLabelsFontStyle = value;
				dataLabelsC.Changed();
			}
		}
	}

	public Font dataLabelsFont
	{
		get
		{
			return _dataLabelsFont;
		}
		set
		{
			if (_dataLabelsFont != value)
			{
				_dataLabelsFont = value;
				dataLabelsC.Changed();
			}
		}
	}

	public Vector2 dataLabelsOffset
	{
		get
		{
			return _dataLabelsOffset;
		}
		set
		{
			if (_dataLabelsOffset != value)
			{
				_dataLabelsOffset = value;
				dataLabelsC.Changed();
			}
		}
	}

	public areaShadingTypes areaShadingType
	{
		get
		{
			return _areaShadingType;
		}
		set
		{
			if (_areaShadingType != value)
			{
				_areaShadingType = value;
				areaShadingTypeC.Changed();
			}
		}
	}

	public bool areaShadingUsesComputeShader
	{
		get
		{
			return _areaShadingUsesComputeShader;
		}
		set
		{
			if (_areaShadingUsesComputeShader != value)
			{
				_areaShadingUsesComputeShader = value;
				areaShadingTypeC.Changed();
			}
		}
	}

	public Color areaShadingColor
	{
		get
		{
			return _areaShadingColor;
		}
		set
		{
			if (_areaShadingColor != value)
			{
				_areaShadingColor = value;
				areaShadingC.Changed();
			}
		}
	}

	public float areaShadingAxisValue
	{
		get
		{
			return _areaShadingAxisValue;
		}
		set
		{
			if (_areaShadingAxisValue != value)
			{
				_areaShadingAxisValue = value;
				areaShadingC.Changed();
			}
		}
	}

	public int pointPrefab
	{
		get
		{
			return _pointPrefab;
		}
		set
		{
			if (_pointPrefab != value)
			{
				_pointPrefab = value;
				prefabC.Changed();
			}
		}
	}

	public int linkPrefab
	{
		get
		{
			return _linkPrefab;
		}
		set
		{
			if (_linkPrefab != value)
			{
				_linkPrefab = value;
				prefabC.Changed();
			}
		}
	}

	public bool seriesIsLine
	{
		get
		{
			return theGraph.graphType == WMG_Axis_Graph.graphTypes.line || theGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked || (theGraph.graphType == WMG_Axis_Graph.graphTypes.combo && comboType == comboTypes.line);
		}
	}

	public bool IsLast
	{
		get
		{
			return theGraph.lineSeries[theGraph.lineSeries.Count - 1].GetComponent<WMG_Series>() == this;
		}
	}

	public WMG_Axis yAxis
	{
		get
		{
			if (theGraph.axesType == WMG_Axis_Graph.axesTypes.DUAL_Y && useSecondYaxis && theGraph.yAxis2 != null)
			{
				return theGraph.yAxis2;
			}
			return theGraph.yAxis;
		}
	}

	public float origPointWidthHeight { get; private set; }

	public float origLineScale { get; private set; }

	public int origDataLabelsFontSize { get; private set; }

	public Vector2 origDataLabelOffset { get; set; }

	public bool currentlyAnimating { get; set; }

	public event SeriesDataChangedHandler SeriesDataChanged;

	public string formatSeriesDataLabel(WMG_Series series, float val)
	{
		float num = Mathf.Pow(10f, series.dataLabelsNumDecimals);
		return (Mathf.Round(val * num) / num).ToString();
	}

	protected virtual void OnSeriesDataChanged()
	{
		SeriesDataChangedHandler seriesDataChangedHandler = this.SeriesDataChanged;
		if (seriesDataChangedHandler != null)
		{
			seriesDataChangedHandler(this);
		}
	}

	public void Init(int index)
	{
		if (!hasInit)
		{
			hasInit = true;
			changeObjs.Add(pointValuesCountC);
			changeObjs.Add(pointValuesC);
			changeObjs.Add(pointValuesValC);
			changeObjs.Add(connectFirstToLastC);
			changeObjs.Add(lineScaleC);
			changeObjs.Add(pointWidthHeightC);
			changeObjs.Add(dataLabelsC);
			changeObjs.Add(lineColorC);
			changeObjs.Add(pointColorC);
			changeObjs.Add(hideLineC);
			changeObjs.Add(hidePointC);
			changeObjs.Add(seriesNameC);
			changeObjs.Add(linePaddingC);
			changeObjs.Add(areaShadingTypeC);
			changeObjs.Add(areaShadingC);
			changeObjs.Add(prefabC);
			if (seriesIsLine)
			{
				nodePrefab = theGraph.pointPrefabs[pointPrefab];
			}
			else
			{
				nodePrefab = theGraph.barPrefab;
			}
			legendEntry = theGraph.legend.createLegendEntry(legendEntryPrefab, this, index);
			createLegendSwatch();
			theGraph.legend.updateLegend();
			pointValues.SetList(_pointValues);
			pointValues.Changed += pointValuesListChanged;
			pointColors.SetList(_pointColors);
			pointColors.Changed += pointColorsListChanged;
			pointValuesCountC.OnChange += PointValuesCountChanged;
			pointValuesC.OnChange += PointValuesChanged;
			pointValuesValC.OnChange += PointValuesValChanged;
			lineScaleC.OnChange += LineScaleChanged;
			pointWidthHeightC.OnChange += PointWidthHeightChanged;
			dataLabelsC.OnChange += DataLabelsChanged;
			lineColorC.OnChange += LineColorChanged;
			pointColorC.OnChange += PointColorChanged;
			hideLineC.OnChange += HideLinesChanged;
			hidePointC.OnChange += HidePointsChanged;
			seriesNameC.OnChange += SeriesNameChanged;
			linePaddingC.OnChange += LinePaddingChanged;
			areaShadingTypeC.OnChange += AreaShadingTypeChanged;
			areaShadingC.OnChange += AreaShadingChanged;
			prefabC.OnChange += PrefabChanged;
			connectFirstToLastC.OnChange += ConnectFirstToLastChanged;
			seriesDataLabeler = formatSeriesDataLabel;
			setOriginalPropertyValues();
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

	public void pointColorsListChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref pointColors, ref _pointColors, oneValChanged, index);
		pointColorC.Changed();
	}

	public void pointValuesListChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref pointValues, ref _pointValues, oneValChanged, index);
		if (countChanged)
		{
			pointValuesCountC.Changed();
			return;
		}
		setAnimatingFromPreviousData();
		if (oneValChanged)
		{
			changedValIndices.Add(index);
			pointValuesValC.Changed();
		}
		else
		{
			pointValuesC.Changed();
		}
	}

	public void PrefabChanged()
	{
		UpdatePrefabType();
		pointValuesCountC.Changed();
	}

	public void pointValuesChanged()
	{
		theGraph.aSeriesPointsChanged();
		UpdateNullVisibility();
		UpdateSprites();
	}

	public void pointValuesCountChanged()
	{
		theGraph.aSeriesPointsChanged();
		CreateOrDeleteSpritesBasedOnPointValues();
		UpdateLineColor();
		UpdatePointColor();
		UpdateLineScale();
		UpdatePointWidthHeight();
		UpdateHideLines();
		UpdateHidePoints();
		UpdateNullVisibility();
		UpdateLinePadding();
		UpdateSprites();
	}

	public void pointValuesValChanged(int index)
	{
		theGraph.aSeriesPointsChanged();
		UpdateNullVisibility();
		UpdateSprites();
	}

	public void PointValuesChanged()
	{
		if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent || (theGraph.IsStacked && !IsLast))
		{
			theGraph.aSeriesPointsChanged();
			theGraph.SeriesChanged(false, true);
		}
		else
		{
			pointValuesChanged();
		}
	}

	public void PointValuesCountChanged()
	{
		if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent || (theGraph.IsStacked && !IsLast))
		{
			theGraph.aSeriesPointsChanged();
			theGraph.SeriesChanged(true, true);
		}
		else
		{
			pointValuesCountChanged();
		}
	}

	public void PointValuesValChanged()
	{
		if (changedValIndices.Count != 1)
		{
			PointValuesChanged();
			return;
		}
		if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent || (theGraph.IsStacked && !IsLast))
		{
			theGraph.aSeriesPointsChanged();
			theGraph.SeriesChanged(false, true);
		}
		else
		{
			pointValuesValChanged(changedValIndices[0]);
		}
		changedValIndices.Clear();
	}

	public void LineColorChanged()
	{
		UpdateLineColor();
	}

	public void ConnectFirstToLastChanged()
	{
		createOrDeletePoints(pointValues.Count);
	}

	public void PointColorChanged()
	{
		UpdatePointColor();
	}

	public void LineScaleChanged()
	{
		UpdateLineScale();
	}

	public void PointWidthHeightChanged()
	{
		UpdatePointWidthHeight();
	}

	public void HideLinesChanged()
	{
		UpdateHideLines();
		UpdateNullVisibility();
	}

	public void HidePointsChanged()
	{
		UpdateHidePoints();
		UpdateNullVisibility();
	}

	public void SeriesNameChanged()
	{
		UpdateSeriesName();
	}

	public void LinePaddingChanged()
	{
		UpdateLinePadding();
	}

	public void AreaShadingTypeChanged()
	{
		createOrDeleteAreaShading(pointValues.Count);
	}

	public void AreaShadingChanged()
	{
		if (areaShadingUsesComputeShader)
		{
			UpdateSprites();
		}
		else
		{
			updateAreaShading(null);
		}
	}

	public void DataLabelsChanged()
	{
		createOrDeleteLabels(pointValues.Count);
		updateDataLabels();
	}

	public void UpdateFromDataSource()
	{
		if (pointValuesDataSource != null)
		{
			List<Vector2> list = pointValuesDataSource.getData<Vector2>();
			if (theGraph.useGroups)
			{
				list = sanitizeGroupData(list);
			}
			pointValues.SetList(list);
		}
	}

	public void RealTimeUpdate()
	{
		if (realTimeRunning)
		{
			DoRealTimeUpdate();
		}
	}

	public List<Vector2> AfterPositions()
	{
		return afterPositions;
	}

	public List<int> AfterHeights()
	{
		return afterHeights;
	}

	public List<int> AfterWidths()
	{
		return afterWidths;
	}

	public bool AnimatingFromPreviousData()
	{
		return animatingFromPreviousData;
	}

	public void setAnimatingFromPreviousData()
	{
		if (!realTimeRunning && !theGraph.IsStacked && theGraph.autoAnimationsEnabled)
		{
			animatingFromPreviousData = true;
		}
	}

	public void setOriginalPropertyValues()
	{
		origPointWidthHeight = pointWidthHeight;
		origLineScale = lineScale;
		origDataLabelsFontSize = dataLabelsFontSize;
		origDataLabelOffset = dataLabelsOffset;
	}

	public List<GameObject> getPoints()
	{
		return points;
	}

	public GameObject getLastPoint()
	{
		return points[points.Count - 1];
	}

	public GameObject getFirstPoint()
	{
		return points[0];
	}

	public List<GameObject> getLines()
	{
		return lines;
	}

	public List<GameObject> getDataLabels()
	{
		return dataLabels;
	}

	public bool getBarIsNegative(int i)
	{
		return barIsNegative[i];
	}

	public Vector2 getNodeValue(WMG_Node aNode)
	{
		for (int i = 0; i < pointValues.Count; i++)
		{
			if (points[i].GetComponent<WMG_Node>() == aNode)
			{
				return pointValues[i];
			}
		}
		return Vector2.zero;
	}

	public void UpdateHidePoints()
	{
		for (int i = 0; i < points.Count; i++)
		{
			theGraph.SetActive(points[i], !hidePoints);
		}
		theGraph.SetActive(legendEntry.swatchNode, !hidePoints);
		if (!areaShadingUsesComputeShader)
		{
			StartCoroutine(SetDelayedAreaShadingChanged());
		}
	}

	public void UpdateNullVisibility()
	{
		if (theGraph.useGroups)
		{
			for (int i = 0; i < points.Count; i++)
			{
				theGraph.SetActive(points[i], pointValues[i].x > 0f);
			}
			if (seriesIsLine)
			{
				for (int j = 0; j < lines.Count; j++)
				{
					theGraph.SetActive(lines[j], true);
				}
				for (int k = 0; k < points.Count; k++)
				{
					if (pointValues[k].x < 0f)
					{
						WMG_Node component = points[k].GetComponent<WMG_Node>();
						for (int l = 0; l < component.links.Count; l++)
						{
							theGraph.SetActive(component.links[l], false);
						}
					}
				}
			}
			if (!areaShadingUsesComputeShader)
			{
				StartCoroutine(SetDelayedAreaShadingChanged());
			}
		}
		if (hidePoints)
		{
			for (int m = 0; m < points.Count; m++)
			{
				theGraph.SetActive(points[m], false);
			}
		}
		if (hideLines || !seriesIsLine)
		{
			for (int n = 0; n < lines.Count; n++)
			{
				theGraph.SetActive(lines[n], false);
			}
		}
	}

	public void UpdateHideLines()
	{
		for (int i = 0; i < lines.Count; i++)
		{
			if (hideLines || !seriesIsLine)
			{
				theGraph.SetActive(lines[i], false);
			}
			else
			{
				theGraph.SetActive(lines[i], true);
			}
		}
		if (hideLines || !seriesIsLine)
		{
			theGraph.SetActive(legendEntry.line, false);
		}
		else
		{
			theGraph.SetActive(legendEntry.line, true);
		}
		if (!areaShadingUsesComputeShader)
		{
			StartCoroutine(SetDelayedAreaShadingChanged());
		}
	}

	public void UpdateLineColor()
	{
		for (int i = 0; i < lines.Count; i++)
		{
			WMG_Link component = lines[i].GetComponent<WMG_Link>();
			theGraph.changeSpriteColor(component.objectToColor, lineColor);
		}
		WMG_Link component2 = legendEntry.line.GetComponent<WMG_Link>();
		theGraph.changeSpriteColor(component2.objectToColor, lineColor);
	}

	public void UpdatePointColor()
	{
		for (int i = 0; i < points.Count; i++)
		{
			WMG_Node component = points[i].GetComponent<WMG_Node>();
			if (usePointColors)
			{
				if (i < pointColors.Count)
				{
					theGraph.changeSpriteColor(component.objectToColor, pointColors[i]);
				}
			}
			else
			{
				theGraph.changeSpriteColor(component.objectToColor, pointColor);
			}
		}
		WMG_Node component2 = legendEntry.swatchNode.GetComponent<WMG_Node>();
		theGraph.changeSpriteColor(component2.objectToColor, pointColor);
	}

	public void UpdateLineScale()
	{
		for (int i = 0; i < lines.Count; i++)
		{
			WMG_Link component = lines[i].GetComponent<WMG_Link>();
			component.objectToScale.transform.localScale = new Vector3(lineScale, component.objectToScale.transform.localScale.y, component.objectToScale.transform.localScale.z);
		}
		WMG_Link component2 = legendEntry.line.GetComponent<WMG_Link>();
		component2.objectToScale.transform.localScale = new Vector3(lineScale, component2.objectToScale.transform.localScale.y, component2.objectToScale.transform.localScale.z);
	}

	public void UpdatePointWidthHeight()
	{
		if (seriesIsLine)
		{
			for (int i = 0; i < points.Count; i++)
			{
				WMG_Node component = points[i].GetComponent<WMG_Node>();
				theGraph.changeSpriteHeight(component.objectToColor, Mathf.RoundToInt(pointWidthHeight));
				theGraph.changeSpriteWidth(component.objectToColor, Mathf.RoundToInt(pointWidthHeight));
			}
		}
		WMG_Node component2 = legendEntry.swatchNode.GetComponent<WMG_Node>();
		theGraph.changeSpriteHeight(component2.objectToColor, Mathf.RoundToInt(pointWidthHeight));
		theGraph.changeSpriteWidth(component2.objectToColor, Mathf.RoundToInt(pointWidthHeight));
	}

	public void UpdatePrefabType()
	{
		if (seriesIsLine)
		{
			nodePrefab = theGraph.pointPrefabs[pointPrefab];
		}
		else
		{
			nodePrefab = theGraph.barPrefab;
		}
		for (int num = points.Count - 1; num >= 0; num--)
		{
			if (points[num] != null)
			{
				WMG_Node component = points[num].GetComponent<WMG_Node>();
				foreach (GameObject link in component.links)
				{
					lines.Remove(link);
				}
				theGraph.DeleteNode(component);
				points.RemoveAt(num);
			}
		}
		if (legendEntry.swatchNode != null)
		{
			theGraph.DeleteNode(legendEntry.swatchNode.GetComponent<WMG_Node>());
			theGraph.DeleteLink(legendEntry.line.GetComponent<WMG_Link>());
		}
	}

	public void UpdateSeriesName()
	{
		theGraph.legend.LegendChanged();
	}

	public void UpdateLinePadding()
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i].GetComponent<WMG_Node>().radius = -1f * linePadding;
		}
		RepositionLines();
	}

	public void RepositionLines()
	{
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].GetComponent<WMG_Link>().Reposition();
		}
	}

	public void CreateOrDeleteSpritesBasedOnPointValues()
	{
		if (theGraph.useGroups)
		{
			pointValues.SetListNoCb(sanitizeGroupData(pointValues.list), ref _pointValues);
		}
		int count = pointValues.Count;
		createOrDeletePoints(count);
		createOrDeleteLabels(count);
		createOrDeleteAreaShading(count);
	}

	private List<Vector2> sanitizeGroupData(List<Vector2> groupData)
	{
		for (int num = groupData.Count - 1; num >= 0; num--)
		{
			int num2 = Mathf.RoundToInt(groupData[num].x);
			if ((float)num2 - groupData[num].x != 0f)
			{
				groupData.RemoveAt(num);
			}
			else if (Mathf.Abs(num2) > theGraph.groups.Count)
			{
				groupData.RemoveAt(num);
			}
			else if (num2 == 0)
			{
				groupData.RemoveAt(num);
			}
		}
		groupData.Sort((Vector2 vec1, Vector2 vec2) => vec1.x.CompareTo(vec2.x));
		List<Vector2> list = new List<Vector2>();
		bool flag = true;
		for (int i = 0; i < groupData.Count; i++)
		{
			if (flag)
			{
				list.Add(groupData[i]);
				flag = false;
			}
			else
			{
				Vector2 vector = list[list.Count - 1];
				list[list.Count - 1] = new Vector2(vector.x, vector.y + groupData[i].y);
			}
			if (i < groupData.Count - 1 && groupData[i].x != groupData[i + 1].x)
			{
				flag = true;
			}
		}
		if (list.Count < theGraph.groups.Count)
		{
			int num3 = theGraph.groups.Count - list.Count;
			for (int j = 0; j < num3; j++)
			{
				list.Insert(0, new Vector2(-1f, 0f));
			}
		}
		if (list.Count > theGraph.groups.Count)
		{
			int num4 = list.Count - theGraph.groups.Count;
			for (int k = 0; k < num4; k++)
			{
				list.RemoveAt(0);
			}
		}
		List<int> list2 = new List<int>();
		for (int l = 0; l < theGraph.groups.Count; l++)
		{
			list2.Add(l + 1);
		}
		for (int num5 = list.Count - 1; num5 >= 0; num5--)
		{
			if (list[num5].x > 0f)
			{
				list2.Remove(Mathf.RoundToInt(list[num5].x));
			}
		}
		for (int m = 0; m < list2.Count; m++)
		{
			list[m] = new Vector2(-1 * list2[m], 0f);
		}
		list.Sort((Vector2 vec1, Vector2 vec2) => Mathf.Abs(vec1.x).CompareTo(Mathf.Abs(vec2.x)));
		return list;
	}

	private void createOrDeletePoints(int pointValuesCount)
	{
		for (int i = 0; i < pointValuesCount; i++)
		{
			if (points.Count <= i)
			{
				GameObject gameObject = theGraph.CreateNode(nodePrefab, nodeParent);
				theGraph.addNodeClickEvent(gameObject);
				theGraph.addNodeMouseEnterEvent(gameObject);
				theGraph.addNodeMouseLeaveEvent(gameObject);
				gameObject.GetComponent<WMG_Node>().radius = -1f * linePadding;
				theGraph.SetActive(gameObject, false);
				points.Add(gameObject);
				barIsNegative.Add(false);
				if (i > 0)
				{
					WMG_Node component = points[i - 1].GetComponent<WMG_Node>();
					gameObject = theGraph.CreateLink(component, gameObject, theGraph.linkPrefabs[linkPrefab], linkParent);
					theGraph.addLinkClickEvent(gameObject);
					theGraph.addLinkMouseEnterEvent(gameObject);
					theGraph.addLinkMouseLeaveEvent(gameObject);
					theGraph.SetActive(gameObject, false);
					lines.Add(gameObject);
				}
			}
		}
		for (int num = points.Count - 1; num >= 0; num--)
		{
			if (points[num] != null && num >= pointValuesCount)
			{
				WMG_Node component2 = points[num].GetComponent<WMG_Node>();
				foreach (GameObject link3 in component2.links)
				{
					lines.Remove(link3);
				}
				theGraph.DeleteNode(component2);
				points.RemoveAt(num);
				barIsNegative.RemoveAt(num);
			}
			if (num > 1 && num < pointValuesCount - 1)
			{
				WMG_Node component3 = points[0].GetComponent<WMG_Node>();
				WMG_Node component4 = points[num].GetComponent<WMG_Node>();
				WMG_Link link = theGraph.GetLink(component3, component4);
				if (link != null)
				{
					lines.Remove(link.gameObject);
					theGraph.DeleteLink(link);
				}
			}
		}
		if (points.Count > 2)
		{
			WMG_Node component5 = points[0].GetComponent<WMG_Node>();
			WMG_Node component6 = points[points.Count - 1].GetComponent<WMG_Node>();
			WMG_Link link2 = theGraph.GetLink(component5, component6);
			if (connectFirstToLast && link2 == null)
			{
				GameObject gameObject2 = theGraph.CreateLink(component5, component6.gameObject, theGraph.linkPrefabs[linkPrefab], linkParent);
				theGraph.addLinkClickEvent(gameObject2);
				theGraph.addLinkMouseEnterEvent(gameObject2);
				theGraph.addLinkMouseLeaveEvent(gameObject2);
				theGraph.SetActive(gameObject2, false);
				lines.Add(gameObject2);
			}
			if (!connectFirstToLast && link2 != null)
			{
				lines.Remove(link2.gameObject);
				theGraph.DeleteLink(link2);
			}
		}
		if (legendEntry.swatchNode == null)
		{
			createLegendSwatch();
		}
	}

	private void createLegendSwatch()
	{
		legendEntry.swatchNode = theGraph.CreateNode(nodePrefab, legendEntry.gameObject);
		theGraph.addNodeClickEvent_Leg(legendEntry.swatchNode);
		theGraph.addNodeMouseEnterEvent_Leg(legendEntry.swatchNode);
		theGraph.addNodeMouseLeaveEvent_Leg(legendEntry.swatchNode);
		WMG_Node component = legendEntry.swatchNode.GetComponent<WMG_Node>();
		theGraph.changeSpritePivot(component.objectToColor, WMG_Text_Functions.WMGpivotTypes.Center);
		component.Reposition(0f, 0f);
		legendEntry.line = theGraph.CreateLink(legendEntry.nodeRight.GetComponent<WMG_Node>(), legendEntry.nodeLeft, theGraph.linkPrefabs[linkPrefab], legendEntry.gameObject);
		theGraph.addLinkClickEvent_Leg(legendEntry.line);
		theGraph.addLinkMouseEnterEvent_Leg(legendEntry.line);
		theGraph.addLinkMouseLeaveEvent_Leg(legendEntry.line);
		theGraph.bringSpriteToFront(legendEntry.swatchNode);
	}

	private void createOrDeleteLabels(int pointValuesCount)
	{
		if (!(dataLabelPrefab != null) || !(dataLabelsParent != null))
		{
			return;
		}
		if (dataLabelsEnabled)
		{
			for (int i = 0; i < pointValuesCount; i++)
			{
				if (dataLabels.Count <= i)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(dataLabelPrefab) as GameObject;
					theGraph.changeSpriteParent(gameObject, dataLabelsParent);
					gameObject.transform.localScale = Vector3.one;
					dataLabels.Add(gameObject);
					gameObject.name = "Data_Label_" + dataLabels.Count;
				}
			}
		}
		int num = pointValuesCount;
		if (!dataLabelsEnabled)
		{
			num = 0;
		}
		else if (theGraph.IsStacked && theGraph.graphType != WMG_Axis_Graph.graphTypes.line_stacked)
		{
			num = 0;
			dataLabelsEnabled = false;
		}
		for (int num2 = dataLabels.Count - 1; num2 >= 0; num2--)
		{
			if (dataLabels[num2] != null && num2 >= num)
			{
				UnityEngine.Object.DestroyImmediate(dataLabels[num2]);
				dataLabels.RemoveAt(num2);
			}
		}
		if (!areaShadingUsesComputeShader)
		{
			StartCoroutine(SetDelayedAreaShadingChanged());
		}
	}

	private void createOrDeleteAreaShading(int pointValuesCount)
	{
		if (areaShadingUsesComputeShader)
		{
			if (areaShadingCSPrefab == null || areaShadingParent == null)
			{
				return;
			}
			if (areaShadingType != 0 && areaShadingRects.Count == 1 && areaShadingRects[0].name == "Area_Shading_CS")
			{
				UpdateSprites();
				return;
			}
			for (int num = areaShadingRects.Count - 1; num >= 0; num--)
			{
				if (areaShadingRects[num] != null && num >= 0)
				{
					UnityEngine.Object.DestroyImmediate(areaShadingRects[num]);
					areaShadingRects.RemoveAt(num);
				}
			}
			if (areaShadingType != 0 && areaShadingRects.Count != 1)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(areaShadingCSPrefab) as GameObject;
				theGraph.changeSpriteParent(gameObject, areaShadingParent);
				theGraph.changeSpriteSizeFloat(gameObject, theGraph.xAxisLength, theGraph.yAxisLength);
				theGraph.changeSpritePivot(gameObject, WMG_Text_Functions.WMGpivotTypes.BottomLeft);
				theGraph.changeSpritePositionTo(gameObject, new Vector3(0f, 0f, 0f));
				gameObject.transform.localScale = Vector3.one;
				areaShadingRects.Add(gameObject);
				gameObject.name = "Area_Shading_CS";
				WMG_Compute_Shader component = gameObject.GetComponent<WMG_Compute_Shader>();
				component.Init();
				UpdateSprites();
			}
		}
		else
		{
			if (areaShadingPrefab == null || areaShadingParent == null)
			{
				return;
			}
			if (areaShadingRects.Count == 1 && areaShadingRects[0].name == "Area_Shading_CS")
			{
				UnityEngine.Object.DestroyImmediate(areaShadingRects[0]);
				areaShadingRects.RemoveAt(0);
			}
			if (areaShadingType != 0)
			{
				for (int i = 0; i < pointValuesCount - 1; i++)
				{
					if (areaShadingRects.Count <= i)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(areaShadingPrefab) as GameObject;
						theGraph.changeSpriteParent(gameObject2, areaShadingParent);
						gameObject2.transform.localScale = Vector3.one;
						areaShadingRects.Add(gameObject2);
						gameObject2.name = "Area_Shading_" + areaShadingRects.Count;
						StartCoroutine(SetDelayedAreaShadingChanged());
					}
				}
			}
			int num2 = pointValuesCount - 1;
			if (areaShadingType == areaShadingTypes.None)
			{
				num2 = 0;
			}
			for (int num3 = areaShadingRects.Count - 1; num3 >= 0; num3--)
			{
				if (areaShadingRects[num3] != null && num3 >= num2)
				{
					UnityEngine.Object.DestroyImmediate(areaShadingRects[num3]);
					areaShadingRects.RemoveAt(num3);
					StartCoroutine(SetDelayedAreaShadingChanged());
				}
			}
			Material aMat = areaShadingMatSolid;
			if (areaShadingType == areaShadingTypes.Gradient)
			{
				aMat = areaShadingMatGradient;
			}
			for (int j = 0; j < areaShadingRects.Count; j++)
			{
				theGraph.setTextureMaterial(areaShadingRects[j], aMat);
				StartCoroutine(SetDelayedAreaShadingChanged());
			}
		}
	}

	private IEnumerator SetDelayedAreaShadingChanged()
	{
		yield return new WaitForEndOfFrame();
		AreaShadingChanged();
		yield return new WaitForEndOfFrame();
		AreaShadingChanged();
	}

	public void UpdateSprites()
	{
		List<GameObject> prevPoints = null;
		if (theGraph.IsStacked)
		{
			for (int i = 1; i < theGraph.lineSeries.Count; i++)
			{
				if (theGraph.activeInHierarchy(theGraph.lineSeries[i]))
				{
					WMG_Series component = theGraph.lineSeries[i].GetComponent<WMG_Series>();
					if (component == this && theGraph.activeInHierarchy(theGraph.lineSeries[i - 1]))
					{
						WMG_Series component2 = theGraph.lineSeries[i - 1].GetComponent<WMG_Series>();
						prevPoints = component2.getPoints();
					}
				}
			}
		}
		List<Vector2> newPositions = new List<Vector2>();
		List<int> newWidths = new List<int>();
		List<int> newHeights = new List<int>();
		bool callUpdateShading = true;
		getNewPointPositionsAndSizes(prevPoints, ref newPositions, ref newWidths, ref newHeights);
		updatePointSprites(newPositions, newWidths, newHeights, ref callUpdateShading);
		updateDataLabels();
		if (callUpdateShading)
		{
			updateAreaShading(newPositions);
		}
	}

	public void updateXdistBetween()
	{
		if (!ManuallySetXDistBetween)
		{
			_xDistBetweenPoints = theGraph.getDistBetween(points.Count, (theGraph.orientationType != WMG_Axis_Graph.orientationTypes.horizontal) ? theGraph.xAxisLength : theGraph.yAxisLength);
		}
	}

	public void updateExtraXSpace()
	{
		if (!ManuallySetExtraXSpace && theGraph.autoUpdateSeriesAxisSpacing)
		{
			if (theGraph.graphType == WMG_Axis_Graph.graphTypes.line || theGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked)
			{
				_extraXSpace = 0f;
			}
			else
			{
				_extraXSpace = xDistBetweenPoints / 2f;
			}
		}
	}

	private void getNewPointPositionsAndSizes(List<GameObject> prevPoints, ref List<Vector2> newPositions, ref List<int> newWidths, ref List<int> newHeights)
	{
		if (points.Count == 0)
		{
			return;
		}
		float val = theGraph.xAxisLength;
		float val2 = theGraph.yAxisLength;
		float val3 = theGraph.xAxis.AxisMaxValue;
		float val4 = yAxis.AxisMaxValue;
		float val5 = theGraph.xAxis.AxisMinValue;
		float val6 = yAxis.AxisMinValue;
		if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
		{
			theGraph.SwapVals(ref val, ref val2);
			theGraph.SwapVals(ref val3, ref val4);
			theGraph.SwapVals(ref val5, ref val6);
		}
		updateXdistBetween();
		updateExtraXSpace();
		for (int i = 0; i < points.Count && i < pointValues.Count; i++)
		{
			float num = 0f;
			float val7 = (pointValues[i].y - val6) / (val4 - val6) * val2;
			if (theGraph.useGroups || !UseXDistBetweenToSpace)
			{
				num = ((!theGraph.useGroups) ? ((pointValues[i].x - val5) / (val3 - val5) * val) : (extraXSpace + xDistBetweenPoints * (Mathf.Abs(pointValues[i].x) - 1f)));
			}
			else if (i > 0)
			{
				float num2 = newPositions[i - 1].x;
				float num3 = 0f;
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
				{
					num2 = newPositions[i - 1].y;
					num3 = theGraph.barWidth;
				}
				num = num2 + xDistBetweenPoints;
				if (!seriesIsLine)
				{
					num += num3;
				}
			}
			else
			{
				num = extraXSpace;
			}
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
			{
				theGraph.SwapVals(ref num, ref val7);
			}
			int num4 = 0;
			int num5 = 0;
			if (seriesIsLine)
			{
				num4 = Mathf.RoundToInt(pointWidthHeight);
				num5 = Mathf.RoundToInt(pointWidthHeight);
				if (theGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked)
				{
					if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
					{
						if (prevPoints != null && i < prevPoints.Count)
						{
							val7 += theGraph.getSpritePositionY(prevPoints[i]);
						}
					}
					else if (prevPoints != null && i < prevPoints.Count)
					{
						num += theGraph.getSpritePositionX(prevPoints[i]);
					}
				}
			}
			else
			{
				if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent && theGraph.TotalPointValues.Count > i)
				{
					if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
					{
						val7 = (pointValues[i].y - val6) / theGraph.TotalPointValues[i] * val2;
					}
					else
					{
						num = (pointValues[i].y - val6) / theGraph.TotalPointValues[i] * val2;
					}
				}
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
				{
					num4 = Mathf.RoundToInt(theGraph.barWidth);
					num5 = Mathf.RoundToInt(val7);
					int num6 = 0;
					if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_side || (theGraph.graphType == WMG_Axis_Graph.graphTypes.combo && comboType == comboTypes.bar))
					{
						num6 = Mathf.RoundToInt((theGraph.barAxisValue - val6) / (val4 - val6) * val2);
					}
					num5 -= num6;
					val7 -= (float)num5;
					barIsNegative[i] = false;
					if (num5 < 0)
					{
						num5 *= -1;
						val7 -= (float)num5;
						barIsNegative[i] = true;
					}
					if (prevPoints != null && i < prevPoints.Count)
					{
						val7 += theGraph.getSpritePositionY(prevPoints[i]) + theGraph.getSpriteHeight(prevPoints[i]);
					}
				}
				else
				{
					num4 = Mathf.RoundToInt(num);
					num5 = Mathf.RoundToInt(theGraph.barWidth);
					int num7 = 0;
					if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_side || (theGraph.graphType == WMG_Axis_Graph.graphTypes.combo && comboType == comboTypes.bar))
					{
						num7 = Mathf.RoundToInt((theGraph.barAxisValue - val6) / (val4 - val6) * val2);
					}
					num4 -= num7;
					num = num7;
					val7 -= theGraph.barWidth;
					barIsNegative[i] = false;
					if (num4 < 0)
					{
						num4 *= -1;
						num -= (float)num4;
						barIsNegative[i] = true;
					}
					if (prevPoints != null && i < prevPoints.Count)
					{
						num += theGraph.getSpritePositionX(prevPoints[i]) + theGraph.getSpriteWidth(prevPoints[i]);
					}
				}
			}
			newWidths.Add(num4);
			newHeights.Add(num5);
			newPositions.Add(new Vector2(num, val7));
		}
	}

	private void updatePointSprites(List<Vector2> newPositions, List<int> newWidths, List<int> newHeights, ref bool callUpdateShading)
	{
		if (points.Count == 0)
		{
			return;
		}
		if (animatingFromPreviousData)
		{
			if (seriesIsLine)
			{
				for (int i = 0; i < points.Count && i < pointValues.Count; i++)
				{
					newPositions[i] = theGraph.getChangeSpritePositionTo(points[i], newPositions[i]);
				}
			}
			afterPositions = new List<Vector2>(newPositions);
			afterWidths = new List<int>(newWidths);
			afterHeights = new List<int>(newHeights);
			OnSeriesDataChanged();
			animatingFromPreviousData = false;
			if (areaShadingUsesComputeShader)
			{
				callUpdateShading = false;
			}
			return;
		}
		for (int j = 0; j < points.Count && j < pointValues.Count; j++)
		{
			if (!seriesIsLine)
			{
				WMG_Node component = points[j].GetComponent<WMG_Node>();
				theGraph.changeBarWidthHeight(component.objectToColor, newWidths[j], newHeights[j]);
			}
			theGraph.changeSpritePositionTo(points[j], new Vector3(newPositions[j].x, newPositions[j].y, 0f));
		}
		RepositionLines();
	}

	private void updateDataLabels()
	{
		if (!dataLabelsEnabled)
		{
			return;
		}
		for (int i = 0; i < dataLabels.Count; i++)
		{
			Vector2 vector = new Vector2(theGraph.getSpritePositionX(points[i]), theGraph.getSpritePositionY(points[i]));
			theGraph.changeLabelFontSize(dataLabels[i], dataLabelsFontSize);
			theGraph.changeLabelColor(dataLabels[i], dataLabelsColor);
			theGraph.changeLabelFontStyle(dataLabels[i], dataLabelsFontStyle);
			if (dataLabelsFont != null)
			{
				theGraph.changeLabelFont(dataLabels[i], dataLabelsFont);
			}
			theGraph.changeLabelText(dataLabels[i], seriesDataLabeler(this, pointValues[i].y));
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
			{
				theGraph.changeSpritePivot(dataLabels[i], WMG_Text_Functions.WMGpivotTypes.Left);
			}
			else
			{
				theGraph.changeSpritePivot(dataLabels[i], WMG_Text_Functions.WMGpivotTypes.Bottom);
			}
			if (seriesIsLine)
			{
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
				{
					theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(dataLabelsOffset.x + vector.x, dataLabelsOffset.y + vector.y + pointWidthHeight / 2f, 0f));
				}
				else
				{
					theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(dataLabelsOffset.x + vector.x + pointWidthHeight / 2f, dataLabelsOffset.y + vector.y, 0f));
				}
			}
			else if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
			{
				float y = dataLabelsOffset.y + vector.y + theGraph.getSpriteHeight(points[i]);
				if (barIsNegative[i])
				{
					y = 0f - dataLabelsOffset.y - theGraph.getSpriteHeight(points[i]) + (float)Mathf.RoundToInt((theGraph.barAxisValue - yAxis.AxisMinValue) / (yAxis.AxisMaxValue - yAxis.AxisMinValue) * theGraph.yAxisLength);
				}
				theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(dataLabelsOffset.x + vector.x + theGraph.barWidth / 2f, y, 0f));
			}
			else
			{
				float x = dataLabelsOffset.x + vector.x + theGraph.getSpriteWidth(points[i]);
				if (barIsNegative[i])
				{
					x = 0f - dataLabelsOffset.x - theGraph.getSpriteWidth(points[i]) + (float)Mathf.RoundToInt((theGraph.barAxisValue - theGraph.xAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue) * theGraph.xAxisLength);
				}
				theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(x, dataLabelsOffset.y + vector.y + theGraph.barWidth / 2f, 0f));
			}
		}
	}

	public void updateAreaShading(List<Vector2> newPositions)
	{
		if (areaShadingType == areaShadingTypes.None)
		{
			return;
		}
		if (areaShadingUsesComputeShader && areaShadingRects.Count == 1)
		{
			WMG_Compute_Shader component = areaShadingRects[0].GetComponent<WMG_Compute_Shader>();
			component.computeShader.SetFloats("color", areaShadingColor.r, areaShadingColor.g, areaShadingColor.b, areaShadingColor.a);
			component.computeShader.SetInt("numPoints", pointValues.Count);
			component.computeShader.SetInt("isFill", (areaShadingType == areaShadingTypes.Solid) ? 1 : 0);
			component.computeShader.SetInt("isHorizontal", (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) ? 1 : 0);
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
			{
				component.computeShader.SetFloat("minVal", (areaShadingAxisValue - theGraph.xAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue));
			}
			else
			{
				component.computeShader.SetFloat("minVal", (areaShadingAxisValue - yAxis.AxisMinValue) / (yAxis.AxisMaxValue - yAxis.AxisMinValue));
			}
			float num = 0f;
			for (int i = 0; i < pointValues.Count; i++)
			{
				Vector2 vector = ((newPositions != null) ? newPositions[i] : theGraph.getSpritePositionXY(points[i]));
				vector = new Vector2(vector.x / theGraph.xAxisLength, vector.y / theGraph.yAxisLength);
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
				{
					vector = new Vector2(vector.y, vector.x);
				}
				component.pointVals[4 * i] = vector.x;
				component.pointVals[4 * i + 1] = vector.y;
				if (vector.y > num)
				{
					num = vector.y;
				}
			}
			component.computeShader.SetFloat("maxVal", num);
			component.computeShader.SetFloats("pointVals", component.pointVals);
			component.dispatchAndUpdateImage();
			return;
		}
		float num2 = float.NegativeInfinity;
		for (int j = 0; j < points.Count && j < pointValues.Count; j++)
		{
			if (pointValues[j].y > num2)
			{
				num2 = pointValues[j].y;
			}
		}
		for (int k = 0; k < points.Count - 1 && k < pointValues.Count; k++)
		{
			int num3 = 180;
			Vector2 vector2 = theGraph.getSpritePositionXY(points[k]);
			Vector2 vector3 = theGraph.getSpritePositionXY(points[k + 1]);
			float num4 = theGraph.yAxisLength / (yAxis.AxisMaxValue - yAxis.AxisMinValue);
			float num5 = (areaShadingAxisValue - yAxis.AxisMinValue) * num4;
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
			{
				num3 = 90;
				vector2 = new Vector2(theGraph.getSpritePositionY(points[k]), theGraph.getSpritePositionX(points[k]));
				vector3 = new Vector2(theGraph.getSpritePositionY(points[k + 1]), theGraph.getSpritePositionX(points[k + 1]));
				num4 = theGraph.xAxisLength / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue);
				num5 = (areaShadingAxisValue - theGraph.xAxis.AxisMinValue) * num4;
			}
			areaShadingRects[k].transform.localEulerAngles = new Vector3(0f, 0f, num3);
			float num6 = Mathf.Max(vector3.y, vector2.y);
			float num7 = Mathf.Min(vector3.y, vector2.y);
			int num8 = Mathf.RoundToInt(vector2.x);
			int num9 = Mathf.RoundToInt(vector3.x - vector2.x);
			float num10 = num6 - num7 + (Mathf.Min(pointValues[k + 1].y, pointValues[k].y) - areaShadingAxisValue) * num4;
			if (num7 < num5)
			{
				float num11 = (vector3.y - vector2.y) / (vector3.x - vector2.x);
				if (vector3.y > vector2.y)
				{
					float num12 = num5 - num7;
					int num13 = Mathf.RoundToInt(num12 / num11);
					num9 -= num13;
					num8 += num13;
				}
				else
				{
					float num14 = num5 - num7;
					int num15 = Mathf.RoundToInt(num14 / num11 * -1f);
					num9 -= num15;
				}
			}
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
			{
				theGraph.changeSpritePositionTo(areaShadingRects[k], new Vector3(num6, num8 + num9, 0f));
			}
			else
			{
				theGraph.changeSpritePositionTo(areaShadingRects[k], new Vector3(num8, num6, 0f));
			}
			theGraph.changeSpriteSizeFloat(areaShadingRects[k], num9, num10);
			if (k > 0)
			{
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
				{
					int num16 = Mathf.RoundToInt(theGraph.getSpritePositionY(areaShadingRects[k])) - Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[k]));
					int num17 = Mathf.RoundToInt(theGraph.getSpritePositionY(areaShadingRects[k - 1]));
					if (num16 > num17)
					{
						theGraph.changeSpriteWidth(areaShadingRects[k], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[k]) + 1f));
					}
					if (num16 < num17)
					{
						theGraph.changeSpriteWidth(areaShadingRects[k], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[k]) - 1f));
					}
				}
				else
				{
					int num18 = Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[k - 1])) + Mathf.RoundToInt(theGraph.getSpritePositionX(areaShadingRects[k - 1]));
					if (num18 > Mathf.RoundToInt(theGraph.getSpritePositionX(areaShadingRects[k])))
					{
						theGraph.changeSpriteWidth(areaShadingRects[k - 1], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[k - 1]) - 1f));
					}
					if (num18 < Mathf.RoundToInt(theGraph.getSpritePositionX(areaShadingRects[k])))
					{
						theGraph.changeSpriteWidth(areaShadingRects[k - 1], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[k - 1]) + 1f));
					}
				}
			}
			Material textureMaterial = theGraph.getTextureMaterial(areaShadingRects[k]);
			if (!(textureMaterial == null))
			{
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)
				{
					textureMaterial.SetFloat("_Slope", (0f - (vector3.y - vector2.y)) / num10);
				}
				else
				{
					textureMaterial.SetFloat("_Slope", (vector3.y - vector2.y) / num10);
				}
				textureMaterial.SetColor("_Color", areaShadingColor);
				textureMaterial.SetFloat("_Transparency", 1f - areaShadingColor.a);
				textureMaterial.SetFloat("_GradientScale", (Mathf.Max(pointValues[k + 1].y, pointValues[k].y) - areaShadingAxisValue) / (num2 - areaShadingAxisValue));
			}
		}
	}

	public void StartRealTimeUpdate()
	{
		if (!realTimeRunning && realTimeDataSource != null)
		{
			realTimeRunning = true;
			pointValues.SetListNoCb(new List<Vector2>(), ref _pointValues);
			pointValues.AddNoCb(new Vector2(0f, realTimeDataSource.getDatum<float>()), ref _pointValues);
			realTimeLoopVar = 0f;
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
			{
				realTimeOrigMax = theGraph.xAxis.AxisMaxValue;
			}
			else
			{
				realTimeOrigMax = yAxis.AxisMaxValue;
			}
		}
	}

	public void StopRealTimeUpdate()
	{
		realTimeRunning = false;
	}

	public void ResumeRealTimeUpdate()
	{
		realTimeRunning = true;
	}

	private void DoRealTimeUpdate()
	{
		float num = 0.0166f;
		realTimeLoopVar += num;
		float datum = realTimeDataSource.getDatum<float>();
		int num2 = 2;
		if (pointValues.Count >= 2)
		{
			float num3 = 0.3f;
			float num4 = (yAxis.AxisMaxValue - yAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue);
			float[] array = new float[num2];
			Vector2 vector = new Vector2(realTimeLoopVar, datum);
			for (int i = 0; i < array.Length; i++)
			{
				Vector2 vector2 = pointValues[pointValues.Count - (i + 1)];
				array[i] = (vector.y - vector2.y) / (vector.x - vector2.x) / num4;
			}
			if (Mathf.Abs(array[0] - array[1]) <= num3)
			{
				pointValues[pointValues.Count - 1] = new Vector2(realTimeLoopVar, datum);
			}
			else
			{
				pointValues.Add(new Vector2(realTimeLoopVar, datum));
			}
		}
		else
		{
			pointValues.Add(new Vector2(realTimeLoopVar, datum));
		}
		if (pointValues.Count > 1 && pointValues[pointValues.Count - 1].x > realTimeOrigMax)
		{
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
			{
				theGraph.xAxis.AxisMinValue = realTimeLoopVar - realTimeOrigMax;
				theGraph.xAxis.AxisMaxValue = realTimeLoopVar;
			}
			else
			{
				yAxis.AxisMinValue = realTimeLoopVar - realTimeOrigMax;
				yAxis.AxisMaxValue = realTimeLoopVar;
			}
			float x = pointValues[0].x;
			float x2 = pointValues[1].x;
			float y = pointValues[0].y;
			float y2 = pointValues[1].y;
			if (Mathf.Approximately(x + num, x2))
			{
				pointValues.RemoveAt(0);
			}
			else
			{
				pointValues[0] = new Vector2(x + num, y + (y2 - y) / (x2 - x) * num);
			}
		}
	}

	public void deleteAllNodesFromGraphManager()
	{
		for (int num = points.Count - 1; num >= 0; num--)
		{
			theGraph.DeleteNode(points[num].GetComponent<WMG_Node>());
		}
		theGraph.DeleteNode(legendEntry.nodeLeft.GetComponent<WMG_Node>());
		theGraph.DeleteNode(legendEntry.nodeRight.GetComponent<WMG_Node>());
		theGraph.DeleteNode(legendEntry.swatchNode.GetComponent<WMG_Node>());
	}
}
