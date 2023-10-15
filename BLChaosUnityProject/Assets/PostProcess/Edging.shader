Shader "extraes/Edging"
{
    Properties
    {
        [MainTexture] _MainTex("_MainTex", any) = "" {}
        _DepthTex("_DepthTex", any) = "" {}
        [ToggleUI] _UseMainTex("_UseMainTex", Float) = 1
        _DepthPow("_DepthPow", Range(-1, 1)) = 0.25
        _Width("_Width", Range(0, 0.05)) = 0.01
        _DepthMult("_DepthMult", Float) = 5
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
        half _UseMainTex;
        half _DepthPow;
        half _DepthMult;
        float _Width;
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
            SAMPLER(sampler_MainTex);

            
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

                half4 albedo = _UseMainTex ? SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, IN.uv) : 1;
                float2 offs = float2(_Width, 0);
                half depth1 = SAMPLE_TEXTURE2D_X(_DepthTex, sampler_DepthTex, IN.uv + offs).r;
                half depth2 = SAMPLE_TEXTURE2D_X(_DepthTex, sampler_DepthTex, IN.uv - offs).r;

                #if !UNITY_REVERSED_Z
                depth1 = 1 - depth1;
                depth2 = 1 - depth2;
                #endif

                depth1 = depth1 ? pow(depth1, _DepthPow) : depth1;
                depth2 = depth2 ? pow(depth2, _DepthPow) : depth2;

                //half umtMult = _UseMainTex > 0 ? 10 : 1;
                
                return albedo * abs(depth1 - depth2)  * _DepthMult;
            }
            ENDHLSL
        }
    }
}