Shader "Mid/Mid_Texture"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _BaseMap;

            void vert(float3 positionOS : POSITION,
                      float2 uv : TEXCOORD0,
                      out float2 uvOut : TEXCOORD0,
                      out float4 positionCS : SV_POSITION)
            {
                positionCS = TransformObjectToHClip(positionOS);
                uvOut = uv;
            }

            float4 frag(float2 uv : TEXCOORD0) : SV_Target
            {
                float2 p = uv;
                return tex2D(_BaseMap, p);
            }
            ENDHLSL
        }
    }
}
