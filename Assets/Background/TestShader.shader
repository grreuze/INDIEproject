// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:36305,y:32089,varname:node_1873,prsc:2|emission-4294-OUT;n:type:ShaderForge.SFN_Color,id:5279,x:32652,y:32213,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:node_5279,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5620674,c2:0.6862785,c3:0.8308824,c4:1;n:type:ShaderForge.SFN_Color,id:9297,x:32473,y:32433,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_9297,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0.3,c4:1;n:type:ShaderForge.SFN_Lerp,id:6484,x:32884,y:32503,varname:node_6484,prsc:2|A-5279-RGB,B-9297-RGB,T-2879-RGB;n:type:ShaderForge.SFN_Tex2d,id:2879,x:32452,y:32694,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_2879,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:2,isnm:False|UVIN-9825-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7621,x:32037,y:32655,varname:node_7621,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:9825,x:32286,y:32673,varname:node_9825,prsc:2,spu:0,spv:0.1|UVIN-7621-UVOUT;n:type:ShaderForge.SFN_Divide,id:1610,x:33586,y:32361,varname:node_1610,prsc:2|A-6484-OUT,B-48-OUT;n:type:ShaderForge.SFN_ValueProperty,id:48,x:33165,y:32599,ptovrint:False,ptlb:Cloud_Texture_Darkening,ptin:_Cloud_Texture_Darkening,varname:node_48,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:4640,x:32657,y:31713,varname:node_4640,prsc:2;n:type:ShaderForge.SFN_Color,id:8911,x:33752,y:31633,ptovrint:False,ptlb:Noise_Color1,ptin:_Noise_Color1,varname:node_8911,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:5849,x:33752,y:31795,ptovrint:False,ptlb:Noise_Color2,ptin:_Noise_Color2,varname:node_5849,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:2991,x:34150,y:31926,varname:node_2991,prsc:2|A-8911-RGB,B-5849-RGB,T-8992-V;n:type:ShaderForge.SFN_TexCoord,id:8992,x:33090,y:32167,varname:node_8992,prsc:2,uv:0;n:type:ShaderForge.SFN_Subtract,id:8842,x:34150,y:31705,varname:node_8842,prsc:2|A-6154-OUT,B-2991-OUT;n:type:ShaderForge.SFN_Vector1,id:6154,x:33949,y:31779,varname:node_6154,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:8541,x:34224,y:31530,varname:node_8541,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9082-OUT;n:type:ShaderForge.SFN_Sin,id:9082,x:33937,y:31416,varname:node_9082,prsc:2|IN-4640-TDB;n:type:ShaderForge.SFN_Lerp,id:392,x:34609,y:31565,varname:node_392,prsc:2|A-8842-OUT,B-2991-OUT,T-8541-OUT;n:type:ShaderForge.SFN_Multiply,id:6533,x:34785,y:32016,varname:node_6533,prsc:2|A-392-OUT,B-4862-OUT;n:type:ShaderForge.SFN_Lerp,id:538,x:35436,y:32088,varname:node_538,prsc:2|A-1904-OUT,B-1610-OUT,T-2480-OUT;n:type:ShaderForge.SFN_Divide,id:8741,x:34869,y:31737,varname:node_8741,prsc:2|A-392-OUT,B-4862-OUT;n:type:ShaderForge.SFN_Lerp,id:5952,x:35032,y:31978,varname:node_5952,prsc:2|A-8741-OUT,B-6533-OUT,T-9902-OUT;n:type:ShaderForge.SFN_RemapRange,id:9902,x:34569,y:31785,varname:node_9902,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:0.8|IN-8541-OUT;n:type:ShaderForge.SFN_Multiply,id:5086,x:35660,y:32088,varname:node_5086,prsc:2|A-538-OUT,B-4317-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4317,x:35436,y:32259,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_4317,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1769,x:33686,y:32002,varname:node_1769,prsc:2|A-2857-OUT,B-98-UVOUT;n:type:ShaderForge.SFN_Noise,id:4862,x:34173,y:32078,varname:node_4862,prsc:2|XY-5105-OUT;n:type:ShaderForge.SFN_Slider,id:2480,x:34939,y:32226,ptovrint:False,ptlb:Noise or Clouds,ptin:_NoiseorClouds,varname:node_2480,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Floor,id:5105,x:33903,y:32133,varname:node_5105,prsc:2|IN-1769-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2857,x:33371,y:31969,ptovrint:False,ptlb:Noise_control,ptin:_Noise_control,varname:node_2857,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:100;n:type:ShaderForge.SFN_Lerp,id:4294,x:36073,y:32147,varname:node_4294,prsc:2|A-5086-OUT,B-256-RGB,T-4909-OUT;n:type:ShaderForge.SFN_Slider,id:4909,x:35447,y:32381,ptovrint:False,ptlb:Base_Color_Strength,ptin:_Base_Color_Strength,varname:node_4909,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Color,id:256,x:35757,y:32452,ptovrint:False,ptlb:Base_Color,ptin:_Base_Color,varname:node_256,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2,c2:0.2,c3:0.2,c4:1;n:type:ShaderForge.SFN_ConstantClamp,id:1904,x:35256,y:31943,varname:node_1904,prsc:2,min:0,max:1.5|IN-5952-OUT;n:type:ShaderForge.SFN_Panner,id:98,x:33469,y:32079,varname:node_98,prsc:2,spu:0,spv:1|UVIN-8992-UVOUT,DIST-9560-OUT;n:type:ShaderForge.SFN_Sin,id:8594,x:33031,y:31892,varname:node_8594,prsc:2|IN-9162-OUT;n:type:ShaderForge.SFN_RemapRange,id:9560,x:33204,y:31969,varname:node_9560,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-8594-OUT;n:type:ShaderForge.SFN_Multiply,id:9162,x:32858,y:31825,varname:node_9162,prsc:2|A-4640-T,B-5867-OUT;n:type:ShaderForge.SFN_Slider,id:5867,x:32512,y:31949,ptovrint:False,ptlb:Noise_Movement_Multiplier,ptin:_Noise_Movement_Multiplier,varname:node_5867,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.15,max:2;proporder:5279-9297-2879-48-8911-5849-4317-2480-2857-4909-256-5867;pass:END;sub:END;*/

Shader "Shader Forge/TestShader" {
    Properties {
        _Color1 ("Color1", Color) = (0.5620674,0.6862785,0.8308824,1)
        _Color2 ("Color2", Color) = (0,0,0.3,1)
        _Texture ("Texture", 2D) = "black" {}
        _Cloud_Texture_Darkening ("Cloud_Texture_Darkening", Float ) = 1
        _Noise_Color1 ("Noise_Color1", Color) = (1,1,1,1)
        _Noise_Color2 ("Noise_Color2", Color) = (0,0,0,1)
        _Intensity ("Intensity", Float ) = 1
        _NoiseorClouds ("Noise or Clouds", Range(0, 1)) = 0.5
        _Noise_control ("Noise_control", Float ) = 100
        _Base_Color_Strength ("Base_Color_Strength", Range(0, 1)) = 0.5
        _Base_Color ("Base_Color", Color) = (0.2,0.2,0.2,1)
        _Noise_Movement_Multiplier ("Noise_Movement_Multiplier", Range(0, 2)) = 0.15
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color1;
            uniform float4 _Color2;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Cloud_Texture_Darkening;
            uniform float4 _Noise_Color1;
            uniform float4 _Noise_Color2;
            uniform float _Intensity;
            uniform float _NoiseorClouds;
            uniform float _Noise_control;
            uniform float _Base_Color_Strength;
            uniform float4 _Base_Color;
            uniform float _Noise_Movement_Multiplier;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float3 node_2991 = lerp(_Noise_Color1.rgb,_Noise_Color2.rgb,i.uv0.g);
                float4 node_4640 = _Time + _TimeEditor;
                float node_8541 = (sin(node_4640.b)*0.5+0.5);
                float3 node_392 = lerp((1.0-node_2991),node_2991,node_8541);
                float2 node_5105 = floor((_Noise_control*(i.uv0+(sin((node_4640.g*_Noise_Movement_Multiplier))*0.5+0.5)*float2(0,1))));
                float2 node_4862_skew = node_5105 + 0.2127+node_5105.x*0.3713*node_5105.y;
                float2 node_4862_rnd = 4.789*sin(489.123*(node_4862_skew));
                float node_4862 = frac(node_4862_rnd.x*node_4862_rnd.y*(1+node_4862_skew.x));
                float4 node_7464 = _Time + _TimeEditor;
                float2 node_9825 = (i.uv0+node_7464.g*float2(0,0.1));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9825, _Texture));
                float3 emissive = lerp((lerp(clamp(lerp((node_392/node_4862),(node_392*node_4862),(node_8541*0.3+0.5)),0,1.5),(lerp(_Color1.rgb,_Color2.rgb,_Texture_var.rgb)/_Cloud_Texture_Darkening),_NoiseorClouds)*_Intensity),_Base_Color.rgb,_Base_Color_Strength);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
