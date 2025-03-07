Shader "Shader Forge/create_body" {
	Properties {
		[HDR] _Color ("Color", Vector) = (0.981,0.8715357,0.8078824,1)
		_MainTex ("MainTex", 2D) = "white" {}
		_Color2 ("Color 2", Vector) = (0.981,0.7104414,0.6116824,1)
		_ColorMask ("Color Mask", 2D) = "black" {}
		_Color4 ("Color 4", Vector) = (1,1,1,1)
		_Texture4 ("Texture 4 (sun)", 2D) = "white" {}
		_Color3 ("Color 3", Vector) = (1,1,1,1)
		_Texture3 ("Texture 3 (paint1)", 2D) = "black" {}
		_Color6 ("Color 6", Vector) = (1,1,1,1)
		_Texture6 ("Texture 6 (paint2)", 2D) = "black" {}
		_Color5 ("Color 5", Vector) = (1,1,1,1)
		_paint1 ("paint1(u/v/r/s)", Vector) = (0,0,0,0.5)
		_paint2 ("paint2(u/v/r/s)", Vector) = (0,0,0,0.5)
		_paintmask ("paint mask", 2D) = "white" {}
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