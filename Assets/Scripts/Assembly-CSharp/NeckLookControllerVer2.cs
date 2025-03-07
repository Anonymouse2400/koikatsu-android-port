using System.IO;
using UnityEngine;

[RequireComponent(typeof(NeckLookCalcVer2))]
public class NeckLookControllerVer2 : MonoBehaviour
{
	public NeckLookCalcVer2 neckLookScript;

	public int ptnNo;

	public Transform target;

	public float rate = 1f;

	public void SaveNeckLookCtrl(BinaryWriter writer)
	{
		writer.Write(ptnNo);
		int num = neckLookScript.aBones.Length;
		writer.Write(num);
		for (int i = 0; i < num; i++)
		{
			Quaternion fixAngle = neckLookScript.aBones[i].fixAngle;
			writer.Write(fixAngle.x);
			writer.Write(fixAngle.y);
			writer.Write(fixAngle.z);
			writer.Write(fixAngle.w);
		}
	}

	public void LoadNeckLookCtrl(BinaryReader reader)
	{
		ptnNo = reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Quaternion quaternion = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			neckLookScript.aBones[i].fixAngleBackup = (neckLookScript.aBones[i].fixAngle = quaternion);
			if (neckLookScript.aBones[i].neckBone != null)
			{
				neckLookScript.aBones[i].neckBone.localRotation = quaternion;
			}
			if (neckLookScript.aBones[i].controlBone != null)
			{
				neckLookScript.aBones[i].controlBone.localRotation = quaternion;
			}
		}
	}

	private void Start()
	{
		if (!target && (bool)Camera.main)
		{
			target = Camera.main.transform;
		}
	}

	private void LateUpdate()
	{
		if (neckLookScript == null)
		{
			return;
		}
		neckLookScript.UpdateCall(ptnNo);
		if (target != null)
		{
			if (null != neckLookScript)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = target.position;
				for (int i = 0; i < 2; i++)
				{
					position2[i] = Mathf.Lerp(position[i], position2[i], rate);
				}
				neckLookScript.NeckUpdateCalc(position2, ptnNo);
			}
		}
		else
		{
			neckLookScript.NeckUpdateCalc(Vector3.zero, ptnNo, true);
		}
	}

	public void ForceLateUpdate()
	{
		LateUpdate();
	}
}
