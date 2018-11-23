// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Unlit-Texture-Transparent" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

    _BgColor ("Bg Color", Color) = (1,1,1,1)
}
SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Lighting Off

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog


            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                //UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                //UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            fixed4 _BgColor;
            fixed4 _FontColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                //UNITY_SETUP_INSTANCE_ID(v);
                //UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);

                fixed alpha = step(col.a, 0.5);

                col.r = saturate(lerp(col.r, _BgColor.r, alpha));
                col.g = saturate(lerp(col.g, _BgColor.g, alpha));
                col.b = saturate(lerp(col.b, _BgColor.b, alpha));

                col.a = saturate(lerp(_Cutoff, _BgColor.a, alpha));



                return col;
            }
        ENDCG
    }
}

}
