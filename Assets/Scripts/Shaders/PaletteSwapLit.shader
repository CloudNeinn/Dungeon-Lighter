Shader "Custom/PaletteSwapLit"
{
    Properties
    {
        _MainTex        ("Sprite Texture",    2D)            = "white" {}
        _NormalMap      ("Normal Map",        2D)            = "bump"  {}
        [Toggle] _UseNormalMap ("Use Normal Map", Float)     = 0
        _SwapCount      ("Number of Color Swaps", Int)       = 0
        _Threshold      ("Color Match Threshold", Range(0.0, 0.2)) = 0.05
        [PerRendererData] _Color ("Tint Color", Color)       = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor",  Color) = (1,1,1,1)
        [HideInInspector] _Flip         ("Flip",       Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex     ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "RenderType"        = "Transparent"
            "RenderPipeline"    = "UniversalPipeline"
            "IgnoreProjector"   = "True"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha         // standard alpha blend (not premul) for URP lit

        // ── Pass 1: Sprite Lit (colour + palette swap) ──────────────────────
        Pass
        {
            Name "Universal2D"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            // URP 2D lighting keywords
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            // URP 2D light textures (declared by LightingUtility if shape lights enabled)
            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif
            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _RendererColor;
                float4 _Flip;
                int    _SwapCount;
                float  _Threshold;
                float  _UseNormalMap;
            CBUFFER_END

            // Palette arrays – max 32 swaps
            float4 _OriginalColors[32];
            float4 _SwappedColors[32];

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS    : SV_POSITION;
                float4 color         : COLOR;
                float2 uv            : TEXCOORD0;
                float2 lightingUV    : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ── Gamma → Linear helpers ──────────────────────────────────────
            float  G2L(float c)   { return (c <= 0.04045) ? c / 12.92 : pow((c + 0.055) / 1.055, 2.4); }
            float3 G2L3(float3 c) { return float3(G2L(c.r), G2L(c.g), G2L(c.b)); }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightSharedCode.hlsl"

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 pos       = IN.positionOS * float3(_Flip.x, _Flip.y, 1);
                OUT.positionCS   = TransformObjectToHClip(pos);
                OUT.uv           = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.lightingUV   = half2(ComputeScreenPos(OUT.positionCS / OUT.positionCS.w).xy);
                OUT.color        = IN.color * _Color * _RendererColor;
                return OUT;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightSharedCode.hlsl"

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                if (texColor.a < 0.001)
                    discard;

                // ── Palette swap ────────────────────────────────────────────
                for (int i = 0; i < _SwapCount; i++)
                {
                    float3 origLinear = G2L3(_OriginalColors[i].rgb);
                    float3 diff = abs(texColor.rgb - origLinear);
                    if (diff.r < _Threshold && diff.g < _Threshold && diff.b < _Threshold)
                    {
                        texColor.rgb = G2L3(_SwappedColors[i].rgb);
                        break;
                    }
                }

                // ── Normal for lighting ─────────────────────────────────────
                half4 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv);
                half3 normalTS = _UseNormalMap > 0.5
                    ? UnpackNormal(normalSample)
                    : half3(0, 0, 1);   // flat normal when no map assigned

                half4 finalColor = texColor * IN.color;

                // Feed into URP 2D lighting
                return CombinedShapeLightShared(finalColor, half4(normalTS, 1), IN.lightingUV);
            }
            ENDHLSL
        }

        // ── Pass 2: Normal map pass (required by URP 2D renderer) ───────────
        Pass
        {
            Name "NormalsRendering"
            Tags { "LightMode" = "NormalsRendering" }

            HLSLPROGRAM
            #pragma vertex   vertNormal
            #pragma fragment fragNormal

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _RendererColor;
                float4 _Flip;
                int    _SwapCount;
                float  _Threshold;
                float  _UseNormalMap;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS  : POSITION;
                float4 color       : COLOR;
                float2 uv          : TEXCOORD0;
                float4 tangent     : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3  normalWS    : TEXCOORD1;
                half3  tangentWS   : TEXCOORD2;
                half3  bitangentWS : TEXCOORD3;
            };

            Varyings vertNormal(Attributes IN)
            {
                Varyings OUT;
                float3 pos       = IN.positionOS * float3(_Flip.x, _Flip.y, 1);
                OUT.positionCS   = TransformObjectToHClip(pos);
                OUT.uv           = TRANSFORM_TEX(IN.uv, _MainTex);

                OUT.normalWS    = TransformObjectToWorldNormal(float3(0, 0, -1));
                OUT.tangentWS   = TransformObjectToWorldDir(IN.tangent.xyz);
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangent.w;
                return OUT;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            half4 fragNormal(Varyings IN) : SV_Target
            {
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 normalTS = _UseNormalMap > 0.5
                    ? UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv))
                    : half3(0, 0, 1);

                return NormalsRenderingShared(mainTex, normalTS,
                    IN.tangentWS, IN.bitangentWS, IN.normalWS);
            }
            ENDHLSL
        }

        // ── Pass 3: Unlit fallback (scene without 2D lights) ────────────────
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex   vertUnlit
            #pragma fragment fragUnlit

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _RendererColor;
                float4 _Flip;
                int    _SwapCount;
                float  _Threshold;
                float  _UseNormalMap;
            CBUFFER_END

            float4 _OriginalColors[32];
            float4 _SwappedColors[32];

            struct Attributes { float3 positionOS : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionCS : SV_POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };

            float  G2L(float c)   { return (c <= 0.04045) ? c / 12.92 : pow((c + 0.055) / 1.055, 2.4); }
            float3 G2L3(float3 c) { return float3(G2L(c.r), G2L(c.g), G2L(c.b)); }

            Varyings vertUnlit(Attributes IN)
            {
                Varyings OUT;
                float3 pos     = IN.positionOS * float3(_Flip.x, _Flip.y, 1);
                OUT.positionCS = TransformObjectToHClip(pos);
                OUT.uv         = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color      = IN.color * _Color * _RendererColor;
                return OUT;
            }

            half4 fragUnlit(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                if (texColor.a < 0.001) discard;

                for (int i = 0; i < _SwapCount; i++)
                {
                    float3 origLinear = G2L3(_OriginalColors[i].rgb);
                    float3 diff = abs(texColor.rgb - origLinear);
                    if (diff.r < _Threshold && diff.g < _Threshold && diff.b < _Threshold)
                    {
                        texColor.rgb = G2L3(_SwappedColors[i].rgb);
                        break;
                    }
                }

                return texColor * IN.color;
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/2D/Sprite-Lit-Default"
}
