Shader "Udemy/NormalMap"
{
    Properties
    {
        _Albedo( "Color texture",2D) = "white"{}
        _Normal( "Normal texture",2D) = "bump"{}
        _BumpStrength("Strength" , Range(0,10))=1
    }
    
    SubShader
    {
        CGPROGRAM
            #pragma surface surf Lambert
            
            struct Input
            {
                float2 uv_Albedo;
                float2 uv_Normal;
            };
            
            sampler2D _Albedo;
            sampler2D _Normal;
            float _BumpStrength;
            
            void surf(Input input , inout SurfaceOutput output)
            {
                output.Albedo = tex2D(_Albedo,input.uv_Albedo).rgb;
                output.Normal = UnpackNormal(tex2D(_Normal,input.uv_Normal));
                output.Normal *= float3(_BumpStrength,_BumpStrength,1);
            }
            
        ENDCG
    }
    
    Fallback "Diffuse"
}