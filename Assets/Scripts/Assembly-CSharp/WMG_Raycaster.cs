using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WMG_Raycaster : GraphicRaycaster
{
	public float AlphaThreshold = 0.9f;

	public bool IncludeMaterialAlpha = true;

	private List<RaycastResult> exclusions = new List<RaycastResult>();

	protected override void OnEnable()
	{
		base.OnEnable();
		GraphicRaycaster component = GetComponent<GraphicRaycaster>();
		if (component != null && component != this)
		{
			Object.DestroyImmediate(component);
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		base.Raycast(eventData, resultAppendList);
		exclusions.Clear();
		foreach (RaycastResult resultAppend in resultAppendList)
		{
			Image component = resultAppend.gameObject.GetComponent<Image>();
			if (component == null)
			{
				continue;
			}
			WMG_Raycatcher component2 = resultAppend.gameObject.GetComponent<WMG_Raycatcher>();
			if (component2 == null)
			{
				continue;
			}
			try
			{
				RectTransform rectTransform = resultAppend.gameObject.transform as RectTransform;
				Vector3 position;
				if ((bool)eventCamera)
				{
					Plane plane = new Plane(rectTransform.forward, rectTransform.position);
					Ray ray = eventCamera.ScreenPointToRay(eventData.position);
					float enter;
					plane.Raycast(ray, out enter);
					position = ray.GetPoint(enter);
				}
				else
				{
					position = eventData.position;
					float num = ((0f - rectTransform.forward.x) * (position.x - rectTransform.position.x) - rectTransform.forward.y * (position.y - rectTransform.position.y)) / rectTransform.forward.z;
					position += new Vector3(0f, 0f, rectTransform.position.z + num);
				}
				Vector3 vector = rectTransform.InverseTransformPoint(position);
				Texture2D texture2D = component.mainTexture as Texture2D;
				Rect textureRect = component.sprite.textureRect;
				float num2 = vector.x * (textureRect.width / rectTransform.sizeDelta.x) + textureRect.width * rectTransform.pivot.x;
				float num3 = vector.y * (textureRect.height / rectTransform.sizeDelta.y) + textureRect.height * rectTransform.pivot.y;
				float num4 = texture2D.GetPixel((int)(num2 + textureRect.x), (int)(num3 + textureRect.y)).a;
				if (component.type == Image.Type.Filled)
				{
					float num5 = Mathf.Atan2(num3 / textureRect.height - 0.5f, num2 / textureRect.width - 0.5f) * 57.29578f;
					if (num5 < 0f)
					{
						num5 += 360f;
					}
					bool flag = true;
					if (component.fillMethod == Image.FillMethod.Radial360)
					{
						float num6 = component.fillAmount * 360f;
						if (component.fillOrigin == 2)
						{
							num5 -= 90f;
						}
						else if (component.fillOrigin == 3)
						{
							num5 -= 180f;
						}
						else if (component.fillOrigin == 0)
						{
							num5 -= 270f;
						}
						if (num5 < 0f)
						{
							num5 += 360f;
						}
						if (component.fillClockwise)
						{
							if (num5 > 0f - num6 + 360f)
							{
								flag = false;
							}
						}
						else if (num5 < num6)
						{
							flag = false;
						}
					}
					if (flag)
					{
						num4 = 0f;
					}
				}
				if (IncludeMaterialAlpha)
				{
					num4 *= component.color.a;
				}
				if (num4 < AlphaThreshold)
				{
					exclusions.Add(resultAppend);
				}
			}
			catch (UnityException)
			{
				if (!Application.isEditor)
				{
				}
			}
		}
		resultAppendList.RemoveAll((RaycastResult res) => exclusions.Contains(res));
	}
}
