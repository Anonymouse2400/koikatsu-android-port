using System.Collections.Generic;
using UnityEngine;

public class WMG_Axis : WMG_GUI_Functions
{
	public enum labelTypes
	{
		ticks = 0,
		ticks_center = 1,
		groups = 2,
		manual = 3
	}

	public delegate string AxisLabelLabeler(WMG_Axis axis, int labelIndex);

	public WMG_Axis_Graph graph;

	[SerializeField]
	private List<string> _axisLabels;

	public WMG_List<string> axisLabels = new WMG_List<string>();

	public GameObject AxisTitle;

	public GameObject GridLines;

	public GameObject AxisTicks;

	public GameObject AxisLine;

	public GameObject AxisArrowUR;

	public GameObject AxisArrowDL;

	public GameObject AxisObj;

	public GameObject AxisLabelObjs;

	[SerializeField]
	private float _AxisMinValue;

	[SerializeField]
	private float _AxisMaxValue;

	[SerializeField]
	private int _AxisNumTicks;

	[SerializeField]
	private bool _MinAutoGrow;

	[SerializeField]
	private bool _MaxAutoGrow;

	[SerializeField]
	private bool _MinAutoShrink;

	[SerializeField]
	private bool _MaxAutoShrink;

	[SerializeField]
	private float _AxisLinePadding;

	[SerializeField]
	private bool _AxisUseNonTickPercent;

	[SerializeField]
	private float _AxisNonTickPercent;

	[SerializeField]
	private bool _HideAxisArrowTopRight;

	[SerializeField]
	private bool _HideAxisArrowBotLeft;

	[SerializeField]
	private bool _AxisArrowTopRight;

	[SerializeField]
	private bool _AxisArrowBotLeft;

	[SerializeField]
	private bool _AxisTicksRightAbove;

	[SerializeField]
	private int _AxisTick;

	[SerializeField]
	private bool _hideTick;

	[SerializeField]
	private labelTypes _LabelType;

	[SerializeField]
	private int _AxisLabelSkipStart;

	[SerializeField]
	private int _AxisLabelSkipInterval;

	[SerializeField]
	private float _AxisLabelRotation;

	[SerializeField]
	private bool _SetLabelsUsingMaxMin;

	[SerializeField]
	private int _AxisLabelSize;

	[SerializeField]
	private Color _AxisLabelColor = Color.white;

	[SerializeField]
	private FontStyle _AxisLabelFontStyle;

	[SerializeField]
	private Font _AxisLabelFont;

	[SerializeField]
	private int _numDecimalsAxisLabels;

	[SerializeField]
	private bool _hideLabels;

	[SerializeField]
	private float _AxisLabelSpaceOffset;

	[SerializeField]
	private float _autoFitRotation;

	[SerializeField]
	private float _autoFitMaxBorder;

	[SerializeField]
	private float _AxisLabelSpacing;

	[SerializeField]
	private float _AxisLabelDistBetween;

	[SerializeField]
	private bool _hideGrid;

	[SerializeField]
	private bool _hideTicks;

	[SerializeField]
	private string _AxisTitleString;

	[SerializeField]
	private Vector2 _AxisTitleOffset;

	[SerializeField]
	private int _AxisTitleFontSize;

	private float GridLineLength;

	private float AxisLinePaddingTot;

	private float AxisPercentagePosition;

	private bool hasInit;

	private WMG_Axis otherAxis;

	private WMG_Axis otherAxis2;

	public Vector2 anchorVec;

	public AxisLabelLabeler axisLabelLabeler;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	private WMG_Change_Obj graphC = new WMG_Change_Obj();

	private WMG_Change_Obj seriesC = new WMG_Change_Obj();

	public float AxisMinValue
	{
		get
		{
			return _AxisMinValue;
		}
		set
		{
			if (_AxisMinValue != value)
			{
				_AxisMinValue = value;
				graphC.Changed();
				seriesC.Changed();
			}
		}
	}

	public float AxisMaxValue
	{
		get
		{
			return _AxisMaxValue;
		}
		set
		{
			if (_AxisMaxValue != value)
			{
				_AxisMaxValue = value;
				graphC.Changed();
				seriesC.Changed();
			}
		}
	}

	public int AxisNumTicks
	{
		get
		{
			return _AxisNumTicks;
		}
		set
		{
			if (_AxisNumTicks != value)
			{
				_AxisNumTicks = value;
				if (_AxisNumTicks < 2)
				{
					_AxisNumTicks = 2;
				}
				graphC.Changed();
			}
		}
	}

	public bool MinAutoGrow
	{
		get
		{
			return _MinAutoGrow;
		}
		set
		{
			if (_MinAutoGrow != value)
			{
				_MinAutoGrow = value;
				graphC.Changed();
				seriesC.Changed();
			}
		}
	}

	public bool MaxAutoGrow
	{
		get
		{
			return _MaxAutoGrow;
		}
		set
		{
			if (_MaxAutoGrow != value)
			{
				_MaxAutoGrow = value;
				graphC.Changed();
				seriesC.Changed();
			}
		}
	}

	public bool MinAutoShrink
	{
		get
		{
			return _MinAutoShrink;
		}
		set
		{
			if (_MinAutoShrink != value)
			{
				_MinAutoShrink = value;
				graphC.Changed();
				seriesC.Changed();
			}
		}
	}

	public bool MaxAutoShrink
	{
		get
		{
			return _MaxAutoShrink;
		}
		set
		{
			if (_MaxAutoShrink != value)
			{
				_MaxAutoShrink = value;
				graphC.Changed();
				seriesC.Changed();
			}
		}
	}

	public float AxisLinePadding
	{
		get
		{
			return _AxisLinePadding;
		}
		set
		{
			if (_AxisLinePadding != value)
			{
				_AxisLinePadding = value;
				graphC.Changed();
			}
		}
	}

	public bool AxisUseNonTickPercent
	{
		get
		{
			return _AxisUseNonTickPercent;
		}
		set
		{
			if (_AxisUseNonTickPercent != value)
			{
				_AxisUseNonTickPercent = value;
				graphC.Changed();
			}
		}
	}

	public float AxisNonTickPercent
	{
		get
		{
			return _AxisNonTickPercent;
		}
		set
		{
			if (_AxisNonTickPercent != value)
			{
				_AxisNonTickPercent = value;
				graphC.Changed();
			}
		}
	}

	public bool HideAxisArrowTopRight
	{
		get
		{
			return _HideAxisArrowTopRight;
		}
		set
		{
			if (_HideAxisArrowTopRight != value)
			{
				_HideAxisArrowTopRight = value;
				graphC.Changed();
			}
		}
	}

	public bool HideAxisArrowBotLeft
	{
		get
		{
			return _HideAxisArrowBotLeft;
		}
		set
		{
			if (_HideAxisArrowBotLeft != value)
			{
				_HideAxisArrowBotLeft = value;
				graphC.Changed();
			}
		}
	}

	public bool AxisArrowTopRight
	{
		get
		{
			return _AxisArrowTopRight;
		}
		set
		{
			if (_AxisArrowTopRight != value)
			{
				_AxisArrowTopRight = value;
				graphC.Changed();
			}
		}
	}

	public bool AxisArrowBotLeft
	{
		get
		{
			return _AxisArrowBotLeft;
		}
		set
		{
			if (_AxisArrowBotLeft != value)
			{
				_AxisArrowBotLeft = value;
				graphC.Changed();
			}
		}
	}

	public bool AxisTicksRightAbove
	{
		get
		{
			return _AxisTicksRightAbove;
		}
		set
		{
			if (_AxisTicksRightAbove != value)
			{
				_AxisTicksRightAbove = value;
				graphC.Changed();
			}
		}
	}

	public int AxisTick
	{
		get
		{
			return _AxisTick;
		}
		set
		{
			if (_AxisTick != value)
			{
				_AxisTick = value;
				graphC.Changed();
			}
		}
	}

	public bool hideTick
	{
		get
		{
			return _hideTick;
		}
		set
		{
			if (_hideTick != value)
			{
				_hideTick = value;
				graphC.Changed();
			}
		}
	}

	public labelTypes LabelType
	{
		get
		{
			return _LabelType;
		}
		set
		{
			if (_LabelType != value)
			{
				_LabelType = value;
				graphC.Changed();
			}
		}
	}

	public int AxisLabelSkipInterval
	{
		get
		{
			return _AxisLabelSkipInterval;
		}
		set
		{
			if (_AxisLabelSkipInterval != value)
			{
				_AxisLabelSkipInterval = value;
				graphC.Changed();
			}
		}
	}

	public int AxisLabelSkipStart
	{
		get
		{
			return _AxisLabelSkipStart;
		}
		set
		{
			if (_AxisLabelSkipStart != value)
			{
				_AxisLabelSkipStart = value;
				graphC.Changed();
			}
		}
	}

	public float AxisLabelRotation
	{
		get
		{
			return _AxisLabelRotation;
		}
		set
		{
			if (_AxisLabelRotation != value)
			{
				_AxisLabelRotation = value;
				graphC.Changed();
			}
		}
	}

	public bool SetLabelsUsingMaxMin
	{
		get
		{
			return _SetLabelsUsingMaxMin;
		}
		set
		{
			if (_SetLabelsUsingMaxMin != value)
			{
				_SetLabelsUsingMaxMin = value;
				graphC.Changed();
			}
		}
	}

	public int AxisLabelSize
	{
		get
		{
			return _AxisLabelSize;
		}
		set
		{
			if (_AxisLabelSize != value)
			{
				_AxisLabelSize = value;
				graphC.Changed();
			}
		}
	}

	public Color AxisLabelColor
	{
		get
		{
			return _AxisLabelColor;
		}
		set
		{
			if (_AxisLabelColor != value)
			{
				_AxisLabelColor = value;
				graphC.Changed();
			}
		}
	}

	public FontStyle AxisLabelFontStyle
	{
		get
		{
			return _AxisLabelFontStyle;
		}
		set
		{
			if (_AxisLabelFontStyle != value)
			{
				_AxisLabelFontStyle = value;
				graphC.Changed();
			}
		}
	}

	public Font AxisLabelFont
	{
		get
		{
			return _AxisLabelFont;
		}
		set
		{
			if (_AxisLabelFont != value)
			{
				_AxisLabelFont = value;
				graphC.Changed();
			}
		}
	}

	public int numDecimalsAxisLabels
	{
		get
		{
			return _numDecimalsAxisLabels;
		}
		set
		{
			if (_numDecimalsAxisLabels != value)
			{
				_numDecimalsAxisLabels = value;
				graphC.Changed();
			}
		}
	}

	public bool hideLabels
	{
		get
		{
			return _hideLabels;
		}
		set
		{
			if (_hideLabels != value)
			{
				_hideLabels = value;
				graphC.Changed();
			}
		}
	}

	public float AxisLabelSpaceOffset
	{
		get
		{
			return _AxisLabelSpaceOffset;
		}
		set
		{
			if (_AxisLabelSpaceOffset != value)
			{
				_AxisLabelSpaceOffset = value;
				graphC.Changed();
			}
		}
	}

	public float autoFitRotation
	{
		get
		{
			return _autoFitRotation;
		}
		set
		{
			if (_autoFitRotation != value)
			{
				_autoFitRotation = value;
				graphC.Changed();
			}
		}
	}

	public float autoFitMaxBorder
	{
		get
		{
			return _autoFitMaxBorder;
		}
		set
		{
			if (_autoFitMaxBorder != value)
			{
				_autoFitMaxBorder = value;
				graphC.Changed();
			}
		}
	}

	public float AxisLabelSpacing
	{
		get
		{
			return _AxisLabelSpacing;
		}
		set
		{
			if (_AxisLabelSpacing != value)
			{
				_AxisLabelSpacing = value;
				graphC.Changed();
			}
		}
	}

	public float AxisLabelDistBetween
	{
		get
		{
			return _AxisLabelDistBetween;
		}
		set
		{
			if (_AxisLabelDistBetween != value)
			{
				_AxisLabelDistBetween = value;
				graphC.Changed();
			}
		}
	}

	public bool hideGrid
	{
		get
		{
			return _hideGrid;
		}
		set
		{
			if (_hideGrid != value)
			{
				_hideGrid = value;
				graphC.Changed();
			}
		}
	}

	public bool hideTicks
	{
		get
		{
			return _hideTicks;
		}
		set
		{
			if (_hideTicks != value)
			{
				_hideTicks = value;
				graphC.Changed();
			}
		}
	}

	public string AxisTitleString
	{
		get
		{
			return _AxisTitleString;
		}
		set
		{
			if (_AxisTitleString != value)
			{
				_AxisTitleString = value;
				graphC.Changed();
			}
		}
	}

	public Vector2 AxisTitleOffset
	{
		get
		{
			return _AxisTitleOffset;
		}
		set
		{
			if (_AxisTitleOffset != value)
			{
				_AxisTitleOffset = value;
				graphC.Changed();
			}
		}
	}

	public int AxisTitleFontSize
	{
		get
		{
			return _AxisTitleFontSize;
		}
		set
		{
			if (_AxisTitleFontSize != value)
			{
				_AxisTitleFontSize = value;
				graphC.Changed();
			}
		}
	}

	public float AxisLength
	{
		get
		{
			if (isY)
			{
				return graph.yAxisLength;
			}
			return graph.xAxisLength;
		}
	}

	public int origAxisLabelSize { get; private set; }

	public float origAxisLabelSpaceOffset { get; private set; }

	public int origAxisTitleFontSize { get; private set; }

	public float origAxisLinePadding { get; private set; }

	public Vector2 origAxisArrowSize { get; private set; }

	public bool isY { get; private set; }

	public bool isSecondary { get; private set; }

	public void Init(WMG_Axis otherAxis, WMG_Axis otherAxis2, bool isY, bool isSecondary)
	{
		if (!hasInit)
		{
			hasInit = true;
			changeObjs.Add(graphC);
			changeObjs.Add(seriesC);
			this.otherAxis = otherAxis;
			this.otherAxis2 = otherAxis2;
			this.isY = isY;
			this.isSecondary = isSecondary;
			axisLabels.SetList(_axisLabels);
			axisLabels.Changed += axisLabelsChanged;
			graphC.OnChange += GraphChanged;
			seriesC.OnChange += SeriesChanged;
			axisLabelLabeler = defaultAxisLabelLabeler;
			setOriginalPropertyValues();
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

	private void GraphChanged()
	{
		graph.graphC.Changed();
	}

	private void SeriesChanged()
	{
		graph.seriesNoCountC.Changed();
	}

	private void axisLabelsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref axisLabels, ref _axisLabels, oneValChanged, index);
		graphC.Changed();
	}

	public void setOriginalPropertyValues()
	{
		origAxisLabelSize = AxisLabelSize;
		origAxisTitleFontSize = AxisTitleFontSize;
		origAxisLabelSpaceOffset = AxisLabelSpaceOffset;
		origAxisLinePadding = AxisLinePadding;
		origAxisArrowSize = getSpriteSize(AxisArrowDL);
	}

	public void setDualYAxes()
	{
		_AxisArrowTopRight = false;
		_AxisArrowBotLeft = false;
		_AxisTicksRightAbove = false;
		_hideTick = false;
		_AxisTick = 0;
		_AxisNonTickPercent = 0f;
		otherAxis.setOtherArrowTopRight(true);
		otherAxis.setOtherArrowBotLeft(false);
		otherAxis.setOtherRightAbove(false);
		otherAxis.setOtherHideTick(false);
		otherAxis.setOtherAxisTick(0);
		otherAxis.setOtherAxisNonTickPercent(0f);
		otherAxis2.setOtherArrowTopRight(true);
		otherAxis2.setOtherArrowBotLeft(false);
		otherAxis2.setOtherRightAbove(true);
		otherAxis2.setOtherHideTick(false);
		otherAxis2.setOtherAxisTick(AxisNumTicks - 1);
		otherAxis2.setOtherAxisNonTickPercent(1f);
	}

	public void setAxisTopRight(bool rightAbove)
	{
		_AxisArrowTopRight = true;
		_AxisArrowBotLeft = false;
		otherAxis.setOtherHideTick(false);
		otherAxis.setOtherAxisTick(0);
		otherAxis.setOtherAxisNonTickPercent(0f);
		_AxisTicksRightAbove = rightAbove;
	}

	public void setAxisBotLeft(bool rightAbove)
	{
		_AxisArrowTopRight = false;
		_AxisArrowBotLeft = true;
		otherAxis.setOtherHideTick(false);
		otherAxis.setOtherAxisTick(AxisNumTicks - 1);
		otherAxis.setOtherAxisNonTickPercent(1f);
		_AxisTicksRightAbove = rightAbove;
	}

	public void setAxisMiddle(bool rightAbove)
	{
		_AxisArrowTopRight = true;
		_AxisArrowBotLeft = true;
		otherAxis.setOtherHideTick(true);
		otherAxis.setOtherAxisTick(AxisNumTicks / 2);
		otherAxis.setOtherAxisNonTickPercent(0.5f);
		_AxisTicksRightAbove = rightAbove;
	}

	public void setOtherAxisNonTickPercent(float val)
	{
		_AxisNonTickPercent = val;
	}

	public void setOtherAxisTick(int val)
	{
		_AxisTick = val;
	}

	public void setOtherHideTick(bool val)
	{
		_hideTick = val;
	}

	public void setOtherRightAbove(bool val)
	{
		_AxisTicksRightAbove = val;
	}

	public void setOtherArrowBotLeft(bool val)
	{
		_AxisArrowBotLeft = val;
	}

	public void setOtherArrowTopRight(bool val)
	{
		_AxisArrowTopRight = val;
	}

	public void possiblyHideTickBasedOnPercent()
	{
		if (otherAxis.AxisUseNonTickPercent && AxisNumTicks % 2 == 0)
		{
			_hideTick = false;
		}
	}

	public void ChangeOrientation()
	{
		labelTypes labelType = LabelType;
		float axisMaxValue = AxisMaxValue;
		float axisMinValue = AxisMinValue;
		int axisNumTicks = AxisNumTicks;
		int tnumDecimalsAxisLabels = numDecimalsAxisLabels;
		bool minAutoGrow = MinAutoGrow;
		bool maxAutoGrow = MaxAutoGrow;
		bool minAutoShrink = MinAutoShrink;
		bool maxAutoShrink = MaxAutoShrink;
		bool setLabelsUsingMaxMin = SetLabelsUsingMaxMin;
		float axisLabelSpacing = AxisLabelSpacing;
		string axisTitleString = AxisTitleString;
		bool tHideTicks = hideTicks;
		List<string> tLabels = new List<string>(axisLabels);
		LabelType = otherAxis.LabelType;
		AxisMaxValue = otherAxis.AxisMaxValue;
		AxisMinValue = otherAxis.AxisMinValue;
		AxisNumTicks = otherAxis.AxisNumTicks;
		hideTicks = otherAxis.hideTicks;
		numDecimalsAxisLabels = otherAxis.numDecimalsAxisLabels;
		MinAutoGrow = otherAxis.MinAutoGrow;
		MaxAutoGrow = otherAxis.MaxAutoGrow;
		MinAutoShrink = otherAxis.MinAutoShrink;
		MaxAutoShrink = otherAxis.MaxAutoShrink;
		SetLabelsUsingMaxMin = otherAxis.SetLabelsUsingMaxMin;
		AxisLabelSpacing = otherAxis.AxisLabelSpacing;
		AxisTitleString = otherAxis.AxisTitleString;
		axisLabels.SetList(otherAxis.axisLabels);
		otherAxis.ChangeOrientationEnd(labelType, axisMaxValue, axisMinValue, axisNumTicks, tnumDecimalsAxisLabels, minAutoGrow, maxAutoGrow, minAutoShrink, maxAutoShrink, setLabelsUsingMaxMin, axisLabelSpacing, axisTitleString, tLabels, tHideTicks);
	}

	public void ChangeOrientationEnd(labelTypes tLabelType, float tAxisMaxValue, float tAxisMinValue, int tAxisNumTicks, int tnumDecimalsAxisLabels, bool tMinAutoGrow, bool tMaxAutoGrow, bool tMinAutoShrink, bool tMaxAutoShrink, bool tSetLabelsUsingMaxMin, float tAxisLabelSpacing, string tAxisTitleString, List<string> tLabels, bool tHideTicks)
	{
		LabelType = tLabelType;
		AxisMaxValue = tAxisMaxValue;
		AxisMinValue = tAxisMinValue;
		AxisNumTicks = tAxisNumTicks;
		hideTicks = tHideTicks;
		numDecimalsAxisLabels = tnumDecimalsAxisLabels;
		MinAutoGrow = tMinAutoGrow;
		MaxAutoGrow = tMaxAutoGrow;
		MinAutoShrink = tMinAutoShrink;
		MaxAutoShrink = tMaxAutoShrink;
		SetLabelsUsingMaxMin = tSetLabelsUsingMaxMin;
		AxisLabelSpacing = tAxisLabelSpacing;
		AxisTitleString = tAxisTitleString;
		axisLabels.SetList(tLabels);
	}

	public void updateAxesRelativeToOrigin(float originVal)
	{
		if (graph.axesType == WMG_Axis_Graph.axesTypes.AUTO_ORIGIN || graph.axesType == (WMG_Axis_Graph.axesTypes)((!isY) ? 3 : 4))
		{
			bool axisTicksRightAbove = otherAxis.AxisTicksRightAbove;
			if (originVal >= otherAxis.AxisMaxValue)
			{
				otherAxis.setAxisBotLeft(false);
				_AxisTicksRightAbove = true;
			}
			else if (originVal <= otherAxis.AxisMinValue)
			{
				otherAxis.setAxisTopRight(false);
				_AxisTicksRightAbove = false;
			}
			else
			{
				otherAxis.setAxisMiddle(false);
				_AxisTicksRightAbove = false;
				_AxisTick = Mathf.RoundToInt((originVal - otherAxis.AxisMinValue) / (otherAxis.AxisMaxValue - otherAxis.AxisMinValue) * (float)(otherAxis.AxisNumTicks - 1));
				_AxisNonTickPercent = (originVal - otherAxis.AxisMinValue) / (otherAxis.AxisMaxValue - otherAxis.AxisMinValue);
			}
			otherAxis.setOtherRightAbove(axisTicksRightAbove);
		}
	}

	public void UpdateAxesGridsAndTicks()
	{
		if (AxisNumTicks <= 1)
		{
			_AxisNumTicks = 1;
			GridLineLength = 0f;
		}
		else
		{
			GridLineLength = AxisLength / (float)(AxisNumTicks - 1);
		}
		if (AxisUseNonTickPercent)
		{
			AxisPercentagePosition = AxisNonTickPercent;
		}
		else if (otherAxis.AxisNumTicks == 1)
		{
			AxisPercentagePosition = 1f;
		}
		else
		{
			AxisPercentagePosition = (float)AxisTick / ((float)otherAxis.AxisNumTicks - 1f);
		}
		if (GridLines != null)
		{
			SetActive(GridLines, !hideGrid);
			if (!hideGrid)
			{
				WMG_Grid component = GridLines.GetComponent<WMG_Grid>();
				if (isY)
				{
					component.gridNumNodesY = AxisNumTicks;
					component.gridLinkLengthY = GridLineLength;
					component.gridLinkLengthX = otherAxis.AxisLength;
				}
				else
				{
					component.gridNumNodesX = AxisNumTicks;
					component.gridLinkLengthX = GridLineLength;
					component.gridLinkLengthY = otherAxis.AxisLength;
				}
				component.Refresh();
			}
		}
		SetActive(AxisTicks, !hideTicks);
		if (!hideTicks)
		{
			WMG_Grid component2 = AxisTicks.GetComponent<WMG_Grid>();
			if (isY)
			{
				component2.gridNumNodesY = AxisNumTicks;
				component2.gridLinkLengthY = GridLineLength;
			}
			else
			{
				component2.gridNumNodesX = AxisNumTicks;
				component2.gridLinkLengthX = GridLineLength;
			}
			component2.Refresh();
			if (!AxisTicksRightAbove)
			{
				if (isY)
				{
					changeSpritePositionToX(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength - (float)(graph.axisWidth / 2) - graph.tickSize.y / 2f);
				}
				else
				{
					changeSpritePositionToY(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength - (float)(graph.axisWidth / 2) - graph.tickSize.y / 2f);
				}
			}
			else if (isY)
			{
				changeSpritePositionToX(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength + (float)(graph.axisWidth / 2) + graph.tickSize.y / 2f);
			}
			else
			{
				changeSpritePositionToY(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength + (float)(graph.axisWidth / 2) + graph.tickSize.y / 2f);
			}
			foreach (WMG_Node axisTickNode in GetAxisTickNodes())
			{
				changeSpriteSize(axisTickNode.objectToScale, Mathf.RoundToInt((!isY) ? graph.tickSize.x : graph.tickSize.y), Mathf.RoundToInt((!isY) ? graph.tickSize.y : graph.tickSize.x));
			}
		}
		AxisLinePaddingTot = 2f * AxisLinePadding;
		float num = 0f;
		if (AxisArrowTopRight)
		{
			num += AxisLinePadding / 2f;
		}
		else
		{
			AxisLinePaddingTot -= AxisLinePadding;
		}
		if (AxisArrowBotLeft)
		{
			num -= AxisLinePadding / 2f;
		}
		else
		{
			AxisLinePaddingTot -= AxisLinePadding;
		}
		if (isY)
		{
			changeSpriteSize(AxisLine, graph.axisWidth, Mathf.RoundToInt(AxisLength + AxisLinePaddingTot));
			changeSpritePositionTo(AxisLine, new Vector3(0f, num + AxisLength / 2f, 0f));
			changeSpritePositionToX(AxisObj, AxisPercentagePosition * otherAxis.AxisLength);
		}
		else
		{
			changeSpriteSize(AxisLine, Mathf.RoundToInt(AxisLength + AxisLinePaddingTot), graph.axisWidth);
			changeSpritePositionTo(AxisLine, new Vector3(num + AxisLength / 2f, 0f, 0f));
			changeSpritePositionToY(AxisObj, AxisPercentagePosition * otherAxis.AxisLength);
		}
		SetActiveAnchoredSprite(AxisArrowUR, !HideAxisArrowTopRight && AxisArrowTopRight);
		SetActiveAnchoredSprite(AxisArrowDL, !HideAxisArrowBotLeft && AxisArrowBotLeft);
	}

	public void UpdateTitle()
	{
		if (!(AxisTitle != null))
		{
			return;
		}
		changeLabelText(AxisTitle, AxisTitleString);
		changeLabelFontSize(AxisTitle, AxisTitleFontSize);
		if (isY)
		{
			if (anchorVec.x == 1f)
			{
				AxisTitle.transform.localEulerAngles = new Vector3(0f, 0f, 270f);
				setAnchor(AxisTitle, anchorVec, anchorVec, new Vector2(AxisTitleOffset.x, AxisTitleOffset.y));
			}
			else
			{
				AxisTitle.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
				setAnchor(AxisTitle, anchorVec, anchorVec, new Vector2(0f - AxisTitleOffset.x, AxisTitleOffset.y));
			}
		}
		else if (anchorVec.y == 1f)
		{
			setAnchor(AxisTitle, anchorVec, anchorVec, new Vector2(AxisTitleOffset.x, AxisTitleOffset.y));
		}
		else
		{
			setAnchor(AxisTitle, anchorVec, anchorVec, new Vector2(AxisTitleOffset.x, 0f - AxisTitleOffset.y));
		}
	}

	public void UpdateAxesMinMaxValues()
	{
		if (!MinAutoGrow && !MaxAutoGrow && !MinAutoShrink && !MaxAutoShrink)
		{
			return;
		}
		float num = float.PositiveInfinity;
		float num2 = float.NegativeInfinity;
		for (int i = 0; i < graph.lineSeries.Count; i++)
		{
			if (!activeInHierarchy(graph.lineSeries[i]))
			{
				continue;
			}
			WMG_Series component = graph.lineSeries[i].GetComponent<WMG_Series>();
			if (graph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
			{
				for (int j = 0; j < component.pointValues.Count; j++)
				{
					if (isY)
					{
						if (component.pointValues[j].y < num)
						{
							num = component.pointValues[j].y;
						}
						if (component.pointValues[j].y > num2)
						{
							num2 = component.pointValues[j].y;
						}
						if ((graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked || graph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) && graph.TotalPointValues[j] + AxisMinValue > num2)
						{
							num2 = graph.TotalPointValues[j] + AxisMinValue;
						}
					}
					else
					{
						if (component.pointValues[j].x < num)
						{
							num = component.pointValues[j].x;
						}
						if (component.pointValues[j].x > num2)
						{
							num2 = component.pointValues[j].x;
						}
					}
				}
				continue;
			}
			for (int k = 0; k < component.pointValues.Count; k++)
			{
				if (isY)
				{
					if (component.pointValues[k].x < num)
					{
						num = component.pointValues[k].x;
					}
					if (component.pointValues[k].x > num2)
					{
						num2 = component.pointValues[k].x;
					}
					continue;
				}
				if (component.pointValues[k].y < num)
				{
					num = component.pointValues[k].y;
				}
				if (component.pointValues[k].y > num2)
				{
					num2 = component.pointValues[k].y;
				}
				if ((graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked || graph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) && graph.TotalPointValues[k] + AxisMinValue > num2)
				{
					num2 = graph.TotalPointValues[k] + AxisMinValue;
				}
			}
		}
		if ((!MinAutoGrow && !MaxAutoGrow && !MinAutoShrink && !MaxAutoShrink) || num == float.PositiveInfinity || num2 == float.NegativeInfinity)
		{
			return;
		}
		float axisMaxValue = AxisMaxValue;
		float axisMinValue = AxisMinValue;
		if (MaxAutoGrow && num2 > axisMaxValue)
		{
			AutoSetAxisMinMax(num2, num, true, true, axisMinValue, axisMaxValue);
		}
		if (MinAutoGrow && num < axisMinValue)
		{
			AutoSetAxisMinMax(num, num2, false, true, axisMinValue, axisMaxValue);
		}
		if (num2 != num)
		{
			if (MaxAutoShrink && graph.autoShrinkAtPercent > (num2 - axisMinValue) / (axisMaxValue - axisMinValue))
			{
				AutoSetAxisMinMax(num2, num, true, false, axisMinValue, axisMaxValue);
			}
			if (MinAutoShrink && graph.autoShrinkAtPercent > (axisMaxValue - num) / (axisMaxValue - axisMinValue))
			{
				AutoSetAxisMinMax(num, num2, false, false, axisMinValue, axisMaxValue);
			}
		}
	}

	private void AutoSetAxisMinMax(float val, float val2, bool max, bool grow, float aMin, float aMax)
	{
		int num = 0;
		num = AxisNumTicks - 1;
		float num2 = 1f + graph.autoGrowAndShrinkByPercent;
		float num3 = 0f;
		num3 = (max ? ((!grow) ? (num2 * (val - val2) / (float)num) : (num2 * (val - aMin) / (float)num)) : ((!grow) ? (num2 * (val2 - val) / (float)num) : (num2 * (aMax - val) / (float)num)));
		if (num3 == 0f || aMax <= aMin)
		{
			return;
		}
		float num4 = num3;
		int num5 = 0;
		if (Mathf.Abs(num4) > 1f)
		{
			while (Mathf.Abs(num4) > 10f)
			{
				num5++;
				num4 /= 10f;
			}
		}
		else
		{
			while (Mathf.Abs(num4) < 0.1f)
			{
				num5--;
				num4 *= 10f;
			}
		}
		float num6 = Mathf.Pow(10f, num5 - 1);
		num4 = num3 - num3 % num6 + num6;
		float num7 = 0f;
		num7 = (max ? ((!grow) ? ((float)num * num4 + val2) : ((float)num * num4 + aMin)) : ((!grow) ? (val2 - (float)num * num4) : (aMax - (float)num * num4)));
		if (max)
		{
			AxisMaxValue = num7;
		}
		else
		{
			AxisMinValue = num7;
		}
	}

	public void UpdateAxesLabels()
	{
		int num = 0;
		num = ((LabelType == labelTypes.ticks) ? AxisNumTicks : ((LabelType == labelTypes.ticks_center) ? (AxisNumTicks - 1) : ((LabelType != labelTypes.groups) ? axisLabels.Count : graph.groups.Count)));
		float distBetween = graph.getDistBetween(graph.groups.Count, AxisLength);
		if (LabelType == labelTypes.ticks)
		{
			_AxisLabelDistBetween = AxisLength / (float)(num - 1);
		}
		else if (LabelType == labelTypes.ticks_center)
		{
			_AxisLabelDistBetween = AxisLength / (float)num;
		}
		else if (LabelType == labelTypes.groups)
		{
			_AxisLabelDistBetween = distBetween;
		}
		WMG_Grid component = AxisLabelObjs.GetComponent<WMG_Grid>();
		if (isY)
		{
			component.gridNumNodesY = num;
			component.gridLinkLengthY = AxisLabelDistBetween;
		}
		else
		{
			component.gridNumNodesX = num;
			component.gridLinkLengthX = AxisLabelDistBetween;
		}
		component.Refresh();
		for (int i = 0; i < num; i++)
		{
			if (axisLabels.Count <= i)
			{
				axisLabels.AddNoCb(string.Empty, ref _axisLabels);
			}
		}
		for (int num2 = axisLabels.Count - 1; num2 >= 0; num2--)
		{
			if (num2 >= num)
			{
				axisLabels.RemoveAtNoCb(num2, ref _axisLabels);
			}
		}
		if (LabelType == labelTypes.ticks)
		{
			_AxisLabelSpacing = 0f;
		}
		else if (LabelType == labelTypes.ticks_center)
		{
			if (AxisNumTicks == 1)
			{
				_AxisLabelSpacing = 0f;
			}
			else
			{
				_AxisLabelSpacing = AxisLength / (float)(AxisNumTicks - 1) / 2f;
			}
		}
		else if (LabelType == labelTypes.groups)
		{
			if (graph.graphType == WMG_Axis_Graph.graphTypes.line || graph.graphType == WMG_Axis_Graph.graphTypes.line_stacked)
			{
				_AxisLabelSpacing = 0f;
			}
			else
			{
				_AxisLabelSpacing = distBetween / 2f;
				if (graph.graphType == WMG_Axis_Graph.graphTypes.bar_side)
				{
					_AxisLabelSpacing += (float)graph.lineSeries.Count * graph.barWidth / 2f;
				}
				else if (graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked)
				{
					_AxisLabelSpacing += graph.barWidth / 2f;
				}
				else if (graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent)
				{
					_AxisLabelSpacing += graph.barWidth / 2f;
				}
				else if (graph.graphType == WMG_Axis_Graph.graphTypes.combo)
				{
					_AxisLabelSpacing += (float)graph.NumComboBarSeries() * graph.barWidth / 2f;
				}
				if (isY)
				{
					_AxisLabelSpacing += 2f;
				}
			}
		}
		float num3 = 0f;
		if (LabelType == labelTypes.ticks || (LabelType == labelTypes.groups && AxisNumTicks == graph.groups.Count))
		{
			num3 = graph.tickSize.y;
		}
		if (isY)
		{
			if (!AxisTicksRightAbove)
			{
				changeSpritePositionToX(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength - num3 - (float)(graph.axisWidth / 2));
			}
			else
			{
				changeSpritePositionToX(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength + num3 + (float)(graph.axisWidth / 2));
			}
		}
		else if (!AxisTicksRightAbove)
		{
			changeSpritePositionToY(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength - num3 - (float)(graph.axisWidth / 2));
		}
		else
		{
			changeSpritePositionToY(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength + num3 + (float)(graph.axisWidth / 2));
		}
		List<WMG_Node> axisLabelNodes = GetAxisLabelNodes();
		if (axisLabelNodes == null)
		{
			return;
		}
		for (int j = 0; j < axisLabels.Count && j < axisLabelNodes.Count; j++)
		{
			SetActive(axisLabelNodes[j].gameObject, !hideLabels);
			if (LabelType == labelTypes.ticks && hideTick && j == otherAxis.AxisTick)
			{
				SetActive(axisLabelNodes[otherAxis.AxisTick].gameObject, false);
			}
			if (!graph._autoFitting)
			{
				axisLabelNodes[j].objectToLabel.transform.localEulerAngles = new Vector3(0f, 0f, AxisLabelRotation);
			}
			if (!isY && !graph.autoFitLabels)
			{
				if (AxisLabelRotation > 0f)
				{
					if (!AxisTicksRightAbove)
					{
						changeSpritePivot(axisLabelNodes[j].objectToLabel, WMGpivotTypes.TopRight);
					}
					else
					{
						changeSpritePivot(axisLabelNodes[j].objectToLabel, WMGpivotTypes.BottomLeft);
					}
				}
				else if (!AxisTicksRightAbove)
				{
					changeSpritePivot(axisLabelNodes[j].objectToLabel, WMGpivotTypes.Top);
				}
				else
				{
					changeSpritePivot(axisLabelNodes[j].objectToLabel, WMGpivotTypes.Bottom);
				}
			}
			if (isY)
			{
				if (!AxisTicksRightAbove)
				{
					changeSpritePivot(axisLabelNodes[j].objectToLabel, WMGpivotTypes.Right);
					changeSpritePositionTo(axisLabelNodes[j].objectToLabel, new Vector3(0f - AxisLabelSpaceOffset, AxisLabelSpacing, 0f));
				}
				else
				{
					changeSpritePivot(axisLabelNodes[j].objectToLabel, WMGpivotTypes.Left);
					changeSpritePositionTo(axisLabelNodes[j].objectToLabel, new Vector3(AxisLabelSpaceOffset, AxisLabelSpacing, 0f));
				}
			}
			else if (!AxisTicksRightAbove)
			{
				changeSpritePositionTo(axisLabelNodes[j].objectToLabel, new Vector3(AxisLabelSpacing, 0f - AxisLabelSpaceOffset, 0f));
			}
			else
			{
				changeSpritePositionTo(axisLabelNodes[j].objectToLabel, new Vector3(AxisLabelSpacing, AxisLabelSpaceOffset, 0f));
			}
			if (!graph._autoFitting)
			{
				changeLabelFontSize(axisLabelNodes[j].objectToLabel, AxisLabelSize);
			}
			changeLabelColor(axisLabelNodes[j].objectToLabel, AxisLabelColor);
			changeLabelFontStyle(axisLabelNodes[j].objectToLabel, AxisLabelFontStyle);
			if (AxisLabelFont != null)
			{
				changeLabelFont(axisLabelNodes[j].objectToLabel, AxisLabelFont);
			}
			axisLabels.SetValNoCb(j, axisLabelLabeler(this, j), ref _axisLabels);
			changeLabelText(axisLabelNodes[j].objectToLabel, axisLabels[j]);
		}
	}

	private string defaultAxisLabelLabeler(WMG_Axis axis, int labelIndex)
	{
		if (axis.LabelType == labelTypes.groups)
		{
			return ((labelIndex - axis.AxisLabelSkipStart) % (axis.AxisLabelSkipInterval + 1) != 0) ? string.Empty : ((labelIndex < axis.AxisLabelSkipStart) ? string.Empty : axis.graph.groups[labelIndex]);
		}
		if (axis.SetLabelsUsingMaxMin)
		{
			float num = axis.AxisMinValue + (float)labelIndex * (axis.AxisMaxValue - axis.AxisMinValue) / (float)(axis.axisLabels.Count - 1);
			if (labelIndex == 0)
			{
				num = axis.AxisMinValue;
			}
			if (axis.graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent && ((axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) || (!axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)))
			{
				num = (float)labelIndex / ((float)axis.axisLabels.Count - 1f) * 100f;
			}
			float num2 = Mathf.Pow(10f, axis.numDecimalsAxisLabels);
			string text = (((labelIndex - axis.AxisLabelSkipStart) % (axis.AxisLabelSkipInterval + 1) != 0) ? string.Empty : ((labelIndex < axis.AxisLabelSkipStart) ? string.Empty : (Mathf.Round(num * num2) / num2).ToString()));
			if (axis.graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent && ((axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) || (!axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal)))
			{
				return (!string.IsNullOrEmpty(text)) ? (text + "%") : string.Empty;
			}
			return text;
		}
		return axis.axisLabels[labelIndex];
	}

	public void AutofitAxesLabels()
	{
		if (!graph.autoFitLabels || graph._autoFitting)
		{
			return;
		}
		graph._autoFitting = true;
		List<WMG_Node> axisLabelNodes = GetAxisLabelNodes();
		float num = graph.autoFitPadding;
		float num2 = graph.autoFitPadding;
		float num3 = graph.autoFitPadding;
		float num4 = graph.autoFitPadding;
		if (!graph.legend.hideLegend && graph.legend.offset >= 0f)
		{
			if (graph.legend.legendType == WMG_Legend.legendTypes.Bottom)
			{
				if (graph.legend.oppositeSideLegend)
				{
					num3 += (float)graph.legend.LegendHeight + graph.legend.offset;
				}
				else
				{
					num4 += (float)graph.legend.LegendHeight + graph.legend.offset;
				}
			}
			else if (graph.legend.oppositeSideLegend)
			{
				num += (float)graph.legend.LegendWidth + graph.legend.offset;
			}
			else
			{
				num2 += (float)graph.legend.LegendWidth + graph.legend.offset;
			}
		}
		float num5 = autoFitMaxBorder;
		Vector2 zero = Vector2.zero;
		if (isY)
		{
			zero = getLabelsMaxDiff(axisLabelNodes, AxisTicksRightAbove, AxisTicksRightAbove, num, num2, num3, num4);
			if (Mathf.Abs(zero.x) > 1f || Mathf.Abs(zero.y) > 1f)
			{
				if (AxisTicksRightAbove)
				{
					graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x, graph.paddingLeftRight.y - zero.x);
					graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x - zero.y, graph.paddingTopBottom.y);
				}
				else
				{
					graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x - zero.x, graph.paddingLeftRight.y);
					graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x, graph.paddingTopBottom.y - zero.y);
				}
				Vector2 vector = ((!AxisTicksRightAbove) ? new Vector2(num5 * getSpriteWidth(graph.gameObject) + num, graph.paddingLeftRight.y) : new Vector2(graph.paddingLeftRight.x, num5 * getSpriteWidth(graph.gameObject) + num2));
				if ((AxisTicksRightAbove && graph.paddingLeftRight.y > vector.y) || (!AxisTicksRightAbove && graph.paddingLeftRight.x > vector.x))
				{
					if (AxisTicksRightAbove)
					{
						graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x, vector.y);
					}
					else
					{
						graph.paddingLeftRight = new Vector2(vector.x, graph.paddingLeftRight.y);
					}
				}
				Vector2 vector2 = (AxisTicksRightAbove ? new Vector2(num5 * getSpriteHeight(graph.gameObject) + num3, graph.paddingTopBottom.y) : new Vector2(graph.paddingTopBottom.x, num5 * getSpriteHeight(graph.gameObject) + num4));
				if ((!AxisTicksRightAbove && graph.paddingTopBottom.y > vector2.y) || (AxisTicksRightAbove && graph.paddingTopBottom.x > vector2.x))
				{
					if (AxisTicksRightAbove)
					{
						graph.paddingTopBottom = new Vector2(vector2.x, graph.paddingTopBottom.y);
					}
					else
					{
						graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x, vector2.y);
					}
				}
				graph.UpdateBG();
			}
		}
		else
		{
			bool flag = false;
			bool flag2 = false;
			if (otherAxis.AxisTicksRightAbove)
			{
				flag2 = true;
			}
			else
			{
				flag = true;
			}
			bool flag3 = true;
			for (int i = 1; i < axisLabelNodes.Count; i++)
			{
				flag3 = flag3 && !rectIntersectRect(axisLabelNodes[i - 1].objectToLabel, axisLabelNodes[i].objectToLabel);
			}
			if (!flag3)
			{
				setLabelRotations(axisLabelNodes, autoFitRotation);
			}
			WMGpivotTypes wMGpivotTypes = WMGpivotTypes.Top;
			wMGpivotTypes = ((axisLabelNodes.Count > 0 && axisLabelNodes[0].objectToLabel.transform.localEulerAngles.z > 0f) ? (AxisTicksRightAbove ? WMGpivotTypes.BottomLeft : WMGpivotTypes.TopRight) : ((!AxisTicksRightAbove) ? WMGpivotTypes.Top : WMGpivotTypes.Bottom));
			foreach (WMG_Node item in axisLabelNodes)
			{
				changeSpritePivot(item.objectToLabel, wMGpivotTypes);
			}
			zero = getLabelsMaxDiff(axisLabelNodes, AxisTicksRightAbove, AxisTicksRightAbove, num, num2, num3, num4);
			if (Mathf.Abs(zero.x) > 1f || Mathf.Abs(zero.y) > 1f)
			{
				if (AxisTicksRightAbove)
				{
					if (flag2)
					{
						graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x, Mathf.Max(graph.paddingLeftRight.y - zero.x, graph.paddingLeftRight.y));
						graph.paddingTopBottom = new Vector2(Mathf.Max(graph.paddingTopBottom.x - zero.y, graph.paddingTopBottom.x), graph.paddingTopBottom.y);
					}
					else
					{
						graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x, graph.paddingLeftRight.y - zero.x);
						graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x - zero.y, graph.paddingTopBottom.y);
					}
				}
				else if (flag)
				{
					graph.paddingLeftRight = new Vector2(Mathf.Max(graph.paddingLeftRight.x - zero.x, graph.paddingLeftRight.x), graph.paddingLeftRight.y);
					graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x, Mathf.Max(graph.paddingTopBottom.y - zero.y, graph.paddingTopBottom.y));
				}
				else
				{
					graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x - zero.x, graph.paddingLeftRight.y);
					graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x, graph.paddingTopBottom.y - zero.y);
				}
				Vector2 vector3 = ((!AxisTicksRightAbove) ? new Vector2(num5 * getSpriteWidth(graph.gameObject) + num, graph.paddingLeftRight.y) : new Vector2(graph.paddingLeftRight.x, num5 * getSpriteWidth(graph.gameObject) + num2));
				if ((AxisTicksRightAbove && graph.paddingLeftRight.y > vector3.y) || (!AxisTicksRightAbove && graph.paddingLeftRight.x > vector3.x))
				{
					if (AxisTicksRightAbove)
					{
						graph.paddingLeftRight = new Vector2(graph.paddingLeftRight.x, vector3.y);
					}
					else
					{
						graph.paddingLeftRight = new Vector2(vector3.x, graph.paddingLeftRight.y);
					}
				}
				Vector2 vector4 = (AxisTicksRightAbove ? new Vector2(num5 * getSpriteHeight(graph.gameObject) + num3, graph.paddingTopBottom.y) : new Vector2(graph.paddingTopBottom.x, num5 * getSpriteHeight(graph.gameObject) + num4));
				if ((!AxisTicksRightAbove && graph.paddingTopBottom.y > vector4.y) || (AxisTicksRightAbove && graph.paddingTopBottom.x > vector4.x))
				{
					if (AxisTicksRightAbove)
					{
						graph.paddingTopBottom = new Vector2(vector4.x, graph.paddingTopBottom.y);
					}
					else
					{
						graph.paddingTopBottom = new Vector2(graph.paddingTopBottom.x, vector4.y);
					}
				}
				graph.UpdateBG();
			}
		}
		graph.GraphChanged();
		graph._autoFitting = false;
	}

	private Vector2 getLabelsMaxDiff(List<WMG_Node> LabelNodes, bool isRight, bool isTop, float paddingLeft, float paddingRight, float paddingTop, float paddingBot)
	{
		float num = float.PositiveInfinity;
		float num2 = float.PositiveInfinity;
		Vector2 xDif = Vector2.zero;
		Vector2 yDif = Vector2.zero;
		foreach (WMG_Node LabelNode in LabelNodes)
		{
			getRectDiffs(LabelNode.objectToLabel, graph.gameObject, ref xDif, ref yDif);
			if (isRight)
			{
				if (xDif.y < num)
				{
					num = xDif.y;
				}
			}
			else if (xDif.x < num)
			{
				num = xDif.x;
			}
			if (isTop)
			{
				if (yDif.y < num2)
				{
					num2 = yDif.y;
				}
			}
			else if (yDif.x < num2)
			{
				num2 = yDif.x;
			}
		}
		return new Vector2(num - ((!isRight) ? paddingLeft : paddingRight), num2 - ((!isTop) ? paddingBot : paddingTop));
	}

	private void setLabelRotations(List<WMG_Node> LabelNodes, float rotation)
	{
		foreach (WMG_Node LabelNode in LabelNodes)
		{
			LabelNode.objectToLabel.transform.localEulerAngles = new Vector3(0f, 0f, rotation);
		}
	}

	private void setFontSizeLabels(List<WMG_Node> LabelNodes, int newLabelSize)
	{
		foreach (WMG_Node LabelNode in LabelNodes)
		{
			changeLabelFontSize(LabelNode.objectToLabel, newLabelSize);
		}
	}

	public void setLabelScales(float newScale)
	{
		foreach (WMG_Node axisLabelNode in GetAxisLabelNodes())
		{
			axisLabelNode.objectToLabel.transform.localScale = new Vector3(newScale, newScale, 1f);
		}
	}

	public List<WMG_Node> GetAxisLabelNodes()
	{
		WMG_Grid component = AxisLabelObjs.GetComponent<WMG_Grid>();
		if (isY)
		{
			return component.getColumn(0);
		}
		return component.getRow(0);
	}

	public List<WMG_Node> GetAxisTickNodes()
	{
		WMG_Grid component = AxisTicks.GetComponent<WMG_Grid>();
		if (isY)
		{
			return component.getColumn(0);
		}
		return component.getRow(0);
	}
}
