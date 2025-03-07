using System;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class ChaFileMakeup
{
	public Version version { get; set; }

	public int eyeshadowId { get; set; }

	public Color eyeshadowColor { get; set; }

	public int cheekId { get; set; }

	public Color cheekColor { get; set; }

	public int lipId { get; set; }

	public Color lipColor { get; set; }

	public int[] paintId { get; set; }

	public Color[] paintColor { get; set; }

	public Vector4[] paintLayout { get; set; }

	public ChaFileMakeup()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		version = ChaFileDefine.ChaFileMakeupVersion;
		eyeshadowId = 0;
		eyeshadowColor = Color.white;
		cheekId = 0;
		cheekColor = Color.white;
		lipId = 0;
		lipColor = Color.white;
		paintId = new int[2];
		paintColor = new Color[2];
		paintLayout = new Vector4[2];
		for (int i = 0; i < 2; i++)
		{
			paintLayout[i] = new Vector4(0.5f, 0.5f, 0.5f, 0.7f);
		}
	}

	public void ComplementWithVersion()
	{
		version = ChaFileDefine.ChaFileMakeupVersion;
	}
}
