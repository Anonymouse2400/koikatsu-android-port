using IllusionUtility.GetUtility;
using IllusionUtility.SetUtility;
using UnityEngine;

public class ChaPVCopyComponent : MonoBehaviour
{
	[SerializeField]
	private bool enable = true;

	[SerializeField]
	private GameObject[] pv;

	[SerializeField]
	private GameObject[] bone;

	private void Start()
	{
	}

	private void Reset()
	{
		string[] array = new string[4] { "cf_pv_hand_L", "cf_pv_hand_R", "cf_pv_leg_L", "cf_pv_leg_R" };
		pv = new GameObject[4];
		for (int i = 0; i < array.Length; i++)
		{
			pv[i] = base.transform.FindLoop(array[i]);
		}
		string[] array2 = new string[4] { "cf_j_hand_L", "cf_j_hand_R", "cf_j_leg03_L", "cf_j_leg03_R" };
		bone = new GameObject[4];
		for (int j = 0; j < array2.Length; j++)
		{
			bone[j] = base.transform.FindLoop(array2[j]);
		}
	}

	private void LateUpdate()
	{
		if (enable && pv != null && bone != null && pv.Length == bone.Length)
		{
			for (int i = 0; i < pv.Length; i++)
			{
				bone[i].transform.CopyPosRotScl(pv[i].transform);
			}
		}
	}
}
