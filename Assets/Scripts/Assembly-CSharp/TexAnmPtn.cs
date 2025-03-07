using UnityEngine;

public class TexAnmPtn : MonoBehaviour
{
	public int UvNumX = 1;

	public int UvNumY = 1;

	public int ChangeTime = 1000;

	private int passTime;

	private int separateTime;

	private int ptnNum = 1;

	private Vector2 uvSize;

	private Renderer rendererData;

	private int lastIndex = -1;

	private void Start()
	{
		rendererData = GetComponent<Renderer>();
		if (null == rendererData)
		{
			base.enabled = false;
		}
		ptnNum = UvNumX * UvNumY;
		separateTime = ChangeTime / ptnNum;
		uvSize = new Vector2(1f / (float)UvNumX, 1f / (float)UvNumY);
	}

	private void Update()
	{
		passTime += (int)(Time.deltaTime * 1000f);
		while (passTime >= ChangeTime)
		{
			passTime -= ChangeTime;
		}
		int value = ((ChangeTime != 0) ? (passTime % ChangeTime / separateTime) : 0);
		int num = Mathf.Clamp(value, 0, ptnNum);
		if (num != lastIndex)
		{
			int num2 = num % UvNumX;
			int num3 = num / UvNumX;
			Vector2 value2 = new Vector2((float)num2 * uvSize.x, 1f - (float)num3 * uvSize.y);
			rendererData.material.SetTextureOffset("_MainTex", value2);
			rendererData.material.SetTextureScale("_MainTex", uvSize);
			lastIndex = num;
		}
	}
}
