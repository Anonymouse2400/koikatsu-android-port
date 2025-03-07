using UnityEngine;

public class MorphCloneMesh : MonoBehaviour
{
	public static void Clone(out Mesh CloneData, Mesh SorceData)
	{
		CloneData = Object.Instantiate(SorceData);
	}
}
