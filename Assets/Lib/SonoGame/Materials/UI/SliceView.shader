Shader "Unlit/SliceView"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        LineTex("Blue Line", 2D) = "white" {}
        [Toggle] renderContent("renderContent", Int) = 1
        [Toggle] bgColoring("bgColoring", Int) = 1
        bgColor("BG Color", Color) = (0.1,0.1,0.1,1)
        [Toggle] useLine("useLine", Int) = 1
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha 
        
        Cull off


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
                UNITY_VERTEX_INPUT_INSTANCE_ID // XR STEREO
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

               UNITY_VERTEX_OUTPUT_STEREO //  XR STEREO
            };

            sampler2D _MainTex;
            sampler2D LineTex;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            int renderContent;
            int bgColoring;
            int useLine;
            float4 bgColor;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
              
                UNITY_SETUP_INSTANCE_ID(v); //  XR STEREO
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the source textures
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 lineCol = tex2D(LineTex, i.uv);
                lineCol.a = useLine * lineCol.a; // toggle line overlay
        
                // background coloring
                if (bgColoring > 0 && all(col.rgb == 0))
                {
                    col.rgb = bgColor.rgb;
                }

                if (renderContent == 0)
                    col.rgb = bgColor.rgb;

                // line overlay
                col = (1 - lineCol.a) * col + (lineCol.a * lineCol); // equals if (lineCol.a > 0) col = lineCol;

                // clip unused pixels
                clip (-1 * (col.a < 0.5));

                return col;
            }
            ENDCG
        }
    }

   
}
