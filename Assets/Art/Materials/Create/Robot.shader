Shader "Custom/Robot"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo2 (RGB)", 2D) = "white" {}
        _BumpMap ("Normal", 2D) = "white" {}
        _BumpMap2 ("Normal2", 2D) = "white" {}
        _NoiseTex ("Noise",2D) = "white" {}
        _MaskTex ("Mask",2D) = "white" {}
        _MaskTex2 ("Mask2",2D) = "white" {}
        _RampTex ("Ramp",2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Specular ("Specular", 2D) = "white" {}
        _Cut ("Dissolve", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MainTex2;
        sampler2D _BumpMap;
        sampler2D _BumpMap2;
        sampler2D _MaskTex;
        sampler2D _MaskTex2;
        sampler2D _RampTex;
        sampler2D _NoiseTex;
        sampler2D _Specular;
        float _Cut;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MainTex2;
            float2 uv_BumpMap;
            float2 uv_BumpMap2;
            float2 uv_MaskTex;
            float2 uv_MaskTex2;
            float2 uv_RampTex;
            float2 uv_NoiseTex;
            float2 uv_Specular;
        };

        half _Glossiness;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);//기본텍스쳐
            fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2);//용암텍스쳐
            half3 defaultNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));//기본 노말
            half3 lavaNormal = UnpackNormal(tex2D(_BumpMap2, IN.uv_BumpMap2));//용암 노말
            
            float4 noise=tex2D(_NoiseTex,IN.uv_NoiseTex);//점점 쌓이는것
            float4 m = tex2D (_MaskTex, IN.uv_MaskTex);//눈 반짝이는 부위
            float4 m2 = tex2D (_MaskTex2, IN.uv_MaskTex2);//용암 부위 빛나는 부위
            float4 Ramp=tex2D(_RampTex, float2(_Time.y,0.5));// 반짝이는 간격
            

            float alpha;
            if(noise.r>=_Cut)
            {
                alpha=1;
            }
            else
            {
                alpha=0;
            }
            o.Albedo=lerp(c.rgb,d.rgb,alpha);
            o.Normal=lerp(defaultNormal,lavaNormal,alpha);
            float3 defaultEmission=c.rgb*m.r*Ramp.r;
            float3 lavaEmission=d.rgb*m2.r;
            float3 d2lEmission=defaultEmission+lavaEmission;
            //o.Emission=defaultEmission;

            //o.Albedo = c.rgb*alpha+d.rgb*(1-alpha);
            //o.Normal=defaultNormal*alpha+lavaNormal*(1-alpha);

            o.Specular=tex2D (_Specular, IN.uv_Specular);
            // Metallic and smoothness come from slider variables
            o.Smoothness = _Glossiness;
            
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
