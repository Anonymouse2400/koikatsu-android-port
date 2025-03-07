using System.Collections.Generic;
using UnityEngine;

public class WMG_Radar_Graph : WMG_Axis_Graph
{
	[SerializeField]
	private List<Color> _dataSeriesColors;

	public WMG_List<Color> dataSeriesColors = new WMG_List<Color>();

	[SerializeField]
	private List<string> _labelStrings;

	public WMG_List<string> labelStrings = new WMG_List<string>();

	public bool randomData;

	[SerializeField]
	private int _numPoints;

	[SerializeField]
	private Vector2 _offset;

	[SerializeField]
	private float _degreeOffset;

	[SerializeField]
	private float _radarMinVal;

	[SerializeField]
	private float _radarMaxVal;

	[SerializeField]
	private int _numGrids;

	[SerializeField]
	private float _gridLineWidth;

	[SerializeField]
	private Color _gridColor;

	[SerializeField]
	private int _numDataSeries;

	[SerializeField]
	private float _dataSeriesLineWidth;

	[SerializeField]
	private Color _labelsColor;

	[SerializeField]
	private float _labelsOffset;

	[SerializeField]
	private int _fontSize;

	[SerializeField]
	private bool _hideLabels;

	public List<WMG_Series> grids;

	public List<WMG_Series> dataSeries;

	public WMG_Series radarLabels;

	private bool createdLabels;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	private WMG_Change_Obj radarGraphC = new WMG_Change_Obj();

	private WMG_Change_Obj gridsC = new WMG_Change_Obj();

	private WMG_Change_Obj labelsC = new WMG_Change_Obj();

	private WMG_Change_Obj dataSeriesC = new WMG_Change_Obj();

	private bool hasInit2;

	public int numPoints
	{
		get
		{
			return _numPoints;
		}
		set
		{
			if (_numPoints != value)
			{
				_numPoints = value;
				radarGraphC.Changed();
			}
		}
	}

	public Vector2 offset
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
				radarGraphC.Changed();
			}
		}
	}

	public float degreeOffset
	{
		get
		{
			return _degreeOffset;
		}
		set
		{
			if (_degreeOffset != value)
			{
				_degreeOffset = value;
				radarGraphC.Changed();
			}
		}
	}

	public float radarMinVal
	{
		get
		{
			return _radarMinVal;
		}
		set
		{
			if (_radarMinVal != value)
			{
				_radarMinVal = value;
				radarGraphC.Changed();
			}
		}
	}

	public float radarMaxVal
	{
		get
		{
			return _radarMaxVal;
		}
		set
		{
			if (_radarMaxVal != value)
			{
				_radarMaxVal = value;
				radarGraphC.Changed();
			}
		}
	}

	public int numGrids
	{
		get
		{
			return _numGrids;
		}
		set
		{
			if (_numGrids != value)
			{
				_numGrids = value;
				gridsC.Changed();
			}
		}
	}

	public float gridLineWidth
	{
		get
		{
			return _gridLineWidth;
		}
		set
		{
			if (_gridLineWidth != value)
			{
				_gridLineWidth = value;
				gridsC.Changed();
			}
		}
	}

	public Color gridColor
	{
		get
		{
			return _gridColor;
		}
		set
		{
			if (_gridColor != value)
			{
				_gridColor = value;
				gridsC.Changed();
			}
		}
	}

	public int numDataSeries
	{
		get
		{
			return _numDataSeries;
		}
		set
		{
			if (_numDataSeries != value)
			{
				_numDataSeries = value;
				dataSeriesC.Changed();
			}
		}
	}

	public float dataSeriesLineWidth
	{
		get
		{
			return _dataSeriesLineWidth;
		}
		set
		{
			if (_dataSeriesLineWidth != value)
			{
				_dataSeriesLineWidth = value;
				dataSeriesC.Changed();
			}
		}
	}

	public Color labelsColor
	{
		get
		{
			return _labelsColor;
		}
		set
		{
			if (_labelsColor != value)
			{
				_labelsColor = value;
				labelsC.Changed();
			}
		}
	}

	public float labelsOffset
	{
		get
		{
			return _labelsOffset;
		}
		set
		{
			if (_labelsOffset != value)
			{
				_labelsOffset = value;
				labelsC.Changed();
			}
		}
	}

	public int fontSize
	{
		get
		{
			return _fontSize;
		}
		set
		{
			if (_fontSize != value)
			{
				_fontSize = value;
				labelsC.Changed();
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
				labelsC.Changed();
			}
		}
	}

	private void Start()
	{
		Init();
		PauseCallbacks();
		radarGraphC.Changed();
	}

	public new void Init()
	{
		if (!hasInit2)
		{
			hasInit2 = true;
			changeObjs.Add(radarGraphC);
			changeObjs.Add(gridsC);
			changeObjs.Add(labelsC);
			changeObjs.Add(dataSeriesC);
			dataSeriesColors.SetList(_dataSeriesColors);
			dataSeriesColors.Changed += dataSeriesColorsChanged;
			labelStrings.SetList(_labelStrings);
			labelStrings.Changed += labelStringsChanged;
			radarGraphC.OnChange += GraphChanged;
			gridsC.OnChange += GridsChanged;
			labelsC.OnChange += LabelsChanged;
			dataSeriesC.OnChange += DataSeriesChanged;
			PauseCallbacks();
		}
	}

	private void Update()
	{
		Refresh();
	}

	public new void Refresh()
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

	public void dataSeriesColorsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref dataSeriesColors, ref _dataSeriesColors, oneValChanged, index);
		dataSeriesC.Changed();
	}

	public void labelStringsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref labelStrings, ref _labelStrings, oneValChanged, index);
		labelsC.Changed();
	}

	public void GridsChanged()
	{
		updateGrids();
	}

	public void DataSeriesChanged()
	{
		updateDataSeries();
	}

	public void LabelsChanged()
	{
		updateLabels();
	}

	public new void GraphChanged()
	{
		updateGrids();
		updateDataSeries();
		updateLabels();
	}

	private void updateLabels()
	{
		if (!createdLabels)
		{
			WMG_Series wMG_Series = addSeriesAt(numDataSeries + numGrids);
			wMG_Series.hideLines = true;
			createdLabels = true;
			wMG_Series.pointPrefab = 3;
			radarLabels = wMG_Series;
		}
		for (int i = 0; i < numPoints; i++)
		{
			if (labelStrings.Count <= i)
			{
				labelStrings.AddNoCb(string.Empty, ref _labelStrings);
			}
		}
		for (int num = labelStrings.Count - 1; num >= 0; num--)
		{
			if (labelStrings[num] != null && num >= numPoints)
			{
				labelStrings.RemoveAtNoCb(num, ref _labelStrings);
			}
		}
		radarLabels.hidePoints = hideLabels;
		radarLabels.pointValues.SetList(GenCircular2(numPoints, offset.x, offset.y, labelsOffset + (radarMaxVal - radarMinVal), degreeOffset));
		List<GameObject> points = radarLabels.getPoints();
		for (int j = 0; j < points.Count && j < numPoints; j++)
		{
			changeLabelFontSize(points[j], fontSize);
			changeLabelText(points[j], labelStrings[j]);
		}
		radarLabels.pointColor = labelsColor;
	}

	private void updateDataSeries()
	{
		for (int i = 0; i < numDataSeries; i++)
		{
			if (dataSeries.Count <= i)
			{
				WMG_Series wMG_Series = addSeriesAt(numGrids + i);
				wMG_Series.connectFirstToLast = true;
				wMG_Series.hidePoints = true;
				dataSeries.Add(wMG_Series);
			}
			if (dataSeriesColors.Count <= i)
			{
				dataSeriesColors.AddNoCb(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f), ref _dataSeriesColors);
			}
		}
		for (int num = dataSeries.Count - 1; num >= 0; num--)
		{
			if (dataSeries[num] != null && num >= numDataSeries)
			{
				deleteSeriesAt(numGrids + num);
				dataSeries.RemoveAt(num);
			}
		}
		for (int num2 = dataSeriesColors.Count - 1; num2 >= 0; num2--)
		{
			if (num2 >= numDataSeries)
			{
				dataSeriesColors.RemoveAtNoCb(num2, ref _dataSeriesColors);
			}
		}
		for (int j = 0; j < numDataSeries; j++)
		{
			WMG_Series component = lineSeries[j + numGrids].GetComponent<WMG_Series>();
			if (randomData)
			{
				component.pointValues.SetList(GenRadar(GenRandomList(numPoints, radarMinVal, radarMaxVal), offset.x, offset.y, degreeOffset));
			}
			component.lineScale = dataSeriesLineWidth;
			component.linePadding = dataSeriesLineWidth;
			component.lineColor = dataSeriesColors[j];
		}
	}

	private void updateGrids()
	{
		for (int i = 0; i < numGrids; i++)
		{
			if (grids.Count <= i)
			{
				WMG_Series wMG_Series = addSeriesAt(i);
				wMG_Series.connectFirstToLast = true;
				wMG_Series.hidePoints = true;
				grids.Add(wMG_Series);
			}
		}
		for (int num = grids.Count - 1; num >= 0; num--)
		{
			if (grids[num] != null && num >= numGrids)
			{
				deleteSeriesAt(num);
				grids.RemoveAt(num);
			}
		}
		for (int j = 0; j < numGrids; j++)
		{
			WMG_Series component = lineSeries[j].GetComponent<WMG_Series>();
			component.pointValues.SetList(GenCircular2(numPoints, offset.x, offset.y, ((float)j + 1f) / (float)numGrids * (radarMaxVal - radarMinVal), degreeOffset));
			component.lineScale = gridLineWidth;
			component.linePadding = gridLineWidth;
			component.lineColor = gridColor;
		}
	}
}
