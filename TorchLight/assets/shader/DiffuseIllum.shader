// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "TorchLight/Diffuse-Illum" {
	Properties {
		//_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IllumTex ("Illum Texture", 2D) = "black" {}
		_IllumFactor ("Illum Factor", Range(0.5,3.0)) = 1.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 150
	
	CGPROGRAM
	#pragma surface surf Lambert
	
	sampler2D _MainTex;
	sampler2D _IllumTex;
	fixed _IllumFactor;
	//fixed4 _Color;
	
	struct Input {
		float2 uv_MainTex;
		float2 uv_IllumTex;
	};
	
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// * _Color;
		fixed4 ic = tex2D(_IllumTex, IN.uv_IllumTex) * _IllumFactor;
		o.Albedo = c.rgb + ic.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
	
	Fallback "Diffuse"
}
