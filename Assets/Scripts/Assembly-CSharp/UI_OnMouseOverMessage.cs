using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_OnMouseOverMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public bool active = true;

	public Image imgComment;

	public TextMeshProUGUI txtComment;

	public string comment = string.Empty;

	private void Start()
	{
		if ((bool)imgComment)
		{
			imgComment.enabled = false;
		}
		if ((bool)txtComment)
		{
			txtComment.enabled = false;
		}
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		if (active)
		{
			if ((bool)imgComment)
			{
				imgComment.enabled = true;
			}
			if ((bool)txtComment)
			{
				txtComment.enabled = true;
				txtComment.text = comment;
			}
		}
		else
		{
			if ((bool)imgComment)
			{
				imgComment.enabled = false;
			}
			if ((bool)txtComment)
			{
				txtComment.enabled = false;
			}
		}
	}

	public void OnPointerExit(PointerEventData ped)
	{
		if ((bool)imgComment)
		{
			imgComment.enabled = false;
		}
		if ((bool)txtComment)
		{
			txtComment.enabled = false;
		}
	}
}
