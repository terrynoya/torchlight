

/**
	A Alpha blend shader(Vertex lighting) that can receive shadow from main direction light
*/
Shader "Hidden/TorchLight/Alpha/AlphaShadowed" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader {
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="False" "RenderType"="TransparentCutout" "LightMode" = "ForwardBase" }
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma surface surf Lambert alpha fullforwardshadows approxview
	
		sampler2D _MainTex;
		fixed4 _Color;
	
		struct Input {
			float2 uv_MainTex;
		};
	
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	//Fallback "Transparent/VertexLit"
	Fallback "TorchLight/Alpha/AlphaTest-VertexLit"
}
