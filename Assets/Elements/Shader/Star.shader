
Shader "Custom/Star" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Make_value_positive ("Make_value_positive", Float ) = -0.05
        _Minopacity ("Min opacity", Float ) = 0.1
        _Maxopacity ("Max opacity", Float ) = 1
        _Position_addition ("Position_addition", Range(-100, 100)) = -20
        _Outline_Width ("Outline_Width", Range(0, 1)) = 0.3
        _Outline_Color ("Outline_Color", Color) = (1,1,1,1)
        _Atmospheric_Opacity_or_Opaque ("Atmospheric_Opacity_or_Opaque", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _Make_value_positive;
			uniform float _Outline_Width;
            uniform float _Minopacity;
            uniform float _Maxopacity;
            uniform float _Position_addition;
            uniform float4 _Outline_Color;
			uniform float _Atmospheric_Opacity_or_Opaque;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + normalize(v.vertex) * _Outline_Width,1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
				float3 emissive = _Outline_Color.rgb;
				float3 finalColor = emissive;
				float node_3817 = clamp(((i.posWorld.b + _Position_addition)*_Make_value_positive),_Minopacity,_Maxopacity);
				fixed4 finalRGBA = fixed4(finalColor,lerp(node_3817,1.0,_Atmospheric_Opacity_or_Opaque));
				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Make_value_positive;
            uniform float _Minopacity;
            uniform float _Maxopacity;
            uniform float _Position_addition;
            uniform float _Atmospheric_Opacity_or_Opaque;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float3 finalColor = emissive;
                float node_3817 = clamp(((i.posWorld.b+_Position_addition)*_Make_value_positive),_Minopacity,_Maxopacity);
                fixed4 finalRGBA = fixed4(finalColor,lerp(node_3817,1.0,_Atmospheric_Opacity_or_Opaque));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}