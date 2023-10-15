/*-----------------------------------------------------------------------------------------------------*
 *-----------------------------------------------------------------------------------------------------*
 * WARNING: THIS FILE WAS CREATED WITH SHADERINJECTOR, AND SHOULD NOT BE EDITED DIRECTLY. MODIFY THE   *
 * BASE INCLUDE AND INJECTED FILES INSTEAD, AND REGENERATE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   *
 *-----------------------------------------------------------------------------------------------------*
 *-----------------------------------------------------------------------------------------------------*/

#if defined(ACID) 
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
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
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
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Interpolators 
{
    float3 normalWS                 : TEXCOORD0;
    float3 positionWS               : TEXCOORD1;
    float4 tangentWS                : TEXCOORD2;
    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

CBUFFER_START(UnityPerMaterial)
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
    output.normalWS = normalInputs.normalWS;
    output.tangentWS = float4(normalInputs.tangentWS, input.tangentOS.w);
    return output;
}

// The patch constant function runs once per triangle, or "patch"
// It runs in parallel to the hull function
TessellationFactors PatchConstantFunction(
    InputPatch<TessellationControlPoint, 3> patch) 
{
    UNITY_SETUP_INSTANCE_ID(patch[0]); // Set up instancing

    TessellationFactors f;
    //if (ShouldClipPatch(patch[0].positionCS, patch[1].positionCS, patch[2].positionCS, _PatchClipToleranceBackface, _PatchClipToleranceFrustum)) 
    //{
        f.edge[0] = f.edge[1] = f.edge[2] = f.inside = 100000;
    //}
    //else 
    //{
    //    // Calculate tessellation factors
    //    f.edge[0] = EdgeTesselationFactor(_TessellationFactor, _TessellationBias, patch[1].positionCS, patch[2].positionCS);
    //    f.edge[1] = EdgeTesselationFactor(_TessellationFactor, _TessellationBias, patch[2].positionCS, patch[0].positionCS);
    //    f.edge[2] = EdgeTesselationFactor(_TessellationFactor, _TessellationBias, patch[0].positionCS, patch[1].positionCS);
    //    f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) / 3.0;
    //}
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
    float3 normalWS = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].normalWS, patch[1].normalWS, patch[2].normalWS);
    float3 tangentWS = BARYCENTRIC_INTERPOLATE(barycentricCoordinates, patch[0].tangentWS.xyz, patch[1].tangentWS.xyz, patch[2].tangentWS.xyz);

    float distToCamera = distance(positionWS, _WorldSpaceCameraPos);
    float acidStren = max(distToCamera - _AcidStart, 0) * _AcidStrength * 1;
    acidStren = pow(acidStren, 1.5);
    positionWS += + float3(0, acidStren, 0);
     
    output.positionCS = TransformWorldToHClip(positionWS);
    output.normalWS = normalWS;
    output.positionWS = positionWS;
    output.tangentWS = float4(tangentWS.xyz, patch[0].tangentWS.w);

    return output;
}

float4 Fragment(Interpolators input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    // Fill the various lighting and surface data structures for the PBR algorithm
    InputData lightingInput = (InputData)0; // Found in URP/Input.hlsl
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = normalize(input.normalWS);
    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(lightingInput.positionWS);
    lightingInput.shadowCoord = GetShadowCoord(lightingInput.positionWS, input.positionCS);
    lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

    SurfaceData surface = (SurfaceData)0; // Found in URP/SurfaceData.hlsl
    surface.albedo = 0.5;
    surface.alpha = 1;
    surface.metallic = 0;
    surface.smoothness = 0.5;
    surface.normalTS = float3(0, 0, 1);
    surface.occlusion = 1;

    return UniversalFragmentPBR(lightingInput, surface);
}

#endif
#else

#define SHADERPASS SHADERPASS_DEPTHNORMALS

#if defined(SHADER_API_MOBILE)
#else
#endif


#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/EncodeNormalsTexture.hlsl"

struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
// Begin Injection VERTEX_IN from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
	float4 tangent : TANGENT;
    float2 uv0 : TEXCOORD0;
// End Injection VERTEX_IN from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 normalWS : NORMAL;
// Begin Injection INTERPOLATORS from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
    float4 tanYZ_bitXY : TEXCOORD0;
    float4 uv0XY_bitZ_fog : TEXCOORD1;
// End Injection INTERPOLATORS from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Begin Injection UNIFORMS from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
	TEXTURE2D(_BumpMap);
	SAMPLER(sampler_BumpMap);
// End Injection UNIFORMS from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------

CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    half4 _BaseColor;
// Begin Injection MATERIAL_CBUFFER from Injection_NormalMap_CBuffer.hlsl ----------------------------------------------------------
float4 _DetailMap_ST;
half  _Details;
half  _Normals;
// End Injection MATERIAL_CBUFFER from Injection_NormalMap_CBuffer.hlsl ----------------------------------------------------------
// Begin Injection MATERIAL_CBUFFER from Injection_SSR_CBuffer.hlsl ----------------------------------------------------------
    float _SSRTemporalMul;
// End Injection MATERIAL_CBUFFER from Injection_SSR_CBuffer.hlsl ----------------------------------------------------------
// Begin Injection MATERIAL_CBUFFER from Injection_Emission_CBuffer.hlsl ----------------------------------------------------------
    half  _Emission;
    half4 _EmissionColor;
    half  _EmissionFalloff;
    half  _BakedMutiplier;
// End Injection MATERIAL_CBUFFER from Injection_Emission_CBuffer.hlsl ----------------------------------------------------------
CBUFFER_END
    

v2f vert(appdata v)
{

    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


    o.vertex = TransformObjectToHClip(v.vertex.xyz);

// Begin Injection VERTEX_NORMAL from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
	VertexNormalInputs ntb = GetVertexNormalInputs(v.normal, v.tangent);
	o.normalWS = float4(ntb.normalWS, ntb.tangentWS.x);
	o.tanYZ_bitXY = float4(ntb.tangentWS.yz, ntb.bitangentWS.xy);
	o.uv0XY_bitZ_fog.zw = ntb.bitangentWS.zz;
	o.uv0XY_bitZ_fog.xy = TRANSFORM_TEX(v.uv0, _BaseMap);
// End Injection VERTEX_NORMAL from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
    

    #if defined(ACID)
    float3 wPos = TransformObjectToWorld(v.vertex.xyz);
    float dist = max(distance(_WorldSpaceCameraPos, wPos) - 20, 0);
    o.vertex.y -= dist;
    #endif


    return o;
}

half4 frag(v2f i) : SV_Target
{
   UNITY_SETUP_INSTANCE_ID(i);
   UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);


   half4 normals = half4(0, 0, 0, 1);

// Begin Injection FRAG_NORMALS from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------
	half4 normalMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv0XY_bitZ_fog.xy);
	half3 normalTS = UnpackNormal(normalMap);
	normalTS = _Normals ? normalTS : half3(0, 0, 1);


	half3x3 TStoWS = half3x3(
		i.normalWS.w, i.tanYZ_bitXY.z, i.normalWS.x,
		i.tanYZ_bitXY.x, i.tanYZ_bitXY.w, i.normalWS.y,
		i.tanYZ_bitXY.y, i.uv0XY_bitZ_fog.z, i.normalWS.z
		);
	half3 normalWS = mul(TStoWS, normalTS);
	normalWS = normalize(normalWS);

	normals = half4(EncodeWSNormalForNormalsTex(normalWS),0);
// End Injection FRAG_NORMALS from Injection_NormalMap_DepthNormals.hlsl ----------------------------------------------------------


    return normals;
}
#endif