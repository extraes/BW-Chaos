Shader "extraes/DepthSaturation"
{
    Properties
    {
        [MainTexture] _MainTex("_MainTex", any) = "" {}
        _DepthTex("_DepthTex", any) = "" {}
        _DepthMult("_DepthMult", Float) = 100
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
        float _DepthMult;
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

            // https://github.com/greggman/hsva-unity/blob/master/Assets/Shaders/HSVRangeShader.shader
            float3 rgb2hsv(float3 c) {
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }


            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                float depth = SAMPLE_TEXTURE2D_X(_DepthTex, sampler_DepthTex, IN.uv).r;
                half4 albedo = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, IN.uv);

                float3 hsv = rgb2hsv(albedo.xyz);
                
                hsv.y *= _DepthMult * depth;

                float3 rgb = hsv2rgb(hsv);

                return float4(rgb.rgb, albedo.a);
            }
            ENDHLSL
        }
    }
}