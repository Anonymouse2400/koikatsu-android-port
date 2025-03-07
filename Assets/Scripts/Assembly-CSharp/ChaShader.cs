using UnityEngine;

[DefaultExecutionOrder(-5)]
public static class ChaShader
{
	public static int _MainTex { get; private set; }

	public static int _Texture2 { get; private set; }

	public static int _Texture3 { get; private set; }

	public static int _Texture4 { get; private set; }

	public static int _Texture5 { get; private set; }

	public static int _Texture6 { get; private set; }

	public static int _Texture7 { get; private set; }

	public static int _Color { get; private set; }

	public static int _Color1_2 { get; private set; }

	public static int _Color2 { get; private set; }

	public static int _Color2_2 { get; private set; }

	public static int _Color3 { get; private set; }

	public static int _Color3_2 { get; private set; }

	public static int _Color4 { get; private set; }

	public static int _Color4_2 { get; private set; }

	public static int _Color5 { get; private set; }

	public static int _Color6 { get; private set; }

	public static int _Color7 { get; private set; }

	public static int _ColorMask { get; private set; }

	public static int _AlphaMask { get; private set; }

	public static int _exppower { get; private set; }

	public static int _expression { get; private set; }

	public static int _isHighLight { get; private set; }

	public static int _DetailMask { get; private set; }

	public static int _NormalMap { get; private set; }

	public static int _overtex1 { get; private set; }

	public static int _overtex2 { get; private set; }

	public static int _overtex3 { get; private set; }

	public static int _overcolor1 { get; private set; }

	public static int _overcolor2 { get; private set; }

	public static int _overcolor3 { get; private set; }

	public static int _reverse { get; private set; }

	public static int _rotation { get; private set; }

	public static int _alpha_a { get; private set; }

	public static int _alpha_b { get; private set; }

	public static int _paint1 { get; private set; }

	public static int _paint2 { get; private set; }

	public static int _hokuro { get; private set; }

	public static int _Blend { get; private set; }

	public static int _grad { get; private set; }

	public static int _nipsize { get; private set; }

	public static int _linetexon { get; private set; }

	public static int _NormalMapDetail { get; private set; }

	public static int _LineMask { get; private set; }

	public static int _DetailNormalMapScale { get; private set; }

	public static int _SpecularPower { get; private set; }

	public static int _SpecularPowerNail { get; private set; }

	public static int _liquidftop { get; private set; }

	public static int _liquidfbot { get; private set; }

	public static int _liquidbtop { get; private set; }

	public static int _liquidbbot { get; private set; }

	public static int _liquidface { get; private set; }

	public static int _PatternScale1u { get; private set; }

	public static int _PatternScale1v { get; private set; }

	public static int _PatternMask1 { get; private set; }

	public static int _PatternScale2u { get; private set; }

	public static int _PatternScale2v { get; private set; }

	public static int _PatternMask2 { get; private set; }

	public static int _PatternScale3u { get; private set; }

	public static int _PatternScale3v { get; private set; }

	public static int _PatternMask3 { get; private set; }

	public static int _PatternScale4u { get; private set; }

	public static int _PatternScale4v { get; private set; }

	public static int _PatternMask4 { get; private set; }

	public static int _TileAnimation { get; private set; }

	public static int _SizeSpeed { get; private set; }

	public static int _SizeWidth { get; private set; }

	public static int _angleSpeed { get; private set; }

	public static int _yurayura { get; private set; }

	public static int _HairGloss { get; private set; }

	public static int _LineColor { get; private set; }

	public static int _nip_specular { get; private set; }

	public static Texture texRamp { get; private set; }

	public static int _RampG { get; private set; }

	public static int _rimG { get; private set; }

	public static int _ambientshadowG { get; private set; }

	public static int _LineColorG { get; private set; }

	public static int _linewidthG { get; private set; }

	static ChaShader()
	{
		_MainTex = Shader.PropertyToID("_MainTex");
		_Texture2 = Shader.PropertyToID("_Texture2");
		_Texture3 = Shader.PropertyToID("_Texture3");
		_Texture4 = Shader.PropertyToID("_Texture4");
		_Texture5 = Shader.PropertyToID("_Texture5");
		_Texture6 = Shader.PropertyToID("_Texture6");
		_Texture7 = Shader.PropertyToID("_Texture7");
		_Color = Shader.PropertyToID("_Color");
		_Color1_2 = Shader.PropertyToID("_Color1_2");
		_Color2 = Shader.PropertyToID("_Color2");
		_Color2_2 = Shader.PropertyToID("_Color2_2");
		_Color3 = Shader.PropertyToID("_Color3");
		_Color3_2 = Shader.PropertyToID("_Color3_2");
		_Color4 = Shader.PropertyToID("_Color4");
		_Color4_2 = Shader.PropertyToID("_Color4_2");
		_Color5 = Shader.PropertyToID("_Color5");
		_Color6 = Shader.PropertyToID("_Color6");
		_Color7 = Shader.PropertyToID("_Color7");
		_ColorMask = Shader.PropertyToID("_ColorMask");
		_AlphaMask = Shader.PropertyToID("_AlphaMask");
		_expression = Shader.PropertyToID("_expression");
		_exppower = Shader.PropertyToID("_exppower");
		_isHighLight = Shader.PropertyToID("_isHighLight");
		_DetailMask = Shader.PropertyToID("_DetailMask");
		_NormalMap = Shader.PropertyToID("_NormalMap");
		_overtex1 = Shader.PropertyToID("_overtex1");
		_overtex2 = Shader.PropertyToID("_overtex2");
		_overtex3 = Shader.PropertyToID("_overtex3");
		_overcolor1 = Shader.PropertyToID("_overcolor1");
		_overcolor2 = Shader.PropertyToID("_overcolor2");
		_overcolor3 = Shader.PropertyToID("_overcolor3");
		_reverse = Shader.PropertyToID("_reverse");
		_rotation = Shader.PropertyToID("_rotation");
		_alpha_a = Shader.PropertyToID("_alpha_a");
		_alpha_b = Shader.PropertyToID("_alpha_b");
		_paint1 = Shader.PropertyToID("_paint1");
		_paint2 = Shader.PropertyToID("_paint2");
		_hokuro = Shader.PropertyToID("_hokuro");
		_Blend = Shader.PropertyToID("_Blend");
		_grad = Shader.PropertyToID("_grad");
		_nipsize = Shader.PropertyToID("_nipsize");
		_linetexon = Shader.PropertyToID("_linetexon");
		_NormalMapDetail = Shader.PropertyToID("_NormalMapDetail");
		_LineMask = Shader.PropertyToID("_LineMask");
		_DetailNormalMapScale = Shader.PropertyToID("_DetailNormalMapScale");
		_SpecularPower = Shader.PropertyToID("_SpecularPower");
		_SpecularPowerNail = Shader.PropertyToID("_SpecularPowerNail");
		_liquidftop = Shader.PropertyToID("_liquidftop");
		_liquidfbot = Shader.PropertyToID("_liquidfbot");
		_liquidbtop = Shader.PropertyToID("_liquidbtop");
		_liquidbbot = Shader.PropertyToID("_liquidbbot");
		_liquidface = Shader.PropertyToID("_liquidface");
		_PatternScale1u = Shader.PropertyToID("_PatternScale1u");
		_PatternScale1v = Shader.PropertyToID("_PatternScale1v");
		_PatternMask1 = Shader.PropertyToID("_PatternMask1");
		_PatternScale2u = Shader.PropertyToID("_PatternScale2u");
		_PatternScale2v = Shader.PropertyToID("_PatternScale2v");
		_PatternMask2 = Shader.PropertyToID("_PatternMask2");
		_PatternScale3u = Shader.PropertyToID("_PatternScale3u");
		_PatternScale3v = Shader.PropertyToID("_PatternScale3v");
		_PatternMask3 = Shader.PropertyToID("_PatternMask3");
		_PatternScale4u = Shader.PropertyToID("_PatternScale4u");
		_PatternScale4v = Shader.PropertyToID("_PatternScale4v");
		_PatternMask4 = Shader.PropertyToID("_PatternMask4");
		_TileAnimation = Shader.PropertyToID("_TileAnimation");
		_SizeSpeed = Shader.PropertyToID("_SizeSpeed");
		_SizeWidth = Shader.PropertyToID("_SizeWidth");
		_angleSpeed = Shader.PropertyToID("_AngleSpeed");
		_yurayura = Shader.PropertyToID("_yurayura");
		_HairGloss = Shader.PropertyToID("_HairGloss");
		_LineColor = Shader.PropertyToID("_LineColor");
		_nip_specular = Shader.PropertyToID("_nip_specular");
		texRamp = null;
		_RampG = Shader.PropertyToID("_RampG");
		_ambientshadowG = Shader.PropertyToID("_ambientshadowG");
		_LineColorG = Shader.PropertyToID("_LineColorG");
		_linewidthG = Shader.PropertyToID("_linewidthG");
		_rimG = Shader.PropertyToID("_rimG");
	}

	public static void ChangeRampTexture(Texture tex)
	{
		texRamp = tex;
		Shader.SetGlobalTexture(_RampG, texRamp);
	}

	public static void ChangeAmbientShaodwColor(float depth)
	{
		Color value = new Color(0.019f, 0.027f, 0.039f);
		value.a = depth;
		Shader.SetGlobalColor(_ambientshadowG, value);
	}

	public static void ChangeLineColor(float depth)
	{
		Shader.SetGlobalColor(value: new Color(0.5f, 0.5f, 0.5f, 1f - depth), nameID: _LineColorG);
	}

	public static void ChangeLineWidth(float width)
	{
		Shader.SetGlobalFloat(_linewidthG, width);
	}
}
