using System.Collections.Generic;
using Illusion.CustomAttributes;
using UnityEngine;

public class ChaForegroundComponent : MonoBehaviour
{
	[Header("眉のCustomRenderQueue")]
	public List<SetRenderQueue_Custom> lstSrqcEyebrow;

	[Header("目のCustomRenderQueue")]
	public List<SetRenderQueue_Custom> lstSrqcEyes;

	[Button("Initialize", "設定の初期化", new object[] { })]
	[Space]
	public int initialize;

	public void Initialize()
	{
		string objName = "cf_O_mayuge";
		string[] array = new string[9] { "cf_O_eyeline", "cf_O_eyeline_low", "cf_Ohitomi_L", "cf_Ohitomi_R", "cf_Ohitomi_L02", "cf_Ohitomi_R02", "cf_O_gag_eye_00", "cf_O_gag_eye_01", "cf_O_gag_eye_02" };
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(base.transform);
		if (lstSrqcEyebrow == null)
		{
			lstSrqcEyebrow = new List<SetRenderQueue_Custom>();
		}
		lstSrqcEyebrow.Clear();
		GameObject objectFromName = findAssist.GetObjectFromName(objName);
		if ((bool)objectFromName)
		{
			SetRenderQueue_Custom component = objectFromName.GetComponent<SetRenderQueue_Custom>();
			if ((bool)component)
			{
				lstSrqcEyebrow.Add(component);
			}
		}
		if (lstSrqcEyes == null)
		{
			lstSrqcEyes = new List<SetRenderQueue_Custom>();
		}
		lstSrqcEyes.Clear();
		string[] array2 = array;
		foreach (string objName2 in array2)
		{
			GameObject objectFromName2 = findAssist.GetObjectFromName(objName2);
			if (!(null == objectFromName2))
			{
				SetRenderQueue_Custom component2 = objectFromName2.GetComponent<SetRenderQueue_Custom>();
				if ((bool)component2)
				{
					lstSrqcEyes.Add(component2);
				}
			}
		}
	}

	private void Reset()
	{
		Initialize();
	}

	public void SetForeground(bool eyes, bool eyebrow)
	{
		if (lstSrqcEyebrow != null)
		{
			for (int i = 0; i < lstSrqcEyebrow.Count; i++)
			{
				lstSrqcEyebrow[i].isSetQueue = eyebrow;
			}
		}
		if (lstSrqcEyes != null)
		{
			for (int j = 0; j < lstSrqcEyes.Count; j++)
			{
				lstSrqcEyes[j].isSetQueue = eyes;
			}
		}
	}
}
