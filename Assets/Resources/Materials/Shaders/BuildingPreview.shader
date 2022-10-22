// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32985,y:32530,varname:node_3138,prsc:2|emission-2250-OUT,alpha-3502-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32081,y:32798,ptovrint:False,ptlb:FresnelColor,ptin:_FresnelColor,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7221,x:32081,y:32988,ptovrint:False,ptlb:FresnelExp,ptin:_FresnelExp,varname:node_7221,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;n:type:ShaderForge.SFN_Color,id:9466,x:32081,y:32619,ptovrint:False,ptlb:MainColor,ptin:_MainColor,varname:node_9466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Fresnel,id:6212,x:32276,y:32897,varname:node_6212,prsc:2|EXP-7221-OUT;n:type:ShaderForge.SFN_Multiply,id:2534,x:32540,y:32823,varname:node_2534,prsc:2|A-7241-RGB,B-6212-OUT;n:type:ShaderForge.SFN_Add,id:2250,x:32745,y:32676,varname:node_2250,prsc:2|A-9466-RGB,B-2534-OUT;n:type:ShaderForge.SFN_Slider,id:3502,x:32081,y:33115,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_3502,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4,max:1;proporder:9466-7241-7221-3502;pass:END;sub:END;*/

Shader "Shader Forge/BuildingPreviewShader" {
    Properties {
        _MainColor ("MainColor", Color) = (0,1,0,1)
        _FresnelColor ("FresnelColor", Color) = (1,1,1,1)
        _FresnelExp ("FresnelExp", Float ) = 1.5
        _Opacity ("Opacity", Range(0, 1)) = 0.4
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
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _FresnelColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _FresnelExp)
                UNITY_DEFINE_INSTANCED_PROP( float4, _MainColor)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _MainColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _MainColor );
                float4 _FresnelColor_var = UNITY_ACCESS_INSTANCED_PROP( Props, _FresnelColor );
                float _FresnelExp_var = UNITY_ACCESS_INSTANCED_PROP( Props, _FresnelExp );
                float3 emissive = (_MainColor_var.rgb+(_FresnelColor_var.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelExp_var)));
                float3 finalColor = emissive;
                float _Opacity_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Opacity );
                return fixed4(finalColor,_Opacity_var);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
