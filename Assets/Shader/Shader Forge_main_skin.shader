Shader "Shader Forge/main_skin" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_overcolor1 ("Over Color1", Vector) = (1,1,1,1)
		_overtex1 ("Over Tex1", 2D) = "black" {}
		_overcolor2 ("Over Color2", Vector) = (1,1,1,1)
		_overtex2 ("Over Tex2", 2D) = "black" {}
		_overcolor3 ("Over Color3", Vector) = (1,1,1,1)
		_overtex3 ("Over Tex3", 2D) = "black" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
		_NormalMapDetail ("Normal Map Detail", 2D) = "bump" {}
		_DetailMask ("Detail Mask", 2D) = "black" {}
		_LineMask ("Line Mask", 2D) = "black" {}
		_AlphaMask ("Alpha Mask", 2D) = "white" {}
		_ShadowColor ("Shadow Color", Vector) = (0.628,0.628,0.628,1)
		_SpecularColor ("Specular Color", Vector) = (1,1,1,0)
		_DetailNormalMapScale ("DetailNormalMapScale", Range(0, 1)) = 0
		_SpeclarHeight ("Speclar Height", Range(0, 1)) = 0.98
		_SpecularPower ("Specular Power", Range(0, 1)) = 0
		_SpecularPowerNail ("Specular Power Nail", Range(0, 1)) = 0
		_ShadowExtend ("Shadow Extend", Range(0, 1)) = 1
		_rimpower ("Rim Width", Range(0, 1)) = 0.5
		_rimV ("Rim Strength", Range(0, 1)) = 0
		_nipsize ("nipsize", Range(0, 1)) = 0.5
		[MaterialToggle] _alpha_a ("alpha_a", Float) = 1
		[MaterialToggle] _alpha_b ("alpha_b", Float) = 1
		[MaterialToggle] _linetexon ("Line Tex On", Float) = 1
		[MaterialToggle] _notusetexspecular ("not use tex specular", Float) = 0
		[MaterialToggle] _nip ("nip?", Float) = 0
		_liquidmask ("Liquid Mask", 2D) = "black" {}
		_Texture2 ("Liquid Tex", 2D) = "black" {}
		_Texture3 ("Liquid Normal", 2D) = "bump" {}
		_LiquidTiling ("Liquid Tiling (u/v/us/vs)", Vector) = (0,0,2,2)
		_liquidftop ("liquidftop", Range(0, 2)) = 0
		_liquidfbot ("liquidfbot", Range(0, 2)) = 0
		_liquidbtop ("liquidbtop", Range(0, 2)) = 0
		_liquidbbot ("liquidbbot", Range(0, 2)) = 0
		_liquidface ("liquidface", Range(0, 2)) = 0
		_nip_specular ("nip_specular", Range(0, 1)) = 0.5
		_tex1mask ("tex1 mask(1=yes)", Float) = 0
		_NormalMask ("NormalMask(G)", 2D) = "black" {}
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
	Fallback "Unlit/Texture"
	//CustomEditor "ShaderForgeMaterialInspector"
}