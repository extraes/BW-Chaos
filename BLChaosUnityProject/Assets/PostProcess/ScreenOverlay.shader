Shader "extraes/ScreenOverlay"
{
    Properties
    {
        [MainTexture] _MainTex("_MainTex", any) = "" {}
        _OverlayTex("_OverlayTex", any) = "" {}
        _OverlayOffset("_OverlayOffset", vector) = (0, 0, 0, 0)
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
        float4 _OverlayTex_ST;
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
            TEXTURE2D_X(_OverlayTex);
            SAMPLER(sampler_OverlayTex);

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

                half4 albedo = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, IN.uv);

                float2 overlayUv = (IN.uv - 0.5) * _OverlayTex_ST.xy;
                overlayUv += 0.5;

                half4 overlay = SAMPLE_TEXTURE2D_X(_OverlayTex, sampler_OverlayTex, overlayUv + _OverlayTex_ST.zw);

                return albedo * (1 - overlay.a) + overlay * (overlay.a);
            }
            ENDHLSL
        }
    }
}
