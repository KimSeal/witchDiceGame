Shader "Custom/colorCoverOutline"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}

        [Header(Overlay Settings)]
        _OverlayColor("Overlay Color", Color) = (0, 0, 1, 1)
        _Transparency("Transparency", Range(0, 1)) = 0.5

        [Header(Outline Settings)]
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _Radius("Outline Radius", Range(0, 10)) = 1
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

            // 투명 처리를 위한 설정
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
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
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;

                float4 _OverlayColor;
                float _Transparency;

                float4 _OutlineColor;
                float _Radius;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // 1. 원본 텍스처 색상 가져오기
                    fixed4 originalCol = tex2D(_MainTex, i.uv);

                // 2. 외곽선 계산을 위한 주변 알파값 탐색 (PixelOutline_reddit 로직)
                float na = 0;
                int r = (int)_Radius; // 반복문을 위해 int로 캐스팅

                for (int nx = -r; nx <= r; nx++)
                {
                    for (int ny = -r; ny <= r; ny++)
                    {
                        if (nx * nx + ny * ny <= r * r) // 반지름 조건 (기존 코드 오류 수정: r -> r*r)
                        {
                            fixed4 nc = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x * nx, _MainTex_TexelSize.y * ny));
                            na += ceil(nc.a);
                        }
                    }
                }

                // 주변에 알파가 있는지 체크 (0 또는 1로 클램프)
                na = clamp(na, 0, 1);

                // 현재 픽셀이 이미 원본 그림 내부라면 외곽선 대상에서 제외
                float isOutline = clamp(na - ceil(originalCol.a), 0, 1);

                // 3. 내부 오버레이 계산 (BlueOverlayWithTransparency 로직)
                fixed4 finalCol = originalCol;
                if (originalCol.a > 0.0)
                {
                    fixed4 overlay = _OverlayColor;
                    overlay.a *= _Transparency;

                    // 원본 색상과 오버레이 색상을 블렌딩
                    finalCol = lerp(originalCol, overlay, overlay.a);

                    // 기존 원본의 알파값을 유지하거나 더 높은 투명도로 유지
                    finalCol.a = max(originalCol.a, finalCol.a);
                }

                // 4. 최종 색상 결정: 내부 영역이면 오버레이된 색상, 외곽선 영역이면 외곽선 색상 적용
                return lerp(finalCol, _OutlineColor, isOutline);
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}
