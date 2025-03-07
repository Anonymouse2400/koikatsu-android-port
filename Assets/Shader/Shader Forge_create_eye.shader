Shader "Shader Forge/create_eye" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_ColorMask ("Color Mask", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_Color2 ("Color 2", Vector) = (0.3382353,0.780933,1,1)
		_Blend ("Blend(Multiply>Vivid)", Range(0, 1)) = 0
		_grad ("grad(uv-s)", Vector) = (0,0,0,0)
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ShaderForgeMaterialInspector"
}