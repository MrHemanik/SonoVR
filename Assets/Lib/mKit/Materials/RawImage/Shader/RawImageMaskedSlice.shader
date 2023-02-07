// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "mKitRawImage/MaskedSlice"
{
	Properties
	{
		_MainTex("_MainTex",2D) = "black" {}
		uAlphaThreshold("Alpha Threshold Mask", Int) = 0.1
		[Toggle] uUseMask("Use Slice Mask", Int) = 1
		uMaskTex("uMaskTex",2D) = "black" {}

	}

		SubShader
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

			Blend SrcAlpha OneMinusSrcAlpha

			// No culling or depth
			//Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				uniform float4 _Color;
		
				uniform sampler2D uMaskTex;
				uniform int uUseMask;	
				uniform float uAlphaThreshold;

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

				v2f vert (appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}
		

				fixed4 frag (v2f i) : SV_Target
				{
					// sample texture
					fixed4 col = tex2D(_MainTex, i.uv); // sample main tex

					float4 maskColor = tex2Dlod(uMaskTex, float4(i.uv.x, i.uv.y, 0, 0))  * uUseMask; // sample mask tex, optional by uUseMask
					
					// set alpha:=1 for mask areas above uAlphaThreshold
					col.a = (maskColor.a > uAlphaThreshold);

					return col;
				}
			ENDCG
		}
	}
}
