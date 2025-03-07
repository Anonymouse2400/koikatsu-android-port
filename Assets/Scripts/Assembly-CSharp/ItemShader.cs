using UnityEngine;

[DefaultExecutionOrder(-5)]
public static class ItemShader
{
	public static int _Color { get; private set; }

	public static int _Color2 { get; private set; }

	public static int _Color3 { get; private set; }

	public static int _Color4 { get; private set; }

	public static int _ShadowColor { get; private set; }

	public static int _PatternMask1 { get; private set; }

	public static int _PatternMask2 { get; private set; }

	public static int _PatternMask3 { get; private set; }

	public static int _Color1_2 { get; private set; }

	public static int _Color2_2 { get; private set; }

	public static int _Color3_2 { get; private set; }

	public static int _Patternuv1 { get; private set; }

	public static int _Patternuv2 { get; private set; }

	public static int _Patternuv3 { get; private set; }

	public static int _patternrotator1 { get; private set; }

	public static int _patternrotator2 { get; private set; }

	public static int _patternrotator3 { get; private set; }

	public static int _patternclamp1 { get; private set; }

	public static int _patternclamp2 { get; private set; }

	public static int _patternclamp3 { get; private set; }

	public static int _alpha { get; private set; }

	public static int _LineColor { get; private set; }

	public static int _LineWidthS { get; private set; }

	public static int _EmissionColor { get; private set; }

	public static int _EmissionPower { get; private set; }

	public static int _LightCancel { get; private set; }

	public static int _MainTex { get; private set; }

	public static int _FaceNormalG { get; private set; }

	public static int _FaceShadowG { get; private set; }

	static ItemShader()
	{
		_Color = Shader.PropertyToID("_Color");
		_Color2 = Shader.PropertyToID("_Color2");
		_Color3 = Shader.PropertyToID("_Color3");
		_Color4 = Shader.PropertyToID("_Color4");
		_ShadowColor = Shader.PropertyToID("_ShadowColor");
		_PatternMask1 = Shader.PropertyToID("_PatternMask1");
		_PatternMask2 = Shader.PropertyToID("_PatternMask2");
		_PatternMask3 = Shader.PropertyToID("_PatternMask3");
		_Color1_2 = Shader.PropertyToID("_Color1_2");
		_Color2_2 = Shader.PropertyToID("_Color2_2");
		_Color3_2 = Shader.PropertyToID("_Color3_2");
		_Patternuv1 = Shader.PropertyToID("_Patternuv1");
		_Patternuv2 = Shader.PropertyToID("_Patternuv2");
		_Patternuv3 = Shader.PropertyToID("_Patternuv3");
		_patternrotator1 = Shader.PropertyToID("_patternrotator1");
		_patternrotator2 = Shader.PropertyToID("_patternrotator2");
		_patternrotator3 = Shader.PropertyToID("_patternrotator3");
		_patternclamp1 = Shader.PropertyToID("_patternclamp1");
		_patternclamp2 = Shader.PropertyToID("_patternclamp2");
		_patternclamp3 = Shader.PropertyToID("_patternclamp3");
		_alpha = Shader.PropertyToID("_alpha");
		_LineColor = Shader.PropertyToID("_LineColor");
		_LineWidthS = Shader.PropertyToID("_LineWidthS");
		_EmissionColor = Shader.PropertyToID("_EmissionColor");
		_EmissionPower = Shader.PropertyToID("_EmissionPower");
		_LightCancel = Shader.PropertyToID("_LightCancel");
		_MainTex = Shader.PropertyToID("_MainTex");
		_FaceNormalG = Shader.PropertyToID("_FaceNormalG");
		_FaceShadowG = Shader.PropertyToID("_FaceShadowG");
	}
}
