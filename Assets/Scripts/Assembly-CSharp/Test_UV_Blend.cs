using UnityEngine;

public class Test_UV_Blend : MonoBehaviour
{
	public UVData uvdata;

	public Transform trfUV;

	[Range(0f, 1f)]
	public float value = 0.5f;

	private float backValue = -1f;

	private bool InitEnd;

	private SkinnedMeshRenderer smr;

	public void Init()
	{
		if (!(null == uvdata) && !(null == trfUV) && uvdata.data.Count == 3)
		{
			smr = trfUV.GetComponent<SkinnedMeshRenderer>();
			if (!(null == smr) && smr.sharedMesh.uv.Length == uvdata.data[0].UV.Count)
			{
				InitEnd = true;
			}
		}
	}

	public bool Blend()
	{
		if (!InitEnd)
		{
			return false;
		}
		int num = 0;
		int num2 = 1;
		float num3 = 0f;
		if (value < 0.5f)
		{
			num = 0;
			num2 = 1;
			num3 = Mathf.InverseLerp(0f, 0.5f, value);
		}
		else
		{
			num = 1;
			num2 = 2;
			num3 = Mathf.InverseLerp(0.5f, 1f, value);
		}
		Vector2[] array = new Vector2[smr.sharedMesh.uv.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Vector2.Lerp(uvdata.data[num].UV[i], uvdata.data[num2].UV[i], num3);
		}
		smr.sharedMesh.uv = array;
		backValue = value;
		return true;
	}

	public void Start()
	{
		Init();
	}

	public void Update()
	{
		if (value != backValue)
		{
			Blend();
		}
	}

	public void ValueChange(float value)
	{
		this.value = value;
	}
}
