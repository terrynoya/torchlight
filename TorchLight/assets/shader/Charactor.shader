Shader "TorchLight/Charactor" {
	Properties {
		_MainTex ("Body Texture", 2D)	= "white" {}
		_EquipTex ("Equip Texture", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
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
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
