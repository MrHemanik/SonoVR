Shader "Unlit/TwoSidedTex"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
  	    _BgColor("Background Color", Color) = (0.2,0.2,0.2,1)
	    _Color("Tint Color", Color) = (1,1,1,1)

		[Toggle] uUseMask("Use Slice Mask", Int) = 1
		uMaskTex("uMaskTex", 2D) = "black" {}
		uOverlayTex("uOverlayTex", 2D) = "black" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "AlphaTest" }

		//Blend SrcAlpha OneMinusSrcAlpha
		
		LOD 100

		Cull Off
		
		ZWrite On 

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D uOverlayTex;
			sampler2D uMaskTex;

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;

			float4 _BgColor;
			float4 _Color;
	
			int uUseMask;
			CBUFFER_END

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
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 col = tex2D(_MainTex, i.uv);
			
				// tint
				col.rgb *= _Color;

				// bg coloring
				if (all(col.rgb == 0))
					col.rgb = _BgColor.rgb;

				// overlay
				float4 overlayCol = tex2D(uOverlayTex, i.uv);
				col = (1 - overlayCol.a) * col + (overlayCol.a * overlayCol);

				// mask
				float4 maskColor = tex2Dlod(uMaskTex, float4(i.uv.x, i.uv.y, 0, 0)) * uUseMask; // sample mask tex, optional by uUseMask
				col.a = maskColor.a > 0 ? _Color.a : 0;
	/*			if (overlayCol.a > 0)
					col = overlayCol;*/

				// clip
				clip(-1 * (col.a==0));

				return col;
			}
			ENDHLSL
		}

		
	}
}
