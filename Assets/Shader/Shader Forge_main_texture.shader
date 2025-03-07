Shader "Shader Forge/main_texture" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_DetailMask ("Detail Mask", 2D) = "black" {}
		_LineMask ("Line Mask", 2D) = "black" {}
		_ShadowColor ("Shadow Color", Vector) = (0.628,0.628,0.628,1)
		_SpeclarHeight ("Speclar Height", Range(0, 1)) = 0.98
		_ShadowExtend ("Shadow Extend", Range(0, 1)) = 1
		_ShadowExtendAnother ("Shadow Extend Another", Range(0, 1)) = 1
		[MaterialToggle] _AnotherRampFull ("Another Ramp Full", Float) = 0
		[MaterialToggle] _DetailBLineG ("DetailB LineG", Float) = 0
		[MaterialToggle] _DetailRLineR ("DetailR LineR", Float) = 0
		_ColorMask ("Color Mask", 2D) = "black" {}
		_Color ("Color", Vector) = (1,0,0,1)
		_Color2 ("Color2", Vector) = (0.1172419,0,1,1)
		_Color3 ("Color3", Vector) = (0.5,0.5,0.5,1)
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
	Fallback "Unlit/Texture"
	//CustomEditor "ShaderForgeMaterialInspector"
}