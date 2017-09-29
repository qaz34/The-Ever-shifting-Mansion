// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ECG"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OriginX("OriginX", float) = 0.5
		_OriginY("OriginY", float) = 0.5
		_Scale("Scale", float) = 3
	    _Timer("Timer", float) = 0
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
			float _OriginX;
			float _OriginY;
			float _Scale;
			float _Timer;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				/* artifical sine wave
				float longWave = 0.5f * (1.0f + sin(_Timer + 2.0f * i.uv.x)); // between 0 and 1
				float shortWave = (1.0f + sin(_Timer + 20 * i.uv.x));
				float y0 = 0.25f + 0.25f * shortWave * longWave * longWave;


				float intensity = (i.uv.y - y0);
				intensity = intensity * intensity;

				intensity = 1 - intensity;

				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;

				fixed4 col = fixed4(intensity, intensity, intensity, 1);*/

				float xBright = fmod(_Timer, 2.0f);
				
				float trail = xBright - i.uv.x;
				if (trail < 0)
				{
					//trail = 0;
				}
				trail = cos(trail);

				float intensity = trail*trail;
				intensity = 1 - intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = intensity * intensity;
				intensity = 10 * intensity * intensity;

				fixed4 green = fixed4(intensity*0.1f, intensity, intensity*0.1f, 1) * tex2D(_MainTex, i.uv);
				fixed4 red = fixed4(intensity * 0.1f, 0, 0, 1) * tex2D(_MainTex, i.uv);

				fixed4 col = red;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);				
				return col;
			}
			ENDCG
		}
	}
}
