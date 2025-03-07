using System.Linq;
using IllusionUtility.GetUtility;
using UnityEngine;

public class MatAnmFrameEx : MonoBehaviour
{
	public bool Usage = true;

	public Animation Anm;

	public MatAnmPtnInfoEx[] PtnInfo;

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
		foreach (var item in PtnInfo.Select((MatAnmPtnInfoEx value, int index) => new { value, index }))
		{
			if (item.value.PtnName != playingClip.name)
			{
				continue;
			}
			num = item.index;
			break;
		}
		if (num != -1)
		{
			MatAnmPtnInfoEx matAnmPtnInfoEx = PtnInfo[num];
			AnimationState animationState = Anm[playingClip.name];
			float num2 = animationState.time;
			while (playingClip.length < num2)
			{
				num2 -= playingClip.length;
			}
			float time = Mathf.InverseLerp(0f, playingClip.length, num2);
			Color value2 = default(Color);
			value2.r = matAnmPtnInfoEx.Value.Evaluate(time).r;
			value2.g = matAnmPtnInfoEx.Value.Evaluate(time).g;
			value2.b = matAnmPtnInfoEx.Value.Evaluate(time).b;
			value2.a = matAnmPtnInfoEx.Value.Evaluate(time).a;
			rendererData.material.SetColor(_Color, value2);
		}
	}
}
