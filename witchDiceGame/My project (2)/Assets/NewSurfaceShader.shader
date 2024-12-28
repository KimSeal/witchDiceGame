Shader "Custom/BlueOverlayWithTransparency"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" { }
        _BlueColor("Blue Color", Color) = (0, 0, 1, 1)
        _Transparency("Transparency", Range(0, 1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            Pass
            {
                // Z-buffer를 사용하지 않도록 ZWrite를 끄고, ZTest 설정
                ZWrite Off  // Z-buffer 쓰기 비활성화
                ZTest LEqual // 깊이 테스트 (현재 Z 값보다 작거나 같은 값만 통과)
                Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 설정

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _BlueColor;
                float _Transparency;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    // 텍스처에서 색상과 알파값 가져오기
                    half4 col = tex2D(_MainTex, i.uv);

                    // 텍스처의 알파(투명도)가 0보다 크다면 파란색을 덮기
                    if (col.a > 0.0)
                    {
                        // 파란색 덮을 부분의 투명도 적용
                        half4 blueOverlay = _BlueColor;
                        blueOverlay.a *= _Transparency; // 투명도 적용

                        // 기존 텍스처 색상과 파란색을 섞어줌
                        col = lerp(col, blueOverlay, blueOverlay.a);
                    }

                    // 알파값이 0인 경우 원래 알파를 그대로 유지
                    col.a = max(col.a, tex2D(_MainTex, i.uv).a);

                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}



/*
Shader "Custom/BlueOverlayWithTransparency"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" { }
        _BlueColor("Blue Color", Color) = (0, 0, 1, 1)
        _Transparency("Transparency", Range(0, 1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 설정
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _BlueColor;
                float _Transparency;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    // 텍스처에서 색상과 알파값 가져오기
                    half4 col = tex2D(_MainTex, i.uv);

                    // 텍스처의 알파(투명도)가 0보다 크다면 파란색을 덮기
                    if (col.a > 0.0)
                    {
                        // 파란색 덮을 부분의 투명도 적용
                        half4 blueOverlay = _BlueColor;
                        blueOverlay.a *= _Transparency; // 투명도 적용

                        // 기존 텍스처 색상과 파란색을 섞어줌
                        col = lerp(col, blueOverlay, blueOverlay.a);
                    }

                    // 알파값이 0인 경우 원래 알파를 그대로 유지
                    // 이 부분이 투명한 부분을 그대로 유지하게 합니다.
                    col.a = max(col.a, tex2D(_MainTex, i.uv).a);

                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
*/