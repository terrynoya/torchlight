// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'

Shader "TorchLight/Charactor" {
	Properties {
		_XRay ("Main Color", Color)	= (1,0,1,1)
		_MainTex ("Body Texture", 2D)	= "white" {}
		_EquipTex ("Equip Texture", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry+1" }
		LOD 200

		// X-Ray Effect Pass
		Pass {
			ZTest Greater 
			ZWrite Off
        	Blend One OneMinusDstColor
			Color [_XRay]
        }
		// End X-Ray
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _EquipTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EquipTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c		= tex2D (_MainTex, IN.uv_MainTex);
			half4 ec	= tex2D (_EquipTex, IN.uv_EquipTex);
			o.Albedo	= c.rgb * (1.0f - ec.a) + ec.rgb * ec.a;
			o.Alpha		= c.a;

			o.Emission	= half3(0.05, 0.05, 0.05);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
