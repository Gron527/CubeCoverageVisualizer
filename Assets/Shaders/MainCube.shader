Shader "Custom/OutlineBuiltIn"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,0.2)       // opacity támogatva!
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _Outline ("Outline Width", Range(0.0, 0.1)) = 0.03
    }

    SubShader
    {
        // ÁTLÁTSZÓ!!!
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        // --- MAIN PASS ---
        Pass
        {
            Name "Main"
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _Color;   // alpha is mûködni fog
            }
            ENDCG
        }

        // --- OUTLINE PASS ---
        Pass
        {
            Name "Outline"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Outline;
            float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // KISEBB displacement  nincs szétesés!
                float3 norm = normalize(v.normal) * (_Outline * 0.2);
                float4 pos = v.vertex + float4(norm, 0);
                o.pos = UnityObjectToClipPos(pos);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}