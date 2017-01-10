// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33149,y:32893,varname:node_4013,prsc:2|diff-1304-RGB,emission-956-OUT,alpha-1104-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32258,y:32749,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:9832,x:32207,y:32914,varname:node_9832,prsc:2;n:type:ShaderForge.SFN_Multiply,id:956,x:32525,y:32852,varname:node_956,prsc:2|A-1304-RGB,B-9832-RGB;n:type:ShaderForge.SFN_FragmentPosition,id:7847,x:32353,y:33210,varname:node_7847,prsc:2;n:type:ShaderForge.SFN_Clamp,id:1104,x:32808,y:33194,varname:node_1104,prsc:2|IN-7847-Z,MIN-651-OUT,MAX-1504-OUT;n:type:ShaderForge.SFN_Vector1,id:651,x:32334,y:33389,varname:node_651,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:1504,x:32334,y:33439,varname:node_1504,prsc:2,v1:0;proporder:1304;pass:END;sub:END;*/

Shader "Atlas/Link" {
    Properties {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
		_Make_value_positive("Make_value_positive", Float) = -0.05
		_Minopacity("Min opacity", Float) = 0.1
		_Maxopacity("Max opacity", Float) = 1
		_Position_addition("Position_addition", Range(-100, 100)) = -20
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
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

            struct VertexInput {
                float4 vertex : POSITION;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
				float node_3817 = clamp(((i.posWorld.b + _Position_addition)*_Make_value_positive), _Minopacity, _Maxopacity);
                fixed4 finalRGBA = fixed4(finalColor,lerp(node_3817, 1.0, 0));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
