Shader "Custom/MainCubeHighlight"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Gloss ("Gloss Strength", Range(0,1)) = 0.4
        _SpecularPower ("Specular Power", Range(1,128)) = 32
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 200

        Pass
        {
            Name "Main"
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BaseColor;
            float _Gloss;
            float _SpecularPower;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldN = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                float3 worldP = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.worldNormal = worldN;
                o.worldPos = worldP;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Fõ szín
                float3 base = _BaseColor.rgb;

                // Fény iránya
                float3 lightDir = normalize(float3(0.4, 1, 0.3));

                // Normál dot fény
                float NdotL = saturate(dot(i.worldNormal, lightDir));

                // Diffuse
                float3 diffuse = base * NdotL;

                // Specular Highlight
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 halfVector = normalize(lightDir + viewDir);

                float spec = pow(saturate(dot(i.worldNormal, halfVector)), _SpecularPower) * _Gloss;

                float3 finalCol = base + diffuse + spec;

                return float4(finalCol, 1);
            }
            ENDCG
        }
    }
}