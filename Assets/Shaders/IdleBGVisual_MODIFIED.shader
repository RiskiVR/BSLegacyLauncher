Shader "!M.O.O.N/IdleBGVisualizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color1("Color 1", Color) = (1,1,1,1)
		_Color2("Color 2", Color) = (1,1,1,1)
		_Color3("Color 3", Color) = (1,1,1,1)
		_AMT("Amt", Vector) = (1,1,1,1)
        _Transparency("Transparency", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Transparency;

			float3 _Color1, _Color2, _Color3;

			float BLOCK_WIDTH = 0;
			float3 _AMT;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }



            float4 frag (v2f i) : SV_Target
			{

				float3 COLOR1 = _Color1.rgb;
				float3 COLOR2 = _Color2.rgb;
				float2 uv = i.uv;

				// To create the BG pattern
				float3 final_color = _Color3;
				float3 bg_color = 0;
				float3 wave_color = 0;



				// To create the waves
				float wave_width = 0.01;
				uv = -1.0 + 2.0 * uv;
				uv.y += 0.1;
				for (int i = 0; i < _AMT.y; i++) {

					uv.y += (0.07 * sin(uv.x + i / _AMT.x + _Time.y));
					wave_width = abs(1.0 / (150.0 * uv.y));
					wave_color += float3(wave_width * 1.9, wave_width, wave_width * 1.5) * _Color1;
				}

				final_color = wave_color;


				return saturate(float4(final_color, _Transparency * (final_color.r + final_color.g + final_color.b)));

			}

            ENDCG
        }
    }
}
