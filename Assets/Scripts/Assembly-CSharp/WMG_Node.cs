using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Node : WMG_GUI_Functions
{
	public int id;

	public float radius;

	public bool isSquare;

	public int numLinks;

	public List<GameObject> links = new List<GameObject>();

	public List<float> linkAngles = new List<float>();

	public GameObject objectToScale;

	public GameObject objectToColor;

	public GameObject objectToLabel;

	public bool isSelected;

	public bool wasSelected;

	public bool BFS_mark;

	public int BFS_depth;

	public float Dijkstra_depth;

	public WMG_Series seriesRef;

	public GameObject CreateLink(GameObject target, UnityEngine.Object prefabLink, int linkId, GameObject parent, bool repos)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabLink) as GameObject;
		Vector3 localPosition = gameObject.transform.localPosition;
		GameObject parent2 = parent;
		if (parent == null)
		{
			parent2 = target.transform.parent.gameObject;
		}
		changeSpriteParent(gameObject, parent2);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = localPosition;
		WMG_Link component = gameObject.GetComponent<WMG_Link>();
		links.Add(gameObject);
		linkAngles.Add(0f);
		WMG_Node component2 = target.GetComponent<WMG_Node>();
		component2.links.Add(gameObject);
		component2.linkAngles.Add(0f);
		component2.numLinks++;
		numLinks++;
		component.Setup(base.gameObject, target, linkId, repos);
		return gameObject;
	}

	public void Reposition(float x, float y)
	{
		changeSpritePositionTo(base.gameObject, new Vector3(x, y, 1f));
		for (int i = 0; i < numLinks; i++)
		{
			WMG_Link component = links[i].GetComponent<WMG_Link>();
			component.Reposition();
		}
	}

	public void SetID(int newID)
	{
		id = newID;
		base.name = "WMG_Node_" + id;
	}

	public void RepositionRelativeToNode(WMG_Node fromNode, bool fixAngle, int degreeStep, float lengthStep)
	{
		float num = base.transform.localPosition.x - fromNode.transform.localPosition.x;
		float num2 = base.transform.localPosition.y - fromNode.transform.localPosition.y;
		float num3 = Mathf.Atan2(num2, num) * 57.29578f;
		if (num3 < 0f)
		{
			num3 += 360f;
		}
		float num4 = Mathf.Sqrt(Mathf.Pow(num2, 2f) + Mathf.Pow(num, 2f));
		if (num4 < 0f)
		{
			num4 = 0f;
		}
		float num5 = num3;
		if (fixAngle)
		{
			num5 = 0f;
			for (int i = 0; i < 360 / degreeStep; i++)
			{
				if (num3 >= (float)(i * degreeStep) - 0.5f * (float)degreeStep && num3 < (float)((i + 1) * degreeStep) - 0.5f * (float)degreeStep)
				{
					num5 = i * degreeStep;
				}
			}
		}
		else
		{
			float num6 = num4 % lengthStep;
			num4 -= num6;
			if (lengthStep - num6 < lengthStep / 2f)
			{
				num4 += lengthStep;
			}
		}
		base.transform.localPosition = new Vector3(fromNode.transform.localPosition.x + num4 * Mathf.Cos((float)Math.PI / 180f * num5), fromNode.transform.localPosition.y + num4 * Mathf.Sin((float)Math.PI / 180f * num5), base.transform.localPosition.z);
		for (int j = 0; j < numLinks; j++)
		{
			WMG_Link component = links[j].GetComponent<WMG_Link>();
			component.Reposition();
		}
	}
}
