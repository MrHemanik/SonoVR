Shader "Sonogame/SurfaceClip"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        //_ClipPlane("Clip Plane (Normal XYZ + Distance)", Vector) = (0,1,0,0)

        _CutoffColor("Cutoff Color", Color) = (1,0,0,1)
        _Emission("_Emission Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "AlphaTest"}
        LOD 200
        //Cull Back // Off

        Pass {
            ZWrite On
            ZTest LEqual
            ColorMask 0
        }
            Blend Zero One
            BlendOp Add

        ZTest LEqual
        

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types // 
        #pragma surface surf Standard fullforwardshadows alpha:blend
        //#pragma surface surf Lambert alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        float4 _ClipPlane;
        float4 _CutoffColor;
        int clipMode; // 0: unclipped, 1: clip both, 2: clip half, 3: clip + alpha

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            //float facing : VFACE;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        //void surf(Input IN, inout SurfaceOutput o)
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // clipMode 1 : show nothing
            clip(-(clipMode == 1));

            //float facing = IN.facing * 0.5 + 0.5; // convert from [-1..1] to [0..1]
            //float facing = 1;

            // Albedo comes from a texture tinted by color
            fixed4 c = _Color; // tex2D(_MainTex, IN.uv_MainTex)* _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            o.Alpha = 1; 
                    
            // discard fragment based on clip plane
            float distance = dot(IN.worldPos, _ClipPlane.xyz);
            distance = distance + _ClipPlane.w; // .w is distance from center 
            
            if (distance > 0 && clipMode > 1) 
            {         
                float intensity = 0.7f;
                //o.Emission = float3(0.5, 0.5, 0.5);
                o.Albedo = float3(intensity, intensity, intensity);
                o.Alpha = (clipMode == 2 ? 0 : intensity); // clip mode 2 and 3
            }
           
        }
        ENDCG
    }
    FallBack "Diffuse"
}
