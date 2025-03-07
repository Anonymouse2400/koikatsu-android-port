Shader "Shader Forge/volumeLight" {
	Properties {
		_color ("color", 2D) = "gray" {}
		_saturation ("saturation", Range(0, 1)) = 1
		_brightness ("brightness", Range(0, 10)) = 1
		_mask ("mask", 2D) = "gray" {}
		_multiplier ("multiplier", Float) = 1
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