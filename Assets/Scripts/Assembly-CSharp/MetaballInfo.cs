using UnityEngine;

public class MetaballInfo : MonoBehaviour
{
	public Rigidbody[] aRigid;

	public Rigidbody rigidBeginning;

	private void Reset()
	{
		aRigid = GetComponentsInChildren<Rigidbody>(true);
		if (aRigid.Length > 0)
		{
			rigidBeginning = aRigid[0];
		}
	}
}
