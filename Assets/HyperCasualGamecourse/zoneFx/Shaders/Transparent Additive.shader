// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Transparent Additive"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Glow ("Glow Power", Range(0.0, 5.0)) = 2.0
	}

	SubShader
	{   
        Tags
        {
        	"RenderType"="Transparent" "Queue"="Transparent"
        }
        //cull off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

		CGINCLUDE
 		#define _GLOSSYENV 1
		ENDCG
         
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf NoLighting noambient vertex:vert alpha:fade
		#pragma exclude_renderers gles
		#include "UnityCG.cginc" 

		sampler2D _MainTex;
		fixed4 _Color;
		fixed _Glow;

		struct Input
		{
			float2 uv_MainTex;
		    float4 vertexColor : COLOR;
		};

		 fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
             return fixed4(s.Albedo, s.Alpha);
         }

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.vertexColor * _Color;
			
			o.Albedo = c.rgb  ;
			o.Alpha = c.a * _Glow;
		}
		
	
		void vert (inout appdata_full v)
		{
          
		}

		ENDCG
	}

	Fallback "Standard"
}