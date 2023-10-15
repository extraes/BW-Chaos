// MIT License

// Copyright (c) 2021 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#ifndef TESSELLATION_FACTORS_INCLUDED
#define TESSELLATION_FACTORS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


// Call this macro to interpolate between a triangle patch, passing the field name
#define BARYCENTRIC_INTERPOLATE(barycentricCoordinates, p0, p1, p2) \
		p0 * barycentricCoordinates.x + \
		p1 * barycentricCoordinates.y + \
		p2 * barycentricCoordinates.z

struct Attributes 
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float4 uv0       : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct TessellationFactors 
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

struct TessellationControlPoint 
{
    float3 positionWS : INTERNALTESSPOS;
    float4 positionCS : SV_POSITION;
    float3 normalWS : NORMAL;
    float4 tangentWS : TANGENT;
    float4 uv0XY_bitZ_fog : TEXCOORD0;
    float4 normXYZ_tanX : TEXCOORD1;
    float4 tanYZ_bitXY : TEXCOORD2;
    float4 positionOS : TEXCOORD3;
    float4 SHVertLights : TEXCOORD4;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators 
{
    float3 normalWS                 : TEXCOORD0;
    float4 positionOS               : TEXCOORD1;
    float3 positionWS               : TEXCOORD2;
    float4 tangentWS                : TEXCOORD3;
    float4 uv0XY_bitZ_fog           : TEXCOORD4;
    float4 normXYZ_tanX             : TEXCOORD5;
    float4 tanYZ_bitXY              : TEXCOORD6;
    float4 SHVertLights             : TEXCOORD7;
    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

TEXTURE2D(_BumpMap);
TEXTURE2D(_MetallicGlossMap);

TEXTURE2D(_DetailMap);
SAMPLER(sampler_DetailMap);

TEXTURE2D(_EmissionMap);

CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    half4 _BaseColor;
    float4 _DetailMap_ST;
    half  _Details;
    half  _Normals;
    float _SSRTemporalMul;
    half  _Emission;
    half4 _EmissionColor;
    half  _EmissionFalloff;
    half  _BakedMutiplier;

    float _TessellationFactor;
    float _TessellationBias;
    float _PatchClipToleranceBackface;
    float _PatchClipToleranceFrustum;
    float _AcidStart;
    float _AcidStrength;
    float _Smoothing;
CBUFFER_END

float3 GetViewDirectionFromPosition(float3 positionWS) 
{
    return normalize(GetCameraPositionWS() - positionWS);
}

float4 GetShadowCoord(float3 positionWS, float4 positionCS) 
{
    // Calculate the shadow coordinate depending on the type of shadows currently in use
#if SHADOWS_SCREEN
    return ComputeScreenPos(positionCS);
#else
    return TransformWorldToShadowCoord(positionWS);
#endif
}

bool IsOutOfBounds(float3 p, float3 lower, float3 higher) 
{
    bool oobX = p.x < lower.x || p.x > higher.x;
    bool oobY = p.y < lower.y || p.y > higher.y;
    bool oobZ = p.z < lower.z || p.z > higher.z;
    return oobX || oobY || oobZ;
}

bool IsOutOfFrustum(float4 positionCS, float tolerance)
{
    float3 cullPt = positionCS.xyz;
    float w = positionCS.w;
    
    float3 lowerBounds = float3(-w, -w, -w * UNITY_RAW_FAR_CLIP_VALUE) - tolerance.xxx;
    float3 higherBounds = float3(w, w, w) + tolerance.xxx;
    return IsOutOfBounds(cullPt, lowerBounds, higherBounds);
}

bool ShouldBackfaceCull(float4 p0PositionCS, float4 p1PositionCS, float4 p2PositionCS, float tolerance)
{
    float3 p0 = p0PositionCS.xyz / p0PositionCS.w;
    float3 p1 = p1PositionCS.xyz / p1PositionCS.w;
    float3 p2 = p2PositionCS.xyz / p2PositionCS.w;
    float3 normal = cross(p1 - p0, p2 - p1);
    
    #if UNITY_REVERSED_Z
    return normal.z < -tolerance;
    #else
    return normal.z > tolerance;
    #endif
}

bool ShouldClipPatch(float4 p0PositionCS, float4 p1PositionCS, float4 p2PositionCS, float toleranceBack, float toleranceFrustum)
{
    bool outside1 = IsOutOfFrustum(p0PositionCS, toleranceFrustum);
    bool outside2 = IsOutOfFrustum(p1PositionCS, toleranceFrustum);
    bool outside3 = IsOutOfFrustum(p2PositionCS, toleranceFrustum);
    bool allOutside = outside1 && outside2 && outside3;
    
    bool doBackfaceCull = ShouldBackfaceCull(p0PositionCS, p1PositionCS, p2PositionCS, toleranceBack);

    return allOutside || doBackfaceCull;
}

float EdgeTesselationFactor(float scale, float bias, float4 p0PositionCS, float4 p1PositionCS)
{
    float factor = distance(p0PositionCS.xyz / p0PositionCS.w, p1PositionCS.xyz / p1PositionCS.w) * (_ScreenParams.y / scale);

    return max(1, factor + bias);
}

float3 PhongProjectedPosition(float3 flatPositionWS, float3 cornerPositionWS, float3 normalWS) 
{
    return flatPositionWS - dot(flatPositionWS - cornerPositionWS, normalWS) * normalWS;
}

float3 CalculatePhongPosition(float3 bary, float smoothing, float3 p0PosWS, float3 p0NormWS, float3 p1PosWS, float3 p1NormWS, float3 p2PosWS, float3 p2NormWS)
{
    float3 flatPositionWS = BARYCENTRIC_INTERPOLATE(bary, p0PosWS, p1PosWS, p2PosWS);
    float3 smoothedPosWS =
        bary.x * PhongProjectedPosition(flatPositionWS, p0PosWS, p0NormWS) +
        bary.y * PhongProjectedPosition(flatPositionWS, p1PosWS, p1NormWS) +
        bary.z * PhongProjectedPosition(flatPositionWS, p2PosWS, p2NormWS);
    return lerp(flatPositionWS, smoothedPosWS, smoothing);
}

TessellationControlPoint Vertex(Attributes input) 
{
    TessellationControlPoint output;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

    float distToCamera = distance(posnInputs.positionWS, _WorldSpaceCameraPos);
    float acidStren = max(distToCamera - _AcidStart, 0) * _AcidStrength;
    acidStren = pow(acidStren, 1.5);
    //float acidStren = 0;

    output.positionWS = posnInputs.positionWS + float3(0, acidStren, 0);
    output.positionCS = posnInputs.positionCS;
    output.positionOS = input.positionOS;
    output.normalWS = normalInputs.normalWS;
    output.normXYZ_tanX = float4(output.normalWS.xyz, normalInputs.tangentWS.x);
    output.tangentWS = float4(normalInputs.tangentWS, input.tangentOS.w);
    output.uv0XY_bitZ_fog.xy = input.uv0.xy;

    half clipZ_0Far = UNITY_Z_0_FAR_FROM_CLIPSPACE(input.positionOS);
    output.uv0XY_bitZ_fog.w = unity_FogParams.x * clipZ_0Far;

    //output.tanYZ_bitXY = half4(ntb.tangentWS.yz, ntb.bitangentWS.xy);
	output.uv0XY_bitZ_fog.z = normalInputs.bitangentWS.z;

    output.tanYZ_bitXY = float4(normalInputs.tangentWS.yz, normalInputs.bitangentWS.xy);
    
    output.SHVertLights = 0;
    // Calculate vertex lights and L2 probe lighting on quest 
    output.SHVertLights.xyz = VertexLighting(output.positionWS, output.normXYZ_tanX.xyz);
#if !defined(LIGHTMAP_ON) && !defined(DYNAMICLIGHTMAP_ON) && defined(SHADER_API_MOBILE)
    output.SHVertLights.xyz += SampleSHVertex(output.normXYZ_tanX.xyz);
#endif

    return output;
}

// The patch constant function runs once per triangle, or "patch"
// It runs in parallel to the hull function
TessellationFactors PatchConstantFunction(
    InputPatch<TessellationControlPoint, 3> patch) 
{
    UNITY_SETUP_INSTANCE_ID(patch[0]); // Set up instancing

    TessellationFactors f;
    if (ShouldClipPatch(patch[0].positionCS, patch[1].positionCS, patch[2].positionCS, _PatchClipToleranceBackface, _PatchClipToleranceFrustum)) 
    {
        f.edge[0] = f.edge[1] = f.edge[2] = f.inside = 1;
    }
    else 
    {
        // Calculate tessellation factors
        f.edge[0] = EdgeTesselationFactor(_TessellationFactor, _TessellationBias, patch[1].positionCS, patch[2].positionCS);
        f.edge[1] = EdgeTesselationFactor(_TessellationFactor, _TessellationBias, patch[2].positionCS, patch[0].positionCS);
        f.edge[2] = EdgeTesselationFactor(_TessellationFactor, _TessellationBias, patch[0].positionCS, patch[1].positionCS);
        f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) / 3.0;
    }
    return f;
}

// The hull function runs once per vertex. You can use it to modify vertex
// data based on values in the entire triangle
[domain("tri")] // Signal we're inputting triangles
[outputcontrolpoints(3)] // Triangles have three points
[outputtopology("triangle_cw")] // Signal we're outputting triangles
[patchconstantfunc("PatchConstantFunction")] // Register the patch constant function
// Select a partitioning mode based on keywords
#if defined(_PARTITIONING_INTEGER)
[partitioning("integer")]
#elif defined(_PARTITIONING_FRAC_EVEN)
[partitioning("fractional_even")]
#elif defined(_PARTITIONING_FRAC_ODD)
[partitioning("fractional_odd")]
#elif defined(_PARTITIONING_POW2)
[partitioning("pow2")]
#else 
[partitioning("fractional_odd")]
#endif
TessellationControlPoint Hull(
    InputPatch<TessellationControlPoint, 3> patch, // Input triangle
    uint id : SV_OutputControlPointID) { // Vertex index on the triangle

    return patch[id];
}

// The domain function runs once per vertex in the final, tessellated mesh
// Use it to reposition vertices and prepare for the fragment stage
[domain("tri")] // Signal we're inputting triangles
Interpolators Domain(
    TessellationFactors factors, // The output of the patch constant function
    OutputPatch<TessellationControlPoint, 3> patch, // The Input triangle
    float3 barycentricCoordinates : SV_DomainLocation) { // The barycentric coordinates of the vertex on the triangle

    Interpolators output;

    // Setup instancing and stereo support (for VR)
    UNITY_SETUP_INSTANCE_ID(patch[0]);
    UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);


    float3 positionWS = CalculatePhongPosition(barycentricCoordinates, _Smoothing,
        patch[0].positionWS, patch[0].normalWS,
        patch[1].positionWS, patch[1].normalWS,
        patch[2].positionWS, patch[2].normalWS);
    float4 positionOS = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].positionOS, patch[1].positionOS, patch[2].positionOS);
    float3 normalWS = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].normalWS, patch[1].normalWS, patch[2].normalWS);
    float3 tangentWS = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].tangentWS.xyz, patch[1].tangentWS.xyz, patch[2].tangentWS.xyz);
    float3 uv0fog = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].uv0XY_bitZ_fog, patch[1].uv0XY_bitZ_fog, patch[2].uv0XY_bitZ_fog);
    float4 tanbit = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].tanYZ_bitXY, patch[1].tanYZ_bitXY, patch[2].tanYZ_bitXY);
    float4 SHVertLights = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].SHVertLights, patch[1].SHVertLights, patch[2].SHVertLights);

    float distToCamera = distance(positionWS, _WorldSpaceCameraPos);
    float acidStren = max(distToCamera - _AcidStart, 0) * _AcidStrength * 1;
    acidStren = pow(acidStren, 1.5);
    positionWS += + float3(0, acidStren, 0);
     
    output.positionCS = TransformWorldToHClip(positionWS);
    output.positionOS = positionOS;
    output.normalWS = normalWS;
    output.positionWS = positionWS;
    output.tangentWS = float4(tangentWS.xyz, patch[0].tangentWS.w);
    output.uv0XY_bitZ_fog.xy = uv0fog;
    output.tanYZ_bitXY = tanbit;
    output.SHVertLights = SHVertLights;

    return output;
}

float4 Fragment(Interpolators input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

/*---------------------------------------------------------------------------------------------------------------------------*/
/*---Read Input Data---------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------*/

    float2 uv_main = mad(float2(input.uv0XY_bitZ_fog.xy), _BaseMap_ST.xy, _BaseMap_ST.zw);
    float2 uv_detail = mad(float2(input.uv0XY_bitZ_fog.xy), _DetailMap_ST.xy, _DetailMap_ST.zw);
    half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv_main) * _BaseColor;
    half4 mas = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_BaseMap, uv_main);


    half metallic = mas.r;
    half ao = mas.g;
    half smoothness = mas.b;


/*---------------------------------------------------------------------------------------------------------------------------*/
/*---Sample Normal Map-------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------*/

    half3 normalTS = half3(0, 0, 1);
    half  geoSmooth = 1;
    half4 normalMap = half4(0, 0, 1, 0);

// Begin Injection NORMAL_MAP from Injection_NormalMaps.hlsl ----------------------------------------------------------
	normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BaseMap, uv_main);
	normalTS = UnpackNormal(normalMap);
	normalTS = _Normals ? normalTS : half3(0, 0, 1);
	geoSmooth = _Normals ? normalMap.b : 1.0;
	smoothness = saturate(smoothness + geoSmooth - 1.0);
// End Injection NORMAL_MAP from Injection_NormalMaps.hlsl ----------------------------------------------------------

/*---------------------------------------------------------------------------------------------------------------------------*/
/*---Read Detail Map---------------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------*/

    #if defined(_DETAILS_ON) 

// Begin Injection DETAIL_MAP from Injection_NormalMaps.hlsl ----------------------------------------------------------
		half4 detailMap = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, uv_detail);
		half3 detailTS = half3(2.0 * detailMap.ag - 1.0, 1.0);
		normalTS = BlendNormal(normalTS, detailTS);
// End Injection DETAIL_MAP from Injection_NormalMaps.hlsl ----------------------------------------------------------
       
        smoothness = saturate(2.0 * detailMap.b * smoothness);
        albedo.rgb = OverlayBlendDetail(detailMap.r, albedo.rgb);

    #endif


/*---------------------------------------------------------------------------------------------------------------------------*/
/*---Transform Normals To Worldspace-----------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------*/

// Begin Injection NORMAL_TRANSFORM from Injection_NormalMaps.hlsl ----------------------------------------------------------
	half3 normalWS = input.normalWS;
	half3x3 TStoWS = half3x3(
		input.normXYZ_tanX.w, input.tanYZ_bitXY.z, normalWS.x,
		input.tanYZ_bitXY.x, input.tanYZ_bitXY.w, normalWS.y,
		input.tanYZ_bitXY.y, input.uv0XY_bitZ_fog.z, normalWS.z
		);
	normalWS = mul(TStoWS, normalTS);
	normalWS = normalize(normalWS);
// End Injection NORMAL_TRANSFORM from Injection_NormalMaps.hlsl ----------------------------------------------------------


/*---------------------------------------------------------------------------------------------------------------------------*/
/*---Lighting Calculations---------------------------------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------------------------------------------------------*/
    
// Begin Injection SPEC_AA from Injection_NormalMaps.hlsl ----------------------------------------------------------
	#if !defined(SHADER_API_MOBILE) && !defined(LITMAS_FEATURE_TP) // Specular antialiasing based on normal derivatives. Only on PC to avoid cost of derivatives on Quest
		smoothness = min(smoothness, SLZGeometricSpecularAA(normalWS));
	#endif
// End Injection SPEC_AA from Injection_NormalMaps.hlsl ----------------------------------------------------------


    //#if defined(LIGHTMAP_ON)
    //    SLZFragData fragData = SLZGetFragData(input.vertex, input.wPos, normalWS, input.uv1.xy, input.uv1.zw, input.SHVertLights.xyz);
    //#else
    SLZFragData fragData = SLZGetFragData(input.positionOS, input.positionWS, normalWS, float2(0, 0), float2(0, 0), input.SHVertLights.xyz);
    //#endif

    half4 emission = half4(0,0,0,0);

// Begin Injection EMISSION from Injection_Emission.hlsl ----------------------------------------------------------
    UNITY_BRANCH if (_Emission)
    {
        emission += SAMPLE_TEXTURE2D(_EmissionMap, sampler_BaseMap, uv_main) * _EmissionColor;
        emission.rgb *= lerp(albedo.rgb, half3(1, 1, 1), emission.a);
        emission.rgb *= pow(abs(fragData.NoV), _EmissionFalloff);
    }
// End Injection EMISSION from Injection_Emission.hlsl ----------------------------------------------------------


    SLZSurfData surfData = SLZGetSurfDataMetallicGloss(albedo.rgb, saturate(metallic), saturate(smoothness), ao, emission.rgb);
    half4 color = half4(1, 1, 1, 1);


// Begin Injection LIGHTING_CALC from Injection_SSR.hlsl ----------------------------------------------------------
	#if defined(_SSR_ENABLED)
        half4 noiseRGBA = GetScreenNoiseRGBA(fragData.screenUV);

        SSRExtraData ssrExtra;
        ssrExtra.meshNormal = input.normXYZ_tanX.xyz;
        ssrExtra.lastClipPos = input.lastVertex;
        ssrExtra.temporalWeight = _SSRTemporalMul;
        ssrExtra.depthDerivativeSum = 0;
        ssrExtra.noise = noiseRGBA;
        ssrExtra.fogFactor = input.uv0XY_bitZ_fog.w;

        color.rgb = max(0,SLZPBRFragmentSSR(fragData, surfData, ssrExtra));
    #else
        color.rgb = SLZPBRFragment(fragData, surfData);
    #endif
// End Injection LIGHTING_CALC from Injection_SSR.hlsl ----------------------------------------------------------


// Begin Injection VOLUMETRIC_FOG from Injection_SSR.hlsl ----------------------------------------------------------
    #if !defined(_SSR_ENABLED)
        color.rgb = MixFog(color.rgb, -fragData.viewDir, input.uv0XY_bitZ_fog.w);
        color = Volumetrics(color, fragData.position);
    #endif
// End Injection VOLUMETRIC_FOG from Injection_SSR.hlsl ----------------------------------------------------------
    return color;
}

#endif