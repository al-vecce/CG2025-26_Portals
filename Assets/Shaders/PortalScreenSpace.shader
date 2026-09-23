Shader "Custom/PortalScreenSpaceMasked"
{
    Properties
    {
        _MainTex ("Render Texture (Screen Space)", 2D) = "white" {}
        _MaskTex ("Mask Texture (Normal UV)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        // Cambiamo il rendering in AlphaTest per supportare il ritaglio (clipping)
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // UV normali del modello
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0; // UV dello schermo
                float2 uvMask : TEXCOORD1;    // UV per la maschera
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uvMask = v.uv; // Passiamo le UV standard al fragment shader
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Leggiamo la maschera usando le UV standard del Quad
                fixed4 maskColor = tex2D(_MaskTex, i.uvMask);
                
                // Se il pixel della maschera è nero (o sotto la soglia), scartalo (crea il buco/trasparenza)
                clip(maskColor.r - _Cutoff);

                // 2. Leggiamo la RenderTexture usando le coordinate schermo prospettiche[cite: 1]
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                return tex2D(_MainTex, screenUV);
            }
            ENDCG
        }
    }
}