Shader "Shader Forge/create_hair" {
	Properties {
		_Color ("Color", Vector) = (0.5019608,0.5019608,0.5019608,1)
		_MainTex ("MainTex", 2D) = "white" {}
		_Color2 ("Color 2", Vector) = (0,1,0,1)
		_Color3 ("Color 3", Vector) = (1,0,0,1)
		_ColorMask ("Color Mask", 2D) = "black" {}
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