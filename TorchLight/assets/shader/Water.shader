Shader "TorchLight/Water" {
	Properties {
		_Brightness ("Brightness", Range(0, 2)) = 1.0
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 2)) = 1.0
	}

	SubShader {
		Tags {"Queue"="AlphaTest" "RenderType"="Transparent" }

		Blend OneMinusSrcAlpha SrcAlpha
		//AlphaTest Greater .01
		//ColorMask RGB
		//Cull Off 
		//Lighting Off 
		//ZWrite Off

		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		fixed _Alpha;
		fixed _Brightness;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Brightness;
			o.Albedo = c.rgb;
			o.Alpha = _Alpha;
		}
		ENDCG
	} 
	FallBack "TorchLight/Alpha/Alpha"
}
