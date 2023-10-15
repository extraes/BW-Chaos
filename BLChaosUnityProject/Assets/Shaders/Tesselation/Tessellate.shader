Shader "extraes/Tessellation"
{
    Properties
    {
        [MainTexture] _MainTex("_MainTex", any) = "" {}
        _FactorEdge1("_FactorEdge1", Vector) = (1,1,1,1)
        _FactorEdge2("_FactorEdge2", Float) = 1
        _FactorEdge3("_FactorEdge3", Float) = 1
        _FactorInside("_FactorInside", Float) = 1
    }

    SubShader
    {
        //Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}
        Blend One Zero
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		ColorMask RGBA

        // Include material cbuffer for all passes. 
        // The cbuffer has to be the same for all passes to make this shader SRP batcher compatible.
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _FactorEdge1;
        float _FactorEdge2;
        float _FactorEdge3;
        float _FactorInside;
        CBUFFER_END

        ENDHLSL

        Pass
        {
            Name "Forward"
            Tags {"Lightmode"="UniversalForward"}

            HLSLPROGRAM
            #pragma target 5.0 // required for tessellation
            #pragma vertex vert
            #pragma fragment frag
            #pragma domain dom
            #pragma hull hull
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TesselationControlPoint
            {
                float2 uv           : TEXCOORD0;
                float3 positionWS   : INTERNALTESSPOS;
                float3 normalWS     : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TesselationFactors 
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct Interpolators 
            {
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D_X(_MainTex);
            SAMPLER(sampler_MainTex);

            TesselationControlPoint vert(Attributes IN)
            {
                TesselationControlPoint OUT;

                //UNITY_SETUP_INSTANCE_ID(IN);
                //UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                //OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                VertexPositionInputs posnInputs = GetVertexPositionInputs(IN.positionOS);
                VertexNormalInputs normInputs = GetVertexNormalInputs(IN.positionOS);

                OUT.positionWS = posnInputs.positionWS;
                OUT.normalWS = normInputs.normalWS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag(TesselationControlPoint IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                return SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, IN.uv);
            }

            // runs once per vert
            [domain("tri")] // inputting triangles
            [outputcontrolpoints(3)] // triangles have 3 points
            [outputtopology("triangle_cw")] // outputting triangles
            [patchconstantfunc("PatchConstantFunction")] // register patch constant function
            [partitioning("integer")] // subdivision algo: integer, fractional_odd, fractional_even, pow2
            TesselationControlPoint hull(
            InputPatch<TesselationControlPoint, 3> patch, // input triangle
            uint id : SV_OutputControlPointID) // vert idx 
            {
                return patch[id];
            }

            TesselationFactors PatchConstantFunction(InputPatch<TesselationControlPoint, 3> patch) 
            {
                UNITY_SETUP_INSTANCE_ID(patch[0]); // setup instanceid for SPI
                TesselationFactors f;
                f.edge[0] = _FactorEdge1.x;
                f.edge[1] = _FactorEdge2;
                f.edge[2] = _FactorEdge3;
                f.inside = _FactorInside;
                return f;
            }

            #define BARYCENTRIC_INTERPOLATE(fieldName) \
                    patch[0].fieldName * barycentricCoordinates.x + \
                    patch[1].fieldName * barycentricCoordinates.y + \
                    patch[2].fieldName * barycentricCoordinates.z

            [domain("tri")] // inputting triangles
            Interpolators dom(
                TesselationFactors factors, // patch const func output
                OutputPatch<TesselationControlPoint, 3> patch, // input triangle
                float3 barycentricCoordinates : SV_DomainLocation) // barycentric vertex coords on tri
            {
                Interpolators output;

                UNITY_SETUP_INSTANCE_ID(patch[0]);
                UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = BARYCENTRIC_INTERPOLATE(positionWS);
                float3 normalWS = BARYCENTRIC_INTERPOLATE(normalWS);

                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = normalWS;
                output.positionWS = positionWS;

                return output;
            }

            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags {"Lightmode" = "DepthNormals"}
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct Attributes
            {
                float4 positionOS   : POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                return 0;
            }
            ENDHLSL
        }
    }
}
