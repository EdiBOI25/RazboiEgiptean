// "Borrowed" and rewritten from: https://www.shadertoy.com/view/3sfczf
// Credit goes to: trinketMage

Shader "EdiCustom/Unlit/MainMenuBackgroundShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Opacity ("Opacity", Range(0, 1)) = 1
        _Speed ("Speed", Range(0, 10)) = 1
        
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (0,0,1,1)
        _Color3 ("Color 3", Color) = (0,1,0,1)
        _Color4 ("Color 4", Color) = (1,1,0,1)
        _Color5 ("Color 5", Color) = (1,0,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Opacity;
            float _Speed;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;


            static const float2x2 m = float2x2(0.80, -0.60, 0.60, 0.80);
            
            float noise(float2 p)
            {
                return sin(p.x) * sin(p.y);
            }
            
            float fbm4(float2 p)
            {
                float f = 0.0;
                f += 0.5000 * noise(p); p = mul(m, p * 2.02);
                f += 0.2500 * noise(p); p = mul(m, p * 2.02);
                f += 0.1250 * noise(p); p = mul(m, p * 2.02);
                f += 0.0625 * noise(p);
                return f / 0.9375;
            }
            
            float fbm6(float2 p)
            {
                float f = 0.0;
                f += 0.500000 * (0.5 + 0.5 * noise(p)); p = mul(m, p * 2.02);
                f += 0.500000 * (0.5 + 0.5 * noise(p)); p = mul(m, p * 2.02);
                f += 0.500000 * (0.5 + 0.5 * noise(p)); p = mul(m, p * 2.02);
                f += 0.250000 * (0.5 + 0.5 * noise(p)); p = mul(m, p * 2.02);
                return f / 0.96875;
            }
            
            float2 fbm4_2(float2 p)
            {
                return float2(fbm4(p), fbm4(p + float2(7.8, 7.8)));
            }
            
            float2 fbm6_2(float2 p)
            {
                return float2(fbm6(p + float2(16.8, 16.8)), fbm6(p + float2(11.5, 11.5)));
            }
            
            float func(float2 q, out float4 ron)
            {
                q += 0.03 * sin(float2(0.27, 0.23) * _Time.y * _Speed + length(q) * float2(4.1, 4.3));
                float2 o = fbm4_2(0.9 * q);
                o += 0.04 * sin(float2(0.12, 0.14) * _Time.y * _Speed + length(o));
                float2 n = fbm6_2(3.0 * o);
                ron = float4(o, n);
                float f = 0.5 + 0.5 * fbm4(1.8 * q + 6.0 * n);
                return lerp(f, f * f * f * 3.5, f * abs(n.x));
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                i.uv *= 2.0;
                float2 p = (2.0 * i.uv - 1.0) * float2(_ScreenParams.x / _ScreenParams.y, 1.0);                
                float e = 2.0 / _ScreenParams.y;
                float4 on = float4(0.0, 0.0, 0.0, 0.0);
                float f = func(p, on);
                
                float3 col = _Color1;
                col = lerp(col, _Color2, f);
                col = lerp(col, _Color3, f * 0.7);
                col = lerp(col, _Color4, 0.2 + 0.5*on.y*on.y);
                col = lerp(col, _Color5, 0.5*smoothstep(1.2,1.3,abs(on.z)+abs(on.w)));
                col = clamp( col*f*2.0, 0.0, 1.0 );
                
                // float4 col = float4(f, f, f, (1.0 / f) * 0.125);
                return float4(col * _Opacity, 1);
            }
            ENDCG
        }
    }
}
