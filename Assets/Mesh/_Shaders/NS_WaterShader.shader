// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:0,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:True,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33498,y:32783,varname:node_2865,prsc:2|diff-472-OUT,spec-358-OUT,gloss-1813-OUT,normal-1113-OUT;n:type:ShaderForge.SFN_Color,id:6665,x:32207,y:31989,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4779412,c2:0.7839757,c3:1,c4:1;n:type:ShaderForge.SFN_Slider,id:358,x:32250,y:32780,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_358,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:32250,y:32882,ptovrint:False,ptlb:Roughness,ptin:_Roughness,varname:_Metallic_copy,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Tex2d,id:8003,x:31945,y:33023,varname:node_8003,prsc:2,tex:3df345f26d374174f8b90a9e24533000,ntxv:0,isnm:False|UVIN-186-UVOUT,TEX-3536-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:3536,x:31515,y:33105,ptovrint:False,ptlb:node_3536,ptin:_node_3536,varname:node_3536,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3df345f26d374174f8b90a9e24533000,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:4381,x:31945,y:33143,varname:node_4381,prsc:2,tex:3df345f26d374174f8b90a9e24533000,ntxv:0,isnm:False|UVIN-3298-UVOUT,TEX-3536-TEX;n:type:ShaderForge.SFN_Panner,id:3298,x:31754,y:33276,varname:node_3298,prsc:2,spu:-0.15,spv:-0.08|UVIN-9342-OUT,DIST-9212-OUT;n:type:ShaderForge.SFN_Panner,id:186,x:31739,y:32864,varname:node_186,prsc:2,spu:0.1,spv:0.15|UVIN-4774-OUT;n:type:ShaderForge.SFN_NormalBlend,id:2506,x:32138,y:33072,varname:node_2506,prsc:2|BSE-8003-RGB,DTL-4381-RGB;n:type:ShaderForge.SFN_Time,id:9001,x:31263,y:33274,varname:node_9001,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:1023,x:31239,y:32726,varname:node_1023,prsc:2;n:type:ShaderForge.SFN_Append,id:9342,x:31508,y:32774,varname:node_9342,prsc:2|A-1023-X,B-1023-Z;n:type:ShaderForge.SFN_Multiply,id:4774,x:31508,y:32915,varname:node_4774,prsc:2|A-9342-OUT,B-8306-OUT;n:type:ShaderForge.SFN_Vector1,id:8306,x:31294,y:32951,varname:node_8306,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Lerp,id:2404,x:32711,y:32355,varname:node_2404,prsc:2|A-6665-RGB,B-8076-OUT,T-5702-OUT;n:type:ShaderForge.SFN_Fresnel,id:7816,x:32165,y:32438,varname:node_7816,prsc:2|EXP-2550-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2550,x:31986,y:32461,ptovrint:False,ptlb:Horizon Reflection,ptin:_HorizonReflection,varname:node_2550,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:12;n:type:ShaderForge.SFN_ConstantClamp,id:5702,x:32498,y:32460,varname:node_5702,prsc:2,min:0,max:1|IN-1021-OUT;n:type:ShaderForge.SFN_Multiply,id:1021,x:32334,y:32460,varname:node_1021,prsc:2|A-7816-OUT,B-220-OUT;n:type:ShaderForge.SFN_Vector1,id:220,x:32165,y:32568,varname:node_220,prsc:2,v1:15;n:type:ShaderForge.SFN_Multiply,id:9212,x:31515,y:33315,varname:node_9212,prsc:2|A-9001-T,B-6886-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6886,x:31349,y:33422,ptovrint:False,ptlb:Animation Speed,ptin:_AnimationSpeed,varname:node_6886,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Desaturate,id:3581,x:32172,y:33342,varname:node_3581,prsc:2|COL-2506-OUT;n:type:ShaderForge.SFN_Add,id:472,x:33267,y:33123,varname:node_472,prsc:2|A-2404-OUT,B-6181-OUT,C-7936-OUT;n:type:ShaderForge.SFN_Clamp01,id:3824,x:32535,y:33356,varname:node_3824,prsc:2|IN-4002-OUT;n:type:ShaderForge.SFN_RemapRange,id:4002,x:32361,y:33356,varname:node_4002,prsc:2,frmn:0,frmx:1,tomn:-21.1,tomx:150|IN-3581-OUT;n:type:ShaderForge.SFN_Multiply,id:7936,x:33198,y:33358,varname:node_7936,prsc:2|A-3824-OUT,B-8202-OUT;n:type:ShaderForge.SFN_Set,id:4914,x:33014,y:32159,varname:Foam_Color,prsc:2|IN-4896-OUT;n:type:ShaderForge.SFN_Get,id:8202,x:32951,y:33499,varname:node_8202,prsc:2|IN-4914-OUT;n:type:ShaderForge.SFN_Clamp01,id:6181,x:32730,y:33152,varname:node_6181,prsc:2|IN-3380-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:3764,x:32253,y:33735,varname:node_3764,prsc:2|IN-3581-OUT,IMIN-2348-OUT,IMAX-692-OUT,OMIN-1146-OUT,OMAX-5462-OUT;n:type:ShaderForge.SFN_Vector1,id:2348,x:32021,y:33721,varname:node_2348,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:692,x:32021,y:33777,varname:node_692,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:1146,x:32021,y:33859,ptovrint:False,ptlb:min,ptin:_min,varname:node_1146,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:134;n:type:ShaderForge.SFN_ValueProperty,id:5462,x:32021,y:33942,ptovrint:False,ptlb:max,ptin:_max,varname:node_5462,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-10.2;n:type:ShaderForge.SFN_OneMinus,id:3380,x:32423,y:33735,varname:node_3380,prsc:2|IN-3764-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6639,x:32638,y:32162,ptovrint:False,ptlb:node_6639,ptin:_node_6639,varname:node_6639,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Abs,id:4896,x:32825,y:32162,varname:node_4896,prsc:2|IN-6639-OUT;n:type:ShaderForge.SFN_AmbientLight,id:2340,x:32207,y:32247,varname:node_2340,prsc:2;n:type:ShaderForge.SFN_Blend,id:8076,x:32397,y:32284,varname:node_8076,prsc:2,blmd:10,clmp:True|SRC-6665-RGB,DST-2340-RGB;n:type:ShaderForge.SFN_Lerp,id:1113,x:32390,y:33044,varname:node_1113,prsc:2|A-8580-OUT,B-2506-OUT,T-3745-OUT;n:type:ShaderForge.SFN_Vector3,id:8580,x:32138,y:32991,varname:node_8580,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_ValueProperty,id:3745,x:32138,y:33218,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_3745,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:6665-358-1813-3536-2550-6886-1146-5462-6639-3745;pass:END;sub:END;*/

Shader "Shader Forge/WaterShader" {
    Properties {
        _Color ("Color", Color) = (0.4779412,0.7839757,1,1)
        [HideInInspector]_Metallic ("Metallic", Range(0, 1)) = 0
        [HideInInspector]_Roughness ("Roughness", Range(0, 1)) = 0.8
        _node_3536 ("node_3536", 2D) = "bump" {}
        _HorizonReflection ("Horizon Reflection", Float ) = 12
        _AnimationSpeed ("Animation Speed", Float ) = 5
        [HideInInspector]_min ("min", Float ) = 134
        [HideInInspector]_max ("max", Float ) = -10.2
        [HideInInspector]_node_6639 ("node_6639", Float ) = 0.1
        _NormalIntensity ("Normal Intensity", Float ) = 1
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
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Metallic;
            uniform float _Roughness;
            uniform sampler2D _node_3536; uniform float4 _node_3536_ST;
            uniform float _HorizonReflection;
            uniform float _AnimationSpeed;
            uniform float _min;
            uniform float _max;
            uniform float _node_6639;
            uniform float _NormalIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9424 = _Time;
                float2 node_9342 = float2(i.posWorld.r,i.posWorld.b);
                float2 node_186 = ((node_9342*0.8)+node_9424.g*float2(0.1,0.15));
                float3 node_8003 = UnpackNormal(tex2D(_node_3536,TRANSFORM_TEX(node_186, _node_3536)));
                float4 node_9001 = _Time;
                float2 node_3298 = (node_9342+(node_9001.g*_AnimationSpeed)*float2(-0.15,-0.08));
                float3 node_4381 = UnpackNormal(tex2D(_node_3536,TRANSFORM_TEX(node_3298, _node_3536)));
                float3 node_2506_nrm_base = node_8003.rgb + float3(0,0,1);
                float3 node_2506_nrm_detail = node_4381.rgb * float3(-1,-1,1);
                float3 node_2506_nrm_combined = node_2506_nrm_base*dot(node_2506_nrm_base, node_2506_nrm_detail)/node_2506_nrm_base.z - node_2506_nrm_detail;
                float3 node_2506 = node_2506_nrm_combined;
                float3 normalLocal = lerp(float3(0,0,1),node_2506,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _Roughness; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Metallic,_Metallic,_Metallic);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*normTerm*specularColor;
                float3 indirectSpecular = (gi.indirect.specular)*specularColor;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_7816 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_HorizonReflection);
                float node_3581 = dot(node_2506,float3(0.3,0.59,0.11));
                float node_2348 = 1.0;
                float node_3824 = saturate((node_3581*171.1+-21.1));
                float Foam_Color = abs(_node_6639);
                float3 diffuseColor = (lerp(_Color.rgb,saturate(( UNITY_LIGHTMODEL_AMBIENT.rgb > 0.5 ? (1.0-(1.0-2.0*(UNITY_LIGHTMODEL_AMBIENT.rgb-0.5))*(1.0-_Color.rgb)) : (2.0*UNITY_LIGHTMODEL_AMBIENT.rgb*_Color.rgb) )),clamp((node_7816*15.0),0,1))+saturate((1.0 - (_min + ( (node_3581 - node_2348) * (_max - _min) ) / (0.0 - node_2348))))+(node_3824*Foam_Color));
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Metallic;
            uniform float _Roughness;
            uniform sampler2D _node_3536; uniform float4 _node_3536_ST;
            uniform float _HorizonReflection;
            uniform float _AnimationSpeed;
            uniform float _min;
            uniform float _max;
            uniform float _node_6639;
            uniform float _NormalIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9785 = _Time;
                float2 node_9342 = float2(i.posWorld.r,i.posWorld.b);
                float2 node_186 = ((node_9342*0.8)+node_9785.g*float2(0.1,0.15));
                float3 node_8003 = UnpackNormal(tex2D(_node_3536,TRANSFORM_TEX(node_186, _node_3536)));
                float4 node_9001 = _Time;
                float2 node_3298 = (node_9342+(node_9001.g*_AnimationSpeed)*float2(-0.15,-0.08));
                float3 node_4381 = UnpackNormal(tex2D(_node_3536,TRANSFORM_TEX(node_3298, _node_3536)));
                float3 node_2506_nrm_base = node_8003.rgb + float3(0,0,1);
                float3 node_2506_nrm_detail = node_4381.rgb * float3(-1,-1,1);
                float3 node_2506_nrm_combined = node_2506_nrm_base*dot(node_2506_nrm_base, node_2506_nrm_detail)/node_2506_nrm_base.z - node_2506_nrm_detail;
                float3 node_2506 = node_2506_nrm_combined;
                float3 normalLocal = lerp(float3(0,0,1),node_2506,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _Roughness; // Convert roughness to gloss
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Metallic,_Metallic,_Metallic);
                float specularMonochrome = max( max(specularColor.r, specularColor.g), specularColor.b);
                float normTerm = (specPow + 8.0 ) / (8.0 * Pi);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*normTerm*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float node_7816 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_HorizonReflection);
                float node_3581 = dot(node_2506,float3(0.3,0.59,0.11));
                float node_2348 = 1.0;
                float node_3824 = saturate((node_3581*171.1+-21.1));
                float Foam_Color = abs(_node_6639);
                float3 diffuseColor = (lerp(_Color.rgb,saturate(( UNITY_LIGHTMODEL_AMBIENT.rgb > 0.5 ? (1.0-(1.0-2.0*(UNITY_LIGHTMODEL_AMBIENT.rgb-0.5))*(1.0-_Color.rgb)) : (2.0*UNITY_LIGHTMODEL_AMBIENT.rgb*_Color.rgb) )),clamp((node_7816*15.0),0,1))+saturate((1.0 - (_min + ( (node_3581 - node_2348) * (_max - _min) ) / (0.0 - node_2348))))+(node_3824*Foam_Color));
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
