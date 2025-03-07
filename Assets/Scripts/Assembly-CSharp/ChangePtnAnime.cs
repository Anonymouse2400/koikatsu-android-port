using UnityEngine;

public class ChangePtnAnime : MonoBehaviour
{
	public Material[] MatChange;

	public Texture[] TexChange;

	public int ChangeTime = 200;

	private int indexT;

	private int indexM;

	private void Start()
	{
	}

	public void Init(Texture[] tex)
	{
		TexChange = new Texture[tex.Length];
		for (int i = 0; i < tex.Length; i++)
		{
			TexChange[i] = tex[i];
		}
	}

	private void Update()
	{
		if (!GetComponent<Renderer>())
		{
			return;
		}
		int num = (int)((long)((double)Time.timeSinceLevelLoad * 1000.0) % 1000000) / ChangeTime;
		if (MatChange != null && MatChange.Length != 0)
		{
			int num2 = num % MatChange.Length;
			if (indexM != num2)
			{
				indexM = num2;
				GetComponent<Renderer>().sharedMaterial = MatChange[indexM];
			}
		}
		if (TexChange != null && TexChange.Length != 0)
		{
			int num3 = num % TexChange.Length;
			if (indexT != num3)
			{
				indexT = num3;
				GetComponent<Renderer>().material.mainTexture = TexChange[indexT];
			}
		}
	}
}
