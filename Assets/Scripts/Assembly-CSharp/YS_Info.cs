using System;
using UnityEngine;

public class YS_Info : MonoBehaviour
{
	[Serializable]
	public class Info : YS_Dictionary<string, GameObject, InfoPair>
	{
	}

	[Serializable]
	public class InfoPair : YS_KeyAndValue<string, GameObject>
	{
	}

	public Info data;
}
