// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Unlit-Texture-Mosaic"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MosaicSize("MosaicSize", Range(0.001, 0.1)) = 0.05
		_Grey("Grey", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			//float4 _MainTex_ST;
			
			fixed _MosaicSize;
			fixed _Grey;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				//// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				i.uv.x = ceil(i.uv.x / _MosaicSize)*_MosaicSize;
				i.uv.y = ceil(i.uv.y / _MosaicSize)*_MosaicSize;
				fixed4 col = tex2D(_MainTex, i.uv);

				if (_Grey > 0)
					col = dot(col.rgb, fixed3(0.299, 0.587, 0.144));

				return col;
			}
			ENDCG
		}
	}
}
