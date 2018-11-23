// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "TSHD/GreyDiffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (0.6,0.6,0.6,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 150

	CGPROGRAM
		#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			float grey = dot(c.rgb, _Color.rgb);
			//float grey = dot(c.rgb, float3(0.299, 0.587, 0.114));
			o.Albedo = grey;
			o.Alpha = c.a;
		}
	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
