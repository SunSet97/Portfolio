Shader "Custom/RobotwithFire"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo2 (RGB)", 2D) = "white" {}
        _BumpMap ("Normal", 2D) = "white" {}
        _BumpMap2 ("Normal2", 2D) = "white" {}
        _Specular1("Specular1", 2D) = "white" {}
        _Specular2("Specular2", 2D) = "white" {}
        _NoiseTex ("Noise",2D) = "white" {}
        _MaskTex ("Mask",2D) = "white" {}
        _MaskTex2 ("Mask2",2D) = "white" {}
        _MaskTex3 ("Mask3",2D) = "white" {}
        //_Occlusion("Occlusion", 2D) = "white" {}


        _SpeCol ("Specular Color", Color) =(1,1,1,1)
        _SpecPow("Specular Power", Range(10,200))=100
        _Flow("Flow",Range(0,1)) = 0
        _Cut ("Dissolve", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf SpecularGloss fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 5.0

        sampler2D _MainTex;
        sampler2D _MainTex2;
        sampler2D _BumpMap;
        sampler2D _BumpMap2;
        sampler2D _Specular1;
        sampler2D _Specular2;
        sampler2D _MaskTex;
        sampler2D _MaskTex2;
        sampler2D _MaskTex3;
        sampler2D _NoiseTex;
        float _Cut;
        float _Flow;
        float4 _SpeCol;
        float _SpecPow;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MainTex2;
            float2 uv_BumpMap;
            float2 uv_BumpMap2;
            float2 uv_Specular1;
            float2 uv_Specular2;
            float2 uv_MaskTex;
            float2 uv_MaskTex2;
            float2 uv_MaskTex3;
            float2 uv_NoiseTex;
            INTERNAL_DATA

        };

        void vert (inout appdata_full v)
        {  
            v.vertex.xyz=v.vertex.xyz+v.normal.xyz*_Cut*0.008f;

        }

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);//기본텍스쳐
            fixed4 d = tex2D (_MainTex2, IN.uv_MainTex2);//용암텍스쳐
            half3 defaultNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));//기본 노말
            half3 lavaNormal = UnpackNormal(tex2D(_BumpMap2, IN.uv_BumpMap2));//용암 노말
            float4 defaultSp= tex2D(_Specular1,IN.uv_Specular1);
            float4 lavatSp= tex2D(_Specular2,IN.uv_Specular2);
            float4 noise=tex2D(_NoiseTex,IN.uv_NoiseTex);//점점 쌓이는것
            float4 m = tex2D (_MaskTex, IN.uv_MaskTex);//눈 반짝이는 부위
            float4 m2 = tex2D (_MaskTex2, float2(IN.uv_MaskTex2.x , IN.uv_MaskTex2.y - _Time.y*_Flow));//용암 부위 빛나는 부위
            float4 m3 = tex2D (_MaskTex3, IN.uv_MaskTex3);//변하지 않을 부위
            

            float alpha;
            if(noise.r>=_Cut)
            {
                alpha=1;
            }
            else
            {
                alpha=0;
            }
            float3 stayMask = c.rgb * m3.r;
            o.Albedo=stayMask+lerp(d.rgb,c.rgb,alpha);
            
            o.Normal=lerp(lavaNormal,defaultNormal,alpha)+defaultNormal;
            float3 defaultEmission=(c.rgb*m.r);
            float3 lavaEmission=(d.rgb*m2.r);
            o.Emission=lerp(lavaEmission,defaultEmission,alpha)+defaultEmission;
            o.Gloss=lerp(lavatSp.a,defaultSp.a,alpha);
            o.Alpha = c.a;
        }
        float4 LightingSpecularGloss(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            float4 final;

            //lmabert term
            float3 DiffColor;
            float ndotl= saturate(dot(s.Normal, lightDir));
            DiffColor= ndotl * s.Albedo *_LightColor0.rgb * atten;

            //Spec Term
            float3 SpecColor;
            float3 H= normalize(lightDir+viewDir);
            float spec= saturate(dot(H,s.Normal));
            spec= pow(spec, _SpecPow);
            SpecColor= spec * _SpeCol.rgb*s.Gloss;

            //final term
            final.rgb=DiffColor.rgb+SpecColor.rgb;
            final.a=s.Alpha;
            return final;

        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
