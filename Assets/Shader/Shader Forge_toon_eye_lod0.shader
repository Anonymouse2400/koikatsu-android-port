Shader "Shader Forge/toon_eye_lod0" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_overcolor1 ("overcolor1", Vector) = (1,1,1,1)
		_overtex1 ("overtex1", 2D) = "black" {}
		_overcolor2 ("overcolor2", Vector) = (1,1,1,1)
		_overtex2 ("overtex2", 2D) = "black" {}
		[MaterialToggle] _isHighLight ("isHighLight", Float) = 0
		_expression ("expression", 2D) = "black" {}
		_exppower ("exppower", Range(0, 1)) = 1
		_shadowcolor ("shadowcolor", Vector) = (0.6298235,0.6403289,0.747,1)
		_rotation ("rotation", Range(0, 1)) = 0
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
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ShaderForgeMaterialInspector"
}