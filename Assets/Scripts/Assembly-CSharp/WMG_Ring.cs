using UnityEngine;

public class WMG_Ring : WMG_GUI_Functions
{
	public GameObject ring;

	public GameObject band;

	public GameObject label;

	public GameObject textLine;

	public GameObject labelText;

	public GameObject labelPoint;

	public GameObject labelBackground;

	public GameObject line;

	private Sprite ringSprite;

	private Sprite bandSprite;

	private WMG_Ring_Graph graph;

	private int ringTexSize;

	private int bandTexSize;

	public void initialize(WMG_Ring_Graph graph)
	{
		ringSprite = WMG_Util.createSprite(getTexture(ring));
		bandSprite = WMG_Util.createSprite(getTexture(band));
		setTexture(ring, ringSprite);
		setTexture(band, bandSprite);
		this.graph = graph;
		changeSpriteParent(label, graph.ringLabelsParent);
	}

	public void updateRing(int ringNum)
	{
		float ringRadius = graph.getRingRadius(ringNum);
		graph.textureChanger(ring, ringSprite, 2 * ringNum + 1, graph.outerRadius * 2f, ringRadius - graph.ringWidth, ringRadius, graph.antiAliasing, graph.antiAliasingStrength);
		if (graph.bandMode)
		{
			SetActive(band, true);
			graph.textureChanger(band, bandSprite, 2 * ringNum + 1 + 1, graph.outerRadius * 2f, ringRadius + graph.bandPadding, graph.getRingRadius(ringNum + 1) - graph.ringWidth - graph.bandPadding, graph.antiAliasing, graph.antiAliasingStrength);
		}
		else
		{
			SetActive(band, false);
		}
	}

	public void updateRingPoint(int ringNum)
	{
		float ringRadius = graph.getRingRadius(ringNum);
		if (graph.bandMode && graph.ringColor.a == 0f)
		{
			float ringRadius2 = graph.getRingRadius(ringNum + 1);
			changeSpritePositionToY(labelPoint, 0f - (ringRadius + (ringRadius2 - ringRadius) / 2f - graph.RingWidthFactor * graph.ringWidth / 2f));
		}
		else
		{
			changeSpritePositionToY(labelPoint, 0f - (ringRadius - graph.RingWidthFactor * graph.ringWidth / 2f));
		}
		int num = Mathf.RoundToInt(graph.RingWidthFactor * graph.ringWidth + graph.RingWidthFactor * graph.ringPointWidthFactor);
		changeSpriteSize(labelPoint, num, num);
	}
}
