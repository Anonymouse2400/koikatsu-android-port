using System.Linq;
using IllusionUtility.GetUtility;
using UnityEngine;

public class MatAnmFrame : MonoBehaviour
{
	public bool Usage = true;

	public Animation Anm;

	public MatAnmPtnInfo[] PtnInfo;

	private Renderer rendererData;

	private int _Color;

	private void Awake()
	{
		_Color = Shader.PropertyToID("_Color");
	}

	private void Start()
	{
		rendererData = GetComponent<Renderer>();
		if (null == rendererData)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (!Usage)
		{
			return;
		}
		AnimationClip playingClip = Anm.GetPlayingClip();
		int num = -1;
		foreach (var item in PtnInfo.Select((MatAnmPtnInfo value, int index) => new { value, index }))
		{
			if (item.value.PtnName != playingClip.name)
			{
				continue;
			}
			num = item.index;
			break;
		}
		if (num == -1)
		{
			return;
		}
		MatAnmPtnInfo matAnmPtnInfo = PtnInfo[num];
		AnimationState animationState = Anm[playingClip.name];
		float num2 = animationState.time;
		while (playingClip.length < num2)
		{
			num2 -= playingClip.length;
		}
		int num3 = (int)Mathf.Lerp(0f, playingClip.frameRate * playingClip.length, Mathf.InverseLerp(0f, playingClip.length, num2));
		bool flag = false;
		Color32 color = default(Color32);
		for (int i = 0; i < matAnmPtnInfo.Param.Length - 1; i++)
		{
			if (matAnmPtnInfo.Param[i].Frame <= num3 && matAnmPtnInfo.Param[i + 1].Frame >= num3)
			{
				float t = Mathf.InverseLerp(matAnmPtnInfo.Param[i].Frame, matAnmPtnInfo.Param[i + 1].Frame, num3);
				color.r = (byte)Mathf.Lerp((int)matAnmPtnInfo.Param[i].ColorVal.r, (int)matAnmPtnInfo.Param[i + 1].ColorVal.r, t);
				color.g = (byte)Mathf.Lerp((int)matAnmPtnInfo.Param[i].ColorVal.g, (int)matAnmPtnInfo.Param[i + 1].ColorVal.g, t);
				color.b = (byte)Mathf.Lerp((int)matAnmPtnInfo.Param[i].ColorVal.b, (int)matAnmPtnInfo.Param[i + 1].ColorVal.b, t);
				color.a = (byte)Mathf.Lerp((int)matAnmPtnInfo.Param[i].ColorVal.a, (int)matAnmPtnInfo.Param[i + 1].ColorVal.a, t);
				rendererData.material.SetColor(_Color, color);
				flag = true;
				break;
			}
		}
		if (flag)
		{
		}
	}
}
