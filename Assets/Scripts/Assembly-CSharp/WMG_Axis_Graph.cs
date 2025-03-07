using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WMG_Axis_Graph : WMG_Graph_Manager
{
	public enum graphTypes
	{
		line = 0,
		line_stacked = 1,
		bar_side = 2,
		bar_stacked = 3,
		bar_stacked_percent = 4,
		combo = 5
	}

	public enum orientationTypes
	{
		vertical = 0,
		horizontal = 1
	}

	public enum axesTypes
	{
		MANUAL = 0,
		CENTER = 1,
		AUTO_ORIGIN = 2,
		AUTO_ORIGIN_X = 3,
		AUTO_ORIGIN_Y = 4,
		I = 5,
		II = 6,
		III = 7,
		IV = 8,
		I_II = 9,
		III_IV = 10,
		II_III = 11,
		I_IV = 12,
		DUAL_Y = 13
	}

	[Flags]
	public enum ResizeProperties
	{
		SeriesPointSize = 1,
		SeriesLineWidth = 2,
		SeriesDataLabelSize = 4,
		SeriesDataLabelOffset = 8,
		LegendFontSize = 0x10,
		LegendEntrySize = 0x20,
		LegendOffset = 0x40,
		AxesWidth = 0x80,
		AxesLabelSize = 0x100,
		AxesLabelOffset = 0x200,
		AxesTitleSize = 0x400,
		AxesLinePadding = 0x800,
		AxesArrowSize = 0x1000,
		AutofitPadding = 0x2000,
		BorderPadding = 0x4000,
		TickSize = 0x8000
	}

	public delegate void GraphBackgroundChangedHandler(WMG_Axis_Graph aGraph);

	[SerializeField]
	public WMG_Axis yAxis;

	[SerializeField]
	public WMG_Axis xAxis;

	[SerializeField]
	public WMG_Axis yAxis2;

	[SerializeField]
	private List<string> _groups;

	public WMG_List<string> groups = new WMG_List<string>();

	public Vector2 tooltipOffset;

	public int tooltipNumberDecimals;

	public bool tooltipDisplaySeriesName;

	public bool tooltipAnimationsEnabled;

	public Ease tooltipAnimationsEasetype;

	public float tooltipAnimationsDuration;

	public Ease autoAnimationsEasetype;

	public float autoAnimationsDuration;

	public List<GameObject> lineSeries;

	public List<UnityEngine.Object> pointPrefabs;

	public List<UnityEngine.Object> linkPrefabs;

	public UnityEngine.Object barPrefab;

	public UnityEngine.Object seriesPrefab;

	public WMG_Legend legend;

	public GameObject graphTitle;

	public GameObject graphBackground;

	public GameObject anchoredParent;

	public GameObject seriesParent;

	public GameObject toolTipPanel;

	public GameObject toolTipLabel;

	[SerializeField]
	private graphTypes _graphType;

	[SerializeField]
	private orientationTypes _orientationType;

	[SerializeField]
	private axesTypes _axesType;

	[SerializeField]
	private bool _resizeEnabled;

	[SerializeField]
	[WMG_EnumFlag]
	private ResizeProperties _resizeProperties;

	[SerializeField]
	private bool _useGroups;

	[SerializeField]
	private Vector2 _paddingLeftRight;

	[SerializeField]
	private Vector2 _paddingTopBottom;

	[SerializeField]
	private Vector2 _theOrigin;

	[SerializeField]
	private float _barWidth;

	[SerializeField]
	private float _barAxisValue;

	[SerializeField]
	private bool _autoUpdateOrigin;

	[SerializeField]
	private bool _autoUpdateBarWidth;

	[SerializeField]
	private float _autoUpdateBarWidthSpacing;

	[SerializeField]
	private bool _autoUpdateSeriesAxisSpacing;

	[SerializeField]
	private bool _autoUpdateBarAxisValue;

	[SerializeField]
	private int _axisWidth;

	[SerializeField]
	private float _autoShrinkAtPercent;

	[SerializeField]
	private float _autoGrowAndShrinkByPercent;

	[SerializeField]
	private bool _tooltipEnabled;

	[SerializeField]
	private bool _autoAnimationsEnabled;

	[SerializeField]
	private bool _autoFitLabels;

	[SerializeField]
	private float _autoFitPadding;

	[SerializeField]
	private Vector2 _tickSize;

	[SerializeField]
	private string _graphTitleString;

	[SerializeField]
	private Vector2 _graphTitleOffset;

	private List<float> totalPointValues = new List<float>();

	private int maxSeriesPointCount;

	private int maxSeriesBarCount;

	private int numComboBarSeries;

	private float origWidth;

	private float origHeight;

	private float origBarWidth;

	private float origAxisWidth;

	private float origAutoFitPadding;

	private Vector2 origTickSize;

	private Vector2 origPaddingLeftRight;

	private Vector2 origPaddingTopBottom;

	private float cachedContainerWidth;

	private float cachedContainerHeight;

	public WMG_Graph_Tooltip theTooltip;

	private WMG_Graph_Auto_Anim autoAnim;

	private bool hasInit;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	public WMG_Change_Obj graphC = new WMG_Change_Obj();

	public WMG_Change_Obj resizeC = new WMG_Change_Obj();

	public WMG_Change_Obj seriesCountC = new WMG_Change_Obj();

	public WMG_Change_Obj seriesNoCountC = new WMG_Change_Obj();

	private WMG_Change_Obj tooltipEnabledC = new WMG_Change_Obj();

	private WMG_Change_Obj autoAnimEnabledC = new WMG_Change_Obj();

	private WMG_Change_Obj orientationC = new WMG_Change_Obj();

	private WMG_Change_Obj graphTypeC = new WMG_Change_Obj();

	public graphTypes graphType
	{
		get
		{
			return _graphType;
		}
		set
		{
			if (_graphType != value)
			{
				_graphType = value;
				graphTypeC.Changed();
				graphC.Changed();
				seriesCountC.Changed();
				legend.legendC.Changed();
			}
		}
	}

	public orientationTypes orientationType
	{
		get
		{
			return _orientationType;
		}
		set
		{
			if (_orientationType != value && (axesType != axesTypes.DUAL_Y || value != orientationTypes.horizontal))
			{
				_orientationType = value;
				orientationC.Changed();
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public axesTypes axesType
	{
		get
		{
			return _axesType;
		}
		set
		{
			if (_axesType != value && (orientationType != orientationTypes.horizontal || value != axesTypes.DUAL_Y) && (!(yAxis2 == null) || value != axesTypes.DUAL_Y))
			{
				_axesType = value;
				graphC.Changed();
				seriesNoCountC.Changed();
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

	public bool useGroups
	{
		get
		{
			return _useGroups;
		}
		set
		{
			if (_useGroups != value)
			{
				_useGroups = value;
				graphC.Changed();
			}
		}
	}

	public Vector2 paddingLeftRight
	{
		get
		{
			return _paddingLeftRight;
		}
		set
		{
			if (_paddingLeftRight != value)
			{
				_paddingLeftRight = value;
				graphC.Changed();
				seriesCountC.Changed();
				legend.legendC.Changed();
			}
		}
	}

	public Vector2 paddingTopBottom
	{
		get
		{
			return _paddingTopBottom;
		}
		set
		{
			if (_paddingTopBottom != value)
			{
				_paddingTopBottom = value;
				graphC.Changed();
				seriesCountC.Changed();
				legend.legendC.Changed();
			}
		}
	}

	public Vector2 theOrigin
	{
		get
		{
			return _theOrigin;
		}
		set
		{
			if (_theOrigin != value)
			{
				_theOrigin = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public float barWidth
	{
		get
		{
			return _barWidth;
		}
		set
		{
			if (_barWidth != value)
			{
				_barWidth = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public float barAxisValue
	{
		get
		{
			return _barAxisValue;
		}
		set
		{
			if (_barAxisValue != value)
			{
				_barAxisValue = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public bool autoUpdateOrigin
	{
		get
		{
			return _autoUpdateOrigin;
		}
		set
		{
			if (_autoUpdateOrigin != value)
			{
				_autoUpdateOrigin = value;
				graphC.Changed();
			}
		}
	}

	public bool autoUpdateBarWidth
	{
		get
		{
			return _autoUpdateBarWidth;
		}
		set
		{
			if (_autoUpdateBarWidth != value)
			{
				_autoUpdateBarWidth = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public float autoUpdateBarWidthSpacing
	{
		get
		{
			return _autoUpdateBarWidthSpacing;
		}
		set
		{
			if (_autoUpdateBarWidthSpacing != value)
			{
				_autoUpdateBarWidthSpacing = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public bool autoUpdateSeriesAxisSpacing
	{
		get
		{
			return _autoUpdateSeriesAxisSpacing;
		}
		set
		{
			if (_autoUpdateSeriesAxisSpacing != value)
			{
				_autoUpdateSeriesAxisSpacing = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public bool autoUpdateBarAxisValue
	{
		get
		{
			return _autoUpdateBarAxisValue;
		}
		set
		{
			if (_autoUpdateBarAxisValue != value)
			{
				_autoUpdateBarAxisValue = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}

	public int axisWidth
	{
		get
		{
			return _axisWidth;
		}
		set
		{
			if (_axisWidth != value)
			{
				_axisWidth = value;
				graphC.Changed();
			}
		}
	}

	public float autoShrinkAtPercent
	{
		get
		{
			return _autoShrinkAtPercent;
		}
		set
		{
			if (_autoShrinkAtPercent != value)
			{
				_autoShrinkAtPercent = value;
				graphC.Changed();
			}
		}
	}

	public float autoGrowAndShrinkByPercent
	{
		get
		{
			return _autoGrowAndShrinkByPercent;
		}
		set
		{
			if (_autoGrowAndShrinkByPercent != value)
			{
				_autoGrowAndShrinkByPercent = value;
				graphC.Changed();
			}
		}
	}

	public bool tooltipEnabled
	{
		get
		{
			return _tooltipEnabled;
		}
		set
		{
			if (_tooltipEnabled != value)
			{
				_tooltipEnabled = value;
				tooltipEnabledC.Changed();
			}
		}
	}

	public bool autoAnimationsEnabled
	{
		get
		{
			return _autoAnimationsEnabled;
		}
		set
		{
			if (_autoAnimationsEnabled != value)
			{
				_autoAnimationsEnabled = value;
				autoAnimEnabledC.Changed();
			}
		}
	}

	public bool autoFitLabels
	{
		get
		{
			return _autoFitLabels;
		}
		set
		{
			if (_autoFitLabels != value)
			{
				_autoFitLabels = value;
				graphC.Changed();
			}
		}
	}

	public float autoFitPadding
	{
		get
		{
			return _autoFitPadding;
		}
		set
		{
			if (_autoFitPadding != value)
			{
				_autoFitPadding = value;
				graphC.Changed();
			}
		}
	}

	public Vector2 tickSize
	{
		get
		{
			return _tickSize;
		}
		set
		{
			if (_tickSize != value)
			{
				_tickSize = value;
				graphC.Changed();
			}
		}
	}

	public string graphTitleString
	{
		get
		{
			return _graphTitleString;
		}
		set
		{
			if (_graphTitleString != value)
			{
				_graphTitleString = value;
				graphC.Changed();
			}
		}
	}

	public Vector2 graphTitleOffset
	{
		get
		{
			return _graphTitleOffset;
		}
		set
		{
			if (_graphTitleOffset != value)
			{
				_graphTitleOffset = value;
				graphC.Changed();
			}
		}
	}

	public float xAxisLength
	{
		get
		{
			return getSpriteWidth(base.gameObject) - paddingLeftRight.x - paddingLeftRight.y;
		}
	}

	public float yAxisLength
	{
		get
		{
			return getSpriteHeight(base.gameObject) - paddingTopBottom.x - paddingTopBottom.y;
		}
	}

	public bool IsStacked
	{
		get
		{
			return graphType == graphTypes.bar_stacked || graphType == graphTypes.bar_stacked_percent || graphType == graphTypes.line_stacked;
		}
	}

	public bool _autoFitting { get; set; }

	public List<float> TotalPointValues
	{
		get
		{
			return totalPointValues;
		}
	}

	public event GraphBackgroundChangedHandler GraphBackgroundChanged;

	public int NumComboBarSeries()
	{
		return numComboBarSeries;
	}

	protected virtual void OnGraphBackgroundChanged()
	{
		GraphBackgroundChangedHandler graphBackgroundChangedHandler = this.GraphBackgroundChanged;
		if (graphBackgroundChangedHandler != null)
		{
			graphBackgroundChangedHandler(this);
		}
	}

	private void Start()
	{
		Init();
		PauseCallbacks();
		AllChanged();
	}

	public void Init()
	{
		if (hasInit)
		{
			return;
		}
		hasInit = true;
		changeObjs.Add(orientationC);
		changeObjs.Add(graphTypeC);
		changeObjs.Add(graphC);
		changeObjs.Add(resizeC);
		changeObjs.Add(seriesCountC);
		changeObjs.Add(seriesNoCountC);
		changeObjs.Add(tooltipEnabledC);
		changeObjs.Add(autoAnimEnabledC);
		legend.Init();
		xAxis.Init(yAxis, yAxis2, false, false);
		yAxis.Init(xAxis, null, true, false);
		if (yAxis2 != null)
		{
			yAxis2.Init(xAxis, null, true, true);
		}
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				component.Init(i);
			}
		}
		theTooltip = base.gameObject.AddComponent<WMG_Graph_Tooltip>();
		theTooltip.hideFlags = HideFlags.HideInInspector;
		theTooltip.theGraph = this;
		if (tooltipEnabled)
		{
			theTooltip.subscribeToEvents(true);
		}
		autoAnim = base.gameObject.AddComponent<WMG_Graph_Auto_Anim>();
		autoAnim.hideFlags = HideFlags.HideInInspector;
		autoAnim.theGraph = this;
		if (autoAnimationsEnabled)
		{
			autoAnim.subscribeToEvents(true);
		}
		groups.SetList(_groups);
		groups.Changed += groupsChanged;
		graphTypeC.OnChange += GraphTypeChanged;
		tooltipEnabledC.OnChange += TooltipEnabledChanged;
		autoAnimEnabledC.OnChange += AutoAnimationsEnabledChanged;
		orientationC.OnChange += OrientationChanged;
		resizeC.OnChange += ResizeChanged;
		graphC.OnChange += GraphChanged;
		seriesCountC.OnChange += SeriesCountChanged;
		seriesNoCountC.OnChange += SeriesNoCountChanged;
		setOriginalPropertyValues();
		PauseCallbacks();
	}

	private void Update()
	{
		updateFromDataSource();
		updateFromResize();
		Refresh();
	}

	public void Refresh()
	{
		ResumeCallbacks();
		PauseCallbacks();
	}

	public void ManualResize()
	{
		PauseCallbacks();
		resizeEnabled = true;
		UpdateFromContainer();
		resizeEnabled = false;
		ResumeCallbacks();
	}

	private void PauseCallbacks()
	{
		yAxis.PauseCallbacks();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.PauseCallbacks();
		}
		xAxis.PauseCallbacks();
		for (int i = 0; i < changeObjs.Count; i++)
		{
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
		for (int j = 0; j < lineSeries.Count; j++)
		{
			if (activeInHierarchy(lineSeries[j]))
			{
				WMG_Series component = lineSeries[j].GetComponent<WMG_Series>();
				component.PauseCallbacks();
			}
		}
		legend.PauseCallbacks();
	}

	private void ResumeCallbacks()
	{
		yAxis.ResumeCallbacks();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.ResumeCallbacks();
		}
		xAxis.ResumeCallbacks();
		for (int i = 0; i < changeObjs.Count; i++)
		{
			changeObjs[i].changesPaused = false;
			if (changeObjs[i].changePaused)
			{
				changeObjs[i].Changed();
			}
		}
		for (int j = 0; j < lineSeries.Count; j++)
		{
			if (activeInHierarchy(lineSeries[j]))
			{
				WMG_Series component = lineSeries[j].GetComponent<WMG_Series>();
				component.ResumeCallbacks();
			}
		}
		legend.ResumeCallbacks();
	}

	private void updateFromResize()
	{
		bool flag = false;
		updateCacheAndFlag(ref cachedContainerWidth, getSpriteWidth(base.gameObject), ref flag);
		updateCacheAndFlag(ref cachedContainerHeight, getSpriteHeight(base.gameObject), ref flag);
		if (flag)
		{
			resizeC.Changed();
			graphC.Changed();
			seriesNoCountC.Changed();
			legend.legendC.Changed();
		}
	}

	private void updateFromDataSource()
	{
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				component.UpdateFromDataSource();
				component.RealTimeUpdate();
			}
		}
	}

	private void OrientationChanged()
	{
		UpdateOrientation();
	}

	private void TooltipEnabledChanged()
	{
		UpdateTooltip();
	}

	private void AutoAnimationsEnabledChanged()
	{
		UpdateAutoAnimEvents();
	}

	private void ResizeChanged()
	{
		UpdateFromContainer();
	}

	private void AllChanged()
	{
		graphC.Changed();
		seriesCountC.Changed();
		legend.legendC.Changed();
	}

	private void GraphTypeChanged()
	{
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				component.prefabC.Changed();
			}
		}
	}

	public void SeriesChanged(bool countChanged, bool instant)
	{
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (!activeInHierarchy(lineSeries[i]))
			{
				continue;
			}
			WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
			if (countChanged)
			{
				if (instant)
				{
					component.pointValuesCountChanged();
				}
				else
				{
					component.pointValuesCountC.Changed();
				}
			}
			else if (instant)
			{
				component.pointValuesChanged();
			}
			else
			{
				component.pointValuesC.Changed();
			}
		}
	}

	private void SeriesCountChanged()
	{
		SeriesChanged(true, false);
	}

	private void SeriesNoCountChanged()
	{
		SeriesChanged(false, false);
	}

	public void aSeriesPointsChanged()
	{
		if (Application.isPlaying)
		{
			UpdateTotals();
			UpdateBarWidth();
			UpdateAxesMinMaxValues();
		}
	}

	public void GraphChanged()
	{
		UpdateTotals();
		UpdateBarWidth();
		UpdateAxesMinMaxValues();
		UpdateAxesType();
		UpdateAxesGridsAndTicks();
		UpdateAxesLabels();
		UpdateSeriesParentPositions();
		UpdateBG();
		UpdateTitles();
	}

	private void groupsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref groups, ref _groups, oneValChanged, index);
		graphC.Changed();
		if (oneValChanged)
		{
			seriesNoCountC.Changed();
		}
		else
		{
			seriesCountC.Changed();
		}
	}

	public void setOriginalPropertyValues()
	{
		cachedContainerWidth = getSpriteWidth(base.gameObject);
		cachedContainerHeight = getSpriteHeight(base.gameObject);
		origWidth = getSpriteWidth(base.gameObject);
		origHeight = getSpriteHeight(base.gameObject);
		origBarWidth = barWidth;
		origAxisWidth = axisWidth;
		origAutoFitPadding = autoFitPadding;
		origTickSize = tickSize;
		origPaddingLeftRight = paddingLeftRight;
		origPaddingTopBottom = paddingTopBottom;
	}

	private void UpdateOrientation()
	{
		yAxis.ChangeOrientation();
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (!activeInHierarchy(lineSeries[i]))
			{
				continue;
			}
			WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
			component.origDataLabelOffset = new Vector2(component.origDataLabelOffset.y, component.origDataLabelOffset.x);
			component.dataLabelsOffset = new Vector2(component.dataLabelsOffset.y, component.dataLabelsOffset.x);
			component.setAnimatingFromPreviousData();
			if (!component.dataLabelsEnabled)
			{
				continue;
			}
			if (orientationType == orientationTypes.horizontal)
			{
				foreach (GameObject dataLabel in component.getDataLabels())
				{
					changeSpritePivot(dataLabel, WMGpivotTypes.Left);
				}
				continue;
			}
			foreach (GameObject dataLabel2 in component.getDataLabels())
			{
				changeSpritePivot(dataLabel2, WMGpivotTypes.Bottom);
			}
		}
	}

	private void UpdateAxesType()
	{
		if (yAxis2 != null)
		{
			yAxis2.AxisTitle.SetActive(axesType == axesTypes.DUAL_Y);
			yAxis2.AxisTicks.SetActive(axesType == axesTypes.DUAL_Y);
			yAxis2.AxisLine.SetActive(axesType == axesTypes.DUAL_Y);
			yAxis2.AxisLabelObjs.SetActive(axesType == axesTypes.DUAL_Y);
		}
		if (axesType == axesTypes.MANUAL)
		{
			return;
		}
		if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_X || axesType == axesTypes.AUTO_ORIGIN_Y)
		{
			updateAxesRelativeToOrigin();
			return;
		}
		updateOriginRelativeToAxes();
		if (axesType == axesTypes.I || axesType == axesTypes.II || axesType == axesTypes.III || axesType == axesTypes.IV || axesType == axesTypes.DUAL_Y)
		{
			if (axesType == axesTypes.I)
			{
				setAxesQuadrant1();
			}
			else if (axesType == axesTypes.II)
			{
				setAxesQuadrant2();
			}
			else if (axesType == axesTypes.III)
			{
				setAxesQuadrant3();
			}
			else if (axesType == axesTypes.IV)
			{
				setAxesQuadrant4();
			}
			else if (axesType == axesTypes.DUAL_Y)
			{
				setAxesDualYaxis();
			}
			return;
		}
		if (axesType == axesTypes.CENTER)
		{
			setAxesQuadrant1_2_3_4();
		}
		else if (axesType == axesTypes.I_II)
		{
			setAxesQuadrant1_2();
		}
		else if (axesType == axesTypes.III_IV)
		{
			setAxesQuadrant3_4();
		}
		else if (axesType == axesTypes.II_III)
		{
			setAxesQuadrant2_3();
		}
		else if (axesType == axesTypes.I_IV)
		{
			setAxesQuadrant1_4();
		}
		yAxis.possiblyHideTickBasedOnPercent();
		xAxis.possiblyHideTickBasedOnPercent();
	}

	private void updateOriginRelativeToAxes()
	{
		if (autoUpdateOrigin)
		{
			if (axesType == axesTypes.I || axesType == axesTypes.DUAL_Y)
			{
				_theOrigin = new Vector2(xAxis.AxisMinValue, yAxis.AxisMinValue);
			}
			else if (axesType == axesTypes.II)
			{
				_theOrigin = new Vector2(xAxis.AxisMaxValue, yAxis.AxisMinValue);
			}
			else if (axesType == axesTypes.III)
			{
				_theOrigin = new Vector2(xAxis.AxisMaxValue, yAxis.AxisMaxValue);
			}
			else if (axesType == axesTypes.IV)
			{
				_theOrigin = new Vector2(xAxis.AxisMinValue, yAxis.AxisMaxValue);
			}
			else if (axesType == axesTypes.CENTER)
			{
				_theOrigin = new Vector2((xAxis.AxisMaxValue + xAxis.AxisMinValue) / 2f, (yAxis.AxisMaxValue + yAxis.AxisMinValue) / 2f);
			}
			else if (axesType == axesTypes.I_II)
			{
				_theOrigin = new Vector2((xAxis.AxisMaxValue + xAxis.AxisMinValue) / 2f, yAxis.AxisMinValue);
			}
			else if (axesType == axesTypes.III_IV)
			{
				_theOrigin = new Vector2((xAxis.AxisMaxValue + xAxis.AxisMinValue) / 2f, yAxis.AxisMaxValue);
			}
			else if (axesType == axesTypes.II_III)
			{
				_theOrigin = new Vector2(xAxis.AxisMaxValue, (yAxis.AxisMaxValue + yAxis.AxisMinValue) / 2f);
			}
			else if (axesType == axesTypes.I_IV)
			{
				_theOrigin = new Vector2(xAxis.AxisMinValue, (yAxis.AxisMaxValue + yAxis.AxisMinValue) / 2f);
			}
		}
		if (autoUpdateBarAxisValue)
		{
			if (orientationType == orientationTypes.vertical)
			{
				_barAxisValue = theOrigin.y;
			}
			else
			{
				_barAxisValue = theOrigin.x;
			}
		}
	}

	private void updateAxesRelativeToOrigin()
	{
		yAxis.updateAxesRelativeToOrigin(theOrigin.x);
		xAxis.updateAxesRelativeToOrigin(theOrigin.y);
		if (autoUpdateBarAxisValue)
		{
			if (orientationType == orientationTypes.vertical)
			{
				_barAxisValue = theOrigin.y;
			}
			else
			{
				_barAxisValue = theOrigin.x;
			}
		}
	}

	private void UpdateAxesMinMaxValues()
	{
		yAxis.UpdateAxesMinMaxValues();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.UpdateAxesMinMaxValues();
		}
		xAxis.UpdateAxesMinMaxValues();
	}

	private void UpdateAxesGridsAndTicks()
	{
		yAxis.UpdateAxesGridsAndTicks();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.UpdateAxesGridsAndTicks();
		}
		xAxis.UpdateAxesGridsAndTicks();
	}

	private void UpdateAxesLabels()
	{
		yAxis.UpdateAxesLabels();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.UpdateAxesLabels();
		}
		xAxis.UpdateAxesLabels();
		yAxis.AutofitAxesLabels();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.AutofitAxesLabels();
		}
		xAxis.AutofitAxesLabels();
	}

	private void UpdateSeriesParentPositions()
	{
		int num = -1;
		bool flag = false;
		int num2 = 0;
		if (graphType == graphTypes.combo)
		{
			for (int i = 0; i < lineSeries.Count; i++)
			{
				if (activeInHierarchy(lineSeries[i]))
				{
					WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
					if (component.comboType == WMG_Series.comboTypes.bar)
					{
						flag = true;
						num2++;
					}
				}
			}
		}
		for (int j = 0; j < lineSeries.Count; j++)
		{
			if (!activeInHierarchy(lineSeries[j]))
			{
				continue;
			}
			WMG_Series component2 = lineSeries[j].GetComponent<WMG_Series>();
			Vector2 axesOffsetFactor = getAxesOffsetFactor();
			axesOffsetFactor = new Vector2((float)(-axisWidth / 2) * axesOffsetFactor.x, (float)(-axisWidth / 2) * axesOffsetFactor.y);
			if (component2.seriesIsLine)
			{
				if (component2.ManuallySetExtraXSpace)
				{
					changeSpritePositionTo(lineSeries[j], new Vector3(component2.extraXSpace, 0f, 0f));
					continue;
				}
				changeSpritePositionTo(lineSeries[j], new Vector3(0f, 0f, 0f));
			}
			else if (orientationType == orientationTypes.vertical)
			{
				if (component2.ManuallySetExtraXSpace)
				{
					changeSpritePositionTo(lineSeries[j], new Vector3(component2.extraXSpace, axesOffsetFactor.y, 0f));
					continue;
				}
				changeSpritePositionTo(lineSeries[j], new Vector3(axesOffsetFactor.x, axesOffsetFactor.y, 0f));
			}
			else
			{
				if (component2.ManuallySetExtraXSpace)
				{
					changeSpritePositionTo(lineSeries[j], new Vector3(component2.extraXSpace, axesOffsetFactor.y + barWidth, 0f));
					continue;
				}
				changeSpritePositionTo(lineSeries[j], new Vector3(axesOffsetFactor.x, axesOffsetFactor.y + barWidth, 0f));
			}
			if (graphType == graphTypes.bar_side)
			{
				if (j > 0)
				{
					if (orientationType == orientationTypes.vertical)
					{
						changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[j - 1], barWidth);
					}
					else
					{
						changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[j - 1], barWidth);
					}
				}
			}
			else if (graphType == graphTypes.combo)
			{
				if (j <= 0)
				{
					continue;
				}
				if (lineSeries[j - 1].GetComponent<WMG_Series>().comboType == WMG_Series.comboTypes.bar)
				{
					num = j - 1;
				}
				if (num > -1 && lineSeries[j].GetComponent<WMG_Series>().comboType == WMG_Series.comboTypes.bar)
				{
					if (orientationType == orientationTypes.vertical)
					{
						changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[num], barWidth);
					}
					else
					{
						changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[num], barWidth);
					}
				}
				if (flag && lineSeries[j].GetComponent<WMG_Series>().comboType == WMG_Series.comboTypes.line)
				{
					changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[0], barWidth / 2f * (float)num2);
				}
			}
			else if (j > 0)
			{
				if (orientationType == orientationTypes.vertical)
				{
					changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[0], 0f);
				}
				else
				{
					changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[0], 0f);
				}
			}
		}
	}

	public void UpdateBG()
	{
		changeSpriteSize(graphBackground, Mathf.RoundToInt(getSpriteWidth(base.gameObject)), Mathf.RoundToInt(getSpriteHeight(base.gameObject)));
		changeSpritePositionTo(graphBackground, new Vector3(0f - paddingLeftRight.x, 0f - paddingTopBottom.y, 0f));
		changeSpriteSize(anchoredParent, Mathf.RoundToInt(getSpriteWidth(base.gameObject)), Mathf.RoundToInt(getSpriteHeight(base.gameObject)));
		changeSpritePositionTo(anchoredParent, new Vector3(0f - paddingLeftRight.x, 0f - paddingTopBottom.y, 0f));
		UpdateBGandSeriesParentPositions(cachedContainerWidth, cachedContainerHeight);
		OnGraphBackgroundChanged();
	}

	public void UpdateBGandSeriesParentPositions(float x, float y)
	{
		Vector2 spritePivot = getSpritePivot(base.gameObject);
		Vector3 newPos = new Vector3((0f - x) * spritePivot.x + paddingLeftRight.x, (0f - y) * spritePivot.y + paddingTopBottom.y);
		changeSpritePositionTo(graphBackground.transform.parent.gameObject, newPos);
		changeSpritePositionTo(seriesParent, newPos);
	}

	private void UpdateTotals()
	{
		int num = 0;
		int num2 = 0;
		numComboBarSeries = 0;
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (!activeInHierarchy(lineSeries[i]))
			{
				continue;
			}
			WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
			if (num < component.pointValues.Count)
			{
				num = component.pointValues.Count;
			}
			if (graphType == graphTypes.combo && component.comboType == WMG_Series.comboTypes.bar)
			{
				numComboBarSeries++;
				if (num2 < component.pointValues.Count)
				{
					num2 = component.pointValues.Count;
				}
			}
		}
		maxSeriesPointCount = num;
		maxSeriesBarCount = num2;
		for (int j = 0; j < num; j++)
		{
			if (totalPointValues.Count <= j)
			{
				totalPointValues.Add(0f);
			}
			totalPointValues[j] = 0f;
			for (int k = 0; k < lineSeries.Count; k++)
			{
				if (!activeInHierarchy(lineSeries[k]))
				{
					continue;
				}
				WMG_Series component2 = lineSeries[k].GetComponent<WMG_Series>();
				if (component2.pointValues.Count > j)
				{
					if (orientationType == orientationTypes.vertical)
					{
						totalPointValues[j] += component2.pointValues[j].y - component2.yAxis.AxisMinValue;
					}
					else
					{
						totalPointValues[j] += component2.pointValues[j].y - xAxis.AxisMinValue;
					}
				}
			}
		}
	}

	private void UpdateBarWidth()
	{
		if (autoUpdateBarWidth)
		{
			if (graphType == graphTypes.line || graphType == graphTypes.line_stacked)
			{
				return;
			}
			float num = xAxisLength;
			if (orientationType == orientationTypes.horizontal)
			{
				num = yAxisLength;
			}
			int num2 = maxSeriesPointCount * lineSeries.Count + 1;
			if (graphType == graphTypes.combo)
			{
				num2 = maxSeriesBarCount * numComboBarSeries + 1;
			}
			if (graphType == graphTypes.bar_stacked || graphType == graphTypes.bar_stacked_percent)
			{
				num2 = maxSeriesPointCount;
			}
			_autoUpdateBarWidthSpacing = Mathf.Clamp01(autoUpdateBarWidthSpacing);
			barWidth = (1f - autoUpdateBarWidthSpacing) * (num - (float)maxSeriesPointCount) / (float)num2;
		}
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				component.updateXdistBetween();
				component.updateExtraXSpace();
			}
		}
		UpdateSeriesParentPositions();
	}

	private void UpdateTitles()
	{
		if (graphTitle != null)
		{
			changeLabelText(graphTitle, graphTitleString);
			changeSpritePositionTo(graphTitle, new Vector3(xAxisLength / 2f + graphTitleOffset.x, yAxisLength + graphTitleOffset.y));
		}
		yAxis.UpdateTitle();
		if (axesType == axesTypes.DUAL_Y)
		{
			yAxis2.UpdateTitle();
		}
		xAxis.UpdateTitle();
	}

	private void UpdateTooltip()
	{
		theTooltip.subscribeToEvents(tooltipEnabled);
	}

	private void UpdateAutoAnimEvents()
	{
		autoAnim.subscribeToEvents(autoAnimationsEnabled);
	}

	public float getDistBetween(int pointsCount, float theAxisLength)
	{
		float num = 0f;
		if (pointsCount - 1 <= 0)
		{
			num = theAxisLength;
			if (graphType == graphTypes.bar_side)
			{
				num -= (float)lineSeries.Count * barWidth;
			}
			else if (graphType == graphTypes.combo)
			{
				num -= (float)numComboBarSeries * barWidth;
			}
			else if (graphType == graphTypes.bar_stacked)
			{
				num -= barWidth;
			}
			else if (graphType == graphTypes.bar_stacked_percent)
			{
				num -= barWidth;
			}
		}
		else
		{
			int num2 = pointsCount - 1;
			if (graphType != 0 && graphType != graphTypes.line_stacked)
			{
				num2++;
			}
			num = theAxisLength / (float)num2;
			if (graphType == graphTypes.bar_side)
			{
				num -= (float)lineSeries.Count * barWidth / (float)num2;
			}
			else if (graphType == graphTypes.combo)
			{
				num -= (float)numComboBarSeries * barWidth / (float)num2;
			}
			else if (graphType == graphTypes.bar_stacked)
			{
				num -= barWidth / (float)num2;
			}
			else if (graphType == graphTypes.bar_stacked_percent)
			{
				num -= barWidth / (float)num2;
			}
		}
		return num;
	}

	[Obsolete("Use xAxis.GetAxisTickNodes")]
	public List<WMG_Node> getXAxisTicks()
	{
		return xAxis.GetAxisTickNodes();
	}

	[Obsolete("Use xAxis.GetAxisLabelNodes")]
	public List<WMG_Node> getXAxisLabels()
	{
		return xAxis.GetAxisLabelNodes();
	}

	[Obsolete("Use yAxis.GetAxisTickNodes")]
	public List<WMG_Node> getYAxisTicks()
	{
		return yAxis.GetAxisTickNodes();
	}

	[Obsolete("Use yAxis.GetAxisLabelNodes")]
	public List<WMG_Node> getYAxisLabels()
	{
		return yAxis.GetAxisLabelNodes();
	}

	public void changeAllLinePivots(WMGpivotTypes newPivot)
	{
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				List<GameObject> lines = component.getLines();
				for (int j = 0; j < lines.Count; j++)
				{
					changeSpritePivot(lines[j], newPivot);
					WMG_Link component2 = lines[j].GetComponent<WMG_Link>();
					component2.Reposition();
				}
			}
		}
	}

	public List<Vector3> getSeriesScaleVectors(bool useLineWidthForX, float x, float y)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				if (useLineWidthForX)
				{
					list.Add(new Vector3(component.lineScale, y, 1f));
				}
				else
				{
					list.Add(new Vector3(x, y, 1f));
				}
			}
		}
		return list;
	}

	public float getMaxPointSize()
	{
		if (graphType == graphTypes.line || graphType == graphTypes.line_stacked || (graphType == graphTypes.combo && numComboBarSeries == 0))
		{
			float num = 0f;
			for (int i = 0; i < lineSeries.Count; i++)
			{
				if (activeInHierarchy(lineSeries[i]))
				{
					WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
					if (component.pointWidthHeight > num)
					{
						num = component.pointWidthHeight;
					}
				}
			}
			return num;
		}
		float pointWidthHeight = barWidth;
		if (graphType == graphTypes.combo)
		{
			for (int j = 0; j < lineSeries.Count; j++)
			{
				if (activeInHierarchy(lineSeries[j]))
				{
					WMG_Series component2 = lineSeries[j].GetComponent<WMG_Series>();
					if (component2.comboType == WMG_Series.comboTypes.line && component2.pointWidthHeight > pointWidthHeight)
					{
						pointWidthHeight = component2.pointWidthHeight;
					}
				}
			}
		}
		return pointWidthHeight;
	}

	public int getMaxNumPoints()
	{
		return maxSeriesPointCount;
	}

	public void setAxesDualYaxis()
	{
		xAxis.setDualYAxes();
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
		yAxis2.anchorVec = new Vector2(1f, 0.5f);
	}

	public void setAxesQuadrant1()
	{
		xAxis.setAxisTopRight(false);
		yAxis.setAxisTopRight(false);
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
	}

	public void setAxesQuadrant2()
	{
		xAxis.setAxisBotLeft(false);
		yAxis.setAxisTopRight(true);
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(1f, 0.5f);
	}

	public void setAxesQuadrant3()
	{
		xAxis.setAxisBotLeft(true);
		yAxis.setAxisBotLeft(true);
		xAxis.anchorVec = new Vector2(0.5f, 1f);
		yAxis.anchorVec = new Vector2(1f, 0.5f);
	}

	public void setAxesQuadrant4()
	{
		xAxis.setAxisTopRight(true);
		yAxis.setAxisBotLeft(false);
		xAxis.anchorVec = new Vector2(0.5f, 1f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
	}

	public void setAxesQuadrant1_2_3_4()
	{
		xAxis.setAxisMiddle(false);
		yAxis.setAxisMiddle(false);
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
	}

	public void setAxesQuadrant1_2()
	{
		xAxis.setAxisMiddle(false);
		yAxis.setAxisTopRight(false);
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
	}

	public void setAxesQuadrant3_4()
	{
		xAxis.setAxisMiddle(true);
		yAxis.setAxisBotLeft(false);
		xAxis.anchorVec = new Vector2(0.5f, 1f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
	}

	public void setAxesQuadrant2_3()
	{
		xAxis.setAxisBotLeft(false);
		yAxis.setAxisMiddle(true);
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(1f, 0.5f);
	}

	public void setAxesQuadrant1_4()
	{
		xAxis.setAxisTopRight(false);
		yAxis.setAxisMiddle(false);
		xAxis.anchorVec = new Vector2(0.5f, 0f);
		yAxis.anchorVec = new Vector2(0f, 0.5f);
	}

	private Vector2 getAxesOffsetFactor()
	{
		if (axesType == axesTypes.I || axesType == axesTypes.DUAL_Y)
		{
			return new Vector2(-1f, -1f);
		}
		if (axesType == axesTypes.II)
		{
			return new Vector2(1f, -1f);
		}
		if (axesType == axesTypes.III)
		{
			return new Vector2(1f, 1f);
		}
		if (axesType == axesTypes.IV)
		{
			return new Vector2(-1f, 1f);
		}
		if (axesType == axesTypes.CENTER)
		{
			return new Vector2(0f, 0f);
		}
		if (axesType == axesTypes.I_II)
		{
			return new Vector2(0f, -1f);
		}
		if (axesType == axesTypes.III_IV)
		{
			return new Vector2(0f, 1f);
		}
		if (axesType == axesTypes.II_III)
		{
			return new Vector2(1f, 0f);
		}
		if (axesType == axesTypes.I_IV)
		{
			return new Vector2(-1f, 0f);
		}
		if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_X || axesType == axesTypes.AUTO_ORIGIN_Y)
		{
			float x = 0f;
			float y = 0f;
			if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_Y)
			{
				if (xAxis.AxisMinValue >= theOrigin.x)
				{
					y = -1f;
				}
				else if (xAxis.AxisMaxValue <= theOrigin.x)
				{
					y = 1f;
				}
			}
			if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_X)
			{
				if (yAxis.AxisMinValue >= theOrigin.y)
				{
					x = -1f;
				}
				else if (yAxis.AxisMaxValue <= theOrigin.y)
				{
					x = 1f;
				}
			}
			return new Vector2(x, y);
		}
		return new Vector2(0f, 0f);
	}

	public void animScaleAllAtOnce(bool isPoint, float duration, float delay, Ease anEaseType, List<Vector3> before, List<Vector3> after)
	{
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				List<GameObject> list = ((!isPoint) ? component.getLines() : component.getPoints());
				for (int j = 0; j < list.Count; j++)
				{
					list[j].transform.localScale = before[i];
					WMG_Anim.animScale(list[j], duration, anEaseType, after[i], delay);
				}
			}
		}
	}

	public void animScaleBySeries(bool isPoint, float duration, float delay, Ease anEaseType, List<Vector3> before, List<Vector3> after)
	{
		Sequence seq = DOTween.Sequence();
		float num = duration / (float)lineSeries.Count;
		float num2 = delay / (float)lineSeries.Count;
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				List<GameObject> list = ((!isPoint) ? component.getLines() : component.getPoints());
				float insTime = (float)i * num + (float)(i + 1) * num2;
				for (int j = 0; j < list.Count; j++)
				{
					list[j].transform.localScale = before[i];
					WMG_Anim.animScaleSeqInsert(ref seq, insTime, list[j], num, anEaseType, after[i], num2);
				}
			}
		}
		seq.Play();
	}

	public void animScaleOneByOne(bool isPoint, float duration, float delay, Ease anEaseType, List<Vector3> before, List<Vector3> after, int loopDir)
	{
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (!activeInHierarchy(lineSeries[i]))
			{
				continue;
			}
			Sequence seq = DOTween.Sequence();
			WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
			List<GameObject> list = ((!isPoint) ? component.getLines() : component.getPoints());
			float duration2 = duration / (float)list.Count;
			float delay2 = delay / (float)list.Count;
			switch (loopDir)
			{
			case 0:
			{
				for (int j = 0; j < list.Count; j++)
				{
					list[j].transform.localScale = before[i];
					WMG_Anim.animScaleSeqAppend(ref seq, list[j], duration2, anEaseType, after[i], delay2);
				}
				break;
			}
			case 1:
			{
				for (int num5 = list.Count - 1; num5 >= 0; num5--)
				{
					list[num5].transform.localScale = before[i];
					WMG_Anim.animScaleSeqAppend(ref seq, list[num5], duration2, anEaseType, after[i], delay2);
				}
				break;
			}
			case 2:
			{
				int num = list.Count - 1;
				int num2 = num / 2;
				int num3 = -1;
				int num4 = 0;
				bool flag = false;
				bool flag2 = false;
				while (!flag || !flag2)
				{
					if (num2 >= 0 && num2 <= num)
					{
						list[num2].transform.localScale = before[i];
						WMG_Anim.animScaleSeqAppend(ref seq, list[num2], duration2, anEaseType, after[i], delay2);
					}
					num4++;
					num3 *= -1;
					num2 += num3 * num4;
					if (num2 < 0)
					{
						flag = true;
					}
					if (num2 > num)
					{
						flag2 = true;
					}
				}
				break;
			}
			}
			seq.Play();
		}
	}

	public WMG_Series addSeries()
	{
		return addSeriesAt(lineSeries.Count);
	}

	public void deleteSeries()
	{
		if (lineSeries.Count > 0)
		{
			deleteSeriesAt(lineSeries.Count - 1);
		}
	}

	public WMG_Series addSeriesAt(int index, WMG_Series.comboTypes comboType = WMG_Series.comboTypes.line)
	{
		if (Application.isPlaying)
		{
			Init();
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(seriesPrefab) as GameObject;
		gameObject.name = "Series" + (index + 1);
		changeSpriteParent(gameObject, seriesParent);
		gameObject.transform.localScale = Vector3.one;
		WMG_Series component = gameObject.GetComponent<WMG_Series>();
		if (autoAnimationsEnabled)
		{
			autoAnim.addSeriesForAutoAnim(component);
		}
		component.theGraph = this;
		lineSeries.Insert(index, gameObject);
		component.comboType = comboType;
		component.Init(index);
		return gameObject.GetComponent<WMG_Series>();
	}

	public void deleteSeriesAt(int index)
	{
		if (Application.isPlaying)
		{
			Init();
		}
		GameObject gameObject = lineSeries[index];
		WMG_Series component = gameObject.GetComponent<WMG_Series>();
		lineSeries.Remove(gameObject);
		if (Application.isPlaying)
		{
			component.deleteAllNodesFromGraphManager();
			legend.deleteLegendEntry(index);
		}
		UnityEngine.Object.DestroyImmediate(gameObject);
		graphC.Changed();
		if (graphType != 0 && graphType != graphTypes.line_stacked)
		{
			seriesNoCountC.Changed();
		}
		legend.legendC.Changed();
	}

	private void UpdateFromContainer()
	{
		if (!resizeEnabled)
		{
			return;
		}
		bool flag = true;
		Vector2 vector = new Vector2(cachedContainerWidth / origWidth, cachedContainerHeight / origHeight);
		Vector2 vector2 = vector;
		if (orientationType == orientationTypes.horizontal)
		{
			vector2 = new Vector2(vector.y, vector.x);
		}
		float num = vector.x;
		if (vector.y < num)
		{
			num = vector.y;
		}
		if ((resizeProperties & ResizeProperties.BorderPadding) == ResizeProperties.BorderPadding)
		{
			if (autoFitLabels)
			{
				if (xAxis.AxisTicksRightAbove)
				{
					paddingLeftRight = new Vector2(getNewResizeVariable(num, origPaddingLeftRight.x), paddingLeftRight.y);
				}
				else
				{
					paddingLeftRight = new Vector2(paddingLeftRight.x, getNewResizeVariable(num, origPaddingLeftRight.y));
				}
				if (yAxis.AxisTicksRightAbove)
				{
					paddingTopBottom = new Vector2(paddingTopBottom.x, getNewResizeVariable(num, origPaddingTopBottom.y));
				}
				else
				{
					paddingTopBottom = new Vector2(getNewResizeVariable(num, origPaddingTopBottom.x), paddingTopBottom.y);
				}
			}
			else
			{
				paddingLeftRight = new Vector2(getNewResizeVariable(num, origPaddingLeftRight.x), getNewResizeVariable(num, origPaddingLeftRight.y));
				paddingTopBottom = new Vector2(getNewResizeVariable(num, origPaddingTopBottom.x), getNewResizeVariable(num, origPaddingTopBottom.y));
			}
		}
		if ((resizeProperties & ResizeProperties.AutofitPadding) == ResizeProperties.AutofitPadding)
		{
			autoFitPadding = getNewResizeVariable(num, origAutoFitPadding);
		}
		if ((resizeProperties & ResizeProperties.TickSize) == ResizeProperties.TickSize)
		{
			tickSize = new Vector2(getNewResizeVariable(num, origTickSize.x), getNewResizeVariable(num, origTickSize.y));
		}
		if ((resizeProperties & ResizeProperties.AxesWidth) == ResizeProperties.AxesWidth)
		{
			axisWidth = Mathf.RoundToInt(getNewResizeVariable(num, origAxisWidth));
		}
		if ((resizeProperties & ResizeProperties.AxesLabelSize) == ResizeProperties.AxesLabelSize)
		{
			if (flag)
			{
				yAxis.setLabelScales(getNewResizeVariable(num, 1f));
				xAxis.setLabelScales(getNewResizeVariable(num, 1f));
			}
			else
			{
				yAxis.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(num, yAxis.origAxisLabelSize));
				xAxis.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(num, xAxis.origAxisLabelSize));
			}
		}
		if ((resizeProperties & ResizeProperties.AxesLabelOffset) == ResizeProperties.AxesLabelOffset)
		{
			yAxis.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(num, yAxis.origAxisLabelSpaceOffset));
			xAxis.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(num, xAxis.origAxisLabelSpaceOffset));
		}
		if ((resizeProperties & ResizeProperties.AxesLabelOffset) == ResizeProperties.AxesLabelOffset)
		{
			yAxis.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(num, yAxis.origAxisTitleFontSize));
			xAxis.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(num, xAxis.origAxisTitleFontSize));
		}
		if ((resizeProperties & ResizeProperties.AxesLinePadding) == ResizeProperties.AxesLinePadding)
		{
			yAxis.AxisLinePadding = getNewResizeVariable(num, yAxis.origAxisLinePadding);
			xAxis.AxisLinePadding = getNewResizeVariable(num, xAxis.origAxisLinePadding);
		}
		if ((resizeProperties & ResizeProperties.AxesArrowSize) == ResizeProperties.AxesArrowSize)
		{
			Vector2 vector3 = new Vector2(getNewResizeVariable(num, yAxis.origAxisArrowSize.x), getNewResizeVariable(num, yAxis.origAxisArrowSize.y));
			changeSpriteSize(yAxis.AxisArrowDL, Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y));
			changeSpriteSize(yAxis.AxisArrowUR, Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y));
			Vector2 vector4 = new Vector2(getNewResizeVariable(num, xAxis.origAxisArrowSize.x), getNewResizeVariable(num, xAxis.origAxisArrowSize.y));
			changeSpriteSize(xAxis.AxisArrowDL, Mathf.RoundToInt(vector4.x), Mathf.RoundToInt(vector4.y));
			changeSpriteSize(xAxis.AxisArrowUR, Mathf.RoundToInt(vector4.x), Mathf.RoundToInt(vector4.y));
		}
		if ((resizeProperties & ResizeProperties.LegendFontSize) == ResizeProperties.LegendFontSize)
		{
			if (flag)
			{
				legend.setLabelScales(getNewResizeVariable(num, 1f));
			}
			else
			{
				legend.legendEntryFontSize = Mathf.RoundToInt(getNewResizeVariable(num, legend.origLegendEntryFontSize));
			}
		}
		if ((resizeProperties & ResizeProperties.LegendEntrySize) == ResizeProperties.LegendEntrySize)
		{
			if (!legend.setWidthFromLabels)
			{
				legend.legendEntryWidth = getNewResizeVariable(num, legend.origLegendEntryWidth);
			}
			legend.legendEntryHeight = getNewResizeVariable(num, legend.origLegendEntryHeight);
		}
		if ((resizeProperties & ResizeProperties.LegendOffset) == ResizeProperties.LegendOffset)
		{
			legend.offset = getNewResizeVariable(num, legend.origOffset);
		}
		if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize)
		{
			legend.legendEntryLinkSpacing = getNewResizeVariable(num, legend.origLegendEntryLinkSpacing);
			legend.legendEntrySpacing = getNewResizeVariable(num, legend.origLegendEntrySpacing);
		}
		if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize)
		{
			barWidth = getNewResizeVariable(vector2.x, origBarWidth);
		}
		if ((resizeProperties & ResizeProperties.SeriesPointSize) != ResizeProperties.SeriesPointSize && (resizeProperties & ResizeProperties.SeriesLineWidth) != ResizeProperties.SeriesLineWidth && (resizeProperties & ResizeProperties.SeriesDataLabelSize) != ResizeProperties.SeriesDataLabelSize && (resizeProperties & ResizeProperties.SeriesDataLabelOffset) != ResizeProperties.SeriesDataLabelOffset)
		{
			return;
		}
		for (int i = 0; i < lineSeries.Count; i++)
		{
			if (activeInHierarchy(lineSeries[i]))
			{
				WMG_Series component = lineSeries[i].GetComponent<WMG_Series>();
				if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize)
				{
					component.pointWidthHeight = getNewResizeVariable(num, component.origPointWidthHeight);
				}
				if ((resizeProperties & ResizeProperties.SeriesLineWidth) == ResizeProperties.SeriesLineWidth)
				{
					component.lineScale = getNewResizeVariable(num, component.origLineScale);
				}
				if ((resizeProperties & ResizeProperties.SeriesDataLabelSize) == ResizeProperties.SeriesDataLabelSize)
				{
					component.dataLabelsFontSize = Mathf.RoundToInt(getNewResizeVariable(num, component.origDataLabelsFontSize));
				}
				if ((resizeProperties & ResizeProperties.SeriesDataLabelOffset) == ResizeProperties.SeriesDataLabelOffset)
				{
					component.dataLabelsOffset = new Vector2(getNewResizeVariable(num, component.origDataLabelOffset.x), getNewResizeVariable(num, component.origDataLabelOffset.y));
				}
			}
		}
	}

	private float getNewResizeVariable(float sizeFactor, float variable)
	{
		return variable + (sizeFactor - 1f) * variable;
	}
}
