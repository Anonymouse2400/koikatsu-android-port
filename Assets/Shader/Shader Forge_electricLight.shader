Shader "Shader Forge/electricLight" {
	Properties {
		_Emission ("Emission", 2D) = "white" {}
		_saturation ("saturation", Range(0, 1)) = 1
		_brightness ("brightness", Range(0, 10)) = 1
		_MaskFast ("Mask (Fast)", 2D) = "white" {}
		_MaskSlow ("Mask (Slow)", 2D) = "white" {}
		[MaterialToggle] _TextureChange ("Texture Change", Float) = 0
		_MaskFull ("Mask(Full)", 2D) = "white" {}
		[MaterialToggle] _FullLighting ("Full Lighting", Float) = 0
		_UVOffset ("UV Offset", Float) = 0.0125
		_ScrollSpeed ("Scroll Speed", Float) = 5
		_ColorPhaseSpeed ("Color Phase Speed", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ShaderForgeMaterialInspector"
}