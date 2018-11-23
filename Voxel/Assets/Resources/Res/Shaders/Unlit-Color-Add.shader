// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Unlit-Color-Add" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Color ("Main Color", Color) = (1,1,1,1)
    _Cutoff ("Color cutoff", Range(0,1)) = 0.5
    _NumAlphaCutoff ("Num Alpha cutoff", Range(0,1)) = 0.5
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
            fixed _NumAlphaCutoff;

            fixed4 _Color;
            //fixed4 _FontColor;

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

                fixed alpha = step(col.a, _Cutoff);

                col.r = saturate(lerp(col.r, _Color.r, alpha));
                col.g = saturate(lerp(col.g, _Color.g, alpha));
                col.b = saturate(lerp(col.b, _Color.b, alpha));
                col.a = saturate(lerp(_NumAlphaCutoff, _Color.a, alpha));
                //col.a = _Color.a;
                return col;
            }
        ENDCG
    }
}

}
