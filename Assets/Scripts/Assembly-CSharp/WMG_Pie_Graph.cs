using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WMG_Pie_Graph : WMG_Graph_Manager
{
	public enum sortMethod
	{
		None = 0,
		Largest_First = 1,
		Smallest_First = 2,
		Alphabetically = 3,
		Reverse_Alphabetically = 4
	}

	[Flags]
	public enum ResizeProperties
	{
		LabelExplodeLength = 1,
		LabelFontSize = 2,
		LegendFontSize = 4,
		LegendSwatchSize = 8,
		LegendEntrySize = 0x10,
		LegendOffset = 0x20,
		AutoCenterPadding = 0x40
	}

	[SerializeField]
	private List<float> _sliceValues;

	public WMG_List<float> sliceValues = new WMG_List<float>();

	[SerializeField]
	private List<string> _sliceLabels;

	public WMG_List<string> sliceLabels = new WMG_List<string>();

	[SerializeField]
	private List<Color> _sliceColors;

	public WMG_List<Color> sliceColors = new WMG_List<Color>();

	public WMG_Data_Source sliceValuesDataSource;

	public WMG_Data_Source sliceLabelsDataSource;

	public WMG_Data_Source sliceColorsDataSource;

	public GameObject background;

	public GameObject backgroundCircle;

	public GameObject slicesParent;

	public WMG_Legend legend;

	public UnityEngine.Object legendEntryPrefab;

	public UnityEngine.Object nodePrefab;

	[SerializeField]
	private bool _resizeEnabled;

	[WMG_EnumFlag]
	[SerializeField]
	private ResizeProperties _resizeProperties;

	[SerializeField]
	private Vector2 _leftRightPadding;

	[SerializeField]
	private Vector2 _topBotPadding;

	[SerializeField]
	private float _bgCircleOffset;

	[SerializeField]
	private bool _autoCenter;

	[SerializeField]
	private float _autoCenterMinPadding;

	[SerializeField]
	private sortMethod _sortBy;

	[SerializeField]
	private bool _swapColorsDuringSort;

	[SerializeField]
	private WMG_Enums.labelTypes _sliceLabelType;

	[SerializeField]
	private float _explodeLength;

	[SerializeField]
	private bool _explodeSymmetrical;

	[SerializeField]
	private bool _useDoughnut;

	[SerializeField]
	private float _doughnutPercentage;

	[SerializeField]
	private bool _limitNumberSlices;

	[SerializeField]
	private bool _includeOthers;

	[SerializeField]
	private int _maxNumberSlices;

	[SerializeField]
	private string _includeOthersLabel;

	[SerializeField]
	private Color _includeOthersColor;

	[SerializeField]
	private float _animationDuration;

	[SerializeField]
	private float _sortAnimationDuration;

	[SerializeField]
	private float _sliceLabelExplodeLength;

	[SerializeField]
	private int _sliceLabelFontSize;

	[SerializeField]
	private int _numberDecimalsInPercents;

	[SerializeField]
	private Color _sliceLabelColor;

	[SerializeField]
	private bool _hideZeroValueLegendEntry;

	[SerializeField]
	private bool _interactivityEnabled;

	public Dictionary<string, WMG_Pie_Graph_Slice> LabelToSliceMap = new Dictionary<string, WMG_Pie_Graph_Slice>();

	private float origPieSize;

	private float origSliceLabelExplodeLength;

	private int origSliceLabelFontSize;

	private float origAutoCenterPadding;

	private float cachedContainerWidth;

	private float cachedContainerHeight;

	private List<GameObject> slices = new List<GameObject>();

	private int numSlices;

	private bool isOtherSlice;

	private float otherSliceValue;

	private float totalVal;

	private bool animSortSwap;

	private bool isAnimating;

	private Color[] colors;

	private Color[] origColors;

	private Sprite pieSprite;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	public WMG_Change_Obj graphC = new WMG_Change_Obj();

	private WMG_Change_Obj resizeC = new WMG_Change_Obj();

	private WMG_Change_Obj doughnutC = new WMG_Change_Obj();

	private WMG_Change_Obj interactivityC = new WMG_Change_Obj();

	private bool hasInit;

	private bool setOrig;

	private bool doughnutHasInit;

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

	public Vector2 leftRightPadding
	{
		get
		{
			return _leftRightPadding;
		}
		set
		{
			if (_leftRightPadding != value)
			{
				_leftRightPadding = value;
				graphC.Changed();
			}
		}
	}

	public Vector2 topBotPadding
	{
		get
		{
			return _topBotPadding;
		}
		set
		{
			if (_topBotPadding != value)
			{
				_topBotPadding = value;
				graphC.Changed();
			}
		}
	}

	public float bgCircleOffset
	{
		get
		{
			return _bgCircleOffset;
		}
		set
		{
			if (_bgCircleOffset != value)
			{
				_bgCircleOffset = value;
				graphC.Changed();
			}
		}
	}

	public bool autoCenter
	{
		get
		{
			return _autoCenter;
		}
		set
		{
			if (_autoCenter != value)
			{
				_autoCenter = value;
				graphC.Changed();
			}
		}
	}

	public float autoCenterMinPadding
	{
		get
		{
			return _autoCenterMinPadding;
		}
		set
		{
			if (_autoCenterMinPadding != value)
			{
				_autoCenterMinPadding = value;
				graphC.Changed();
			}
		}
	}

	public sortMethod sortBy
	{
		get
		{
			return _sortBy;
		}
		set
		{
			if (_sortBy != value)
			{
				_sortBy = value;
				graphC.Changed();
			}
		}
	}

	public bool swapColorsDuringSort
	{
		get
		{
			return _swapColorsDuringSort;
		}
		set
		{
			if (_swapColorsDuringSort != value)
			{
				_swapColorsDuringSort = value;
				graphC.Changed();
			}
		}
	}

	public WMG_Enums.labelTypes sliceLabelType
	{
		get
		{
			return _sliceLabelType;
		}
		set
		{
			if (_sliceLabelType != value)
			{
				_sliceLabelType = value;
				graphC.Changed();
			}
		}
	}

	public float explodeLength
	{
		get
		{
			return _explodeLength;
		}
		set
		{
			if (_explodeLength != value)
			{
				_explodeLength = value;
				graphC.Changed();
			}
		}
	}

	public bool explodeSymmetrical
	{
		get
		{
			return _explodeSymmetrical;
		}
		set
		{
			if (_explodeSymmetrical != value)
			{
				_explodeSymmetrical = value;
				graphC.Changed();
			}
		}
	}

	public bool useDoughnut
	{
		get
		{
			return _useDoughnut;
		}
		set
		{
			if (_useDoughnut != value)
			{
				_useDoughnut = value;
				doughnutC.Changed();
			}
		}
	}

	public float doughnutPercentage
	{
		get
		{
			return _doughnutPercentage;
		}
		set
		{
			if (_doughnutPercentage != value)
			{
				_doughnutPercentage = value;
				doughnutC.Changed();
			}
		}
	}

	public bool limitNumberSlices
	{
		get
		{
			return _limitNumberSlices;
		}
		set
		{
			if (_limitNumberSlices != value)
			{
				_limitNumberSlices = value;
				graphC.Changed();
			}
		}
	}

	public bool includeOthers
	{
		get
		{
			return _includeOthers;
		}
		set
		{
			if (_includeOthers != value)
			{
				_includeOthers = value;
				graphC.Changed();
			}
		}
	}

	public int maxNumberSlices
	{
		get
		{
			return _maxNumberSlices;
		}
		set
		{
			if (_maxNumberSlices != value)
			{
				_maxNumberSlices = value;
				graphC.Changed();
			}
		}
	}

	public string includeOthersLabel
	{
		get
		{
			return _includeOthersLabel;
		}
		set
		{
			if (_includeOthersLabel != value)
			{
				_includeOthersLabel = value;
				graphC.Changed();
			}
		}
	}

	public Color includeOthersColor
	{
		get
		{
			return _includeOthersColor;
		}
		set
		{
			if (_includeOthersColor != value)
			{
				_includeOthersColor = value;
				graphC.Changed();
			}
		}
	}

	public float animationDuration
	{
		get
		{
			return _animationDuration;
		}
		set
		{
			if (_animationDuration != value)
			{
				_animationDuration = value;
				graphC.Changed();
			}
		}
	}

	public float sortAnimationDuration
	{
		get
		{
			return _sortAnimationDuration;
		}
		set
		{
			if (_sortAnimationDuration != value)
			{
				_sortAnimationDuration = value;
				graphC.Changed();
			}
		}
	}

	public float sliceLabelExplodeLength
	{
		get
		{
			return _sliceLabelExplodeLength;
		}
		set
		{
			if (_sliceLabelExplodeLength != value)
			{
				_sliceLabelExplodeLength = value;
				graphC.Changed();
			}
		}
	}

	public int sliceLabelFontSize
	{
		get
		{
			return _sliceLabelFontSize;
		}
		set
		{
			if (_sliceLabelFontSize != value)
			{
				_sliceLabelFontSize = value;
				graphC.Changed();
			}
		}
	}

	public int numberDecimalsInPercents
	{
		get
		{
			return _numberDecimalsInPercents;
		}
		set
		{
			if (_numberDecimalsInPercents != value)
			{
				_numberDecimalsInPercents = value;
				graphC.Changed();
			}
		}
	}

	public Color sliceLabelColor
	{
		get
		{
			return _sliceLabelColor;
		}
		set
		{
			if (_sliceLabelColor != value)
			{
				_sliceLabelColor = value;
				graphC.Changed();
			}
		}
	}

	public bool hideZeroValueLegendEntry
	{
		get
		{
			return _hideZeroValueLegendEntry;
		}
		set
		{
			if (_hideZeroValueLegendEntry != value)
			{
				_hideZeroValueLegendEntry = value;
				graphC.Changed();
			}
		}
	}

	public bool interactivityEnabled
	{
		get
		{
			return _interactivityEnabled;
		}
		set
		{
			if (_interactivityEnabled != value)
			{
				_interactivityEnabled = value;
				interactivityC.Changed();
			}
		}
	}

	public float pieSize
	{
		get
		{
			return Mathf.Min(getSpriteWidth(base.gameObject) - leftRightPadding.x - leftRightPadding.y + 2f * explodeLength, getSpriteHeight(base.gameObject) - topBotPadding.x - topBotPadding.y + 2f * explodeLength);
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
		if (!hasInit)
		{
			hasInit = true;
			legend.Init();
			changeObjs.Add(graphC);
			changeObjs.Add(resizeC);
			changeObjs.Add(doughnutC);
			changeObjs.Add(interactivityC);
			if (animationDuration > 0f)
			{
				UpdateVisuals(true);
			}
			if (useDoughnut)
			{
				initDoughnut();
			}
			cachedContainerWidth = getSpriteWidth(base.gameObject);
			cachedContainerHeight = getSpriteHeight(base.gameObject);
			sliceValues.SetList(_sliceValues);
			sliceValues.Changed += sliceValuesChanged;
			sliceLabels.SetList(_sliceLabels);
			sliceLabels.Changed += sliceLabelsChanged;
			sliceColors.SetList(_sliceColors);
			sliceColors.Changed += sliceColorsChanged;
			graphC.OnChange += GraphChanged;
			resizeC.OnChange += ResizeChanged;
			doughnutC.OnChange += DoughtnutChanged;
			interactivityC.OnChange += InteractivityChanged;
			PauseCallbacks();
		}
	}

	private void initDoughnut()
	{
		if (!doughnutHasInit)
		{
			doughnutHasInit = true;
			GameObject gameObject = UnityEngine.Object.Instantiate(nodePrefab) as GameObject;
			Texture2D texture = getTexture(gameObject.GetComponent<WMG_Pie_Graph_Slice>().objectToColor);
			colors = texture.GetPixels();
			origColors = texture.GetPixels();
			pieSprite = WMG_Util.createSprite(texture);
			UnityEngine.Object.Destroy(gameObject);
			for (int i = 0; i < slices.Count; i++)
			{
				WMG_Pie_Graph_Slice component = slices[i].GetComponent<WMG_Pie_Graph_Slice>();
				setTexture(component.objectToColor, pieSprite);
				setTexture(component.objectToMask, pieSprite);
			}
		}
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

	private void PauseCallbacks()
	{
		for (int i = 0; i < changeObjs.Count; i++)
		{
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
		legend.PauseCallbacks();
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
		}
	}

	private void updateFromDataSource()
	{
		if (sliceValuesDataSource != null)
		{
			sliceValues.SetList(sliceValuesDataSource.getData<float>());
		}
		if (sliceLabelsDataSource != null)
		{
			sliceLabels.SetList(sliceLabelsDataSource.getData<string>());
		}
		if (sliceColorsDataSource != null)
		{
			sliceColors.SetList(sliceColorsDataSource.getData<Color>());
		}
		if ((sliceValuesDataSource != null || sliceLabelsDataSource != null || sliceColorsDataSource != null) && sortBy != 0)
		{
			sortData();
		}
	}

	private void ResizeChanged()
	{
		UpdateFromContainer();
		UpdateVisuals(true);
	}

	private void DoughtnutChanged()
	{
		if (useDoughnut)
		{
			initDoughnut();
			UpdateDoughnut();
		}
	}

	private void InteractivityChanged()
	{
		if (interactivityEnabled)
		{
			explodeSymmetrical = false;
		}
		for (int i = 0; i < slices.Count; i++)
		{
			WMG_Pie_Graph_Slice component = slices[i].GetComponent<WMG_Pie_Graph_Slice>();
			WMG_Raycatcher component2 = component.objectToColor.GetComponent<WMG_Raycatcher>();
			if (component2 != null && !interactivityEnabled)
			{
				UnityEngine.Object.Destroy(component2);
			}
			if (component2 == null && interactivityEnabled)
			{
				component.objectToColor.AddComponent<WMG_Raycatcher>();
			}
			if (interactivityEnabled)
			{
				setAsNotInteractible(component.objectToLabel);
			}
		}
		Canvas canv = null;
		getFirstCanvasOnSelfOrParent(base.transform, ref canv);
		if (!canv)
		{
			return;
		}
		WMG_Raycaster component3 = canv.GetComponent<WMG_Raycaster>();
		if (component3 == null)
		{
			if (interactivityEnabled)
			{
				canv.gameObject.AddComponent<WMG_Raycaster>();
			}
		}
		else if (!interactivityEnabled)
		{
			UnityEngine.Object.DestroyImmediate(component3);
			addRaycaster(canv.gameObject);
		}
	}

	private void GraphChanged()
	{
		if (!isAnimating)
		{
			UpdateVisuals(false);
		}
	}

	private void AllChanged()
	{
		if (useDoughnut)
		{
			initDoughnut();
			UpdateDoughnut();
		}
		InteractivityChanged();
		if (!isAnimating)
		{
			UpdateVisuals(false);
		}
	}

	private void sliceValuesChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref sliceValues, ref _sliceValues, oneValChanged, index);
		graphC.Changed();
	}

	private void sliceLabelsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref sliceLabels, ref _sliceLabels, oneValChanged, index);
		graphC.Changed();
	}

	private void sliceColorsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index)
	{
		WMG_Util.listChanged(editorChange, ref sliceColors, ref _sliceColors, oneValChanged, index);
		graphC.Changed();
	}

	private void setOriginalPropertyValues()
	{
		origPieSize = pieSize;
		origSliceLabelExplodeLength = sliceLabelExplodeLength;
		origSliceLabelFontSize = sliceLabelFontSize;
		origAutoCenterPadding = autoCenterMinPadding;
	}

	private void UpdateFromContainer()
	{
		if (!resizeEnabled)
		{
			return;
		}
		float sizeFactor = pieSize / origPieSize;
		if ((resizeProperties & ResizeProperties.LabelExplodeLength) == ResizeProperties.LabelExplodeLength)
		{
			sliceLabelExplodeLength = getNewResizeVariable(sizeFactor, origSliceLabelExplodeLength);
		}
		if ((resizeProperties & ResizeProperties.LabelFontSize) == ResizeProperties.LabelFontSize)
		{
			sliceLabelFontSize = Mathf.RoundToInt(getNewResizeVariable(sizeFactor, origSliceLabelFontSize));
		}
		if ((resizeProperties & ResizeProperties.LegendFontSize) == ResizeProperties.LegendFontSize)
		{
			legend.legendEntryFontSize = Mathf.RoundToInt(getNewResizeVariable(sizeFactor, legend.origLegendEntryFontSize));
		}
		if ((resizeProperties & ResizeProperties.LegendEntrySize) == ResizeProperties.LegendEntrySize)
		{
			if (!legend.setWidthFromLabels)
			{
				legend.legendEntryWidth = getNewResizeVariable(sizeFactor, legend.origLegendEntryWidth);
			}
			legend.legendEntryHeight = getNewResizeVariable(sizeFactor, legend.origLegendEntryHeight);
		}
		if ((resizeProperties & ResizeProperties.LegendSwatchSize) == ResizeProperties.LegendSwatchSize)
		{
			legend.pieSwatchSize = getNewResizeVariable(sizeFactor, legend.origPieSwatchSize);
		}
		if ((resizeProperties & ResizeProperties.LegendOffset) == ResizeProperties.LegendOffset)
		{
			legend.offset = getNewResizeVariable(sizeFactor, legend.origOffset);
		}
		if ((resizeProperties & ResizeProperties.AutoCenterPadding) == ResizeProperties.AutoCenterPadding)
		{
			autoCenterMinPadding = Mathf.RoundToInt(getNewResizeVariable(sizeFactor, origAutoCenterPadding));
		}
	}

	private float getNewResizeVariable(float sizeFactor, float variable)
	{
		return variable + (sizeFactor - 1f) * variable / 2f;
	}

	public void updateBG(int thePieSize)
	{
		changeSpriteSize(backgroundCircle, Mathf.RoundToInt((float)thePieSize + bgCircleOffset), Mathf.RoundToInt((float)thePieSize + bgCircleOffset));
		Vector2 paddingOffset = getPaddingOffset();
		changeSpritePositionTo(slicesParent, new Vector3(0f - paddingOffset.x, 0f - paddingOffset.y));
	}

	public Vector2 getPaddingOffset()
	{
		Vector2 spritePivot = getSpritePivot(base.gameObject);
		float num = (float)Mathf.RoundToInt(getSpriteWidth(base.gameObject)) * (spritePivot.x - 0.5f);
		float num2 = (float)Mathf.RoundToInt(getSpriteHeight(base.gameObject)) * (spritePivot.y - 0.5f);
		return new Vector2((0f - leftRightPadding.x) * 0.5f + leftRightPadding.y * 0.5f + num, topBotPadding.x * 0.5f - topBotPadding.y * 0.5f + num2);
	}

	public List<GameObject> getSlices()
	{
		return slices;
	}

	private void UpdateData()
	{
		isOtherSlice = false;
		numSlices = sliceValues.Count;
		if (limitNumberSlices && numSlices > maxNumberSlices)
		{
			numSlices = maxNumberSlices;
			if (includeOthers)
			{
				isOtherSlice = true;
				numSlices++;
			}
		}
		otherSliceValue = 0f;
		totalVal = 0f;
		for (int i = 0; i < sliceValues.Count; i++)
		{
			totalVal += sliceValues[i];
			if (isOtherSlice && i >= maxNumberSlices)
			{
				otherSliceValue += sliceValues[i];
			}
			if (limitNumberSlices && !isOtherSlice && i >= maxNumberSlices)
			{
				totalVal -= sliceValues[i];
			}
		}
	}

	private void CreateOrDeleteSlicesBasedOnValues()
	{
		LabelToSliceMap.Clear();
		for (int i = 0; i < numSlices; i++)
		{
			if (sliceLabels.Count <= i)
			{
				sliceLabels.Add(string.Empty);
			}
			if (sliceColors.Count <= i)
			{
				sliceColors.Add(Color.white);
			}
			if (slices.Count <= i)
			{
				GameObject gameObject = CreateNode(nodePrefab, slicesParent);
				slices.Add(gameObject);
				WMG_Pie_Graph_Slice component = gameObject.GetComponent<WMG_Pie_Graph_Slice>();
				component.pieRef = this;
				component.sliceIndex = i;
				if (useDoughnut)
				{
					setTexture(component.objectToColor, pieSprite);
					setTexture(component.objectToMask, pieSprite);
				}
				if (interactivityEnabled)
				{
					component.objectToColor.AddComponent<WMG_Raycatcher>();
					setAsNotInteractible(component.objectToLabel);
				}
				addPieSliceMouseEnterEvent(component.objectToColor);
				addPieSliceClickEvent(component.objectToColor);
			}
			if (legend.legendEntries.Count <= i)
			{
				WMG_Legend_Entry wMG_Legend_Entry = legend.createLegendEntry(legendEntryPrefab);
				addPieLegendEntryClickEvent(wMG_Legend_Entry.gameObject);
			}
		}
		for (int num = slices.Count - 1; num >= 0; num--)
		{
			if (slices[num] != null && num >= numSlices)
			{
				WMG_Pie_Graph_Slice component2 = slices[num].GetComponent<WMG_Pie_Graph_Slice>();
				DeleteNode(component2);
				slices.RemoveAt(num);
			}
		}
		for (int num2 = legend.legendEntries.Count - 1; num2 >= 0; num2--)
		{
			if (legend.legendEntries[num2] != null && num2 >= numSlices)
			{
				legend.deleteLegendEntry(num2);
			}
		}
	}

	private void UpdateVisuals(bool noAnim)
	{
		UpdateData();
		CreateOrDeleteSlicesBasedOnValues();
		if (totalVal == 0f && numSlices > 0)
		{
			return;
		}
		for (int i = 0; i < numSlices; i++)
		{
			WMG_Pie_Graph_Slice component = slices[i].GetComponent<WMG_Pie_Graph_Slice>();
			SetActive(component.objectToMask, explodeSymmetrical);
			if (explodeSymmetrical)
			{
				changeSpriteParent(component.objectToColor, component.objectToMask);
				continue;
			}
			changeSpriteParent(component.objectToColor, component.gameObject);
			bringSpriteToFront(component.objectToLabel);
		}
		int num = Mathf.RoundToInt(pieSize);
		updateBG(num);
		if (animationDuration == 0f && sortBy != 0)
		{
			sortData();
		}
		float num2 = 0f;
		if (!noAnim)
		{
			animSortSwap = false;
		}
		for (int j = 0; j < numSlices; j++)
		{
			float num3 = -1f * num2;
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			WMG_Pie_Graph_Slice component2 = slices[j].GetComponent<WMG_Pie_Graph_Slice>();
			if (sliceLabelType != 0 && !activeInHierarchy(component2.objectToLabel))
			{
				SetActive(component2.objectToLabel, true);
			}
			if (sliceLabelType == WMG_Enums.labelTypes.None && activeInHierarchy(component2.objectToLabel))
			{
				SetActive(component2.objectToLabel, false);
			}
			if (!explodeSymmetrical)
			{
				changeSpriteSize(component2.objectToColor, num, num);
			}
			else
			{
				changeSpriteSize(component2.objectToColor, num, num);
				changeSpriteSize(component2.objectToMask, num + Mathf.RoundToInt(explodeLength * 4f), num + Mathf.RoundToInt(explodeLength * 4f));
			}
			Color aColor = sliceColors[j];
			string text = sliceLabels[j];
			float num4 = sliceValues[j];
			if (isOtherSlice && j == numSlices - 1)
			{
				aColor = includeOthersColor;
				text = includeOthersLabel;
				num4 = otherSliceValue;
			}
			if (!LabelToSliceMap.ContainsKey(text))
			{
				LabelToSliceMap.Add(text, component2);
			}
			if (num4 == 0f)
			{
				SetActive(component2.objectToLabel, false);
			}
			float num5 = num4 / totalVal;
			component2.slicePercent = num5 * 360f;
			float num6 = num3 * -1f + 0.5f * num5 * 360f;
			float num7 = sliceLabelExplodeLength + (float)(num / 2);
			float num8 = Mathf.Sin(num6 * ((float)Math.PI / 180f));
			float num9 = Mathf.Cos(num6 * ((float)Math.PI / 180f));
			if (!noAnim && animationDuration > 0f)
			{
				isAnimating = true;
				WMG_Anim.animFill(component2.objectToColor, animationDuration, Ease.Linear, num5);
				WMG_Anim.animPosition(component2.objectToLabel, animationDuration, Ease.Linear, new Vector3(num7 * num8, num7 * num9));
				int newI = j;
				WMG_Anim.animPositionCallbackC(slices[j], animationDuration, Ease.Linear, new Vector3(explodeLength * num8, explodeLength * num9), delegate
				{
					shrinkSlices(newI);
				});
				if (!explodeSymmetrical)
				{
					WMG_Anim.animRotation(component2.objectToColor, animationDuration, Ease.Linear, new Vector3(0f, 0f, num3), false);
					WMG_Anim.animPosition(component2.objectToColor, animationDuration, Ease.Linear, Vector3.zero);
				}
				else
				{
					WMG_Anim.animRotation(component2.objectToColor, animationDuration, Ease.Linear, Vector3.zero, false);
					Vector2 vector = new Vector2((0f - explodeLength) * num8, (0f - explodeLength) * num9);
					float num10 = Mathf.Sin(num3 * ((float)Math.PI / 180f));
					float num11 = Mathf.Cos(num3 * ((float)Math.PI / 180f));
					WMG_Anim.animPosition(component2.objectToColor, animationDuration, Ease.Linear, new Vector3(num11 * vector.x + num10 * vector.y, num11 * vector.y - num10 * vector.x));
					WMG_Anim.animRotation(component2.objectToMask, animationDuration, Ease.Linear, new Vector3(0f, 0f, num3), false);
					WMG_Anim.animFill(component2.objectToMask, animationDuration, Ease.Linear, num5);
				}
			}
			else
			{
				changeSpriteFill(component2.objectToColor, num5);
				component2.objectToLabel.transform.localPosition = new Vector3(num7 * num8, num7 * num9);
				slices[j].transform.localPosition = new Vector3(explodeLength * num8, explodeLength * num9);
				if (!explodeSymmetrical)
				{
					component2.objectToColor.transform.localEulerAngles = new Vector3(0f, 0f, num3);
					component2.objectToColor.transform.localPosition = Vector3.zero;
				}
				else
				{
					component2.objectToColor.transform.localEulerAngles = Vector3.zero;
					Vector2 vector2 = new Vector2((0f - explodeLength) * num8, (0f - explodeLength) * num9);
					float num12 = Mathf.Sin(num3 * ((float)Math.PI / 180f));
					float num13 = Mathf.Cos(num3 * ((float)Math.PI / 180f));
					component2.objectToColor.transform.localPosition = new Vector3(num13 * vector2.x + num12 * vector2.y, num13 * vector2.y - num12 * vector2.x);
					component2.objectToMask.transform.localEulerAngles = new Vector3(0f, 0f, num3);
					changeSpriteFill(component2.objectToMask, num5);
				}
			}
			changeSpriteColor(component2.objectToColor, aColor);
			changeSpriteColor(component2.objectToMask, aColor);
			changeLabelText(component2.objectToLabel, getLabelText(text, sliceLabelType, num4, num5, numberDecimalsInPercents));
			changeLabelFontSize(component2.objectToLabel, sliceLabelFontSize);
			changeSpriteColor(component2.objectToLabel, sliceLabelColor);
			slices[j].name = text;
			legend.legendEntries[j].name = text;
			num2 += num5 * 360f;
			component2.slicePercentPosition = num2 - component2.slicePercent / 2f;
			WMG_Legend_Entry wMG_Legend_Entry = legend.legendEntries[j];
			changeLabelText(wMG_Legend_Entry.label, getLabelText(text, legend.labelType, num4, num5, legend.numDecimals));
			changeSpriteColor(wMG_Legend_Entry.swatchNode, aColor);
			if (hideZeroValueLegendEntry)
			{
				if (num4 == 0f)
				{
					SetActive(wMG_Legend_Entry.gameObject, false);
				}
				else
				{
					SetActive(wMG_Legend_Entry.gameObject, true);
				}
			}
			else
			{
				SetActive(wMG_Legend_Entry.gameObject, true);
			}
		}
		legend.LegendChanged();
		updateAutoCenter();
		if (!setOrig)
		{
			setOrig = true;
			setOriginalPropertyValues();
		}
	}

	private void updateAutoCenter()
	{
		if (!autoCenter)
		{
			return;
		}
		float num = autoCenterMinPadding + explodeLength + bgCircleOffset / 2f;
		if (legend.hideLegend)
		{
			leftRightPadding = new Vector2(num, num);
			topBotPadding = new Vector2(num, num);
		}
		else if (legend.legendType == WMG_Legend.legendTypes.Right)
		{
			topBotPadding = new Vector2(num, num);
			if (legend.oppositeSideLegend)
			{
				leftRightPadding = new Vector2(num + (float)legend.LegendWidth + Mathf.Abs(legend.offset), num);
			}
			else
			{
				leftRightPadding = new Vector2(num, num + (float)legend.LegendWidth + Mathf.Abs(legend.offset));
			}
		}
		else
		{
			leftRightPadding = new Vector2(num, num);
			if (!legend.oppositeSideLegend)
			{
				topBotPadding = new Vector2(num, num + (float)legend.LegendHeight + Mathf.Abs(legend.offset));
			}
			else
			{
				topBotPadding = new Vector2(num + (float)legend.LegendHeight + Mathf.Abs(legend.offset), num);
			}
		}
	}

	private void shrinkSlices(int sliceNum)
	{
		if (!animSortSwap && sortBy != 0)
		{
			animSortSwap = sortData();
		}
		if (animSortSwap)
		{
			if (sortAnimationDuration > 0f)
			{
				WMG_Anim.animScaleCallbackC(slices[sliceNum], sortAnimationDuration / 2f, Ease.Linear, Vector3.zero, delegate
				{
					enlargeSlices(sliceNum);
				});
			}
			else
			{
				isAnimating = false;
				UpdateVisuals(true);
			}
		}
		else
		{
			isAnimating = false;
		}
	}

	private void enlargeSlices(int sliceNum)
	{
		if (sliceNum == 0)
		{
			UpdateVisuals(true);
		}
		WMG_Anim.animScaleCallbackC(slices[sliceNum], sortAnimationDuration / 2f, Ease.Linear, Vector3.one, delegate
		{
			endSortAnimating(sliceNum);
		});
	}

	private void endSortAnimating(int sliceNum)
	{
		if (sliceNum == numSlices - 1)
		{
			animSortSwap = false;
			isAnimating = false;
		}
	}

	private bool sortData()
	{
		bool result = false;
		bool flag = true;
		bool flag2 = false;
		int num = numSlices;
		for (int i = 1; i <= num; i++)
		{
			if (!flag)
			{
				break;
			}
			flag = false;
			for (int j = 0; j < num - 1; j++)
			{
				flag2 = false;
				if (sortBy == sortMethod.Largest_First)
				{
					if (sliceValues[j + 1] > sliceValues[j])
					{
						flag2 = true;
					}
				}
				else if (sortBy == sortMethod.Smallest_First)
				{
					if (sliceValues[j + 1] < sliceValues[j])
					{
						flag2 = true;
					}
				}
				else if (sortBy == sortMethod.Alphabetically)
				{
					if (sliceLabels[j + 1].CompareTo(sliceLabels[j]) == -1)
					{
						flag2 = true;
					}
				}
				else if (sortBy == sortMethod.Reverse_Alphabetically && sliceLabels[j + 1].CompareTo(sliceLabels[j]) == 1)
				{
					flag2 = true;
				}
				if (flag2)
				{
					float val = sliceValues[j];
					sliceValues.SetValNoCb(j, sliceValues[j + 1], ref _sliceValues);
					sliceValues.SetValNoCb(j + 1, val, ref _sliceValues);
					string val2 = sliceLabels[j];
					sliceLabels.SetValNoCb(j, sliceLabels[j + 1], ref _sliceLabels);
					sliceLabels.SetValNoCb(j + 1, val2, ref _sliceLabels);
					GameObject value = slices[j];
					slices[j].GetComponent<WMG_Pie_Graph_Slice>().sliceIndex = j + 1;
					slices[j] = slices[j + 1];
					slices[j + 1].GetComponent<WMG_Pie_Graph_Slice>().sliceIndex = j;
					slices[j + 1] = value;
					if (swapColorsDuringSort)
					{
						Color val3 = sliceColors[j];
						sliceColors.SetValNoCb(j, sliceColors[j + 1], ref _sliceColors);
						sliceColors.SetValNoCb(j + 1, val3, ref _sliceColors);
					}
					flag = true;
					result = true;
				}
			}
		}
		return result;
	}

	private void UpdateDoughnut()
	{
		WMG_Util.updateBandColors(ref colors, pieSize, doughnutPercentage * pieSize / 2f, pieSize / 2f, true, 2f, origColors);
		pieSprite.texture.SetPixels(colors);
		pieSprite.texture.Apply();
	}

	public Vector3 getCalloutSlicePosition(string label, float amt)
	{
		if (LabelToSliceMap.ContainsKey(label))
		{
			return getPositionFromExplode(LabelToSliceMap[label], amt);
		}
		return Vector3.zero;
	}

	public Vector3 getPositionFromExplode(WMG_Pie_Graph_Slice slice, float amt)
	{
		float f = (float)Math.PI / 180f * (0f - slice.slicePercentPosition + 90f);
		return new Vector3(amt * Mathf.Cos(f), amt * Mathf.Sin(f), 0f);
	}
}
