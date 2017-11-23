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
		_Color("Color", Color) = (0,1,0,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" }
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

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
				fixed4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
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

					fixed4 col;
					col = _Color * intensity * tex2D(_MainTex, i.uv);
					//col.a = 1;

					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);				
					return col;
				}
				ENDCG
			}
		}
}
