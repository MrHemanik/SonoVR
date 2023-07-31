///Blatant Copy of BlendSlices, but with background transparency filter
Shader "Unlit/AfterglowBlend"
{
	Properties
	{
		texCount("texCount", Int) = 0
		[Toggle] useMax("useMax", Int) = 1
		[Toggle] useMask("useMask", Int) = 1
		maskTex("maskTex", 2D) = "white" {}
		
		[Toggle] bgColoring("bgColoring", Int) = 1
		bgColor("BG Color", Color) = (0.1,0.1,0.1,0)
		
		texArray_0("texArray_0", 2D) = "" {}
		texArray_1("texArray_1", 2D) = "" {}
		texArray_2("texArray_2", 2D) = "" {}
		texArray_3("texArray_3", 2D) = "" {}
		texArray_4("texArray_4", 2D) = "" {}
		texArray_5("texArray_5", 2D) = "" {}
		texArray_6("texArray_6", 2D) = "" {}
		texArray_7("texArray_7", 2D) = "" {}
		texArray_8("texArray_8", 2D) = "" {}
		texArray_9("texArray_9", 2D) = "" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			//BlendOp Max 

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

			uniform sampler2D maskTex;

			//sampler2D _bgTexture;
			uniform sampler2D texArray_0, texArray_1, texArray_2, texArray_3, texArray_4, texArray_5,
				              texArray_6, texArray_7, texArray_8, texArray_9, texArray_10;
			
			uniform int texCount;
			uniform int useMax;
			uniform int useMask;
			uniform int bgColoring;
			uniform float4 bgColor;

			float4 maskTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, maskTex); 

				return o;
			}
			
			fixed4 Combine(fixed4 col1, fixed4 col2)
			{
				if (useMax)
					return max(col1, col2);
				else
					return any(col2.rgb) ? col2 : col1;
			}

			fixed4 frag(v2f inp) : SV_Target
			{
				fixed4 result = 0;

				if (texCount > 0)
					result = tex2Dlod(texArray_0, float4(inp.uv, 0, 0));
				if (texCount > 1)
					result = Combine(result, tex2Dlod(texArray_1, float4(inp.uv, 0, 0)));
				if (texCount > 2)
					result = Combine(result, tex2Dlod(texArray_2, float4(inp.uv, 0, 0)));
				if (texCount > 3)
					result = Combine(result, tex2Dlod(texArray_3, float4(inp.uv, 0, 0)));
				if (texCount > 4)
					result = Combine(result, tex2Dlod(texArray_4, float4(inp.uv, 0, 0)));
				if (texCount > 5)
					result = Combine(result, tex2Dlod(texArray_5, float4(inp.uv, 0, 0)));
				if (texCount > 6)
					result = Combine(result, tex2Dlod(texArray_6, float4(inp.uv, 0, 0)));
				if (texCount > 7)
					result = Combine(result, tex2Dlod(texArray_7, float4(inp.uv, 0, 0)));
				if (texCount > 8)
					result = Combine(result, tex2Dlod(texArray_8, float4(inp.uv, 0, 0)));
				if (texCount > 9)
					result = Combine(result, tex2Dlod(texArray_9, float4(inp.uv, 0, 0)));


				if (bgColoring > 0 && all(result.rgb == 0))
				{
					result.rgba = bgColor.rgba;
				}

				// apply mask 
				if (useMask)
				{
					fixed4 mask = tex2Dlod(maskTex, float4(inp.uv.x, inp.uv.y, 0, 0));
					result.a *= mask.a; // apply alpha
					clip(-1 * (mask.a < 0.01)); // or clip
				}
				// set very dark colors to transparent
				if(all(result.rbg <= 0.1f))
				{				
					result.a = 0;
				}

				return result;

			}
			ENDCG
		}
	}
}
