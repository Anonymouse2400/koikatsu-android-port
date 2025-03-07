using UnityEngine;

public class WMG_Bezier_Band : WMG_GUI_Functions
{
	public GameObject bandFillSpriteGO;

	public GameObject bandLineSpriteGO;

	public GameObject labelParent;

	public GameObject percentLabel;

	public GameObject label;

	private Sprite bandFillSprite;

	private Sprite bandLineSprite;

	private Material bandFillMat;

	private Material bandLineMat;

	private Color[] bandFillColors;

	private Color[] bandLineColors;

	private int texSize;

	private WMG_Bezier_Band_Graph graph;

	public float cumulativePercent;

	public float prevCumulativePercent;

	private int size;

	private int maxS;

	private int superSamplingRate = 3;

	private int xPad = 2;

	public void Init(WMG_Bezier_Band_Graph graph)
	{
		this.graph = graph;
		bandFillSprite = WMG_Util.createSprite(getTexture(bandFillSpriteGO));
		bandLineSprite = WMG_Util.createSprite(getTexture(bandLineSpriteGO));
		texSize = bandFillSprite.texture.width;
		bandFillColors = new Color[texSize * texSize];
		bandLineColors = new Color[texSize * texSize];
		setTexture(bandFillSpriteGO, bandFillSprite);
		setTexture(bandLineSpriteGO, bandLineSprite);
		size = Mathf.RoundToInt(Mathf.Sqrt(bandFillColors.Length));
		maxS = size - 1;
	}

	public void setCumulativePercents(float val, float prev)
	{
		cumulativePercent = val / graph.TotalVal;
		prevCumulativePercent = prev / graph.TotalVal;
	}

	public void setFillColor(Color color)
	{
		changeSpriteColor(bandFillSpriteGO, color);
	}

	public void setLineColor(Color color)
	{
		changeSpriteColor(bandLineSpriteGO, color);
	}

	public void UpdateBand()
	{
		updateColors(ref bandFillColors, ref bandLineColors);
		bandFillSprite.texture.SetPixels(bandFillColors);
		bandFillSprite.texture.Apply();
		bandLineSprite.texture.SetPixels(bandLineColors);
		bandLineSprite.texture.Apply();
	}

	private void updateColors(ref Color[] bandFillColors, ref Color[] bandLineColors)
	{
		bandFillColors = new Color[texSize * texSize];
		bandLineColors = new Color[texSize * texSize];
		int num = Mathf.Max(0, graph.bandLineWidth - 1);
		int[] array = new int[size];
		for (int i = 0; i <= 1; i++)
		{
			float num2 = graph.startHeightPercent * (float)maxS;
			float num3 = prevCumulativePercent;
			if (i == 1)
			{
				num3 = cumulativePercent;
			}
			Vector2 p = new Vector2(0f, (float)maxS / 2f + (0.5f - num3) * num2);
			Vector2 p2 = new Vector2(maxS, (1f - num3) * (float)maxS - (float)((i != 1) ? graph.bandSpacing : (-graph.bandSpacing)));
			if (i == 1 && cumulativePercent == 1f && graph.bandSpacing < graph.bandLineWidth)
			{
				p2 = new Vector2(p2.x, p2.y + (float)(graph.bandLineWidth - graph.bandSpacing));
			}
			Vector2 p3 = new Vector2(graph.cubicBezierP1.x * p2.x, p.y + graph.cubicBezierP1.y * (p2.y - p.y));
			Vector2 p4 = new Vector2((1f - graph.cubicBezierP2.x) * p2.x, p2.y - graph.cubicBezierP2.y * (p2.y - p.y));
			int[] array2 = new int[size];
			for (int j = 0; j < size * superSamplingRate; j++)
			{
				Vector2 vector = cubicBezier(p, p3, p4, p2, (float)j / ((float)(size * superSamplingRate) - 1f));
				int num4 = Mathf.RoundToInt(vector.x);
				int a = Mathf.RoundToInt(vector.y);
				array2[num4] = Mathf.Max(a, array2[num4]);
			}
			p = new Vector2(p.x, p.y - (float)num);
			p2 = new Vector2(p2.x, p2.y - (float)num);
			p3 = new Vector2(graph.cubicBezierP1.x * p2.x, p.y + graph.cubicBezierP1.y * (p2.y - p.y));
			p4 = new Vector2((1f - graph.cubicBezierP2.x) * p2.x, p2.y - graph.cubicBezierP2.y * (p2.y - p.y));
			int[] array3 = new int[size];
			for (int k = 0; k < size; k++)
			{
				array3[k] = maxS;
			}
			for (int l = 0; l < size * superSamplingRate; l++)
			{
				Vector2 vector2 = cubicBezier(p, p3, p4, p2, (float)l / ((float)(size * superSamplingRate) - 1f));
				int num5 = Mathf.RoundToInt(vector2.x);
				int a2 = Mathf.RoundToInt(vector2.y);
				array3[num5] = Mathf.Min(a2, array3[num5]);
			}
			for (int m = xPad; m < size - xPad; m++)
			{
				int num6 = array2[m] - array3[m];
				num6++;
				if (i == 0)
				{
					array[m] = array3[m] - 1;
				}
				for (int n = 0; n < num6; n++)
				{
					int num7 = array2[m] - n;
					float num8 = 1f - Mathf.Abs((float)n - (float)num6 / 2f) / ((float)num6 / 2f);
					bandLineColors[m + size * num7] = new Color(1f, 1f, 1f, num8 * num8);
				}
				if (i == 1)
				{
					int num9 = array[m] - array2[m];
					for (int num10 = 0; num10 < num9; num10++)
					{
						int num11 = array[m] - num10;
						bandFillColors[m + size * num11] = Color.white;
					}
					int num12 = Mathf.Min(num / 2, num9 / 2);
					for (int num13 = 0; num13 < num12; num13++)
					{
						float a3 = ((float)num13 + 1f) / (float)num12;
						int num14 = array[m] - num13;
						bandFillColors[m + size * num14] = new Color(1f, 1f, 1f, a3);
					}
					for (int num15 = 0; num15 < num12; num15++)
					{
						float a4 = ((float)num15 + 1f) / (float)num12;
						int num16 = array2[m] + num15;
						bandFillColors[m + size * num16] = new Color(1f, 1f, 1f, a4);
					}
				}
			}
		}
	}

	private Vector2 cubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		float num = 1f - t;
		float num2 = num * num * num;
		float num3 = num * num * t * 3f;
		float num4 = num * t * t * 3f;
		float num5 = t * t * t;
		return num2 * p0 + num3 * p1 + num4 * p2 + num5 * p3;
	}
}
