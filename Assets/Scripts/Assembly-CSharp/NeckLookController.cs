using System.IO;
using UnityEngine;

public class NeckLookController : MonoBehaviour
{
	public NeckLookCalc neckLookScript;

	public int ptnNo;

	public Transform target;

	public float rate = 1f;

	private void Start()
	{
		if (!target && (bool)Camera.main)
		{
			target = Camera.main.transform;
		}
	}

	private void Update()
	{
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
			neckLookScript.NeckUpdateCalc(neckLookScript.backupPos, ptnNo);
		}
	}

	public void SaveNeckLookCtrl(BinaryWriter writer)
	{
		writer.Write(ptnNo);
		Quaternion fixAngle = neckLookScript.fixAngle;
		writer.Write(fixAngle.x);
		writer.Write(fixAngle.y);
		writer.Write(fixAngle.z);
		writer.Write(fixAngle.w);
	}

	public void LoadNeckLookCtrl(BinaryReader reader)
	{
		ptnNo = reader.ReadInt32();
		Quaternion fixAngle = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		neckLookScript.SetFixAngle(fixAngle);
	}

	public void SetPtn(int value)
	{
		ptnNo = value;
	}

	public void SetPtnSlide(int ptn1, int ptn2)
	{
		ptnNo = ptn1;
	}
}
