Shader "SLZ/LitMAS/LitMAS Acid"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        [ToggleUI] _Normals("Normal Map enabled", Float) = 1
        [NoScaleOffset][Normal] _BumpMap ("Normal map", 2D) = "bump" {}
        [NoScaleOffset]_MetallicGlossMap("MAS", 2D) = "white" {}
        [Space(30)][Header(Emissions)][Space(10)][ToggleUI] _Emission("Emission Enable", Float) = 0
        [NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}
        [HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionFalloff("Emission Falloff", Float) = 1
        _BakedMutiplier("Emission Baked Mutiplier", Float) = 1
        [Space(30)][Header(Details)][Space(10)][Toggle(_DETAILS_ON)] _Details("Details enabled", Float) = 0
        _DetailMap("DetailMap", 2D) = "gray" {}
        [Space(30)][Header(Screen Space Reflections)][Space(10)][Toggle(_NO_SSR)] _SSROff("Disable SSR", Float) = 0
        [Header(This should be 0 for skinned meshes)]
        _SSRTemporalMul("Temporal Accumulation Factor", Range(0, 2)) = 1.0

        _TessellationFactor("Tessellation factor", Float) = 1
        _TessellationBias("Tessellation bias", Float) = 1
        _PatchClipToleranceBackface("Patch clip tolerance (Backface)", Range(0, 1)) = 0.02
        _PatchClipToleranceFrustum("Patch clip tolerance (Frustum)", Range(0, 1)) = 0.02
        _Smoothing("Smoothening factor", Range(0, 10)) = 0.02
        _AcidStart("Acid Start", Range(0, 100)) = 20
        _AcidStrength("Acid Strength", Range(-10, 10)) = 1
        // This keyword enum allows us to choose between partitioning modes. It's best to try them out for yourself
        [KeywordEnum(INTEGER, FRAC_EVEN, FRAC_ODD, POW2)] _PARTITIONING("Partition algoritm", Float) = 0
    }
    SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline"  "RenderType" = "Opaque" "Queue" = "Geometry" }
        Blend One Zero
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		ColorMask RGBA
        LOD 100


        Pass
        {
            Name "Forward"
            Tags {"Lightmode"="UniversalForward"}
            HLSLPROGRAM
            //
            //#pragma hull Hull
            //#pragma domain Domain
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0

            #define LITMAS_FEATURE_LIGHTMAPPING
            #define LITMAS_FEATURE_TS_NORMALS
            #define LITMAS_FEATURE_EMISSION
            #define LITMAS_FEATURE_SSR
            #define ACID
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/PlatformCompiler.hlsl"
            #include_with_pragmas "LitMASInclude/ShaderInjector/StandardForward.hlsl"
            //
            ENDHLSL
        }

		Pass
        {
            Name "DepthOnly"
            Tags {"Lightmode"="DepthOnly"}
			ZWrite On
			//ZTest Off
			ColorMask 0

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #define ACID
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/PlatformCompiler.hlsl"
            #include_with_pragmas "LitMASInclude/DepthOnly.hlsl" 

            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags {"Lightmode" = "DepthNormals"}
            ZWrite On
            //ZTest Off
            //ColorMask 0

            HLSLPROGRAM
            #pragma hull Hull
            #pragma domain Domain
            #pragma vertex Vertex
            #pragma fragment Fragment
            #define ACID
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/PlatformCompiler.hlsl"
            #include_with_pragmas "LitMASInclude/ShaderInjector/StandardDepthNormals.hlsl" 

            ENDHLSL
        }

 		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
            Cull Off
			ColorMask 0

			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #define ACID
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/PlatformCompiler.hlsl"
            #include_with_pragmas "LitMASInclude/ShadowCaster.hlsl"

			ENDHLSL
		}

        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM

            #define _NORMAL_DROPOFF_TS 1
            #define _EMISSION
            #define _NORMALMAP 1

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ EDITOR_VISUALIZATION

            #define SHADERPASS SHADERPASS_META
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/PlatformCompiler.hlsl"
            #include "LitMASInclude/ShaderInjector/StandardMeta.hlsl" 
            ENDHLSL
        }

        Pass
		{
			
            Name "BakedRaytrace"
            Tags{ "LightMode" = "BakedRaytrace" }
			HLSLPROGRAM

            #include "LitMASInclude/BakedRayTrace.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraphLitGUI"
    Fallback "Hidden/InternalErrorShader"
}
