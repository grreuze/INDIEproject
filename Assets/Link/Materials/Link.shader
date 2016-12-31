// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33149,y:32893,varname:node_4013,prsc:2|diff-1304-RGB,emission-956-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32258,y:32749,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:9832,x:32207,y:32914,varname:node_9832,prsc:2;n:type:ShaderForge.SFN_Multiply,id:956,x:32525,y:32852,varname:node_956,prsc:2|A-1304-RGB,B-9832-RGB;n:type:ShaderForge.SFN_Sin,id:1745,x:32319,y:33214,varname:node_1745,prsc:2|IN-6738-OUT;n:type:ShaderForge.SFN_Multiply,id:6738,x:32149,y:33247,varname:node_6738,prsc:2|A-6673-OUT,B-6814-OUT,C-9719-OUT;n:type:ShaderForge.SFN_TexCoord,id:6847,x:31247,y:32995,varname:node_6847,prsc:2,uv:0;n:type:ShaderForge.SFN_ComponentMask,id:2345,x:31447,y:33005,varname:node_2345,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-6847-U;n:type:ShaderForge.SFN_Tau,id:6673,x:31943,y:33154,varname:node_6673,prsc:2;n:type:ShaderForge.SFN_Add,id:6814,x:31732,y:33290,varname:node_6814,prsc:2|A-2345-OUT,B-6564-OUT;n:type:ShaderForge.SFN_Time,id:7666,x:31243,y:33288,varname:node_7666,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:53,x:32515,y:33229,varname:node_53,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-1745-OUT;n:type:ShaderForge.SFN_NormalVector,id:5463,x:32459,y:33454,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:1772,x:32761,y:33389,varname:node_1772,prsc:2|A-53-OUT,B-5463-OUT,C-5192-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5192,x:32470,y:33645,ptovrint:False,ptlb:node_5192,ptin:_node_5192,varname:node_5192,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3639,x:31243,y:33497,ptovrint:False,ptlb:node_3639,ptin:_node_3639,varname:node_3639,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:6564,x:31510,y:33410,varname:node_6564,prsc:2|A-7666-T,B-3639-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9719,x:31897,y:33481,ptovrint:False,ptlb:node_9719,ptin:_node_9719,varname:node_9719,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;proporder:1304-5192-3639-9719;pass:END;sub:END;*/

Shader "Atlas/Link" {
    Properties {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _node_5192 ("node_5192", Float ) = 1
        _node_3639 ("node_3639", Float ) = 0.1
        _node_9719 ("node_9719", Float ) = 3
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
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
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(0)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*i.vertexColor.rgb);
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
