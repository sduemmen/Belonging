Shader "Custom/InteractiveSnow" {
	Properties{
		[Header(Main)]	
		_Noise("Snow Noise", 2D) = "gray" {}	
		_NoiseScale("Noise Scale", Range(0,0.1)) = 0.01
		_NoiseWeight("Noise Weight", Range(0,4)) = 0.1
		[HDR]_ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1)
		[Space]
		[Header(Tesselation)]
		_MaxTessDistance("Max Tessellation Distance", Range(10,100)) = 50
		_Tess("Tessellation", Range(1,64)) = 64
		[Space]
		[Header(Snow)]
		[HDR]_Color("Snow Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Snow Texture", 2D) = "white" {}		
		_SnowHeight("Snow Height", Range(-2,2)) = 0.3
		_SnowTextureOpacity("Snow Texture Opacity", Range(0,1)) = 0.3
		_SnowTextureScale("Snow Texture Scale", Range(0,2)) = 0.3

		[Space]
		[Header(Snow Path)]
		_PathBlending("Path Color Blending", Range(0,3)) = 2
		_SnowPathStrength("Snow Path Smoothness", Range(0,4)) = 2
		[HDR]_PathColorIn("Snow Path Color", Color) = (1,1,1,1)
		[HDR]_PathColorOut("Snow Path Color2", Color) = (0.5,0.5,1,1)
		
		[Space]
		[Header(Sparkles)]
		_SparkleScale("Sparkle Scale", Range(0, 5)) = 10
		_SparkleIntensity("Sparkle Intensity", Float) = 0.9
		_SparkleNoise("Sparkle Noise", 2D) = "gray" {}
		[Space]
		[Header(Rim)]
		_RimPower("Rim Power", Range(0,20)) = 20
		[HDR]_RimColor("Rim Color Snow", Color) = (0.5,0.5,0.5,1)
		
		_Min("Min", Range(-1, 2)) = 0
		_Max("Max", Range(-1, 2)) = 0
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		CGPROGRAM
		
		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
		#pragma surface surf ToonRamp vertex:vert addshadow nolightmap tessellate:tessDistance fullforwardshadows
		#pragma target 4.0
		#pragma require tessellation tessHW
		#include "Tessellation.cginc"

		float4 _ShadowColor;

		inline half4 LightingToonRamp(inout SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
			#endif
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (atten + (_ShadowColor * (1-atten)));
			s.Emission = s.Emission * c.rgb * 100 * (atten + (_ShadowColor * (1-atten)));
			c.a = 0;
			return c;
		}

		uniform float3 _InverseSunDirection;
		uniform float3 _InverseMoonDirection;
		uniform float3 _Position;
		uniform sampler2D _GlobalEffectRT;
		uniform float _OrthographicCamSize;

		float _Tess;
		float _MaxTessDistance;

		float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
		{
			float3 worldPosition = mul(unity_ObjectToWorld, vertex).xyz;
			float dist = distance(worldPosition, _WorldSpaceCameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);
			return f * tess;
		}

		float4 DistanceBasedTess(float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess)
		{
			float3 f;
			f.x = CalcDistanceTessFactor(v0, minDist, maxDist, tess);
			f.y = CalcDistanceTessFactor(v1, minDist, maxDist, tess);
			f.z = CalcDistanceTessFactor(v2, minDist, maxDist, tess);

			return UnityCalcTriEdgeTessFactors(f);
		}

		float4 tessDistance(appdata_full v0, appdata_full v1, appdata_full v2)
		{
			float minDist = 10.0;
			float maxDist = _MaxTessDistance;

			return DistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
		}

		sampler2D _MainTex, _Noise, _SparkleNoise;
		float4 _Color, _RimColor;
		float _RimPower;
		float _SnowTextureScale, _NoiseScale;
		float _SnowHeight, _SnowPathStrength;
		float4 _PathColorIn, _PathColorOut;
		float _PathBlending;
		float _NoiseWeight;
		float _SparkleScale, _SparkleIntensity;
		float _SnowTextureOpacity;
		float _Min, _Max;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			float3 worldPos; // world position built-in value
			float3 viewDir;// view direction built-in value we're using for rimlight
			float3 worldNormal;
			float4 screenPos;
		};

		void vert(inout appdata_full v)
		{	
			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
			// Effects RenderTexture Reading
			float2 uv = worldPosition.xz - _Position.xz;
			uv = uv / (_OrthographicCamSize * 2);
			uv += 0.5;			
			float4 RTEffect = tex2Dlod(_GlobalEffectRT, float4(uv, 0, 0));
			
			// smoothstep edges to prevent bleeding
			RTEffect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
			RTEffect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);
			
			// Snow Noise in worldSpace
			float SnowNoise = tex2Dlod(_Noise, float4(worldPosition.xz * _NoiseScale, 0, 0));
			
			// move vertices up where snow is, and where there is no path	
			v.vertex.xyz += normalize(v.normal) *(_SnowHeight + (SnowNoise * _NoiseWeight)) * saturate(1-RTEffect.g * _SnowPathStrength);
		}
		
		void surf(Input IN, inout SurfaceOutput o) {
			// Effects RenderTexture Reading
			float2 uv = IN.worldPos.xz - _Position.xz;
			uv /= (_OrthographicCamSize * 2);
			uv += 0.5;

			float4 effect = tex2D(_GlobalEffectRT, float2 (uv.x, uv.y));
			effect *=  smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
			effect *=  smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);

			// worldspace Snow texture
			float3 snowtexture = tex2D(_MainTex, IN.worldPos.zx * _SnowTextureScale);

			//lerp between snow color and snow texture
			float3 mainColors = lerp(_Color,snowtexture * _Color, _SnowTextureOpacity);
			//lerp the colors using the RT effect path 
			float3 path = lerp(_PathColorOut * effect.g,_PathColorIn, saturate(effect.g * _PathBlending));
			o.Albedo = lerp(mainColors,path, saturate(effect.g));

			// scale 0.05, intensity 1000
			// float viewDistance = length(_WorldSpaceCameraPos - IN.worldPos);
			// float3 viewDirection = normalize(float3(viewDistance, 0, viewDistance));
			// viewDirection = abs(viewDirection);
			
			fixed3 sparkleNoiseSample = tex2D(_SparkleNoise, IN.worldPos.xz * _SparkleScale).rgb;
			fixed3 sparkleNoise = normalize(sparkleNoiseSample - float3(0.5, 0.5, 0.5));
			
			half3 sparkleNormal = normalize(sparkleNoise + IN.worldNormal);
			half sparkle = saturate(dot(abs(IN.viewDir), sparkleNormal));
			sparkle = pow(sparkle, _SparkleIntensity);

			o.Emission = sparkle;
		}
		ENDCG

	}

	Fallback "Diffuse"
}

//Shader "Custom/InteractiveSnow" {
//	Properties{
//		[Header(Displacement)]	
//		_SnowNoise("Displacement Noise", 2D) = "gray" {}	
//		_SnowNoiseScale("Noise Scale", Range(0,1)) = 0.001
//		_SnowNoiseWeight("Noise Weight", Range(0,4)) = 0.1
//		
//		[Space]
//		[Header(Snow)]
//		[HDR]_SnowColor("Snow Color", Color) = (0.5,0.5,0.5,1)
//		_MainTex("Snow Texture", 2D) = "white" {}		
//		_SnowHeight("Snow Height", Range(-2,2)) = 0.3
//		_SnowTextureOpacity("Snow Texture Opacity", Range(0,1)) = 0.3
//		_SnowTextureScale("Snow Texture Scale", Range(0,2)) = 0.3
//		[HDR]_ShadowColor("Shadow Color", Color) = (0.5,0.5,0.5,1)
//		
//		[Space]
//		[Header(Tesselation)]
//		_MaxTessDistance("Max Tessellation Distance", Range(10,100)) = 50
//		_Tess("Tessellation", Range(1,64)) = 64
//
//		[Space]
//		[Header(Snow Path)]
//		_PathBlending("Path Color Blending", Range(0,3)) = 2
//		_SnowPathStrength("Snow Path Smoothness", Range(0,4)) = 2
//		[HDR]_PathColorIn("Snow Path Color", Color) = (1,1,1,1)
//		[HDR]_PathColorOut("Snow Path Color2", Color) = (0.5,0.5,1,1)
//		
//		[Space]
//		[Header(Sparkles)]
//		_SparkleScale("Sparkle Scale", Range(0,5)) = 1
//		_SparkIntensity("Sparkle Intensity", Range(0,200)) = 50
//		_SparkleNoise("Sparkle Noise", 2D) = "gray" {}
////		[Space]
////		[Header(Rim)]
////		_RimPower("Rim Power", Range(0,20)) = 20
////		[HDR]_RimColor("Rim Color Snow", Color) = (0.5,0.5,0.5,1)
//	}
//
//	SubShader{
//		Tags{
//			"RenderType"="Opaque"
//		}
//		CGPROGRAM
//		
//		// custom lighting function that uses a texture ramp based
//		// on angle between light direction and normal
//		#pragma surface surf ToonRamp vertex:vert addshadow nolightmap tessellate:tessDistance fullforwardshadows
//		#pragma target 4.0
//		#pragma require tessellation tessHW 
//		
//		#include "Tessellation.cginc"
//
//		float4 _ShadowColor;
//		
//		inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
//		{
//			#ifndef USING_DIRECTIONAL_LIGHT
//				lightDir = normalize(lightDir);
//			#endif
//			half4 c;
//			c.rgb = s.Albedo * _LightColor0.rgb * (atten + (_ShadowColor * (1-atten)));
//			c.a = 0;
//			return c;
//		}
//		
//		uniform float3 _Position;
//		uniform sampler2D _GlobalEffectRT;
//		uniform float _OrthographicCamSize;
//
//		float _Tess;
//		float _MaxTessDistance;
//
//		float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
//		{
//			float3 worldPosition = mul(unity_ObjectToWorld, vertex).xyz;
//			float dist = distance(worldPosition, _WorldSpaceCameraPos);
//			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);
//			return f * tess;
//		}
//
//		float4 DistanceBasedTess(float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess)
//		{
//			float3 f;
//			f.x = CalcDistanceTessFactor(v0, minDist, maxDist, tess);
//			f.y = CalcDistanceTessFactor(v1, minDist, maxDist, tess);
//			f.z = CalcDistanceTessFactor(v2, minDist, maxDist, tess);
//
//			return UnityCalcTriEdgeTessFactors(f);
//		}
//
//		float4 tessDistance(appdata_full v0, appdata_full v1, appdata_full v2)
//		{
//			float minDist = 10.0;
//			float maxDist = _MaxTessDistance;
//
//			return DistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
//		}
//        
//        sampler2D _SnowNoise;
//        float _SnowNoiseScale;
//        float _SnowNoiseWeight;
//        
//		float4 _SnowColor;
//		
//		sampler2D _MainTex;
//		float _SnowTextureScale;
//        
//		float _SnowHeight;
//        float _SnowPathStrength;
//        
//        sampler2D _SparkleNoise;
//		float _SparkleScale;
//        float _SparkleIntensity;
//        
//        float _SnowTextureOpacity;
//        // float4 _RimColor;
//		// float _RimPower;
//		float4 _PathColorIn, _PathColorOut;
//		float _PathBlending;
//
//		struct Input {
//			float2 uv_MainTex;
//			float2 uv_Normal;
//			float4 vertex : SV_POSITION;
//			float3 worldPos;
//			float3 viewDir;
//			// float2 uv : TEXCOORD0;
//			// float3 wPos : TEXCOORD1; //passes world position into frag shader
//   //          float3 wNormal : TEXCOORD2; //passes world normal into frag shader
//		};
//
//		void vert(inout appdata_full v)
//		{
//			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
//			// Effects RenderTexture Reading
//			float2 uv = worldPosition.xz - _Position.xz;
//			uv = uv / (_OrthographicCamSize * 2);
//			uv += 0.5;
//			float4 RTEffect = tex2Dlod(_GlobalEffectRT, float4(uv, 0, 0));
//			// smoothstep edges to prevent bleeding
//			RTEffect *= smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
//			RTEffect *= smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);
//			// Snow Noise in worldSpace
//			float SnowNoise = tex2Dlod(_SnowNoise, float4(worldPosition.xz * _SnowNoiseScale, 0, 0));
//			// move vertices up where snow is, and where there is no path
//			v.vertex.xyz += normalize(v.normal) *(_SnowHeight + (SnowNoise * _SnowNoiseWeight)) * saturate(1-RTEffect.g * _SnowPathStrength);
//
//			// set fragment input
//			// o.vertex = UnityObjectToClipPos(v.vertex);
//			// o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
//			// o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//			// o.wNormal = UnityObjectToWorldNormal(v.normal);
//			// return o;
//		}
//        
//		void surf(Input i, inout SurfaceOutput o) {
//			// Effects RenderTexture Reading
//			float2 uv = i.worldPos.xz; // - _Position.xz;
//			// uv /= _OrthographicCamSize * 2;
//			// uv += 0.5;
//		
//			float4 effect = tex2D(_GlobalEffectRT, float2 (uv.x, uv.y));
//			effect *= smoothstep(0.99, 0.9, uv.x) * smoothstep(0.99, 0.9,1- uv.x);
//			effect *= smoothstep(0.99, 0.9, uv.y) * smoothstep(0.99, 0.9,1- uv.y);
//			
//			// worldspace Noise texture
//			//float3 noisetexture = tex2D(_SnowNoise, IN.worldPos.zx * _SnowNoiseScale);
//		
//			// worldspace Snow texture
//			float3 snowtexture = tex2D(_MainTex, i.worldPos.zx * _SnowTextureScale);
//		
//			// rim light for snow, blending in the noise texture
//			// half rim = 1.0 - dot(normalize(IN.viewDir), o.Normal);// * noisetexture;
//			// float3 coloredRim =  _RimColor * pow(rim, _RimPower);
//		
//			//lerp between snow color and snow texture
//			float3 mainColors = lerp(_SnowColor,snowtexture * _SnowColor, _SnowTextureOpacity);
//			//lerp the colors using the RT effect path
//			float3 path = lerp(_PathColorOut * effect.g,_PathColorIn, saturate(effect.g * _PathBlending));
//			o.Albedo = lerp(mainColors,path, saturate(effect.g));
//		
//			// sparkles in worldspace
//			// float sparklesStatic = tex2D(_SparkleNoise, IN.worldPos.xz * _SparkleScale);
//			// cutoff and where there is no path
//			// float cutoffSparkles = step(_SparkCutoff, sparklesStatic)* (1 - saturate(effect.g));
//			// cutoffSparkles = saturate(dot(GetWorldSpaceNormalizeViewDir(IN.worldPos.xyz), cutoffSparkles));
//			// add a glow and sparkles on the snow
//			// o.Emission = coloredRim + cutoffSparkles * 4;
//			// o.Emission = cutoffSparkles * 4;
//
//			fixed4 col = tex2D(_MainTex, i.uv_MainTex);
//			half3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos); //get the view direction
//        	fixed3 sparklemap = tex2D(_SparkleNoise, uv*_SparkleScale); //sample the noise texture
//            sparklemap -= half3(0.5,0.5,0.5); //change the noise texture into a random direction
//            sparklemap = normalize(sparklemap); 
//            half sparkle = pow(saturate((dot(-viewDirection, normalize(sparklemap + i.uv_MainTex)))),_SparkleIntensity); //get a value based on how close you are to looking at the normal offset by a random direction 
//
//        	// o.Emission = sparkle;
//			o.Albedo = viewDirection;
//		}
//		ENDCG
//	}
//	Fallback "Diffuse"
//}