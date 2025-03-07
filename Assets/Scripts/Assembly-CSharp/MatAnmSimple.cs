using UnityEngine;

public class MatAnmSimple : MonoBehaviour
{
	public MatAnmSimpleInfo[] ptn;

	private int _Color;

	private void Awake()
	{
		_Color = Shader.PropertyToID("_Color");
	}

	private void Start()
	{
	}

	private void Update()
	{
		MatAnmSimpleInfo[] array = ptn;
		foreach (MatAnmSimpleInfo matAnmSimpleInfo in array)
		{
			if (!(null == matAnmSimpleInfo.mr))
			{
				matAnmSimpleInfo.Update(_Color);
			}
		}
	}
}
