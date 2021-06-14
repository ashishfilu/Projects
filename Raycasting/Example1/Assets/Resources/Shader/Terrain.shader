Shader "Unlit/Terrain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };

        float boundsY;
        float normalOffsetWeight;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float h = clamp(0,1,IN.worldPos.y / boundsY);
            float3 tex = tex2D(_MainTex, float2(h,0.5)).rgb;
            o.Albedo = tex;
        }
        ENDCG
    }
}
