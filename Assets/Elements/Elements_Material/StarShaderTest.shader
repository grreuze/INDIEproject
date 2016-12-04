// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2865,x:32794,y:32756,varname:node_2865,prsc:2|emission-6665-RGB,alpha-2341-OUT,olwid-2288-OUT,olcol-596-RGB;n:type:ShaderForge.SFN_Color,id:6665,x:32238,y:32601,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_FragmentPosition,id:6841,x:31512,y:32675,varname:node_6841,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8823,x:31836,y:32895,varname:node_8823,prsc:2|A-238-OUT,B-9014-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9014,x:31590,y:32964,ptovrint:False,ptlb:Make_value_positive,ptin:_Make_value_positive,varname:node_9014,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.05;n:type:ShaderForge.SFN_Clamp,id:3817,x:32082,y:32878,varname:node_3817,prsc:2|IN-8823-OUT,MIN-6835-OUT,MAX-6940-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6835,x:31767,y:33062,ptovrint:False,ptlb:Min opacity,ptin:_Minopacity,varname:node_6835,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:6940,x:31782,y:33141,ptovrint:False,ptlb:Max opacity,ptin:_Maxopacity,varname:node_6940,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:238,x:31667,y:32790,varname:node_238,prsc:2|A-6841-Z,B-9448-OUT;n:type:ShaderForge.SFN_Slider,id:9448,x:31283,y:32847,ptovrint:False,ptlb:Position_addition,ptin:_Position_addition,varname:node_9448,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-100,cur:-20,max:100;n:type:ShaderForge.SFN_Color,id:596,x:32392,y:33418,ptovrint:False,ptlb:Outline_Color,ptin:_Outline_Color,varname:node_596,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Slider,id:5308,x:31970,y:33403,ptovrint:False,ptlb:Outline_Width,ptin:_Outline_Width,varname:node_5308,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:-1,max:1;n:type:ShaderForge.SFN_Lerp,id:2341,x:32428,y:32998,varname:node_2341,prsc:2|A-3817-OUT,B-8067-OUT,T-6420-OUT;n:type:ShaderForge.SFN_Vector1,id:8067,x:32093,y:33019,varname:node_8067,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6420,x:32103,y:33127,ptovrint:False,ptlb:Atmospheric_Opacity_or_Opaque,ptin:_Atmospheric_Opacity_or_Opaque,varname:node_6420,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_RemapRange,id:2288,x:32325,y:33157,varname:node_2288,prsc:2,frmn:0.1,frmx:1,tomn:-1,tomx:0.3|IN-3817-OUT;proporder:6665-9014-6835-6940-9448-5308-596-6420;pass:END;sub:END;*/

Shader "Shader Forge/StarShaderTest" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Make_value_positive ("Make_value_positive", Float ) = -0.05
        _Minopacity ("Min opacity", Float ) = 0.1
        _Maxopacity ("Max opacity", Float ) = 1
        _Position_addition ("Position_addition", Range(-100, 100)) = -20
        _Outline_Width ("Outline_Width", Range(-1, 1)) = -1
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
            uniform float _Minopacity;
            uniform float _Maxopacity;
            uniform float _Position_addition;
            uniform float4 _Outline_Color;
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
                float node_3817 = clamp(((mul(unity_ObjectToWorld, v.vertex).b+_Position_addition)*_Make_value_positive),_Minopacity,_Maxopacity);
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + normalize(v.vertex)*(node_3817*1.444444+-1.144444),1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                return fixed4(_Outline_Color.rgb,0);
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
