Shader "Udemy/Bunny"
{
   Properties
   {
        _Color("Bunny Color" , Color) = (1,1,1,1)
   }
   
   SubShader
   {
        CGPROGRAM
            #pragma surface surf Lambert
            
            struct Input
            {
                float2 uv_texture;
            };
            float4 _Color;
            
            void surf(Input input , inout SurfaceOutput output)
            {
                output.Albedo = _Color.rgb;
            }
            
        ENDCG
   }
   
   Fallback "Diffuse" 
}