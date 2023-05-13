// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RhombusFlameShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlameGradient ("Flame Gradient", 2D) = "white" {}
        _FlameOpacity ("Flame Opacity", Range(0.0, 1.0)) = 1.0
        _WaveAmplitude ("Wave Amplitude", Range(0.0, 0.15)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0.0, 0.2)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off 
            Blend SrcAlpha OneMinusSrcAlpha
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
            sampler2D _FlameGradient;
            float4 _MainTex_ST;

            float _WaveAmplitude;
            float _WaveSpeed;

            float _FlameOpacity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Animate the flame vertices based on their y position and time
                float waveFrequency = 3.0;
                float waveSpeed = _WaveSpeed;
                float waveAmplitude = _WaveAmplitude;

                // Calculate the factor that depends on the y-coordinate
                float movementFactor = (v.vertex.y + 0.5f) / 1.0f;

                float time = sin(_Time.y);
                time /= 15;

                // -0.5f to center mesh to GameObject.
                float xOffset = sin(v.vertex.y * waveFrequency * waveSpeed + time) * waveAmplitude * movementFactor - 0.5f;
                float3 newPosition = v.vertex.xyz + float3(xOffset, 0, 0);
                o.vertex = UnityObjectToClipPos(newPosition);

                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the distance from the center of the texture (0.5, 0.5)
                float distanceFromCenter = distance(i.uv, float2(0.5, 0.5));

                // Normalize the distance to the range [0, 1]
                float normalizedDistance = saturate(distanceFromCenter * 2.0);
                normalizedDistance += 0.26f;

                fixed4 colorWhite = fixed4(1, 1, 1, 1);
                fixed4 colorOrange = fixed4(1, 0.5, 0, 1);
                fixed4 colorRed = fixed4(1, 0, 0, 1);

                // Interpolate between the colors based on the normalized distance
                fixed4 color;
                if (normalizedDistance < 1.0) {
                    color = lerp(colorWhite, colorOrange, (normalizedDistance - 0.2) * 2.0);
                } else {
                    color = lerp(colorOrange, colorRed, (normalizedDistance - 0.7) * 2.0);
                }

                color.a = lerp(1.0, 0.5, normalizedDistance);
                color.a *= _FlameOpacity;

                return color;
            }
            ENDCG
        }
    }
}
