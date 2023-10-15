Shader "extraes/Fisheye"
{
    Properties
    {
        [MainTexture] _MainTex("_MainTex", any) = "" {}
        _SpherizeStrength("_SpherizeStrength", float) = -10
        [ShowAsVector2]_SpherizeCenter("_SpherizeCenter", Vector) = (0.5, 0.5, 0.5, 0.5)
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
        half2 _SpherizeCenter;
        half _SpherizeStrength;
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

                //IN.uv -= 0.5;

                //IN.uv *= IN.uv; 
                //IN.uv = pow(IN.uv, 0.5);

                half2 spherize_delta = IN.uv - _SpherizeCenter;
                half spherize_delta_dot = dot(spherize_delta.xy, spherize_delta.xy);
                half spherize_delta_squared = spherize_delta_dot * spherize_delta_dot;
                half2 spherize_delta_offset = spherize_delta_squared * _SpherizeStrength;

                IN.uv = IN.uv + spherize_delta * spherize_delta_offset;

                //IN.uv += 0.5;

                return SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, IN.uv);
            }
            ENDHLSL
        }
    }
}