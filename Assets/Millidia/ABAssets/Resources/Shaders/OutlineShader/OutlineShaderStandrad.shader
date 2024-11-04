Shader "MILLIDIA/OutlineShaderStandrad"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        
		 // 新增的描边属性
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Range(0, 0.05)) = 0.015
        
        // 新增的自发光属性
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)  // 自发光颜色
        _EmissionTex ("Emission (RGB)", 2D) = "black" {}      // 自发光贴图
        _UseEmission ("Use Emission", Float) = 0.0            //
        
        _ColorFactor("ColorFactor" , Float) = 1.0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _EmissionTex;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _UseEmission;
        half _ColorFactor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * _ColorFactor;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            
            // 处理自发光效果，如果启用则使用自发光贴图和颜色
            if (_UseEmission > 0)
            {
                fixed4 emission = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionColor;
                o.Emission = emission.rgb;
            }
        }
        ENDCG


        
		  Pass {
        Name "OUTLINE"
        Tags { "LightMode" = "Always" }

        Cull Front // 反转剔除方式，来渲染物体的背面

        ZWrite On
        ZTest LEqual
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        uniform float _OutlineThickness;
        uniform float4 _OutlineColor;

        struct appdata_t {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };

        struct v2f {
            float4 pos : POSITION;
            float4 color : COLOR;
        };

        v2f vert(appdata_t v)
        {
            // 法线方向膨胀顶点
            v2f o;
            float3 norm = normalize(v.normal);
            o.pos = UnityObjectToClipPos(v.vertex + norm * _OutlineThickness);
            o.color = _OutlineColor;
            return o;
        }

        half4 frag(v2f i) : COLOR
        {
            // 使用插值让描边颜色更加平滑过渡
            half edgeFactor = smoothstep(0.0, 1.0, i.color.a);
            return half4(i.color.rgb * edgeFactor, i.color.a);
        }
        ENDCG
        
        
    }
    }
    FallBack "Diffuse"
}
