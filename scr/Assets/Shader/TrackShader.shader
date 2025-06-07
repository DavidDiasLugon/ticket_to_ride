// Unity Shader
Shader "TicketToRide/TrackShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _PlayerColor ("Player Color", Color) = (1,1,1,1)
        _ClaimedIntensity ("Claimed Intensity", Range(0, 1)) = 0
        
        // --- NOVAS PROPRIEDADES PARA HIGHLIGHT ---
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _PlayerColor;
            float _ClaimedIntensity;
            
            // Variáveis para as novas propriedades
            fixed4 _HighlightColor;
            float _HighlightIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.uv);
                if (spriteColor.a < 0.1) {
                    discard;
                }

                // 1. Mistura a cor do jogador com a cor de highlight
                // A função lerp faz a interpolação linear entre as duas cores
                // Se _HighlightIntensity for 0, o resultado é a cor pura do jogador.
                // Se for > 0, ele mistura com a cor de highlight.
                fixed4 baseOrHighlightedColor = lerp(_PlayerColor, _HighlightColor, _HighlightIntensity);

                // 2. Lógica das listras diagonais (sem alteração)
                float stripePattern = sin((i.uv.x + i.uv.y) * 25.0);
                float sharpStripes = step(0.5, stripePattern);
                fixed4 stripeColor = fixed4(1, 1, 1, 0.7) * sharpStripes;

                // 3. Mistura a cor (já com highlight) com as listras
                fixed4 finalColor = lerp(baseOrHighlightedColor, baseOrHighlightedColor + stripeColor, _ClaimedIntensity);

                finalColor.a = spriteColor.a;
                return finalColor;
            }
            ENDCG
        }
    }
}