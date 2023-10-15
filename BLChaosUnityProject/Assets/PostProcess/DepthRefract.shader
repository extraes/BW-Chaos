Shader "extraes/DepthRefract"
{
    Properties
    {
        [MainTexture] _MainTex("_MainTex", any) = "" {}
        _DepthTex("_DepthTex", any) = "" {}
        _RefractionScaleX("_RefractionScaleX", Float) = 200
        _RefractionScaleY("_RefractionScaleY", Float) = 200
        _Strength("_Strength", Float) = 1
        _DepthPow("_DepthPow", Range(-1, 1)) = 0.25
        [ToggleUI] _OneMinusDepth("_OneMinusDepth", Float) = 0
    }

    SubShader
    {
        //Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Cull Off 
        ZWrite Off 
        ZTest Always

        // Include material cbuffer for all passes. 
        // The cbuffer has to be the same for all passes to make this shader SRP batcher compatible.
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _DepthTex_ST;
        half _RefractionScaleX;
        half _RefractionScaleY;
        half _Strength;
        half _OneMinusDepth;
        half _DepthPow;
        CBUFFER_END

        ENDHLSL

        Pass
        {
            //Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D_X(_MainTex);
            SamplerState sampler_LinearMirror_MainTex;
            TEXTURE2D_X(_DepthTex);
            SAMPLER(sampler_DepthTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = float4(IN.positionOS.xyz, 1);
                OUT.uv = IN.uv;

                #if UNITY_UV_STARTS_AT_TOP
                OUT.positionHCS.y *= -1;
                #endif

                //OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                //OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                float depth = SAMPLE_TEXTURE2D_X(_DepthTex, sampler_DepthTex, IN.uv);

                #if UNITY_UNITY_REVERSED_Z
                depth = 1 - depth;
                #endif

                depth = _OneMinusDepth ? 1 - depth : depth;

                depth = depth ? pow(depth, _DepthPow) : depth;

                float2 offset = sin(float2(_RefractionScaleX, _RefractionScaleY) * IN.uv);
                offset /= (_RefractionScaleX, _RefractionScaleY);
                offset *= _Strength;
                offset *= depth;
                
                return SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearMirror_MainTex, IN.uv + offset);
            }
            ENDHLSL
        }
    }
}