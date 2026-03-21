Shader "Custom/PaletteSwap"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _PaletteTex ("Palette Lookup Texture", 2D) = "white" {}
        _SwapCount ("Number of Color Swaps", Int) = 0
        _Threshold ("Color Match Threshold", Range(0.0, 0.2)) = 0.05
        [PerRendererData] _Color ("Tint Color", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "IgnoreProjector"   = "True"
            "RenderType"        = "Transparent"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            int       _SwapCount;
            float     _Threshold;
            fixed4    _Color;

            float4 _OriginalColors[32];
            float4 _SwappedColors[32];

            // Convert a single gamma-space channel to linear
            float GammaToLinear(float c)
            {
                return (c <= 0.04045) ? c / 12.92 : pow((c + 0.055) / 1.055, 2.4);
            }

            float3 GammaToLinear3(float3 c)
            {
                return float3(GammaToLinear(c.r), GammaToLinear(c.g), GammaToLinear(c.b));
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex   = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color    = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, IN.texcoord);

                if (texColor.a < 0.001)
                    discard;

                fixed4 finalColor = texColor;

                // The texture is sampled in whatever color space the project uses.
                // The colors set from C# (Color picker) are in gamma space.
                // We convert the picker colors to linear so the comparison is apples-to-apples.
                for (int i = 0; i < _SwapCount; i++)
                {
                    // Convert stored gamma-space original to linear for comparison
                    float3 origLinear = GammaToLinear3(_OriginalColors[i].rgb);
                    float3 diff = abs(texColor.rgb - origLinear);

                    if (diff.r < _Threshold && diff.g < _Threshold && diff.b < _Threshold)
                    {
                        // Also convert the replacement to linear
                        finalColor.rgb = GammaToLinear3(_SwappedColors[i].rgb);
                        break;
                    }
                }

                finalColor *= IN.color;
                finalColor.rgb *= finalColor.a; // pre-multiply alpha
                return finalColor;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
