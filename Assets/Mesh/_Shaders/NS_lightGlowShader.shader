// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:1,fgcg:0.9168357,fgcb:0.8529412,fgca:1,fgde:0.002,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33039,y:32867,varname:node_3138,prsc:2|emission-9935-OUT;n:type:ShaderForge.SFN_Lerp,id:8679,x:32410,y:32860,varname:node_8679,prsc:2|A-8054-RGB,B-4130-RGB,T-8199-OUT;n:type:ShaderForge.SFN_ViewVector,id:2468,x:31266,y:32189,varname:node_2468,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:8837,x:31266,y:32048,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:3011,x:31438,y:32114,varname:node_3011,prsc:2,dt:0|A-8837-OUT,B-2468-OUT;n:type:ShaderForge.SFN_Add,id:6289,x:31834,y:32229,varname:node_6289,prsc:2|A-2829-OUT,B-6063-OUT;n:type:ShaderForge.SFN_Color,id:4130,x:32195,y:32888,ptovrint:False,ptlb:Color Gradient B,ptin:_ColorGradientB,varname:node_4130,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:8054,x:32195,y:32728,ptovrint:False,ptlb:Glow Color,ptin:_GlowColor,varname:node_8054,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Power,id:9251,x:31672,y:32048,varname:node_9251,prsc:2|VAL-3011-OUT,EXP-7794-OUT;n:type:ShaderForge.SFN_Vector1,id:7794,x:31438,y:32048,varname:node_7794,prsc:2,v1:5;n:type:ShaderForge.SFN_Clamp01,id:4741,x:32582,y:32293,cmnt:Falloff product,varname:node_4741,prsc:2|IN-4936-OUT;n:type:ShaderForge.SFN_Sqrt,id:2829,x:31672,y:32176,varname:node_2829,prsc:2|IN-3011-OUT;n:type:ShaderForge.SFN_Divide,id:4301,x:31996,y:32095,varname:node_4301,prsc:2|A-9251-OUT,B-6289-OUT;n:type:ShaderForge.SFN_Multiply,id:3028,x:32410,y:33053,varname:node_3028,prsc:2|A-8199-OUT,B-7053-OUT;n:type:ShaderForge.SFN_DepthBlend,id:4129,x:32410,y:33202,varname:node_4129,prsc:2|DIST-5472-OUT;n:type:ShaderForge.SFN_Multiply,id:4132,x:32599,y:33075,varname:node_4132,prsc:2|A-3028-OUT,B-4129-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5472,x:32230,y:33236,ptovrint:False,ptlb:db,ptin:_db,varname:node_5472,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:829,x:31955,y:33188,ptovrint:False,ptlb:Glow Intensity,ptin:_GlowIntensity,varname:node_829,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:9935,x:32785,y:32963,varname:node_9935,prsc:2|A-8679-OUT,B-4132-OUT;n:type:ShaderForge.SFN_Get,id:8199,x:32174,y:33031,varname:node_8199,prsc:2|IN-4871-OUT;n:type:ShaderForge.SFN_Set,id:4871,x:33004,y:32299,varname:Faloffproduct,prsc:2|IN-4926-OUT;n:type:ShaderForge.SFN_Tex2d,id:9236,x:31485,y:32883,varname:node_9236,prsc:2,ntxv:0,isnm:False|UVIN-7379-UVOUT,TEX-3839-TEX;n:type:ShaderForge.SFN_Tex2d,id:919,x:31485,y:33035,varname:node_919,prsc:2,ntxv:0,isnm:False|UVIN-9537-UVOUT,TEX-3839-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:3839,x:31217,y:33060,ptovrint:False,ptlb:Dust ,ptin:_Dust,varname:node_3839,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:7379,x:31217,y:32703,varname:node_7379,prsc:2,spu:0,spv:0.2|UVIN-7293-OUT,DIST-3094-OUT;n:type:ShaderForge.SFN_Panner,id:9537,x:31217,y:32878,varname:node_9537,prsc:2,spu:0.1,spv:-0.15|UVIN-1336-OUT,DIST-3094-OUT;n:type:ShaderForge.SFN_TexCoord,id:7823,x:30569,y:32895,varname:node_7823,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:2681,x:30554,y:33157,varname:node_2681,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3094,x:30876,y:33154,varname:node_3094,prsc:2|A-2681-T,B-6019-OUT;n:type:ShaderForge.SFN_Multiply,id:90,x:31939,y:32716,varname:node_90,prsc:2|A-2489-OUT,B-2672-OUT;n:type:ShaderForge.SFN_Add,id:2672,x:31681,y:32930,varname:node_2672,prsc:2|A-9236-R,B-919-R;n:type:ShaderForge.SFN_Add,id:9578,x:32221,y:32321,varname:node_9578,prsc:2|A-4301-OUT,B-90-OUT;n:type:ShaderForge.SFN_Multiply,id:4936,x:32392,y:32293,varname:node_4936,prsc:2|A-4301-OUT,B-9578-OUT;n:type:ShaderForge.SFN_Multiply,id:1336,x:30873,y:32893,varname:node_1336,prsc:2|A-7823-UVOUT,B-2102-OUT;n:type:ShaderForge.SFN_Vector1,id:3635,x:30569,y:32712,varname:node_3635,prsc:2,v1:0.85;n:type:ShaderForge.SFN_Slider,id:2489,x:31568,y:32745,ptovrint:False,ptlb:Dust intensity,ptin:_Dustintensity,varname:node_2489,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Vector1,id:6063,x:31656,y:32337,cmnt:Attenuation value,varname:node_6063,prsc:2,v1:0;n:type:ShaderForge.SFN_ConstantClamp,id:7053,x:32110,y:33075,varname:node_7053,prsc:2,min:0,max:50|IN-829-OUT;n:type:ShaderForge.SFN_Slider,id:6019,x:30397,y:33326,ptovrint:False,ptlb:Wind Intensity,ptin:_WindIntensity,varname:node_6019,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:0.1;n:type:ShaderForge.SFN_ObjectScale,id:9140,x:30716,y:33580,varname:node_9140,prsc:2,rcp:False;n:type:ShaderForge.SFN_Desaturate,id:111,x:30921,y:33580,varname:node_111,prsc:2|COL-9140-XYZ;n:type:ShaderForge.SFN_OneMinus,id:2102,x:31148,y:33580,varname:node_2102,prsc:2|IN-111-OUT;n:type:ShaderForge.SFN_FaceSign,id:9227,x:32392,y:32472,varname:node_9227,prsc:2,fstp:0;n:type:ShaderForge.SFN_Lerp,id:4926,x:32865,y:32435,varname:node_4926,prsc:2|A-4741-OUT,B-9874-OUT,T-9227-VFACE;n:type:ShaderForge.SFN_Vector1,id:9874,x:32648,y:32426,varname:node_9874,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:7293,x:30873,y:32698,varname:node_7293,prsc:2|A-3635-OUT,B-1336-OUT;proporder:4130-8054-5472-829-3839-2489-6019;pass:END;sub:END;*/

Shader "Shader Forge/lightGlowShader" {
    Properties {
        [HideInInspector]_ColorGradientB ("Color Gradient B", Color) = (0.5,0.5,0.5,1)
        _GlowColor ("Glow Color", Color) = (0.5,0.5,0.5,1)
        [HideInInspector]_db ("db", Float ) = 2
        _GlowIntensity ("Glow Intensity", Float ) = 0.5
        _Dust ("Dust ", 2D) = "white" {}
        _Dustintensity ("Dust intensity", Range(0, 1)) = 0.5
        _WindIntensity ("Wind Intensity", Range(0, 0.1)) = 0.1
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _ColorGradientB;
            uniform float4 _GlowColor;
            uniform float _db;
            uniform float _GlowIntensity;
            uniform sampler2D _Dust; uniform float4 _Dust_ST;
            uniform float _Dustintensity;
            uniform float _WindIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float node_3011 = dot(i.normalDir,viewDirection);
                float node_4301 = (pow(node_3011,5.0)/(sqrt(node_3011)+0.0));
                float4 node_2681 = _Time;
                float node_3094 = (node_2681.g*_WindIntensity);
                float node_2102 = (1.0 - dot(objScale,float3(0.3,0.59,0.11)));
                float2 node_1336 = (i.uv0*node_2102);
                float2 node_7379 = ((0.85*node_1336)+node_3094*float2(0,0.2));
                float4 node_9236 = tex2D(_Dust,TRANSFORM_TEX(node_7379, _Dust));
                float2 node_9537 = (node_1336+node_3094*float2(0.1,-0.15));
                float4 node_919 = tex2D(_Dust,TRANSFORM_TEX(node_9537, _Dust));
                float node_90 = (_Dustintensity*(node_9236.r+node_919.r));
                float Faloffproduct = lerp(saturate((node_4301*(node_4301+node_90))),0.0,isFrontFace);
                float node_8199 = Faloffproduct;
                float3 emissive = (lerp(_GlowColor.rgb,_ColorGradientB.rgb,node_8199)*((node_8199*clamp(_GlowIntensity,0,50))*saturate((sceneZ-partZ)/_db)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
