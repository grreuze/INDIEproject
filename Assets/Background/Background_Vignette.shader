// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3310,x:32871,y:32712,varname:node_3310,prsc:2|emission-3283-OUT;n:type:ShaderForge.SFN_Color,id:5979,x:32068,y:32711,ptovrint:False,ptlb:node_5979,ptin:_node_5979,varname:node_5979,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4,c2:0.4,c3:0.4,c4:1;n:type:ShaderForge.SFN_TexCoord,id:6915,x:31524,y:32930,varname:node_6915,prsc:2,uv:0;n:type:ShaderForge.SFN_Clamp01,id:9527,x:32228,y:32918,varname:node_9527,prsc:2|IN-5226-OUT;n:type:ShaderForge.SFN_RemapRange,id:9325,x:32438,y:32884,varname:node_9325,prsc:2,frmn:0,frmx:1,tomn:0.8,tomx:0.4|IN-9527-OUT;n:type:ShaderForge.SFN_Multiply,id:3283,x:32665,y:32753,varname:node_3283,prsc:2|A-5979-RGB,B-9325-OUT;n:type:ShaderForge.SFN_Length,id:5226,x:32033,y:32947,varname:node_5226,prsc:2|IN-4599-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:4599,x:31912,y:33182,varname:node_4599,prsc:2|IN-6915-UVOUT,IMIN-5063-OUT,IMAX-1949-OUT,OMIN-6829-OUT,OMAX-5331-OUT;n:type:ShaderForge.SFN_Vector1,id:5063,x:31477,y:33137,varname:node_5063,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:1949,x:31477,y:33198,varname:node_1949,prsc:2,v1:1;n:type:ShaderForge.SFN_Time,id:9810,x:30810,y:33295,varname:node_9810,prsc:2;n:type:ShaderForge.SFN_Sin,id:3669,x:31166,y:33292,varname:node_3669,prsc:2|IN-2377-OUT;n:type:ShaderForge.SFN_Multiply,id:6829,x:31680,y:33342,varname:node_6829,prsc:2|A-4464-OUT,B-1655-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1655,x:31333,y:33292,varname:node_1655,prsc:2,min:-1,max:1|IN-3669-OUT;n:type:ShaderForge.SFN_Vector1,id:4464,x:31333,y:33231,varname:node_4464,prsc:2,v1:-1;n:type:ShaderForge.SFN_Multiply,id:5331,x:31680,y:33472,varname:node_5331,prsc:2|A-1949-OUT,B-1655-OUT;n:type:ShaderForge.SFN_Multiply,id:2377,x:31000,y:33292,varname:node_2377,prsc:2|A-1437-OUT,B-9810-T;n:type:ShaderForge.SFN_ValueProperty,id:1437,x:30791,y:33189,ptovrint:False,ptlb:node_1437,ptin:_node_1437,varname:node_1437,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;proporder:5979-1437;pass:END;sub:END;*/

Shader "Custom/Background_Vignette" {
    Properties {
        _node_5979 ("node_5979", Color) = (0.4,0.4,0.4,1)
        _node_1437 ("node_1437", Float ) = 0.3
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _node_5979;
            uniform float _node_1437;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_5063 = 0.0;
                float node_1949 = 1.0;
                float4 node_9810 = _Time + _TimeEditor;
                float node_3669 = sin((_node_1437*node_9810.g));
                float node_1655 = clamp(node_3669,-1,1);
                float node_6829 = ((-1.0)*node_1655);
                float3 emissive = (_node_5979.rgb*(saturate(length((node_6829 + ( (i.uv0 - node_5063) * ((node_1949*node_1655) - node_6829) ) / (node_1949 - node_5063))))*-0.4+0.8));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
