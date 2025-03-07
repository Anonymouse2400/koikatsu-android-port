using UnityEngine;

public class bl_CreateItemExample : MonoBehaviour
{
	private bl_MMExampleManager exampler;

	private bl_MiniMap MiniMap;

	private void Start()
	{
		exampler = GetComponent<bl_MMExampleManager>();
		MiniMap = exampler.GetActiveMiniMap;
	}

	private void Update()
	{
		if (!(MiniMap == null))
		{
			if (Input.GetButtonDown("Fire1"))
			{
				CreateItem();
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				MiniMap.DoHitEffect();
			}
		}
	}

	private void CreateItem()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			bl_MMItemInfo item = new bl_MMItemInfo(hitInfo.point);
			MiniMap.CreateNewItem(item);
		}
	}
}
