using System.Linq;
using Illusion.CustomAttributes;
using UnityEngine;

public class CustomRaycastCtrl : MonoBehaviour
{
	[Button("GetRaycastCtrlComponents", "取得", new object[] { })]
	public int getRaycastCtrlComponents;

	[Button("UpdateRaycastCtrl", "全更新", new object[] { })]
	public int updateRaycastCtrl;

	[SerializeField]
	private UI_RaycastCtrl[] raycastCtrl;

	private void GetRaycastCtrlComponents()
	{
		UI_RaycastCtrl[] componentsInChildren = GetComponentsInChildren<UI_RaycastCtrl>(true);
		raycastCtrl = componentsInChildren.ToArray();
	}

	private void UpdateRaycastCtrl()
	{
		UI_RaycastCtrl[] array = raycastCtrl;
		foreach (UI_RaycastCtrl uI_RaycastCtrl in array)
		{
			uI_RaycastCtrl.Reset();
		}
	}

	private void Reset()
	{
		GetRaycastCtrlComponents();
	}
}
